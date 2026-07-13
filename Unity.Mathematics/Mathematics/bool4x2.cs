using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct bool4x2 : IEquatable<bool4x2>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool4x2(bool4 c0, bool4 c1)
		{
			this.c0 = c0;
			this.c1 = c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool4x2(bool m00, bool m01, bool m10, bool m11, bool m20, bool m21, bool m30, bool m31)
		{
			this.c0 = new bool4(m00, m10, m20, m30);
			this.c1 = new bool4(m01, m11, m21, m31);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool4x2(bool v)
		{
			this.c0 = v;
			this.c1 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator bool4x2(bool v)
		{
			return new bool4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator ==(bool4x2 lhs, bool4x2 rhs)
		{
			return new bool4x2(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator ==(bool4x2 lhs, bool rhs)
		{
			return new bool4x2(lhs.c0 == rhs, lhs.c1 == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator ==(bool lhs, bool4x2 rhs)
		{
			return new bool4x2(lhs == rhs.c0, lhs == rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator !=(bool4x2 lhs, bool4x2 rhs)
		{
			return new bool4x2(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator !=(bool4x2 lhs, bool rhs)
		{
			return new bool4x2(lhs.c0 != rhs, lhs.c1 != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator !=(bool lhs, bool4x2 rhs)
		{
			return new bool4x2(lhs != rhs.c0, lhs != rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator !(bool4x2 val)
		{
			return new bool4x2(!val.c0, !val.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator &(bool4x2 lhs, bool4x2 rhs)
		{
			return new bool4x2(lhs.c0 & rhs.c0, lhs.c1 & rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator &(bool4x2 lhs, bool rhs)
		{
			return new bool4x2(lhs.c0 & rhs, lhs.c1 & rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator &(bool lhs, bool4x2 rhs)
		{
			return new bool4x2(lhs & rhs.c0, lhs & rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator |(bool4x2 lhs, bool4x2 rhs)
		{
			return new bool4x2(lhs.c0 | rhs.c0, lhs.c1 | rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator |(bool4x2 lhs, bool rhs)
		{
			return new bool4x2(lhs.c0 | rhs, lhs.c1 | rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator |(bool lhs, bool4x2 rhs)
		{
			return new bool4x2(lhs | rhs.c0, lhs | rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator ^(bool4x2 lhs, bool4x2 rhs)
		{
			return new bool4x2(lhs.c0 ^ rhs.c0, lhs.c1 ^ rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator ^(bool4x2 lhs, bool rhs)
		{
			return new bool4x2(lhs.c0 ^ rhs, lhs.c1 ^ rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 operator ^(bool lhs, bool4x2 rhs)
		{
			return new bool4x2(lhs ^ rhs.c0, lhs ^ rhs.c1);
		}

		public unsafe ref bool4 this[int index]
		{
			get
			{
				fixed (bool4x2* ptr = &this)
				{
					return ref *(bool4*)(ptr + (IntPtr)index * (IntPtr)sizeof(bool4) / (IntPtr)sizeof(bool4x2));
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(bool4x2 rhs)
		{
			return this.c0.Equals(rhs.c0) && this.c1.Equals(rhs.c1);
		}

		public override bool Equals(object o)
		{
			if (o is bool4x2)
			{
				bool4x2 bool4x = (bool4x2)o;
				return this.Equals(bool4x);
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
			return string.Format("bool4x2({0}, {1},  {2}, {3},  {4}, {5},  {6}, {7})", new object[]
			{
				this.c0.x,
				this.c1.x,
				this.c0.y,
				this.c1.y,
				this.c0.z,
				this.c1.z,
				this.c0.w,
				this.c1.w
			});
		}

		public bool4 c0;

		public bool4 c1;
	}
}
