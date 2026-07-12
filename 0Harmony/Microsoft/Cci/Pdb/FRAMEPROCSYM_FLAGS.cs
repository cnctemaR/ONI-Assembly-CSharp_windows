using System;

namespace Microsoft.Cci.Pdb
{
	[Flags]
	internal enum FRAMEPROCSYM_FLAGS : uint
	{
		fHasAlloca = 1U,
		fHasSetJmp = 2U,
		fHasLongJmp = 4U,
		fHasInlAsm = 8U,
		fHasEH = 16U,
		fInlSpec = 32U,
		fHasSEH = 64U,
		fNaked = 128U,
		fSecurityChecks = 256U,
		fAsyncEH = 512U,
		fGSNoStackOrdering = 1024U,
		fWasInlined = 2048U
	}
}
