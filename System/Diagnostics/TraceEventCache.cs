using System;
using System.Collections;
using System.Threading;

namespace System.Diagnostics
{
	public class TraceEventCache
	{
		public TraceEventCache()
		{
			this.started = DateTime.Now;
			this.manager = Trace.CorrelationManager;
			this.callstack = Environment.StackTrace;
			this.timestamp = Stopwatch.GetTimestamp();
			this.thread = Thread.CurrentThread.Name;
			this.process = Process.GetCurrentProcess().Id;
		}

		public string Callstack
		{
			get
			{
				return this.callstack;
			}
		}

		public DateTime DateTime
		{
			get
			{
				return this.started;
			}
		}

		public Stack LogicalOperationStack
		{
			get
			{
				return this.manager.LogicalOperationStack;
			}
		}

		public int ProcessId
		{
			get
			{
				return this.process;
			}
		}

		public string ThreadId
		{
			get
			{
				return this.thread;
			}
		}

		public long Timestamp
		{
			get
			{
				return this.timestamp;
			}
		}

		private DateTime started;

		private CorrelationManager manager;

		private string callstack;

		private string thread;

		private int process;

		private long timestamp;
	}
}
