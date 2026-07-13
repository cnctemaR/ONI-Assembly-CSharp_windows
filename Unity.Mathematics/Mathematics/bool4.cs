using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[DebuggerTypeProxy(typeof(bool4.DebuggerProxy))]
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct bool4 : IEquatable<bool4>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool4(bool x, bool y, bool z, bool w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool4(bool x, bool y, bool2 zw)
		{
			this.x = x;
			this.y = y;
			this.z = zw.x;
			this.w = zw.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool4(bool x, bool2 yz, bool w)
		{
			this.x = x;
			this.y = yz.x;
			this.z = yz.y;
			this.w = w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool4(bool x, bool3 yzw)
		{
			this.x = x;
			this.y = yzw.x;
			this.z = yzw.y;
			this.w = yzw.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool4(bool2 xy, bool z, bool w)
		{
			this.x = xy.x;
			this.y = xy.y;
			this.z = z;
			this.w = w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool4(bool2 xy, bool2 zw)
		{
			this.x = xy.x;
			this.y = xy.y;
			this.z = zw.x;
			this.w = zw.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool4(bool3 xyz, bool w)
		{
			this.x = xyz.x;
			this.y = xyz.y;
			this.z = xyz.z;
			this.w = w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool4(bool4 xyzw)
		{
			this.x = xyzw.x;
			this.y = xyzw.y;
			this.z = xyzw.z;
			this.w = xyzw.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool4(bool v)
		{
			this.x = v;
			this.y = v;
			this.z = v;
			this.w = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator bool4(bool v)
		{
			return new bool4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator ==(bool4 lhs, bool4 rhs)
		{
			return new bool4(lhs.x == rhs.x, lhs.y == rhs.y, lhs.z == rhs.z, lhs.w == rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator ==(bool4 lhs, bool rhs)
		{
			return new bool4(lhs.x == rhs, lhs.y == rhs, lhs.z == rhs, lhs.w == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator ==(bool lhs, bool4 rhs)
		{
			return new bool4(lhs == rhs.x, lhs == rhs.y, lhs == rhs.z, lhs == rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator !=(bool4 lhs, bool4 rhs)
		{
			return new bool4(lhs.x != rhs.x, lhs.y != rhs.y, lhs.z != rhs.z, lhs.w != rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator !=(bool4 lhs, bool rhs)
		{
			return new bool4(lhs.x != rhs, lhs.y != rhs, lhs.z != rhs, lhs.w != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator !=(bool lhs, bool4 rhs)
		{
			return new bool4(lhs != rhs.x, lhs != rhs.y, lhs != rhs.z, lhs != rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator !(bool4 val)
		{
			return new bool4(!val.x, !val.y, !val.z, !val.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator &(bool4 lhs, bool4 rhs)
		{
			return new bool4(lhs.x & rhs.x, lhs.y & rhs.y, lhs.z & rhs.z, lhs.w & rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator &(bool4 lhs, bool rhs)
		{
			return new bool4(lhs.x && rhs, lhs.y && rhs, lhs.z && rhs, lhs.w && rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator &(bool lhs, bool4 rhs)
		{
			return new bool4(lhs & rhs.x, lhs & rhs.y, lhs & rhs.z, lhs & rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator |(bool4 lhs, bool4 rhs)
		{
			return new bool4(lhs.x | rhs.x, lhs.y | rhs.y, lhs.z | rhs.z, lhs.w | rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator |(bool4 lhs, bool rhs)
		{
			return new bool4(lhs.x || rhs, lhs.y || rhs, lhs.z || rhs, lhs.w || rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator |(bool lhs, bool4 rhs)
		{
			return new bool4(lhs | rhs.x, lhs | rhs.y, lhs | rhs.z, lhs | rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator ^(bool4 lhs, bool4 rhs)
		{
			return new bool4(lhs.x ^ rhs.x, lhs.y ^ rhs.y, lhs.z ^ rhs.z, lhs.w ^ rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator ^(bool4 lhs, bool rhs)
		{
			return new bool4(lhs.x ^ rhs, lhs.y ^ rhs, lhs.z ^ rhs, lhs.w ^ rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator ^(bool lhs, bool4 rhs)
		{
			return new bool4(lhs ^ rhs.x, lhs ^ rhs.y, lhs ^ rhs.z, lhs ^ rhs.w);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xxxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.x, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xxxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.x, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xxxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.x, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xxxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.x, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xxyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.x, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xxyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.x, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xxyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.x, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xxyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.x, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xxzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.x, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xxzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.x, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xxzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.x, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xxzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.x, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xxwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.x, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xxwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.x, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xxwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.x, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xxww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.x, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xyxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.y, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xyxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.y, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xyxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.y, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xyxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.y, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xyyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.y, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xyyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.y, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xyyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.y, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xyyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.y, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xyzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.y, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xyzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.y, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xyzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.y, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xyzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.y, this.z, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.y = value.y;
				this.z = value.z;
				this.w = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xywx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.y, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xywy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.y, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xywz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.y, this.w, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.y = value.y;
				this.w = value.z;
				this.z = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xyww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.y, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xzxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.z, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xzxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.z, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xzxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.z, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xzxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.z, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xzyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.z, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xzyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.z, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xzyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.z, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xzyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.z, this.y, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.z = value.y;
				this.y = value.z;
				this.w = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xzzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.z, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xzzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.z, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xzzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.z, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xzzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.z, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xzwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.z, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xzwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.z, this.w, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.z = value.y;
				this.w = value.z;
				this.y = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xzwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.z, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xzww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.z, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xwxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.w, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xwxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.w, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xwxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.w, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xwxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.w, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xwyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.w, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xwyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.w, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xwyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.w, this.y, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.w = value.y;
				this.y = value.z;
				this.z = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xwyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.w, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xwzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.w, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xwzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.w, this.z, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.w = value.y;
				this.z = value.z;
				this.y = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xwzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.w, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xwzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.w, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xwwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.w, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xwwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.w, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xwwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.w, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 xwww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.x, this.w, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yxxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.x, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yxxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.x, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yxxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.x, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yxxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.x, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yxyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.x, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yxyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.x, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yxyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.x, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yxyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.x, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yxzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.x, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yxzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.x, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yxzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.x, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yxzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.x, this.z, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.x = value.y;
				this.z = value.z;
				this.w = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yxwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.x, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yxwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.x, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yxwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.x, this.w, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.x = value.y;
				this.w = value.z;
				this.z = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yxww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.x, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yyxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.y, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yyxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.y, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yyxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.y, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yyxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.y, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yyyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.y, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yyyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.y, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yyyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.y, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yyyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.y, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yyzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.y, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yyzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.y, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yyzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.y, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yyzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.y, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yywx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.y, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yywy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.y, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yywz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.y, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yyww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.y, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yzxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.z, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yzxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.z, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yzxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.z, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yzxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.z, this.x, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.z = value.y;
				this.x = value.z;
				this.w = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yzyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.z, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yzyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.z, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yzyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.z, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yzyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.z, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yzzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.z, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yzzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.z, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yzzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.z, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yzzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.z, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yzwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.z, this.w, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.z = value.y;
				this.w = value.z;
				this.x = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yzwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.z, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yzwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.z, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 yzww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.z, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 ywxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.w, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 ywxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.w, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 ywxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.w, this.x, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.w = value.y;
				this.x = value.z;
				this.z = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 ywxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.w, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 ywyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.w, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 ywyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.w, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 ywyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.w, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 ywyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.w, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 ywzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.w, this.z, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.w = value.y;
				this.z = value.z;
				this.x = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 ywzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.w, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 ywzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.w, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 ywzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.w, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 ywwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.w, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 ywwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.w, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 ywwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.w, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 ywww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.y, this.w, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zxxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.x, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zxxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.x, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zxxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.x, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zxxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.x, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zxyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.x, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zxyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.x, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zxyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.x, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zxyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.x, this.y, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.x = value.y;
				this.y = value.z;
				this.w = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zxzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.x, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zxzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.x, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zxzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.x, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zxzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.x, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zxwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.x, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zxwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.x, this.w, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.x = value.y;
				this.w = value.z;
				this.y = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zxwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.x, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zxww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.x, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zyxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.y, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zyxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.y, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zyxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.y, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zyxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.y, this.x, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.y = value.y;
				this.x = value.z;
				this.w = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zyyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.y, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zyyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.y, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zyyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.y, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zyyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.y, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zyzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.y, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zyzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.y, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zyzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.y, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zyzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.y, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zywx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.y, this.w, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.y = value.y;
				this.w = value.z;
				this.x = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zywy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.y, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zywz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.y, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zyww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.y, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zzxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.z, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zzxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.z, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zzxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.z, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zzxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.z, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zzyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.z, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zzyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.z, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zzyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.z, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zzyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.z, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zzzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.z, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zzzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.z, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zzzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.z, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zzzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.z, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zzwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.z, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zzwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.z, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zzwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.z, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zzww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.z, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zwxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.w, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zwxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.w, this.x, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.w = value.y;
				this.x = value.z;
				this.y = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zwxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.w, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zwxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.w, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zwyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.w, this.y, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.w = value.y;
				this.y = value.z;
				this.x = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zwyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.w, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zwyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.w, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zwyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.w, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zwzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.w, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zwzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.w, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zwzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.w, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zwzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.w, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zwwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.w, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zwwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.w, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zwwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.w, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 zwww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.z, this.w, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wxxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.x, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wxxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.x, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wxxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.x, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wxxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.x, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wxyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.x, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wxyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.x, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wxyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.x, this.y, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.x = value.y;
				this.y = value.z;
				this.z = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wxyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.x, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wxzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.x, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wxzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.x, this.z, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.x = value.y;
				this.z = value.z;
				this.y = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wxzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.x, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wxzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.x, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wxwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.x, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wxwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.x, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wxwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.x, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wxww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.x, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wyxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.y, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wyxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.y, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wyxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.y, this.x, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.y = value.y;
				this.x = value.z;
				this.z = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wyxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.y, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wyyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.y, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wyyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.y, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wyyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.y, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wyyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.y, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wyzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.y, this.z, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.y = value.y;
				this.z = value.z;
				this.x = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wyzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.y, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wyzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.y, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wyzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.y, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wywx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.y, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wywy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.y, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wywz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.y, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wyww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.y, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wzxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.z, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wzxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.z, this.x, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.z = value.y;
				this.x = value.z;
				this.y = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wzxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.z, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wzxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.z, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wzyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.z, this.y, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.z = value.y;
				this.y = value.z;
				this.x = value.w;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wzyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.z, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wzyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.z, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wzyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.z, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wzzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.z, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wzzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.z, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wzzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.z, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wzzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.z, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wzwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.z, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wzwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.z, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wzwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.z, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wzww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.z, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wwxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.w, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wwxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.w, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wwxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.w, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wwxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.w, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wwyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.w, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wwyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.w, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wwyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.w, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wwyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.w, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wwzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.w, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wwzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.w, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wwzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.w, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wwzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.w, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wwwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.w, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wwwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.w, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wwwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.w, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool4 wwww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool4(this.w, this.w, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 xxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.x, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 xxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.x, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 xxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.x, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 xxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.x, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 xyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.x, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 xyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.x, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 xyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.x, this.y, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.y = value.y;
				this.z = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 xyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.x, this.y, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.y = value.y;
				this.w = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 xzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.x, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 xzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.x, this.z, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.z = value.y;
				this.y = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 xzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.x, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 xzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.x, this.z, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.z = value.y;
				this.w = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 xwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.x, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 xwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.x, this.w, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.w = value.y;
				this.y = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 xwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.x, this.w, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.w = value.y;
				this.z = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 xww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.x, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 yxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.y, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 yxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.y, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 yxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.y, this.x, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.x = value.y;
				this.z = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 yxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.y, this.x, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.x = value.y;
				this.w = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 yyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.y, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 yyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.y, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 yyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.y, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 yyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.y, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 yzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.y, this.z, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.z = value.y;
				this.x = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 yzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.y, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 yzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.y, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 yzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.y, this.z, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.z = value.y;
				this.w = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 ywx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.y, this.w, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.w = value.y;
				this.x = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 ywy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.y, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 ywz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.y, this.w, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.w = value.y;
				this.z = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 yww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.y, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 zxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.z, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 zxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.z, this.x, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.x = value.y;
				this.y = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 zxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.z, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 zxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.z, this.x, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.x = value.y;
				this.w = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 zyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.z, this.y, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.y = value.y;
				this.x = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 zyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.z, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 zyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.z, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 zyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.z, this.y, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.y = value.y;
				this.w = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 zzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.z, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 zzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.z, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 zzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.z, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 zzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.z, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 zwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.z, this.w, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.w = value.y;
				this.x = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 zwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.z, this.w, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.w = value.y;
				this.y = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 zwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.z, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 zww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.z, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 wxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.w, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 wxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.w, this.x, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.x = value.y;
				this.y = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 wxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.w, this.x, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.x = value.y;
				this.z = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 wxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.w, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 wyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.w, this.y, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.y = value.y;
				this.x = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 wyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.w, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 wyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.w, this.y, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.y = value.y;
				this.z = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 wyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.w, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 wzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.w, this.z, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.z = value.y;
				this.x = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 wzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.w, this.z, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.z = value.y;
				this.y = value.z;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 wzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.w, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 wzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.w, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 wwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.w, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 wwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.w, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 wwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.w, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool3 www
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool3(this.w, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool2 xx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool2(this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool2 xy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool2(this.x, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.y = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool2 xz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool2(this.x, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.z = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool2 xw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool2(this.x, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.w = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool2 yx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool2(this.y, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.x = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool2 yy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool2(this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool2 yz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool2(this.y, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.z = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool2 yw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool2(this.y, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.w = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool2 zx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool2(this.z, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.x = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool2 zy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool2(this.z, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.y = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool2 zz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool2(this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool2 zw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool2(this.z, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.w = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool2 wx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool2(this.w, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.x = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool2 wy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool2(this.w, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.y = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool2 wz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool2(this.w, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.z = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool2 ww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new bool2(this.w, this.w);
			}
		}

		public unsafe bool this[int index]
		{
			get
			{
				fixed (bool4* ptr = &this)
				{
					return ((byte*)ptr)[index] != 0;
				}
			}
			set
			{
				fixed (bool* ptr = &this.x)
				{
					ptr[index] = value;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(bool4 rhs)
		{
			return this.x == rhs.x && this.y == rhs.y && this.z == rhs.z && this.w == rhs.w;
		}

		public override bool Equals(object o)
		{
			if (o is bool4)
			{
				bool4 @bool = (bool4)o;
				return this.Equals(@bool);
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
			return string.Format("bool4({0}, {1}, {2}, {3})", new object[] { this.x, this.y, this.z, this.w });
		}

		[MarshalAs(UnmanagedType.U1)]
		public bool x;

		[MarshalAs(UnmanagedType.U1)]
		public bool y;

		[MarshalAs(UnmanagedType.U1)]
		public bool z;

		[MarshalAs(UnmanagedType.U1)]
		public bool w;

		internal sealed class DebuggerProxy
		{
			public DebuggerProxy(bool4 v)
			{
				this.x = v.x;
				this.y = v.y;
				this.z = v.z;
				this.w = v.w;
			}

			public bool x;

			public bool y;

			public bool z;

			public bool w;
		}
	}
}
