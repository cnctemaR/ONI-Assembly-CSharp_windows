using System;
using System.Runtime.CompilerServices;

namespace Unity.Collections.LowLevel.Unsafe
{
	[GenerateTestsForBurstCompatibility]
	public static class NativeReferenceUnsafeUtility
	{
		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static T* GetUnsafePtr<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeReference<T> reference) where T : struct, ValueType
		{
			return (T*)reference.m_Data;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static T* GetUnsafeReadOnlyPtr<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeReference<T> reference) where T : struct, ValueType
		{
			return (T*)reference.m_Data;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static T* GetUnsafePtrWithoutChecks<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeReference<T> reference) where T : struct, ValueType
		{
			return (T*)reference.m_Data;
		}
	}
}
