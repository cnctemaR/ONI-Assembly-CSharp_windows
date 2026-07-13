using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace Unity.Mathematics
{
	[DebuggerTypeProxy(typeof(float2.DebuggerProxy))]
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct float2 : IEquatable<float2>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2(float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2(float2 xy)
		{
			this.x = xy.x;
			this.y = xy.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2(float v)
		{
			this.x = v;
			this.y = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2(bool v)
		{
			this.x = (v ? 1f : 0f);
			this.y = (v ? 1f : 0f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2(bool2 v)
		{
			this.x = (v.x ? 1f : 0f);
			this.y = (v.y ? 1f : 0f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2(int v)
		{
			this.x = (float)v;
			this.y = (float)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2(int2 v)
		{
			this.x = (float)v.x;
			this.y = (float)v.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2(uint v)
		{
			this.x = v;
			this.y = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2(uint2 v)
		{
			this.x = v.x;
			this.y = v.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2(half v)
		{
			this.x = v;
			this.y = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2(half2 v)
		{
			this.x = v.x;
			this.y = v.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2(double v)
		{
			this.x = (float)v;
			this.y = (float)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float2(double2 v)
		{
			this.x = (float)v.x;
			this.y = (float)v.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float2(float v)
		{
			return new float2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float2(bool v)
		{
			return new float2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float2(bool2 v)
		{
			return new float2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float2(int v)
		{
			return new float2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float2(int2 v)
		{
			return new float2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float2(uint v)
		{
			return new float2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float2(uint2 v)
		{
			return new float2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float2(half v)
		{
			return new float2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float2(half2 v)
		{
			return new float2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float2(double v)
		{
			return new float2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float2(double2 v)
		{
			return new float2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 operator *(float2 lhs, float2 rhs)
		{
			return new float2(lhs.x * rhs.x, lhs.y * rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 operator *(float2 lhs, float rhs)
		{
			return new float2(lhs.x * rhs, lhs.y * rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 operator *(float lhs, float2 rhs)
		{
			return new float2(lhs * rhs.x, lhs * rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 operator +(float2 lhs, float2 rhs)
		{
			return new float2(lhs.x + rhs.x, lhs.y + rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 operator +(float2 lhs, float rhs)
		{
			return new float2(lhs.x + rhs, lhs.y + rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 operator +(float lhs, float2 rhs)
		{
			return new float2(lhs + rhs.x, lhs + rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 operator -(float2 lhs, float2 rhs)
		{
			return new float2(lhs.x - rhs.x, lhs.y - rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 operator -(float2 lhs, float rhs)
		{
			return new float2(lhs.x - rhs, lhs.y - rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 operator -(float lhs, float2 rhs)
		{
			return new float2(lhs - rhs.x, lhs - rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 operator /(float2 lhs, float2 rhs)
		{
			return new float2(lhs.x / rhs.x, lhs.y / rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 operator /(float2 lhs, float rhs)
		{
			return new float2(lhs.x / rhs, lhs.y / rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 operator /(float lhs, float2 rhs)
		{
			return new float2(lhs / rhs.x, lhs / rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 operator %(float2 lhs, float2 rhs)
		{
			return new float2(lhs.x % rhs.x, lhs.y % rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 operator %(float2 lhs, float rhs)
		{
			return new float2(lhs.x % rhs, lhs.y % rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 operator %(float lhs, float2 rhs)
		{
			return new float2(lhs % rhs.x, lhs % rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 operator ++(float2 val)
		{
			float num = val.x + 1f;
			val.x = num;
			float num2 = num;
			num = val.y + 1f;
			val.y = num;
			return new float2(num2, num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 operator --(float2 val)
		{
			float num = val.x - 1f;
			val.x = num;
			float num2 = num;
			num = val.y - 1f;
			val.y = num;
			return new float2(num2, num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator <(float2 lhs, float2 rhs)
		{
			return new bool2(lhs.x < rhs.x, lhs.y < rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator <(float2 lhs, float rhs)
		{
			return new bool2(lhs.x < rhs, lhs.y < rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator <(float lhs, float2 rhs)
		{
			return new bool2(lhs < rhs.x, lhs < rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator <=(float2 lhs, float2 rhs)
		{
			return new bool2(lhs.x <= rhs.x, lhs.y <= rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator <=(float2 lhs, float rhs)
		{
			return new bool2(lhs.x <= rhs, lhs.y <= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator <=(float lhs, float2 rhs)
		{
			return new bool2(lhs <= rhs.x, lhs <= rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator >(float2 lhs, float2 rhs)
		{
			return new bool2(lhs.x > rhs.x, lhs.y > rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator >(float2 lhs, float rhs)
		{
			return new bool2(lhs.x > rhs, lhs.y > rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator >(float lhs, float2 rhs)
		{
			return new bool2(lhs > rhs.x, lhs > rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator >=(float2 lhs, float2 rhs)
		{
			return new bool2(lhs.x >= rhs.x, lhs.y >= rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator >=(float2 lhs, float rhs)
		{
			return new bool2(lhs.x >= rhs, lhs.y >= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator >=(float lhs, float2 rhs)
		{
			return new bool2(lhs >= rhs.x, lhs >= rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 operator -(float2 val)
		{
			return new float2(-val.x, -val.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 operator +(float2 val)
		{
			return new float2(val.x, val.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator ==(float2 lhs, float2 rhs)
		{
			return new bool2(lhs.x == rhs.x, lhs.y == rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator ==(float2 lhs, float rhs)
		{
			return new bool2(lhs.x == rhs, lhs.y == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator ==(float lhs, float2 rhs)
		{
			return new bool2(lhs == rhs.x, lhs == rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator !=(float2 lhs, float2 rhs)
		{
			return new bool2(lhs.x != rhs.x, lhs.y != rhs.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator !=(float2 lhs, float rhs)
		{
			return new bool2(lhs.x != rhs, lhs.y != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 operator !=(float lhs, float2 rhs)
		{
			return new bool2(lhs != rhs.x, lhs != rhs.y);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xxxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.x, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xxxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.x, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xxyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.x, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xxyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.x, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xyxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.y, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xyxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.y, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xyyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.y, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xyyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.y, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yxxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.x, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yxxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.x, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yxyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.x, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yxyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.x, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yyxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.y, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yyxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.y, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yyyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.y, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yyyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.y, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 xxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.x, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 xxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.x, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 xyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.x, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 xyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.x, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 yxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.y, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 yxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.y, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 yyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.y, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 yyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.y, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float2 xx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float2(this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float2 xy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float2(this.x, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.y = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float2 yx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float2(this.y, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.x = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float2 yy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float2(this.y, this.y);
			}
		}

		public unsafe float this[int index]
		{
			get
			{
				fixed (float2* ptr = &this)
				{
					return ((float*)ptr)[index];
				}
			}
			set
			{
				fixed (float* ptr = &this.x)
				{
					ptr[index] = value;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(float2 rhs)
		{
			return this.x == rhs.x && this.y == rhs.y;
		}

		public override bool Equals(object o)
		{
			if (o is float2)
			{
				float2 @float = (float2)o;
				return this.Equals(@float);
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
			return string.Format("float2({0}f, {1}f)", this.x, this.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("float2({0}f, {1}f)", this.x.ToString(format, formatProvider), this.y.ToString(format, formatProvider));
		}

		public static implicit operator Vector2(float2 v)
		{
			return new Vector2(v.x, v.y);
		}

		public static implicit operator float2(Vector2 v)
		{
			return new float2(v.x, v.y);
		}

		public float x;

		public float y;

		public static readonly float2 zero;

		internal sealed class DebuggerProxy
		{
			public DebuggerProxy(float2 v)
			{
				this.x = v.x;
				this.y = v.y;
			}

			public float x;

			public float y;
		}
	}
}
