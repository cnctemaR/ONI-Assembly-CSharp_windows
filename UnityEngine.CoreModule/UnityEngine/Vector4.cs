using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Math/Vector4.h")]
	[NativeClass("Vector4f")]
	[RequiredByNativeCode(Optional = true, GenerateProxy = true)]
	[Il2CppEagerStaticClassConstruction]
	public struct Vector4 : IEquatable<Vector4>, IFormattable
	{
		public float this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				float num;
				switch (index)
				{
				case 0:
					num = this.x;
					break;
				case 1:
					num = this.y;
					break;
				case 2:
					num = this.z;
					break;
				case 3:
					num = this.w;
					break;
				default:
					throw new IndexOutOfRangeException("Invalid Vector4 index!");
				}
				return num;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				switch (index)
				{
				case 0:
					this.x = value;
					break;
				case 1:
					this.y = value;
					break;
				case 2:
					this.z = value;
					break;
				case 3:
					this.w = value;
					break;
				default:
					throw new IndexOutOfRangeException("Invalid Vector4 index!");
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector4(float x, float y, float z, float w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector4(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = 0f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector4(float x, float y)
		{
			this.x = x;
			this.y = y;
			this.z = 0f;
			this.w = 0f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Set(float newX, float newY, float newZ, float newW)
		{
			this.x = newX;
			this.y = newY;
			this.z = newZ;
			this.w = newW;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 Lerp(Vector4 a, Vector4 b, float t)
		{
			t = Mathf.Clamp01(t);
			return new Vector4
			{
				x = a.x + (b.x - a.x) * t,
				y = a.y + (b.y - a.y) * t,
				z = a.z + (b.z - a.z) * t,
				w = a.w + (b.w - a.w) * t
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 Lerp(in Vector4 a, in Vector4 b, float t)
		{
			t = Mathf.Clamp01(t);
			return new Vector4
			{
				x = a.x + (b.x - a.x) * t,
				y = a.y + (b.y - a.y) * t,
				z = a.z + (b.z - a.z) * t,
				w = a.w + (b.w - a.w) * t
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 LerpUnclamped(Vector4 a, Vector4 b, float t)
		{
			return new Vector4
			{
				x = a.x + (b.x - a.x) * t,
				y = a.y + (b.y - a.y) * t,
				z = a.z + (b.z - a.z) * t,
				w = a.w + (b.w - a.w) * t
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 LerpUnclamped(in Vector4 a, in Vector4 b, float t)
		{
			return new Vector4
			{
				x = a.x + (b.x - a.x) * t,
				y = a.y + (b.y - a.y) * t,
				z = a.z + (b.z - a.z) * t,
				w = a.w + (b.w - a.w) * t
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 MoveTowards(Vector4 current, Vector4 target, float maxDistanceDelta)
		{
			float num = target.x - current.x;
			float num2 = target.y - current.y;
			float num3 = target.z - current.z;
			float num4 = target.w - current.w;
			float num5 = num * num + num2 * num2 + num3 * num3 + num4 * num4;
			bool flag = num5 == 0f || (maxDistanceDelta >= 0f && num5 <= maxDistanceDelta * maxDistanceDelta);
			Vector4 vector;
			if (flag)
			{
				vector = target;
			}
			else
			{
				float num6 = (float)Math.Sqrt((double)num5);
				Vector4 vector2;
				vector2.x = current.x + num / num6 * maxDistanceDelta;
				vector2.y = current.y + num2 / num6 * maxDistanceDelta;
				vector2.z = current.z + num3 / num6 * maxDistanceDelta;
				vector2.w = current.w + num4 / num6 * maxDistanceDelta;
				vector = vector2;
			}
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 MoveTowards(in Vector4 current, in Vector4 target, float maxDistanceDelta)
		{
			float num = target.x - current.x;
			float num2 = target.y - current.y;
			float num3 = target.z - current.z;
			float num4 = target.w - current.w;
			float num5 = num * num + num2 * num2 + num3 * num3 + num4 * num4;
			bool flag = num5 == 0f || (maxDistanceDelta >= 0f && num5 <= maxDistanceDelta * maxDistanceDelta);
			Vector4 vector;
			if (flag)
			{
				vector = target;
			}
			else
			{
				float num6 = (float)Math.Sqrt((double)num5);
				Vector4 vector2;
				vector2.x = current.x + num / num6 * maxDistanceDelta;
				vector2.y = current.y + num2 / num6 * maxDistanceDelta;
				vector2.z = current.z + num3 / num6 * maxDistanceDelta;
				vector2.w = current.w + num4 / num6 * maxDistanceDelta;
				vector = vector2;
			}
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 Scale(Vector4 a, Vector4 b)
		{
			return new Vector4
			{
				x = a.x * b.x,
				y = a.y * b.y,
				z = a.z * b.z,
				w = a.w * b.w
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 Scale(in Vector4 a, in Vector4 b)
		{
			return new Vector4
			{
				x = a.x * b.x,
				y = a.y * b.y,
				z = a.z * b.z,
				w = a.w * b.w
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Scale(Vector4 scale)
		{
			this.x *= scale.x;
			this.y *= scale.y;
			this.z *= scale.z;
			this.w *= scale.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Scale(in Vector4 scale)
		{
			this.x *= scale.x;
			this.y *= scale.y;
			this.z *= scale.z;
			this.w *= scale.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly int GetHashCode()
		{
			return this.x.GetHashCode() ^ (this.y.GetHashCode() << 2) ^ (this.z.GetHashCode() >> 2) ^ (this.w.GetHashCode() >> 1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly bool Equals(object other)
		{
			Vector4 vector;
			bool flag;
			if (other is Vector4)
			{
				vector = (Vector4)other;
				flag = true;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			return flag2 && this.Equals(in vector);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(Vector4 other)
		{
			return this.x == other.x && this.y == other.y && this.z == other.z && this.w == other.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(in Vector4 other)
		{
			return this.x == other.x && this.y == other.y && this.z == other.z && this.w == other.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 Normalize(Vector4 a)
		{
			float magnitude = a.magnitude;
			return (magnitude > 1E-05f) ? new Vector4
			{
				x = a.x / magnitude,
				y = a.y / magnitude,
				z = a.z / magnitude,
				w = a.w / magnitude
			} : Vector4.zeroVector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 Normalize(in Vector4 a)
		{
			float magnitude = a.magnitude;
			return (magnitude > 1E-05f) ? new Vector4
			{
				x = a.x / magnitude,
				y = a.y / magnitude,
				z = a.z / magnitude,
				w = a.w / magnitude
			} : Vector4.zeroVector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Normalize()
		{
			float num = Vector4.Magnitude(in this);
			bool flag = num > 1E-05f;
			if (flag)
			{
				this.x /= num;
				this.y /= num;
				this.z /= num;
				this.w /= num;
			}
			else
			{
				this.x = 0f;
				this.y = 0f;
				this.z = 0f;
				this.w = 0f;
			}
		}

		public readonly Vector4 normalized
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector4.Normalize(in this);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Dot(Vector4 a, Vector4 b)
		{
			return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Dot(in Vector4 a, in Vector4 b)
		{
			return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 Project(Vector4 a, Vector4 b)
		{
			return b * (Vector4.Dot(in a, in b) / Vector4.Dot(in b, in b));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 Project(in Vector4 a, in Vector4 b)
		{
			return b * (Vector4.Dot(in a, in b) / Vector4.Dot(in b, in b));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Distance(Vector4 a, Vector4 b)
		{
			return Vector4.Magnitude(a - b);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Distance(in Vector4 a, in Vector4 b)
		{
			return Vector4.Magnitude(a - b);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Magnitude(Vector4 a)
		{
			return (float)Math.Sqrt((double)Vector4.Dot(in a, in a));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Magnitude(in Vector4 a)
		{
			return (float)Math.Sqrt((double)Vector4.Dot(in a, in a));
		}

		public readonly float magnitude
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (float)Math.Sqrt((double)Vector4.Dot(in this, in this));
			}
		}

		public readonly float sqrMagnitude
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector4.Dot(in this, in this);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 Min(Vector4 lhs, Vector4 rhs)
		{
			return new Vector4
			{
				x = Mathf.Min(lhs.x, rhs.x),
				y = Mathf.Min(lhs.y, rhs.y),
				z = Mathf.Min(lhs.z, rhs.z),
				w = Mathf.Min(lhs.w, rhs.w)
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 Min(in Vector4 lhs, in Vector4 rhs)
		{
			return new Vector4
			{
				x = Mathf.Min(lhs.x, rhs.x),
				y = Mathf.Min(lhs.y, rhs.y),
				z = Mathf.Min(lhs.z, rhs.z),
				w = Mathf.Min(lhs.w, rhs.w)
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 Max(Vector4 lhs, Vector4 rhs)
		{
			return new Vector4
			{
				x = Mathf.Max(lhs.x, rhs.x),
				y = Mathf.Max(lhs.y, rhs.y),
				z = Mathf.Max(lhs.z, rhs.z),
				w = Mathf.Max(lhs.w, rhs.w)
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 Max(in Vector4 lhs, in Vector4 rhs)
		{
			return new Vector4
			{
				x = Mathf.Max(lhs.x, rhs.x),
				y = Mathf.Max(lhs.y, rhs.y),
				z = Mathf.Max(lhs.z, rhs.z),
				w = Mathf.Max(lhs.w, rhs.w)
			};
		}

		public static Vector4 zero
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector4.zeroVector;
			}
		}

		public static Vector4 one
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector4.oneVector;
			}
		}

		public static Vector4 positiveInfinity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector4.positiveInfinityVector;
			}
		}

		public static Vector4 negativeInfinity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector4.negativeInfinityVector;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 operator +(Vector4 a, Vector4 b)
		{
			return new Vector4
			{
				x = a.x + b.x,
				y = a.y + b.y,
				z = a.z + b.z,
				w = a.w + b.w
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 operator -(Vector4 a, Vector4 b)
		{
			return new Vector4
			{
				x = a.x - b.x,
				y = a.y - b.y,
				z = a.z - b.z,
				w = a.w - b.w
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 operator -(Vector4 a)
		{
			return new Vector4
			{
				x = -a.x,
				y = -a.y,
				z = -a.z,
				w = -a.w
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 operator *(Vector4 a, float d)
		{
			return new Vector4
			{
				x = a.x * d,
				y = a.y * d,
				z = a.z * d,
				w = a.w * d
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 operator *(float d, Vector4 a)
		{
			return new Vector4
			{
				x = a.x * d,
				y = a.y * d,
				z = a.z * d,
				w = a.w * d
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 operator /(Vector4 a, float d)
		{
			return new Vector4
			{
				x = a.x / d,
				y = a.y / d,
				z = a.z / d,
				w = a.w / d
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Vector4 lhs, Vector4 rhs)
		{
			float num = lhs.x - rhs.x;
			float num2 = lhs.y - rhs.y;
			float num3 = lhs.z - rhs.z;
			float num4 = lhs.w - rhs.w;
			float num5 = num * num + num2 * num2 + num3 * num3 + num4 * num4;
			return num5 < 9.9999994E-11f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Vector4 lhs, Vector4 rhs)
		{
			return !(lhs == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector4(Vector3 v)
		{
			return new Vector4
			{
				x = v.x,
				y = v.y,
				z = v.z,
				w = 0f
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector3(Vector4 v)
		{
			return new Vector3
			{
				x = v.x,
				y = v.y,
				z = v.z
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector4(Vector2 v)
		{
			return new Vector4
			{
				x = v.x,
				y = v.y,
				z = 0f,
				w = 0f
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector2(Vector4 v)
		{
			return new Vector2
			{
				x = v.x,
				y = v.y
			};
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
			bool flag = string.IsNullOrEmpty(format);
			if (flag)
			{
				format = "F2";
			}
			bool flag2 = formatProvider == null;
			if (flag2)
			{
				formatProvider = CultureInfo.InvariantCulture.NumberFormat;
			}
			return string.Format("({0}, {1}, {2}, {3})", new object[]
			{
				this.x.ToString(format, formatProvider),
				this.y.ToString(format, formatProvider),
				this.z.ToString(format, formatProvider),
				this.w.ToString(format, formatProvider)
			});
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float SqrMagnitude(Vector4 a)
		{
			return a.sqrMagnitude;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float SqrMagnitude(in Vector4 a)
		{
			return a.sqrMagnitude;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly float SqrMagnitude()
		{
			return this.sqrMagnitude;
		}

		public const float kEpsilon = 1E-05f;

		public float x;

		public float y;

		public float z;

		public float w;

		private static readonly Vector4 zeroVector = new Vector4(0f, 0f, 0f, 0f);

		private static readonly Vector4 oneVector = new Vector4(1f, 1f, 1f, 1f);

		private static readonly Vector4 positiveInfinityVector = new Vector4(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

		private static readonly Vector4 negativeInfinityVector = new Vector4(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
	}
}
