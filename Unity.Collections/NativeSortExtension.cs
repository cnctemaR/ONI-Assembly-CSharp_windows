using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace Unity.Collections
{
	[BurstCompatible]
	public static class NativeSortExtension
	{
		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static void Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(T* array, int length) where T : struct, ValueType, IComparable<T>
		{
			NativeSortExtension.IntroSort<T, NativeSortExtension.DefaultComparer<T>>((void*)array, length, default(NativeSortExtension.DefaultComparer<T>));
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(NativeSortExtension.DefaultComparer<int>)
		})]
		public unsafe static void Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(T* array, int length, U comp) where T : struct, ValueType where U : IComparer<T>
		{
			NativeSortExtension.IntroSort<T, U>((void*)array, length, comp);
		}

		[NotBurstCompatible]
		[Obsolete("Instead call SortJob(T*, int).Schedule(JobHandle). (RemovedAfter 2021-06-20)", false)]
		public unsafe static JobHandle Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(T* array, int length, JobHandle inputDeps) where T : struct, ValueType, IComparable<T>
		{
			return NativeSortExtension.Sort<T, NativeSortExtension.DefaultComparer<T>>(array, length, default(NativeSortExtension.DefaultComparer<T>), inputDeps);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) }, RequiredUnityDefine = "UNITY_2020_2_OR_NEWER")]
		public unsafe static SortJob<T, NativeSortExtension.DefaultComparer<T>> SortJob<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(T* array, int length) where T : struct, ValueType, IComparable<T>
		{
			return new SortJob<T, NativeSortExtension.DefaultComparer<T>>
			{
				Data = array,
				Length = length,
				Comp = default(NativeSortExtension.DefaultComparer<T>)
			};
		}

		[NotBurstCompatible]
		[Obsolete("Instead call SortJob(T*, int, U).Schedule(JobHandle). (RemovedAfter 2021-06-20)", false)]
		public unsafe static JobHandle Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(T* array, int length, U comp, JobHandle inputDeps) where T : struct, ValueType where U : IComparer<T>
		{
			if (length == 0)
			{
				return inputDeps;
			}
			int num = (length + 1023) / 1024;
			int num2 = math.max(1, 128);
			int num3 = num / num2;
			JobHandle jobHandle = new NativeSortExtension.SegmentSort<T, U>
			{
				Data = array,
				Comp = comp,
				Length = length,
				SegmentWidth = 1024
			}.Schedule(num, num3, inputDeps);
			return new NativeSortExtension.SegmentSortMerge<T, U>
			{
				Data = array,
				Comp = comp,
				Length = length,
				SegmentWidth = 1024
			}.Schedule(jobHandle);
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(NativeSortExtension.DefaultComparer<int>)
		}, RequiredUnityDefine = "UNITY_2020_2_OR_NEWER")]
		public unsafe static SortJob<T, U> SortJob<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(T* array, int length, U comp) where T : struct, ValueType where U : IComparer<T>
		{
			return new SortJob<T, U>
			{
				Data = array,
				Length = length,
				Comp = comp
			};
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static int BinarySearch<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(T* ptr, int length, T value) where T : struct, ValueType, IComparable<T>
		{
			return NativeSortExtension.BinarySearch<T, NativeSortExtension.DefaultComparer<T>>(ptr, length, value, default(NativeSortExtension.DefaultComparer<T>));
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(NativeSortExtension.DefaultComparer<int>)
		})]
		public unsafe static int BinarySearch<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(T* ptr, int length, T value, U comp) where T : struct, ValueType where U : IComparer<T>
		{
			int num = 0;
			for (int num2 = length; num2 != 0; num2 >>= 1)
			{
				int num3 = num + (num2 >> 1);
				T t = ptr[(IntPtr)num3 * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
				int num4 = comp.Compare(value, t);
				if (num4 == 0)
				{
					return num3;
				}
				if (num4 > 0)
				{
					num = num3 + 1;
					num2--;
				}
			}
			return ~num;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public static void Sort<T>(this NativeArray<T> array) where T : struct, IComparable<T>
		{
			NativeSortExtension.IntroSortStruct<T, NativeSortExtension.DefaultComparer<T>>(array.GetUnsafePtr<T>(), array.Length, default(NativeSortExtension.DefaultComparer<T>));
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(NativeSortExtension.DefaultComparer<int>)
		})]
		public static void Sort<T, U>(this NativeArray<T> array, U comp) where T : struct where U : IComparer<T>
		{
			NativeSortExtension.IntroSortStruct<T, U>(array.GetUnsafePtr<T>(), array.Length, comp);
		}

		[NotBurstCompatible]
		[Obsolete("Instead call SortJob(this NativeArray<T>).Schedule(JobHandle). (RemovedAfter 2021-06-20)", false)]
		public unsafe static JobHandle Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeArray<T> array, JobHandle inputDeps) where T : struct, ValueType, IComparable<T>
		{
			return NativeSortExtension.Sort<T, NativeSortExtension.DefaultComparer<T>>((T*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<T>(array), array.Length, default(NativeSortExtension.DefaultComparer<T>), inputDeps);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) }, RequiredUnityDefine = "UNITY_2020_2_OR_NEWER")]
		public unsafe static SortJob<T, NativeSortExtension.DefaultComparer<T>> SortJob<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeArray<T> array) where T : struct, ValueType, IComparable<T>
		{
			return NativeSortExtension.SortJob<T, NativeSortExtension.DefaultComparer<T>>((T*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<T>(array), array.Length, default(NativeSortExtension.DefaultComparer<T>));
		}

		[NotBurstCompatible]
		[Obsolete("Instead call SortJob(this NativeArray<T>, U).Schedule(JobHandle). (RemovedAfter 2021-06-20)", false)]
		public unsafe static JobHandle Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this NativeArray<T> array, U comp, JobHandle inputDeps) where T : struct, ValueType where U : IComparer<T>
		{
			return NativeSortExtension.Sort<T, U>((T*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<T>(array), array.Length, comp, inputDeps);
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(NativeSortExtension.DefaultComparer<int>)
		}, RequiredUnityDefine = "UNITY_2020_2_OR_NEWER")]
		public unsafe static SortJob<T, U> SortJob<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this NativeArray<T> array, U comp) where T : struct, ValueType where U : IComparer<T>
		{
			return new SortJob<T, U>
			{
				Data = (T*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<T>(array),
				Length = array.Length,
				Comp = comp
			};
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public static int BinarySearch<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeArray<T> array, T value) where T : struct, ValueType, IComparable<T>
		{
			return array.BinarySearch(value, default(NativeSortExtension.DefaultComparer<T>));
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(NativeSortExtension.DefaultComparer<int>)
		})]
		public unsafe static int BinarySearch<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this NativeArray<T> array, T value, U comp) where T : struct, ValueType where U : IComparer<T>
		{
			return NativeSortExtension.BinarySearch<T, U>((T*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<T>(array), array.Length, value, comp);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public static void Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeList<T> list) where T : struct, ValueType, IComparable<T>
		{
			list.Sort(default(NativeSortExtension.DefaultComparer<T>));
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(NativeSortExtension.DefaultComparer<int>)
		})]
		public static void Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this NativeList<T> list, U comp) where T : struct, ValueType where U : IComparer<T>
		{
			NativeSortExtension.IntroSort<T, U>(list.GetUnsafePtr<T>(), list.Length, comp);
		}

		[NotBurstCompatible]
		[Obsolete("Instead call SortJob(this NativeList<T>).Schedule(JobHandle). (RemovedAfter 2021-06-20)", false)]
		public static JobHandle Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeList<T> array, JobHandle inputDeps) where T : struct, ValueType, IComparable<T>
		{
			return array.Sort(default(NativeSortExtension.DefaultComparer<T>), inputDeps);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) }, RequiredUnityDefine = "UNITY_2020_2_OR_NEWER")]
		public unsafe static SortJob<T, NativeSortExtension.DefaultComparer<T>> SortJob<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeList<T> list) where T : struct, ValueType, IComparable<T>
		{
			return NativeSortExtension.SortJob<T, NativeSortExtension.DefaultComparer<T>>((T*)list.GetUnsafePtr<T>(), list.Length, default(NativeSortExtension.DefaultComparer<T>));
		}

		[NotBurstCompatible]
		[Obsolete("Instead call SortJob(this NativeList<T>, U).Schedule(JobHandle). (RemovedAfter 2021-06-20)", false)]
		public unsafe static JobHandle Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this NativeList<T> list, U comp, JobHandle inputDeps) where T : struct, ValueType where U : IComparer<T>
		{
			return NativeSortExtension.Sort<T, U>((T*)list.GetUnsafePtr<T>(), list.Length, comp, inputDeps);
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(NativeSortExtension.DefaultComparer<int>)
		}, RequiredUnityDefine = "UNITY_2020_2_OR_NEWER")]
		public unsafe static SortJob<T, U> SortJob<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this NativeList<T> list, U comp) where T : struct, ValueType where U : IComparer<T>
		{
			return NativeSortExtension.SortJob<T, U>((T*)list.GetUnsafePtr<T>(), list.Length, comp);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public static int BinarySearch<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeList<T> list, T value) where T : struct, ValueType, IComparable<T>
		{
			return list.BinarySearch(value, default(NativeSortExtension.DefaultComparer<T>));
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(NativeSortExtension.DefaultComparer<int>)
		})]
		public unsafe static int BinarySearch<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this NativeList<T> list, T value, U comp) where T : struct, ValueType where U : IComparer<T>
		{
			return NativeSortExtension.BinarySearch<T, U>((T*)list.GetUnsafePtr<T>(), list.Length, value, comp);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public static void Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this UnsafeList<T> list) where T : struct, ValueType, IComparable<T>
		{
			list.Sort(default(NativeSortExtension.DefaultComparer<T>));
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(NativeSortExtension.DefaultComparer<int>)
		})]
		public unsafe static void Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this UnsafeList<T> list, U comp) where T : struct, ValueType where U : IComparer<T>
		{
			NativeSortExtension.IntroSort<T, U>((void*)list.Ptr, list.Length, comp);
		}

		[NotBurstCompatible]
		[Obsolete("Instead call SortJob(this UnsafeList<T>).Schedule(JobHandle). (RemovedAfter 2021-06-20)", false)]
		public static JobHandle Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this UnsafeList<T> list, JobHandle inputDeps) where T : struct, ValueType, IComparable<T>
		{
			return list.Sort(default(NativeSortExtension.DefaultComparer<T>), inputDeps);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) }, RequiredUnityDefine = "UNITY_2020_2_OR_NEWER")]
		public static SortJob<T, NativeSortExtension.DefaultComparer<T>> SortJob<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this UnsafeList<T> list) where T : struct, ValueType, IComparable<T>
		{
			return NativeSortExtension.SortJob<T, NativeSortExtension.DefaultComparer<T>>(list.Ptr, list.Length, default(NativeSortExtension.DefaultComparer<T>));
		}

		[NotBurstCompatible]
		[Obsolete("Instead call SortJob(this UnsafeList<T>, U).Schedule(JobHandle). (RemovedAfter 2021-06-20)", false)]
		public static JobHandle Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this UnsafeList<T> list, U comp, JobHandle inputDeps) where T : struct, ValueType where U : IComparer<T>
		{
			return NativeSortExtension.Sort<T, U>(list.Ptr, list.Length, comp, inputDeps);
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(NativeSortExtension.DefaultComparer<int>)
		}, RequiredUnityDefine = "UNITY_2020_2_OR_NEWER")]
		public static SortJob<T, U> SortJob<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this UnsafeList<T> list, U comp) where T : struct, ValueType where U : IComparer<T>
		{
			return NativeSortExtension.SortJob<T, U>(list.Ptr, list.Length, comp);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public static int BinarySearch<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this UnsafeList<T> list, T value) where T : struct, ValueType, IComparable<T>
		{
			return list.BinarySearch(value, default(NativeSortExtension.DefaultComparer<T>));
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(NativeSortExtension.DefaultComparer<int>)
		})]
		public static int BinarySearch<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this UnsafeList<T> list, T value, U comp) where T : struct, ValueType where U : IComparer<T>
		{
			return NativeSortExtension.BinarySearch<T, U>(list.Ptr, list.Length, value, comp);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public static void Sort<T>(this NativeSlice<T> slice) where T : struct, IComparable<T>
		{
			slice.Sort(default(NativeSortExtension.DefaultComparer<T>));
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(NativeSortExtension.DefaultComparer<int>)
		})]
		public static void Sort<T, U>(this NativeSlice<T> slice, U comp) where T : struct where U : IComparer<T>
		{
			NativeSortExtension.IntroSortStruct<T, U>(slice.GetUnsafePtr<T>(), slice.Length, comp);
		}

		[NotBurstCompatible]
		[Obsolete("Instead call SortJob(this NativeSlice<T>).Schedule(JobHandle). (RemovedAfter 2021-06-20)", false)]
		public static JobHandle Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeSlice<T> slice, JobHandle inputDeps) where T : struct, ValueType, IComparable<T>
		{
			return slice.Sort(default(NativeSortExtension.DefaultComparer<T>), inputDeps);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) }, RequiredUnityDefine = "UNITY_2020_2_OR_NEWER")]
		public unsafe static SortJob<T, NativeSortExtension.DefaultComparer<T>> SortJob<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeSlice<T> slice) where T : struct, ValueType, IComparable<T>
		{
			return NativeSortExtension.SortJob<T, NativeSortExtension.DefaultComparer<T>>((T*)slice.GetUnsafePtr<T>(), slice.Length, default(NativeSortExtension.DefaultComparer<T>));
		}

		[NotBurstCompatible]
		[Obsolete("Instead call SortJob(this NativeSlice<T>, U).Schedule(JobHandle). (RemovedAfter 2021-06-20)", false)]
		public unsafe static JobHandle Sort<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this NativeSlice<T> slice, U comp, JobHandle inputDeps) where T : struct, ValueType where U : IComparer<T>
		{
			return NativeSortExtension.Sort<T, U>((T*)slice.GetUnsafePtr<T>(), slice.Length, comp, inputDeps);
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(NativeSortExtension.DefaultComparer<int>)
		}, RequiredUnityDefine = "UNITY_2020_2_OR_NEWER")]
		public unsafe static SortJob<T, U> SortJob<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this NativeSlice<T> slice, U comp) where T : struct, ValueType where U : IComparer<T>
		{
			return NativeSortExtension.SortJob<T, U>((T*)slice.GetUnsafePtr<T>(), slice.Length, comp);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public static int BinarySearch<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeSlice<T> slice, T value) where T : struct, ValueType, IComparable<T>
		{
			return slice.BinarySearch(value, default(NativeSortExtension.DefaultComparer<T>));
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(NativeSortExtension.DefaultComparer<int>)
		})]
		public unsafe static int BinarySearch<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(this NativeSlice<T> slice, T value, U comp) where T : struct, ValueType where U : IComparer<T>
		{
			return NativeSortExtension.BinarySearch<T, U>((T*)slice.GetUnsafePtr<T>(), slice.Length, value, comp);
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(NativeSortExtension.DefaultComparer<int>)
		})]
		internal unsafe static void IntroSort<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(void* array, int length, U comp) where T : struct, ValueType where U : IComparer<T>
		{
			NativeSortExtension.IntroSort<T, U>(array, 0, length - 1, 2 * CollectionHelper.Log2Floor(length), comp);
		}

		private unsafe static void IntroSort<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(void* array, int lo, int hi, int depth, U comp) where T : struct, ValueType where U : IComparer<T>
		{
			while (hi > lo)
			{
				int num = hi - lo + 1;
				if (num <= 16)
				{
					if (num == 1)
					{
						return;
					}
					if (num == 2)
					{
						NativeSortExtension.SwapIfGreaterWithItems<T, U>(array, lo, hi, comp);
						return;
					}
					if (num == 3)
					{
						NativeSortExtension.SwapIfGreaterWithItems<T, U>(array, lo, hi - 1, comp);
						NativeSortExtension.SwapIfGreaterWithItems<T, U>(array, lo, hi, comp);
						NativeSortExtension.SwapIfGreaterWithItems<T, U>(array, hi - 1, hi, comp);
						return;
					}
					NativeSortExtension.InsertionSort<T, U>(array, lo, hi, comp);
					return;
				}
				else
				{
					if (depth == 0)
					{
						NativeSortExtension.HeapSort<T, U>(array, lo, hi, comp);
						return;
					}
					depth--;
					int num2 = NativeSortExtension.Partition<T, U>(array, lo, hi, comp);
					NativeSortExtension.IntroSort<T, U>(array, num2 + 1, hi, depth, comp);
					hi = num2 - 1;
				}
			}
		}

		private unsafe static void InsertionSort<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(void* array, int lo, int hi, U comp) where T : struct, ValueType where U : IComparer<T>
		{
			for (int i = lo; i < hi; i++)
			{
				int num = i;
				T t = UnsafeUtility.ReadArrayElement<T>(array, i + 1);
				while (num >= lo && comp.Compare(t, UnsafeUtility.ReadArrayElement<T>(array, num)) < 0)
				{
					UnsafeUtility.WriteArrayElement<T>(array, num + 1, UnsafeUtility.ReadArrayElement<T>(array, num));
					num--;
				}
				UnsafeUtility.WriteArrayElement<T>(array, num + 1, t);
			}
		}

		private unsafe static int Partition<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(void* array, int lo, int hi, U comp) where T : struct, ValueType where U : IComparer<T>
		{
			int num = lo + (hi - lo) / 2;
			NativeSortExtension.SwapIfGreaterWithItems<T, U>(array, lo, num, comp);
			NativeSortExtension.SwapIfGreaterWithItems<T, U>(array, lo, hi, comp);
			NativeSortExtension.SwapIfGreaterWithItems<T, U>(array, num, hi, comp);
			T t = UnsafeUtility.ReadArrayElement<T>(array, num);
			NativeSortExtension.Swap<T>(array, num, hi - 1);
			int i = lo;
			int num2 = hi - 1;
			while (i < num2)
			{
				while (comp.Compare(t, UnsafeUtility.ReadArrayElement<T>(array, ++i)) > 0)
				{
				}
				while (comp.Compare(t, UnsafeUtility.ReadArrayElement<T>(array, --num2)) < 0)
				{
				}
				if (i >= num2)
				{
					break;
				}
				NativeSortExtension.Swap<T>(array, i, num2);
			}
			NativeSortExtension.Swap<T>(array, i, hi - 1);
			return i;
		}

		private unsafe static void HeapSort<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(void* array, int lo, int hi, U comp) where T : struct, ValueType where U : IComparer<T>
		{
			int num = hi - lo + 1;
			for (int i = num / 2; i >= 1; i--)
			{
				NativeSortExtension.Heapify<T, U>(array, i, num, lo, comp);
			}
			for (int j = num; j > 1; j--)
			{
				NativeSortExtension.Swap<T>(array, lo, lo + j - 1);
				NativeSortExtension.Heapify<T, U>(array, 1, j - 1, lo, comp);
			}
		}

		private unsafe static void Heapify<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(void* array, int i, int n, int lo, U comp) where T : struct, ValueType where U : IComparer<T>
		{
			T t = UnsafeUtility.ReadArrayElement<T>(array, lo + i - 1);
			while (i <= n / 2)
			{
				int num = 2 * i;
				if (num < n && comp.Compare(UnsafeUtility.ReadArrayElement<T>(array, lo + num - 1), UnsafeUtility.ReadArrayElement<T>(array, lo + num)) < 0)
				{
					num++;
				}
				if (comp.Compare(UnsafeUtility.ReadArrayElement<T>(array, lo + num - 1), t) < 0)
				{
					break;
				}
				UnsafeUtility.WriteArrayElement<T>(array, lo + i - 1, UnsafeUtility.ReadArrayElement<T>(array, lo + num - 1));
				i = num;
			}
			UnsafeUtility.WriteArrayElement<T>(array, lo + i - 1, t);
		}

		private unsafe static void Swap<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(void* array, int lhs, int rhs) where T : struct, ValueType
		{
			T t = UnsafeUtility.ReadArrayElement<T>(array, lhs);
			UnsafeUtility.WriteArrayElement<T>(array, lhs, UnsafeUtility.ReadArrayElement<T>(array, rhs));
			UnsafeUtility.WriteArrayElement<T>(array, rhs, t);
		}

		private unsafe static void SwapIfGreaterWithItems<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U>(void* array, int lhs, int rhs, U comp) where T : struct, ValueType where U : IComparer<T>
		{
			if (lhs != rhs && comp.Compare(UnsafeUtility.ReadArrayElement<T>(array, lhs), UnsafeUtility.ReadArrayElement<T>(array, rhs)) > 0)
			{
				NativeSortExtension.Swap<T>(array, lhs, rhs);
			}
		}

		private unsafe static void IntroSortStruct<T, U>(void* array, int length, U comp) where T : struct where U : IComparer<T>
		{
			NativeSortExtension.IntroSortStruct<T, U>(array, 0, length - 1, 2 * CollectionHelper.Log2Floor(length), comp);
		}

		private unsafe static void IntroSortStruct<T, U>(void* array, int lo, int hi, int depth, U comp) where T : struct where U : IComparer<T>
		{
			while (hi > lo)
			{
				int num = hi - lo + 1;
				if (num <= 16)
				{
					if (num == 1)
					{
						return;
					}
					if (num == 2)
					{
						NativeSortExtension.SwapIfGreaterWithItemsStruct<T, U>(array, lo, hi, comp);
						return;
					}
					if (num == 3)
					{
						NativeSortExtension.SwapIfGreaterWithItemsStruct<T, U>(array, lo, hi - 1, comp);
						NativeSortExtension.SwapIfGreaterWithItemsStruct<T, U>(array, lo, hi, comp);
						NativeSortExtension.SwapIfGreaterWithItemsStruct<T, U>(array, hi - 1, hi, comp);
						return;
					}
					NativeSortExtension.InsertionSortStruct<T, U>(array, lo, hi, comp);
					return;
				}
				else
				{
					if (depth == 0)
					{
						NativeSortExtension.HeapSortStruct<T, U>(array, lo, hi, comp);
						return;
					}
					depth--;
					int num2 = NativeSortExtension.PartitionStruct<T, U>(array, lo, hi, comp);
					NativeSortExtension.IntroSortStruct<T, U>(array, num2 + 1, hi, depth, comp);
					hi = num2 - 1;
				}
			}
		}

		private unsafe static void InsertionSortStruct<T, U>(void* array, int lo, int hi, U comp) where T : struct where U : IComparer<T>
		{
			for (int i = lo; i < hi; i++)
			{
				int num = i;
				T t = UnsafeUtility.ReadArrayElement<T>(array, i + 1);
				while (num >= lo && comp.Compare(t, UnsafeUtility.ReadArrayElement<T>(array, num)) < 0)
				{
					UnsafeUtility.WriteArrayElement<T>(array, num + 1, UnsafeUtility.ReadArrayElement<T>(array, num));
					num--;
				}
				UnsafeUtility.WriteArrayElement<T>(array, num + 1, t);
			}
		}

		private unsafe static int PartitionStruct<T, U>(void* array, int lo, int hi, U comp) where T : struct where U : IComparer<T>
		{
			int num = lo + (hi - lo) / 2;
			NativeSortExtension.SwapIfGreaterWithItemsStruct<T, U>(array, lo, num, comp);
			NativeSortExtension.SwapIfGreaterWithItemsStruct<T, U>(array, lo, hi, comp);
			NativeSortExtension.SwapIfGreaterWithItemsStruct<T, U>(array, num, hi, comp);
			T t = UnsafeUtility.ReadArrayElement<T>(array, num);
			NativeSortExtension.SwapStruct<T>(array, num, hi - 1);
			int i = lo;
			int num2 = hi - 1;
			while (i < num2)
			{
				while (comp.Compare(t, UnsafeUtility.ReadArrayElement<T>(array, ++i)) > 0)
				{
				}
				while (comp.Compare(t, UnsafeUtility.ReadArrayElement<T>(array, --num2)) < 0)
				{
				}
				if (i >= num2)
				{
					break;
				}
				NativeSortExtension.SwapStruct<T>(array, i, num2);
			}
			NativeSortExtension.SwapStruct<T>(array, i, hi - 1);
			return i;
		}

		private unsafe static void HeapSortStruct<T, U>(void* array, int lo, int hi, U comp) where T : struct where U : IComparer<T>
		{
			int num = hi - lo + 1;
			for (int i = num / 2; i >= 1; i--)
			{
				NativeSortExtension.HeapifyStruct<T, U>(array, i, num, lo, comp);
			}
			for (int j = num; j > 1; j--)
			{
				NativeSortExtension.SwapStruct<T>(array, lo, lo + j - 1);
				NativeSortExtension.HeapifyStruct<T, U>(array, 1, j - 1, lo, comp);
			}
		}

		private unsafe static void HeapifyStruct<T, U>(void* array, int i, int n, int lo, U comp) where T : struct where U : IComparer<T>
		{
			T t = UnsafeUtility.ReadArrayElement<T>(array, lo + i - 1);
			while (i <= n / 2)
			{
				int num = 2 * i;
				if (num < n && comp.Compare(UnsafeUtility.ReadArrayElement<T>(array, lo + num - 1), UnsafeUtility.ReadArrayElement<T>(array, lo + num)) < 0)
				{
					num++;
				}
				if (comp.Compare(UnsafeUtility.ReadArrayElement<T>(array, lo + num - 1), t) < 0)
				{
					break;
				}
				UnsafeUtility.WriteArrayElement<T>(array, lo + i - 1, UnsafeUtility.ReadArrayElement<T>(array, lo + num - 1));
				i = num;
			}
			UnsafeUtility.WriteArrayElement<T>(array, lo + i - 1, t);
		}

		private unsafe static void SwapStruct<T>(void* array, int lhs, int rhs) where T : struct
		{
			T t = UnsafeUtility.ReadArrayElement<T>(array, lhs);
			UnsafeUtility.WriteArrayElement<T>(array, lhs, UnsafeUtility.ReadArrayElement<T>(array, rhs));
			UnsafeUtility.WriteArrayElement<T>(array, rhs, t);
		}

		private unsafe static void SwapIfGreaterWithItemsStruct<T, U>(void* array, int lhs, int rhs, U comp) where T : struct where U : IComparer<T>
		{
			if (lhs != rhs && comp.Compare(UnsafeUtility.ReadArrayElement<T>(array, lhs), UnsafeUtility.ReadArrayElement<T>(array, rhs)) > 0)
			{
				NativeSortExtension.SwapStruct<T>(array, lhs, rhs);
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckStrideMatchesSize<T>(int stride) where T : struct
		{
			if (stride != UnsafeUtility.SizeOf<T>())
			{
				throw new InvalidOperationException("Sort requires that stride matches the size of the source type");
			}
		}

		private const int k_IntrosortSizeThreshold = 16;

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public struct DefaultComparer<T> : IComparer<T> where T : IComparable<T>
		{
			public int Compare(T x, T y)
			{
				return x.CompareTo(y);
			}
		}

		[BurstCompile]
		private struct SegmentSort<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U> : IJobParallelFor where T : struct, ValueType where U : IComparer<T>
		{
			public void Execute(int index)
			{
				int num = index * this.SegmentWidth;
				int num2 = ((this.Length - num < this.SegmentWidth) ? (this.Length - num) : this.SegmentWidth);
				NativeSortExtension.Sort<T, U>(this.Data + (IntPtr)num * (IntPtr)sizeof(T) / (IntPtr)sizeof(T), num2, this.Comp);
			}

			[NativeDisableUnsafePtrRestriction]
			public unsafe T* Data;

			public U Comp;

			public int Length;

			public int SegmentWidth;
		}

		[BurstCompile]
		private struct SegmentSortMerge<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U> : IJob where T : struct, ValueType where U : IComparer<T>
		{
			public unsafe void Execute()
			{
				int num = (this.Length + (this.SegmentWidth - 1)) / this.SegmentWidth;
				int* ptr;
				checked
				{
					ptr = stackalloc int[unchecked((UIntPtr)num) * 4];
				}
				T* ptr2 = (T*)Memory.Unmanaged.Allocate((long)(UnsafeUtility.SizeOf<T>() * this.Length), 16, Allocator.Temp);
				for (int i = 0; i < this.Length; i++)
				{
					int num2 = -1;
					T t = default(T);
					for (int j = 0; j < num; j++)
					{
						int num3 = j * this.SegmentWidth;
						int num4 = ptr[j];
						int num5 = ((this.Length - num3 < this.SegmentWidth) ? (this.Length - num3) : this.SegmentWidth);
						if (num4 != num5)
						{
							T t2 = this.Data[(IntPtr)(num3 + num4) * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
							if (num2 == -1 || this.Comp.Compare(t2, t) <= 0)
							{
								t = t2;
								num2 = j;
							}
						}
					}
					ptr[num2]++;
					ptr2[(IntPtr)i * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)] = t;
				}
				UnsafeUtility.MemCpy((void*)this.Data, (void*)ptr2, (long)(UnsafeUtility.SizeOf<T>() * this.Length));
			}

			[NativeDisableUnsafePtrRestriction]
			public unsafe T* Data;

			public U Comp;

			public int Length;

			public int SegmentWidth;
		}
	}
}
