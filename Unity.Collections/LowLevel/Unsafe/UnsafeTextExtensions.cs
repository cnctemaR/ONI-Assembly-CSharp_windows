using System;

namespace Unity.Collections.LowLevel.Unsafe
{
	internal static class UnsafeTextExtensions
	{
		public static ref UnsafeList<byte> AsUnsafeListOfBytes(this UnsafeText text)
		{
			return UnsafeUtility.As<UntypedUnsafeList, UnsafeList<byte>>(ref text.m_UntypedListData);
		}
	}
}
