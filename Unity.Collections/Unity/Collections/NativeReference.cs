using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.Collections
{
	[NativeContainer]
	[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
	public struct NativeReference<[global::System.Runtime.CompilerServices.IsUnmanaged] T> : INativeDisposable, IDisposable, IEquatable<NativeReference<T>> where T : struct, ValueType
	{
		public NativeReference(AllocatorManager.AllocatorHandle allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory)
		{
			NativeReference<T>.Allocate(allocator, out this);
			if (options == NativeArrayOptions.ClearMemory)
			{
				UnsafeUtility.MemClear(this.m_Data, (long)UnsafeUtility.SizeOf<T>());
			}
		}

		public unsafe NativeReference(T value, AllocatorManager.AllocatorHandle allocator)
		{
			NativeReference<T>.Allocate(allocator, out this);
			*(T*)this.m_Data = value;
		}

		private static void Allocate(AllocatorManager.AllocatorHandle allocator, out NativeReference<T> reference)
		{
			reference = default(NativeReference<T>);
			reference.m_Data = Memory.Unmanaged.Allocate((long)UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), allocator);
			reference.m_AllocatorLabel = allocator;
		}

		public unsafe T Value
		{
			get
			{
				return *(T*)this.m_Data;
			}
			set
			{
				*(T*)this.m_Data = value;
			}
		}

		public readonly bool IsCreated
		{
			get
			{
				return this.m_Data != null;
			}
		}

		public void Dispose()
		{
			if (!this.IsCreated)
			{
				return;
			}
			if (CollectionHelper.ShouldDeallocate(this.m_AllocatorLabel))
			{
				Memory.Unmanaged.Free(this.m_Data, this.m_AllocatorLabel);
				this.m_AllocatorLabel = Allocator.Invalid;
			}
			this.m_Data = null;
		}

		public JobHandle Dispose(JobHandle inputDeps)
		{
			if (!this.IsCreated)
			{
				return inputDeps;
			}
			if (CollectionHelper.ShouldDeallocate(this.m_AllocatorLabel))
			{
				JobHandle jobHandle = new NativeReferenceDisposeJob
				{
					Data = new NativeReferenceDispose
					{
						m_Data = this.m_Data,
						m_AllocatorLabel = this.m_AllocatorLabel
					}
				}.Schedule(inputDeps);
				this.m_Data = null;
				this.m_AllocatorLabel = Allocator.Invalid;
				return jobHandle;
			}
			this.m_Data = null;
			return inputDeps;
		}

		public void CopyFrom(NativeReference<T> reference)
		{
			NativeReference<T>.Copy(this, reference);
		}

		public void CopyTo(NativeReference<T> reference)
		{
			NativeReference<T>.Copy(reference, this);
		}

		[ExcludeFromBurstCompatTesting("Equals boxes because Value does not implement IEquatable<T>")]
		public bool Equals(NativeReference<T> other)
		{
			T value = this.Value;
			return value.Equals(other.Value);
		}

		[ExcludeFromBurstCompatTesting("Takes managed object")]
		public override bool Equals(object obj)
		{
			return obj != null && obj is NativeReference<T> && this.Equals((NativeReference<T>)obj);
		}

		public override int GetHashCode()
		{
			T value = this.Value;
			return value.GetHashCode();
		}

		public static bool operator ==(NativeReference<T> left, NativeReference<T> right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(NativeReference<T> left, NativeReference<T> right)
		{
			return !left.Equals(right);
		}

		public static void Copy(NativeReference<T> dst, NativeReference<T> src)
		{
			UnsafeUtility.MemCpy(dst.m_Data, src.m_Data, (long)UnsafeUtility.SizeOf<T>());
		}

		public NativeReference<T>.ReadOnly AsReadOnly()
		{
			return new NativeReference<T>.ReadOnly(this.m_Data);
		}

		public static implicit operator NativeReference<T>.ReadOnly(NativeReference<T> nativeReference)
		{
			return nativeReference.AsReadOnly();
		}

		[NativeDisableUnsafePtrRestriction]
		internal unsafe void* m_Data;

		internal AllocatorManager.AllocatorHandle m_AllocatorLabel;

		[NativeContainer]
		[NativeContainerIsReadOnly]
		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public struct ReadOnly
		{
			internal unsafe ReadOnly(void* data)
			{
				this.m_Data = data;
			}

			public unsafe T Value
			{
				get
				{
					return *(T*)this.m_Data;
				}
			}

			[NativeDisableUnsafePtrRestriction]
			private unsafe readonly void* m_Data;
		}
	}
}
