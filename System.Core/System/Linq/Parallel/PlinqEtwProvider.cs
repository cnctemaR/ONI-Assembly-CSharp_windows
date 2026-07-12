using System;
using System.Diagnostics.Tracing;
using System.Threading;
using System.Threading.Tasks;

namespace System.Linq.Parallel
{
	[EventSource(Name = "System.Linq.Parallel.PlinqEventSource", Guid = "159eeeec-4a14-4418-a8fe-faabcd987887")]
	internal sealed class PlinqEtwProvider : EventSource
	{
		private PlinqEtwProvider()
		{
		}

		[NonEvent]
		internal static int NextQueryId()
		{
			return Interlocked.Increment(ref PlinqEtwProvider.s_queryId);
		}

		[NonEvent]
		internal void ParallelQueryBegin(int queryId)
		{
			if (base.IsEnabled(EventLevel.Informational, EventKeywords.All))
			{
				int valueOrDefault = Task.CurrentId.GetValueOrDefault();
				this.ParallelQueryBegin(PlinqEtwProvider.s_defaultSchedulerId, valueOrDefault, queryId);
			}
		}

		[Event(1, Level = EventLevel.Informational, Task = (EventTask)1, Opcode = EventOpcode.Start)]
		private void ParallelQueryBegin(int taskSchedulerId, int taskId, int queryId)
		{
			base.WriteEvent(1, taskSchedulerId, taskId, queryId);
		}

		[NonEvent]
		internal void ParallelQueryEnd(int queryId)
		{
			if (base.IsEnabled(EventLevel.Informational, EventKeywords.All))
			{
				int valueOrDefault = Task.CurrentId.GetValueOrDefault();
				this.ParallelQueryEnd(PlinqEtwProvider.s_defaultSchedulerId, valueOrDefault, queryId);
			}
		}

		[Event(2, Level = EventLevel.Informational, Task = (EventTask)1, Opcode = EventOpcode.Stop)]
		private void ParallelQueryEnd(int taskSchedulerId, int taskId, int queryId)
		{
			base.WriteEvent(2, taskSchedulerId, taskId, queryId);
		}

		[NonEvent]
		internal void ParallelQueryFork(int queryId)
		{
			if (base.IsEnabled(EventLevel.Verbose, EventKeywords.All))
			{
				int valueOrDefault = Task.CurrentId.GetValueOrDefault();
				this.ParallelQueryFork(PlinqEtwProvider.s_defaultSchedulerId, valueOrDefault, queryId);
			}
		}

		[Event(3, Level = EventLevel.Verbose, Task = (EventTask)2, Opcode = EventOpcode.Start)]
		private void ParallelQueryFork(int taskSchedulerId, int taskId, int queryId)
		{
			base.WriteEvent(3, taskSchedulerId, taskId, queryId);
		}

		[NonEvent]
		internal void ParallelQueryJoin(int queryId)
		{
			if (base.IsEnabled(EventLevel.Verbose, EventKeywords.All))
			{
				int valueOrDefault = Task.CurrentId.GetValueOrDefault();
				this.ParallelQueryJoin(PlinqEtwProvider.s_defaultSchedulerId, valueOrDefault, queryId);
			}
		}

		[Event(4, Level = EventLevel.Verbose, Task = (EventTask)2, Opcode = EventOpcode.Stop)]
		private void ParallelQueryJoin(int taskSchedulerId, int taskId, int queryId)
		{
			base.WriteEvent(4, taskSchedulerId, taskId, queryId);
		}

		internal static PlinqEtwProvider Log = new PlinqEtwProvider();

		private static readonly int s_defaultSchedulerId = TaskScheduler.Default.Id;

		private static int s_queryId = 0;

		private const EventKeywords ALL_KEYWORDS = EventKeywords.All;

		private const int PARALLELQUERYBEGIN_EVENTID = 1;

		private const int PARALLELQUERYEND_EVENTID = 2;

		private const int PARALLELQUERYFORK_EVENTID = 3;

		private const int PARALLELQUERYJOIN_EVENTID = 4;

		public class Tasks
		{
			public const EventTask Query = (EventTask)1;

			public const EventTask ForkJoin = (EventTask)2;
		}
	}
}
