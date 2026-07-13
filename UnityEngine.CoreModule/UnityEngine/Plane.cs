using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	public struct Plane : IEquatable<Plane>, IFormattable
	{
		public Vector3 normal
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.m_Normal;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Normal = value;
			}
		}

		public float distance
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.m_Distance;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Distance = value;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Plane(Vector3 inNormal, Vector3 inPoint)
		{
			this.m_Normal.x = inNormal.x;
			this.m_Normal.y = inNormal.y;
			this.m_Normal.z = inNormal.z;
			this.m_Normal.Normalize();
			this.m_Distance = -Vector3.Dot(in this.m_Normal, in inPoint);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Plane(in Vector3 inNormal, in Vector3 inPoint)
		{
			this.m_Normal.x = inNormal.x;
			this.m_Normal.y = inNormal.y;
			this.m_Normal.z = inNormal.z;
			this.m_Normal.Normalize();
			this.m_Distance = -Vector3.Dot(in this.m_Normal, in inPoint);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Plane(Vector3 inNormal, float d)
		{
			this.m_Normal.x = inNormal.x;
			this.m_Normal.y = inNormal.y;
			this.m_Normal.z = inNormal.z;
			this.m_Normal.Normalize();
			this.m_Distance = d;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Plane(in Vector3 inNormal, float d)
		{
			this.m_Normal.x = inNormal.x;
			this.m_Normal.y = inNormal.y;
			this.m_Normal.z = inNormal.z;
			this.m_Normal.Normalize();
			this.m_Distance = d;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Plane(Vector3 a, Vector3 b, Vector3 c)
		{
			Vector3 vector;
			vector.x = b.x - a.x;
			vector.y = b.y - a.y;
			vector.z = b.z - a.z;
			Vector3 vector2;
			vector2.x = c.x - a.x;
			vector2.y = c.y - a.y;
			vector2.z = c.z - a.z;
			Vector3 vector3 = Vector3.Cross(in vector, in vector2);
			this.m_Normal.x = vector3.x;
			this.m_Normal.y = vector3.y;
			this.m_Normal.z = vector3.z;
			this.m_Normal.Normalize();
			this.m_Distance = -Vector3.Dot(in this.m_Normal, in a);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Plane(in Vector3 a, in Vector3 b, in Vector3 c)
		{
			Vector3 vector;
			vector.x = b.x - a.x;
			vector.y = b.y - a.y;
			vector.z = b.z - a.z;
			Vector3 vector2;
			vector2.x = c.x - a.x;
			vector2.y = c.y - a.y;
			vector2.z = c.z - a.z;
			Vector3 vector3 = Vector3.Cross(in vector, in vector2);
			this.m_Normal.x = vector3.x;
			this.m_Normal.y = vector3.y;
			this.m_Normal.z = vector3.z;
			this.m_Normal.Normalize();
			this.m_Distance = -Vector3.Dot(in this.m_Normal, in a);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetNormalAndPosition(Vector3 inNormal, Vector3 inPoint)
		{
			this.m_Normal.x = inNormal.x;
			this.m_Normal.y = inNormal.y;
			this.m_Normal.z = inNormal.z;
			this.m_Normal.Normalize();
			this.m_Distance = -Vector3.Dot(in this.m_Normal, in inPoint);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetNormalAndPosition(in Vector3 inNormal, in Vector3 inPoint)
		{
			this.m_Normal.x = inNormal.x;
			this.m_Normal.y = inNormal.y;
			this.m_Normal.z = inNormal.z;
			this.m_Normal.Normalize();
			this.m_Distance = -Vector3.Dot(in this.m_Normal, in inPoint);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Set3Points(Vector3 a, Vector3 b, Vector3 c)
		{
			Vector3 vector;
			vector.x = b.x - a.x;
			vector.y = b.y - a.y;
			vector.z = b.z - a.z;
			Vector3 vector2;
			vector2.x = c.x - a.x;
			vector2.y = c.y - a.y;
			vector2.z = c.z - a.z;
			Vector3 vector3 = Vector3.Cross(in vector, in vector2);
			this.m_Normal.x = vector3.x;
			this.m_Normal.y = vector3.y;
			this.m_Normal.z = vector3.z;
			this.m_Normal.Normalize();
			this.m_Distance = -Vector3.Dot(in this.m_Normal, in a);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Set3Points(in Vector3 a, in Vector3 b, in Vector3 c)
		{
			Vector3 vector;
			vector.x = b.x - a.x;
			vector.y = b.y - a.y;
			vector.z = b.z - a.z;
			Vector3 vector2;
			vector2.x = c.x - a.x;
			vector2.y = c.y - a.y;
			vector2.z = c.z - a.z;
			Vector3 vector3 = Vector3.Cross(in vector, in vector2);
			this.m_Normal.x = vector3.x;
			this.m_Normal.y = vector3.y;
			this.m_Normal.z = vector3.z;
			this.m_Normal.Normalize();
			this.m_Distance = -Vector3.Dot(in this.m_Normal, in a);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Flip()
		{
			this.m_Normal.x = -this.m_Normal.x;
			this.m_Normal.y = -this.m_Normal.y;
			this.m_Normal.z = -this.m_Normal.z;
			this.m_Distance = -this.m_Distance;
		}

		public readonly Plane flipped
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Plane(-this.m_Normal, -this.m_Distance);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Translate(Vector3 translation)
		{
			this.m_Distance += Vector3.Dot(in this.m_Normal, in translation);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Translate(in Vector3 translation)
		{
			this.m_Distance += Vector3.Dot(in this.m_Normal, in translation);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Plane Translate(Plane plane, Vector3 translation)
		{
			return new Plane(in plane.m_Normal, plane.m_Distance + Vector3.Dot(in plane.m_Normal, in translation));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Plane Translate(in Plane plane, in Vector3 translation)
		{
			return new Plane(in plane.m_Normal, plane.m_Distance + Vector3.Dot(in plane.m_Normal, in translation));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector3 ClosestPointOnPlane(Vector3 point)
		{
			float num = Vector3.Dot(in this.m_Normal, in point) + this.m_Distance;
			Vector3 vector;
			vector.x = point.x - this.m_Normal.x * num;
			vector.y = point.y - this.m_Normal.y * num;
			vector.z = point.z - this.m_Normal.z * num;
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector3 ClosestPointOnPlane(in Vector3 point)
		{
			float num = Vector3.Dot(in this.m_Normal, in point) + this.m_Distance;
			Vector3 vector;
			vector.x = point.x - this.m_Normal.x * num;
			vector.y = point.y - this.m_Normal.y * num;
			vector.z = point.z - this.m_Normal.z * num;
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly float GetDistanceToPoint(Vector3 point)
		{
			return Vector3.Dot(in this.m_Normal, in point) + this.m_Distance;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly float GetDistanceToPoint(in Vector3 point)
		{
			return Vector3.Dot(in this.m_Normal, in point) + this.m_Distance;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool GetSide(Vector3 point)
		{
			return Vector3.Dot(in this.m_Normal, in point) + this.m_Distance > 0f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool GetSide(in Vector3 point)
		{
			return Vector3.Dot(in this.m_Normal, in point) + this.m_Distance > 0f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool SameSide(Vector3 inPt0, Vector3 inPt1)
		{
			float distanceToPoint = this.GetDistanceToPoint(in inPt0);
			float distanceToPoint2 = this.GetDistanceToPoint(in inPt1);
			return (distanceToPoint > 0f && distanceToPoint2 > 0f) || (distanceToPoint <= 0f && distanceToPoint2 <= 0f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool SameSide(in Vector3 inPt0, in Vector3 inPt1)
		{
			float distanceToPoint = this.GetDistanceToPoint(in inPt0);
			float distanceToPoint2 = this.GetDistanceToPoint(in inPt1);
			return (distanceToPoint > 0f && distanceToPoint2 > 0f) || (distanceToPoint <= 0f && distanceToPoint2 <= 0f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Raycast(Ray ray, out float enter)
		{
			Vector3 vector = ray.direction;
			float num = Vector3.Dot(in vector, in this.m_Normal);
			vector = ray.origin;
			float num2 = -Vector3.Dot(in vector, in this.m_Normal) - this.m_Distance;
			bool flag = Mathf.Approximately(num, 0f);
			bool flag2;
			if (flag)
			{
				enter = 0f;
				flag2 = false;
			}
			else
			{
				enter = num2 / num;
				flag2 = enter > 0f;
			}
			return flag2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Raycast(in Ray ray, out float enter)
		{
			Vector3 vector = ray.direction;
			float num = Vector3.Dot(in vector, in this.m_Normal);
			vector = ray.origin;
			float num2 = -Vector3.Dot(in vector, in this.m_Normal) - this.m_Distance;
			bool flag = Mathf.Approximately(num, 0f);
			bool flag2;
			if (flag)
			{
				enter = 0f;
				flag2 = false;
			}
			else
			{
				enter = num2 / num;
				flag2 = enter > 0f;
			}
			return flag2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Plane lhs, Plane rhs)
		{
			return lhs.m_Normal == rhs.m_Normal && lhs.m_Distance == rhs.m_Distance;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Plane lhs, Plane rhs)
		{
			return !(lhs == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly bool Equals(object other)
		{
			Plane plane;
			bool flag;
			if (other is Plane)
			{
				plane = (Plane)other;
				flag = true;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			return flag2 && this.Equals(in plane);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(Plane other)
		{
			return this == other;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(in Plane other)
		{
			return this == other;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly int GetHashCode()
		{
			return this.m_Distance.GetHashCode() ^ (this.m_Normal.GetHashCode() << 2);
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
			return string.Format("(normal:{0}, distance:{1})", this.m_Normal.ToString(format, formatProvider), this.m_Distance.ToString(format, formatProvider));
		}

		internal const int size = 16;

		private Vector3 m_Normal;

		private float m_Distance;
	}
}
