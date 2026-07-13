using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.Collections
{
	[GenerateTestsForBurstCompatibility(RequiredUnityDefine = "UNITY_2020_2_OR_NEWER", GenericTypeArguments = new Type[]
	{
		typeof(int),
		typeof(NativeSortExtension.DefaultComparer<int>)
	})]
	public struct SortJobDefer<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U> where T : struct, ValueType where U : IComparer<T>
	{
		public JobHandle Schedule(JobHandle inputDeps = default(JobHandle))
		{
			SortJobDefer<T, U>.SegmentSort segmentSort = new SortJobDefer<T, U>.SegmentSort
			{
				DataRO = this.Data,
				Data = this.Data.m_ListData,
				Comp = this.Comp,
				SegmentWidth = 1024
			};
			JobHandle jobHandle = (ref segmentSort).ScheduleByRef<SortJobDefer<T, U>.SegmentSort, T>(this.Data, 1024, inputDeps);
			return new SortJobDefer<T, U>.SegmentSortMerge
			{
				Data = this.Data,
				Comp = this.Comp,
				SegmentWidth = 1024
			}.Schedule(jobHandle);
		}

		public NativeList<T> Data;

		public U Comp;

		[BurstCompile]
		public struct SegmentSort : IJobParallelForDefer
		{
			public unsafe void Execute(int index)
			{
				int num = index * this.SegmentWidth;
				int num2 = ((this.Data->Length - num < this.SegmentWidth) ? (this.Data->Length - num) : this.SegmentWidth);
				NativeSortExtension.Sort<T, U>(this.Data->Ptr + (IntPtr)num * (IntPtr)sizeof(T) / (IntPtr)sizeof(T), num2, this.Comp);
			}

			[ReadOnly]
			internal NativeList<T> DataRO;

			[NativeDisableUnsafePtrRestriction]
			internal unsafe UnsafeList<T>* Data;

			internal U Comp;

			internal int SegmentWidth;
		}

		[BurstCompile]
		public struct SegmentSortMerge : IJob
		{
			public unsafe void Execute()
			{
				int length = this.Data.Length;
				T* unsafePtr = this.Data.GetUnsafePtr<T>();
				int num = (length + (this.SegmentWidth - 1)) / this.SegmentWidth;
				int* ptr;
				checked
				{
					ptr = stackalloc int[unchecked((UIntPtr)num) * 4];
				}
				T* ptr2 = (T*)Memory.Unmanaged.Allocate((long)(UnsafeUtility.SizeOf<T>() * length), 16, Allocator.Temp);
				for (int i = 0; i < length; i++)
				{
					int num2 = -1;
					T t = default(T);
					for (int j = 0; j < num; j++)
					{
						int num3 = j * this.SegmentWidth;
						int num4 = ptr[j];
						int num5 = ((length - num3 < this.SegmentWidth) ? (length - num3) : this.SegmentWidth);
						if (num4 != num5)
						{
							T t2 = unsafePtr[(IntPtr)(num3 + num4) * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
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
				UnsafeUtility.MemCpy((void*)unsafePtr, (void*)ptr2, (long)(UnsafeUtility.SizeOf<T>() * length));
			}

			[NativeDisableUnsafePtrRestriction]
			internal NativeList<T> Data;

			internal U Comp;

			internal int SegmentWidth;
		}
	}
}
