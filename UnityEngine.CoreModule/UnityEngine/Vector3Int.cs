using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[Il2CppEagerStaticClassConstruction]
	public struct Vector3Int : IEquatable<Vector3Int>, IFormattable
	{
		public int x
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.m_X;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_X = value;
			}
		}

		public int y
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.m_Y;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Y = value;
			}
		}

		public int z
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.m_Z;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Z = value;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3Int(int x, int y)
		{
			this.m_X = x;
			this.m_Y = y;
			this.m_Z = 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3Int(int x, int y, int z)
		{
			this.m_X = x;
			this.m_Y = y;
			this.m_Z = z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Set(int x, int y, int z)
		{
			this.m_X = x;
			this.m_Y = y;
			this.m_Z = z;
		}

		public int this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				int num;
				switch (index)
				{
				case 0:
					num = this.m_X;
					break;
				case 1:
					num = this.m_Y;
					break;
				case 2:
					num = this.m_Z;
					break;
				default:
					throw new IndexOutOfRangeException(string.Format("Invalid Vector3Int index addressed: {0}!", index));
				}
				return num;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				switch (index)
				{
				case 0:
					this.m_X = value;
					break;
				case 1:
					this.m_Y = value;
					break;
				case 2:
					this.m_Z = value;
					break;
				default:
					throw new IndexOutOfRangeException(string.Format("Invalid Vector3Int index addressed: {0}!", index));
				}
			}
		}

		public readonly float magnitude
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Mathf.Sqrt((float)(this.m_X * this.m_X + this.m_Y * this.m_Y + this.m_Z * this.m_Z));
			}
		}

		public readonly int sqrMagnitude
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_X * this.m_X + this.m_Y * this.m_Y + this.m_Z * this.m_Z;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Distance(Vector3Int a, Vector3Int b)
		{
			return (a - b).magnitude;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Distance(in Vector3Int a, in Vector3Int b)
		{
			return (a - b).magnitude;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int Min(Vector3Int lhs, Vector3Int rhs)
		{
			return new Vector3Int(Mathf.Min(lhs.m_X, rhs.m_X), Mathf.Min(lhs.m_Y, rhs.m_Y), Mathf.Min(lhs.m_Z, rhs.m_Z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int Min(in Vector3Int lhs, in Vector3Int rhs)
		{
			return new Vector3Int(Mathf.Min(lhs.m_X, rhs.m_X), Mathf.Min(lhs.m_Y, rhs.m_Y), Mathf.Min(lhs.m_Z, rhs.m_Z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int Max(Vector3Int lhs, Vector3Int rhs)
		{
			return new Vector3Int(Mathf.Max(lhs.m_X, rhs.m_X), Mathf.Max(lhs.m_Y, rhs.m_Y), Mathf.Max(lhs.m_Z, rhs.m_Z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int Max(in Vector3Int lhs, in Vector3Int rhs)
		{
			return new Vector3Int(Mathf.Max(lhs.m_X, rhs.m_X), Mathf.Max(lhs.m_Y, rhs.m_Y), Mathf.Max(lhs.m_Z, rhs.m_Z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int Scale(Vector3Int a, Vector3Int b)
		{
			return new Vector3Int(a.m_X * b.m_X, a.m_Y * b.m_Y, a.m_Z * b.m_Z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int Scale(in Vector3Int a, in Vector3Int b)
		{
			return new Vector3Int(a.m_X * b.m_X, a.m_Y * b.m_Y, a.m_Z * b.m_Z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Scale(Vector3Int scale)
		{
			this.m_X *= scale.m_X;
			this.m_Y *= scale.m_Y;
			this.m_Z *= scale.m_Z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Scale(in Vector3Int scale)
		{
			this.m_X *= scale.m_X;
			this.m_Y *= scale.m_Y;
			this.m_Z *= scale.m_Z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clamp(Vector3Int min, Vector3Int max)
		{
			this.m_X = Mathf.Clamp(this.m_X, min.m_X, max.m_X);
			this.m_Y = Mathf.Clamp(this.m_Y, min.m_Y, max.m_Y);
			this.m_Z = Mathf.Clamp(this.m_Z, min.m_Z, max.m_Z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clamp(in Vector3Int min, in Vector3Int max)
		{
			this.m_X = Mathf.Clamp(this.m_X, min.m_X, max.m_X);
			this.m_Y = Mathf.Clamp(this.m_Y, min.m_Y, max.m_Y);
			this.m_Z = Mathf.Clamp(this.m_Z, min.m_Z, max.m_Z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector3(Vector3Int v)
		{
			return new Vector3
			{
				x = (float)v.m_X,
				y = (float)v.m_Y,
				z = (float)v.m_Z
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator Vector2Int(Vector3Int v)
		{
			return new Vector2Int(v.m_X, v.m_Y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int FloorToInt(Vector3 v)
		{
			return new Vector3Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y), Mathf.FloorToInt(v.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int FloorToInt(in Vector3 v)
		{
			return new Vector3Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y), Mathf.FloorToInt(v.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int CeilToInt(Vector3 v)
		{
			return new Vector3Int(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y), Mathf.CeilToInt(v.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int CeilToInt(in Vector3 v)
		{
			return new Vector3Int(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y), Mathf.CeilToInt(v.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int RoundToInt(Vector3 v)
		{
			return new Vector3Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int RoundToInt(in Vector3 v)
		{
			return new Vector3Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int operator +(Vector3Int a, Vector3Int b)
		{
			return new Vector3Int(a.m_X + b.m_X, a.m_Y + b.m_Y, a.m_Z + b.m_Z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int operator -(Vector3Int a, Vector3Int b)
		{
			return new Vector3Int(a.m_X - b.m_X, a.m_Y - b.m_Y, a.m_Z - b.m_Z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int operator *(Vector3Int a, Vector3Int b)
		{
			return new Vector3Int(a.m_X * b.m_X, a.m_Y * b.m_Y, a.m_Z * b.m_Z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int operator -(Vector3Int a)
		{
			return new Vector3Int(-a.m_X, -a.m_Y, -a.m_Z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int operator *(Vector3Int a, int b)
		{
			return new Vector3Int(a.m_X * b, a.m_Y * b, a.m_Z * b);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int operator *(int a, Vector3Int b)
		{
			return new Vector3Int(a * b.m_X, a * b.m_Y, a * b.m_Z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int operator /(Vector3Int a, int b)
		{
			return new Vector3Int(a.m_X / b, a.m_Y / b, a.m_Z / b);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Vector3Int lhs, Vector3Int rhs)
		{
			return lhs.m_X == rhs.m_X && lhs.m_Y == rhs.m_Y && lhs.m_Z == rhs.m_Z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Vector3Int lhs, Vector3Int rhs)
		{
			return !(lhs == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly bool Equals(object other)
		{
			Vector3Int vector3Int;
			bool flag;
			if (other is Vector3Int)
			{
				vector3Int = (Vector3Int)other;
				flag = true;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			return flag2 && this.Equals(in vector3Int);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(Vector3Int other)
		{
			return this == other;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(in Vector3Int other)
		{
			return this == other;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly int GetHashCode()
		{
			int hashCode = this.m_Y.GetHashCode();
			int hashCode2 = this.m_Z.GetHashCode();
			return this.m_X.GetHashCode() ^ (hashCode << 4) ^ (hashCode >> 28) ^ (hashCode2 >> 4) ^ (hashCode2 << 28);
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
			return string.Format("({0}, {1}, {2})", this.m_X.ToString(format, formatProvider), this.m_Y.ToString(format, formatProvider), this.m_Z.ToString(format, formatProvider));
		}

		public static Vector3Int zero
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector3Int.s_Zero;
			}
		}

		public static Vector3Int one
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector3Int.s_One;
			}
		}

		public static Vector3Int up
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector3Int.s_Up;
			}
		}

		public static Vector3Int down
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector3Int.s_Down;
			}
		}

		public static Vector3Int left
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector3Int.s_Left;
			}
		}

		public static Vector3Int right
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector3Int.s_Right;
			}
		}

		public static Vector3Int forward
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector3Int.s_Forward;
			}
		}

		public static Vector3Int back
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector3Int.s_Back;
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

		private static readonly Vector3Int s_Forward = new Vector3Int(0, 0, 1);

		private static readonly Vector3Int s_Back = new Vector3Int(0, 0, -1);
	}
}
