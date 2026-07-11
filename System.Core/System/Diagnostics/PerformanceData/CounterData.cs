using System;
using System.Security;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics.PerformanceData
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class CounterData
	{
		internal CounterData()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public long RawValue
		{
			[SecurityCritical]
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return 0L;
			}
			[SecurityCritical]
			set
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
			}
		}

		public long Value
		{
			[SecurityCritical]
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return 0L;
			}
			[SecurityCritical]
			set
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
			}
		}

		[SecurityCritical]
		public void Decrement()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		[SecurityCritical]
		public void Increment()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		[SecurityCritical]
		public void IncrementBy(long value)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
