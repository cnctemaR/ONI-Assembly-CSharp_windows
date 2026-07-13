using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[NativeType("Runtime/Math/Vector2Int.h")]
	[Il2CppEagerStaticClassConstruction]
	public struct Vector2Int : IEquatable<Vector2Int>, IFormattable
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2Int(int x, int y)
		{
			this.m_X = x;
			this.m_Y = y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Set(int x, int y)
		{
			this.m_X = x;
			this.m_Y = y;
		}

		public int this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				int num;
				if (index != 0)
				{
					if (index != 1)
					{
						throw new IndexOutOfRangeException(string.Format("Invalid Vector2Int index addressed: {0}!", index));
					}
					num = this.m_Y;
				}
				else
				{
					num = this.m_X;
				}
				return num;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				if (index != 0)
				{
					if (index != 1)
					{
						throw new IndexOutOfRangeException(string.Format("Invalid Vector2Int index addressed: {0}!", index));
					}
					this.m_Y = value;
				}
				else
				{
					this.m_X = value;
				}
			}
		}

		public readonly float magnitude
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Mathf.Sqrt((float)(this.m_X * this.m_X + this.m_Y * this.m_Y));
			}
		}

		public readonly int sqrMagnitude
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_X * this.m_X + this.m_Y * this.m_Y;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Distance(Vector2Int a, Vector2Int b)
		{
			float num = (float)(a.m_X - b.m_X);
			float num2 = (float)(a.m_Y - b.m_Y);
			return (float)Math.Sqrt((double)(num * num + num2 * num2));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Distance(in Vector2Int a, in Vector2Int b)
		{
			float num = (float)(a.m_X - b.m_X);
			float num2 = (float)(a.m_Y - b.m_Y);
			return (float)Math.Sqrt((double)(num * num + num2 * num2));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2Int Min(Vector2Int lhs, Vector2Int rhs)
		{
			return new Vector2Int(Mathf.Min(lhs.m_X, rhs.m_X), Mathf.Min(lhs.m_Y, rhs.m_Y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2Int Min(in Vector2Int lhs, in Vector2Int rhs)
		{
			return new Vector2Int(Mathf.Min(lhs.m_X, rhs.m_X), Mathf.Min(lhs.m_Y, rhs.m_Y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2Int Max(Vector2Int lhs, Vector2Int rhs)
		{
			return new Vector2Int(Mathf.Max(lhs.m_X, rhs.m_X), Mathf.Max(lhs.m_Y, rhs.m_Y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2Int Max(in Vector2Int lhs, in Vector2Int rhs)
		{
			return new Vector2Int(Mathf.Max(lhs.m_X, rhs.m_X), Mathf.Max(lhs.m_Y, rhs.m_Y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2Int Scale(Vector2Int a, Vector2Int b)
		{
			return new Vector2Int(a.m_X * b.m_X, a.m_Y * b.m_Y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2Int Scale(in Vector2Int a, in Vector2Int b)
		{
			return new Vector2Int(a.m_X * b.m_X, a.m_Y * b.m_Y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Scale(Vector2Int scale)
		{
			this.m_X *= scale.m_X;
			this.m_Y *= scale.m_Y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Scale(in Vector2Int scale)
		{
			this.m_X *= scale.m_X;
			this.m_Y *= scale.m_Y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clamp(Vector2Int min, Vector2Int max)
		{
			this.m_X = Mathf.Clamp(this.m_X, min.m_X, max.m_X);
			this.m_Y = Mathf.Clamp(this.m_Y, min.m_Y, max.m_Y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clamp(in Vector2Int min, in Vector2Int max)
		{
			this.m_X = Mathf.Clamp(this.m_X, min.m_X, max.m_X);
			this.m_Y = Mathf.Clamp(this.m_Y, min.m_Y, max.m_Y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector2(Vector2Int v)
		{
			return new Vector2
			{
				x = (float)v.m_X,
				y = (float)v.m_Y
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator Vector3Int(Vector2Int v)
		{
			return new Vector3Int(v.m_X, v.m_Y, 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2Int FloorToInt(Vector2 v)
		{
			return new Vector2Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2Int FloorToInt(in Vector2 v)
		{
			return new Vector2Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2Int CeilToInt(Vector2 v)
		{
			return new Vector2Int(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2Int CeilToInt(in Vector2 v)
		{
			return new Vector2Int(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2Int RoundToInt(Vector2 v)
		{
			return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2Int RoundToInt(in Vector2 v)
		{
			return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2Int operator -(Vector2Int v)
		{
			return new Vector2Int(-v.m_X, -v.m_Y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2Int operator +(Vector2Int a, Vector2Int b)
		{
			return new Vector2Int(a.m_X + b.m_X, a.m_Y + b.m_Y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2Int operator -(Vector2Int a, Vector2Int b)
		{
			return new Vector2Int(a.m_X - b.m_X, a.m_Y - b.m_Y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2Int operator *(Vector2Int a, Vector2Int b)
		{
			return new Vector2Int(a.m_X * b.m_X, a.m_Y * b.m_Y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2Int operator *(int a, Vector2Int b)
		{
			return new Vector2Int(a * b.m_X, a * b.m_Y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2Int operator *(Vector2Int a, int b)
		{
			return new Vector2Int(a.m_X * b, a.m_Y * b);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2Int operator /(Vector2Int a, int b)
		{
			return new Vector2Int(a.m_X / b, a.m_Y / b);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Vector2Int lhs, Vector2Int rhs)
		{
			return lhs.m_X == rhs.m_X && lhs.m_Y == rhs.m_Y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Vector2Int lhs, Vector2Int rhs)
		{
			return !(lhs == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly bool Equals(object other)
		{
			Vector2Int vector2Int;
			bool flag;
			if (other is Vector2Int)
			{
				vector2Int = (Vector2Int)other;
				flag = true;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			return flag2 && this.Equals(in vector2Int);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(Vector2Int other)
		{
			return this.m_X == other.m_X && this.m_Y == other.m_Y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(in Vector2Int other)
		{
			return this.m_X == other.m_X && this.m_Y == other.m_Y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly int GetHashCode()
		{
			return (this.m_X * 73856093) ^ (this.m_Y * 83492791);
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
			return string.Format("({0}, {1})", this.m_X.ToString(format, formatProvider), this.m_Y.ToString(format, formatProvider));
		}

		public static Vector2Int zero
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector2Int.s_Zero;
			}
		}

		public static Vector2Int one
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector2Int.s_One;
			}
		}

		public static Vector2Int up
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector2Int.s_Up;
			}
		}

		public static Vector2Int down
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector2Int.s_Down;
			}
		}

		public static Vector2Int left
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector2Int.s_Left;
			}
		}

		public static Vector2Int right
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector2Int.s_Right;
			}
		}

		private int m_X;

		private int m_Y;

		private static readonly Vector2Int s_Zero = new Vector2Int(0, 0);

		private static readonly Vector2Int s_One = new Vector2Int(1, 1);

		private static readonly Vector2Int s_Up = new Vector2Int(0, 1);

		private static readonly Vector2Int s_Down = new Vector2Int(0, -1);

		private static readonly Vector2Int s_Left = new Vector2Int(-1, 0);

		private static readonly Vector2Int s_Right = new Vector2Int(1, 0);
	}
}
