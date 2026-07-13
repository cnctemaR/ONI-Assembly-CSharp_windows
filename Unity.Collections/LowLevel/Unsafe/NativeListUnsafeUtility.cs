using System;
using System.Runtime.CompilerServices;

namespace Unity.Collections.LowLevel.Unsafe
{
	[BurstCompatible]
	public static class NativeListUnsafeUtility
	{
		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static void* GetUnsafePtr<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeList<T> list) where T : struct, ValueType
		{
			return (void*)list.m_ListData->Ptr;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static void* GetUnsafeReadOnlyPtr<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeList<T> list) where T : struct, ValueType
		{
			return (void*)list.m_ListData->Ptr;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static void* GetInternalListDataPtrUnchecked<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(ref NativeList<T> list) where T : struct, ValueType
		{
			return (void*)list.m_ListData;
		}
	}
}
