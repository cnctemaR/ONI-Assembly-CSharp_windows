using System;

namespace Unity.Collections.LowLevel.Unsafe
{
	internal static class UnsafePtrListExtensions
	{
		public static ref UnsafeList ListData(this UnsafePtrList from)
		{
			return UnsafeUtility.As<UnsafePtrList, UnsafeList>(ref from);
		}
	}
}
