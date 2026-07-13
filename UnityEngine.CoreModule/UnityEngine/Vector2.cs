using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[Il2CppEagerStaticClassConstruction]
	[RequiredByNativeCode(Optional = true, GenerateProxy = true)]
	[NativeClass("Vector2f")]
	public struct Vector2 : IEquatable<Vector2>, IFormattable
	{
		public float this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				float num;
				if (index != 0)
				{
					if (index != 1)
					{
						throw new IndexOutOfRangeException("Invalid Vector2 index!");
					}
					num = this.y;
				}
				else
				{
					num = this.x;
				}
				return num;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				if (index != 0)
				{
					if (index != 1)
					{
						throw new IndexOutOfRangeException("Invalid Vector2 index!");
					}
					this.y = value;
				}
				else
				{
					this.x = value;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2(float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Set(float newX, float newY)
		{
			this.x = newX;
			this.y = newY;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
		{
			t = Mathf.Clamp01(t);
			Vector2 vector;
			vector.x = a.x + (b.x - a.x) * t;
			vector.y = a.y + (b.y - a.y) * t;
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Lerp(in Vector2 a, in Vector2 b, float t)
		{
			t = Mathf.Clamp01(t);
			Vector2 vector;
			vector.x = a.x + (b.x - a.x) * t;
			vector.y = a.y + (b.y - a.y) * t;
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 LerpUnclamped(Vector2 a, Vector2 b, float t)
		{
			return new Vector2
			{
				x = a.x + (b.x - a.x) * t,
				y = a.y + (b.y - a.y) * t
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 LerpUnclamped(in Vector2 a, in Vector2 b, float t)
		{
			return new Vector2
			{
				x = a.x + (b.x - a.x) * t,
				y = a.y + (b.y - a.y) * t
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 MoveTowards(Vector2 current, Vector2 target, float maxDistanceDelta)
		{
			float num = target.x - current.x;
			float num2 = target.y - current.y;
			float num3 = num * num + num2 * num2;
			bool flag = num3 == 0f || (maxDistanceDelta >= 0f && num3 <= maxDistanceDelta * maxDistanceDelta);
			Vector2 vector;
			if (flag)
			{
				vector = target;
			}
			else
			{
				float num4 = (float)Math.Sqrt((double)num3);
				Vector2 vector2;
				vector2.x = current.x + num / num4 * maxDistanceDelta;
				vector2.y = current.y + num2 / num4 * maxDistanceDelta;
				vector = vector2;
			}
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 MoveTowards(in Vector2 current, in Vector2 target, float maxDistanceDelta)
		{
			float num = target.x - current.x;
			float num2 = target.y - current.y;
			float num3 = num * num + num2 * num2;
			bool flag = num3 == 0f || (maxDistanceDelta >= 0f && num3 <= maxDistanceDelta * maxDistanceDelta);
			Vector2 vector;
			if (flag)
			{
				vector = target;
			}
			else
			{
				float num4 = (float)Math.Sqrt((double)num3);
				Vector2 vector2;
				vector2.x = current.x + num / num4 * maxDistanceDelta;
				vector2.y = current.y + num2 / num4 * maxDistanceDelta;
				vector = vector2;
			}
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Scale(Vector2 a, Vector2 b)
		{
			return new Vector2
			{
				x = a.x * b.x,
				y = a.y * b.y
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Scale(in Vector2 a, in Vector2 b)
		{
			return new Vector2
			{
				x = a.x * b.x,
				y = a.y * b.y
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Scale(Vector2 scale)
		{
			this.x *= scale.x;
			this.y *= scale.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Scale(in Vector2 scale)
		{
			this.x *= scale.x;
			this.y *= scale.y;
		}

		public static Vector2 Normalize(Vector2 value)
		{
			float magnitude = value.magnitude;
			return (magnitude > 1E-05f) ? new Vector2
			{
				x = value.x / magnitude,
				y = value.y / magnitude
			} : Vector2.zeroVector;
		}

		public static Vector2 Normalize(in Vector2 value)
		{
			float magnitude = value.magnitude;
			return (magnitude > 1E-05f) ? new Vector2
			{
				x = value.x / magnitude,
				y = value.y / magnitude
			} : Vector2.zeroVector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Normalize()
		{
			float magnitude = this.magnitude;
			bool flag = magnitude > 1E-05f;
			if (flag)
			{
				this.x /= magnitude;
				this.y /= magnitude;
			}
			else
			{
				this.x = 0f;
				this.y = 0f;
			}
		}

		public readonly Vector2 normalized
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector2.Normalize(in this);
			}
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
			return string.Format("({0}, {1})", this.x.ToString(format, formatProvider), this.y.ToString(format, formatProvider));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly int GetHashCode()
		{
			return this.x.GetHashCode() ^ (this.y.GetHashCode() << 2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly bool Equals(object other)
		{
			Vector2 vector;
			bool flag;
			if (other is Vector2)
			{
				vector = (Vector2)other;
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
		public readonly bool Equals(Vector2 other)
		{
			return this.x == other.x && this.y == other.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(in Vector2 other)
		{
			return this.x == other.x && this.y == other.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Reflect(Vector2 inDirection, Vector2 inNormal)
		{
			float num = -2f * Vector2.Dot(in inNormal, in inDirection);
			Vector2 vector;
			vector.x = num * inNormal.x + inDirection.x;
			vector.y = num * inNormal.y + inDirection.y;
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Reflect(in Vector2 inDirection, in Vector2 inNormal)
		{
			float num = -2f * Vector2.Dot(in inNormal, in inDirection);
			Vector2 vector;
			vector.x = num * inNormal.x + inDirection.x;
			vector.y = num * inNormal.y + inDirection.y;
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Perpendicular(Vector2 inDirection)
		{
			return new Vector2
			{
				x = -inDirection.y,
				y = inDirection.x
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Perpendicular(in Vector2 inDirection)
		{
			return new Vector2
			{
				x = -inDirection.y,
				y = inDirection.x
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Dot(Vector2 lhs, Vector2 rhs)
		{
			return lhs.x * rhs.x + lhs.y * rhs.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Dot(in Vector2 lhs, in Vector2 rhs)
		{
			return lhs.x * rhs.x + lhs.y * rhs.y;
		}

		public readonly float magnitude
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (float)Math.Sqrt((double)(this.x * this.x + this.y * this.y));
			}
		}

		public readonly float sqrMagnitude
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.x * this.x + this.y * this.y;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Angle(Vector2 from, Vector2 to)
		{
			float num = from.sqrMagnitude * to.sqrMagnitude;
			bool flag = num < 1E-30f;
			float num2;
			if (flag)
			{
				num2 = 0f;
			}
			else
			{
				num = (float)Math.Sqrt((double)num);
				float num3 = Mathf.Clamp(Vector2.Dot(in from, in to) / num, -1f, 1f);
				num2 = (float)Math.Acos((double)num3) * 57.29578f;
			}
			return num2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Angle(in Vector2 from, in Vector2 to)
		{
			float num = from.sqrMagnitude * to.sqrMagnitude;
			bool flag = num < 1E-30f;
			float num2;
			if (flag)
			{
				num2 = 0f;
			}
			else
			{
				num = (float)Math.Sqrt((double)num);
				float num3 = Mathf.Clamp(Vector2.Dot(in from, in to) / num, -1f, 1f);
				num2 = (float)Math.Acos((double)num3) * 57.29578f;
			}
			return num2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float SignedAngle(Vector2 from, Vector2 to)
		{
			float num = Vector2.Angle(in from, in to);
			float num2 = Mathf.Sign(from.x * to.y - from.y * to.x);
			return num * num2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float SignedAngle(in Vector2 from, in Vector2 to)
		{
			float num = Vector2.Angle(in from, in to);
			float num2 = Mathf.Sign(from.x * to.y - from.y * to.x);
			return num * num2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Distance(Vector2 a, Vector2 b)
		{
			float num = a.x - b.x;
			float num2 = a.y - b.y;
			return (float)Math.Sqrt((double)(num * num + num2 * num2));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Distance(in Vector2 a, in Vector2 b)
		{
			float num = a.x - b.x;
			float num2 = a.y - b.y;
			return (float)Math.Sqrt((double)(num * num + num2 * num2));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 ClampMagnitude(Vector2 vector, float maxLength)
		{
			float sqrMagnitude = vector.sqrMagnitude;
			bool flag = sqrMagnitude > maxLength * maxLength;
			Vector2 vector3;
			if (flag)
			{
				float num = (float)Math.Sqrt((double)sqrMagnitude);
				float num2 = vector.x / num;
				float num3 = vector.y / num;
				Vector2 vector2;
				vector2.x = num2 * maxLength;
				vector2.y = num3 * maxLength;
				vector3 = vector2;
			}
			else
			{
				vector3 = vector;
			}
			return vector3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 ClampMagnitude(in Vector2 vector, float maxLength)
		{
			float sqrMagnitude = vector.sqrMagnitude;
			bool flag = sqrMagnitude > maxLength * maxLength;
			Vector2 vector3;
			if (flag)
			{
				float num = (float)Math.Sqrt((double)sqrMagnitude);
				float num2 = vector.x / num;
				float num3 = vector.y / num;
				Vector2 vector2;
				vector2.x = num2 * maxLength;
				vector2.y = num3 * maxLength;
				vector3 = vector2;
			}
			else
			{
				vector3 = vector;
			}
			return vector3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float SqrMagnitude(Vector2 a)
		{
			return a.sqrMagnitude;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float SqrMagnitude(in Vector2 a)
		{
			return a.sqrMagnitude;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly float SqrMagnitude()
		{
			return this.sqrMagnitude;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Min(Vector2 lhs, Vector2 rhs)
		{
			return new Vector2
			{
				x = Mathf.Min(lhs.x, rhs.x),
				y = Mathf.Min(lhs.y, rhs.y)
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Min(in Vector2 lhs, in Vector2 rhs)
		{
			return new Vector2
			{
				x = Mathf.Min(lhs.x, rhs.x),
				y = Mathf.Min(lhs.y, rhs.y)
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Max(Vector2 lhs, Vector2 rhs)
		{
			return new Vector2
			{
				x = Mathf.Max(lhs.x, rhs.x),
				y = Mathf.Max(lhs.y, rhs.y)
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Max(in Vector2 lhs, in Vector2 rhs)
		{
			return new Vector2
			{
				x = Mathf.Max(lhs.x, rhs.x),
				y = Mathf.Max(lhs.y, rhs.y)
			};
		}

		[ExcludeFromDocs]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 SmoothDamp(Vector2 current, Vector2 target, ref Vector2 currentVelocity, float smoothTime, float maxSpeed)
		{
			return Vector2.SmoothDamp(in current, in target, ref currentVelocity, smoothTime, maxSpeed, Time.deltaTime);
		}

		[ExcludeFromDocs]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 SmoothDamp(in Vector2 current, in Vector2 target, ref Vector2 currentVelocity, float smoothTime, float maxSpeed)
		{
			return Vector2.SmoothDamp(in current, in target, ref currentVelocity, smoothTime, maxSpeed, Time.deltaTime);
		}

		[ExcludeFromDocs]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 SmoothDamp(Vector2 current, Vector2 target, ref Vector2 currentVelocity, float smoothTime)
		{
			return Vector2.SmoothDamp(in current, in target, ref currentVelocity, smoothTime, float.PositiveInfinity, Time.deltaTime);
		}

		[ExcludeFromDocs]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 SmoothDamp(in Vector2 current, in Vector2 target, ref Vector2 currentVelocity, float smoothTime)
		{
			return Vector2.SmoothDamp(in current, in target, ref currentVelocity, smoothTime, float.PositiveInfinity, Time.deltaTime);
		}

		public static Vector2 SmoothDamp(Vector2 current, Vector2 target, ref Vector2 currentVelocity, float smoothTime, [DefaultValue("Mathf.Infinity")] float maxSpeed, [DefaultValue("Time.deltaTime")] float deltaTime)
		{
			smoothTime = Mathf.Max(0.0001f, smoothTime);
			float num = 2f / smoothTime;
			float num2 = num * deltaTime;
			float num3 = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
			float num4 = current.x - target.x;
			float num5 = current.y - target.y;
			float num6 = maxSpeed * smoothTime;
			float num7 = num6 * num6;
			float num8 = num4 * num4 + num5 * num5;
			bool flag = num8 > num7;
			if (flag)
			{
				float num9 = (float)Math.Sqrt((double)num8);
				num4 = num4 / num9 * num6;
				num5 = num5 / num9 * num6;
			}
			float num10 = current.x - num4;
			float num11 = current.y - num5;
			float num12 = (currentVelocity.x + num * num4) * deltaTime;
			float num13 = (currentVelocity.y + num * num5) * deltaTime;
			currentVelocity.x = (currentVelocity.x - num * num12) * num3;
			currentVelocity.y = (currentVelocity.y - num * num13) * num3;
			float num14 = num10 + (num4 + num12) * num3;
			float num15 = num11 + (num5 + num13) * num3;
			float num16 = target.x - current.x;
			float num17 = target.y - current.y;
			float num18 = num14 - target.x;
			float num19 = num15 - target.y;
			bool flag2 = num16 * num18 + num17 * num19 > 0f;
			if (flag2)
			{
				num14 = target.x;
				num15 = target.y;
				currentVelocity.x = (num14 - target.x) / deltaTime;
				currentVelocity.y = (num15 - target.y) / deltaTime;
			}
			Vector2 vector;
			vector.x = num14;
			vector.y = num15;
			return vector;
		}

		public static Vector2 SmoothDamp(in Vector2 current, in Vector2 target, ref Vector2 currentVelocity, float smoothTime, [DefaultValue("Mathf.Infinity")] float maxSpeed, [DefaultValue("Time.deltaTime")] float deltaTime)
		{
			smoothTime = Mathf.Max(0.0001f, smoothTime);
			float num = 2f / smoothTime;
			float num2 = num * deltaTime;
			float num3 = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
			float num4 = current.x - target.x;
			float num5 = current.y - target.y;
			float num6 = maxSpeed * smoothTime;
			float num7 = num6 * num6;
			float num8 = num4 * num4 + num5 * num5;
			bool flag = num8 > num7;
			if (flag)
			{
				float num9 = (float)Math.Sqrt((double)num8);
				num4 = num4 / num9 * num6;
				num5 = num5 / num9 * num6;
			}
			float num10 = current.x - num4;
			float num11 = current.y - num5;
			float num12 = (currentVelocity.x + num * num4) * deltaTime;
			float num13 = (currentVelocity.y + num * num5) * deltaTime;
			currentVelocity.x = (currentVelocity.x - num * num12) * num3;
			currentVelocity.y = (currentVelocity.y - num * num13) * num3;
			float num14 = num10 + (num4 + num12) * num3;
			float num15 = num11 + (num5 + num13) * num3;
			float num16 = target.x - current.x;
			float num17 = target.y - current.y;
			float num18 = num14 - target.x;
			float num19 = num15 - target.y;
			bool flag2 = num16 * num18 + num17 * num19 > 0f;
			if (flag2)
			{
				num14 = target.x;
				num15 = target.y;
				currentVelocity.x = (num14 - target.x) / deltaTime;
				currentVelocity.y = (num15 - target.y) / deltaTime;
			}
			Vector2 vector;
			vector.x = num14;
			vector.y = num15;
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 operator +(Vector2 a, Vector2 b)
		{
			return new Vector2
			{
				x = a.x + b.x,
				y = a.y + b.y
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 operator -(Vector2 a, Vector2 b)
		{
			return new Vector2
			{
				x = a.x - b.x,
				y = a.y - b.y
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 operator *(Vector2 a, Vector2 b)
		{
			return new Vector2
			{
				x = a.x * b.x,
				y = a.y * b.y
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 operator /(Vector2 a, Vector2 b)
		{
			return new Vector2
			{
				x = a.x / b.x,
				y = a.y / b.y
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 operator -(Vector2 a)
		{
			return new Vector2
			{
				x = -a.x,
				y = -a.y
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 operator *(Vector2 a, float d)
		{
			return new Vector2
			{
				x = a.x * d,
				y = a.y * d
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 operator *(float d, Vector2 a)
		{
			return new Vector2
			{
				x = a.x * d,
				y = a.y * d
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 operator /(Vector2 a, float d)
		{
			return new Vector2
			{
				x = a.x / d,
				y = a.y / d
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Vector2 lhs, Vector2 rhs)
		{
			float num = lhs.x - rhs.x;
			float num2 = lhs.y - rhs.y;
			return num * num + num2 * num2 < 9.9999994E-11f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Vector2 lhs, Vector2 rhs)
		{
			return !(lhs == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector2(Vector3 v)
		{
			return new Vector2
			{
				x = v.x,
				y = v.y
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector3(Vector2 v)
		{
			return new Vector3
			{
				x = v.x,
				y = v.y,
				z = 0f
			};
		}

		public static Vector2 zero
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector2.zeroVector;
			}
		}

		public static Vector2 one
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector2.oneVector;
			}
		}

		public static Vector2 up
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector2.upVector;
			}
		}

		public static Vector2 down
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector2.downVector;
			}
		}

		public static Vector2 left
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector2.leftVector;
			}
		}

		public static Vector2 right
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector2.rightVector;
			}
		}

		public static Vector2 positiveInfinity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector2.positiveInfinityVector;
			}
		}

		public static Vector2 negativeInfinity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector2.negativeInfinityVector;
			}
		}

		public float x;

		public float y;

		private static readonly Vector2 zeroVector = new Vector2(0f, 0f);

		private static readonly Vector2 oneVector = new Vector2(1f, 1f);

		private static readonly Vector2 upVector = new Vector2(0f, 1f);

		private static readonly Vector2 downVector = new Vector2(0f, -1f);

		private static readonly Vector2 leftVector = new Vector2(-1f, 0f);

		private static readonly Vector2 rightVector = new Vector2(1f, 0f);

		private static readonly Vector2 positiveInfinityVector = new Vector2(float.PositiveInfinity, float.PositiveInfinity);

		private static readonly Vector2 negativeInfinityVector = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

		public const float kEpsilon = 1E-05f;

		public const float kEpsilonNormalSqrt = 1E-15f;
	}
}
