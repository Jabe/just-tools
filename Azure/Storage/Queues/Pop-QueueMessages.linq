<Query Kind="Statements">
  <NuGetReference>Azure.Storage.Queues</NuGetReference>
  <Namespace>Azure.Storage.Queues</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>Azure.Storage.Queues.Models</Namespace>
  <Namespace>System.Buffers.Text</Namespace>
</Query>

var connectionString = Util.Cache(() => Util.ReadLine("Enter connection string (e.g. https://xxx.queue.core.windows.net/yyy?sv=...)"));
var maxMessages = 10;

var client = new QueueClient(new Uri(connectionString));
var response = await client.ReceiveMessagesAsync(maxMessages);

static JsonElement Decode(QueueMessage message)
{
	ReadOnlySpan<byte> data = message.Body;
	Span<byte> buffer = stackalloc byte[data.Length];
	
	// Assume base64
	Base64.DecodeFromUtf8(data, buffer, out _, out var written);
	
	// Assume JSON
	return JsonSerializer.Deserialize<JsonElement>(buffer[..written]);
}

foreach (var message in response.Value)
{
	Decode(message).Dump();
	await client.DeleteMessageAsync(message.MessageId, message.PopReceipt);
}
