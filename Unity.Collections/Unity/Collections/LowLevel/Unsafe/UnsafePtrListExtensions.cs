using System;
using System.Runtime.CompilerServices;

namespace Unity.Collections.LowLevel.Unsafe
{
	[GenerateTestsForBurstCompatibility]
	internal static class UnsafePtrListExtensions
	{
		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref UnsafeList<IntPtr> ListData<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this UnsafePtrList<T> from) where T : struct, ValueType
		{
			return UnsafeUtility.As<UnsafePtrList<T>, UnsafeList<IntPtr>>(ref from);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static UnsafeList<IntPtr> ListDataRO<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this UnsafePtrList<T> from) where T : struct, ValueType
		{
			return *UnsafeUtility.As<UnsafePtrList<T>, UnsafeList<IntPtr>>(ref from);
		}
	}
}
