using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	[GenerateTestsForBurstCompatibility]
	public static class NativeArrayExtensions
	{
		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public static bool Contains<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this NativeArray<T> array, U value) where T : struct, ValueType, IEquatable<U>
		{
			return NativeArrayExtensions.IndexOf<T, U>(array.GetUnsafeReadOnlyPtr<T>(), array.Length, value) != -1;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public static int IndexOf<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this NativeArray<T> array, U value) where T : struct, ValueType, IEquatable<U>
		{
			return NativeArrayExtensions.IndexOf<T, U>(array.GetUnsafeReadOnlyPtr<T>(), array.Length, value);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public static bool Contains<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this NativeArray<T>.ReadOnly array, U value) where T : struct, ValueType, IEquatable<U>
		{
			return NativeArrayExtensions.IndexOf<T, U>(array.GetUnsafeReadOnlyPtr<T>(), array.m_Length, value) != -1;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public static int IndexOf<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this NativeArray<T>.ReadOnly array, U value) where T : struct, ValueType, IEquatable<U>
		{
			return NativeArrayExtensions.IndexOf<T, U>(array.GetUnsafeReadOnlyPtr<T>(), array.m_Length, value);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public unsafe static bool Contains<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(void* ptr, int length, U value) where T : struct, ValueType, IEquatable<U>
		{
			return NativeArrayExtensions.IndexOf<T, U>(ptr, length, value) != -1;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public unsafe static int IndexOf<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(void* ptr, int length, U value) where T : struct, ValueType, IEquatable<U>
		{
			for (int num = 0; num != length; num++)
			{
				T t = UnsafeUtility.ReadArrayElement<T>(ptr, num);
				if (t.Equals(value))
				{
					return num;
				}
			}
			return -1;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public static void CopyFrom<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeArray<T> container, NativeList<T> other) where T : struct, ValueType, IEquatable<T>
		{
			container.CopyFrom(other.AsArray());
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public static void CopyFrom<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeArray<T> container, in NativeHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			NativeHashSet<T> nativeHashSet = other;
			using (NativeArray<T> nativeArray = nativeHashSet.ToNativeArray(Allocator.TempJob))
			{
				container.CopyFrom(nativeArray);
			}
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public static void CopyFrom<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeArray<T> container, in UnsafeHashSet<T> other) where T : struct, ValueType, IEquatable<T>
		{
			UnsafeHashSet<T> unsafeHashSet = other;
			using (NativeArray<T> nativeArray = unsafeHashSet.ToNativeArray(Allocator.TempJob))
			{
				container.CopyFrom(nativeArray);
			}
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public static NativeArray<U> Reinterpret<[global::System.Runtime.CompilerServices.IsUnmanaged] T, [global::System.Runtime.CompilerServices.IsUnmanaged] U>(this NativeArray<T> array) where T : struct, ValueType where U : struct, ValueType
		{
			int num = UnsafeUtility.SizeOf<T>();
			int num2 = UnsafeUtility.SizeOf<U>();
			long num3 = (long)array.Length * (long)num / (long)num2;
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<U>(NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<T>(array), (int)num3, Allocator.None);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public static bool ArraysEqual<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeArray<T> container, NativeArray<T> other) where T : struct, ValueType, IEquatable<T>
		{
			if (container.Length != other.Length)
			{
				return false;
			}
			for (int num = 0; num != container.Length; num++)
			{
				T t = container[num];
				if (!t.Equals(other[num]))
				{
					return false;
				}
			}
			return true;
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		private static void CheckReinterpretSize<[global::System.Runtime.CompilerServices.IsUnmanaged] T, [global::System.Runtime.CompilerServices.IsUnmanaged] U>(ref NativeArray<T> array) where T : struct, ValueType where U : struct, ValueType
		{
			int num = UnsafeUtility.SizeOf<T>();
			int num2 = UnsafeUtility.SizeOf<U>();
			long num3 = (long)array.Length * (long)num;
			if (num3 / (long)num2 * (long)num2 != num3)
			{
				throw new InvalidOperationException(string.Format("Types {0} (array length {1}) and {2} cannot be aliased due to size constraints. The size of the types and lengths involved must line up.", typeof(T), array.Length, typeof(U)));
			}
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		internal static void Initialize<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeArray<T> array, int length, AllocatorManager.AllocatorHandle allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory) where T : struct, ValueType
		{
			AllocatorManager.AllocatorHandle allocatorHandle = allocator;
			array = default(NativeArray<T>);
			array.m_Buffer = (ref allocatorHandle).AllocateStruct(default(T), length);
			array.m_Length = length;
			array.m_AllocatorLabel = (allocator.IsAutoDispose ? Allocator.None : allocator.ToAllocator);
			if (options == NativeArrayOptions.ClearMemory)
			{
				UnsafeUtility.MemClear(array.m_Buffer, (long)(array.m_Length * UnsafeUtility.SizeOf<T>()));
			}
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(AllocatorManager.AllocatorHandle)
		})]
		internal static void Initialize<[global::System.Runtime.CompilerServices.IsUnmanaged] T, [global::System.Runtime.CompilerServices.IsUnmanaged] U>(this NativeArray<T> array, int length, ref U allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory) where T : struct, ValueType where U : struct, ValueType, AllocatorManager.IAllocator
		{
			array = default(NativeArray<T>);
			array.m_Buffer = (ref allocator).AllocateStruct(default(T), length);
			array.m_Length = length;
			array.m_AllocatorLabel = (allocator.IsAutoDispose ? Allocator.None : allocator.ToAllocator);
			if (options == NativeArrayOptions.ClearMemory)
			{
				UnsafeUtility.MemClear(array.m_Buffer, (long)(array.m_Length * UnsafeUtility.SizeOf<T>()));
			}
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		internal static void DisposeCheckAllocator<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeArray<T> array) where T : struct, ValueType
		{
			if (array.m_Buffer == null)
			{
				throw new ObjectDisposedException("The NativeArray is already disposed.");
			}
			if (!AllocatorManager.IsCustomAllocator(array.m_AllocatorLabel))
			{
				array.Dispose();
				return;
			}
			AllocatorManager.Free(array.m_AllocatorLabel, array.m_Buffer);
			array.m_AllocatorLabel = Allocator.Invalid;
			array.m_Buffer = null;
		}

		public struct NativeArrayStaticId<[global::System.Runtime.CompilerServices.IsUnmanaged] T> where T : struct, ValueType
		{
			internal static readonly SharedStatic<int> s_staticSafetyId = SharedStatic<int>.GetOrCreate<NativeArray<T>>(0U);
		}
	}
}
