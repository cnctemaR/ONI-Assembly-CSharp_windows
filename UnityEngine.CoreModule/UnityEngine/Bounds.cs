using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeType(Header = "Runtime/Geometry/AABB.h")]
	[NativeHeader("Runtime/Math/MathScripting.h")]
	[NativeHeader("Runtime/Geometry/Intersection.h")]
	[NativeClass("AABB")]
	[NativeHeader("Runtime/Geometry/Ray.h")]
	[RequiredByNativeCode(Optional = true, GenerateProxy = true)]
	[NativeHeader("Runtime/Geometry/AABB.h")]
	public struct Bounds : IEquatable<Bounds>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Bounds(Vector3 center, Vector3 size)
		{
			this.m_Center = center;
			this.m_Extents = size * 0.5f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
		{
			return this.center.GetHashCode() ^ (this.extents.GetHashCode() << 2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object other)
		{
			bool flag = !(other is Bounds);
			return !flag && this.Equals((Bounds)other);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(Bounds other)
		{
			return this.center.Equals(other.center) && this.extents.Equals(other.extents);
		}

		public Vector3 center
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
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
			get
			{
				return this.m_Extents * 2f;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Extents = value * 0.5f;
			}
		}

		public Vector3 extents
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
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
			get
			{
				return this.center - this.extents;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.SetMinMax(value, this.max);
			}
		}

		public Vector3 max
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.center + this.extents;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.SetMinMax(this.min, value);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Bounds lhs, Bounds rhs)
		{
			return lhs.center == rhs.center && lhs.extents == rhs.extents;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Bounds lhs, Bounds rhs)
		{
			return !(lhs == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetMinMax(Vector3 min, Vector3 max)
		{
			this.extents = (max - min) * 0.5f;
			this.center = min + this.extents;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Encapsulate(Vector3 point)
		{
			this.SetMinMax(Vector3.Min(this.min, point), Vector3.Max(this.max, point));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Encapsulate(Bounds bounds)
		{
			this.Encapsulate(bounds.center - bounds.extents);
			this.Encapsulate(bounds.center + bounds.extents);
		}

		public void Expand(float amount)
		{
			amount *= 0.5f;
			this.extents += new Vector3(amount, amount, amount);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Expand(Vector3 amount)
		{
			this.extents += amount * 0.5f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Intersects(Bounds bounds)
		{
			return this.min.x <= bounds.max.x && this.max.x >= bounds.min.x && this.min.y <= bounds.max.y && this.max.y >= bounds.min.y && this.min.z <= bounds.max.z && this.max.z >= bounds.min.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IntersectRay(Ray ray)
		{
			float num;
			return Bounds.IntersectRayAABB(ray, this, out num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IntersectRay(Ray ray, out float distance)
		{
			return Bounds.IntersectRayAABB(ray, this, out distance);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString()
		{
			return this.ToString(null, null);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format)
		{
			return this.ToString(format, null);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
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
			return UnityString.Format("Center: {0}, Extents: {1}", new object[]
			{
				this.m_Center.ToString(format, formatProvider),
				this.m_Extents.ToString(format, formatProvider)
			});
		}

		[NativeMethod("IsInside", IsThreadSafe = true)]
		public bool Contains(Vector3 point)
		{
			return Bounds.Contains_Injected(ref this, ref point);
		}

		[FreeFunction("BoundsScripting::SqrDistance", HasExplicitThis = true, IsThreadSafe = true)]
		public float SqrDistance(Vector3 point)
		{
			return Bounds.SqrDistance_Injected(ref this, ref point);
		}

		[FreeFunction("IntersectRayAABB", IsThreadSafe = true)]
		private static bool IntersectRayAABB(Ray ray, Bounds bounds, out float dist)
		{
			return Bounds.IntersectRayAABB_Injected(ref ray, ref bounds, out dist);
		}

		[FreeFunction("BoundsScripting::ClosestPoint", HasExplicitThis = true, IsThreadSafe = true)]
		public Vector3 ClosestPoint(Vector3 point)
		{
			Vector3 vector;
			Bounds.ClosestPoint_Injected(ref this, ref point, out vector);
			return vector;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Contains_Injected(ref Bounds _unity_self, ref Vector3 point);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float SqrDistance_Injected(ref Bounds _unity_self, ref Vector3 point);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IntersectRayAABB_Injected(ref Ray ray, ref Bounds bounds, out float dist);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClosestPoint_Injected(ref Bounds _unity_self, ref Vector3 point, out Vector3 ret);

		private Vector3 m_Center;

		[NativeName("m_Extent")]
		private Vector3 m_Extents;
	}
}
