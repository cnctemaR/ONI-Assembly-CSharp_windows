using System;
using System.Runtime.CompilerServices;
using System.Security;

namespace System
{
	[FriendAccessAllowed]
	internal class CLRConfig
	{
		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[FriendAccessAllowed]
		internal static bool CheckLegacyManagedDeflateStream()
		{
			return false;
		}

		[SuppressUnmanagedCodeSecurity]
		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool CheckThrowUnobservedTaskExceptions();
	}
}
