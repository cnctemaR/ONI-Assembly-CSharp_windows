using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace Unity.Collections
{
	[GenerateTestsForBurstCompatibility]
	public static class CollectionHelper
	{
		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		internal static void CheckAllocator(AllocatorManager.AllocatorHandle allocator)
		{
			if (!CollectionHelper.ShouldDeallocate(allocator))
			{
				throw new ArgumentException(string.Format("Allocator {0} must not be None or Invalid", allocator));
			}
		}

		public static int Log2Floor(int value)
		{
			return 31 - math.lzcnt((uint)value);
		}

		public static int Log2Ceil(int value)
		{
			return 32 - math.lzcnt((uint)(value - 1));
		}

		public static int Align(int size, int alignmentPowerOfTwo)
		{
			if (alignmentPowerOfTwo == 0)
			{
				return size;
			}
			return (size + alignmentPowerOfTwo - 1) & ~(alignmentPowerOfTwo - 1);
		}

		public static ulong Align(ulong size, ulong alignmentPowerOfTwo)
		{
			if (alignmentPowerOfTwo == 0UL)
			{
				return size;
			}
			return (size + alignmentPowerOfTwo - 1UL) & ~(alignmentPowerOfTwo - 1UL);
		}

		internal unsafe static void* AlignPointer(void* ptr, int alignmentPowerOfTwo)
		{
			if (alignmentPowerOfTwo == 0)
			{
				return ptr;
			}
			UIntPtr uintPtr = (UIntPtr)((IntPtr)alignmentPowerOfTwo);
			return (void*)(((byte*)((byte*)ptr + uintPtr) - 1) & ~(uintPtr - 1));
		}

		public unsafe static bool IsAligned(void* p, int alignmentPowerOfTwo)
		{
			return ((byte*)p & ((byte*)((long)alignmentPowerOfTwo) - 1L)) == null;
		}

		public static bool IsAligned(ulong offset, int alignmentPowerOfTwo)
		{
			return (offset & (ulong)((long)alignmentPowerOfTwo - 1L)) == 0UL;
		}

		public static bool IsPowerOfTwo(int value)
		{
			return (value & (value - 1)) == 0;
		}

		public unsafe static uint Hash(void* ptr, int bytes)
		{
			ulong num = 5381UL;
			while (bytes > 0)
			{
				int num2 = --bytes;
				ulong num3 = (ulong)((byte*)ptr)[num2];
				num = (num << 5) + num + num3;
			}
			return (uint)num;
		}

		[ExcludeFromBurstCompatTesting("Used only for debugging, and uses managed strings")]
		internal static void WriteLayout(Type type)
		{
			Console.WriteLine(string.Format("   Offset | Bytes  | Name     Layout: {0}", 0), type.Name);
			foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				Console.WriteLine("   {0, 6} | {1, 6} | {2}", Marshal.OffsetOf(type, fieldInfo.Name), Marshal.SizeOf(fieldInfo.FieldType), fieldInfo.Name);
			}
		}

		internal static bool ShouldDeallocate(AllocatorManager.AllocatorHandle allocator)
		{
			return allocator.ToAllocator > Allocator.None;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[return: AssumeRange(0L, 2147483647L)]
		internal static int AssumePositive(int value)
		{
			return value;
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		[GenerateTestsForBurstCompatibility(RequiredUnityDefine = "ENABLE_UNITY_COLLECTIONS_CHECKS", GenericTypeArguments = new Type[] { typeof(NativeArray<int>) })]
		internal static void CheckIsUnmanaged<T>()
		{
			if (!UnsafeUtility.IsUnmanaged<T>())
			{
				throw new ArgumentException(string.Format("{0} used in native collection is not blittable or not primitive", typeof(T)));
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		internal static void CheckIntPositivePowerOfTwo(int value)
		{
			if (value <= 0 || (value & (value - 1)) != 0)
			{
				throw new ArgumentException(string.Format("Alignment requested: {0} is not a non-zero, positive power of two.", value));
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		internal static void CheckUlongPositivePowerOfTwo(ulong value)
		{
			if (value <= 0UL || (value & (value - 1UL)) != 0UL)
			{
				throw new ArgumentException(string.Format("Alignment requested: {0} is not a non-zero, positive power of two.", value));
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void CheckIndexInRange(int index, int length)
		{
			if (index >= length)
			{
				throw new IndexOutOfRangeException(string.Format("Index {0} is out of range in container of '{1}' Length.", index, length));
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		internal static void CheckCapacityInRange(int capacity, int length)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException(string.Format("Capacity {0} must be positive.", capacity));
			}
			if (capacity < length)
			{
				throw new ArgumentOutOfRangeException(string.Format("Capacity {0} is out of range in container of '{1}' Length.", capacity, length));
			}
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(AllocatorManager.AllocatorHandle)
		})]
		public static NativeArray<T> CreateNativeArray<[global::System.Runtime.CompilerServices.IsUnmanaged] T, [global::System.Runtime.CompilerServices.IsUnmanaged] U>(int length, ref U allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory) where T : struct, ValueType where U : struct, ValueType, AllocatorManager.IAllocator
		{
			NativeArray<T> nativeArray;
			if (!allocator.IsCustomAllocator)
			{
				nativeArray = new NativeArray<T>(length, allocator.ToAllocator, options);
			}
			else
			{
				nativeArray = default(NativeArray<T>);
				(ref nativeArray).Initialize<T, U>(length, ref allocator, options);
			}
			return nativeArray;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public static NativeArray<T> CreateNativeArray<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(int length, AllocatorManager.AllocatorHandle allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory) where T : struct, ValueType
		{
			NativeArray<T> nativeArray;
			if (!AllocatorManager.IsCustomAllocator(allocator))
			{
				nativeArray = new NativeArray<T>(length, allocator.ToAllocator, options);
			}
			else
			{
				nativeArray = default(NativeArray<T>);
				(ref nativeArray).Initialize<T>(length, allocator, options);
			}
			return nativeArray;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public static NativeArray<T> CreateNativeArray<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(NativeArray<T> array, AllocatorManager.AllocatorHandle allocator) where T : struct, ValueType
		{
			NativeArray<T> nativeArray;
			if (!AllocatorManager.IsCustomAllocator(allocator))
			{
				nativeArray = new NativeArray<T>(array, allocator.ToAllocator);
			}
			else
			{
				nativeArray = default(NativeArray<T>);
				(ref nativeArray).Initialize<T>(array.Length, allocator, NativeArrayOptions.ClearMemory);
				nativeArray.CopyFrom(array);
			}
			return nativeArray;
		}

		[ExcludeFromBurstCompatTesting("Managed array")]
		public static NativeArray<T> CreateNativeArray<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(T[] array, AllocatorManager.AllocatorHandle allocator) where T : struct, ValueType
		{
			NativeArray<T> nativeArray;
			if (!AllocatorManager.IsCustomAllocator(allocator))
			{
				nativeArray = new NativeArray<T>(array, allocator.ToAllocator);
			}
			else
			{
				nativeArray = default(NativeArray<T>);
				(ref nativeArray).Initialize<T>(array.Length, allocator, NativeArrayOptions.ClearMemory);
				nativeArray.CopyFrom(array);
			}
			return nativeArray;
		}

		[ExcludeFromBurstCompatTesting("Managed array")]
		public static NativeArray<T> CreateNativeArray<[global::System.Runtime.CompilerServices.IsUnmanaged] T, [global::System.Runtime.CompilerServices.IsUnmanaged] U>(T[] array, ref U allocator) where T : struct, ValueType where U : struct, ValueType, AllocatorManager.IAllocator
		{
			NativeArray<T> nativeArray;
			if (!allocator.IsCustomAllocator)
			{
				nativeArray = new NativeArray<T>(array, allocator.ToAllocator);
			}
			else
			{
				nativeArray = default(NativeArray<T>);
				(ref nativeArray).Initialize<T, U>(array.Length, ref allocator, NativeArrayOptions.ClearMemory);
				nativeArray.CopyFrom(array);
			}
			return nativeArray;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public static void DisposeNativeArray<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(NativeArray<T> nativeArray, AllocatorManager.AllocatorHandle allocator) where T : struct, ValueType
		{
			(ref nativeArray).DisposeCheckAllocator<T>();
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public static void Dispose<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(NativeArray<T> nativeArray) where T : struct, ValueType
		{
			(ref nativeArray).DisposeCheckAllocator<T>();
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		private static void CheckConvertArguments<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(int length) where T : struct, ValueType
		{
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", "Length must be >= 0");
			}
			if (!UnsafeUtility.IsUnmanaged<T>())
			{
				throw new InvalidOperationException(string.Format("{0} used in NativeArray<{1}> must be unmanaged (contain no managed types).", typeof(T), typeof(T)));
			}
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static NativeArray<T> ConvertExistingDataToNativeArray<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(void* dataPointer, int length, AllocatorManager.AllocatorHandle allocator, bool setTempMemoryHandle = false) where T : struct, ValueType
		{
			NativeArray<T> nativeArray = default(NativeArray<T>);
			nativeArray.m_Buffer = dataPointer;
			nativeArray.m_Length = length;
			if (!allocator.IsCustomAllocator)
			{
				nativeArray.m_AllocatorLabel = allocator.ToAllocator;
			}
			else
			{
				nativeArray.m_AllocatorLabel = Allocator.None;
			}
			return nativeArray;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static NativeArray<T> ConvertExistingNativeListToNativeArray<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(ref NativeList<T> nativeList, int length, AllocatorManager.AllocatorHandle allocator) where T : struct, ValueType
		{
			return CollectionHelper.ConvertExistingDataToNativeArray<T>((void*)nativeList.GetUnsafePtr<T>(), length, allocator, false);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int),
			typeof(AllocatorManager.AllocatorHandle)
		})]
		public static NativeParallelMultiHashMap<TKey, TValue> CreateNativeParallelMultiHashMap<[global::System.Runtime.CompilerServices.IsUnmanaged] TKey, [global::System.Runtime.CompilerServices.IsUnmanaged] TValue, [global::System.Runtime.CompilerServices.IsUnmanaged] U>(int length, ref U allocator) where TKey : struct, ValueType, IEquatable<TKey> where TValue : struct, ValueType where U : struct, ValueType, AllocatorManager.IAllocator
		{
			NativeParallelMultiHashMap<TKey, TValue> nativeParallelMultiHashMap = default(NativeParallelMultiHashMap<TKey, TValue>);
			nativeParallelMultiHashMap.Initialize<U>(length, ref allocator);
			return nativeParallelMultiHashMap;
		}

		[GenerateTestsForBurstCompatibility(RequiredUnityDefine = "ENABLE_UNITY_COLLECTIONS_CHECKS", GenericTypeArguments = new Type[] { typeof(CollectionHelper.DummyJob) })]
		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		public static void CheckReflectionDataCorrect<T>(IntPtr reflectionData)
		{
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		[BurstDiscard]
		private static void CheckReflectionDataCorrectInternal<T>(IntPtr reflectionData, ref bool burstCompiled)
		{
			if (reflectionData == IntPtr.Zero)
			{
				throw new InvalidOperationException(string.Format("Reflection data was not set up by an Initialize() call. For generic job types, please include [assembly: RegisterGenericJobType(typeof({0}))] in your source file.", typeof(T)));
			}
			burstCompiled = false;
		}

		public const int CacheLineSize = 64;

		[StructLayout(LayoutKind.Explicit)]
		internal struct LongDoubleUnion
		{
			[FieldOffset(0)]
			internal long longValue;

			[FieldOffset(0)]
			internal double doubleValue;
		}

		[BurstCompile]
		public struct DummyJob : IJob
		{
			public void Execute()
			{
			}
		}
	}
}
