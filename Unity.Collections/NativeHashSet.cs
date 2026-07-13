using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.Collections
{
	[DebuggerTypeProxy(typeof(NativeHashSetDebuggerTypeProxy<>))]
	[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
	public struct NativeHashSet<[global::System.Runtime.CompilerServices.IsUnmanaged] T> : INativeDisposable, IDisposable, IEnumerable<T>, IEnumerable where T : struct, ValueType, IEquatable<T>
	{
		public NativeHashSet(int capacity, AllocatorManager.AllocatorHandle allocator)
		{
			this.m_Data = new NativeHashMap<T, bool>(capacity, allocator);
		}

		public bool IsEmpty
		{
			get
			{
				return this.m_Data.IsEmpty;
			}
		}

		public int Count()
		{
			return this.m_Data.Count();
		}

		public int Capacity
		{
			get
			{
				return this.m_Data.Capacity;
			}
			set
			{
				this.m_Data.Capacity = value;
			}
		}

		public bool IsCreated
		{
			get
			{
				return this.m_Data.IsCreated;
			}
		}

		public void Dispose()
		{
			this.m_Data.Dispose();
		}

		[NotBurstCompatible]
		public JobHandle Dispose(JobHandle inputDeps)
		{
			return this.m_Data.Dispose(inputDeps);
		}

		public void Clear()
		{
			this.m_Data.Clear();
		}

		public bool Add(T item)
		{
			return this.m_Data.TryAdd(item, false);
		}

		public bool Remove(T item)
		{
			return this.m_Data.Remove(item);
		}

		public bool Contains(T item)
		{
			return this.m_Data.ContainsKey(item);
		}

		public NativeArray<T> ToNativeArray(AllocatorManager.AllocatorHandle allocator)
		{
			return this.m_Data.GetKeyArray(allocator);
		}

		public NativeHashSet<T>.ParallelWriter AsParallelWriter()
		{
			NativeHashSet<T>.ParallelWriter parallelWriter;
			parallelWriter.m_Data = this.m_Data.AsParallelWriter();
			return parallelWriter;
		}

		public NativeHashSet<T>.Enumerator GetEnumerator()
		{
			return new NativeHashSet<T>.Enumerator
			{
				m_Enumerator = new UnsafeHashMapDataEnumerator(this.m_Data.m_HashMapData.m_Buffer)
			};
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		internal NativeHashMap<T, bool> m_Data;

		[NativeContainerIsAtomicWriteOnly]
		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public struct ParallelWriter
		{
			public int Capacity
			{
				get
				{
					return this.m_Data.Capacity;
				}
			}

			public bool Add(T item)
			{
				return this.m_Data.TryAdd(item, false);
			}

			internal NativeHashMap<T, bool>.ParallelWriter m_Data;
		}

		[NativeContainer]
		[NativeContainerIsReadOnly]
		public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
		{
			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				return this.m_Enumerator.MoveNext();
			}

			public void Reset()
			{
				this.m_Enumerator.Reset();
			}

			public T Current
			{
				get
				{
					return this.m_Enumerator.GetCurrentKey<T>();
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			internal UnsafeHashMapDataEnumerator m_Enumerator;
		}
	}
}
