using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Hierarchy
{
	internal struct NativeSparseArray<[global::System.Runtime.CompilerServices.IsUnmanaged] TKey, [global::System.Runtime.CompilerServices.IsUnmanaged] TValue> : IDisposable where TKey : struct, ValueType, IEquatable<TKey> where TValue : struct, ValueType
	{
		public bool IsCreated
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Ptr != null;
			}
		}

		public int Capacity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Capacity;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.Allocate(value);
			}
		}

		public int Count
		{
			get
			{
				return this.m_Count;
			}
		}

		public unsafe TValue this[in TKey key]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				int num = this.m_KeyIndex(in key);
				this.ThrowIfIndexOutOfRange(num);
				ref NativeSparseArray<TKey, TValue>.Pair ptr = ref this.m_Ptr[(IntPtr)num * (IntPtr)sizeof(NativeSparseArray<TKey, TValue>.Pair) / (IntPtr)sizeof(NativeSparseArray<TKey, TValue>.Pair)];
				bool flag = !this.m_KeyEqual(in ptr.Key, in key);
				if (flag)
				{
					TKey tkey = key;
					throw new KeyNotFoundException(tkey.ToString());
				}
				return ptr.Value;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				int num = this.m_KeyIndex(in key);
				this.ThrowIfIndexIsNegative(num);
				this.EnsureCapacity(num + 1, NativeSparseArrayResizePolicy.ExactSize);
				ref NativeSparseArray<TKey, TValue>.Pair ptr = ref this.m_Ptr[(IntPtr)num * (IntPtr)sizeof(NativeSparseArray<TKey, TValue>.Pair) / (IntPtr)sizeof(NativeSparseArray<TKey, TValue>.Pair)];
				NativeSparseArray<TKey, TValue>.KeyEqual keyEqual = this.m_KeyEqual;
				ref NativeSparseArray<TKey, TValue>.Pair ptr2 = ref ptr;
				TKey tkey = default(TKey);
				bool flag = keyEqual(in ptr2.Key, in tkey);
				if (flag)
				{
					this.m_Count++;
				}
				this.m_Ptr[(IntPtr)num * (IntPtr)sizeof(NativeSparseArray<TKey, TValue>.Pair) / (IntPtr)sizeof(NativeSparseArray<TKey, TValue>.Pair)] = new NativeSparseArray<TKey, TValue>.Pair(in key, in value);
			}
		}

		public NativeSparseArray(NativeSparseArray<TKey, TValue>.KeyIndex keyIndex, Allocator allocator)
		{
			this.m_Ptr = null;
			this.m_Capacity = 0;
			this.m_Count = 0;
			this.m_Allocator = allocator;
			this.m_InitValue = default(NativeSparseArray<TKey, TValue>.Pair);
			this.m_KeyIndex = keyIndex;
			this.m_KeyEqual = delegate(in TKey lhs, in TKey rhs)
			{
				TKey tkey = lhs;
				return tkey.Equals(rhs);
			};
		}

		public NativeSparseArray(NativeSparseArray<TKey, TValue>.KeyIndex keyIndex, NativeSparseArray<TKey, TValue>.KeyEqual keyEqual, Allocator allocator)
		{
			this.m_Ptr = null;
			this.m_Capacity = 0;
			this.m_Count = 0;
			this.m_Allocator = allocator;
			this.m_InitValue = default(NativeSparseArray<TKey, TValue>.Pair);
			this.m_KeyIndex = keyIndex;
			this.m_KeyEqual = keyEqual;
		}

		public NativeSparseArray(in TValue initValue, NativeSparseArray<TKey, TValue>.KeyIndex keyIndex, Allocator allocator)
		{
			this.m_Ptr = null;
			this.m_Capacity = 0;
			this.m_Count = 0;
			this.m_Allocator = allocator;
			TKey tkey = default(TKey);
			this.m_InitValue = new NativeSparseArray<TKey, TValue>.Pair(in tkey, in initValue);
			this.m_KeyIndex = keyIndex;
			this.m_KeyEqual = delegate(in TKey lhs, in TKey rhs)
			{
				TKey tkey2 = lhs;
				return tkey2.Equals(rhs);
			};
		}

		public NativeSparseArray(in TValue initValue, NativeSparseArray<TKey, TValue>.KeyIndex keyIndex, NativeSparseArray<TKey, TValue>.KeyEqual keyEqual, Allocator allocator)
		{
			this.m_Ptr = null;
			this.m_Capacity = 0;
			this.m_Count = 0;
			this.m_Allocator = allocator;
			TKey tkey = default(TKey);
			this.m_InitValue = new NativeSparseArray<TKey, TValue>.Pair(in tkey, in initValue);
			this.m_KeyIndex = keyIndex;
			this.m_KeyEqual = keyEqual;
		}

		public void Dispose()
		{
			this.Deallocate();
		}

		public void Reserve(int capacity)
		{
			this.EnsureCapacity(capacity, NativeSparseArrayResizePolicy.ExactSize);
		}

		public unsafe bool ContainsKey(in TKey key)
		{
			int num = this.m_KeyIndex(in key);
			this.ThrowIfIndexOutOfRange(num);
			ref NativeSparseArray<TKey, TValue>.Pair ptr = ref this.m_Ptr[(IntPtr)num * (IntPtr)sizeof(NativeSparseArray<TKey, TValue>.Pair) / (IntPtr)sizeof(NativeSparseArray<TKey, TValue>.Pair)];
			return this.m_KeyEqual(in ptr.Key, in key);
		}

		public unsafe void Add(in TKey key, in TValue value, NativeSparseArrayResizePolicy policy = NativeSparseArrayResizePolicy.ExactSize)
		{
			int num = this.m_KeyIndex(in key);
			this.ThrowIfIndexIsNegative(num);
			this.EnsureCapacity(num + 1, policy);
			ref NativeSparseArray<TKey, TValue>.Pair ptr = ref this.m_Ptr[(IntPtr)num * (IntPtr)sizeof(NativeSparseArray<TKey, TValue>.Pair) / (IntPtr)sizeof(NativeSparseArray<TKey, TValue>.Pair)];
			bool flag = this.m_KeyEqual(in ptr.Key, in key);
			if (flag)
			{
				throw new ArgumentException(string.Format("an element with the same key [{0}] already exists", key));
			}
			NativeSparseArray<TKey, TValue>.KeyEqual keyEqual = this.m_KeyEqual;
			ref NativeSparseArray<TKey, TValue>.Pair ptr2 = ref ptr;
			TKey tkey = default(TKey);
			bool flag2 = keyEqual(in ptr2.Key, in tkey);
			if (flag2)
			{
				this.m_Count++;
			}
			this.m_Ptr[(IntPtr)num * (IntPtr)sizeof(NativeSparseArray<TKey, TValue>.Pair) / (IntPtr)sizeof(NativeSparseArray<TKey, TValue>.Pair)] = new NativeSparseArray<TKey, TValue>.Pair(in key, in value);
		}

		public unsafe void AddNoResize(in TKey key, in TValue value)
		{
			int num = this.m_KeyIndex(in key);
			this.ThrowIfIndexOutOfRange(num);
			ref NativeSparseArray<TKey, TValue>.Pair ptr = ref this.m_Ptr[(IntPtr)num * (IntPtr)sizeof(NativeSparseArray<TKey, TValue>.Pair) / (IntPtr)sizeof(NativeSparseArray<TKey, TValue>.Pair)];
			bool flag = this.m_KeyEqual(in ptr.Key, in key);
			if (flag)
			{
				throw new ArgumentException(string.Format("an element with the same key [{0}] already exists", key));
			}
			NativeSparseArray<TKey, TValue>.KeyEqual keyEqual = this.m_KeyEqual;
			ref NativeSparseArray<TKey, TValue>.Pair ptr2 = ref ptr;
			TKey tkey = default(TKey);
			bool flag2 = keyEqual(in ptr2.Key, in tkey);
			if (flag2)
			{
				this.m_Count++;
			}
			this.m_Ptr[(IntPtr)num * (IntPtr)sizeof(NativeSparseArray<TKey, TValue>.Pair) / (IntPtr)sizeof(NativeSparseArray<TKey, TValue>.Pair)] = new NativeSparseArray<TKey, TValue>.Pair(in key, in value);
		}

		public unsafe bool TryAdd(in TKey key, in TValue value, NativeSparseArrayResizePolicy policy = NativeSparseArrayResizePolicy.ExactSize)
		{
			int num = this.m_KeyIndex(in key);
			this.ThrowIfIndexIsNegative(num);
			this.EnsureCapacity(num + 1, policy);
			ref NativeSparseArray<TKey, TValue>.Pair ptr = ref this.m_Ptr[(IntPtr)num * (IntPtr)sizeof(NativeSparseArray<TKey, TValue>.Pair) / (IntPtr)sizeof(NativeSparseArray<TKey, TValue>.Pair)];
			bool flag = this.m_KeyEqual(in ptr.Key, in key);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				NativeSparseArray<TKey, TValue>.KeyEqual keyEqual = this.m_KeyEqual;
				ref NativeSparseArray<TKey, TValue>.Pair ptr2 = ref ptr;
				TKey tkey = default(TKey);
				bool flag3 = keyEqual(in ptr2.Key, in tkey);
				if (flag3)
				{
					this.m_Count++;
				}
				this.m_Ptr[(IntPtr)num * (IntPtr)sizeof(NativeSparseArray<TKey, TValue>.Pair) / (IntPtr)sizeof(NativeSparseArray<TKey, TValue>.Pair)] = new NativeSparseArray<TKey, TValue>.Pair(in key, in value);
				flag2 = true;
			}
			return flag2;
		}

		public unsafe bool TryAddNoResize(in TKey key, in TValue value)
		{
			int num = this.m_KeyIndex(in key);
			this.ThrowIfIndexOutOfRange(num);
			ref NativeSparseArray<TKey, TValue>.Pair ptr = ref this.m_Ptr[(IntPtr)num * (IntPtr)sizeof(NativeSparseArray<TKey, TValue>.Pair) / (IntPtr)sizeof(NativeSparseArray<TKey, TValue>.Pair)];
			bool flag = this.m_KeyEqual(in ptr.Key, in key);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				NativeSparseArray<TKey, TValue>.KeyEqual keyEqual = this.m_KeyEqual;
				ref NativeSparseArray<TKey, TValue>.Pair ptr2 = ref ptr;
				TKey tkey = default(TKey);
				bool flag3 = keyEqual(in ptr2.Key, in tkey);
				if (flag3)
				{
					this.m_Count++;
				}
				this.m_Ptr[(IntPtr)num * (IntPtr)sizeof(NativeSparseArray<TKey, TValue>.Pair) / (IntPtr)sizeof(NativeSparseArray<TKey, TValue>.Pair)] = new NativeSparseArray<TKey, TValue>.Pair(in key, in value);
				flag2 = true;
			}
			return flag2;
		}

		public unsafe bool TryGetValue(in TKey key, out TValue value)
		{
			int num = this.m_KeyIndex(in key);
			this.ThrowIfIndexOutOfRange(num);
			ref NativeSparseArray<TKey, TValue>.Pair ptr = ref this.m_Ptr[(IntPtr)num * (IntPtr)sizeof(NativeSparseArray<TKey, TValue>.Pair) / (IntPtr)sizeof(NativeSparseArray<TKey, TValue>.Pair)];
			bool flag = this.m_KeyEqual(in ptr.Key, in key);
			bool flag2;
			if (flag)
			{
				value = ptr.Value;
				flag2 = true;
			}
			else
			{
				value = default(TValue);
				flag2 = false;
			}
			return flag2;
		}

		public unsafe bool Remove(in TKey key)
		{
			int num = this.m_KeyIndex(in key);
			this.ThrowIfIndexOutOfRange(num);
			ref NativeSparseArray<TKey, TValue>.Pair ptr = ref this.m_Ptr[(IntPtr)num * (IntPtr)sizeof(NativeSparseArray<TKey, TValue>.Pair) / (IntPtr)sizeof(NativeSparseArray<TKey, TValue>.Pair)];
			bool flag = !this.m_KeyEqual(in ptr.Key, in key);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				this.m_Ptr[(IntPtr)num * (IntPtr)sizeof(NativeSparseArray<TKey, TValue>.Pair) / (IntPtr)sizeof(NativeSparseArray<TKey, TValue>.Pair)] = this.m_InitValue;
				this.m_Count--;
				flag2 = true;
			}
			return flag2;
		}

		public unsafe void Clear()
		{
			bool flag = this.m_Ptr != null;
			if (flag)
			{
				fixed (NativeSparseArray<TKey, TValue>.Pair* ptr = &this.m_InitValue)
				{
					void* ptr2 = (void*)ptr;
					UnsafeUtility.MemCpyReplicate((void*)this.m_Ptr, ptr2, sizeof(NativeSparseArray<TKey, TValue>.Pair), this.m_Capacity);
				}
			}
			this.m_Count = 0;
		}

		private unsafe void Allocate(int capacity)
		{
			bool flag = capacity < 0;
			if (flag)
			{
				throw new ArgumentException(string.Format("capacity [{0}] cannot be negative", capacity));
			}
			int num = sizeof(NativeSparseArray<TKey, TValue>.Pair);
			int num2 = UnsafeUtility.AlignOf<NativeSparseArray<TKey, TValue>.Pair>();
			bool flag2 = this.m_Ptr == null;
			if (flag2)
			{
				this.m_Ptr = (NativeSparseArray<TKey, TValue>.Pair*)UnsafeUtility.Malloc((long)(capacity * num), num2, this.m_Allocator);
				fixed (NativeSparseArray<TKey, TValue>.Pair* ptr = &this.m_InitValue)
				{
					NativeSparseArray<TKey, TValue>.Pair* ptr2 = ptr;
					UnsafeUtility.MemCpyReplicate((void*)this.m_Ptr, (void*)ptr2, num, capacity);
				}
			}
			else
			{
				this.m_Ptr = (NativeSparseArray<TKey, TValue>.Pair*)NativeSparseArray<TKey, TValue>.Realloc((void*)this.m_Ptr, (long)(capacity * num), num2, this.m_Allocator);
				bool flag3 = capacity > this.m_Capacity;
				if (flag3)
				{
					fixed (NativeSparseArray<TKey, TValue>.Pair* ptr3 = &this.m_InitValue)
					{
						NativeSparseArray<TKey, TValue>.Pair* ptr4 = ptr3;
						UnsafeUtility.MemCpyReplicate((void*)(this.m_Ptr + (IntPtr)this.m_Capacity * (IntPtr)sizeof(NativeSparseArray<TKey, TValue>.Pair) / (IntPtr)sizeof(NativeSparseArray<TKey, TValue>.Pair)), (void*)ptr4, num, capacity - this.m_Capacity);
					}
				}
			}
			this.m_Capacity = capacity;
		}

		private unsafe void Deallocate()
		{
			bool flag = this.m_Ptr != null;
			if (flag)
			{
				UnsafeUtility.Free((void*)this.m_Ptr, this.m_Allocator);
				this.m_Ptr = null;
			}
			this.m_Capacity = 0;
			this.m_Count = 0;
		}

		private void EnsureCapacity(int capacity, NativeSparseArrayResizePolicy policy)
		{
			bool flag = capacity <= this.m_Capacity;
			if (!flag)
			{
				NativeSparseArrayResizePolicy nativeSparseArrayResizePolicy = policy;
				NativeSparseArrayResizePolicy nativeSparseArrayResizePolicy2 = nativeSparseArrayResizePolicy;
				if (nativeSparseArrayResizePolicy2 != NativeSparseArrayResizePolicy.ExactSize)
				{
					if (nativeSparseArrayResizePolicy2 != NativeSparseArrayResizePolicy.DoubleSize)
					{
						throw new NotImplementedException(policy.ToString());
					}
					this.Allocate(Math.Max(capacity, this.m_Capacity * 2));
				}
				else
				{
					this.Allocate(capacity);
				}
			}
		}

		private void ThrowIfIndexIsNegative(int index)
		{
			bool flag = index < 0;
			if (flag)
			{
				throw new InvalidOperationException(string.Format("key index [{0}] cannot be negative", index));
			}
		}

		private void ThrowIfIndexOutOfRange(int index)
		{
			this.ThrowIfIndexIsNegative(index);
			bool flag = index >= this.m_Capacity;
			if (flag)
			{
				throw new InvalidOperationException(string.Format("key index [{0}] is out of range [0, {1}]", index, this.m_Capacity));
			}
		}

		private unsafe static void* Realloc(void* ptr, long size, int alignment, Allocator allocator)
		{
			bool flag = ptr == null;
			void* ptr2;
			if (flag)
			{
				ptr2 = UnsafeUtility.Malloc(size, alignment, allocator);
			}
			else
			{
				void* ptr3 = UnsafeUtility.Malloc(size, alignment, allocator);
				UnsafeUtility.MemCpy(ptr3, ptr, size);
				UnsafeUtility.Free(ptr, allocator);
				ptr2 = ptr3;
			}
			return ptr2;
		}

		private unsafe NativeSparseArray<TKey, TValue>.Pair* m_Ptr;

		private int m_Capacity;

		private int m_Count;

		private readonly Allocator m_Allocator;

		private readonly NativeSparseArray<TKey, TValue>.Pair m_InitValue;

		private readonly NativeSparseArray<TKey, TValue>.KeyIndex m_KeyIndex;

		private readonly NativeSparseArray<TKey, TValue>.KeyEqual m_KeyEqual;

		private readonly struct Pair
		{
			public Pair(in TKey key, in TValue value)
			{
				this.Key = key;
				this.Value = value;
			}

			public readonly TKey Key;

			public readonly TValue Value;
		}

		public delegate int KeyIndex(in TKey key);

		public delegate bool KeyEqual(in TKey lhs, in TKey rhs);
	}
}
