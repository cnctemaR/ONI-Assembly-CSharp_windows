using System;
using System.Runtime.InteropServices;

namespace System.Security.Principal
{
	[ComVisible(true)]
	[Serializable]
	public enum TokenImpersonationLevel
	{
		Anonymous = 1,
		Delegation = 4,
		Identification = 2,
		Impersonation,
		None = 0
	}
}
