using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace TaskApi;

public interface IRabbitService
{
    Task InitializeAsync(CancellationToken ct = default);
    Task PublishTaskCompletedEventAsync(TaskItem task, CancellationToken ct = default);
    Task ShutdownAsync(CancellationToken ct = default);
}

public class RabbitService(ConnectionFactory factory, ILogger<RabbitService> logger) : IRabbitService, IAsyncDisposable
{
    private readonly ConnectionFactory _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly ILogger<RabbitService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private bool _disposed;

    private const string ExchangeName = "task.events";
    public const string QueueName = "tasks.completed.queue";
    private const string RoutingKey = "task.completed";

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        if (_connection is not null) return;

        try
        {
            _connection = await _factory.CreateConnectionAsync(ct);
            _channel = await _connection.CreateChannelAsync(cancellationToken: ct);
            await _channel.ExchangeDeclareAsync(exchange: ExchangeName, type: "direct", durable: true, autoDelete: false, cancellationToken: ct);
            await _channel.QueueDeclareAsync(queue: QueueName, durable: true, exclusive: false, autoDelete: false, cancellationToken: ct);
            await _channel.QueueBindAsync(queue: QueueName, exchange: ExchangeName, routingKey: RoutingKey, cancellationToken: ct);
        }
        catch (BrokerUnreachableException ex)
        {
            _logger.LogWarning(ex, "RabbitMQ недоступен при инициализации. Публикация событий будет отключена.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при инициализации RabbitMQ.");
            throw;
        }
    }

    // не используем пул соединений и не делаем retry
    public async Task PublishTaskCompletedEventAsync(TaskItem task, CancellationToken ct)
    {
        if (_channel is null)
            throw new InvalidOperationException("Сервис не инициализирован. Вызовите InitializeAsync.");

        var payload = new
        {
            TaskId = task.Id.ToString(),
            task.Title,
            CompletedAt = DateTime.UtcNow
        };

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload));

        try
        {
            await _channel.BasicPublishAsync(
                exchange: ExchangeName,
                routingKey: RoutingKey,
                mandatory: false,
                body: body.AsMemory(),
                cancellationToken: ct);
        }
        catch (OperationInterruptedException ex)
        {
            var reason = ex.ShutdownReason;
            if (reason is null)
                _logger.LogWarning(ex, "AMQP операция прервана, причина не указана. TaskId={TaskId}", task.Id);
            else           
                _logger.LogError(ex, "AMQP ошибка при публикации. ReplyCode={ReplyCode}, Text={Text}, ClassId={ClassId}. TaskId={TaskId}",
                reason.ReplyCode, reason.ReplyText, reason.ClassId, task.Id);
        }
        catch (BrokerUnreachableException ex)
        {
            _logger.LogWarning(ex, "RabbitMQ недоступен. Сообщение не отправлено. TaskId={TaskId}, Title={Title}", task.Id, task.Title);
        }
        catch (IOException ex)
        {
            _logger.LogWarning(ex, "Ошибка сети при публикации события. TaskId={TaskId}", task.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Неожиданная ошибка при публикации события. TaskId={TaskId}", task.Id);
        }
    }

    public async Task ShutdownAsync(CancellationToken ct)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(TimeSpan.FromSeconds(5));

        try
        {
            if (_channel is not null)
                await _channel.CloseAsync(0, "Shutdown");
            if (_connection is not null)
                await _connection.CloseAsync();
        }
        catch { }
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            _disposed = true;
            if (_channel is not null)
                await _channel.DisposeAsync();
            if (_connection is not null)
                await _connection.DisposeAsync();
        }
    }
}