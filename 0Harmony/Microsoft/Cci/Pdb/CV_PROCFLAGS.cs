using System;

namespace Microsoft.Cci.Pdb
{
	[Flags]
	internal enum CV_PROCFLAGS : byte
	{
		CV_PFLAG_NOFPO = 1,
		CV_PFLAG_INT = 2,
		CV_PFLAG_FAR = 4,
		CV_PFLAG_NEVER = 8,
		CV_PFLAG_NOTREACHED = 16,
		CV_PFLAG_CUST_CALL = 32,
		CV_PFLAG_NOINLINE = 64,
		CV_PFLAG_OPTDBGINFO = 128
	}
}
