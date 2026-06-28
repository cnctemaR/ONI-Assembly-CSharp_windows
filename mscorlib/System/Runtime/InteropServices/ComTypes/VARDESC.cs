using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct VARDESC
	{
		public int memid;

		public string lpstrSchema;

		public VARDESC.DESCUNION desc;

		public ELEMDESC elemdescVar;

		public short wVarFlags;

		public VARKIND varkind;

		[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
		public struct DESCUNION
		{
			[FieldOffset(0)]
			public IntPtr lpvarValue;

			[FieldOffset(0)]
			public int oInst;
		}
	}
}
