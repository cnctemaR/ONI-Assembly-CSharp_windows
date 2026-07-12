using System;
using System.Diagnostics.Tracing;

namespace System.Threading.Tasks
{
	[EventSource(Name = "System.Threading.Tasks.Parallel.EventSource")]
	internal sealed class ParallelEtwProvider : EventSource
	{
		private ParallelEtwProvider()
		{
		}

		[Event(1, Level = EventLevel.Informational, Task = (EventTask)1, Opcode = EventOpcode.Start)]
		public unsafe void ParallelLoopBegin(int OriginatingTaskSchedulerID, int OriginatingTaskID, int ForkJoinContextID, ParallelEtwProvider.ForkJoinOperationType OperationType, long InclusiveFrom, long ExclusiveTo)
		{
			if (base.IsEnabled(EventLevel.Informational, EventKeywords.All))
			{
				EventSource.EventData* ptr;
				checked
				{
					ptr = stackalloc EventSource.EventData[unchecked((UIntPtr)6) * (UIntPtr)sizeof(EventSource.EventData)];
				}
				*ptr = new EventSource.EventData
				{
					Size = 4,
					DataPointer = (IntPtr)((void*)(&OriginatingTaskSchedulerID))
				};
				ptr[1] = new EventSource.EventData
				{
					Size = 4,
					DataPointer = (IntPtr)((void*)(&OriginatingTaskID))
				};
				ptr[2] = new EventSource.EventData
				{
					Size = 4,
					DataPointer = (IntPtr)((void*)(&ForkJoinContextID))
				};
				ptr[3] = new EventSource.EventData
				{
					Size = 4,
					DataPointer = (IntPtr)((void*)(&OperationType))
				};
				ptr[4] = new EventSource.EventData
				{
					Size = 8,
					DataPointer = (IntPtr)((void*)(&InclusiveFrom))
				};
				ptr[5] = new EventSource.EventData
				{
					Size = 8,
					DataPointer = (IntPtr)((void*)(&ExclusiveTo))
				};
				base.WriteEventCore(1, 6, ptr);
			}
		}

		[Event(2, Level = EventLevel.Informational, Task = (EventTask)1, Opcode = EventOpcode.Stop)]
		public unsafe void ParallelLoopEnd(int OriginatingTaskSchedulerID, int OriginatingTaskID, int ForkJoinContextID, long TotalIterations)
		{
			if (base.IsEnabled(EventLevel.Informational, EventKeywords.All))
			{
				EventSource.EventData* ptr;
				checked
				{
					ptr = stackalloc EventSource.EventData[unchecked((UIntPtr)4) * (UIntPtr)sizeof(EventSource.EventData)];
				}
				*ptr = new EventSource.EventData
				{
					Size = 4,
					DataPointer = (IntPtr)((void*)(&OriginatingTaskSchedulerID))
				};
				ptr[1] = new EventSource.EventData
				{
					Size = 4,
					DataPointer = (IntPtr)((void*)(&OriginatingTaskID))
				};
				ptr[2] = new EventSource.EventData
				{
					Size = 4,
					DataPointer = (IntPtr)((void*)(&ForkJoinContextID))
				};
				ptr[3] = new EventSource.EventData
				{
					Size = 8,
					DataPointer = (IntPtr)((void*)(&TotalIterations))
				};
				base.WriteEventCore(2, 4, ptr);
			}
		}

		[Event(3, Level = EventLevel.Informational, Task = (EventTask)2, Opcode = EventOpcode.Start)]
		public unsafe void ParallelInvokeBegin(int OriginatingTaskSchedulerID, int OriginatingTaskID, int ForkJoinContextID, ParallelEtwProvider.ForkJoinOperationType OperationType, int ActionCount)
		{
			if (base.IsEnabled(EventLevel.Informational, EventKeywords.All))
			{
				EventSource.EventData* ptr;
				checked
				{
					ptr = stackalloc EventSource.EventData[unchecked((UIntPtr)5) * (UIntPtr)sizeof(EventSource.EventData)];
				}
				*ptr = new EventSource.EventData
				{
					Size = 4,
					DataPointer = (IntPtr)((void*)(&OriginatingTaskSchedulerID))
				};
				ptr[1] = new EventSource.EventData
				{
					Size = 4,
					DataPointer = (IntPtr)((void*)(&OriginatingTaskID))
				};
				ptr[2] = new EventSource.EventData
				{
					Size = 4,
					DataPointer = (IntPtr)((void*)(&ForkJoinContextID))
				};
				ptr[3] = new EventSource.EventData
				{
					Size = 4,
					DataPointer = (IntPtr)((void*)(&OperationType))
				};
				ptr[4] = new EventSource.EventData
				{
					Size = 4,
					DataPointer = (IntPtr)((void*)(&ActionCount))
				};
				base.WriteEventCore(3, 5, ptr);
			}
		}

		[Event(4, Level = EventLevel.Informational, Task = (EventTask)2, Opcode = EventOpcode.Stop)]
		public void ParallelInvokeEnd(int OriginatingTaskSchedulerID, int OriginatingTaskID, int ForkJoinContextID)
		{
			if (base.IsEnabled(EventLevel.Informational, EventKeywords.All))
			{
				base.WriteEvent(4, OriginatingTaskSchedulerID, OriginatingTaskID, ForkJoinContextID);
			}
		}

		[Event(5, Level = EventLevel.Verbose, Task = (EventTask)5, Opcode = EventOpcode.Start)]
		public void ParallelFork(int OriginatingTaskSchedulerID, int OriginatingTaskID, int ForkJoinContextID)
		{
			if (base.IsEnabled(EventLevel.Verbose, EventKeywords.All))
			{
				base.WriteEvent(5, OriginatingTaskSchedulerID, OriginatingTaskID, ForkJoinContextID);
			}
		}

		[Event(6, Level = EventLevel.Verbose, Task = (EventTask)5, Opcode = EventOpcode.Stop)]
		public void ParallelJoin(int OriginatingTaskSchedulerID, int OriginatingTaskID, int ForkJoinContextID)
		{
			if (base.IsEnabled(EventLevel.Verbose, EventKeywords.All))
			{
				base.WriteEvent(6, OriginatingTaskSchedulerID, OriginatingTaskID, ForkJoinContextID);
			}
		}

		public static readonly ParallelEtwProvider Log = new ParallelEtwProvider();

		private const EventKeywords ALL_KEYWORDS = EventKeywords.All;

		private const int PARALLELLOOPBEGIN_ID = 1;

		private const int PARALLELLOOPEND_ID = 2;

		private const int PARALLELINVOKEBEGIN_ID = 3;

		private const int PARALLELINVOKEEND_ID = 4;

		private const int PARALLELFORK_ID = 5;

		private const int PARALLELJOIN_ID = 6;

		public enum ForkJoinOperationType
		{
			ParallelInvoke = 1,
			ParallelFor,
			ParallelForEach
		}

		public class Tasks
		{
			public const EventTask Loop = (EventTask)1;

			public const EventTask Invoke = (EventTask)2;

			public const EventTask ForkJoin = (EventTask)5;
		}
	}
}
