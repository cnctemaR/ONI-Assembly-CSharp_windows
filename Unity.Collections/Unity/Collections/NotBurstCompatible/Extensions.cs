using System;
using System.Runtime.CompilerServices;

namespace Unity.Collections.NotBurstCompatible
{
	public static class Extensions
	{
		[ExcludeFromBurstCompatTesting("Returns managed array")]
		public static T[] ToArray<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeHashSet<T> set) where T : struct, ValueType, IEquatable<T>
		{
			NativeArray<T> nativeArray = set.ToNativeArray(Allocator.TempJob);
			T[] array = nativeArray.ToArray();
			nativeArray.Dispose();
			return array;
		}

		[ExcludeFromBurstCompatTesting("Returns managed array")]
		public static T[] ToArray<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeParallelHashSet<T> set) where T : struct, ValueType, IEquatable<T>
		{
			NativeArray<T> nativeArray = set.ToNativeArray(Allocator.TempJob);
			T[] array = nativeArray.ToArray();
			nativeArray.Dispose();
			return array;
		}

		[ExcludeFromBurstCompatTesting("Returns managed array")]
		public static T[] ToArrayNBC<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeList<T> list) where T : struct, ValueType
		{
			return list.AsArray().ToArray();
		}

		[ExcludeFromBurstCompatTesting("Takes managed array")]
		public static void CopyFromNBC<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeList<T> list, T[] array) where T : struct, ValueType
		{
			list.Clear();
			list.Resize(array.Length, NativeArrayOptions.UninitializedMemory);
			list.AsArray().CopyFrom(array);
		}
	}
}
