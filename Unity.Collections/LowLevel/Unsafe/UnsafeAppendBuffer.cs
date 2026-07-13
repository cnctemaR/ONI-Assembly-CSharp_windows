using System;
using System.Diagnostics;
using Unity.Collections.LowLevel.Unsafe.NotBurstCompatible;
using Unity.Jobs;
using Unity.Mathematics;

namespace Unity.Collections.LowLevel.Unsafe
{
	[BurstCompatible]
	public struct UnsafeAppendBuffer : INativeDisposable, IDisposable
	{
		public UnsafeAppendBuffer(int initialCapacity, int alignment, AllocatorManager.AllocatorHandle allocator)
		{
			this.Alignment = alignment;
			this.Allocator = allocator;
			this.Ptr = null;
			this.Length = 0;
			this.Capacity = 0;
			this.SetCapacity(initialCapacity);
		}

		public unsafe UnsafeAppendBuffer(void* ptr, int length)
		{
			this.Alignment = 0;
			this.Allocator = AllocatorManager.None;
			this.Ptr = (byte*)ptr;
			this.Length = 0;
			this.Capacity = length;
		}

		public bool IsEmpty
		{
			get
			{
				return this.Length == 0;
			}
		}

		public bool IsCreated
		{
			get
			{
				return this.Ptr != null;
			}
		}

		public void Dispose()
		{
			if (CollectionHelper.ShouldDeallocate(this.Allocator))
			{
				Memory.Unmanaged.Free<byte>(this.Ptr, this.Allocator);
				this.Allocator = AllocatorManager.Invalid;
			}
			this.Ptr = null;
			this.Length = 0;
			this.Capacity = 0;
		}

		[NotBurstCompatible]
		public unsafe JobHandle Dispose(JobHandle inputDeps)
		{
			if (CollectionHelper.ShouldDeallocate(this.Allocator))
			{
				JobHandle jobHandle = new UnsafeDisposeJob
				{
					Ptr = (void*)this.Ptr,
					Allocator = this.Allocator
				}.Schedule(inputDeps);
				this.Ptr = null;
				this.Allocator = AllocatorManager.Invalid;
				return jobHandle;
			}
			this.Ptr = null;
			return inputDeps;
		}

		public void Reset()
		{
			this.Length = 0;
		}

		public unsafe void SetCapacity(int capacity)
		{
			if (capacity <= this.Capacity)
			{
				return;
			}
			capacity = math.max(64, math.ceilpow2(capacity));
			byte* ptr = (byte*)Memory.Unmanaged.Allocate((long)capacity, this.Alignment, this.Allocator);
			if (this.Ptr != null)
			{
				UnsafeUtility.MemCpy((void*)ptr, (void*)this.Ptr, (long)this.Length);
				Memory.Unmanaged.Free<byte>(this.Ptr, this.Allocator);
			}
			this.Ptr = ptr;
			this.Capacity = capacity;
		}

