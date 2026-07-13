using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Jobs;

namespace Unity.Collections.LowLevel.Unsafe
{
	[GenerateTestsForBurstCompatibility]
	[DebuggerDisplay("Length = {Length}, Capacity = {Capacity}, IsCreated = {IsCreated}, IsEmpty = {IsEmpty}")]
	public struct UnsafeText : INativeDisposable, IDisposable, IUTF8Bytes, INativeList<byte>, IIndexable<byte>
	{
		public unsafe UnsafeText(int capacity, AllocatorManager.AllocatorHandle allocator)
		{
			this.m_UntypedListData = default(UntypedUnsafeList);
			*(ref this).AsUnsafeListOfBytes() = new UnsafeList<byte>(capacity + 1, allocator, NativeArrayOptions.UninitializedMemory);
			this.Length = 0;
		}

		public readonly bool IsCreated
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.AsUnsafeListOfBytesRO().IsCreated;
			}
		}

		internal unsafe static UnsafeText* Alloc(AllocatorManager.AllocatorHandle allocator)
		{
			return (UnsafeText*)Memory.Unmanaged.Allocate((long)sizeof(UnsafeText), UnsafeUtility.AlignOf<UnsafeText>(), allocator);
		}

		internal unsafe static void Free(UnsafeText* data)
		{
			if (data == null)
			{
				throw new InvalidOperationException("UnsafeText has yet to be created or has been destroyed!");
			}
			AllocatorManager.AllocatorHandle allocator = data->m_UntypedListData.Allocator;
			data->Dispose();
			Memory.Unmanaged.Free<UnsafeText>(data, allocator);
		}

		public void Dispose()
		{
			(ref this).AsUnsafeListOfBytes().Dispose();
		}

		public JobHandle Dispose(JobHandle inputDeps)
		{
			return (ref this).AsUnsafeListOfBytes().Dispose(inputDeps);
		}

		public readonly bool IsEmpty
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return !this.IsCreated || this.Length == 0;
			}
		}

		public byte this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return UnsafeUtility.ReadArrayElement<byte>(this.m_UntypedListData.Ptr, index);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.AsUnsafeListOfBytesRO().Capacity - 1;
			}
			set
			{
				(ref this).AsUnsafeListOfBytes().SetCapacity(value + 1);
			}
		}

		public int Length
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.AsUnsafeListOfBytesRO().Length - 1;
			}
			set
			{
				(ref this).AsUnsafeListOfBytes().Resize(value + 1, NativeArrayOptions.UninitializedMemory);
				(ref this).AsUnsafeListOfBytes()[value] = 0;
			}
		}

		[ExcludeFromBurstCompatTesting("Returns managed string")]
		public override string ToString()
		{
			if (!this.IsCreated)
			{
				return "";
			}
			return (ref this).ConvertToString<UnsafeText>();
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
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
		[Conditional("UNITY_DOTS_DEBUG")]
		private void ThrowCopyError(CopyError error, string source)
		{
			throw new ArgumentException(string.Format("UnsafeText: {0} while copying \"{1}\"", error, source));
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
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
