using System;

namespace Microsoft.Cci.Pdb
{
	[Flags]
	internal enum LeafPointerAttr : uint
	{
		ptrtype = 31U,
		ptrmode = 224U,
		isflat32 = 256U,
		isvolatile = 512U,
		isconst = 1024U,
		isunaligned = 2048U,
		isrestrict = 4096U
	}
}
