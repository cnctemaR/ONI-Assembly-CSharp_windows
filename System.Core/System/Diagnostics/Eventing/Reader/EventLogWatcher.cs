using System;
using System.Security;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics.Eventing.Reader
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public class EventLogWatcher : IDisposable
	{
		public EventLogWatcher(EventLogQuery eventQuery)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public EventLogWatcher(EventLogQuery eventQuery, EventBookmark bookmark)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public EventLogWatcher(EventLogQuery eventQuery, EventBookmark bookmark, bool readExistingEvents)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public EventLogWatcher(string path)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public bool Enabled
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return default(bool);
			}
			set
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
			}
		}

		public event EventHandler<EventRecordWrittenEventArgs> EventRecordWritten
		{
			add
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
			}
			remove
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
			}
		}

		public void Dispose()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		[SecuritySafeCritical]
		protected virtual void Dispose(bool disposing)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
