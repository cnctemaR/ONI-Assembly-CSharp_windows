using System;
using System.Runtime.CompilerServices;

namespace Mono
{
	internal static class Runtime
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void mono_runtime_install_handlers();

		internal static void InstallSignalHandlers()
		{
			Runtime.mono_runtime_install_handlers();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetDisplayName();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetNativeStackTrace(Exception exception);

		public static bool SetGCAllowSynchronousMajor(bool flag)
		{
			return true;
		}
	}
}
