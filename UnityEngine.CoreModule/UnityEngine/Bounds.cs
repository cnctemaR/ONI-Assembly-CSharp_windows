using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode(Optional = true, GenerateProxy = true)]
	[NativeClass("AABB")]
	[NativeHeader("Runtime/Geometry/Ray.h")]
	[NativeHeader("Runtime/Geometry/Intersection.h")]
	[NativeHeader("Runtime/Math/MathScripting.h")]
	[NativeType(Header = "Runtime/Geometry/AABB.h")]
	[NativeHeader("Runtime/Geometry/AABB.h")]
	public struct Bounds : IEquatable<Bounds>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Bounds(Vector3 center, Vector3 size)
		{
			this.m_Center.x = center.x;
			this.m_Center.y = center.y;
			this.m_Center.z = center.z;
			this.m_Extents.x = size.x * 0.5f;
			this.m_Extents.y = size.y * 0.5f;
			this.m_Extents.z = size.z * 0.5f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Bounds(in Vector3 center, in Vector3 size)
		{
			this.m_Center.x = center.x;
			this.m_Center.y = center.y;
			this.m_Center.z = center.z;
			this.m_Extents.x = size.x * 0.5f;
			this.m_Extents.y = size.y * 0.5f;
			this.m_Extents.z = size.z * 0.5f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly int GetHashCode()
		{
			return this.m_Center.GetHashCode() ^ (this.m_Extents.GetHashCode() << 2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly bool Equals(object other)
		{
			Bounds bounds;
			bool flag;
			if (other is Bounds)
			{
				bounds = (Bounds)other;
				flag = true;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			return flag2 && this.Equals(in bounds);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(Bounds other)
		{
			return this.m_Center.Equals(in other.m_Center) && this.m_Extents.Equals(in other.m_Extents);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(in Bounds other)
		{
			return this.m_Center.Equals(in other.m_Center) && this.m_Extents.Equals(in other.m_Extents);
		}

		public Vector3 center
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.m_Center;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Center = value;
			}
		}

		public Vector3 size
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return new Vector3
				{
					x = this.m_Extents.x * 2f,
					y = this.m_Extents.y * 2f,
					z = this.m_Extents.z * 2f
				};
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Extents.x = value.x * 0.5f;
				this.m_Extents.y = value.y * 0.5f;
				this.m_Extents.z = value.z * 0.5f;
			}
		}

		public Vector3 extents
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.m_Extents;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Extents = value;
			}
		}

		public Vector3 min
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return new Vector3
				{
					x = this.m_Center.x - this.m_Extents.x,
					y = this.m_Center.y - this.m_Extents.y,
					z = this.m_Center.z - this.m_Extents.z
				};
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				Vector3 max = this.max;
				this.SetMinMax(in value, in max);
			}
		}

		public Vector3 max
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return new Vector3
				{
					x = this.m_Center.x + this.m_Extents.x,
					y = this.m_Center.y + this.m_Extents.y,
					z = this.m_Center.z + this.m_Extents.z
				};
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				Vector3 min = this.min;
				this.SetMinMax(in min, in value);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Bounds lhs, Bounds rhs)
		{
			return lhs.m_Center == rhs.m_Center && lhs.m_Extents == rhs.m_Extents;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Bounds lhs, Bounds rhs)
		{
			return !(lhs == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetMinMax(Vector3 min, Vector3 max)
		{
			this.m_Extents.x = (max.x - min.x) * 0.5f;
			this.m_Extents.y = (max.y - min.y) * 0.5f;
			this.m_Extents.z = (max.z - min.z) * 0.5f;
			this.m_Center.x = min.x + this.m_Extents.x;
			this.m_Center.y = min.y + this.m_Extents.y;
			this.m_Center.z = min.z + this.m_Extents.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetMinMax(in Vector3 min, in Vector3 max)
		{
			this.m_Extents.x = (max.x - min.x) * 0.5f;
			this.m_Extents.y = (max.y - min.y) * 0.5f;
			this.m_Extents.z = (max.z - min.z) * 0.5f;
			this.m_Center.x = min.x + this.m_Extents.x;
			this.m_Center.y = min.y + this.m_Extents.y;
			this.m_Center.z = min.z + this.m_Extents.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Encapsulate(Vector3 point)
		{
			Vector3 vector = this.min;
			Vector3 vector2 = Vector3.Min(in vector, in point);
			vector = this.max;
			Vector3 vector3 = Vector3.Max(in vector, in point);
			this.SetMinMax(in vector2, in vector3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Encapsulate(in Vector3 point)
		{
			Vector3 vector = this.min;
			Vector3 vector2 = Vector3.Min(in vector, in point);
			vector = this.max;
			Vector3 vector3 = Vector3.Max(in vector, in point);
			this.SetMinMax(in vector2, in vector3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Encapsulate(Bounds bounds)
		{
			Vector3 min = bounds.min;
			Vector3 max = bounds.max;
			this.Encapsulate(in min);
			this.Encapsulate(in max);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Encapsulate(in Bounds bounds)
		{
			Vector3 min = bounds.min;
			Vector3 max = bounds.max;
			this.Encapsulate(in min);
			this.Encapsulate(in max);
		}

		public void Expand(float amount)
		{
			amount *= 0.5f;
			this.m_Extents.x = this.m_Extents.x + amount;
			this.m_Extents.y = this.m_Extents.y + amount;
			this.m_Extents.z = this.m_Extents.z + amount;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Expand(Vector3 amount)
		{
			this.m_Extents.x = this.m_Extents.x + amount.x * 0.5f;
			this.m_Extents.y = this.m_Extents.y + amount.y * 0.5f;
			this.m_Extents.z = this.m_Extents.z + amount.z * 0.5f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Expand(in Vector3 amount)
		{
			this.m_Extents.x = this.m_Extents.x + amount.x * 0.5f;
			this.m_Extents.y = this.m_Extents.y + amount.y * 0.5f;
			this.m_Extents.z = this.m_Extents.z + amount.z * 0.5f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Intersects(Bounds bounds)
		{
			Vector3 min = this.min;
			Vector3 max = this.max;
			Vector3 min2 = bounds.min;
			Vector3 max2 = bounds.max;
			return min.x <= max2.x && max.x >= min2.x && min.y <= max2.y && max.y >= min2.y && min.z <= max2.z && max.z >= min2.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Intersects(in Bounds bounds)
		{
			Vector3 min = this.min;
			Vector3 max = this.max;
			Vector3 min2 = bounds.min;
			Vector3 max2 = bounds.max;
			return min.x <= max2.x && max.x >= min2.x && min.y <= max2.y && max.y >= min2.y && min.z <= max2.z && max.z >= min2.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool IntersectRay(Ray ray)
		{
			float num;
			return Bounds.IntersectRayAABB(in ray, in this, out num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool IntersectRay(in Ray ray)
		{
			float num;
			return Bounds.IntersectRayAABB(in ray, in this, out num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool IntersectRay(Ray ray, out float distance)
		{
			return Bounds.IntersectRayAABB(in ray, in this, out distance);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool IntersectRay(in Ray ray, out float distance)
		{
			return Bounds.IntersectRayAABB(in ray, in this, out distance);
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
			return string.Format("Center: {0}, Extents: {1}", this.m_Center.ToString(format, formatProvider), this.m_Extents.ToString(format, formatProvider));
		}

		[NativeMethod("IsInside", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private readonly extern bool Internal_Contains(in Vector3 point);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Contains(Vector3 point)
		{
			return this.Internal_Contains(in point);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Contains(in Vector3 point)
		{
			return this.Internal_Contains(in point);
		}

		[FreeFunction("BoundsScripting::SqrDistance", HasExplicitThis = true, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private readonly extern float Internal_SqrDistance(in Vector3 point);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly float SqrDistance(Vector3 point)
		{
			return this.Internal_SqrDistance(in point);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly float SqrDistance(in Vector3 point)
		{
			return this.Internal_SqrDistance(in point);
		}

		[FreeFunction("IntersectRayAABB", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IntersectRayAABB(in Ray ray, in Bounds bounds, out float dist);

		[FreeFunction("BoundsScripting::ClosestPoint", HasExplicitThis = true, IsThreadSafe = true)]
		private readonly Vector3 Internal_ClosestPoint(in Vector3 point)
		{
			Vector3 vector;
			Bounds.Internal_ClosestPoint_Injected(ref this, in point, out vector);
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector3 ClosestPoint(Vector3 point)
		{
			return this.Internal_ClosestPoint(in point);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector3 ClosestPoint(in Vector3 point)
		{
			return this.Internal_ClosestPoint(in point);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_ClosestPoint_Injected(ref Bounds _unity_self, in Vector3 point, out Vector3 ret);

		private Vector3 m_Center;

		[NativeName("m_Extent")]
		private Vector3 m_Extents;
	}
}
