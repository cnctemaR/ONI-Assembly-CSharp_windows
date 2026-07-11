using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics.Eventing.Reader
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public class EventLogReader : IDisposable
	{
		public EventLogReader(EventLogQuery eventQuery)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		[SecurityCritical]
		public EventLogReader(EventLogQuery eventQuery, EventBookmark bookmark)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public EventLogReader(string path)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public EventLogReader(string path, PathType pathType)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public int BatchSize
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return 0;
			}
			set
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
			}
		}

		public IList<EventLogStatus> LogStatus
		{
			[SecurityCritical]
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		public void CancelReading()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
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

		public EventRecord ReadEvent()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}

		[SecurityCritical]
		public EventRecord ReadEvent(TimeSpan timeout)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}

		public void Seek(EventBookmark bookmark)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		[SecurityCritical]
		public void Seek(EventBookmark bookmark, long offset)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		[SecurityCritical]
		public void Seek(SeekOrigin origin, long offset)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
