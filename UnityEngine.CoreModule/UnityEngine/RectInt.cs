using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>A 2D Rectangle defined by x, y, width, height with integers.</para>
	/// </summary>
	[UsedByNativeCode]
	public struct RectInt
	{
		public RectInt(int xMin, int yMin, int width, int height)
		{
			this.m_XMin = xMin;
			this.m_YMin = yMin;
			this.m_Width = width;
			this.m_Height = height;
		}

		public RectInt(Vector2Int position, Vector2Int size)
		{
			this.m_XMin = position.x;
			this.m_YMin = position.y;
			this.m_Width = size.x;
			this.m_Height = size.y;
		}

		/// <summary>
		///   <para>Left coordinate of the rectangle.</para>
		/// </summary>
		public int x
		{
			get
			{
				return this.m_XMin;
			}
			set
			{
				this.m_XMin = value;
			}
		}

		/// <summary>
		///   <para>Top coordinate of the rectangle.</para>
		/// </summary>
		public int y
		{
			get
			{
				return this.m_YMin;
			}
			set
			{
				this.m_YMin = value;
			}
		}

		/// <summary>
		///   <para>Center coordinate of the rectangle.</para>
		/// </summary>
		public Vector2 center
		{
			get
			{
				return new Vector2((float)this.x + (float)this.m_Width / 2f, (float)this.y + (float)this.m_Height / 2f);
			}
		}

		/// <summary>
		///   <para>Lower left corner of the rectangle.</para>
		/// </summary>
		public Vector2Int min
		{
			get
			{
				return new Vector2Int(this.xMin, this.yMin);
			}
			set
			{
				this.xMin = value.x;
				this.yMin = value.y;
			}
		}

		/// <summary>
		///   <para>Upper right corner of the rectangle.</para>
		/// </summary>
		public Vector2Int max
		{
			get
			{
				return new Vector2Int(this.xMax, this.yMax);
			}
			set
			{
				this.xMax = value.x;
				this.yMax = value.y;
			}
		}

		/// <summary>
		///   <para>Width of the rectangle.</para>
		/// </summary>
		public int width
		{
			get
			{
				return this.m_Width;
			}
			set
			{
				this.m_Width = value;
			}
		}

		/// <summary>
		///   <para>Height of the rectangle.</para>
		/// </summary>
		public int height
		{
			get
			{
				return this.m_Height;
			}
			set
			{
				this.m_Height = value;
			}
		}

		/// <summary>
		///   <para>Returns the minimum X value of the RectInt.</para>
		/// </summary>
		public int xMin
		{
			get
			{
				return Math.Min(this.m_XMin, this.m_XMin + this.m_Width);
			}
			set
			{
				int xMax = this.xMax;
				this.m_XMin = value;
				this.m_Width = xMax - this.m_XMin;
			}
		}

		/// <summary>
		///   <para>Returns the minimum Y value of the RectInt.</para>
		/// </summary>
		public int yMin
		{
			get
			{
				return Math.Min(this.m_YMin, this.m_YMin + this.m_Height);
			}
			set
			{
				int yMax = this.yMax;
				this.m_YMin = value;
				this.m_Height = yMax - this.m_YMin;
			}
		}

		/// <summary>
		///   <para>Returns the maximum X value of the RectInt.</para>
		/// </summary>
		public int xMax
		{
			get
			{
				return Math.Max(this.m_XMin, this.m_XMin + this.m_Width);
			}
			set
			{
				this.m_Width = value - this.m_XMin;
			}
		}

		/// <summary>
		///   <para>Returns the maximum Y value of the RectInt.</para>
		/// </summary>
		public int yMax
		{
			get
			{
				return Math.Max(this.m_YMin, this.m_YMin + this.m_Height);
			}
			set
			{
				this.m_Height = value - this.m_YMin;
			}
		}

		/// <summary>
		///   <para>Returns the position (x, y) of the RectInt.</para>
		/// </summary>
		public Vector2Int position
		{
			get
			{
				return new Vector2Int(this.m_XMin, this.m_YMin);
			}
			set
			{
				this.m_XMin = value.x;
				this.m_YMin = value.y;
			}
		}

		/// <summary>
		///   <para>Returns the width and height of the RectInt.</para>
		/// </summary>
		public Vector2Int size
		{
			get
			{
				return new Vector2Int(this.m_Width, this.m_Height);
			}
			set
			{
				this.m_Width = value.x;
				this.m_Height = value.y;
			}
		}

		/// <summary>
		///   <para>Sets the bounds to the min and max value of the rect.</para>
		/// </summary>
		/// <param name="minPosition"></param>
		/// <param name="maxPosition"></param>
		public void SetMinMax(Vector2Int minPosition, Vector2Int maxPosition)
		{
			this.min = minPosition;
			this.max = maxPosition;
		}

		/// <summary>
		///   <para>Clamps the position and size of the RectInt to the given bounds.</para>
		/// </summary>
		/// <param name="bounds">Bounds to clamp the RectInt.</param>
		public void ClampToBounds(RectInt bounds)
		{
			this.position = new Vector2Int(Math.Max(Math.Min(bounds.xMax, this.position.x), bounds.xMin), Math.Max(Math.Min(bounds.yMax, this.position.y), bounds.yMin));
			this.size = new Vector2Int(Math.Min(bounds.xMax - this.position.x, this.size.x), Math.Min(bounds.yMax - this.position.y, this.size.y));
		}

		/// <summary>
		///   <para>Returns true if the given position is within the RectInt.</para>
		/// </summary>
		/// <param name="position">Position to check.</param>
		/// <param name="inclusive">Whether the max limits are included in the check.</param>
		/// <returns>
		///   <para>Whether the position is within the RectInt.</para>
		/// </returns>
		public bool Contains(Vector2Int position)
		{
			return position.x >= this.m_XMin && position.y >= this.m_YMin && position.x < this.m_XMin + this.m_Width && position.y < this.m_YMin + this.m_Height;
		}

		/// <summary>
		///   <para>Returns the x, y, width and height of the RectInt.</para>
		/// </summary>
		public override string ToString()
		{
			return UnityString.Format("(x:{0}, y:{1}, width:{2}, height:{3})", new object[] { this.x, this.y, this.width, this.height });
		}

		/// <summary>
		///   <para>A RectInt.PositionCollection that contains all positions within the RectInt.</para>
		/// </summary>
		public RectInt.PositionEnumerator allPositionsWithin
		{
			get
			{
				return new RectInt.PositionEnumerator(this.min, this.max);
			}
		}

		private int m_XMin;

		private int m_YMin;

		private int m_Width;

		private int m_Height;

		/// <summary>
		///   <para>An iterator that allows you to iterate over all positions within the RectInt.</para>
		/// </summary>
		public struct PositionEnumerator : IEnumerator<Vector2Int>, IEnumerator, IDisposable
		{
			public PositionEnumerator(Vector2Int min, Vector2Int max)
			{
				this._current = min;
				this._min = min;
				this._max = max;
				this.Reset();
			}

			/// <summary>
			///   <para>Returns this as an iterator that allows you to iterate over all positions within the RectInt.</para>
			/// </summary>
			/// <returns>
			///   <para>This RectInt.PositionEnumerator.</para>
			/// </returns>
			public RectInt.PositionEnumerator GetEnumerator()
			{
				return this;
			}

			/// <summary>
			///   <para>Moves the enumerator to the next position.</para>
			/// </summary>
			/// <returns>
			///   <para>Whether the enumerator has successfully moved to the next position.</para>
			/// </returns>
			public bool MoveNext()
			{
				bool flag;
				if (this._current.y >= this._max.y)
				{
					flag = false;
				}
				else
				{
					this._current.x = this._current.x + 1;
					if (this._current.x >= this._max.x)
					{
						this._current.x = this._min.x;
						this._current.y = this._current.y + 1;
						if (this._current.y >= this._max.y)
						{
							return false;
						}
					}
					flag = true;
				}
				return flag;
			}

			/// <summary>
			///   <para>Resets this enumerator to its starting state.</para>
			/// </summary>
			public void Reset()
			{
				this._current = this._min;
				this._current.x = this._current.x - 1;
			}

			/// <summary>
			///   <para>Current position of the enumerator.</para>
			/// </summary>
			public Vector2Int Current
			{
				get
				{
					return this._current;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			void IDisposable.Dispose()
			{
			}

			private readonly Vector2Int _min;

			private readonly Vector2Int _max;

			private Vector2Int _current;
		}
	}
}
