using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics.Eventing.Reader
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public class EventLogPropertySelector : IDisposable
	{
		[SecurityCritical]
		public EventLogPropertySelector(IEnumerable<string> propertyQueries)
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
	}
}
