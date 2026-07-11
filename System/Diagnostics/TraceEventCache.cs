using System;
using System.Collections;
using System.Globalization;
using System.Threading;

namespace System.Diagnostics
{
	public class TraceEventCache
	{
		internal Guid ActivityId
		{
			get
			{
				return Trace.CorrelationManager.ActivityId;
			}
		}

		public string Callstack
		{
			get
			{
				if (this.stackTrace == null)
				{
					this.stackTrace = Environment.StackTrace;
				}
				return this.stackTrace;
			}
		}

		public Stack LogicalOperationStack
		{
			get
			{
				return Trace.CorrelationManager.LogicalOperationStack;
			}
		}

		public DateTime DateTime
		{
			get
			{
				if (this.dateTime == DateTime.MinValue)
				{
					this.dateTime = DateTime.UtcNow;
				}
				return this.dateTime;
			}
		}

		public int ProcessId
		{
			get
			{
				return TraceEventCache.GetProcessId();
			}
		}

		public string ThreadId
		{
			get
			{
				return TraceEventCache.GetThreadId().ToString(CultureInfo.InvariantCulture);
			}
		}

		public long Timestamp
		{
			get
			{
				if (this.timeStamp == -1L)
				{
					this.timeStamp = Stopwatch.GetTimestamp();
				}
				return this.timeStamp;
			}
		}

		private static void InitProcessInfo()
		{
			if (TraceEventCache.processName == null)
			{
				Process currentProcess = Process.GetCurrentProcess();
				try
				{
					TraceEventCache.processId = currentProcess.Id;
					TraceEventCache.processName = currentProcess.ProcessName;
				}
				finally
				{
					currentProcess.Dispose();
				}
			}
		}

		internal static int GetProcessId()
		{
			TraceEventCache.InitProcessInfo();
			return TraceEventCache.processId;
		}

		internal static string GetProcessName()
		{
			TraceEventCache.InitProcessInfo();
			return TraceEventCache.processName;
		}

		internal static int GetThreadId()
		{
			return Thread.CurrentThread.ManagedThreadId;
		}

		private static volatile int processId;

		private static volatile string processName;

		private long timeStamp = -1L;

		private DateTime dateTime = DateTime.MinValue;

		private string stackTrace;
	}
}
