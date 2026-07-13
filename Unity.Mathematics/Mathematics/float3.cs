using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace Unity.Mathematics
{
	[DebuggerTypeProxy(typeof(float3.DebuggerProxy))]
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct float3 : IEquatable<float3>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3(float x, float2 yz)
		{
			this.x = x;
			this.y = yz.x;
			this.z = yz.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3(float2 xy, float z)
		{
			this.x = xy.x;
			this.y = xy.y;
			this.z = z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3(float3 xyz)
		{
			this.x = xyz.x;
			this.y = xyz.y;
			this.z = xyz.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3(float v)
		{
			this.x = v;
			this.y = v;
			this.z = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3(bool v)
		{
			this.x = (v ? 1f : 0f);
			this.y = (v ? 1f : 0f);
			this.z = (v ? 1f : 0f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3(bool3 v)
		{
			this.x = (v.x ? 1f : 0f);
			this.y = (v.y ? 1f : 0f);
			this.z = (v.z ? 1f : 0f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3(int v)
		{
			this.x = (float)v;
			this.y = (float)v;
			this.z = (float)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3(int3 v)
		{
			this.x = (float)v.x;
			this.y = (float)v.y;
			this.z = (float)v.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3(uint v)
		{
			this.x = v;
			this.y = v;
			this.z = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3(uint3 v)
		{
			this.x = v.x;
			this.y = v.y;
			this.z = v.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3(half v)
		{
			this.x = v;
			this.y = v;
			this.z = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3(half3 v)
		{
			this.x = v.x;
			this.y = v.y;
			this.z = v.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3(double v)
		{
			this.x = (float)v;
			this.y = (float)v;
			this.z = (float)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3(double3 v)
		{
			this.x = (float)v.x;
			this.y = (float)v.y;
			this.z = (float)v.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float3(float v)
		{
			return new float3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float3(bool v)
		{
			return new float3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float3(bool3 v)
		{
			return new float3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float3(int v)
		{
			return new float3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float3(int3 v)
		{
			return new float3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float3(uint v)
		{
			return new float3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float3(uint3 v)
		{
			return new float3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float3(half v)
		{
			return new float3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float3(half3 v)
		{
			return new float3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float3(double v)
		{
			return new float3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float3(double3 v)
		{
			return new float3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 operator *(float3 lhs, float3 rhs)
		{
			return new float3(lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 operator *(float3 lhs, float rhs)
		{
			return new float3(lhs.x * rhs, lhs.y * rhs, lhs.z * rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 operator *(float lhs, float3 rhs)
		{
			return new float3(lhs * rhs.x, lhs * rhs.y, lhs * rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 operator +(float3 lhs, float3 rhs)
		{
			return new float3(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 operator +(float3 lhs, float rhs)
		{
			return new float3(lhs.x + rhs, lhs.y + rhs, lhs.z + rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 operator +(float lhs, float3 rhs)
		{
			return new float3(lhs + rhs.x, lhs + rhs.y, lhs + rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 operator -(float3 lhs, float3 rhs)
		{
			return new float3(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 operator -(float3 lhs, float rhs)
		{
			return new float3(lhs.x - rhs, lhs.y - rhs, lhs.z - rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 operator -(float lhs, float3 rhs)
		{
			return new float3(lhs - rhs.x, lhs - rhs.y, lhs - rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 operator /(float3 lhs, float3 rhs)
		{
			return new float3(lhs.x / rhs.x, lhs.y / rhs.y, lhs.z / rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 operator /(float3 lhs, float rhs)
		{
			return new float3(lhs.x / rhs, lhs.y / rhs, lhs.z / rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 operator /(float lhs, float3 rhs)
		{
			return new float3(lhs / rhs.x, lhs / rhs.y, lhs / rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 operator %(float3 lhs, float3 rhs)
		{
			return new float3(lhs.x % rhs.x, lhs.y % rhs.y, lhs.z % rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 operator %(float3 lhs, float rhs)
		{
			return new float3(lhs.x % rhs, lhs.y % rhs, lhs.z % rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 operator %(float lhs, float3 rhs)
		{
			return new float3(lhs % rhs.x, lhs % rhs.y, lhs % rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 operator ++(float3 val)
		{
			float num = val.x + 1f;
			val.x = num;
			float num2 = num;
			num = val.y + 1f;
			val.y = num;
			float num3 = num;
			num = val.z + 1f;
			val.z = num;
			return new float3(num2, num3, num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 operator --(float3 val)
		{
			float num = val.x - 1f;
			val.x = num;
			float num2 = num;
			num = val.y - 1f;
			val.y = num;
			float num3 = num;
			num = val.z - 1f;
			val.z = num;
			return new float3(num2, num3, num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator <(float3 lhs, float3 rhs)
		{
			return new bool3(lhs.x < rhs.x, lhs.y < rhs.y, lhs.z < rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator <(float3 lhs, float rhs)
		{
			return new bool3(lhs.x < rhs, lhs.y < rhs, lhs.z < rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator <(float lhs, float3 rhs)
		{
			return new bool3(lhs < rhs.x, lhs < rhs.y, lhs < rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator <=(float3 lhs, float3 rhs)
		{
			return new bool3(lhs.x <= rhs.x, lhs.y <= rhs.y, lhs.z <= rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator <=(float3 lhs, float rhs)
		{
			return new bool3(lhs.x <= rhs, lhs.y <= rhs, lhs.z <= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator <=(float lhs, float3 rhs)
		{
			return new bool3(lhs <= rhs.x, lhs <= rhs.y, lhs <= rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator >(float3 lhs, float3 rhs)
		{
			return new bool3(lhs.x > rhs.x, lhs.y > rhs.y, lhs.z > rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator >(float3 lhs, float rhs)
		{
			return new bool3(lhs.x > rhs, lhs.y > rhs, lhs.z > rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator >(float lhs, float3 rhs)
		{
			return new bool3(lhs > rhs.x, lhs > rhs.y, lhs > rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator >=(float3 lhs, float3 rhs)
		{
			return new bool3(lhs.x >= rhs.x, lhs.y >= rhs.y, lhs.z >= rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator >=(float3 lhs, float rhs)
		{
			return new bool3(lhs.x >= rhs, lhs.y >= rhs, lhs.z >= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator >=(float lhs, float3 rhs)
		{
			return new bool3(lhs >= rhs.x, lhs >= rhs.y, lhs >= rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 operator -(float3 val)
		{
			return new float3(-val.x, -val.y, -val.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 operator +(float3 val)
		{
			return new float3(val.x, val.y, val.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator ==(float3 lhs, float3 rhs)
		{
			return new bool3(lhs.x == rhs.x, lhs.y == rhs.y, lhs.z == rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator ==(float3 lhs, float rhs)
		{
			return new bool3(lhs.x == rhs, lhs.y == rhs, lhs.z == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator ==(float lhs, float3 rhs)
		{
			return new bool3(lhs == rhs.x, lhs == rhs.y, lhs == rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator !=(float3 lhs, float3 rhs)
		{
			return new bool3(lhs.x != rhs.x, lhs.y != rhs.y, lhs.z != rhs.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator !=(float3 lhs, float rhs)
		{
			return new bool3(lhs.x != rhs, lhs.y != rhs, lhs.z != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 operator !=(float lhs, float3 rhs)
		{
			return new bool3(lhs != rhs.x, lhs != rhs.y, lhs != rhs.z);
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

		public unsafe float this[int index]
		{
			get
			{
				fixed (float3* ptr = &this)
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
		public bool Equals(float3 rhs)
		{
			return this.x == rhs.x && this.y == rhs.y && this.z == rhs.z;
		}

		public override bool Equals(object o)
		{
			if (o is float3)
			{
				float3 @float = (float3)o;
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
			return string.Format("float3({0}f, {1}f, {2}f)", this.x, this.y, this.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("float3({0}f, {1}f, {2}f)", this.x.ToString(format, formatProvider), this.y.ToString(format, formatProvider), this.z.ToString(format, formatProvider));
		}

		public static implicit operator Vector3(float3 v)
		{
			return new Vector3(v.x, v.y, v.z);
		}

		public static implicit operator float3(Vector3 v)
		{
			return new float3(v.x, v.y, v.z);
		}

		public float x;

		public float y;

		public float z;

		public static readonly float3 zero;

		internal sealed class DebuggerProxy
		{
			public DebuggerProxy(float3 v)
			{
				this.x = v.x;
				this.y = v.y;
				this.z = v.z;
			}

			public float x;

			public float y;

			public float z;
		}
	}
}
