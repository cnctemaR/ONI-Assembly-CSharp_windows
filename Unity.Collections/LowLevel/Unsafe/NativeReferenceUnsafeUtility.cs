using System;
using System.Runtime.CompilerServices;

namespace Unity.Collections.LowLevel.Unsafe
{
	[BurstCompatible]
	public static class NativeReferenceUnsafeUtility
	{
		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static void* GetUnsafePtr<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeReference<T> reference) where T : struct, ValueType
		{
			return reference.m_Data;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static void* GetUnsafeReadOnlyPtr<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeReference<T> reference) where T : struct, ValueType
		{
			return reference.m_Data;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static void* GetUnsafePtrWithoutChecks<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeReference<T> reference) where T : struct, ValueType
		{
			return reference.m_Data;
		}
	}
}
