using Microsoft.AspNetCore.Mvc.Testing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TaskApi;
using Xunit;

namespace TaskApiTest;

public class CompleteTaskIntegrationTest(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
	private readonly HttpClient _client = factory.CreateClient();

    [Fact]
	public async Task CompleteTask_PublishesRabbitMqEvent()
	{
		var createPayload = "{\"title\": \"Test\"}";
		var createResponse = await _client.PostAsync("/tasks", new StringContent(createPayload, Encoding.UTF8, "application/json"));
		createResponse.EnsureSuccessStatusCode();

		var content = await createResponse.Content.ReadAsStringAsync();
		var doc = JsonDocument.Parse(content);
		var id = Guid.Parse(doc.RootElement.GetProperty("id").GetString());

		var completeResponse = await _client.PutAsync($"/tasks/{id}/complete", null);
		completeResponse.EnsureSuccessStatusCode();

		var receivedMessage = await WaitForRabbitMessage(id, timeoutSeconds: 10);
		Assert.NotNull(receivedMessage);

		Assert.Equal(id.ToString(), receivedMessage.TaskId);
		Assert.Equal("Test", receivedMessage.Title);
		Assert.NotNull(receivedMessage.CompletedAt);
	}

	private static async Task<RabbitMessage> WaitForRabbitMessage(Guid taskId, int timeoutSeconds)
	{
		var connectionFactory = new ConnectionFactory
		{
			HostName = "localhost",
			Port = 5672,
			UserName = "guest",
			Password = "guest"
		};

		using var connection = await connectionFactory.CreateConnectionAsync();
		using var channel = await connection.CreateChannelAsync();
		await channel.QueueDeclarePassiveAsync(RabbitService.QueueName);

		var tcs = new TaskCompletionSource<RabbitMessage>();
		var consumer = new AsyncEventingBasicConsumer(channel);
		consumer.ReceivedAsync += async (sender, ea) =>
		{
			var body = ea.Body.ToArray();
			var json = Encoding.UTF8.GetString(body);
			try
			{
				var msg = JsonSerializer.Deserialize<RabbitMessage>(json);
				if (msg is not null && Guid.TryParse(msg.TaskId, out var parsedId) && parsedId == taskId)
					tcs.TrySetResult(msg);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[Error] {ex.Message}");
			}

			await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
		};

		string consumerTag = await channel.BasicConsumeAsync(queue: RabbitService.QueueName, autoAck: false, consumer: consumer);
		await channel.BasicConsumeAsync(queue: RabbitService.QueueName, autoAck: false, consumer: consumer);

		if (await Task.WhenAny(tcs.Task, Task.Delay(timeoutSeconds * 1000)) == tcs.Task)
		{
			await channel.BasicCancelAsync(consumerTag);
			return tcs.Task.Result;
		}
		else
			throw new TimeoutException($"Подтверждение для {taskId} не получено за {timeoutSeconds} секунд.");
	}
}

#nullable enable
public record RabbitMessage
{
	public string TaskId { get; set; } = string.Empty;
	public string Title { get; set; } = string.Empty;
	public string? CompletedAt { get; set; }
}


