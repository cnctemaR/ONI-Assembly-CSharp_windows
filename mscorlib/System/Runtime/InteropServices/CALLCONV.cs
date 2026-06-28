using System;

namespace System.Runtime.InteropServices
{
	[Obsolete]
	[Serializable]
	public enum CALLCONV
	{
		CC_CDECL = 1,
		CC_PASCAL,
		CC_MSCPASCAL = 2,
		CC_MACPASCAL,
		CC_STDCALL,
		CC_RESERVED,
		CC_SYSCALL,
		CC_MPWCDECL,
		CC_MPWPASCAL,
		CC_MAX
	}
}
