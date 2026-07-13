using System;
using System.Runtime.CompilerServices;

namespace MonoMod
{
	internal static class ILHelpers
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T TailCallDelegatePtr<T>(IntPtr source)
		{
			return calli(T(), source);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T TailCallFunc<T>(Func<T> func)
		{
			return func();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static ref T ObjectAsRef<T>(object obj)
		{
			fixed (object obj2 = obj)
			{
				T** ptr = (T**)(&obj2);
				return *(IntPtr*)ptr;
			}
		}
	}
}
