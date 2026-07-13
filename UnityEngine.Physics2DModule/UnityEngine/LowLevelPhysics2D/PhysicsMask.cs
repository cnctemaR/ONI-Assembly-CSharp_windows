using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UnityEngine.LowLevelPhysics2D
{
	[Serializable]
	public struct PhysicsMask : IEnumerable<int>, IEnumerable
	{
		public PhysicsMask(params int[] bitIndicies)
		{
			ulong num = 0UL;
			foreach (int num2 in bitIndicies)
			{
				bool flag = num2 >= 0 && num2 <= 63;
				if (!flag)
				{
					throw new ArgumentOutOfRangeException("bitIndex", string.Format("Bit index is out of range [0, 63]: {0}.", num2));
				}
				num |= 1UL << num2;
			}
			this.bitMask = num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public PhysicsMask(LayerMask layerMask)
		{
			this.bitMask = (ulong)((long)layerMask.value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly LayerMask ToLayerMask()
		{
			return (int)((uint)this.bitMask);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetBit(int bitIndex)
		{
			bool flag = bitIndex >= 0 && bitIndex <= 63;
			if (flag)
			{
				this.bitMask |= 1UL << bitIndex;
				return;
			}
			throw new ArgumentOutOfRangeException("bitIndex", string.Format("Bit index is out of range (0 to 63): {0}.", bitIndex));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ResetBit(int bitIndex)
		{
			bool flag = bitIndex >= 0 && bitIndex <= 63;
			if (flag)
			{
				this.bitMask &= ~(1UL << bitIndex);
				return;
			}
			throw new ArgumentOutOfRangeException("bitIndex", string.Format("Bit index is out of range [0, 63]: {0}.", bitIndex));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool IsBitSet(int bitIndex)
		{
			bool flag = bitIndex >= 0 && bitIndex <= 63;
			if (flag)
			{
				return (this.bitMask & (1UL << bitIndex)) > 0UL;
			}
			throw new ArgumentOutOfRangeException("bitIndex", string.Format("Bit index is out of range [0, 63]: {0}.", bitIndex));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool AreBitsSet(PhysicsMask physicsMask)
		{
			return (this.bitMask & physicsMask) == physicsMask;
		}

		public readonly PhysicsMask.ResetBitIterator resetBits
		{
			get
			{
				return new PhysicsMask.ResetBitIterator(this);
			}
		}

		public readonly PhysicsMask.SetBitIterator setBits
		{
			get
			{
				return new PhysicsMask.SetBitIterator(this);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator PhysicsMask(ulong value)
		{
			return new PhysicsMask
			{
				bitMask = value
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator ulong(PhysicsMask bitMask)
		{
			return bitMask.bitMask;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static PhysicsMask operator |(PhysicsMask bitMaskA, PhysicsMask bitMaskB)
		{
			return bitMaskA.bitMask | bitMaskB.bitMask;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static PhysicsMask operator &(PhysicsMask bitMaskA, PhysicsMask bitMaskB)
		{
			return bitMaskA.bitMask & bitMaskB.bitMask;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static PhysicsMask operator ^(PhysicsMask bitMaskA, PhysicsMask bitMaskB)
		{
			return bitMaskA.bitMask ^ bitMaskB.bitMask;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static PhysicsMask operator ~(PhysicsMask bitMask)
		{
			return ~bitMask.bitMask;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static PhysicsMask operator <<(PhysicsMask bitMask, int bitShift)
		{
			return bitMask.bitMask << bitShift;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static PhysicsMask operator >>(PhysicsMask bitMask, int bitShift)
		{
			return bitMask.bitMask >> bitShift;
		}

		public readonly IEnumerator<int> GetEnumerator()
		{
			return new PhysicsMask.SetBitIterator(this);
		}

		readonly IEnumerator IEnumerable.GetEnumerator()
		{
			return new PhysicsMask.SetBitIterator(this);
		}

		public override readonly string ToString()
		{
			return string.Format("bitMask={0}", this.bitMask);
		}

		public ulong bitMask;

		public static readonly PhysicsMask None = default(PhysicsMask);

		public static readonly PhysicsMask One = new PhysicsMask
		{
			bitMask = 1UL
		};

		public static readonly PhysicsMask All = new PhysicsMask
		{
			bitMask = ulong.MaxValue
		};

		public struct ResetBitIterator : IEnumerable<int>, IEnumerable, IEnumerator<int>, IEnumerator, IDisposable
		{
			public ResetBitIterator(PhysicsMask bitMask)
			{
				this.m_BitIndex = -1;
				this.bitMask = bitMask;
			}

			int IEnumerator<int>.Current
			{
				get
				{
					return this.m_BitIndex;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.m_BitIndex;
				}
			}

			bool IEnumerator.MoveNext()
			{
				bool flag = this.m_BitIndex >= 63;
				bool flag2;
				if (flag)
				{
					flag2 = false;
				}
				else
				{
					bool flag3;
					do
					{
						int num = this.m_BitIndex + 1;
						this.m_BitIndex = num;
						if (num >= 64)
						{
							break;
						}
						flag3 = (this.bitMask & (1UL << this.m_BitIndex)) == 0UL;
					}
					while (!flag3);
					flag2 = this.m_BitIndex < 64;
				}
				return flag2;
			}

			void IEnumerator.Reset()
			{
				this.m_BitIndex = -1;
			}

			public IEnumerator<int> GetEnumerator()
			{
				return this;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			readonly void IDisposable.Dispose()
			{
			}

			private int m_BitIndex;

			private ulong bitMask;
		}

		public struct SetBitIterator : IEnumerable<int>, IEnumerable, IEnumerator<int>, IEnumerator, IDisposable
		{
			public SetBitIterator(PhysicsMask bitMask)
			{
				this.m_BitIndex = -1;
				this.bitMask = bitMask;
			}

			int IEnumerator<int>.Current
			{
				get
				{
					return this.m_BitIndex;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.m_BitIndex;
				}
			}

			bool IEnumerator.MoveNext()
			{
				bool flag = this.bitMask == 0UL || this.m_BitIndex >= 63;
				bool flag2;
				if (flag)
				{
					flag2 = false;
				}
				else
				{
					bool flag3;
					do
					{
						int num = this.m_BitIndex + 1;
						this.m_BitIndex = num;
						if (num >= 64)
						{
							break;
						}
						flag3 = (this.bitMask & (1UL << this.m_BitIndex)) > 0UL;
					}
					while (!flag3);
					flag2 = this.m_BitIndex < 64;
				}
				return flag2;
			}

			void IEnumerator.Reset()
			{
				this.m_BitIndex = -1;
			}

			public IEnumerator<int> GetEnumerator()
			{
				return this;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			readonly void IDisposable.Dispose()
			{
			}

			private int m_BitIndex;

			private ulong bitMask;
		}

		[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
		public class ShowAsPhysicsMaskAttribute : PropertyAttribute
		{
			public ShowAsPhysicsMaskAttribute()
				: base(true)
			{
			}
		}
	}
}
