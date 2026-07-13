using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.Collections
{
	[NativeContainer]
	[DebuggerDisplay("Length = {Length}, IsCreated = {IsCreated}")]
	[GenerateTestsForBurstCompatibility]
	public struct NativeBitArray : INativeDisposable, IDisposable
	{
		public unsafe NativeBitArray(int numBits, AllocatorManager.AllocatorHandle allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory)
		{
			this.m_BitArray = UnsafeBitArray.Alloc(allocator);
			this.m_Allocator = allocator;
			*this.m_BitArray = new UnsafeBitArray(numBits, allocator, options);
		}

		public unsafe readonly bool IsCreated
		{
			get
			{
				return this.m_BitArray != null && this.m_BitArray->IsCreated;
			}
		}

		public readonly bool IsEmpty
		{
			get
			{
				return !this.IsCreated || this.Length == 0;
			}
		}

		public unsafe void Resize(int numBits, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
		{
			this.m_BitArray->Resize(numBits, options);
		}

		public unsafe void SetCapacity(int capacityInBits)
		{
			this.m_BitArray->SetCapacity(capacityInBits);
		}

		public unsafe void TrimExcess()
		{
			this.m_BitArray->TrimExcess();
		}

		public void Dispose()
		{
			if (!this.IsCreated)
			{
				return;
			}
			UnsafeBitArray.Free(this.m_BitArray, this.m_Allocator);
			this.m_BitArray = null;
			this.m_Allocator = AllocatorManager.Invalid;
		}

		public JobHandle Dispose(JobHandle inputDeps)
		{
			if (!this.IsCreated)
			{
				return inputDeps;
			}
			JobHandle jobHandle = new NativeBitArrayDisposeJob
			{
				Data = new NativeBitArrayDispose
				{
					m_BitArrayData = this.m_BitArray,
					m_Allocator = this.m_Allocator
				}
			}.Schedule(inputDeps);
			this.m_BitArray = null;
			this.m_Allocator = AllocatorManager.Invalid;
			return jobHandle;
		}

		public unsafe readonly int Length
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return CollectionHelper.AssumePositive(this.m_BitArray->Length);
			}
		}

		public unsafe readonly int Capacity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return CollectionHelper.AssumePositive(this.m_BitArray->Capacity);
			}
		}

		public unsafe void Clear()
		{
			this.m_BitArray->Clear();
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe NativeArray<T> AsNativeArray<[global::System.Runtime.CompilerServices.IsUnmanaged] T>() where T : struct, ValueType
		{
			int num = UnsafeUtility.SizeOf<T>() * 8;
			int num2 = this.m_BitArray->Length / num;
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)this.m_BitArray->Ptr, num2, Allocator.None);
		}

		public unsafe void Set(int pos, bool value)
		{
			this.m_BitArray->Set(pos, value);
		}

		public unsafe void SetBits(int pos, bool value, int numBits)
		{
			this.m_BitArray->SetBits(pos, value, numBits);
		}

		public unsafe void SetBits(int pos, ulong value, int numBits = 1)
		{
			this.m_BitArray->SetBits(pos, value, numBits);
		}

		public unsafe ulong GetBits(int pos, int numBits = 1)
		{
			return this.m_BitArray->GetBits(pos, numBits);
		}

		public unsafe bool IsSet(int pos)
		{
			return this.m_BitArray->IsSet(pos);
		}

		public unsafe void Copy(int dstPos, int srcPos, int numBits)
		{
			this.m_BitArray->Copy(dstPos, srcPos, numBits);
		}

		public unsafe void Copy(int dstPos, ref NativeBitArray srcBitArray, int srcPos, int numBits)
		{
			this.m_BitArray->Copy(dstPos, ref *srcBitArray.m_BitArray, srcPos, numBits);
		}

		public unsafe int Find(int pos, int numBits)
		{
			return this.m_BitArray->Find(pos, numBits);
		}

		public unsafe int Find(int pos, int count, int numBits)
		{
			return this.m_BitArray->Find(pos, count, numBits);
		}

		public unsafe bool TestNone(int pos, int numBits = 1)
		{
			return this.m_BitArray->TestNone(pos, numBits);
		}

		public unsafe bool TestAny(int pos, int numBits = 1)
		{
			return this.m_BitArray->TestAny(pos, numBits);
		}

		public unsafe bool TestAll(int pos, int numBits = 1)
		{
			return this.m_BitArray->TestAll(pos, numBits);
		}

		public unsafe int CountBits(int pos, int numBits = 1)
		{
			return this.m_BitArray->CountBits(pos, numBits);
		}

		public NativeBitArray.ReadOnly AsReadOnly()
		{
			return new NativeBitArray.ReadOnly(ref this);
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private readonly void CheckRead()
		{
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		private unsafe void CheckReadBounds<[global::System.Runtime.CompilerServices.IsUnmanaged] T>() where T : struct, ValueType
		{
			int num = UnsafeUtility.SizeOf<T>() * 8;
			int num2 = this.m_BitArray->Length / num;
			if (num2 == 0)
			{
				throw new InvalidOperationException(string.Format("Number of bits in the NativeBitArray {0} is not sufficient to cast to NativeArray<T> {1}.", this.m_BitArray->Length, UnsafeUtility.SizeOf<T>() * 8));
			}
			if (this.m_BitArray->Length != num * num2)
			{
				throw new InvalidOperationException(string.Format("Number of bits in the NativeBitArray {0} couldn't hold multiple of T {1}. Output array would be truncated.", this.m_BitArray->Length, UnsafeUtility.SizeOf<T>()));
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckWrite()
		{
		}

		[NativeDisableUnsafePtrRestriction]
		internal unsafe UnsafeBitArray* m_BitArray;

		internal AllocatorManager.AllocatorHandle m_Allocator;

		[NativeContainer]
		[NativeContainerIsReadOnly]
		public struct ReadOnly
		{
			public readonly bool IsCreated
			{
				get
				{
					return this.m_BitArray.IsCreated;
				}
			}

			public readonly bool IsEmpty
			{
				get
				{
					return this.m_BitArray.IsEmpty;
				}
			}

			internal unsafe ReadOnly(ref NativeBitArray data)
			{
				this.m_BitArray = data.m_BitArray->AsReadOnly();
			}

			public readonly int Length
			{
				get
				{
					return CollectionHelper.AssumePositive(this.m_BitArray.Length);
				}
			}

			public readonly ulong GetBits(int pos, int numBits = 1)
			{
				return this.m_BitArray.GetBits(pos, numBits);
			}

			public readonly bool IsSet(int pos)
			{
				return this.m_BitArray.IsSet(pos);
			}

			public readonly int Find(int pos, int numBits)
			{
				return this.m_BitArray.Find(pos, numBits);
			}

			public readonly int Find(int pos, int count, int numBits)
			{
				return this.m_BitArray.Find(pos, count, numBits);
			}

			public readonly bool TestNone(int pos, int numBits = 1)
			{
				return this.m_BitArray.TestNone(pos, numBits);
			}

			public readonly bool TestAny(int pos, int numBits = 1)
			{
				return this.m_BitArray.TestAny(pos, numBits);
			}

			public readonly bool TestAll(int pos, int numBits = 1)
			{
				return this.m_BitArray.TestAll(pos, numBits);
			}

			public readonly int CountBits(int pos, int numBits = 1)
			{
				return this.m_BitArray.CountBits(pos, numBits);
			}

			[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
			private readonly void CheckRead()
			{
			}

			[NativeDisableUnsafePtrRestriction]
			internal UnsafeBitArray.ReadOnly m_BitArray;
		}
	}
}
