using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[DebuggerTypeProxy(typeof(half2.DebuggerProxy))]
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct half2 : IEquatable<half2>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public half2(half x, half y)
		{
			this.x = x;
			this.y = y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public half2(half2 xy)
		{
			this.x = xy.x;
			this.y = xy.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public half2(half v)
		{
			this.x = v;
			this.y = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public half2(float v)
		{
			this.x = (half)v;
			this.y = (half)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public half2(float2 v)
		{
			this.x = (half)v.x;
			this.y = (half)v.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public half2(double v)
		{
			this.x = (half)v;
			this.y = (half)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public half2(double2 v)
		{
			this.x = (half)v.x;
			this.y = (half)v.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator half2(half v)
		{
			return new half2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator half2(float v)
		{
			return new half2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator half2(float2 v)
		{
			return new half2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator half2(double v)
		{
			return new half2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator half2(double2 v)
		{
			return new half2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator ==(half2 lhs, half2 rhs)
		{
			return new bool2(lhs.x == rhs.x, lhs.y == rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator ==(half2 lhs, half rhs)
		{
			return new bool2(lhs.x == rhs, lhs.y == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator ==(half lhs, half2 rhs)
		{
			return new bool2(lhs == rhs.x, lhs == rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator !=(half2 lhs, half2 rhs)
		{
			return new bool2(lhs.x != rhs.x, lhs.y != rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator !=(half2 lhs, half rhs)
		{
			return new bool2(lhs.x != rhs, lhs.y != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator !=(half lhs, half2 rhs)
		{
			return new bool2(lhs != rhs.x, lhs != rhs.y);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public half4 xxxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new half4(this.x, this.x, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public half4 xxxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new half4(this.x, this.x, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public half4 xxyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new half4(this.x, this.x, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public half4 xxyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new half4(this.x, this.x, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public half4 xyxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new half4(this.x, this.y, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public half4 xyxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new half4(this.x, this.y, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public half4 xyyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new half4(this.x, this.y, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public half4 xyyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new half4(this.x, this.y, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public half4 yxxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new half4(this.y, this.x, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public half4 yxxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new half4(this.y, this.x, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public half4 yxyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new half4(this.y, this.x, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public half4 yxyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new half4(this.y, this.x, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public half4 yyxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new half4(this.y, this.y, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public half4 yyxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new half4(this.y, this.y, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public half4 yyyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new half4(this.y, this.y, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public half4 yyyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new half4(this.y, this.y, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public half3 xxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new half3(this.x, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public half3 xxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new half3(this.x, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public half3 xyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new half3(this.x, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public half3 xyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new half3(this.x, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public half3 yxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new half3(this.y, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public half3 yxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new half3(this.y, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public half3 yyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new half3(this.y, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public half3 yyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new half3(this.y, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public half2 xx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new half2(this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public half2 xy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new half2(this.x, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.y = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public half2 yx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new half2(this.y, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.x = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public half2 yy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new half2(this.y, this.y);
			}
		}

		public unsafe half this[int index]
		{
			get
			{
				fixed (half2* ptr = &this)
				{
					return ((half*)ptr)[index];
				}
			}
			set
			{
				fixed (half* ptr = &this.x)
				{
					ptr[index] = value;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(half2 rhs)
		{
			return this.x == rhs.x && this.y == rhs.y;
		}

		public override bool Equals(object o)
		{
			if (o is half2)
			{
				half2 half = (half2)o;
				return this.Equals(half);
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
			return string.Format("half2({0}, {1})", this.x, this.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("half2({0}, {1})", this.x.ToString(format, formatProvider), this.y.ToString(format, formatProvider));
		}

		public half x;

		public half y;

		public static readonly half2 zero;

		internal sealed class DebuggerProxy
		{
			public DebuggerProxy(half2 v)
			{
				this.x = v.x;
				this.y = v.y;
			}

			public half x;

			public half y;
		}
	}
}
