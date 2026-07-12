using System;

namespace Microsoft.Cci.Pdb
{
	[Flags]
	internal enum COMPILESYM_FLAGS : uint
	{
		iLanguage = 255U,
		fEC = 256U,
		fNoDbgInfo = 512U,
		fLTCG = 1024U,
		fNoDataAlign = 2048U,
		fManagedPresent = 4096U,
		fSecurityChecks = 8192U,
		fHotPatch = 16384U,
		fCVTCIL = 32768U,
		fMSILModule = 65536U
	}
}
