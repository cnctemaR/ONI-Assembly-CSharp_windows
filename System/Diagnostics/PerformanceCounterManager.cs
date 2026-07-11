using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Diagnostics
{
	[Obsolete("use PerformanceCounter")]
	[Guid("82840be1-d273-11d2-b94a-00600893b17a")]
	[global::System.MonoTODO("not implemented")]
	[ComVisible(true)]
	[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
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
