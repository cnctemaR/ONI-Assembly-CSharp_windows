using System;
using System.ComponentModel;

namespace System.Diagnostics
{
	[global::System.ComponentModel.Designer("System.Diagnostics.Design.ProcessThreadDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public class ProcessThread : global::System.ComponentModel.Component
	{
		[global::System.MonoTODO("Parse parameters")]
		internal ProcessThread()
		{
		}

		[global::System.MonoTODO]
		[MonitoringDescription("The base priority of this thread.")]
		public int BasePriority
		{
			get
			{
				return 0;
			}
		}

		[global::System.MonoTODO]
		[MonitoringDescription("The current priority of this thread.")]
		public int CurrentPriority
		{
			get
			{
				return 0;
			}
		}

		[global::System.MonoTODO]
		[MonitoringDescription("The ID of this thread.")]
		public int Id
		{
			get
			{
				return 0;
			}
		}

		[global::System.ComponentModel.Browsable(false)]
		[global::System.MonoTODO]
		public int IdealProcessor
		{
			set
			{
			}
		}

		[global::System.MonoTODO]
		[MonitoringDescription("Thread gets a priority boot when interactively used by a user.")]
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

		[global::System.MonoTODO]
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

		[global::System.MonoTODO]
		[MonitoringDescription("The amount of CPU time used in privileged mode.")]
		public TimeSpan PrivilegedProcessorTime
		{
			get
			{
				return new TimeSpan(0L);
			}
		}

		[global::System.MonoTODO]
		[global::System.ComponentModel.Browsable(false)]
		public IntPtr ProcessorAffinity
		{
			set
			{
			}
		}

		[MonitoringDescription("The start address in memory of this thread.")]
		[global::System.MonoTODO]
		public IntPtr StartAddress
		{
			get
			{
				return (IntPtr)0;
			}
		}

		[MonitoringDescription("The time this thread was started.")]
		[global::System.MonoTODO]
		public DateTime StartTime
		{
			get
			{
				return new DateTime(0L);
			}
		}

		[MonitoringDescription("The current state of this thread.")]
		[global::System.MonoTODO]
		public ThreadState ThreadState
		{
			get
			{
				return ThreadState.Initialized;
			}
		}

		[MonitoringDescription("The total amount of CPU time used.")]
		[global::System.MonoTODO]
		public TimeSpan TotalProcessorTime
		{
			get
			{
				return new TimeSpan(0L);
			}
		}

		[global::System.MonoTODO]
		[MonitoringDescription("The amount of CPU time used in user mode.")]
		public TimeSpan UserProcessorTime
		{
			get
			{
				return new TimeSpan(0L);
			}
		}

		[MonitoringDescription("The reason why this thread is waiting.")]
		[global::System.MonoTODO]
		public ThreadWaitReason WaitReason
		{
			get
			{
				return ThreadWaitReason.Executive;
			}
		}

		[global::System.MonoTODO]
		public void ResetIdealProcessor()
		{
		}
	}
}
