using System;
using System.Runtime.CompilerServices;

namespace Unity.Collections.LowLevel.Unsafe
{
	internal static class UnsafeTextExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref UnsafeList<byte> AsUnsafeListOfBytes(this UnsafeText text)
		{
			return UnsafeUtility.As<UntypedUnsafeList, UnsafeList<byte>>(ref text.m_UntypedListData);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static UnsafeList<byte> AsUnsafeListOfBytesRO(this UnsafeText text)
		{
			return *UnsafeUtility.As<UntypedUnsafeList, UnsafeList<byte>>(ref text.m_UntypedListData);
		}
	}
}
