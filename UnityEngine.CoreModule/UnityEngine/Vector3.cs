using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[Il2CppEagerStaticClassConstruction]
	[RequiredByNativeCode(Optional = true, GenerateProxy = true)]
	[NativeClass("Vector3f")]
	[NativeHeader("Runtime/Math/Vector3.h")]
	[NativeType(Header = "Runtime/Math/Vector3.h")]
	[NativeHeader("Runtime/Math/MathScripting.h")]
	public struct Vector3 : IEquatable<Vector3>, IFormattable
	{
		[FreeFunction("VectorScripting::Slerp", IsThreadSafe = true)]
		private static Vector3 Internal_Slerp(in Vector3 a, in Vector3 b, float t)
		{
			Vector3 vector;
			Vector3.Internal_Slerp_Injected(in a, in b, t, out vector);
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 Slerp(Vector3 a, Vector3 b, float t)
		{
			return Vector3.Internal_Slerp(in a, in b, t);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 Slerp(in Vector3 a, in Vector3 b, float t)
		{
			return Vector3.Internal_Slerp(in a, in b, t);
		}

		[FreeFunction("VectorScripting::SlerpUnclamped", IsThreadSafe = true)]
		private static Vector3 Internal_SlerpUnclamped(in Vector3 a, in Vector3 b, float t)
		{
			Vector3 vector;
			Vector3.Internal_SlerpUnclamped_Injected(in a, in b, t, out vector);
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 SlerpUnclamped(Vector3 a, Vector3 b, float t)
		{
			return Vector3.Internal_SlerpUnclamped(in a, in b, t);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 SlerpUnclamped(in Vector3 a, in Vector3 b, float t)
		{
			return Vector3.Internal_SlerpUnclamped(in a, in b, t);
		}

		[FreeFunction("VectorScripting::OrthoNormalize", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void OrthoNormalize2(ref Vector3 a, ref Vector3 b);

		public static void OrthoNormalize(ref Vector3 normal, ref Vector3 tangent)
		{
			Vector3.OrthoNormalize2(ref normal, ref tangent);
		}

		[FreeFunction("VectorScripting::OrthoNormalize", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void OrthoNormalize3(ref Vector3 a, ref Vector3 b, ref Vector3 c);

		public static void OrthoNormalize(ref Vector3 normal, ref Vector3 tangent, ref Vector3 binormal)
		{
			Vector3.OrthoNormalize3(ref normal, ref tangent, ref binormal);
		}

		[FreeFunction("VectorScripting::RotateTowards", IsThreadSafe = true)]
		private static Vector3 Internal_RotateTowards(in Vector3 current, in Vector3 target, float maxRadiansDelta, float maxMagnitudeDelta)
		{
			Vector3 vector;
			Vector3.Internal_RotateTowards_Injected(in current, in target, maxRadiansDelta, maxMagnitudeDelta, out vector);
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 RotateTowards(Vector3 current, Vector3 target, float maxRadiansDelta, float maxMagnitudeDelta)
		{
			return Vector3.Internal_RotateTowards(in current, in target, maxRadiansDelta, maxMagnitudeDelta);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 RotateTowards(in Vector3 current, in Vector3 target, float maxRadiansDelta, float maxMagnitudeDelta)
		{
			return Vector3.Internal_RotateTowards(in current, in target, maxRadiansDelta, maxMagnitudeDelta);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
		{
			t = Mathf.Clamp01(t);
			Vector3 vector;
			vector.x = a.x + (b.x - a.x) * t;
			vector.y = a.y + (b.y - a.y) * t;
			vector.z = a.z + (b.z - a.z) * t;
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 Lerp(in Vector3 a, in Vector3 b, float t)
		{
			t = Mathf.Clamp01(t);
			Vector3 vector;
			vector.x = a.x + (b.x - a.x) * t;
			vector.y = a.y + (b.y - a.y) * t;
			vector.z = a.z + (b.z - a.z) * t;
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 LerpUnclamped(Vector3 a, Vector3 b, float t)
		{
			return new Vector3
			{
				x = a.x + (b.x - a.x) * t,
				y = a.y + (b.y - a.y) * t,
				z = a.z + (b.z - a.z) * t
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 LerpUnclamped(in Vector3 a, in Vector3 b, float t)
		{
			return new Vector3
			{
				x = a.x + (b.x - a.x) * t,
				y = a.y + (b.y - a.y) * t,
				z = a.z + (b.z - a.z) * t
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 MoveTowards(Vector3 current, Vector3 target, float maxDistanceDelta)
		{
			float num = target.x - current.x;
			float num2 = target.y - current.y;
			float num3 = target.z - current.z;
			float num4 = num * num + num2 * num2 + num3 * num3;
			bool flag = num4 == 0f || (maxDistanceDelta >= 0f && num4 <= maxDistanceDelta * maxDistanceDelta);
			Vector3 vector;
			if (flag)
			{
				vector = target;
			}
			else
			{
				float num5 = (float)Math.Sqrt((double)num4);
				Vector3 vector2;
				vector2.x = current.x + num / num5 * maxDistanceDelta;
				vector2.y = current.y + num2 / num5 * maxDistanceDelta;
				vector2.z = current.z + num3 / num5 * maxDistanceDelta;
				vector = vector2;
			}
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 MoveTowards(in Vector3 current, in Vector3 target, float maxDistanceDelta)
		{
			float num = target.x - current.x;
			float num2 = target.y - current.y;
			float num3 = target.z - current.z;
			float num4 = num * num + num2 * num2 + num3 * num3;
			bool flag = num4 == 0f || (maxDistanceDelta >= 0f && num4 <= maxDistanceDelta * maxDistanceDelta);
			Vector3 vector;
			if (flag)
			{
				vector = target;
			}
			else
			{
				float num5 = (float)Math.Sqrt((double)num4);
				Vector3 vector2;
				vector2.x = current.x + num / num5 * maxDistanceDelta;
				vector2.y = current.y + num2 / num5 * maxDistanceDelta;
				vector2.z = current.z + num3 / num5 * maxDistanceDelta;
				vector = vector2;
			}
			return vector;
		}

		[ExcludeFromDocs]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, float maxSpeed)
		{
			float deltaTime = Time.deltaTime;
			return Vector3.SmoothDamp(in current, in target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
		}

		[ExcludeFromDocs]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 SmoothDamp(in Vector3 current, in Vector3 target, ref Vector3 currentVelocity, float smoothTime, float maxSpeed)
		{
			float deltaTime = Time.deltaTime;
			return Vector3.SmoothDamp(in current, in target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
		}

		[ExcludeFromDocs]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime)
		{
			float deltaTime = Time.deltaTime;
			float positiveInfinity = float.PositiveInfinity;
			return Vector3.SmoothDamp(in current, in target, ref currentVelocity, smoothTime, positiveInfinity, deltaTime);
		}

		[ExcludeFromDocs]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 SmoothDamp(in Vector3 current, in Vector3 target, ref Vector3 currentVelocity, float smoothTime)
		{
			float deltaTime = Time.deltaTime;
			float positiveInfinity = float.PositiveInfinity;
			return Vector3.SmoothDamp(in current, in target, ref currentVelocity, smoothTime, positiveInfinity, deltaTime);
		}

		public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, [DefaultValue("Mathf.Infinity")] float maxSpeed, [DefaultValue("Time.deltaTime")] float deltaTime)
		{
			smoothTime = Mathf.Max(0.0001f, smoothTime);
			float num = 2f / smoothTime;
			float num2 = num * deltaTime;
			float num3 = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
			float num4 = current.x - target.x;
			float num5 = current.y - target.y;
			float num6 = current.z - target.z;
			float num7 = maxSpeed * smoothTime;
			float num8 = num7 * num7;
			float num9 = num4 * num4 + num5 * num5 + num6 * num6;
			bool flag = num9 > num8;
			if (flag)
			{
				float num10 = (float)Math.Sqrt((double)num9);
				num4 = num4 / num10 * num7;
				num5 = num5 / num10 * num7;
				num6 = num6 / num10 * num7;
			}
			float num11 = current.x - num4;
			float num12 = current.y - num5;
			float num13 = current.z - num6;
			float num14 = (currentVelocity.x + num * num4) * deltaTime;
			float num15 = (currentVelocity.y + num * num5) * deltaTime;
			float num16 = (currentVelocity.z + num * num6) * deltaTime;
			currentVelocity.x = (currentVelocity.x - num * num14) * num3;
			currentVelocity.y = (currentVelocity.y - num * num15) * num3;
			currentVelocity.z = (currentVelocity.z - num * num16) * num3;
			float num17 = num11 + (num4 + num14) * num3;
			float num18 = num12 + (num5 + num15) * num3;
			float num19 = num13 + (num6 + num16) * num3;
			float num20 = target.x - current.x;
			float num21 = target.y - current.y;
			float num22 = target.z - current.z;
			float num23 = num17 - target.x;
			float num24 = num18 - target.y;
			float num25 = num19 - target.z;
			bool flag2 = num20 * num23 + num21 * num24 + num22 * num25 > 0f;
			if (flag2)
			{
				num17 = target.x;
				num18 = target.y;
				num19 = target.z;
				currentVelocity.x = (num17 - target.x) / deltaTime;
				currentVelocity.y = (num18 - target.y) / deltaTime;
				currentVelocity.z = (num19 - target.z) / deltaTime;
			}
			Vector3 vector;
			vector.x = num17;
			vector.y = num18;
			vector.z = num19;
			return vector;
		}

		public static Vector3 SmoothDamp(in Vector3 current, in Vector3 target, ref Vector3 currentVelocity, float smoothTime, [DefaultValue("Mathf.Infinity")] float maxSpeed, [DefaultValue("Time.deltaTime")] float deltaTime)
		{
			smoothTime = Mathf.Max(0.0001f, smoothTime);
			float num = 2f / smoothTime;
			float num2 = num * deltaTime;
			float num3 = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
			float num4 = current.x - target.x;
			float num5 = current.y - target.y;
			float num6 = current.z - target.z;
			float num7 = maxSpeed * smoothTime;
			float num8 = num7 * num7;
			float num9 = num4 * num4 + num5 * num5 + num6 * num6;
			bool flag = num9 > num8;
			if (flag)
			{
				float num10 = (float)Math.Sqrt((double)num9);
				num4 = num4 / num10 * num7;
				num5 = num5 / num10 * num7;
				num6 = num6 / num10 * num7;
			}
			float num11 = current.x - num4;
			float num12 = current.y - num5;
			float num13 = current.z - num6;
			float num14 = (currentVelocity.x + num * num4) * deltaTime;
			float num15 = (currentVelocity.y + num * num5) * deltaTime;
			float num16 = (currentVelocity.z + num * num6) * deltaTime;
			currentVelocity.x = (currentVelocity.x - num * num14) * num3;
			currentVelocity.y = (currentVelocity.y - num * num15) * num3;
			currentVelocity.z = (currentVelocity.z - num * num16) * num3;
			float num17 = num11 + (num4 + num14) * num3;
			float num18 = num12 + (num5 + num15) * num3;
			float num19 = num13 + (num6 + num16) * num3;
			float num20 = target.x - current.x;
			float num21 = target.y - current.y;
			float num22 = target.z - current.z;
			float num23 = num17 - target.x;
			float num24 = num18 - target.y;
			float num25 = num19 - target.z;
			bool flag2 = num20 * num23 + num21 * num24 + num22 * num25 > 0f;
			if (flag2)
			{
				num17 = target.x;
				num18 = target.y;
				num19 = target.z;
				currentVelocity.x = (num17 - target.x) / deltaTime;
				currentVelocity.y = (num18 - target.y) / deltaTime;
				currentVelocity.z = (num19 - target.z) / deltaTime;
			}
			Vector3 vector;
			vector.x = num17;
			vector.y = num18;
			vector.z = num19;
			return vector;
		}

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
				default:
					throw new IndexOutOfRangeException("Invalid Vector3 index!");
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
				default:
					throw new IndexOutOfRangeException("Invalid Vector3 index!");
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3(float x, float y)
		{
			this.x = x;
			this.y = y;
			this.z = 0f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Set(float newX, float newY, float newZ)
		{
			this.x = newX;
			this.y = newY;
			this.z = newZ;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 Scale(Vector3 a, Vector3 b)
		{
			return new Vector3
			{
				x = a.x * b.x,
				y = a.y * b.y,
				z = a.z * b.z
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 Scale(in Vector3 a, in Vector3 b)
		{
			return new Vector3
			{
				x = a.x * b.x,
				y = a.y * b.y,
				z = a.z * b.z
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Scale(Vector3 scale)
		{
			this.x *= scale.x;
			this.y *= scale.y;
			this.z *= scale.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Scale(in Vector3 scale)
		{
			this.x *= scale.x;
			this.y *= scale.y;
			this.z *= scale.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 Cross(Vector3 lhs, Vector3 rhs)
		{
			return new Vector3
			{
				x = lhs.y * rhs.z - lhs.z * rhs.y,
				y = lhs.z * rhs.x - lhs.x * rhs.z,
				z = lhs.x * rhs.y - lhs.y * rhs.x
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 Cross(in Vector3 lhs, in Vector3 rhs)
		{
			return new Vector3
			{
				x = lhs.y * rhs.z - lhs.z * rhs.y,
				y = lhs.z * rhs.x - lhs.x * rhs.z,
				z = lhs.x * rhs.y - lhs.y * rhs.x
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly int GetHashCode()
		{
			return this.x.GetHashCode() ^ (this.y.GetHashCode() << 2) ^ (this.z.GetHashCode() >> 2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly bool Equals(object other)
		{
			Vector3 vector;
			bool flag;
			if (other is Vector3)
			{
				vector = (Vector3)other;
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
		public readonly bool Equals(Vector3 other)
		{
			return this.x == other.x && this.y == other.y && this.z == other.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(in Vector3 other)
		{
			return this.x == other.x && this.y == other.y && this.z == other.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 Reflect(Vector3 inDirection, Vector3 inNormal)
		{
			float num = -2f * Vector3.Dot(in inNormal, in inDirection);
			Vector3 vector;
			vector.x = num * inNormal.x + inDirection.x;
			vector.y = num * inNormal.y + inDirection.y;
			vector.z = num * inNormal.z + inDirection.z;
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 Reflect(in Vector3 inDirection, in Vector3 inNormal)
		{
			float num = -2f * Vector3.Dot(in inNormal, in inDirection);
			Vector3 vector;
			vector.x = num * inNormal.x + inDirection.x;
			vector.y = num * inNormal.y + inDirection.y;
			vector.z = num * inNormal.z + inDirection.z;
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 Normalize(Vector3 value)
		{
			float magnitude = value.magnitude;
			return (magnitude > 1E-05f) ? new Vector3
			{
				x = value.x / magnitude,
				y = value.y / magnitude,
				z = value.z / magnitude
			} : Vector3.zeroVector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 Normalize(in Vector3 value)
		{
			float magnitude = value.magnitude;
			return (magnitude > 1E-05f) ? new Vector3
			{
				x = value.x / magnitude,
				y = value.y / magnitude,
				z = value.z / magnitude
			} : Vector3.zeroVector;
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
				this.z /= magnitude;
			}
			else
			{
				this.x = 0f;
				this.y = 0f;
				this.z = 0f;
			}
		}

		public readonly Vector3 normalized
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector3.Normalize(in this);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Dot(Vector3 lhs, Vector3 rhs)
		{
			return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Dot(in Vector3 lhs, in Vector3 rhs)
		{
			return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 Project(Vector3 vector, Vector3 onNormal)
		{
			float num = Vector3.Dot(in onNormal, in onNormal);
			bool flag = num < Mathf.Epsilon;
			Vector3 vector2;
			if (flag)
			{
				vector2 = Vector3.zero;
			}
			else
			{
				float num2 = Vector3.Dot(in vector, in onNormal) / num;
				Vector3 vector3;
				vector3.x = onNormal.x * num2;
				vector3.y = onNormal.y * num2;
				vector3.z = onNormal.z * num2;
				vector2 = vector3;
			}
			return vector2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 Project(in Vector3 vector, in Vector3 onNormal)
		{
			float num = Vector3.Dot(in onNormal, in onNormal);
			bool flag = num < Mathf.Epsilon;
			Vector3 vector2;
			if (flag)
			{
				vector2 = Vector3.zero;
			}
			else
			{
				float num2 = Vector3.Dot(in vector, in onNormal) / num;
				Vector3 vector3;
				vector3.x = onNormal.x * num2;
				vector3.y = onNormal.y * num2;
				vector3.z = onNormal.z * num2;
				vector2 = vector3;
			}
			return vector2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 ProjectOnPlane(Vector3 vector, Vector3 planeNormal)
		{
			float num = Vector3.Dot(in planeNormal, in planeNormal);
			bool flag = num < Mathf.Epsilon;
			Vector3 vector2;
			if (flag)
			{
				vector2 = vector;
			}
			else
			{
				float num2 = Vector3.Dot(in vector, in planeNormal) / num;
				Vector3 vector3;
				vector3.x = vector.x - planeNormal.x * num2;
				vector3.y = vector.y - planeNormal.y * num2;
				vector3.z = vector.z - planeNormal.z * num2;
				vector2 = vector3;
			}
			return vector2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 ProjectOnPlane(in Vector3 vector, in Vector3 planeNormal)
		{
			float num = Vector3.Dot(in planeNormal, in planeNormal);
			bool flag = num < Mathf.Epsilon;
			Vector3 vector2;
			if (flag)
			{
				vector2 = vector;
			}
			else
			{
				float num2 = Vector3.Dot(in vector, in planeNormal) / num;
				Vector3 vector3;
				vector3.x = vector.x - planeNormal.x * num2;
				vector3.y = vector.y - planeNormal.y * num2;
				vector3.z = vector.z - planeNormal.z * num2;
				vector2 = vector3;
			}
			return vector2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Angle(Vector3 from, Vector3 to)
		{
			float num = (float)Math.Sqrt((double)(from.sqrMagnitude * to.sqrMagnitude));
			bool flag = num < 1E-15f;
			float num2;
			if (flag)
			{
				num2 = 0f;
			}
			else
			{
				float num3 = Mathf.Clamp(Vector3.Dot(in from, in to) / num, -1f, 1f);
				num2 = (float)Math.Acos((double)num3) * 57.29578f;
			}
			return num2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Angle(in Vector3 from, in Vector3 to)
		{
			float num = (float)Math.Sqrt((double)(from.sqrMagnitude * to.sqrMagnitude));
			bool flag = num < 1E-15f;
			float num2;
			if (flag)
			{
				num2 = 0f;
			}
			else
			{
				float num3 = Mathf.Clamp(Vector3.Dot(in from, in to) / num, -1f, 1f);
				num2 = (float)Math.Acos((double)num3) * 57.29578f;
			}
			return num2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float SignedAngle(Vector3 from, Vector3 to, Vector3 axis)
		{
			float num = Vector3.Angle(in from, in to);
			float num2 = from.y * to.z - from.z * to.y;
			float num3 = from.z * to.x - from.x * to.z;
			float num4 = from.x * to.y - from.y * to.x;
			float num5 = Mathf.Sign(axis.x * num2 + axis.y * num3 + axis.z * num4);
			return num * num5;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float SignedAngle(in Vector3 from, in Vector3 to, in Vector3 axis)
		{
			float num = Vector3.Angle(in from, in to);
			float num2 = from.y * to.z - from.z * to.y;
			float num3 = from.z * to.x - from.x * to.z;
			float num4 = from.x * to.y - from.y * to.x;
			float num5 = Mathf.Sign(axis.x * num2 + axis.y * num3 + axis.z * num4);
			return num * num5;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Distance(Vector3 a, Vector3 b)
		{
			float num = a.x - b.x;
			float num2 = a.y - b.y;
			float num3 = a.z - b.z;
			return (float)Math.Sqrt((double)(num * num + num2 * num2 + num3 * num3));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Distance(in Vector3 a, in Vector3 b)
		{
			float num = a.x - b.x;
			float num2 = a.y - b.y;
			float num3 = a.z - b.z;
			return (float)Math.Sqrt((double)(num * num + num2 * num2 + num3 * num3));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 ClampMagnitude(Vector3 vector, float maxLength)
		{
			float sqrMagnitude = vector.sqrMagnitude;
			bool flag = sqrMagnitude > maxLength * maxLength;
			Vector3 vector3;
			if (flag)
			{
				float num = (float)Math.Sqrt((double)sqrMagnitude);
				float num2 = vector.x / num;
				float num3 = vector.y / num;
				float num4 = vector.z / num;
				Vector3 vector2;
				vector2.x = num2 * maxLength;
				vector2.y = num3 * maxLength;
				vector2.z = num4 * maxLength;
				vector3 = vector2;
			}
			else
			{
				vector3 = vector;
			}
			return vector3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 ClampMagnitude(in Vector3 vector, float maxLength)
		{
			float sqrMagnitude = vector.sqrMagnitude;
			bool flag = sqrMagnitude > maxLength * maxLength;
			Vector3 vector3;
			if (flag)
			{
				float num = (float)Math.Sqrt((double)sqrMagnitude);
				float num2 = vector.x / num;
				float num3 = vector.y / num;
				float num4 = vector.z / num;
				Vector3 vector2;
				vector2.x = num2 * maxLength;
				vector2.y = num3 * maxLength;
				vector2.z = num4 * maxLength;
				vector3 = vector2;
			}
			else
			{
				vector3 = vector;
			}
			return vector3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Magnitude(Vector3 vector)
		{
			return vector.magnitude;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Magnitude(in Vector3 vector)
		{
			return vector.magnitude;
		}

		public readonly float magnitude
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (float)Math.Sqrt((double)(this.x * this.x + this.y * this.y + this.z * this.z));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float SqrMagnitude(Vector3 vector)
		{
			return vector.sqrMagnitude;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float SqrMagnitude(in Vector3 vector)
		{
			return vector.sqrMagnitude;
		}

		public readonly float sqrMagnitude
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.x * this.x + this.y * this.y + this.z * this.z;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 Min(Vector3 lhs, Vector3 rhs)
		{
			return new Vector3
			{
				x = Mathf.Min(lhs.x, rhs.x),
				y = Mathf.Min(lhs.y, rhs.y),
				z = Mathf.Min(lhs.z, rhs.z)
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 Min(in Vector3 lhs, in Vector3 rhs)
		{
			return new Vector3
			{
				x = Mathf.Min(lhs.x, rhs.x),
				y = Mathf.Min(lhs.y, rhs.y),
				z = Mathf.Min(lhs.z, rhs.z)
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 Max(Vector3 lhs, Vector3 rhs)
		{
			return new Vector3
			{
				x = Mathf.Max(lhs.x, rhs.x),
				y = Mathf.Max(lhs.y, rhs.y),
				z = Mathf.Max(lhs.z, rhs.z)
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 Max(in Vector3 lhs, in Vector3 rhs)
		{
			return new Vector3
			{
				x = Mathf.Max(lhs.x, rhs.x),
				y = Mathf.Max(lhs.y, rhs.y),
				z = Mathf.Max(lhs.z, rhs.z)
			};
		}

		public static Vector3 zero
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector3.zeroVector;
			}
		}

		public static Vector3 one
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector3.oneVector;
			}
		}

		public static Vector3 forward
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector3.forwardVector;
			}
		}

		public static Vector3 back
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector3.backVector;
			}
		}

		public static Vector3 up
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector3.upVector;
			}
		}

		public static Vector3 down
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector3.downVector;
			}
		}

		public static Vector3 left
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector3.leftVector;
			}
		}

		public static Vector3 right
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector3.rightVector;
			}
		}

		public static Vector3 positiveInfinity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector3.positiveInfinityVector;
			}
		}

		public static Vector3 negativeInfinity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Vector3.negativeInfinityVector;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 operator +(Vector3 a, Vector3 b)
		{
			return new Vector3
			{
				x = a.x + b.x,
				y = a.y + b.y,
				z = a.z + b.z
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 operator -(Vector3 a, Vector3 b)
		{
			return new Vector3
			{
				x = a.x - b.x,
				y = a.y - b.y,
				z = a.z - b.z
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 operator -(Vector3 a)
		{
			return new Vector3
			{
				x = -a.x,
				y = -a.y,
				z = -a.z
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 operator *(Vector3 a, float d)
		{
			return new Vector3
			{
				x = a.x * d,
				y = a.y * d,
				z = a.z * d
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 operator *(float d, Vector3 a)
		{
			return new Vector3
			{
				x = a.x * d,
				y = a.y * d,
				z = a.z * d
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 operator /(Vector3 a, float d)
		{
			return new Vector3
			{
				x = a.x / d,
				y = a.y / d,
				z = a.z / d
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Vector3 lhs, Vector3 rhs)
		{
			float num = lhs.x - rhs.x;
			float num2 = lhs.y - rhs.y;
			float num3 = lhs.z - rhs.z;
			float num4 = num * num + num2 * num2 + num3 * num3;
			return num4 < 9.9999994E-11f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Vector3 lhs, Vector3 rhs)
		{
			return !(lhs == rhs);
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
			return string.Format("({0}, {1}, {2})", this.x.ToString(format, formatProvider), this.y.ToString(format, formatProvider), this.z.ToString(format, formatProvider));
		}

		[Obsolete("Use Vector3.forward instead.")]
		public static Vector3 fwd
		{
			get
			{
				return new Vector3(0f, 0f, 1f);
			}
		}

		[Obsolete("Use Vector3.Angle instead. AngleBetween uses radians instead of degrees and was deprecated for this reason")]
		public static float AngleBetween(Vector3 from, Vector3 to)
		{
			return (float)Math.Acos((double)Mathf.Clamp(Vector3.Dot(from.normalized, to.normalized), -1f, 1f));
		}

		[Obsolete("Use Vector3.ProjectOnPlane instead.")]
		public static Vector3 Exclude(Vector3 excludeThis, Vector3 fromThat)
		{
			return Vector3.ProjectOnPlane(fromThat, excludeThis);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Slerp_Injected(in Vector3 a, in Vector3 b, float t, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SlerpUnclamped_Injected(in Vector3 a, in Vector3 b, float t, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_RotateTowards_Injected(in Vector3 current, in Vector3 target, float maxRadiansDelta, float maxMagnitudeDelta, out Vector3 ret);

		public const float kEpsilon = 1E-05f;

		public const float kEpsilonNormalSqrt = 1E-15f;

		public float x;

		public float y;

		public float z;

		private static readonly Vector3 zeroVector = new Vector3(0f, 0f, 0f);

		private static readonly Vector3 oneVector = new Vector3(1f, 1f, 1f);

		private static readonly Vector3 upVector = new Vector3(0f, 1f, 0f);

		private static readonly Vector3 downVector = new Vector3(0f, -1f, 0f);

		private static readonly Vector3 leftVector = new Vector3(-1f, 0f, 0f);

		private static readonly Vector3 rightVector = new Vector3(1f, 0f, 0f);

		private static readonly Vector3 forwardVector = new Vector3(0f, 0f, 1f);

		private static readonly Vector3 backVector = new Vector3(0f, 0f, -1f);

		private static readonly Vector3 positiveInfinityVector = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

		private static readonly Vector3 negativeInfinityVector = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
	}
}