		public void ResizeUninitialized(int length)
		{
			this.SetCapacity(length);
			this.Length = length;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe void Add<T>(T value) where T : struct
		{
			int num = UnsafeUtility.SizeOf<T>();
			this.SetCapacity(this.Length + num);
			UnsafeUtility.CopyStructureToPtr<T>(ref value, (void*)(this.Ptr + this.Length));
			this.Length += num;
		}

		public unsafe void Add(void* ptr, int structSize)
		{
			this.SetCapacity(this.Length + structSize);
			UnsafeUtility.MemCpy((void*)(this.Ptr + this.Length), ptr, (long)structSize);
			this.Length += structSize;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe void AddArray<T>(void* ptr, int length) where T : struct
		{
			this.Add<int>(length);
			if (length != 0)
			{
				this.Add(ptr, length * UnsafeUtility.SizeOf<T>());
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public void Add<T>(NativeArray<T> value) where T : struct
		{
			this.Add<int>(value.Length);
			this.Add(value.GetUnsafeReadOnlyPtr<T>(), UnsafeUtility.SizeOf<T>() * value.Length);
		}

		[NotBurstCompatible]
		[Obsolete("Please use `AddNBC` from `Unity.Collections.LowLevel.Unsafe.NotBurstCompatible` namespace instead. (RemovedAfter 2021-06-22)", false)]
		public void Add(string value)
		{
			(ref this).AddNBC(value);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe T Pop<T>() where T : struct
		{
			int num = UnsafeUtility.SizeOf<T>();
			byte* ptr = this.Ptr;
			long num2 = (long)this.Length;
			T t = UnsafeUtility.ReadArrayElement<T>((void*)((byte*)((byte*)ptr + num2) - (long)num), 0);
			this.Length -= num;
			return t;
		}

		public unsafe void Pop(void* ptr, int structSize)
		{
			long num = this.Ptr;
			long num2 = (long)this.Length;
			long num3 = num + num2 - (long)structSize;
			UnsafeUtility.MemCpy(ptr, num3, (long)structSize);
			this.Length -= structSize;
		}

		[NotBurstCompatible]
		[Obsolete("Please use `ToBytesNBC` from `Unity.Collections.LowLevel.Unsafe.NotBurstCompatible` namespace instead. (RemovedAfter 2021-06-22)", false)]
		public byte[] ToBytes()
		{
			return (ref this).ToBytesNBC();
		}

		public UnsafeAppendBuffer.Reader AsReader()
		{
			return new UnsafeAppendBuffer.Reader(ref this);
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckAlignment(int alignment)
		{
			int num = ((alignment == 0) ? 1 : 0);
			bool flag = ((alignment - 1) & alignment) == 0;
			if (num != 0 || !flag)
			{
				throw new ArgumentException(string.Format("Specified alignment must be non-zero positive power of two. Requested: {0}", alignment));
			}
		}

		[NativeDisableUnsafePtrRestriction]
		public unsafe byte* Ptr;

		public int Length;

		public int Capacity;

		public AllocatorManager.AllocatorHandle Allocator;

		public readonly int Alignment;

		[BurstCompatible]
		public struct Reader
		{
			public Reader(ref UnsafeAppendBuffer buffer)
			{
				this.Ptr = buffer.Ptr;
				this.Size = buffer.Length;
				this.Offset = 0;
			}

			public unsafe Reader(void* ptr, int length)
			{
				this.Ptr = (byte*)ptr;
				this.Size = length;
				this.Offset = 0;
			}

			public bool EndOfBuffer
			{
				get
				{
					return this.Offset == this.Size;
				}
			}

			[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
			public unsafe void ReadNext<T>(out T value) where T : struct
			{
				int num = UnsafeUtility.SizeOf<T>();
				UnsafeUtility.CopyPtrToStructure<T>((void*)(this.Ptr + this.Offset), out value);
				this.Offset += num;
			}

			[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
			public unsafe T ReadNext<T>() where T : struct
			{
				int num = UnsafeUtility.SizeOf<T>();
				T t = UnsafeUtility.ReadArrayElement<T>((void*)(this.Ptr + this.Offset), 0);
				this.Offset += num;
				return t;
			}

			public unsafe void* ReadNext(int structSize)
			{
				void* ptr = (void*)((IntPtr)((void*)this.Ptr) + this.Offset);
				this.Offset += structSize;
				return ptr;
			}

			[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
			public unsafe void ReadNext<T>(out NativeArray<T> value, AllocatorManager.AllocatorHandle allocator) where T : struct
			{
				int num = this.ReadNext<int>();
				value = CollectionHelper.CreateNativeArray<T>(num, allocator, NativeArrayOptions.ClearMemory);
				int num2 = num * UnsafeUtility.SizeOf<T>();
				if (num2 > 0)
				{
					void* ptr = this.ReadNext(num2);
					UnsafeUtility.MemCpy(value.GetUnsafePtr<T>(), ptr, (long)num2);
				}
			}

			[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
			public unsafe void* ReadNextArray<T>(out int length) where T : struct
			{
				length = this.ReadNext<int>();
				if (length != 0)
				{
					return this.ReadNext(length * UnsafeUtility.SizeOf<T>());
				}
				return null;
			}

			[NotBurstCompatible]
			[Obsolete("Please use `ReadNextNBC` from `Unity.Collections.LowLevel.Unsafe.NotBurstCompatible` namespace instead. (RemovedAfter 2021-06-22)", false)]
			public void ReadNext(out string value)
			{
				(ref this).ReadNextNBC(out value);
			}

			[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
			private void CheckBounds(int structSize)
			{
				if (this.Offset + structSize > this.Size)
				{
					throw new ArgumentException(string.Format("Requested value outside bounds of UnsafeAppendOnlyBuffer. Remaining bytes: {0} Requested: {1}", this.Size - this.Offset, structSize));
				}
			}

			public unsafe readonly byte* Ptr;

			public readonly int Size;

			public int Offset;
		}
	}
}
