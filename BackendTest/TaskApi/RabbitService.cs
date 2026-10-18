using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace TaskApi;

public interface IRabbitService
{
    Task InitializeAsync(CancellationToken ct = default);
    Task PublishTaskCompletedEventAsync(TaskItem task, CancellationToken ct = default);
    Task ShutdownAsync(CancellationToken ct = default);
}

public class RabbitService(ConnectionFactory factory) : IRabbitService, IAsyncDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;
    
    private readonly ConnectionFactory _factory = factory;
    private bool _disposed;

    private const string ExchangeName = "task.events";
    public const string QueueName = "tasks.completed.queue";
    private const string RoutingKey = "task.completed";

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        if (_connection is not null) return;

        _connection = await _factory.CreateConnectionAsync(ct);
        _channel = await _connection.CreateChannelAsync(cancellationToken: ct);
        await _channel.ExchangeDeclareAsync(exchange: ExchangeName, type: "direct", durable: true, autoDelete: false, cancellationToken: ct);
        await _channel.QueueDeclareAsync(queue: QueueName, durable: true, exclusive: false, autoDelete: false, cancellationToken: ct);
        await _channel.QueueBindAsync(queue: QueueName, exchange: ExchangeName, routingKey: RoutingKey, cancellationToken: ct);
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

        await _channel.BasicPublishAsync(
            exchange: ExchangeName,
            routingKey: RoutingKey,
            mandatory: false,
            body: body.AsMemory(),
            cancellationToken: ct);
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