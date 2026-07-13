using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.Collections
{
	[NativeContainer]
	[DebuggerDisplay("Length = {Length}, IsCreated = {IsCreated}")]
	[BurstCompatible]
	public struct NativeBitArray : INativeDisposable, IDisposable
	{
		public NativeBitArray(int numBits, AllocatorManager.AllocatorHandle allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory)
		{
			this = new NativeBitArray(numBits, allocator, options, 2);
		}

		private NativeBitArray(int numBits, AllocatorManager.AllocatorHandle allocator, NativeArrayOptions options, int disposeSentinelStackDepth)
		{
			this.m_BitArray = new UnsafeBitArray(numBits, allocator, options);
		}

		public bool IsCreated
		{
			get
			{
				return this.m_BitArray.IsCreated;
			}
		}

		public void Dispose()
		{
			this.m_BitArray.Dispose();
		}

		[NotBurstCompatible]
		public JobHandle Dispose(JobHandle inputDeps)
		{
			return this.m_BitArray.Dispose(inputDeps);
		}

		public int Length
		{
			get
			{
				return CollectionHelper.AssumePositive(this.m_BitArray.Length);
			}
		}

		public void Clear()
		{
			this.m_BitArray.Clear();
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe NativeArray<T> AsNativeArray<[global::System.Runtime.CompilerServices.IsUnmanaged] T>() where T : struct, ValueType
		{
			int num = UnsafeUtility.SizeOf<T>() * 8;
			int num2 = this.m_BitArray.Length / num;
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)this.m_BitArray.Ptr, num2, Allocator.None);
		}

		public void Set(int pos, bool value)
		{
			this.m_BitArray.Set(pos, value);
		}

		public void SetBits(int pos, bool value, int numBits)
		{
			this.m_BitArray.SetBits(pos, value, numBits);
		}

		public void SetBits(int pos, ulong value, int numBits = 1)
		{
			this.m_BitArray.SetBits(pos, value, numBits);
		}

		public ulong GetBits(int pos, int numBits = 1)
		{
			return this.m_BitArray.GetBits(pos, numBits);
		}

		public bool IsSet(int pos)
		{
			return this.m_BitArray.IsSet(pos);
		}

		public void Copy(int dstPos, int srcPos, int numBits)
		{
			this.m_BitArray.Copy(dstPos, srcPos, numBits);
		}

		public void Copy(int dstPos, ref NativeBitArray srcBitArray, int srcPos, int numBits)
		{
			this.m_BitArray.Copy(dstPos, ref srcBitArray.m_BitArray, srcPos, numBits);
		}

		public int Find(int pos, int numBits)
		{
			return this.m_BitArray.Find(pos, numBits);
		}

		public int Find(int pos, int count, int numBits)
		{
			return this.m_BitArray.Find(pos, count, numBits);
		}

		public bool TestNone(int pos, int numBits = 1)
		{
			return this.m_BitArray.TestNone(pos, numBits);
		}

		public bool TestAny(int pos, int numBits = 1)
		{
			return this.m_BitArray.TestAny(pos, numBits);
		}

		public bool TestAll(int pos, int numBits = 1)
		{
			return this.m_BitArray.TestAll(pos, numBits);
		}

		public int CountBits(int pos, int numBits = 1)
		{
			return this.m_BitArray.CountBits(pos, numBits);
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckRead()
		{
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckReadBounds<[global::System.Runtime.CompilerServices.IsUnmanaged] T>() where T : struct, ValueType
		{
			int num = UnsafeUtility.SizeOf<T>() * 8;
			int num2 = this.m_BitArray.Length / num;
			if (num2 == 0)
			{
				throw new InvalidOperationException(string.Format("Number of bits in the NativeBitArray {0} is not sufficient to cast to NativeArray<T> {1}.", this.m_BitArray.Length, UnsafeUtility.SizeOf<T>() * 8));
			}
			if (this.m_BitArray.Length != num * num2)
			{
				throw new InvalidOperationException(string.Format("Number of bits in the NativeBitArray {0} couldn't hold multiple of T {1}. Output array would be truncated.", this.m_BitArray.Length, UnsafeUtility.SizeOf<T>()));
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckWrite()
		{
		}

		[NativeDisableUnsafePtrRestriction]
		internal UnsafeBitArray m_BitArray;
	}
}
