using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	public struct BoundsInt : IEquatable<BoundsInt>, IFormattable
	{
		public int x
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.m_Position.x;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Position.x = value;
			}
		}

		public int y
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.m_Position.y;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Position.y = value;
			}
		}

		public int z
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.m_Position.z;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Position.z = value;
			}
		}

		public readonly Vector3 center
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Vector3
				{
					x = (float)this.m_Position.x + (float)this.m_Size.x * 0.5f,
					y = (float)this.m_Position.y + (float)this.m_Size.y * 0.5f,
					z = (float)this.m_Position.z + (float)this.m_Size.z * 0.5f
				};
			}
		}

		public Vector3Int min
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return new Vector3Int(this.xMin, this.yMin, this.zMin);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.xMin = value.x;
				this.yMin = value.y;
				this.zMin = value.z;
			}
		}

		public Vector3Int max
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return new Vector3Int(this.xMax, this.yMax, this.zMax);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.xMax = value.x;
				this.yMax = value.y;
				this.zMax = value.z;
			}
		}

		public int xMin
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return Math.Min(this.m_Position.x, this.m_Position.x + this.m_Size.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				int xMax = this.xMax;
				this.m_Position.x = value;
				this.m_Size.x = xMax - this.m_Position.x;
			}
		}

		public int yMin
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return Math.Min(this.m_Position.y, this.m_Position.y + this.m_Size.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				int yMax = this.yMax;
				this.m_Position.y = value;
				this.m_Size.y = yMax - this.m_Position.y;
			}
		}

		public int zMin
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return Math.Min(this.m_Position.z, this.m_Position.z + this.m_Size.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				int zMax = this.zMax;
				this.m_Position.z = value;
				this.m_Size.z = zMax - this.m_Position.z;
			}
		}

		public int xMax
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return Math.Max(this.m_Position.x, this.m_Position.x + this.m_Size.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Size.x = value - this.m_Position.x;
			}
		}

		public int yMax
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return Math.Max(this.m_Position.y, this.m_Position.y + this.m_Size.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Size.y = value - this.m_Position.y;
			}
		}

		public int zMax
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return Math.Max(this.m_Position.z, this.m_Position.z + this.m_Size.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Size.z = value - this.m_Position.z;
			}
		}

		public Vector3Int position
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.m_Position;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Position = value;
			}
		}

		public Vector3Int size
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.m_Size;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Size = value;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BoundsInt(int xMin, int yMin, int zMin, int sizeX, int sizeY, int sizeZ)
		{
			this.m_Position = new Vector3Int(xMin, yMin, zMin);
			this.m_Size = new Vector3Int(sizeX, sizeY, sizeZ);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BoundsInt(Vector3Int position, Vector3Int size)
		{
			this.m_Position = position;
			this.m_Size = size;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BoundsInt(in Vector3Int position, in Vector3Int size)
		{
			this.m_Position = position;
			this.m_Size = size;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetMinMax(Vector3Int minPosition, Vector3Int maxPosition)
		{
			this.xMin = minPosition.x;
			this.yMin = minPosition.y;
			this.zMin = minPosition.z;
			this.xMax = maxPosition.x;
			this.yMax = maxPosition.y;
			this.zMax = maxPosition.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetMinMax(in Vector3Int minPosition, in Vector3Int maxPosition)
		{
			this.xMin = minPosition.x;
			this.yMin = minPosition.y;
			this.zMin = minPosition.z;
			this.xMax = maxPosition.x;
			this.yMax = maxPosition.y;
			this.zMax = maxPosition.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ClampToBounds(BoundsInt bounds)
		{
			this.m_Position.x = Math.Max(Math.Min(bounds.xMax, this.m_Position.x), bounds.xMin);
			this.m_Position.y = Math.Max(Math.Min(bounds.yMax, this.m_Position.y), bounds.yMin);
			this.m_Position.z = Math.Max(Math.Min(bounds.zMax, this.m_Position.z), bounds.zMin);
			this.m_Size.x = Math.Min(bounds.xMax - this.m_Position.x, this.m_Size.x);
			this.m_Size.y = Math.Min(bounds.yMax - this.m_Position.y, this.m_Size.y);
			this.m_Size.z = Math.Min(bounds.zMax - this.m_Position.z, this.m_Size.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ClampToBounds(in BoundsInt bounds)
		{
			this.m_Position.x = Math.Max(Math.Min(bounds.xMax, this.m_Position.x), bounds.xMin);
			this.m_Position.y = Math.Max(Math.Min(bounds.yMax, this.m_Position.y), bounds.yMin);
			this.m_Position.z = Math.Max(Math.Min(bounds.zMax, this.m_Position.z), bounds.zMin);
			this.m_Size.x = Math.Min(bounds.xMax - this.m_Position.x, this.m_Size.x);
			this.m_Size.y = Math.Min(bounds.yMax - this.m_Position.y, this.m_Size.y);
			this.m_Size.z = Math.Min(bounds.zMax - this.m_Position.z, this.m_Size.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains(Vector3Int position)
		{
			return position.x >= this.xMin && position.y >= this.yMin && position.z >= this.zMin && position.x < this.xMax && position.y < this.yMax && position.z < this.zMax;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains(in Vector3Int position)
		{
			return position.x >= this.xMin && position.y >= this.yMin && position.z >= this.zMin && position.x < this.xMax && position.y < this.yMax && position.z < this.zMax;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly string ToString()
		{
			return this.ToString(null, null);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly string ToString(string format)
		{
			return this.ToString(format, null);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly string ToString(string format, IFormatProvider formatProvider)
		{
			bool flag = formatProvider == null;
			if (flag)
			{
				formatProvider = CultureInfo.InvariantCulture.NumberFormat;
			}
			return string.Format("Position: {0}, Size: {1}", this.m_Position.ToString(format, formatProvider), this.m_Size.ToString(format, formatProvider));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(BoundsInt lhs, BoundsInt rhs)
		{
			return lhs.m_Position == rhs.m_Position && lhs.m_Size == rhs.m_Size;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(BoundsInt lhs, BoundsInt rhs)
		{
			return !(lhs == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly bool Equals(object other)
		{
			BoundsInt boundsInt;
			bool flag;
			if (other is BoundsInt)
			{
				boundsInt = (BoundsInt)other;
				flag = true;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			return flag2 && this.Equals(in boundsInt);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(BoundsInt other)
		{
			return this.m_Position.Equals(in other.m_Position) && this.m_Size.Equals(in other.m_Size);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(in BoundsInt other)
		{
			return this.m_Position.Equals(in other.m_Position) && this.m_Size.Equals(in other.m_Size);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly int GetHashCode()
		{
			return this.m_Position.GetHashCode() ^ (this.m_Size.GetHashCode() << 2);
		}

		public readonly BoundsInt.PositionEnumerator allPositionsWithin
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				Vector3Int min = this.min;
				Vector3Int max = this.max;
				return new BoundsInt.PositionEnumerator(in min, in max);
			}
		}

		private Vector3Int m_Position;

		private Vector3Int m_Size;

		public struct PositionEnumerator : IEnumerator<Vector3Int>, IEnumerator, IDisposable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public PositionEnumerator(in Vector3Int min, in Vector3Int max)
			{
				this._min = min;
				this._max = max;
				this._current = this._min;
				int x = this._current.x;
				this._current.x = x - 1;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public BoundsInt.PositionEnumerator GetEnumerator()
			{
				return this;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				bool flag = this._current.z >= this._max.z || this._current.y >= this._max.y;
				bool flag2;
				if (flag)
				{
					flag2 = false;
				}
				else
				{
					int num = this._current.x;
					this._current.x = num + 1;
					bool flag3 = this._current.x >= this._max.x;
					if (flag3)
					{
						this._current.x = this._min.x;
						bool flag4 = this._current.x >= this._max.x;
						if (flag4)
						{
							return false;
						}
						num = this._current.y;
						this._current.y = num + 1;
						bool flag5 = this._current.y >= this._max.y;
						if (flag5)
						{
							this._current.y = this._min.y;
							num = this._current.z;
							this._current.z = num + 1;
							bool flag6 = this._current.z >= this._max.z;
							if (flag6)
							{
								return false;
							}
						}
					}
					flag2 = true;
				}
				return flag2;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Reset()
			{
				this._current = this._min;
				int x = this._current.x;
				this._current.x = x - 1;
			}

			public readonly Vector3Int Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this._current;
				}
			}

			object IEnumerator.Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this.Current;
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void IDisposable.Dispose()
			{
			}

			private readonly Vector3Int _min;

			private readonly Vector3Int _max;

			private Vector3Int _current;
		}
	}
}
