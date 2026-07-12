using System;

namespace Microsoft.Cci.Pdb
{
	[Flags]
	internal enum FRAMEDATA_FLAGS : uint
	{
		fHasSEH = 1U,
		fHasEH = 2U,
		fIsFunctionStart = 4U
	}
}
