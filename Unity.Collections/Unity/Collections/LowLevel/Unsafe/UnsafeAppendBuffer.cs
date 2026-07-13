using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Jobs;
using Unity.Mathematics;

namespace Unity.Collections.LowLevel.Unsafe
{
	[GenerateTestsForBurstCompatibility]
	public struct UnsafeAppendBuffer : INativeDisposable, IDisposable
	{
		public UnsafeAppendBuffer(int initialCapacity, int alignment, AllocatorManager.AllocatorHandle allocator)
		{
			this.Alignment = alignment;
			this.Allocator = allocator;
			this.Ptr = null;
			this.Length = 0;
			this.Capacity = 0;
			this.SetCapacity(math.max(initialCapacity, 1));
		}

		public unsafe UnsafeAppendBuffer(void* ptr, int length)
		{
			this.Alignment = 0;
			this.Allocator = AllocatorManager.None;
			this.Ptr = (byte*)ptr;
			this.Length = 0;
			this.Capacity = length;
		}

		public readonly bool IsEmpty
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.Length == 0;
			}
		}

		public readonly bool IsCreated
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.Ptr != null;
			}
		}

		public void Dispose()
		{
			if (!this.IsCreated)
			{
				return;
			}
			if (CollectionHelper.ShouldDeallocate(this.Allocator))
			{
				Memory.Unmanaged.Free<byte>(this.Ptr, this.Allocator);
				this.Allocator = AllocatorManager.Invalid;
			}
			this.Ptr = null;
			this.Length = 0;
			this.Capacity = 0;
		}

		public unsafe JobHandle Dispose(JobHandle inputDeps)
		{
			if (!this.IsCreated)
			{
				return inputDeps;
			}
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

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe void Add<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(T value) where T : struct, ValueType
		{
			int num = UnsafeUtility.SizeOf<T>();
			this.SetCapacity(this.Length + num);
			void* ptr = (void*)(this.Ptr + this.Length);
			if (CollectionHelper.IsAligned(ptr, UnsafeUtility.AlignOf<T>()))
			{
				UnsafeUtility.CopyStructureToPtr<T>(ref value, ptr);
			}
			else
			{
				UnsafeUtility.MemCpy(ptr, (void*)(&value), (long)num);
			}
			this.Length += num;
		}

		public unsafe void Add(void* ptr, int structSize)
		{
			this.SetCapacity(this.Length + structSize);
			UnsafeUtility.MemCpy((void*)(this.Ptr + this.Length), ptr, (long)structSize);
			this.Length += structSize;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe void AddArray<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(void* ptr, int length) where T : struct, ValueType
		{
			this.Add<int>(length);
			if (length != 0)
			{
				this.Add(ptr, length * UnsafeUtility.SizeOf<T>());
			}
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public void Add<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(NativeArray<T> value) where T : struct, ValueType
		{
			this.Add<int>(value.Length);
			this.Add(value.GetUnsafeReadOnlyPtr<T>(), UnsafeUtility.SizeOf<T>() * value.Length);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe T Pop<[global::System.Runtime.CompilerServices.IsUnmanaged] T>() where T : struct, ValueType
		{
			int num = UnsafeUtility.SizeOf<T>();
			long num2 = this.Ptr;
			long num3 = (long)this.Length;
			long num4 = num2 + num3 - (long)num;
			T t;
			if (CollectionHelper.IsAligned((ulong)num4, UnsafeUtility.AlignOf<T>()))
			{
				t = UnsafeUtility.ReadArrayElement<T>(num4, 0);
			}
			else
			{
				UnsafeUtility.MemCpy((void*)(&t), num4, (long)num);
			}
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

		public UnsafeAppendBuffer.Reader AsReader()
		{
			return new UnsafeAppendBuffer.Reader(ref this);
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
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

		[GenerateTestsForBurstCompatibility]
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

			[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
			public unsafe void ReadNext<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(out T value) where T : struct, ValueType
			{
				int num = UnsafeUtility.SizeOf<T>();
				void* ptr = (void*)(this.Ptr + this.Offset);
				if (CollectionHelper.IsAligned(ptr, UnsafeUtility.AlignOf<T>()))
				{
					UnsafeUtility.CopyPtrToStructure<T>(ptr, out value);
				}
				else
				{
					fixed (T* ptr2 = &value)
					{
						void* ptr3 = (void*)ptr2;
						UnsafeUtility.MemCpy(ptr3, ptr, (long)num);
					}
				}
				this.Offset += num;
			}

			[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
			public unsafe T ReadNext<[global::System.Runtime.CompilerServices.IsUnmanaged] T>() where T : struct, ValueType
			{
				int num = UnsafeUtility.SizeOf<T>();
				void* ptr = (void*)(this.Ptr + this.Offset);
				T t;
				if (CollectionHelper.IsAligned(ptr, UnsafeUtility.AlignOf<T>()))
				{
					t = UnsafeUtility.ReadArrayElement<T>(ptr, 0);
				}
				else
				{
					UnsafeUtility.MemCpy((void*)(&t), ptr, (long)num);
				}
				this.Offset += num;
				return t;
			}

			public unsafe void* ReadNext(int structSize)
			{
				void* ptr = (void*)((IntPtr)((void*)this.Ptr) + this.Offset);
				this.Offset += structSize;
				return ptr;
			}

			[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
			public unsafe void ReadNext<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(out NativeArray<T> value, AllocatorManager.AllocatorHandle allocator) where T : struct, ValueType
			{
				int num = this.ReadNext<int>();
				value = CollectionHelper.CreateNativeArray<T>(num, allocator, NativeArrayOptions.UninitializedMemory);
				int num2 = num * UnsafeUtility.SizeOf<T>();
				if (num2 > 0)
				{
					void* ptr = this.ReadNext(num2);
					UnsafeUtility.MemCpy(value.GetUnsafePtr<T>(), ptr, (long)num2);
				}
			}

			[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
			public unsafe void* ReadNextArray<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(out int length) where T : struct, ValueType
			{
				length = this.ReadNext<int>();
				if (length != 0)
				{
					return this.ReadNext(length * UnsafeUtility.SizeOf<T>());
				}
				return null;
			}

			[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
			[Conditional("UNITY_DOTS_DEBUG")]
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
