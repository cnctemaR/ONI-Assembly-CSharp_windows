using System;
using System.Runtime.ConstrainedExecution;
using System.Security;

namespace System.Runtime
{
	public sealed class MemoryFailPoint : CriticalFinalizerObject, IDisposable
	{
		[MonoTODO]
		public MemoryFailPoint(int sizeInMegabytes)
		{
			throw new NotImplementedException();
		}

		~MemoryFailPoint()
		{
		}

		[SecuritySafeCritical]
		[MonoTODO]
		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
