using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeType(Header = "Runtime/Math/Quaternion.h")]
	[Il2CppEagerStaticClassConstruction]
	[UsedByNativeCode]
	[NativeHeader("Runtime/Math/MathScripting.h")]
	public struct Quaternion : IEquatable<Quaternion>, IFormattable
	{
		[FreeFunction("FromToQuaternionSafe", IsThreadSafe = true)]
		private static Quaternion Internal_FromToRotation(in Vector3 fromDirection, in Vector3 toDirection)
		{
			Quaternion quaternion;
			Quaternion.Internal_FromToRotation_Injected(in fromDirection, in toDirection, out quaternion);
			return quaternion;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion FromToRotation(Vector3 fromDirection, Vector3 toDirection)
		{
			return Quaternion.Internal_FromToRotation(in fromDirection, in toDirection);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion FromToRotation(in Vector3 fromDirection, in Vector3 toDirection)
		{
			return Quaternion.Internal_FromToRotation(in fromDirection, in toDirection);
		}

		[FreeFunction("QuaternionScripting::Inverse", IsThreadSafe = true)]
		private static Quaternion Internal_Inverse(in Quaternion rotation)
		{
			Quaternion quaternion;
			Quaternion.Internal_Inverse_Injected(in rotation, out quaternion);
			return quaternion;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion Inverse(Quaternion rotation)
		{
			return Quaternion.Internal_Inverse(in rotation);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion Inverse(in Quaternion rotation)
		{
			return Quaternion.Internal_Inverse(in rotation);
		}

		[FreeFunction("QuaternionScripting::Slerp", IsThreadSafe = true)]
		private static Quaternion Internal_Slerp(in Quaternion a, in Quaternion b, float t)
		{
			Quaternion quaternion;
			Quaternion.Internal_Slerp_Injected(in a, in b, t, out quaternion);
			return quaternion;
		}

		[FreeFunction("QuaternionScripting::SlerpUnclamped", IsThreadSafe = true)]
		private static Quaternion Internal_SlerpUnclamped(in Quaternion a, in Quaternion b, float t)
		{
			Quaternion quaternion;
			Quaternion.Internal_SlerpUnclamped_Injected(in a, in b, t, out quaternion);
			return quaternion;
		}

		[FreeFunction("QuaternionScripting::Lerp", IsThreadSafe = true)]
		private static Quaternion Internal_Lerp(in Quaternion a, in Quaternion b, float t)
		{
			Quaternion quaternion;
			Quaternion.Internal_Lerp_Injected(in a, in b, t, out quaternion);
			return quaternion;
		}

		[FreeFunction("QuaternionScripting::LerpUnclamped", IsThreadSafe = true)]
		private static Quaternion Internal_LerpUnclamped(in Quaternion a, in Quaternion b, float t)
		{
			Quaternion quaternion;
			Quaternion.Internal_LerpUnclamped_Injected(in a, in b, t, out quaternion);
			return quaternion;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion Slerp(Quaternion a, Quaternion b, float t)
		{
			return Quaternion.Internal_Slerp(in a, in b, t);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion SlerpUnclamped(Quaternion a, Quaternion b, float t)
		{
			return Quaternion.Internal_SlerpUnclamped(in a, in b, t);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion Lerp(Quaternion a, Quaternion b, float t)
		{
			return Quaternion.Internal_Lerp(in a, in b, t);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion LerpUnclamped(Quaternion a, Quaternion b, float t)
		{
			return Quaternion.Internal_LerpUnclamped(in a, in b, t);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion Slerp(in Quaternion a, in Quaternion b, float t)
		{
			return Quaternion.Internal_Slerp(in a, in b, t);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion SlerpUnclamped(in Quaternion a, in Quaternion b, float t)
		{
			return Quaternion.Internal_SlerpUnclamped(in a, in b, t);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion Lerp(in Quaternion a, in Quaternion b, float t)
		{
			return Quaternion.Internal_Lerp(in a, in b, t);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion LerpUnclamped(in Quaternion a, in Quaternion b, float t)
		{
			return Quaternion.Internal_LerpUnclamped(in a, in b, t);
		}

		[FreeFunction("EulerToQuaternion", IsThreadSafe = true)]
		private static Quaternion Internal_FromEulerRad(in Vector3 euler)
		{
			Quaternion quaternion;
			Quaternion.Internal_FromEulerRad_Injected(in euler, out quaternion);
			return quaternion;
		}

		[FreeFunction("QuaternionScripting::ToEuler", IsThreadSafe = true)]
		private static Vector3 Internal_ToEulerRad(in Quaternion rotation)
		{
			Vector3 vector;
			Quaternion.Internal_ToEulerRad_Injected(in rotation, out vector);
			return vector;
		}

		[FreeFunction("QuaternionScripting::ToAxisAngle", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_ToAxisAngleRad(in Quaternion q, out Vector3 axis, out float angle);

		[FreeFunction("QuaternionScripting::AngleAxis", IsThreadSafe = true)]
		private static Quaternion Internal_AngleAxis(float angle, in Vector3 axis)
		{
			Quaternion quaternion;
			Quaternion.Internal_AngleAxis_Injected(angle, in axis, out quaternion);
			return quaternion;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion AngleAxis(float angle, Vector3 axis)
		{
			return Quaternion.Internal_AngleAxis(angle, in axis);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion AngleAxis(float angle, in Vector3 axis)
		{
			return Quaternion.Internal_AngleAxis(angle, in axis);
		}

		[FreeFunction("QuaternionScripting::LookRotation", IsThreadSafe = true)]
		private static Quaternion Internal_LookRotation(in Vector3 forward, [DefaultValue("Vector3.up")] in Vector3 upwards)
		{
			Quaternion quaternion;
			Quaternion.Internal_LookRotation_Injected(in forward, in upwards, out quaternion);
			return quaternion;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion LookRotation(Vector3 forward, [DefaultValue("Vector3.up")] Vector3 upwards)
		{
			return Quaternion.Internal_LookRotation(in forward, in upwards);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion LookRotation(in Vector3 forward, [DefaultValue("Vector3.up")] in Vector3 upwards)
		{
			return Quaternion.Internal_LookRotation(in forward, in upwards);
		}

		[ExcludeFromDocs]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion LookRotation(Vector3 forward)
		{
			Vector3 up = Vector3.up;
			return Quaternion.Internal_LookRotation(in forward, in up);
		}

		[ExcludeFromDocs]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion LookRotation(in Vector3 forward)
		{
			Vector3 up = Vector3.up;
			return Quaternion.Internal_LookRotation(in forward, in up);
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
				case 3:
					num = this.w;
					break;
				default:
					throw new IndexOutOfRangeException("Invalid Quaternion index!");
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
					throw new IndexOutOfRangeException("Invalid Quaternion index!");
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Quaternion(float x, float y, float z, float w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Set(float newX, float newY, float newZ, float newW)
		{
			this.x = newX;
			this.y = newY;
			this.z = newZ;
			this.w = newW;
		}

		public static Quaternion identity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Quaternion.identityQuaternion;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion operator *(Quaternion lhs, Quaternion rhs)
		{
			return new Quaternion
			{
				x = lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y,
				y = lhs.w * rhs.y + lhs.y * rhs.w + lhs.z * rhs.x - lhs.x * rhs.z,
				z = lhs.w * rhs.z + lhs.z * rhs.w + lhs.x * rhs.y - lhs.y * rhs.x,
				w = lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z
			};
		}

		public static Vector3 operator *(Quaternion rotation, Vector3 point)
		{
			float num = rotation.x * 2f;
			float num2 = rotation.y * 2f;
			float num3 = rotation.z * 2f;
			float num4 = rotation.x * num;
			float num5 = rotation.y * num2;
			float num6 = rotation.z * num3;
			float num7 = rotation.x * num2;
			float num8 = rotation.x * num3;
			float num9 = rotation.y * num3;
			float num10 = rotation.w * num;
			float num11 = rotation.w * num2;
			float num12 = rotation.w * num3;
			Vector3 vector;
			vector.x = (1f - (num5 + num6)) * point.x + (num7 - num12) * point.y + (num8 + num11) * point.z;
			vector.y = (num7 + num12) * point.x + (1f - (num4 + num6)) * point.y + (num9 - num10) * point.z;
			vector.z = (num8 - num11) * point.x + (num9 + num10) * point.y + (1f - (num4 + num5)) * point.z;
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Quaternion lhs, Quaternion rhs)
		{
			return Quaternion.IsEqualUsingDot(Quaternion.Dot(in lhs, in rhs));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Quaternion lhs, Quaternion rhs)
		{
			return !(lhs == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsEqualUsingDot(float dot)
		{
			return dot > 0.999999f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Dot(Quaternion a, Quaternion b)
		{
			return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Dot(in Quaternion a, in Quaternion b)
		{
			return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
		}

		[ExcludeFromDocs]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetLookRotation(Vector3 view)
		{
			Vector3 up = Vector3.up;
			this.SetLookRotation(in view, in up);
		}

		[ExcludeFromDocs]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetLookRotation(in Vector3 view)
		{
			Vector3 up = Vector3.up;
			this.SetLookRotation(in view, in up);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetLookRotation(Vector3 view, [DefaultValue("Vector3.up")] Vector3 up)
		{
			this = Quaternion.LookRotation(in view, in up);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetLookRotation(in Vector3 view, [DefaultValue("Vector3.up")] in Vector3 up)
		{
			this = Quaternion.LookRotation(in view, in up);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Angle(Quaternion a, Quaternion b)
		{
			float num = Mathf.Min(Mathf.Abs(Quaternion.Dot(in a, in b)), 1f);
			return Quaternion.IsEqualUsingDot(num) ? 0f : (Mathf.Acos(num) * 2f * 57.29578f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Angle(in Quaternion a, in Quaternion b)
		{
			float num = Mathf.Min(Mathf.Abs(Quaternion.Dot(in a, in b)), 1f);
			return Quaternion.IsEqualUsingDot(num) ? 0f : (Mathf.Acos(num) * 2f * 57.29578f);
		}

		private static Vector3 Internal_MakePositive(Vector3 euler)
		{
			float num = -0.005729578f;
			float num2 = 360f + num;
			bool flag = euler.x < num;
			if (flag)
			{
				euler.x += 360f;
			}
			else
			{
				bool flag2 = euler.x > num2;
				if (flag2)
				{
					euler.x -= 360f;
				}
			}
			bool flag3 = euler.y < num;
			if (flag3)
			{
				euler.y += 360f;
			}
			else
			{
				bool flag4 = euler.y > num2;
				if (flag4)
				{
					euler.y -= 360f;
				}
			}
			bool flag5 = euler.z < num;
			if (flag5)
			{
				euler.z += 360f;
			}
			else
			{
				bool flag6 = euler.z > num2;
				if (flag6)
				{
					euler.z -= 360f;
				}
			}
			return euler;
		}

		private static Vector3 Internal_MakePositive(in Vector3 eulerAngles)
		{
			float num = -0.005729578f;
			float num2 = 360f + num;
			Vector3 vector = eulerAngles;
			bool flag = vector.x < num;
			if (flag)
			{
				vector.x += 360f;
			}
			else
			{
				bool flag2 = vector.x > num2;
				if (flag2)
				{
					vector.x -= 360f;
				}
			}
			bool flag3 = vector.y < num;
			if (flag3)
			{
				vector.y += 360f;
			}
			else
			{
				bool flag4 = vector.y > num2;
				if (flag4)
				{
					vector.y -= 360f;
				}
			}
			bool flag5 = vector.z < num;
			if (flag5)
			{
				vector.z += 360f;
			}
			else
			{
				bool flag6 = vector.z > num2;
				if (flag6)
				{
					vector.z -= 360f;
				}
			}
			return vector;
		}

		public Vector3 eulerAngles
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return Quaternion.Internal_MakePositive(Quaternion.Internal_ToEulerRad(in this) * 57.29578f);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				Vector3 vector = value * 0.017453292f;
				this = Quaternion.Internal_FromEulerRad(in vector);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion Euler(float x, float y, float z)
		{
			Vector3 vector = default(Vector3);
			vector.x = x * 0.017453292f;
			vector.y = y * 0.017453292f;
			vector.z = z * 0.017453292f;
			return Quaternion.Internal_FromEulerRad(in vector);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion Euler(Vector3 euler)
		{
			Vector3 vector = euler * 0.017453292f;
			return Quaternion.Internal_FromEulerRad(in vector);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion Euler(in Vector3 euler)
		{
			Vector3 vector = euler * 0.017453292f;
			return Quaternion.Internal_FromEulerRad(in vector);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ToAngleAxis(out float angle, out Vector3 axis)
		{
			Quaternion.Internal_ToAxisAngleRad(in this, out axis, out angle);
			angle *= 57.29578f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetFromToRotation(Vector3 fromDirection, Vector3 toDirection)
		{
			this = Quaternion.FromToRotation(in fromDirection, in toDirection);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetFromToRotation(in Vector3 fromDirection, in Vector3 toDirection)
		{
			this = Quaternion.FromToRotation(in fromDirection, in toDirection);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion RotateTowards(Quaternion from, Quaternion to, float maxDegreesDelta)
		{
			float num = Quaternion.Angle(in from, in to);
			bool flag = num == 0f;
			Quaternion quaternion;
			if (flag)
			{
				quaternion = to;
			}
			else
			{
				quaternion = Quaternion.SlerpUnclamped(in from, in to, Mathf.Min(1f, maxDegreesDelta / num));
			}
			return quaternion;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion RotateTowards(in Quaternion from, in Quaternion to, float maxDegreesDelta)
		{
			float num = Quaternion.Angle(in from, in to);
			bool flag = num == 0f;
			Quaternion quaternion;
			if (flag)
			{
				quaternion = to;
			}
			else
			{
				quaternion = Quaternion.SlerpUnclamped(in from, in to, Mathf.Min(1f, maxDegreesDelta / num));
			}
			return quaternion;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion Normalize(Quaternion q)
		{
			float num = Mathf.Sqrt(Quaternion.Dot(in q, in q));
			bool flag = num < Mathf.Epsilon;
			Quaternion quaternion;
			if (flag)
			{
				quaternion = Quaternion.identity;
			}
			else
			{
				quaternion = new Quaternion
				{
					x = q.x / num,
					y = q.y / num,
					z = q.z / num,
					w = q.w / num
				};
			}
			return quaternion;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion Normalize(in Quaternion q)
		{
			float num = Mathf.Sqrt(Quaternion.Dot(in q, in q));
			bool flag = num < Mathf.Epsilon;
			Quaternion quaternion;
			if (flag)
			{
				quaternion = Quaternion.identity;
			}
			else
			{
				quaternion = new Quaternion
				{
					x = q.x / num,
					y = q.y / num,
					z = q.z / num,
					w = q.w / num
				};
			}
			return quaternion;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Normalize()
		{
			this = Quaternion.Normalize(in this);
		}

		public readonly Quaternion normalized
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Quaternion.Normalize(in this);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly int GetHashCode()
		{
			return this.x.GetHashCode() ^ (this.y.GetHashCode() << 2) ^ (this.z.GetHashCode() >> 2) ^ (this.w.GetHashCode() >> 1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly bool Equals(object other)
		{
			Quaternion quaternion;
			bool flag;
			if (other is Quaternion)
			{
				quaternion = (Quaternion)other;
				flag = true;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			return flag2 && this.Equals(in quaternion);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(Quaternion other)
		{
			return this.x.Equals(other.x) && this.y.Equals(other.y) && this.z.Equals(other.z) && this.w.Equals(other.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(in Quaternion other)
		{
			return this.x.Equals(other.x) && this.y.Equals(other.y) && this.z.Equals(other.z) && this.w.Equals(other.w);
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
				format = "F5";
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

		[Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion EulerRotation(float x, float y, float z)
		{
			Vector3 vector = new Vector3(x, y, z);
			return Quaternion.Internal_FromEulerRad(in vector);
		}

		[Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion EulerRotation(Vector3 euler)
		{
			return Quaternion.Internal_FromEulerRad(in euler);
		}

		[Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetEulerRotation(float x, float y, float z)
		{
			Vector3 vector = new Vector3(x, y, z);
			this = Quaternion.Internal_FromEulerRad(in vector);
		}

		[Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetEulerRotation(Vector3 euler)
		{
			this = Quaternion.Internal_FromEulerRad(in euler);
		}

		[Obsolete("Use Quaternion.eulerAngles instead. This function was deprecated because it uses radians instead of degrees.")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector3 ToEuler()
		{
			return Quaternion.Internal_ToEulerRad(in this);
		}

		[Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion EulerAngles(float x, float y, float z)
		{
			Vector3 vector = new Vector3(x, y, z);
			return Quaternion.Internal_FromEulerRad(in vector);
		}

		[Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion EulerAngles(Vector3 euler)
		{
			return Quaternion.Internal_FromEulerRad(in euler);
		}

		[Obsolete("Use Quaternion.ToAngleAxis instead. This function was deprecated because it uses radians instead of degrees.")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly void ToAxisAngle(out Vector3 axis, out float angle)
		{
			Quaternion.Internal_ToAxisAngleRad(in this, out axis, out angle);
		}

		[Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetEulerAngles(float x, float y, float z)
		{
			this.SetEulerRotation(new Vector3(x, y, z));
		}

		[Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetEulerAngles(Vector3 euler)
		{
			this = Quaternion.Internal_FromEulerRad(in euler);
		}

		[Obsolete("Use Quaternion.eulerAngles instead. This function was deprecated because it uses radians instead of degrees.")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 ToEulerAngles(Quaternion rotation)
		{
			return Quaternion.Internal_ToEulerRad(in rotation);
		}

		[Obsolete("Use Quaternion.eulerAngles instead. This function was deprecated because it uses radians instead of degrees.")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector3 ToEulerAngles()
		{
			return Quaternion.Internal_ToEulerRad(in this);
		}

		[Obsolete("Use Quaternion.AngleAxis instead. This function was deprecated because it uses radians instead of degrees.")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetAxisAngle(Vector3 axis, float angle)
		{
			this = Quaternion.Internal_AngleAxis(57.29578f * angle, in axis);
		}

		[Obsolete("Use Quaternion.AngleAxis instead. This function was deprecated because it uses radians instead of degrees")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion AxisAngle(Vector3 axis, float angle)
		{
			return Quaternion.Internal_AngleAxis(57.29578f * angle, in axis);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_FromToRotation_Injected(in Vector3 fromDirection, in Vector3 toDirection, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Inverse_Injected(in Quaternion rotation, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Slerp_Injected(in Quaternion a, in Quaternion b, float t, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SlerpUnclamped_Injected(in Quaternion a, in Quaternion b, float t, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Lerp_Injected(in Quaternion a, in Quaternion b, float t, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_LerpUnclamped_Injected(in Quaternion a, in Quaternion b, float t, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_FromEulerRad_Injected(in Vector3 euler, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_ToEulerRad_Injected(in Quaternion rotation, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_AngleAxis_Injected(float angle, in Vector3 axis, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_LookRotation_Injected(in Vector3 forward, [DefaultValue("Vector3.up")] in Vector3 upwards, out Quaternion ret);

		public float x;

		public float y;

		public float z;

		public float w;

		private static readonly Quaternion identityQuaternion = new Quaternion(0f, 0f, 0f, 1f);

		public const float kEpsilon = 1E-06f;
	}
}
