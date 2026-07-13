using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct bool2x2 : IEquatable<bool2x2>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool2x2(bool2 c0, bool2 c1)
		{
			this.c0 = c0;
			this.c1 = c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool2x2(bool m00, bool m01, bool m10, bool m11)
		{
			this.c0 = new bool2(m00, m10);
			this.c1 = new bool2(m01, m11);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool2x2(bool v)
		{
			this.c0 = v;
			this.c1 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator bool2x2(bool v)
		{
			return new bool2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator ==(bool2x2 lhs, bool2x2 rhs)
		{
			return new bool2x2(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator ==(bool2x2 lhs, bool rhs)
		{
			return new bool2x2(lhs.c0 == rhs, lhs.c1 == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator ==(bool lhs, bool2x2 rhs)
		{
			return new bool2x2(lhs == rhs.c0, lhs == rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator !=(bool2x2 lhs, bool2x2 rhs)
		{
			return new bool2x2(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator !=(bool2x2 lhs, bool rhs)
		{
			return new bool2x2(lhs.c0 != rhs, lhs.c1 != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator !=(bool lhs, bool2x2 rhs)
		{
			return new bool2x2(lhs != rhs.c0, lhs != rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator !(bool2x2 val)
		{
			return new bool2x2(!val.c0, !val.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator &(bool2x2 lhs, bool2x2 rhs)
		{
			return new bool2x2(lhs.c0 & rhs.c0, lhs.c1 & rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator &(bool2x2 lhs, bool rhs)
		{
			return new bool2x2(lhs.c0 & rhs, lhs.c1 & rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator &(bool lhs, bool2x2 rhs)
		{
			return new bool2x2(lhs & rhs.c0, lhs & rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator |(bool2x2 lhs, bool2x2 rhs)
		{
			return new bool2x2(lhs.c0 | rhs.c0, lhs.c1 | rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator |(bool2x2 lhs, bool rhs)
		{
			return new bool2x2(lhs.c0 | rhs, lhs.c1 | rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator |(bool lhs, bool2x2 rhs)
		{
			return new bool2x2(lhs | rhs.c0, lhs | rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator ^(bool2x2 lhs, bool2x2 rhs)
		{
			return new bool2x2(lhs.c0 ^ rhs.c0, lhs.c1 ^ rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator ^(bool2x2 lhs, bool rhs)
		{
			return new bool2x2(lhs.c0 ^ rhs, lhs.c1 ^ rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 operator ^(bool lhs, bool2x2 rhs)
		{
			return new bool2x2(lhs ^ rhs.c0, lhs ^ rhs.c1);
		}

		public unsafe ref bool2 this[int index]
		{
			get
			{
				fixed (bool2x2* ptr = &this)
				{
					return ref *(bool2*)(ptr + (IntPtr)index * (IntPtr)sizeof(bool2) / (IntPtr)sizeof(bool2x2));
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(bool2x2 rhs)
		{
			return this.c0.Equals(rhs.c0) && this.c1.Equals(rhs.c1);
		}

		public override bool Equals(object o)
		{
			if (o is bool2x2)
			{
				bool2x2 bool2x = (bool2x2)o;
				return this.Equals(bool2x);
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
			return string.Format("bool2x2({0}, {1},  {2}, {3})", new object[]
			{
				this.c0.x,
				this.c1.x,
				this.c0.y,
				this.c1.y
			});
		}

		public bool2 c0;

		public bool2 c1;
	}
}
