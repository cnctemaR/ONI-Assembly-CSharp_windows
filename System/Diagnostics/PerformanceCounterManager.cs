using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Diagnostics
{
	[MonoTODO("not implemented")]
	[Obsolete("use PerformanceCounter")]
	[ComVisible(true)]
	[Guid("82840be1-d273-11d2-b94a-00600893b17a")]
	[PermissionSet(SecurityAction.LinkDemand, Unrestricted = true)]
	public sealed class PerformanceCounterManager : ICollectData
	{
		[Obsolete("use PerformanceCounter")]
		public PerformanceCounterManager()
		{
		}

		void ICollectData.CloseData()
		{
			throw new NotImplementedException();
		}

		void ICollectData.CollectData(int callIdx, IntPtr valueNamePtr, IntPtr dataPtr, int totalBytes, out IntPtr res)
		{
			throw new NotImplementedException();
		}
	}
}
