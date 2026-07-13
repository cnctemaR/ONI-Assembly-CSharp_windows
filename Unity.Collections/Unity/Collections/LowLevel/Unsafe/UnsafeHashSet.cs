using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Jobs;

namespace Unity.Collections.LowLevel.Unsafe
{
	[DebuggerTypeProxy(typeof(UnsafeHashSetDebuggerTypeProxy<>))]
	[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
	public struct UnsafeHashSet<[global::System.Runtime.CompilerServices.IsUnmanaged] T> : INativeDisposable, IDisposable, IEnumerable<T>, IEnumerable where T : struct, ValueType, IEquatable<T>
	{
		public UnsafeHashSet(int initialCapacity, AllocatorManager.AllocatorHandle allocator)
		{
			this.m_Data = default(HashMapHelper<T>);
			this.m_Data.Init(initialCapacity, 0, 256, allocator);
		}

		public readonly bool IsEmpty
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return !this.IsCreated || this.m_Data.IsEmpty;
			}
		}

		public readonly int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Data.Count;
			}
		}

		public int Capacity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.m_Data.Capacity;
			}
			set
			{
				this.m_Data.Resize(value);
			}
		}

		public readonly bool IsCreated
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Data.IsCreated;
			}
		}

		public void Dispose()
		{
			if (!this.IsCreated)
			{
				return;
			}
			this.m_Data.Dispose();
		}

		public unsafe JobHandle Dispose(JobHandle inputDeps)
		{
			if (!this.IsCreated)
			{
				return inputDeps;
			}
			JobHandle jobHandle = new UnsafeDisposeJob
			{
				Ptr = (void*)this.m_Data.Ptr,
				Allocator = this.m_Data.Allocator
			}.Schedule(inputDeps);
			this.m_Data.Ptr = null;
			return jobHandle;
		}

		public void Clear()
		{
			this.m_Data.Clear();
		}

		public bool Add(T item)
		{
			return -1 != this.m_Data.TryAdd(in item);
		}

		public bool Remove(T item)
		{
			return -1 != this.m_Data.TryRemove(item);
		}

		public bool Contains(T item)
		{
			return -1 != this.m_Data.Find(item);
		}

		public void TrimExcess()
		{
			this.m_Data.TrimExcess();
		}

		public NativeArray<T> ToNativeArray(AllocatorManager.AllocatorHandle allocator)
		{
			return this.m_Data.GetKeyArray(allocator);
		}

		public unsafe UnsafeHashSet<T>.Enumerator GetEnumerator()
		{
			fixed (HashMapHelper<T>* ptr = &this.m_Data)
			{
				HashMapHelper<T>* ptr2 = ptr;
				return new UnsafeHashSet<T>.Enumerator
				{
					m_Enumerator = new HashMapHelper<T>.Enumerator(ptr2)
				};
			}
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public UnsafeHashSet<T>.ReadOnly AsReadOnly()
		{
			return new UnsafeHashSet<T>.ReadOnly(ref this.m_Data);
		}

		internal HashMapHelper<T> m_Data;

		public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
		{
			public void Dispose()
			{
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				return this.m_Enumerator.MoveNext();
			}

			public void Reset()
			{
				this.m_Enumerator.Reset();
			}

			public unsafe T Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this.m_Enumerator.m_Data->Keys[(IntPtr)this.m_Enumerator.m_Index * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			internal HashMapHelper<T>.Enumerator m_Enumerator;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public struct ReadOnly : IEnumerable<T>, IEnumerable
		{
			internal ReadOnly(ref HashMapHelper<T> data)
			{
				this.m_Data = data;
			}

			public readonly bool IsCreated
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this.m_Data.IsCreated;
				}
			}

			public readonly bool IsEmpty
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this.m_Data.IsEmpty;
				}
			}

			public readonly int Count
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this.m_Data.Count;
				}
			}

			public readonly int Capacity
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this.m_Data.Capacity;
				}
			}

			public readonly bool Contains(T item)
			{
				int num = -1;
				HashMapHelper<T> data = this.m_Data;
				return num != data.Find(item);
			}

			public readonly NativeArray<T> ToNativeArray(AllocatorManager.AllocatorHandle allocator)
			{
				HashMapHelper<T> data = this.m_Data;
				return data.GetKeyArray(allocator);
			}

			public unsafe readonly UnsafeHashSet<T>.Enumerator GetEnumerator()
			{
				fixed (HashMapHelper<T>* ptr = &this.m_Data)
				{
					HashMapHelper<T>* ptr2 = ptr;
					return new UnsafeHashSet<T>.Enumerator
					{
						m_Enumerator = new HashMapHelper<T>.Enumerator(ptr2)
					};
				}
			}

			IEnumerator<T> IEnumerable<T>.GetEnumerator()
			{
				throw new NotImplementedException();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				throw new NotImplementedException();
			}

			internal HashMapHelper<T> m_Data;
		}
	}
}
