using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct bool3x4 : IEquatable<bool3x4>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool3x4(bool3 c0, bool3 c1, bool3 c2, bool3 c3)
		{
			this.c0 = c0;
			this.c1 = c1;
			this.c2 = c2;
			this.c3 = c3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool3x4(bool m00, bool m01, bool m02, bool m03, bool m10, bool m11, bool m12, bool m13, bool m20, bool m21, bool m22, bool m23)
		{
			this.c0 = new bool3(m00, m10, m20);
			this.c1 = new bool3(m01, m11, m21);
			this.c2 = new bool3(m02, m12, m22);
			this.c3 = new bool3(m03, m13, m23);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool3x4(bool v)
		{
			this.c0 = v;
			this.c1 = v;
			this.c2 = v;
			this.c3 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator bool3x4(bool v)
		{
			return new bool3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator ==(bool3x4 lhs, bool3x4 rhs)
		{
			return new bool3x4(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1, lhs.c2 == rhs.c2, lhs.c3 == rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator ==(bool3x4 lhs, bool rhs)
		{
			return new bool3x4(lhs.c0 == rhs, lhs.c1 == rhs, lhs.c2 == rhs, lhs.c3 == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator ==(bool lhs, bool3x4 rhs)
		{
			return new bool3x4(lhs == rhs.c0, lhs == rhs.c1, lhs == rhs.c2, lhs == rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator !=(bool3x4 lhs, bool3x4 rhs)
		{
			return new bool3x4(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1, lhs.c2 != rhs.c2, lhs.c3 != rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator !=(bool3x4 lhs, bool rhs)
		{
			return new bool3x4(lhs.c0 != rhs, lhs.c1 != rhs, lhs.c2 != rhs, lhs.c3 != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator !=(bool lhs, bool3x4 rhs)
		{
			return new bool3x4(lhs != rhs.c0, lhs != rhs.c1, lhs != rhs.c2, lhs != rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator !(bool3x4 val)
		{
			return new bool3x4(!val.c0, !val.c1, !val.c2, !val.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator &(bool3x4 lhs, bool3x4 rhs)
		{
			return new bool3x4(lhs.c0 & rhs.c0, lhs.c1 & rhs.c1, lhs.c2 & rhs.c2, lhs.c3 & rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator &(bool3x4 lhs, bool rhs)
		{
			return new bool3x4(lhs.c0 & rhs, lhs.c1 & rhs, lhs.c2 & rhs, lhs.c3 & rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator &(bool lhs, bool3x4 rhs)
		{
			return new bool3x4(lhs & rhs.c0, lhs & rhs.c1, lhs & rhs.c2, lhs & rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator |(bool3x4 lhs, bool3x4 rhs)
		{
			return new bool3x4(lhs.c0 | rhs.c0, lhs.c1 | rhs.c1, lhs.c2 | rhs.c2, lhs.c3 | rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator |(bool3x4 lhs, bool rhs)
		{
			return new bool3x4(lhs.c0 | rhs, lhs.c1 | rhs, lhs.c2 | rhs, lhs.c3 | rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator |(bool lhs, bool3x4 rhs)
		{
			return new bool3x4(lhs | rhs.c0, lhs | rhs.c1, lhs | rhs.c2, lhs | rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator ^(bool3x4 lhs, bool3x4 rhs)
		{
			return new bool3x4(lhs.c0 ^ rhs.c0, lhs.c1 ^ rhs.c1, lhs.c2 ^ rhs.c2, lhs.c3 ^ rhs.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator ^(bool3x4 lhs, bool rhs)
		{
			return new bool3x4(lhs.c0 ^ rhs, lhs.c1 ^ rhs, lhs.c2 ^ rhs, lhs.c3 ^ rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 operator ^(bool lhs, bool3x4 rhs)
		{
			return new bool3x4(lhs ^ rhs.c0, lhs ^ rhs.c1, lhs ^ rhs.c2, lhs ^ rhs.c3);
		}

		public unsafe ref bool3 this[int index]
		{
			get
			{
				fixed (bool3x4* ptr = &this)
				{
					return ref *(bool3*)(ptr + (IntPtr)index * (IntPtr)sizeof(bool3) / (IntPtr)sizeof(bool3x4));
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(bool3x4 rhs)
		{
			return this.c0.Equals(rhs.c0) && this.c1.Equals(rhs.c1) && this.c2.Equals(rhs.c2) && this.c3.Equals(rhs.c3);
		}

		public override bool Equals(object o)
		{
			if (o is bool3x4)
			{
				bool3x4 bool3x = (bool3x4)o;
				return this.Equals(bool3x);
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
		{
			return (int)math.hash(this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString()
		{
			return string.Format("bool3x4({0}, {1}, {2}, {3},  {4}, {5}, {6}, {7},  {8}, {9}, {10}, {11})", new object[]
			{
				this.c0.x,
				this.c1.x,
				this.c2.x,
				this.c3.x,
				this.c0.y,
				this.c1.y,
				this.c2.y,
				this.c3.y,
				this.c0.z,
				this.c1.z,
				this.c2.z,
				this.c3.z
			});
		}

		public bool3 c0;

		public bool3 c1;

		public bool3 c2;

		public bool3 c3;
	}
}
