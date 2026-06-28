using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[ComVisible(false)]
	public struct BOID
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public byte[] rgb;
	}
}
