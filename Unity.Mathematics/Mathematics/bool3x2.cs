using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct bool3x2 : IEquatable<bool3x2>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool3x2(bool3 c0, bool3 c1)
		{
			this.c0 = c0;
			this.c1 = c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool3x2(bool m00, bool m01, bool m10, bool m11, bool m20, bool m21)
		{
			this.c0 = new bool3(m00, m10, m20);
			this.c1 = new bool3(m01, m11, m21);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool3x2(bool v)
		{
			this.c0 = v;
			this.c1 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator bool3x2(bool v)
		{
			return new bool3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator ==(bool3x2 lhs, bool3x2 rhs)
		{
			return new bool3x2(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator ==(bool3x2 lhs, bool rhs)
		{
			return new bool3x2(lhs.c0 == rhs, lhs.c1 == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator ==(bool lhs, bool3x2 rhs)
		{
			return new bool3x2(lhs == rhs.c0, lhs == rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator !=(bool3x2 lhs, bool3x2 rhs)
		{
			return new bool3x2(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator !=(bool3x2 lhs, bool rhs)
		{
			return new bool3x2(lhs.c0 != rhs, lhs.c1 != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator !=(bool lhs, bool3x2 rhs)
		{
			return new bool3x2(lhs != rhs.c0, lhs != rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator !(bool3x2 val)
		{
			return new bool3x2(!val.c0, !val.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator &(bool3x2 lhs, bool3x2 rhs)
		{
			return new bool3x2(lhs.c0 & rhs.c0, lhs.c1 & rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator &(bool3x2 lhs, bool rhs)
		{
			return new bool3x2(lhs.c0 & rhs, lhs.c1 & rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator &(bool lhs, bool3x2 rhs)
		{
			return new bool3x2(lhs & rhs.c0, lhs & rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator |(bool3x2 lhs, bool3x2 rhs)
		{
			return new bool3x2(lhs.c0 | rhs.c0, lhs.c1 | rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator |(bool3x2 lhs, bool rhs)
		{
			return new bool3x2(lhs.c0 | rhs, lhs.c1 | rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator |(bool lhs, bool3x2 rhs)
		{
			return new bool3x2(lhs | rhs.c0, lhs | rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator ^(bool3x2 lhs, bool3x2 rhs)
		{
			return new bool3x2(lhs.c0 ^ rhs.c0, lhs.c1 ^ rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator ^(bool3x2 lhs, bool rhs)
		{
			return new bool3x2(lhs.c0 ^ rhs, lhs.c1 ^ rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator ^(bool lhs, bool3x2 rhs)
		{
			return new bool3x2(lhs ^ rhs.c0, lhs ^ rhs.c1);
		}

		public unsafe ref bool3 this[int index]
		{
			get
			{
				fixed (bool3x2* ptr = &this)
				{
					return ref *(bool3*)(ptr + (IntPtr)index * (IntPtr)sizeof(bool3) / (IntPtr)sizeof(bool3x2));
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(bool3x2 rhs)
		{
			return this.c0.Equals(rhs.c0) && this.c1.Equals(rhs.c1);
		}

		public override bool Equals(object o)
		{
			if (o is bool3x2)
			{
				bool3x2 bool3x = (bool3x2)o;
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
			return string.Format("bool3x2({0}, {1},  {2}, {3},  {4}, {5})", new object[]
			{
				this.c0.x,
				this.c1.x,
				this.c0.y,
				this.c1.y,
				this.c0.z,
				this.c1.z
			});
		}

		public bool3 c0;

		public bool3 c1;
	}
}
