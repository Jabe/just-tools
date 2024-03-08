<Query Kind="Statements">
  <NuGetReference>System.Reactive</NuGetReference>
  <Namespace>System.Reactive.Linq</Namespace>
  <Namespace>System.Reactive.Subjects</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

var client = Observable.FromAsync(CreateExpensiveClient).Replay(1);
client.Connect(); // or use AutoConnect() in chain.

// This is just:
//var client = new ReplaySubject<RemoteClient>(1);
//Observable.FromAsync(CreateExpensiveClient).Subscribe(client);

client.SelectMany(c => c.GetItemAsync(1)).Subscribe(c => c.Dump());
client.SelectMany(c => c.GetItemAsync(2)).Subscribe(c => c.Dump());
client.SelectMany(c => c.GetItemAsync(3)).Subscribe(c => c.Dump());

await Task.Delay(500);

client.SelectMany(c => c.GetItemAsync(4)).Subscribe(c => c.Dump());

await Task.Delay(500);

async Task<RemoteClient> CreateExpensiveClient()
{
	var iid = RemoteClient.InstanceID++;
	$"Creating expensive client {iid}...".Dump();
	await Task.Delay(250);
	return new(iid);
}

class RemoteClient(int id)
{
	public static int InstanceID;

	public Task<string> GetItemAsync(int id) => Task.Delay(250).ContinueWith(t => $"Item-{id}");

	public override string ToString() => $"{nameof(InstanceID)} = {id}";
}