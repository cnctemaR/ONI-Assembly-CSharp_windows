using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace Unity.Mathematics
{
	[DebuggerTypeProxy(typeof(float4.DebuggerProxy))]
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct float4 : IEquatable<float4>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4(float x, float y, float z, float w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4(float x, float y, float2 zw)
		{
			this.x = x;
			this.y = y;
			this.z = zw.x;
			this.w = zw.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4(float x, float2 yz, float w)
		{
			this.x = x;
			this.y = yz.x;
			this.z = yz.y;
			this.w = w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4(float x, float3 yzw)
		{
			this.x = x;
			this.y = yzw.x;
			this.z = yzw.y;
			this.w = yzw.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4(float2 xy, float z, float w)
		{
			this.x = xy.x;
			this.y = xy.y;
			this.z = z;
			this.w = w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4(float2 xy, float2 zw)
		{
			this.x = xy.x;
			this.y = xy.y;
			this.z = zw.x;
			this.w = zw.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4(float3 xyz, float w)
		{
			this.x = xyz.x;
			this.y = xyz.y;
			this.z = xyz.z;
			this.w = w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4(float4 xyzw)
		{
			this.x = xyzw.x;
			this.y = xyzw.y;
			this.z = xyzw.z;
			this.w = xyzw.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4(float v)
		{
			this.x = v;
			this.y = v;
			this.z = v;
			this.w = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4(bool v)
		{
			this.x = (v ? 1f : 0f);
			this.y = (v ? 1f : 0f);
			this.z = (v ? 1f : 0f);
			this.w = (v ? 1f : 0f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4(bool4 v)
		{
			this.x = (v.x ? 1f : 0f);
			this.y = (v.y ? 1f : 0f);
			this.z = (v.z ? 1f : 0f);
			this.w = (v.w ? 1f : 0f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4(int v)
		{
			this.x = (float)v;
			this.y = (float)v;
			this.z = (float)v;
			this.w = (float)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4(int4 v)
		{
			this.x = (float)v.x;
			this.y = (float)v.y;
			this.z = (float)v.z;
			this.w = (float)v.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4(uint v)
		{
			this.x = v;
			this.y = v;
			this.z = v;
			this.w = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4(uint4 v)
		{
			this.x = v.x;
			this.y = v.y;
			this.z = v.z;
			this.w = v.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4(half v)
		{
			this.x = v;
			this.y = v;
			this.z = v;
			this.w = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4(half4 v)
		{
			this.x = v.x;
			this.y = v.y;
			this.z = v.z;
			this.w = v.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4(double v)
		{
			this.x = (float)v;
			this.y = (float)v;
			this.z = (float)v;
			this.w = (float)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float4(double4 v)
		{
			this.x = (float)v.x;
			this.y = (float)v.y;
			this.z = (float)v.z;
			this.w = (float)v.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float4(float v)
		{
			return new float4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float4(bool v)
		{
			return new float4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float4(bool4 v)
		{
			return new float4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float4(int v)
		{
			return new float4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float4(int4 v)
		{
			return new float4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float4(uint v)
		{
			return new float4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float4(uint4 v)
		{
			return new float4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float4(half v)
		{
			return new float4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float4(half4 v)
		{
			return new float4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float4(double v)
		{
			return new float4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float4(double4 v)
		{
			return new float4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 operator *(float4 lhs, float4 rhs)
		{
			return new float4(lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z, lhs.w * rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 operator *(float4 lhs, float rhs)
		{
			return new float4(lhs.x * rhs, lhs.y * rhs, lhs.z * rhs, lhs.w * rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 operator *(float lhs, float4 rhs)
		{
			return new float4(lhs * rhs.x, lhs * rhs.y, lhs * rhs.z, lhs * rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 operator +(float4 lhs, float4 rhs)
		{
			return new float4(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z, lhs.w + rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 operator +(float4 lhs, float rhs)
		{
			return new float4(lhs.x + rhs, lhs.y + rhs, lhs.z + rhs, lhs.w + rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 operator +(float lhs, float4 rhs)
		{
			return new float4(lhs + rhs.x, lhs + rhs.y, lhs + rhs.z, lhs + rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 operator -(float4 lhs, float4 rhs)
		{
			return new float4(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z, lhs.w - rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 operator -(float4 lhs, float rhs)
		{
			return new float4(lhs.x - rhs, lhs.y - rhs, lhs.z - rhs, lhs.w - rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 operator -(float lhs, float4 rhs)
		{
			return new float4(lhs - rhs.x, lhs - rhs.y, lhs - rhs.z, lhs - rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 operator /(float4 lhs, float4 rhs)
		{
			return new float4(lhs.x / rhs.x, lhs.y / rhs.y, lhs.z / rhs.z, lhs.w / rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 operator /(float4 lhs, float rhs)
		{
			return new float4(lhs.x / rhs, lhs.y / rhs, lhs.z / rhs, lhs.w / rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 operator /(float lhs, float4 rhs)
		{
			return new float4(lhs / rhs.x, lhs / rhs.y, lhs / rhs.z, lhs / rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 operator %(float4 lhs, float4 rhs)
		{
			return new float4(lhs.x % rhs.x, lhs.y % rhs.y, lhs.z % rhs.z, lhs.w % rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 operator %(float4 lhs, float rhs)
		{
			return new float4(lhs.x % rhs, lhs.y % rhs, lhs.z % rhs, lhs.w % rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 operator %(float lhs, float4 rhs)
		{
			return new float4(lhs % rhs.x, lhs % rhs.y, lhs % rhs.z, lhs % rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 operator ++(float4 val)
		{
			float num = val.x + 1f;
			val.x = num;
			float num2 = num;
			num = val.y + 1f;
			val.y = num;
			float num3 = num;
			num = val.z + 1f;
			val.z = num;
			float num4 = num;
			num = val.w + 1f;
			val.w = num;
			return new float4(num2, num3, num4, num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 operator --(float4 val)
		{
			float num = val.x - 1f;
			val.x = num;
			float num2 = num;
			num = val.y - 1f;
			val.y = num;
			float num3 = num;
			num = val.z - 1f;
			val.z = num;
			float num4 = num;
			num = val.w - 1f;
			val.w = num;
			return new float4(num2, num3, num4, num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator <(float4 lhs, float4 rhs)
		{
			return new bool4(lhs.x < rhs.x, lhs.y < rhs.y, lhs.z < rhs.z, lhs.w < rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator <(float4 lhs, float rhs)
		{
			return new bool4(lhs.x < rhs, lhs.y < rhs, lhs.z < rhs, lhs.w < rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator <(float lhs, float4 rhs)
		{
			return new bool4(lhs < rhs.x, lhs < rhs.y, lhs < rhs.z, lhs < rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator <=(float4 lhs, float4 rhs)
		{
			return new bool4(lhs.x <= rhs.x, lhs.y <= rhs.y, lhs.z <= rhs.z, lhs.w <= rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator <=(float4 lhs, float rhs)
		{
			return new bool4(lhs.x <= rhs, lhs.y <= rhs, lhs.z <= rhs, lhs.w <= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator <=(float lhs, float4 rhs)
		{
			return new bool4(lhs <= rhs.x, lhs <= rhs.y, lhs <= rhs.z, lhs <= rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator >(float4 lhs, float4 rhs)
		{
			return new bool4(lhs.x > rhs.x, lhs.y > rhs.y, lhs.z > rhs.z, lhs.w > rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator >(float4 lhs, float rhs)
		{
			return new bool4(lhs.x > rhs, lhs.y > rhs, lhs.z > rhs, lhs.w > rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator >(float lhs, float4 rhs)
		{
			return new bool4(lhs > rhs.x, lhs > rhs.y, lhs > rhs.z, lhs > rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator >=(float4 lhs, float4 rhs)
		{
			return new bool4(lhs.x >= rhs.x, lhs.y >= rhs.y, lhs.z >= rhs.z, lhs.w >= rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator >=(float4 lhs, float rhs)
		{
			return new bool4(lhs.x >= rhs, lhs.y >= rhs, lhs.z >= rhs, lhs.w >= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator >=(float lhs, float4 rhs)
		{
			return new bool4(lhs >= rhs.x, lhs >= rhs.y, lhs >= rhs.z, lhs >= rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 operator -(float4 val)
		{
			return new float4(-val.x, -val.y, -val.z, -val.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 operator +(float4 val)
		{
			return new float4(val.x, val.y, val.z, val.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator ==(float4 lhs, float4 rhs)
		{
			return new bool4(lhs.x == rhs.x, lhs.y == rhs.y, lhs.z == rhs.z, lhs.w == rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator ==(float4 lhs, float rhs)
		{
			return new bool4(lhs.x == rhs, lhs.y == rhs, lhs.z == rhs, lhs.w == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator ==(float lhs, float4 rhs)
		{
			return new bool4(lhs == rhs.x, lhs == rhs.y, lhs == rhs.z, lhs == rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator !=(float4 lhs, float4 rhs)
		{
			return new bool4(lhs.x != rhs.x, lhs.y != rhs.y, lhs.z != rhs.z, lhs.w != rhs.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator !=(float4 lhs, float rhs)
		{
			return new bool4(lhs.x != rhs, lhs.y != rhs, lhs.z != rhs, lhs.w != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 operator !=(float lhs, float4 rhs)
		{
			return new bool4(lhs != rhs.x, lhs != rhs.y, lhs != rhs.z, lhs != rhs.w);
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
		public float4 xxxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.x, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xxxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.x, this.x, this.w);
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
		public float4 xxyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.x, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xxyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.x, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xxzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.x, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xxzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.x, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xxzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.x, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xxzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.x, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xxwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.x, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xxwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.x, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xxwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.x, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xxww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.x, this.w, this.w);
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
		public float4 xyxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.y, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xyxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.y, this.x, this.w);
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
		public float4 xyyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.y, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xyyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.y, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xyzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.y, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xyzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.y, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xyzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.y, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xyzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.y, this.z, this.w);
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
		public float4 xywx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.y, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xywy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.y, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xywz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.y, this.w, this.z);
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
		public float4 xyww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.y, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xzxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.z, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xzxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.z, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xzxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.z, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xzxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.z, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xzyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.z, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xzyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.z, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xzyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.z, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xzyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.z, this.y, this.w);
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
		public float4 xzzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.z, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xzzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.z, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xzzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.z, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xzzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.z, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xzwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.z, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xzwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.z, this.w, this.y);
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
		public float4 xzwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.z, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xzww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.z, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xwxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.w, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xwxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.w, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xwxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.w, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xwxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.w, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xwyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.w, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xwyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.w, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xwyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.w, this.y, this.z);
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
		public float4 xwyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.w, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xwzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.w, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xwzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.w, this.z, this.y);
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
		public float4 xwzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.w, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xwzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.w, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xwwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.w, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xwwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.w, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xwwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.w, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 xwww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.x, this.w, this.w, this.w);
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
		public float4 yxxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.x, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yxxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.x, this.x, this.w);
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
		public float4 yxyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.x, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yxyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.x, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yxzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.x, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yxzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.x, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yxzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.x, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yxzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.x, this.z, this.w);
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
		public float4 yxwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.x, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yxwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.x, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yxwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.x, this.w, this.z);
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
		public float4 yxww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.x, this.w, this.w);
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
		public float4 yyxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.y, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yyxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.y, this.x, this.w);
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
		public float4 yyyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.y, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yyyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.y, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yyzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.y, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yyzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.y, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yyzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.y, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yyzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.y, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yywx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.y, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yywy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.y, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yywz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.y, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yyww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.y, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yzxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.z, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yzxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.z, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yzxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.z, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yzxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.z, this.x, this.w);
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
		public float4 yzyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.z, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yzyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.z, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yzyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.z, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yzyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.z, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yzzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.z, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yzzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.z, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yzzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.z, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yzzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.z, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yzwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.z, this.w, this.x);
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
		public float4 yzwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.z, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yzwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.z, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 yzww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.z, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 ywxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.w, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 ywxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.w, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 ywxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.w, this.x, this.z);
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
		public float4 ywxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.w, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 ywyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.w, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 ywyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.w, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 ywyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.w, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 ywyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.w, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 ywzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.w, this.z, this.x);
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
		public float4 ywzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.w, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 ywzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.w, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 ywzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.w, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 ywwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.w, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 ywwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.w, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 ywwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.w, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 ywww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.y, this.w, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zxxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.x, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zxxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.x, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zxxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.x, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zxxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.x, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zxyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.x, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zxyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.x, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zxyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.x, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zxyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.x, this.y, this.w);
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
		public float4 zxzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.x, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zxzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.x, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zxzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.x, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zxzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.x, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zxwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.x, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zxwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.x, this.w, this.y);
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
		public float4 zxwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.x, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zxww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.x, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zyxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.y, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zyxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.y, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zyxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.y, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zyxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.y, this.x, this.w);
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
		public float4 zyyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.y, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zyyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.y, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zyyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.y, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zyyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.y, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zyzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.y, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zyzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.y, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zyzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.y, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zyzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.y, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zywx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.y, this.w, this.x);
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
		public float4 zywy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.y, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zywz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.y, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zyww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.y, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zzxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.z, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zzxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.z, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zzxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.z, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zzxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.z, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zzyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.z, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zzyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.z, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zzyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.z, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zzyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.z, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zzzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.z, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zzzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.z, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zzzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.z, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zzzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.z, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zzwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.z, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zzwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.z, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zzwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.z, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zzww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.z, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zwxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.w, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zwxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.w, this.x, this.y);
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
		public float4 zwxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.w, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zwxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.w, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zwyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.w, this.y, this.x);
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
		public float4 zwyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.w, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zwyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.w, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zwyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.w, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zwzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.w, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zwzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.w, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zwzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.w, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zwzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.w, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zwwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.w, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zwwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.w, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zwwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.w, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 zwww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.z, this.w, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wxxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.x, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wxxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.x, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wxxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.x, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wxxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.x, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wxyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.x, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wxyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.x, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wxyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.x, this.y, this.z);
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
		public float4 wxyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.x, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wxzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.x, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wxzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.x, this.z, this.y);
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
		public float4 wxzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.x, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wxzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.x, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wxwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.x, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wxwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.x, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wxwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.x, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wxww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.x, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wyxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.y, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wyxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.y, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wyxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.y, this.x, this.z);
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
		public float4 wyxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.y, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wyyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.y, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wyyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.y, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wyyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.y, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wyyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.y, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wyzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.y, this.z, this.x);
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
		public float4 wyzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.y, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wyzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.y, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wyzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.y, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wywx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.y, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wywy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.y, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wywz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.y, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wyww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.y, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wzxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.z, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wzxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.z, this.x, this.y);
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
		public float4 wzxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.z, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wzxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.z, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wzyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.z, this.y, this.x);
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
		public float4 wzyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.z, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wzyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.z, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wzyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.z, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wzzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.z, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wzzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.z, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wzzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.z, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wzzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.z, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wzwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.z, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wzwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.z, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wzwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.z, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wzww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.z, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wwxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.w, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wwxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.w, this.x, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wwxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.w, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wwxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.w, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wwyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.w, this.y, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wwyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.w, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wwyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.w, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wwyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.w, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wwzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.w, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wwzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.w, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wwzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.w, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wwzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.w, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wwwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.w, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wwwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.w, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wwwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.w, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float4 wwww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float4(this.w, this.w, this.w, this.w);
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
		public float3 xxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.x, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 xxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.x, this.x, this.w);
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
		public float3 xyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.x, this.y, this.z);
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
		public float3 xyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.x, this.y, this.w);
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
		public float3 xzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.x, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 xzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.x, this.z, this.y);
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
		public float3 xzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.x, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 xzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.x, this.z, this.w);
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
		public float3 xwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.x, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 xwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.x, this.w, this.y);
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
		public float3 xwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.x, this.w, this.z);
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
		public float3 xww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.x, this.w, this.w);
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
		public float3 yxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.y, this.x, this.z);
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
		public float3 yxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.y, this.x, this.w);
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
		public float3 yyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.y, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 yyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.y, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 yzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.y, this.z, this.x);
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
		public float3 yzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.y, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 yzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.y, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 yzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.y, this.z, this.w);
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
		public float3 ywx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.y, this.w, this.x);
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
		public float3 ywy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.y, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 ywz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.y, this.w, this.z);
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
		public float3 yww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.y, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 zxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.z, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 zxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.z, this.x, this.y);
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
		public float3 zxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.z, this.x, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 zxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.z, this.x, this.w);
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
		public float3 zyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.z, this.y, this.x);
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
		public float3 zyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.z, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 zyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.z, this.y, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 zyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.z, this.y, this.w);
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
		public float3 zzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.z, this.z, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 zzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.z, this.z, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 zzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.z, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 zzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.z, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 zwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.z, this.w, this.x);
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
		public float3 zwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.z, this.w, this.y);
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
		public float3 zwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.z, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 zww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.z, this.w, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 wxx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.w, this.x, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 wxy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.w, this.x, this.y);
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
		public float3 wxz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.w, this.x, this.z);
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
		public float3 wxw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.w, this.x, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 wyx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.w, this.y, this.x);
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
		public float3 wyy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.w, this.y, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 wyz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.w, this.y, this.z);
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
		public float3 wyw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.w, this.y, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 wzx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.w, this.z, this.x);
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
		public float3 wzy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.w, this.z, this.y);
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
		public float3 wzz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.w, this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 wzw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.w, this.z, this.w);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 wwx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.w, this.w, this.x);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 wwy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.w, this.w, this.y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 wwz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.w, this.w, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float3 www
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float3(this.w, this.w, this.w);
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
		public float2 xz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float2(this.x, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.z = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float2 xw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float2(this.x, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.x = value.x;
				this.w = value.y;
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

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float2 yz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float2(this.y, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.z = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float2 yw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float2(this.y, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.y = value.x;
				this.w = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float2 zx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float2(this.z, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.x = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float2 zy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float2(this.z, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.y = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float2 zz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float2(this.z, this.z);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float2 zw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float2(this.z, this.w);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.z = value.x;
				this.w = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float2 wx
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float2(this.w, this.x);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.x = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float2 wy
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float2(this.w, this.y);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.y = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float2 wz
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float2(this.w, this.z);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.w = value.x;
				this.z = value.y;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public float2 ww
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new float2(this.w, this.w);
			}
		}

		public unsafe float this[int index]
		{
			get
			{
				fixed (float4* ptr = &this)
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
		public bool Equals(float4 rhs)
		{
			return this.x == rhs.x && this.y == rhs.y && this.z == rhs.z && this.w == rhs.w;
		}

		public override bool Equals(object o)
		{
			if (o is float4)
			{
				float4 @float = (float4)o;
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
			return string.Format("float4({0}f, {1}f, {2}f, {3}f)", new object[] { this.x, this.y, this.z, this.w });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("float4({0}f, {1}f, {2}f, {3}f)", new object[]
			{
				this.x.ToString(format, formatProvider),
				this.y.ToString(format, formatProvider),
				this.z.ToString(format, formatProvider),
				this.w.ToString(format, formatProvider)
			});
		}

		public static implicit operator float4(Vector4 v)
		{
			return new float4(v.x, v.y, v.z, v.w);
		}

		public static implicit operator Vector4(float4 v)
		{
			return new Vector4(v.x, v.y, v.z, v.w);
		}

		public float x;

		public float y;

		public float z;

		public float w;

		public static readonly float4 zero;

		internal sealed class DebuggerProxy
		{
			public DebuggerProxy(float4 v)
			{
				this.x = v.x;
				this.y = v.y;
				this.z = v.z;
				this.w = v.w;
			}

			public float x;

			public float y;

			public float z;

			public float w;
		}
	}
}
