using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[ComVisible(false)]
	public struct XACTTRANSINFO
	{
		public int grfRMSupported;

		public int grfRMSupportedRetaining;

		public int grfTCSupported;

		public int grfTCSupportedRetaining;

		public int isoFlags;

		public int isoLevel;

		public BOID uow;
	}
}
