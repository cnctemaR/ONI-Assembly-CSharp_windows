using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Unity.Collections
{
	[GenerateTestsForBurstCompatibility(RequiredUnityDefine = "UNITY_2020_2_OR_NEWER", GenericTypeArguments = new Type[]
	{
		typeof(int),
		typeof(NativeSortExtension.DefaultComparer<int>)
	})]
	public struct SortJob<[global::System.Runtime.CompilerServices.IsUnmanaged] T, U> where T : struct, ValueType where U : IComparer<T>
	{
		public JobHandle Schedule(JobHandle inputDeps = default(JobHandle))
		{
			if (this.Length == 0)
			{
				return inputDeps;
			}
			int num = (this.Length + 1023) / 1024;
			int threadIndexCount = JobsUtility.ThreadIndexCount;
			int num2 = math.max(1, threadIndexCount);
			int num3 = num / num2;
			JobHandle jobHandle = new SortJob<T, U>.SegmentSort
			{
				Data = this.Data,
				Comp = this.Comp,
				Length = this.Length,
				SegmentWidth = 1024
			}.Schedule(num, num3, inputDeps);
			return new SortJob<T, U>.SegmentSortMerge
			{
				Data = this.Data,
				Comp = this.Comp,
				Length = this.Length,
				SegmentWidth = 1024
			}.Schedule(jobHandle);
		}

		public unsafe T* Data;

		public U Comp;

		public int Length;

		[BurstCompile]
		public struct SegmentSort : IJobParallelFor
		{
			public void Execute(int index)
			{
				int num = index * this.SegmentWidth;
				int num2 = ((this.Length - num < this.SegmentWidth) ? (this.Length - num) : this.SegmentWidth);
				NativeSortExtension.Sort<T, U>(this.Data + (IntPtr)num * (IntPtr)sizeof(T) / (IntPtr)sizeof(T), num2, this.Comp);
			}

			[NativeDisableUnsafePtrRestriction]
			internal unsafe T* Data;

			internal U Comp;

			internal int Length;

			internal int SegmentWidth;
		}

		[BurstCompile]
		public struct SegmentSortMerge : IJob
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
			internal unsafe T* Data;

			internal U Comp;

			internal int Length;

			internal int SegmentWidth;
		}
	}
}
