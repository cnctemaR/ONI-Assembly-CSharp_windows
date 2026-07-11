using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Representation of 3D vectors and points using integers.</para>
	/// </summary>
	[UsedByNativeCode]
	public struct Vector3Int : IEquatable<Vector3Int>
	{
		public Vector3Int(int x, int y, int z)
		{
			this.m_X = x;
			this.m_Y = y;
			this.m_Z = z;
		}

		/// <summary>
		///   <para>X component of the vector.</para>
		/// </summary>
		public int x
		{
			get
			{
				return this.m_X;
			}
			set
			{
				this.m_X = value;
			}
		}

		/// <summary>
		///   <para>Y component of the vector.</para>
		/// </summary>
		public int y
		{
			get
			{
				return this.m_Y;
			}
			set
			{
				this.m_Y = value;
			}
		}

		/// <summary>
		///   <para>Z component of the vector.</para>
		/// </summary>
		public int z
		{
			get
			{
				return this.m_Z;
			}
			set
			{
				this.m_Z = value;
			}
		}

		/// <summary>
		///   <para>Set x, y and z components of an existing Vector3Int.</para>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		public void Set(int x, int y, int z)
		{
			this.m_X = x;
			this.m_Y = y;
			this.m_Z = z;
		}

		public int this[int index]
		{
			get
			{
				int num;
				switch (index)
				{
				case 0:
					num = this.x;
					break;
				case 1:
					num = this.y;
					break;
				case 2:
					num = this.z;
					break;
				default:
					throw new IndexOutOfRangeException(UnityString.Format("Invalid Vector3Int index addressed: {0}!", new object[] { index }));
				}
				return num;
			}
			set
			{
				switch (index)
				{
				case 0:
					this.x = value;
					break;
				case 1:
					this.y = value;
					break;
				case 2:
					this.z = value;
					break;
				default:
					throw new IndexOutOfRangeException(UnityString.Format("Invalid Vector3Int index addressed: {0}!", new object[] { index }));
				}
			}
		}

		/// <summary>
		///   <para>Returns the length of this vector (Read Only).</para>
		/// </summary>
		public float magnitude
		{
			get
			{
				return Mathf.Sqrt((float)(this.x * this.x + this.y * this.y + this.z * this.z));
			}
		}

		/// <summary>
		///   <para>Returns the squared length of this vector (Read Only).</para>
		/// </summary>
		public int sqrMagnitude
		{
			get
			{
				return this.x * this.x + this.y * this.y + this.z * this.z;
			}
		}

		/// <summary>
		///   <para>Returns the distance between a and b.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public static float Distance(Vector3Int a, Vector3Int b)
		{
			return (a - b).magnitude;
		}

		/// <summary>
		///   <para>Returns a vector that is made from the smallest components of two vectors.</para>
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		public static Vector3Int Min(Vector3Int lhs, Vector3Int rhs)
		{
			return new Vector3Int(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y), Mathf.Min(lhs.z, rhs.z));
		}

		/// <summary>
		///   <para>Returns a vector that is made from the largest components of two vectors.</para>
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		public static Vector3Int Max(Vector3Int lhs, Vector3Int rhs)
		{
			return new Vector3Int(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y), Mathf.Max(lhs.z, rhs.z));
		}

		/// <summary>
		///   <para>Multiplies two vectors component-wise.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public static Vector3Int Scale(Vector3Int a, Vector3Int b)
		{
			return new Vector3Int(a.x * b.x, a.y * b.y, a.z * b.z);
		}

		/// <summary>
		///   <para>Multiplies every component of this vector by the same component of scale.</para>
		/// </summary>
		/// <param name="scale"></param>
		public void Scale(Vector3Int scale)
		{
			this.x *= scale.x;
			this.y *= scale.y;
			this.z *= scale.z;
		}

		/// <summary>
		///   <para>Clamps the Vector3Int to the bounds given by min and max.</para>
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public void Clamp(Vector3Int min, Vector3Int max)
		{
			this.x = Math.Max(min.x, this.x);
			this.x = Math.Min(max.x, this.x);
			this.y = Math.Max(min.y, this.y);
			this.y = Math.Min(max.y, this.y);
			this.z = Math.Max(min.z, this.z);
			this.z = Math.Min(max.z, this.z);
		}

		public static implicit operator Vector3(Vector3Int v)
		{
			return new Vector3((float)v.x, (float)v.y, (float)v.z);
		}

		/// <summary>
		///   <para>Converts a  Vector3 to a Vector3Int by doing a Floor to each value.</para>
		/// </summary>
		/// <param name="v"></param>
		public static Vector3Int FloorToInt(Vector3 v)
		{
			return new Vector3Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y), Mathf.FloorToInt(v.z));
		}

		/// <summary>
		///   <para>Converts a  Vector3 to a Vector3Int by doing a Ceiling to each value.</para>
		/// </summary>
		/// <param name="v"></param>
		public static Vector3Int CeilToInt(Vector3 v)
		{
			return new Vector3Int(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y), Mathf.CeilToInt(v.z));
		}

		/// <summary>
		///   <para>Converts a  Vector3 to a Vector3Int by doing a Round to each value.</para>
		/// </summary>
		/// <param name="v"></param>
		public static Vector3Int RoundToInt(Vector3 v)
		{
			return new Vector3Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
		}

		public static Vector3Int operator +(Vector3Int a, Vector3Int b)
		{
			return new Vector3Int(a.x + b.x, a.y + b.y, a.z + b.z);
		}

		public static Vector3Int operator -(Vector3Int a, Vector3Int b)
		{
			return new Vector3Int(a.x - b.x, a.y - b.y, a.z - b.z);
		}

		public static Vector3Int operator *(Vector3Int a, Vector3Int b)
		{
			return new Vector3Int(a.x * b.x, a.y * b.y, a.z * b.z);
		}

		public static Vector3Int operator *(Vector3Int a, int b)
		{
			return new Vector3Int(a.x * b, a.y * b, a.z * b);
		}

		public static bool operator ==(Vector3Int lhs, Vector3Int rhs)
		{
			return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
		}

		public static bool operator !=(Vector3Int lhs, Vector3Int rhs)
		{
			return !(lhs == rhs);
		}

		/// <summary>
		///   <para>Returns true if the objects are equal.</para>
		/// </summary>
		/// <param name="other"></param>
		public override bool Equals(object other)
		{
			return other is Vector3Int && this.Equals((Vector3Int)other);
		}

		public bool Equals(Vector3Int other)
		{
			return this == other;
		}

		/// <summary>
		///   <para>Gets the hash code for the Vector3Int.</para>
		/// </summary>
		/// <returns>
		///   <para>The hash code of the Vector3Int.</para>
		/// </returns>
		public override int GetHashCode()
		{
			return this.x.GetHashCode() ^ (this.y.GetHashCode() << 2) ^ (this.z.GetHashCode() >> 2);
		}

		/// <summary>
		///   <para>Returns a nicely formatted string for this vector.</para>
		/// </summary>
		/// <param name="format"></param>
		public override string ToString()
		{
			return UnityString.Format("({0}, {1}, {2})", new object[] { this.x, this.y, this.z });
		}

		/// <summary>
		///   <para>Returns a nicely formatted string for this vector.</para>
		/// </summary>
		/// <param name="format"></param>
		public string ToString(string format)
		{
			return UnityString.Format("({0}, {1}, {2})", new object[]
			{
				this.x.ToString(format),
				this.y.ToString(format),
				this.z.ToString(format)
			});
		}

		/// <summary>
		///   <para>Shorthand for writing Vector3Int (0, 0, 0).</para>
		/// </summary>
		public static Vector3Int zero
		{
			get
			{
				return Vector3Int.s_Zero;
			}
		}

		/// <summary>
		///   <para>Shorthand for writing Vector3Int (1, 1, 1).</para>
		/// </summary>
		public static Vector3Int one
		{
			get
			{
				return Vector3Int.s_One;
			}
		}

		/// <summary>
		///   <para>Shorthand for writing Vector3Int (0, 1, 0).</para>
		/// </summary>
		public static Vector3Int up
		{
			get
			{
				return Vector3Int.s_Up;
			}
		}

		/// <summary>
		///   <para>Shorthand for writing Vector3Int (0, -1, 0).</para>
		/// </summary>
		public static Vector3Int down
		{
			get
			{
				return Vector3Int.s_Down;
			}
		}

		/// <summary>
		///   <para>Shorthand for writing Vector3Int (-1, 0, 0).</para>
		/// </summary>
		public static Vector3Int left
		{
			get
			{
				return Vector3Int.s_Left;
			}
		}

		/// <summary>
		///   <para>Shorthand for writing Vector3Int (1, 0, 0).</para>
		/// </summary>
		public static Vector3Int right
		{
			get
			{
				return Vector3Int.s_Right;
			}
		}

		private int m_X;

		private int m_Y;

		private int m_Z;

		private static readonly Vector3Int s_Zero = new Vector3Int(0, 0, 0);

		private static readonly Vector3Int s_One = new Vector3Int(1, 1, 1);

		private static readonly Vector3Int s_Up = new Vector3Int(0, 1, 0);

		private static readonly Vector3Int s_Down = new Vector3Int(0, -1, 0);

		private static readonly Vector3Int s_Left = new Vector3Int(-1, 0, 0);

		private static readonly Vector3Int s_Right = new Vector3Int(1, 0, 0);
	}
}
