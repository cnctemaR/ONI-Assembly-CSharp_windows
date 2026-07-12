using System;
using System.Runtime.CompilerServices;
using System.Security;

namespace System
{
	[FriendAccessAllowed]
	internal class CLRConfig
	{
		[FriendAccessAllowed]
		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
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
