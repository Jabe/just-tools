<Query Kind="Statements">
  <NuGetReference>System.Reactive</NuGetReference>
  <Namespace>System.Reactive.Linq</Namespace>
  <Namespace>System.Reactive.Subjects</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

var debug = 0;

var period = new BehaviorSubject<TimeSpan>(TimeSpan.FromSeconds(2));

period
	.DistinctUntilChanged()
	.Select(p => Observable.Interval(p.Dump("New interval")))
	.Switch()
	.StartWith(-1) // Start immediately
	.SelectMany(async x => (await PollAsync()).Dump(x < 0 ? "Initial poll" : "Poll completed; next period"))
	.Subscribe(period);

ValueTask<TimeSpan> PollAsync()
{
	return debug++ < 3 ? ValueTask.FromResult(TimeSpan.FromSeconds(2)) : ValueTask.FromResult(TimeSpan.FromSeconds(5));
}