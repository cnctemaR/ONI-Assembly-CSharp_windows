using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	[BurstCompatible]
	public static class NativeArrayExtensions
	{
		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public static bool Contains<T, U>(this NativeArray<T> array, U value) where T : struct, IEquatable<U>
		{
			return NativeArrayExtensions.IndexOf<T, U>(array.GetUnsafeReadOnlyPtr<T>(), array.Length, value) != -1;
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public static int IndexOf<T, U>(this NativeArray<T> array, U value) where T : struct, IEquatable<U>
		{
			return NativeArrayExtensions.IndexOf<T, U>(array.GetUnsafeReadOnlyPtr<T>(), array.Length, value);
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public static bool Contains<T, U>(this NativeArray<T>.ReadOnly array, U value) where T : struct, IEquatable<U>
		{
			return NativeArrayExtensions.IndexOf<T, U>(array.m_Buffer, array.m_Length, value) != -1;
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public static int IndexOf<T, U>(this NativeArray<T>.ReadOnly array, U value) where T : struct, IEquatable<U>
		{
			return NativeArrayExtensions.IndexOf<T, U>(array.m_Buffer, array.m_Length, value);
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public static bool Contains<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this NativeList<T> list, U value) where T : struct, ValueType, IEquatable<U>
		{
			return NativeArrayExtensions.IndexOf<T, U>(list.GetUnsafeReadOnlyPtr<T>(), list.Length, value) != -1;
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public static int IndexOf<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this NativeList<T> list, U value) where T : struct, ValueType, IEquatable<U>
		{
			return NativeArrayExtensions.IndexOf<T, U>(list.GetUnsafeReadOnlyPtr<T>(), list.Length, value);
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public unsafe static bool Contains<T, U>(void* ptr, int length, U value) where T : struct, IEquatable<U>
		{
			return NativeArrayExtensions.IndexOf<T, U>(ptr, length, value) != -1;
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public unsafe static int IndexOf<T, U>(void* ptr, int length, U value) where T : struct, IEquatable<U>
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

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public static NativeArray<U> Reinterpret<T, U>(this NativeArray<T> array) where T : struct where U : struct
		{
			int num = UnsafeUtility.SizeOf<T>();
			int num2 = UnsafeUtility.SizeOf<U>();
			long num3 = (long)array.Length * (long)num / (long)num2;
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<U>(NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<T>(array), (int)num3, Allocator.None);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public static bool ArraysEqual<T>(this NativeArray<T> array, NativeArray<T> other) where T : struct, IEquatable<T>
		{
			if (array.Length != other.Length)
			{
				return false;
			}
			for (int num = 0; num != array.Length; num++)
			{
				T t = array[num];
				if (!t.Equals(other[num]))
				{
					return false;
				}
			}
			return true;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public static bool ArraysEqual<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeList<T> array, NativeArray<T> other) where T : struct, ValueType, IEquatable<T>
		{
			return array.AsArray().ArraysEqual<T>(other);
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckReinterpretSize<T, U>(ref NativeArray<T> array) where T : struct where U : struct
		{
			int num = UnsafeUtility.SizeOf<T>();
			int num2 = UnsafeUtility.SizeOf<U>();
			long num3 = (long)array.Length * (long)num;
			if (num3 / (long)num2 * (long)num2 != num3)
			{
				throw new InvalidOperationException(string.Format("Types {0} (array length {1}) and {2} cannot be aliased due to size constraints. The size of the types and lengths involved must line up.", typeof(T), array.Length, typeof(U)));
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		internal static void Initialize<T>(this NativeArray<T> array, int length, AllocatorManager.AllocatorHandle allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory) where T : struct
		{
			AllocatorManager.AllocatorHandle allocatorHandle = allocator;
			array.m_Buffer = (ref allocatorHandle).AllocateStruct(default(T), length);
			array.m_Length = length;
			array.m_AllocatorLabel = Allocator.None;
			if (options == NativeArrayOptions.ClearMemory)
			{
				UnsafeUtility.MemClear(array.m_Buffer, (long)(array.m_Length * UnsafeUtility.SizeOf<T>()));
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(AllocatorManager.AllocatorHandle)
		})]
		internal static void Initialize<T, [global::System.Runtime.CompilerServices.IsUnmanaged] U>(this NativeArray<T> array, int length, ref U allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory) where T : struct where U : struct, ValueType, AllocatorManager.IAllocator
		{
			array.m_Buffer = (ref allocator).AllocateStruct(default(T), length);
			array.m_Length = length;
			array.m_AllocatorLabel = Allocator.None;
			if (options == NativeArrayOptions.ClearMemory)
			{
				UnsafeUtility.MemClear(array.m_Buffer, (long)(array.m_Length * UnsafeUtility.SizeOf<T>()));
			}
		}

		public struct NativeArrayStaticId<T> where T : struct
		{
			internal static readonly SharedStatic<int> s_staticSafetyId = SharedStatic<int>.GetOrCreate<NativeArray<T>>(0U);
		}
	}
}
