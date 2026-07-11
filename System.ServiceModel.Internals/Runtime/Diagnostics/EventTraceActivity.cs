using System;
using System.Diagnostics;
using System.Security;

namespace System.Runtime.Diagnostics
{
	internal class EventTraceActivity
	{
		public EventTraceActivity(bool setOnThread = false)
			: this(Guid.NewGuid(), setOnThread)
		{
		}

		public EventTraceActivity(Guid guid, bool setOnThread = false)
		{
			this.ActivityId = guid;
			if (setOnThread)
			{
				this.SetActivityIdOnThread();
			}
		}

		public static EventTraceActivity Empty
		{
			get
			{
				if (EventTraceActivity.empty == null)
				{
					EventTraceActivity.empty = new EventTraceActivity(Guid.Empty, false);
				}
				return EventTraceActivity.empty;
			}
		}

		public static string Name
		{
			get
			{
				return "E2EActivity";
			}
		}

		[SecuritySafeCritical]
		public static EventTraceActivity GetFromThreadOrCreate(bool clearIdOnThread = false)
		{
			Guid guid = Trace.CorrelationManager.ActivityId;
			if (guid == Guid.Empty)
			{
				guid = Guid.NewGuid();
			}
			else if (clearIdOnThread)
			{
				Trace.CorrelationManager.ActivityId = Guid.Empty;
			}
			return new EventTraceActivity(guid, false);
		}

		[SecuritySafeCritical]
		public static Guid GetActivityIdFromThread()
		{
			return Trace.CorrelationManager.ActivityId;
		}

		public void SetActivityId(Guid guid)
		{
			this.ActivityId = guid;
		}

		[SecuritySafeCritical]
		private void SetActivityIdOnThread()
		{
			Trace.CorrelationManager.ActivityId = this.ActivityId;
		}

		public Guid ActivityId;

		private static EventTraceActivity empty;
	}
}
