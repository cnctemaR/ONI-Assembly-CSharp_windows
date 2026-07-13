using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.Collections
{
	[DebuggerTypeProxy(typeof(NativeParallelHashSetDebuggerTypeProxy<>))]
	[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
	public struct NativeParallelHashSet<[global::System.Runtime.CompilerServices.IsUnmanaged] T> : INativeDisposable, IDisposable, IEnumerable<T>, IEnumerable where T : struct, ValueType, IEquatable<T>
	{
		public NativeParallelHashSet(int capacity, AllocatorManager.AllocatorHandle allocator)
		{
			this.m_Data = new NativeParallelHashMap<T, bool>(capacity, allocator);
		}

		public readonly bool IsEmpty
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.m_Data.Capacity;
			}
			set
			{
				this.m_Data.Capacity = value;
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
			this.m_Data.Dispose();
		}

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

		public NativeParallelHashSet<T>.ParallelWriter AsParallelWriter()
		{
			NativeParallelHashSet<T>.ParallelWriter parallelWriter;
			parallelWriter.m_Data = this.m_Data.AsParallelWriter();
			return parallelWriter;
		}

		public NativeParallelHashSet<T>.Enumerator GetEnumerator()
		{
			return new NativeParallelHashSet<T>.Enumerator
			{
				m_Enumerator = new UnsafeParallelHashMapDataEnumerator(this.m_Data.m_HashMapData.m_Buffer)
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

		public NativeParallelHashSet<T>.ReadOnly AsReadOnly()
		{
			return new NativeParallelHashSet<T>.ReadOnly(ref this);
		}

		internal NativeParallelHashMap<T, bool> m_Data;

		[NativeContainerIsAtomicWriteOnly]
		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public struct ParallelWriter
		{
			public readonly int Capacity
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this.m_Data.Capacity;
				}
			}

			public bool Add(T item)
			{
				return this.m_Data.TryAdd(item, false);
			}

			public bool Add(T item, int threadIndexOverride)
			{
				return this.m_Data.TryAdd(item, false, threadIndexOverride);
			}

			internal NativeParallelHashMap<T, bool>.ParallelWriter m_Data;
		}

		[NativeContainer]
		[NativeContainerIsReadOnly]
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

			public T Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
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

			internal UnsafeParallelHashMapDataEnumerator m_Enumerator;
		}

		[NativeContainer]
		[NativeContainerIsReadOnly]
		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public struct ReadOnly : IEnumerable<T>, IEnumerable
		{
			internal ReadOnly(ref NativeParallelHashSet<T> data)
			{
				this.m_Data = data.m_Data.m_HashMapData;
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
					return !this.IsCreated || this.m_Data.IsEmpty;
				}
			}

			public readonly int Count()
			{
				return this.m_Data.Count();
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
				UnsafeParallelHashMap<T, bool> data = this.m_Data;
				return data.ContainsKey(item);
			}

			public readonly NativeArray<T> ToNativeArray(AllocatorManager.AllocatorHandle allocator)
			{
				UnsafeParallelHashMap<T, bool> data = this.m_Data;
				return data.GetKeyArray(allocator);
			}

			public readonly NativeParallelHashSet<T>.Enumerator GetEnumerator()
			{
				return new NativeParallelHashSet<T>.Enumerator
				{
					m_Enumerator = new UnsafeParallelHashMapDataEnumerator(this.m_Data.m_Buffer)
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

			[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
			private readonly void CheckRead()
			{
			}

			internal UnsafeParallelHashMap<T, bool> m_Data;
		}
	}
}
