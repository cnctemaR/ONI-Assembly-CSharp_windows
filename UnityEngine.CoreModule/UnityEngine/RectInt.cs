using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	public struct RectInt : IEquatable<RectInt>, IFormattable
	{
		public int x
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.m_XMin;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_XMin = value;
			}
		}

		public int y
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.m_YMin;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_YMin = value;
			}
		}

		public readonly Vector2 center
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Vector2
				{
					x = (float)this.m_XMin + (float)this.m_Width * 0.5f,
					y = (float)this.m_YMin + (float)this.m_Height * 0.5f
				};
			}
		}

		public Vector2Int min
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return new Vector2Int(this.xMin, this.yMin);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.xMin = value.x;
				this.yMin = value.y;
			}
		}

		public Vector2Int max
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return new Vector2Int(this.xMax, this.yMax);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.xMax = value.x;
				this.yMax = value.y;
			}
		}

		public int width
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.m_Width;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Width = value;
			}
		}

		public int height
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.m_Height;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Height = value;
			}
		}

		public int xMin
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return Mathf.Min(this.m_XMin, this.m_XMin + this.m_Width);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				int xMax = this.xMax;
				this.m_XMin = value;
				this.m_Width = xMax - this.m_XMin;
			}
		}

		public int yMin
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return Mathf.Min(this.m_YMin, this.m_YMin + this.m_Height);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				int yMax = this.yMax;
				this.m_YMin = value;
				this.m_Height = yMax - this.m_YMin;
			}
		}

		public int xMax
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return Mathf.Max(this.m_XMin, this.m_XMin + this.m_Width);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Width = value - this.m_XMin;
			}
		}

		public int yMax
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return Mathf.Max(this.m_YMin, this.m_YMin + this.m_Height);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Height = value - this.m_YMin;
			}
		}

		public Vector2Int position
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return new Vector2Int(this.m_XMin, this.m_YMin);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_XMin = value.x;
				this.m_YMin = value.y;
			}
		}

		public Vector2Int size
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return new Vector2Int(this.m_Width, this.m_Height);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Width = value.x;
				this.m_Height = value.y;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetMinMax(Vector2Int minPosition, Vector2Int maxPosition)
		{
			this.min = minPosition;
			this.max = maxPosition;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetMinMax(in Vector2Int minPosition, in Vector2Int maxPosition)
		{
			this.min = minPosition;
			this.max = maxPosition;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RectInt(int xMin, int yMin, int width, int height)
		{
			this.m_XMin = xMin;
			this.m_YMin = yMin;
			this.m_Width = width;
			this.m_Height = height;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RectInt(Vector2Int position, Vector2Int size)
		{
			this.m_XMin = position.x;
			this.m_YMin = position.y;
			this.m_Width = size.x;
			this.m_Height = size.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RectInt(in Vector2Int position, in Vector2Int size)
		{
			this.m_XMin = position.x;
			this.m_YMin = position.y;
			this.m_Width = size.x;
			this.m_Height = size.y;
		}

		public static RectInt zero
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return RectInt.kZero;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ClampToBounds(RectInt bounds)
		{
			int xMin = bounds.xMin;
			int xMax = bounds.xMax;
			int yMin = bounds.yMin;
			int yMax = bounds.yMax;
			this.m_XMin = Math.Max(Math.Min(xMax, this.m_XMin), xMin);
			this.m_YMin = Math.Max(Math.Min(yMax, this.m_YMin), yMin);
			this.m_Width = Math.Min(xMax - this.m_XMin, this.m_Width);
			this.m_Height = Math.Min(yMax - this.m_YMin, this.m_Height);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ClampToBounds(in RectInt bounds)
		{
			int xMin = bounds.xMin;
			int xMax = bounds.xMax;
			int yMin = bounds.yMin;
			int yMax = bounds.yMax;
			this.m_XMin = Math.Max(Math.Min(xMax, this.m_XMin), xMin);
			this.m_YMin = Math.Max(Math.Min(yMax, this.m_YMin), yMin);
			this.m_Width = Math.Min(xMax - this.m_XMin, this.m_Width);
			this.m_Height = Math.Min(yMax - this.m_YMin, this.m_Height);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Contains(Vector2Int position)
		{
			int x = position.x;
			int y = position.y;
			return x >= this.xMin && y >= this.yMin && x < this.xMax && y < this.yMax;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Contains(in Vector2Int position)
		{
			int x = position.x;
			int y = position.y;
			return x >= this.xMin && y >= this.yMin && x < this.xMax && y < this.yMax;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Overlaps(RectInt other)
		{
			return other.xMin < this.xMax && other.xMax > this.xMin && other.yMin < this.yMax && other.yMax > this.yMin;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Overlaps(in RectInt other)
		{
			return other.xMin < this.xMax && other.xMax > this.xMin && other.yMin < this.yMax && other.yMax > this.yMin;
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
			return string.Format("(x:{0}, y:{1}, width:{2}, height:{3})", new object[]
			{
				this.x.ToString(format, formatProvider),
				this.y.ToString(format, formatProvider),
				this.width.ToString(format, formatProvider),
				this.height.ToString(format, formatProvider)
			});
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(RectInt lhs, RectInt rhs)
		{
			return !(lhs == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(RectInt lhs, RectInt rhs)
		{
			return lhs.m_XMin == rhs.m_XMin && lhs.m_YMin == rhs.m_YMin && lhs.m_Width == rhs.m_Width && lhs.m_Height == rhs.m_Height;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly int GetHashCode()
		{
			int hashCode = this.m_XMin.GetHashCode();
			int hashCode2 = this.m_YMin.GetHashCode();
			int hashCode3 = this.m_Width.GetHashCode();
			int hashCode4 = this.m_Height.GetHashCode();
			return hashCode ^ (hashCode2 << 4) ^ (hashCode2 >> 28) ^ (hashCode3 >> 4) ^ (hashCode3 << 28) ^ (hashCode4 >> 4) ^ (hashCode4 << 28);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly bool Equals(object other)
		{
			RectInt rectInt;
			bool flag;
			if (other is RectInt)
			{
				rectInt = (RectInt)other;
				flag = true;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			return flag2 && this.Equals(in rectInt);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(RectInt other)
		{
			return this.m_XMin == other.m_XMin && this.m_YMin == other.m_YMin && this.m_Width == other.m_Width && this.m_Height == other.m_Height;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(in RectInt other)
		{
			return this.m_XMin == other.m_XMin && this.m_YMin == other.m_YMin && this.m_Width == other.m_Width && this.m_Height == other.m_Height;
		}

		public readonly RectInt.PositionEnumerator allPositionsWithin
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				Vector2Int min = this.min;
				Vector2Int max = this.max;
				return new RectInt.PositionEnumerator(in min, in max);
			}
		}

		private int m_XMin;

		private int m_YMin;

		private int m_Width;

		private int m_Height;

		private static readonly RectInt kZero = new RectInt(0, 0, 0, 0);

		public struct PositionEnumerator : IEnumerator<Vector2Int>, IEnumerator, IDisposable
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public PositionEnumerator(in Vector2Int min, in Vector2Int max)
			{
				this._min = min;
				this._max = max;
				this._current = this._min;
				int x = this._current.x;
				this._current.x = x - 1;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public RectInt.PositionEnumerator GetEnumerator()
			{
				return this;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				bool flag = this._current.y >= this._max.y;
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
							return false;
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

			public readonly Vector2Int Current
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

			private readonly Vector2Int _min;

			private readonly Vector2Int _max;

			private Vector2Int _current;
		}
	}
}
