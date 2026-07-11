using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Represents an axis aligned bounding box with all values as integers.</para>
	/// </summary>
	[UsedByNativeCode]
	public struct BoundsInt : IEquatable<BoundsInt>
	{
		public BoundsInt(int xMin, int yMin, int zMin, int sizeX, int sizeY, int sizeZ)
		{
			this.m_Position = new Vector3Int(xMin, yMin, zMin);
			this.m_Size = new Vector3Int(sizeX, sizeY, sizeZ);
		}

		public BoundsInt(Vector3Int position, Vector3Int size)
		{
			this.m_Position = position;
			this.m_Size = size;
		}

		/// <summary>
		///   <para>X value of the minimal point of the box.</para>
		/// </summary>
		public int x
		{
			get
			{
				return this.m_Position.x;
			}
			set
			{
				this.m_Position.x = value;
			}
		}

		/// <summary>
		///   <para>Y value of the minimal point of the box.</para>
		/// </summary>
		public int y
		{
			get
			{
				return this.m_Position.y;
			}
			set
			{
				this.m_Position.y = value;
			}
		}

		/// <summary>
		///   <para>Z value of the minimal point of the box.</para>
		/// </summary>
		public int z
		{
			get
			{
				return this.m_Position.z;
			}
			set
			{
				this.m_Position.z = value;
			}
		}

		/// <summary>
		///   <para>The center of the bounding box.</para>
		/// </summary>
		public Vector3 center
		{
			get
			{
				return new Vector3((float)this.x + (float)this.m_Size.x / 2f, (float)this.y + (float)this.m_Size.y / 2f, (float)this.z + (float)this.m_Size.z / 2f);
			}
		}

		/// <summary>
		///   <para>The minimal point of the box.</para>
		/// </summary>
		public Vector3Int min
		{
			get
			{
				return new Vector3Int(this.xMin, this.yMin, this.zMin);
			}
			set
			{
				this.xMin = value.x;
				this.yMin = value.y;
				this.zMin = value.z;
			}
		}

		/// <summary>
		///   <para>The maximal point of the box.</para>
		/// </summary>
		public Vector3Int max
		{
			get
			{
				return new Vector3Int(this.xMax, this.yMax, this.zMax);
			}
			set
			{
				this.xMax = value.x;
				this.yMax = value.y;
				this.zMax = value.z;
			}
		}

		/// <summary>
		///   <para>The minimal x point of the box.</para>
		/// </summary>
		public int xMin
		{
			get
			{
				return Math.Min(this.m_Position.x, this.m_Position.x + this.m_Size.x);
			}
			set
			{
				int xMax = this.xMax;
				this.m_Position.x = value;
				this.m_Size.x = xMax - this.m_Position.x;
			}
		}

		/// <summary>
		///   <para>The minimal y point of the box.</para>
		/// </summary>
		public int yMin
		{
			get
			{
				return Math.Min(this.m_Position.y, this.m_Position.y + this.m_Size.y);
			}
			set
			{
				int yMax = this.yMax;
				this.m_Position.y = value;
				this.m_Size.y = yMax - this.m_Position.y;
			}
		}

		/// <summary>
		///   <para>The minimal z point of the box.</para>
		/// </summary>
		public int zMin
		{
			get
			{
				return Math.Min(this.m_Position.z, this.m_Position.z + this.m_Size.z);
			}
			set
			{
				int zMax = this.zMax;
				this.m_Position.z = value;
				this.m_Size.z = zMax - this.m_Position.z;
			}
		}

		/// <summary>
		///   <para>The maximal x point of the box.</para>
		/// </summary>
		public int xMax
		{
			get
			{
				return Math.Max(this.m_Position.x, this.m_Position.x + this.m_Size.x);
			}
			set
			{
				this.m_Size.x = value - this.m_Position.x;
			}
		}

		/// <summary>
		///   <para>The maximal y point of the box.</para>
		/// </summary>
		public int yMax
		{
			get
			{
				return Math.Max(this.m_Position.y, this.m_Position.y + this.m_Size.y);
			}
			set
			{
				this.m_Size.y = value - this.m_Position.y;
			}
		}

		/// <summary>
		///   <para>The maximal z point of the box.</para>
		/// </summary>
		public int zMax
		{
			get
			{
				return Math.Max(this.m_Position.z, this.m_Position.z + this.m_Size.z);
			}
			set
			{
				this.m_Size.z = value - this.m_Position.z;
			}
		}

		/// <summary>
		///   <para>The position of the bounding box.</para>
		/// </summary>
		public Vector3Int position
		{
			get
			{
				return this.m_Position;
			}
			set
			{
				this.m_Position = value;
			}
		}

		/// <summary>
		///   <para>The total size of the box.</para>
		/// </summary>
		public Vector3Int size
		{
			get
			{
				return this.m_Size;
			}
			set
			{
				this.m_Size = value;
			}
		}

		/// <summary>
		///   <para>Sets the bounds to the min and max value of the box.</para>
		/// </summary>
		/// <param name="minPosition"></param>
		/// <param name="maxPosition"></param>
		public void SetMinMax(Vector3Int minPosition, Vector3Int maxPosition)
		{
			this.min = minPosition;
			this.max = maxPosition;
		}

		/// <summary>
		///   <para>Clamps the position and size of this bounding box to the given bounds.</para>
		/// </summary>
		/// <param name="bounds">Bounds to clamp to.</param>
		public void ClampToBounds(BoundsInt bounds)
		{
			this.position = new Vector3Int(Math.Max(Math.Min(bounds.xMax, this.position.x), bounds.xMin), Math.Max(Math.Min(bounds.yMax, this.position.y), bounds.yMin), Math.Max(Math.Min(bounds.zMax, this.position.z), bounds.zMin));
			this.size = new Vector3Int(Math.Min(bounds.xMax - this.position.x, this.size.x), Math.Min(bounds.yMax - this.position.y, this.size.y), Math.Min(bounds.zMax - this.position.z, this.size.z));
		}

		/// <summary>
		///   <para>Is point contained in the bounding box?</para>
		/// </summary>
		/// <param name="position">Point to check.</param>
		/// <param name="inclusive">Whether the max limits are included in the check.</param>
		/// <returns>
		///   <para>Is point contained in the bounding box?</para>
		/// </returns>
		public bool Contains(Vector3Int position)
		{
			return position.x >= this.m_Position.x && position.y >= this.m_Position.y && position.z >= this.m_Position.z && position.x < this.m_Position.x + this.m_Size.x && position.y < this.m_Position.y + this.m_Size.y && position.z < this.m_Position.z + this.m_Size.z;
		}

		/// <summary>
		///   <para>Returns a nicely formatted string for the bounds.</para>
		/// </summary>
		public override string ToString()
		{
			return UnityString.Format("Position: {0}, Size: {1}", new object[] { this.m_Position, this.m_Size });
		}

		public static bool operator ==(BoundsInt lhs, BoundsInt rhs)
		{
			return lhs.m_Position == rhs.m_Position && lhs.m_Size == rhs.m_Size;
		}

		public static bool operator !=(BoundsInt lhs, BoundsInt rhs)
		{
			return !(lhs == rhs);
		}

		public override bool Equals(object other)
		{
			return other is BoundsInt && this.Equals((BoundsInt)other);
		}

		public bool Equals(BoundsInt other)
		{
			return this.m_Position.Equals(other.m_Position) && this.m_Size.Equals(other.m_Size);
		}

		public override int GetHashCode()
		{
			return this.m_Position.GetHashCode() ^ (this.m_Size.GetHashCode() << 2);
		}

		/// <summary>
		///   <para>A BoundsInt.PositionCollection that contains all positions within the BoundsInt.</para>
		/// </summary>
		public BoundsInt.PositionEnumerator allPositionsWithin
		{
			get
			{
				return new BoundsInt.PositionEnumerator(this.min, this.max);
			}
		}

		private Vector3Int m_Position;

		private Vector3Int m_Size;

		/// <summary>
		///   <para>An iterator that allows you to iterate over all positions within the BoundsInt.</para>
		/// </summary>
		public struct PositionEnumerator : IEnumerator<Vector3Int>, IEnumerator, IDisposable
		{
			public PositionEnumerator(Vector3Int min, Vector3Int max)
			{
				this._current = min;
				this._min = min;
				this._max = max;
				this.Reset();
			}

			/// <summary>
			///   <para>Returns this as an iterator that allows you to iterate over all positions within the BoundsInt.</para>
			/// </summary>
			/// <returns>
			///   <para>This BoundsInt.PositionEnumerator.</para>
			/// </returns>
			public BoundsInt.PositionEnumerator GetEnumerator()
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
				if (this._current.z >= this._max.z)
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
							this._current.y = this._min.y;
							this._current.z = this._current.z + 1;
							if (this._current.z >= this._max.z)
							{
								return false;
							}
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
			public Vector3Int Current
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

			private readonly Vector3Int _min;

			private readonly Vector3Int _max;

			private Vector3Int _current;
		}
	}
}
