using System;
using System.ComponentModel;

namespace System.Diagnostics
{
	[Designer("System.Diagnostics.Design.ProcessThreadDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public class ProcessThread : Component
	{
		[MonoTODO("Parse parameters")]
		internal ProcessThread()
		{
		}

		[MonitoringDescription("The base priority of this thread.")]
		[MonoTODO]
		public int BasePriority
		{
			get
			{
				return 0;
			}
		}

		[MonoTODO]
		[MonitoringDescription("The current priority of this thread.")]
		public int CurrentPriority
		{
			get
			{
				return 0;
			}
		}

		[MonitoringDescription("The ID of this thread.")]
		[MonoTODO]
		public int Id
		{
			get
			{
				return 0;
			}
		}

		[Browsable(false)]
		[MonoTODO]
		public int IdealProcessor
		{
			set
			{
			}
		}

		[MonitoringDescription("Thread gets a priority boot when interactively used by a user.")]
		[MonoTODO]
		public bool PriorityBoostEnabled
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		[MonoTODO]
		[MonitoringDescription("The priority level of this thread.")]
		public ThreadPriorityLevel PriorityLevel
		{
			get
			{
				return ThreadPriorityLevel.Idle;
			}
			set
			{
			}
		}

		[MonitoringDescription("The amount of CPU time used in privileged mode.")]
		[MonoTODO]
		public TimeSpan PrivilegedProcessorTime
		{
			get
			{
				return new TimeSpan(0L);
			}
		}

		[Browsable(false)]
		[MonoTODO]
		public IntPtr ProcessorAffinity
		{
			set
			{
			}
		}

		[MonitoringDescription("The start address in memory of this thread.")]
		[MonoTODO]
		public IntPtr StartAddress
		{
			get
			{
				return (IntPtr)0;
			}
		}

		[MonoTODO]
		[MonitoringDescription("The time this thread was started.")]
		public DateTime StartTime
		{
			get
			{
				return new DateTime(0L);
			}
		}

		[MonitoringDescription("The current state of this thread.")]
		[MonoTODO]
		public ThreadState ThreadState
		{
			get
			{
				return ThreadState.Initialized;
			}
		}

		[MonitoringDescription("The total amount of CPU time used.")]
		[MonoTODO]
		public TimeSpan TotalProcessorTime
		{
			get
			{
				return new TimeSpan(0L);
			}
		}

		[MonoTODO]
		[MonitoringDescription("The amount of CPU time used in user mode.")]
		public TimeSpan UserProcessorTime
		{
			get
			{
				return new TimeSpan(0L);
			}
		}

		[MonoTODO]
		[MonitoringDescription("The reason why this thread is waiting.")]
		public ThreadWaitReason WaitReason
		{
			get
			{
				return ThreadWaitReason.Executive;
			}
		}

		[MonoTODO]
		public void ResetIdealProcessor()
		{
		}
	}
}
