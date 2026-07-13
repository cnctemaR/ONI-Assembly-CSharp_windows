using System;
using System.Runtime.CompilerServices;

namespace Unity.Collections.LowLevel.Unsafe
{
	[BurstCompatible]
	internal static class UnsafePtrListTExtensions
	{
		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public static ref UnsafeList<IntPtr> ListData<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this UnsafePtrList<T> from) where T : struct, ValueType
		{
			return UnsafeUtility.As<UnsafePtrList<T>, UnsafeList<IntPtr>>(ref from);
		}
	}
}
