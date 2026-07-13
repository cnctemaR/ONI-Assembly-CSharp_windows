using System;
using System.Diagnostics;
using Unity.Jobs;

namespace Unity.Collections.LowLevel.Unsafe
{
	[BurstCompatible]
	[DebuggerDisplay("Length = {Length}, Capacity = {Capacity}, IsCreated = {IsCreated}, IsEmpty = {IsEmpty}")]
	public struct UnsafeText : INativeDisposable, IDisposable, IUTF8Bytes, INativeList<byte>, IIndexable<byte>
	{
		public unsafe UnsafeText(int capacity, AllocatorManager.AllocatorHandle allocator)
		{
			this.m_UntypedListData = default(UntypedUnsafeList);
			*(ref this).AsUnsafeListOfBytes() = new UnsafeList<byte>(capacity + 1, allocator, NativeArrayOptions.UninitializedMemory);
			this.Length = 0;
		}

		public bool IsCreated
		{
			get
			{
				return (ref this).AsUnsafeListOfBytes().IsCreated;
			}
		}

		public void Dispose()
		{
			(ref this).AsUnsafeListOfBytes().Dispose();
		}

		[NotBurstCompatible]
		public JobHandle Dispose(JobHandle inputDeps)
		{
			return (ref this).AsUnsafeListOfBytes().Dispose(inputDeps);
		}

		public bool IsEmpty
		{
			get
			{
				return !this.IsCreated || this.Length == 0;
			}
		}

		public byte this[int index]
		{
			get
			{
				return UnsafeUtility.ReadArrayElement<byte>(this.m_UntypedListData.Ptr, index);
			}
			set
			{
				UnsafeUtility.WriteArrayElement<byte>(this.m_UntypedListData.Ptr, index, value);
			}
		}

		public ref byte ElementAt(int index)
		{
			return UnsafeUtility.ArrayElementAsRef<byte>(this.m_UntypedListData.Ptr, index);
		}

		public void Clear()
		{
			this.Length = 0;
		}

		public unsafe byte* GetUnsafePtr()
		{
			return (byte*)this.m_UntypedListData.Ptr;
		}

		public bool TryResize(int newLength, NativeArrayOptions clearOptions = NativeArrayOptions.ClearMemory)
		{
			(ref this).AsUnsafeListOfBytes().Resize(newLength + 1, clearOptions);
			(ref this).AsUnsafeListOfBytes()[newLength] = 0;
			return true;
		}

		public int Capacity
		{
			get
			{
				return (ref this).AsUnsafeListOfBytes().Capacity - 1;
			}
			set
			{
				(ref this).AsUnsafeListOfBytes().SetCapacity(value + 1);
			}
		}

		public int Length
		{
			get
			{
				return (ref this).AsUnsafeListOfBytes().Length - 1;
			}
			set
			{
				(ref this).AsUnsafeListOfBytes().Resize(value + 1, NativeArrayOptions.UninitializedMemory);
				(ref this).AsUnsafeListOfBytes()[value] = 0;
			}
		}

		[NotBurstCompatible]
		public override string ToString()
		{
			if (!this.IsCreated)
			{
				return "";
			}
			return (ref this).ConvertToString<UnsafeText>();
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckIndexInRange(int index)
		{
			if (index < 0)
			{
				throw new IndexOutOfRangeException(string.Format("Index {0} must be positive.", index));
			}
			if (index >= this.Length)
			{
				throw new IndexOutOfRangeException(string.Format("Index {0} is out of range in UnsafeText of {1} length.", index, this.Length));
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void ThrowCopyError(CopyError error, string source)
		{
			throw new ArgumentException(string.Format("UnsafeText: {0} while copying \"{1}\"", error, source));
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckCapacityInRange(int value, int length)
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException(string.Format("Value {0} must be positive.", value));
			}
			if (value < length)
			{
				throw new ArgumentOutOfRangeException(string.Format("Value {0} is out of range in NativeList of '{1}' Length.", value, length));
			}
		}

		internal UntypedUnsafeList m_UntypedListData;
	}
}
