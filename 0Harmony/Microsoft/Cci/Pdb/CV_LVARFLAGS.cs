using System;

namespace Microsoft.Cci.Pdb
{
	[Flags]
	internal enum CV_LVARFLAGS : ushort
	{
		fIsParam = 1,
		fAddrTaken = 2,
		fCompGenx = 4,
		fIsAggregate = 8,
		fIsAggregated = 16,
		fIsAliased = 32,
		fIsAlias = 64
	}
}
