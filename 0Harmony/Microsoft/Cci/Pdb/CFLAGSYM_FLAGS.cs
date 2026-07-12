using System;

namespace Microsoft.Cci.Pdb
{
	[Flags]
	internal enum CFLAGSYM_FLAGS : ushort
	{
		pcode = 1,
		floatprec = 6,
		floatpkg = 24,
		ambdata = 224,
		ambcode = 1792,
		mode32 = 2048
	}
}
