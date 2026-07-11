using System;
using System.Security;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics.PerformanceData
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class CounterSetInstanceCounterDataSet : IDisposable
	{
		internal CounterSetInstanceCounterDataSet()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public CounterData get_Item(int counterId)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}

		public CounterData this[string counterName]
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
