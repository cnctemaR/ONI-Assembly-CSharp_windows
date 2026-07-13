using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Jobs;

namespace Unity.Collections.LowLevel.Unsafe
{
	public static class UnsafeListExtension
	{
		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		internal static ref UnsafeList ListData<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this UnsafeList<T> from) where T : struct, ValueType
		{
			return UnsafeUtility.As<UnsafeList<T>, UnsafeList>(ref from);
		}

		public static void Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this UnsafeList list) where T : struct, ValueType, IComparable<T>
		{
			list.Sort(default(NativeSortExtension.DefaultComparer<T>));
		}

		public static void Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this UnsafeList list, U comp) where T : struct, ValueType where U : IComparer<T>
		{
			NativeSortExtension.IntroSort<T, U>(list.Ptr, list.Length, comp);
		}

		[NotBurstCompatible]
		[Obsolete("Instead call SortJob(this UnsafeList).Schedule(JobHandle). (RemovedAfter 2021-06-20)", false)]
		public static JobHandle Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this UnsafeList container, JobHandle inputDeps) where T : struct, ValueType, IComparable<T>
		{
			return container.Sort(default(NativeSortExtension.DefaultComparer<T>), inputDeps);
		}

		public unsafe static SortJob<T, NativeSortExtension.DefaultComparer<T>> SortJob<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this UnsafeList list) where T : struct, ValueType, IComparable<T>
		{
			return NativeSortExtension.SortJob<T, NativeSortExtension.DefaultComparer<T>>((T*)list.Ptr, list.Length, default(NativeSortExtension.DefaultComparer<T>));
		}

		[NotBurstCompatible]
		[Obsolete("Instead call SortJob(this UnsafeList, U).Schedule(JobHandle). (RemovedAfter 2021-06-20)", false)]
		public unsafe static JobHandle Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this UnsafeList container, U comp, JobHandle inputDeps) where T : struct, ValueType where U : IComparer<T>
		{
			return NativeSortExtension.Sort<T, U>((T*)container.Ptr, container.Length, comp, inputDeps);
		}

		public unsafe static SortJob<T, U> SortJob<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this UnsafeList list, U comp) where T : struct, ValueType where U : IComparer<T>
		{
			return NativeSortExtension.SortJob<T, U>((T*)list.Ptr, list.Length, comp);
		}

		public static int BinarySearch<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this UnsafeList container, T value) where T : struct, ValueType, IComparable<T>
		{
			return container.BinarySearch(value, default(NativeSortExtension.DefaultComparer<T>));
		}

		public unsafe static int BinarySearch<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this UnsafeList container, T value, U comp) where T : struct, ValueType where U : IComparer<T>
		{
			return NativeSortExtension.BinarySearch<T, U>((T*)container.Ptr, container.Length, value, comp);
		}
	}
}
