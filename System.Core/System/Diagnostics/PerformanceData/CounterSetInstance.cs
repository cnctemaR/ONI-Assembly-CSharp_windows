using System;
using System.Security;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics.PerformanceData
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class CounterSetInstance : IDisposable
	{
		internal CounterSetInstance()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public CounterSetInstanceCounterDataSet Counters
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		[SecurityCritical]
		public void Dispose()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
