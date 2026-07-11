using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Math/Vector3.h")]
	[ThreadAndSerializationSafe]
	[NativeHeader("Runtime/Math/MathScripting.h")]
	[RequiredByNativeCode(Optional = true, GenerateProxy = true)]
	[NativeType(Header = "Runtime/Math/Vector3.h")]
	[NativeClass("Vector3f")]
	public struct Vector3 : IEquatable<Vector3>
	{
		public Vector3(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public Vector3(float x, float y)
		{
			this.x = x;
			this.y = y;
			this.z = 0f;
		}

		[FreeFunction("VectorScripting::Slerp", IsThreadSafe = true)]
		public static Vector3 Slerp(Vector3 a, Vector3 b, float t)
		{
			Vector3 vector;
			Vector3.Slerp_Injected(ref a, ref b, t, out vector);
			return vector;
		}

		[FreeFunction("VectorScripting::SlerpUnclamped", IsThreadSafe = true)]
		public static Vector3 SlerpUnclamped(Vector3 a, Vector3 b, float t)
		{
			Vector3 vector;
			Vector3.SlerpUnclamped_Injected(ref a, ref b, t, out vector);
			return vector;
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

		[FreeFunction(IsThreadSafe = true)]
		public static Vector3 RotateTowards(Vector3 current, Vector3 target, float maxRadiansDelta, float maxMagnitudeDelta)
		{
			Vector3 vector;
			Vector3.RotateTowards_Injected(ref current, ref target, maxRadiansDelta, maxMagnitudeDelta, out vector);
			return vector;
		}

		public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
		{
			t = Mathf.Clamp01(t);
			return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
		}

		public static Vector3 LerpUnclamped(Vector3 a, Vector3 b, float t)
		{
			return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
		}

		public static Vector3 MoveTowards(Vector3 current, Vector3 target, float maxDistanceDelta)
		{
			Vector3 vector = target - current;
			float magnitude = vector.magnitude;
			Vector3 vector2;
			if (magnitude <= maxDistanceDelta || magnitude < 1E-45f)
			{
				vector2 = target;
			}
			else
			{
				vector2 = current + vector / magnitude * maxDistanceDelta;
			}
			return vector2;
		}

		[ExcludeFromDocs]
		public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, float maxSpeed)
		{
			float deltaTime = Time.deltaTime;
			return Vector3.SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
		}

		[ExcludeFromDocs]
		public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime)
		{
			float deltaTime = Time.deltaTime;
			float positiveInfinity = float.PositiveInfinity;
			return Vector3.SmoothDamp(current, target, ref currentVelocity, smoothTime, positiveInfinity, deltaTime);
		}

		public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, [DefaultValue("Mathf.Infinity")] float maxSpeed, [DefaultValue("Time.deltaTime")] float deltaTime)
		{
			smoothTime = Mathf.Max(0.0001f, smoothTime);
			float num = 2f / smoothTime;
			float num2 = num * deltaTime;
			float num3 = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
			Vector3 vector = current - target;
			Vector3 vector2 = target;
			float num4 = maxSpeed * smoothTime;
			vector = Vector3.ClampMagnitude(vector, num4);
			target = current - vector;
			Vector3 vector3 = (currentVelocity + num * vector) * deltaTime;
			currentVelocity = (currentVelocity - num * vector3) * num3;
			Vector3 vector4 = target + (vector + vector3) * num3;
			if (Vector3.Dot(vector2 - current, vector4 - vector2) > 0f)
			{
				vector4 = vector2;
				currentVelocity = (vector4 - vector2) / deltaTime;
			}
			return vector4;
		}

		public float this[int index]
		{
			get
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

		public void Set(float newX, float newY, float newZ)
		{
			this.x = newX;
			this.y = newY;
			this.z = newZ;
		}

		public static Vector3 Scale(Vector3 a, Vector3 b)
		{
			return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
		}

		public void Scale(Vector3 scale)
		{
			this.x *= scale.x;
			this.y *= scale.y;
			this.z *= scale.z;
		}

		public static Vector3 Cross(Vector3 lhs, Vector3 rhs)
		{
			return new Vector3(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
		}

		public override int GetHashCode()
		{
			return this.x.GetHashCode() ^ (this.y.GetHashCode() << 2) ^ (this.z.GetHashCode() >> 2);
		}

		public override bool Equals(object other)
		{
			return other is Vector3 && this.Equals((Vector3)other);
		}

		public bool Equals(Vector3 other)
		{
			return this.x.Equals(other.x) && this.y.Equals(other.y) && this.z.Equals(other.z);
		}

		public static Vector3 Reflect(Vector3 inDirection, Vector3 inNormal)
		{
			return -2f * Vector3.Dot(inNormal, inDirection) * inNormal + inDirection;
		}

		public static Vector3 Normalize(Vector3 value)
		{
			float num = Vector3.Magnitude(value);
			Vector3 vector;
			if (num > 1E-05f)
			{
				vector = value / num;
			}
			else
			{
				vector = Vector3.zero;
			}
			return vector;
		}

		public void Normalize()
		{
			float num = Vector3.Magnitude(this);
			if (num > 1E-05f)
			{
				this /= num;
			}
			else
			{
				this = Vector3.zero;
			}
		}

		public Vector3 normalized
		{
			get
			{
				return Vector3.Normalize(this);
			}
		}

		public static float Dot(Vector3 lhs, Vector3 rhs)
		{
			return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
		}

		public static Vector3 Project(Vector3 vector, Vector3 onNormal)
		{
			float num = Vector3.Dot(onNormal, onNormal);
			Vector3 vector2;
			if (num < Mathf.Epsilon)
			{
				vector2 = Vector3.zero;
			}
			else
			{
				vector2 = onNormal * Vector3.Dot(vector, onNormal) / num;
			}
			return vector2;
		}

		public static Vector3 ProjectOnPlane(Vector3 vector, Vector3 planeNormal)
		{
			return vector - Vector3.Project(vector, planeNormal);
		}

		public static float Angle(Vector3 from, Vector3 to)
		{
			float num = Mathf.Sqrt(from.sqrMagnitude * to.sqrMagnitude);
			float num2;
			if (num < 1E-15f)
			{
				num2 = 0f;
			}
			else
			{
				float num3 = Mathf.Clamp(Vector3.Dot(from, to) / num, -1f, 1f);
				num2 = Mathf.Acos(num3) * 57.29578f;
			}
			return num2;
		}

		public static float SignedAngle(Vector3 from, Vector3 to, Vector3 axis)
		{
			float num = Vector3.Angle(from, to);
			float num2 = Mathf.Sign(Vector3.Dot(axis, Vector3.Cross(from, to)));
			return num * num2;
		}

		public static float Distance(Vector3 a, Vector3 b)
		{
			Vector3 vector = new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
			return Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
		}

		public static Vector3 ClampMagnitude(Vector3 vector, float maxLength)
		{
			Vector3 vector2;
			if (vector.sqrMagnitude > maxLength * maxLength)
			{
				vector2 = vector.normalized * maxLength;
			}
			else
			{
				vector2 = vector;
			}
			return vector2;
		}

		public static float Magnitude(Vector3 vector)
		{
			return Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
		}

		public float magnitude
		{
			get
			{
				return Mathf.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z);
			}
		}

		public static float SqrMagnitude(Vector3 vector)
		{
			return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
		}

		public float sqrMagnitude
		{
			get
			{
				return this.x * this.x + this.y * this.y + this.z * this.z;
			}
		}

		public static Vector3 Min(Vector3 lhs, Vector3 rhs)
		{
			return new Vector3(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y), Mathf.Min(lhs.z, rhs.z));
		}

		public static Vector3 Max(Vector3 lhs, Vector3 rhs)
		{
			return new Vector3(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y), Mathf.Max(lhs.z, rhs.z));
		}

		public static Vector3 zero
		{
			get
			{
				return Vector3.zeroVector;
			}
		}

		public static Vector3 one
		{
			get
			{
				return Vector3.oneVector;
			}
		}

		public static Vector3 forward
		{
			get
			{
				return Vector3.forwardVector;
			}
		}

		public static Vector3 back
		{
			get
			{
				return Vector3.backVector;
			}
		}

		public static Vector3 up
		{
			get
			{
				return Vector3.upVector;
			}
		}

		public static Vector3 down
		{
			get
			{
				return Vector3.downVector;
			}
		}

		public static Vector3 left
		{
			get
			{
				return Vector3.leftVector;
			}
		}

		public static Vector3 right
		{
			get
			{
				return Vector3.rightVector;
			}
		}

		public static Vector3 positiveInfinity
		{
			get
			{
				return Vector3.positiveInfinityVector;
			}
		}

		public static Vector3 negativeInfinity
		{
			get
			{
				return Vector3.negativeInfinityVector;
			}
		}

		public static Vector3 operator +(Vector3 a, Vector3 b)
		{
			return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
		}

		public static Vector3 operator -(Vector3 a, Vector3 b)
		{
			return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
		}

		public static Vector3 operator -(Vector3 a)
		{
			return new Vector3(-a.x, -a.y, -a.z);
		}

		public static Vector3 operator *(Vector3 a, float d)
		{
			return new Vector3(a.x * d, a.y * d, a.z * d);
		}

		public static Vector3 operator *(float d, Vector3 a)
		{
			return new Vector3(a.x * d, a.y * d, a.z * d);
		}

		public static Vector3 operator /(Vector3 a, float d)
		{
			return new Vector3(a.x / d, a.y / d, a.z / d);
		}

		public static bool operator ==(Vector3 lhs, Vector3 rhs)
		{
			return Vector3.SqrMagnitude(lhs - rhs) < 9.9999994E-11f;
		}

		public static bool operator !=(Vector3 lhs, Vector3 rhs)
		{
			return !(lhs == rhs);
		}

		public override string ToString()
		{
			return UnityString.Format("({0:F1}, {1:F1}, {2:F1})", new object[] { this.x, this.y, this.z });
		}

		public string ToString(string format)
		{
			return UnityString.Format("({0}, {1}, {2})", new object[]
			{
				this.x.ToString(format),
				this.y.ToString(format),
				this.z.ToString(format)
			});
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
			return Mathf.Acos(Mathf.Clamp(Vector3.Dot(from.normalized, to.normalized), -1f, 1f));
		}

		[Obsolete("Use Vector3.ProjectOnPlane instead.")]
		public static Vector3 Exclude(Vector3 excludeThis, Vector3 fromThat)
		{
			return Vector3.ProjectOnPlane(fromThat, excludeThis);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Slerp_Injected(ref Vector3 a, ref Vector3 b, float t, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SlerpUnclamped_Injected(ref Vector3 a, ref Vector3 b, float t, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RotateTowards_Injected(ref Vector3 current, ref Vector3 target, float maxRadiansDelta, float maxMagnitudeDelta, out Vector3 ret);

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
