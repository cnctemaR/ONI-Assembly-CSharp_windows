using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Runtime.InteropServices
{
	[NullableContext(2)]
	[Nullable(0)]
	internal static class MarshalEx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetLastPInvokeError()
		{
			return Marshal.GetLastWin32Error();
		}

		public static void SetLastPInvokeError(int error)
		{
			Action<int> marshal_SetLastWin32Error = MarshalEx.Marshal_SetLastWin32Error;
			if (marshal_SetLastWin32Error == null)
			{
				throw new PlatformNotSupportedException("Cannot set last P/Invoke error (no method Marshal.SetLastWin32Error or Marshal.SetLastPInvokeError)");
			}
			marshal_SetLastWin32Error(error);
		}

		private static readonly MethodInfo Marshal_SetLastWin32Error_Meth = typeof(Marshal).GetMethod("SetLastPInvokeError", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) ?? typeof(Marshal).GetMethod("SetLastWin32Error", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

		private static readonly Action<int> Marshal_SetLastWin32Error = ((MarshalEx.Marshal_SetLastWin32Error_Meth == null) ? null : ((Action<int>)Delegate.CreateDelegate(typeof(Action<int>), MarshalEx.Marshal_SetLastWin32Error_Meth)));
	}
}
