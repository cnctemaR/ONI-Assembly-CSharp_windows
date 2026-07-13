using System;
using System.Diagnostics;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Unity.Collections
{
	[BurstCompatible]
	internal struct FixedList
	{
		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		internal static int PaddingBytes<T>() where T : struct
		{
			return math.max(0, math.min(6, (1 << math.tzcnt(UnsafeUtility.SizeOf<T>())) - 2));
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		internal static int StorageBytes<BUFFER, T>() where BUFFER : struct where T : struct
		{
			return UnsafeUtility.SizeOf<BUFFER>() - FixedList.PaddingBytes<T>();
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		internal static int Capacity<BUFFER, T>() where BUFFER : struct where T : struct
		{
			return FixedList.StorageBytes<BUFFER, T>() / UnsafeUtility.SizeOf<T>();
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		internal static void CheckResize<BUFFER, T>(int newLength) where BUFFER : struct where T : struct
		{
			int num = FixedList.Capacity<BUFFER, T>();
			if (newLength < 0 || newLength > num)
			{
				throw new IndexOutOfRangeException(string.Format("NewLength {0} is out of range of '{1}' Capacity.", newLength, num));
			}
		}
	}
}
