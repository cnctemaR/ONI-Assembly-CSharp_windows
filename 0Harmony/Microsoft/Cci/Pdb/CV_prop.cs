using System;

namespace Microsoft.Cci.Pdb
{
	[Flags]
	internal enum CV_prop : ushort
	{
		packed = 1,
		ctor = 2,
		ovlops = 4,
		isnested = 8,
		cnested = 16,
		opassign = 32,
		opcast = 64,
		fwdref = 128,
		scoped = 256
	}
}
