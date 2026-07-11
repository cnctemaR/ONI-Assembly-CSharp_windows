using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeHeader("Modules/Physics2D/Public/PhysicsSceneHandle2D.h")]
	public struct PhysicsScene2D : IEquatable<PhysicsScene2D>
	{
		public override string ToString()
		{
			return UnityString.Format("({0})", new object[] { this.m_Handle });
		}

		public static bool operator ==(PhysicsScene2D lhs, PhysicsScene2D rhs)
		{
			return lhs.m_Handle == rhs.m_Handle;
		}

		public static bool operator !=(PhysicsScene2D lhs, PhysicsScene2D rhs)
		{
			return lhs.m_Handle != rhs.m_Handle;
		}

		public override int GetHashCode()
		{
			return this.m_Handle;
		}

		public override bool Equals(object other)
		{
			bool flag;
			if (!(other is PhysicsScene2D))
			{
				flag = false;
			}
			else
			{
				PhysicsScene2D physicsScene2D = (PhysicsScene2D)other;
				flag = this.m_Handle == physicsScene2D.m_Handle;
			}
			return flag;
		}

		public bool Equals(PhysicsScene2D other)
		{
			return this.m_Handle == other.m_Handle;
		}

		public bool IsValid()
		{
			return PhysicsScene2D.IsValid_Internal(this);
		}

		[StaticAccessor("GetPhysicsManager2D()", StaticAccessorType.Arrow)]
		[NativeMethod("IsPhysicsSceneValid")]
		private static bool IsValid_Internal(PhysicsScene2D physicsScene)
		{
			return PhysicsScene2D.IsValid_Internal_Injected(ref physicsScene);
		}

		public bool IsEmpty()
		{
			if (this.IsValid())
			{
				return PhysicsScene2D.IsEmpty_Internal(this);
			}
			throw new InvalidOperationException("Cannot check if physics scene is empty as it is invalid.");
		}

		[StaticAccessor("GetPhysicsManager2D()", StaticAccessorType.Arrow)]
		[NativeMethod("IsPhysicsWorldEmpty")]
		private static bool IsEmpty_Internal(PhysicsScene2D physicsScene)
		{
			return PhysicsScene2D.IsEmpty_Internal_Injected(ref physicsScene);
		}

		public bool Simulate(float step)
		{
			if (this.IsValid())
			{
				return Physics2D.Simulate_Internal(this, step);
			}
			throw new InvalidOperationException("Cannot simulate the physics scene as it is invalid.");
		}

		public RaycastHit2D Linecast(Vector2 start, Vector2 end, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return PhysicsScene2D.Linecast_Internal(this, start, end, contactFilter2D);
		}

		public RaycastHit2D Linecast(Vector2 start, Vector2 end, ContactFilter2D contactFilter)
		{
			return PhysicsScene2D.Linecast_Internal(this, start, end, contactFilter);
		}

		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		[NativeMethod("Linecast_Binding")]
		private static RaycastHit2D Linecast_Internal(PhysicsScene2D physicsScene, Vector2 start, Vector2 end, ContactFilter2D contactFilter)
		{
			RaycastHit2D raycastHit2D;
			PhysicsScene2D.Linecast_Internal_Injected(ref physicsScene, ref start, ref end, ref contactFilter, out raycastHit2D);
			return raycastHit2D;
		}

		public int Linecast(Vector2 start, Vector2 end, RaycastHit2D[] results, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return PhysicsScene2D.LinecastNonAlloc_Internal(this, start, end, contactFilter2D, results);
		}

		public int Linecast(Vector2 start, Vector2 end, ContactFilter2D contactFilter, RaycastHit2D[] results)
		{
			return PhysicsScene2D.LinecastNonAlloc_Internal(this, start, end, contactFilter, results);
		}

		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		[NativeMethod("LinecastNonAlloc_Binding")]
		private static int LinecastNonAlloc_Internal(PhysicsScene2D physicsScene, Vector2 start, Vector2 end, ContactFilter2D contactFilter, [Out] RaycastHit2D[] results)
		{
			return PhysicsScene2D.LinecastNonAlloc_Internal_Injected(ref physicsScene, ref start, ref end, ref contactFilter, results);
		}

		public RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return PhysicsScene2D.Raycast_Internal(this, origin, direction, distance, contactFilter2D);
		}

		public RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance, ContactFilter2D contactFilter)
		{
			return PhysicsScene2D.Raycast_Internal(this, origin, direction, distance, contactFilter);
		}

		[NativeMethod("Raycast_Binding")]
		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		private static RaycastHit2D Raycast_Internal(PhysicsScene2D physicsScene, Vector2 origin, Vector2 direction, float distance, ContactFilter2D contactFilter)
		{
			RaycastHit2D raycastHit2D;
			PhysicsScene2D.Raycast_Internal_Injected(ref physicsScene, ref origin, ref direction, distance, ref contactFilter, out raycastHit2D);
			return raycastHit2D;
		}

		public int Raycast(Vector2 origin, Vector2 direction, float distance, RaycastHit2D[] results, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return PhysicsScene2D.RaycastNonAlloc_Internal(this, origin, direction, distance, contactFilter2D, results);
		}

		public int Raycast(Vector2 origin, Vector2 direction, float distance, ContactFilter2D contactFilter, RaycastHit2D[] results)
		{
			return PhysicsScene2D.RaycastNonAlloc_Internal(this, origin, direction, distance, contactFilter, results);
		}

		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		[NativeMethod("RaycastNonAlloc_Binding")]
		private static int RaycastNonAlloc_Internal(PhysicsScene2D physicsScene, Vector2 origin, Vector2 direction, float distance, ContactFilter2D contactFilter, [Out] RaycastHit2D[] results)
		{
			return PhysicsScene2D.RaycastNonAlloc_Internal_Injected(ref physicsScene, ref origin, ref direction, distance, ref contactFilter, results);
		}

		public RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction, float distance, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return PhysicsScene2D.CircleCast_Internal(this, origin, radius, direction, distance, contactFilter2D);
		}

		public RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction, float distance, ContactFilter2D contactFilter)
		{
			return PhysicsScene2D.CircleCast_Internal(this, origin, radius, direction, distance, contactFilter);
		}

		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		[NativeMethod("CircleCast_Binding")]
		private static RaycastHit2D CircleCast_Internal(PhysicsScene2D physicsScene, Vector2 origin, float radius, Vector2 direction, float distance, ContactFilter2D contactFilter)
		{
			RaycastHit2D raycastHit2D;
			PhysicsScene2D.CircleCast_Internal_Injected(ref physicsScene, ref origin, radius, ref direction, distance, ref contactFilter, out raycastHit2D);
			return raycastHit2D;
		}

		public int CircleCast(Vector2 origin, float radius, Vector2 direction, float distance, RaycastHit2D[] results, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return PhysicsScene2D.CircleCastNonAlloc_Internal(this, origin, radius, direction, distance, contactFilter2D, results);
		}

		public int CircleCast(Vector2 origin, float radius, Vector2 direction, float distance, ContactFilter2D contactFilter, RaycastHit2D[] results)
		{
			return PhysicsScene2D.CircleCastNonAlloc_Internal(this, origin, radius, direction, distance, contactFilter, results);
		}

		[NativeMethod("CircleCastNonAlloc_Binding")]
		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		private static int CircleCastNonAlloc_Internal(PhysicsScene2D physicsScene, Vector2 origin, float radius, Vector2 direction, float distance, ContactFilter2D contactFilter, [Out] RaycastHit2D[] results)
		{
			return PhysicsScene2D.CircleCastNonAlloc_Internal_Injected(ref physicsScene, ref origin, radius, ref direction, distance, ref contactFilter, results);
		}

		public RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return PhysicsScene2D.BoxCast_Internal(this, origin, size, angle, direction, distance, contactFilter2D);
		}

		public RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter)
		{
			return PhysicsScene2D.BoxCast_Internal(this, origin, size, angle, direction, distance, contactFilter);
		}

		[NativeMethod("BoxCast_Binding")]
		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		private static RaycastHit2D BoxCast_Internal(PhysicsScene2D physicsScene, Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter)
		{
			RaycastHit2D raycastHit2D;
			PhysicsScene2D.BoxCast_Internal_Injected(ref physicsScene, ref origin, ref size, angle, ref direction, distance, ref contactFilter, out raycastHit2D);
			return raycastHit2D;
		}

		public int BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, RaycastHit2D[] results, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return PhysicsScene2D.BoxCastNonAlloc_Internal(this, origin, size, angle, direction, distance, contactFilter2D, results);
		}

		public int BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter, RaycastHit2D[] results)
		{
			return PhysicsScene2D.BoxCastNonAlloc_Internal(this, origin, size, angle, direction, distance, contactFilter, results);
		}

		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		[NativeMethod("BoxCastNonAlloc_Binding")]
		private static int BoxCastNonAlloc_Internal(PhysicsScene2D physicsScene, Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter, [Out] RaycastHit2D[] results)
		{
			return PhysicsScene2D.BoxCastNonAlloc_Internal_Injected(ref physicsScene, ref origin, ref size, angle, ref direction, distance, ref contactFilter, results);
		}

		public RaycastHit2D CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return PhysicsScene2D.CapsuleCast_Internal(this, origin, size, capsuleDirection, angle, direction, distance, contactFilter2D);
		}

		public RaycastHit2D CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter)
		{
			return PhysicsScene2D.CapsuleCast_Internal(this, origin, size, capsuleDirection, angle, direction, distance, contactFilter);
		}

		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		[NativeMethod("CapsuleCast_Binding")]
		private static RaycastHit2D CapsuleCast_Internal(PhysicsScene2D physicsScene, Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter)
		{
			RaycastHit2D raycastHit2D;
			PhysicsScene2D.CapsuleCast_Internal_Injected(ref physicsScene, ref origin, ref size, capsuleDirection, angle, ref direction, distance, ref contactFilter, out raycastHit2D);
			return raycastHit2D;
		}

		public int CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, RaycastHit2D[] results, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return PhysicsScene2D.CapsuleCastNonAlloc_Internal(this, origin, size, capsuleDirection, angle, direction, distance, contactFilter2D, results);
		}

		public int CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter, RaycastHit2D[] results)
		{
			return PhysicsScene2D.CapsuleCastNonAlloc_Internal(this, origin, size, capsuleDirection, angle, direction, distance, contactFilter, results);
		}

		[NativeMethod("CapsuleCastNonAlloc_Binding")]
		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		private static int CapsuleCastNonAlloc_Internal(PhysicsScene2D physicsScene, Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter, [Out] RaycastHit2D[] results)
		{
			return PhysicsScene2D.CapsuleCastNonAlloc_Internal_Injected(ref physicsScene, ref origin, ref size, capsuleDirection, angle, ref direction, distance, ref contactFilter, results);
		}

		public RaycastHit2D GetRayIntersection(Ray ray, float distance, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
		{
			return PhysicsScene2D.GetRayIntersection_Internal(this, ray.origin, ray.direction, distance, layerMask);
		}

		[NativeMethod("GetRayIntersection_Binding")]
		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		private static RaycastHit2D GetRayIntersection_Internal(PhysicsScene2D physicsScene, Vector3 origin, Vector3 direction, float distance, int layerMask)
		{
			RaycastHit2D raycastHit2D;
			PhysicsScene2D.GetRayIntersection_Internal_Injected(ref physicsScene, ref origin, ref direction, distance, layerMask, out raycastHit2D);
			return raycastHit2D;
		}

		public int GetRayIntersection(Ray ray, float distance, RaycastHit2D[] results, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
		{
			return PhysicsScene2D.GetRayIntersectionNonAlloc_Internal(this, ray.origin, ray.direction, distance, layerMask, results);
		}

		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		[NativeMethod("GetRayIntersectionNonAlloc_Binding")]
		private static int GetRayIntersectionNonAlloc_Internal(PhysicsScene2D physicsScene, Vector3 origin, Vector3 direction, float distance, int layerMask, [Out] RaycastHit2D[] results)
		{
			return PhysicsScene2D.GetRayIntersectionNonAlloc_Internal_Injected(ref physicsScene, ref origin, ref direction, distance, layerMask, results);
		}

		public Collider2D OverlapPoint(Vector2 point, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return PhysicsScene2D.OverlapPoint_Internal(this, point, contactFilter2D);
		}

		public Collider2D OverlapPoint(Vector2 point, ContactFilter2D contactFilter)
		{
			return PhysicsScene2D.OverlapPoint_Internal(this, point, contactFilter);
		}

		[NativeMethod("OverlapPoint_Binding")]
		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		private static Collider2D OverlapPoint_Internal(PhysicsScene2D physicsScene, Vector2 point, ContactFilter2D contactFilter)
		{
			return PhysicsScene2D.OverlapPoint_Internal_Injected(ref physicsScene, ref point, ref contactFilter);
		}

		public int OverlapPoint(Vector2 point, Collider2D[] results, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return PhysicsScene2D.OverlapPointNonAlloc_Internal(this, point, contactFilter2D, results);
		}

		public int OverlapPoint(Vector2 point, ContactFilter2D contactFilter, Collider2D[] results)
		{
			return PhysicsScene2D.OverlapPointNonAlloc_Internal(this, point, contactFilter, results);
		}

		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		[NativeMethod("OverlapPointNonAlloc_Binding")]
		private static int OverlapPointNonAlloc_Internal(PhysicsScene2D physicsScene, Vector2 point, ContactFilter2D contactFilter, [Out] Collider2D[] results)
		{
			return PhysicsScene2D.OverlapPointNonAlloc_Internal_Injected(ref physicsScene, ref point, ref contactFilter, results);
		}

		public Collider2D OverlapCircle(Vector2 point, float radius, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return PhysicsScene2D.OverlapCircle_Internal(this, point, radius, contactFilter2D);
		}

		public Collider2D OverlapCircle(Vector2 point, float radius, ContactFilter2D contactFilter)
		{
			return PhysicsScene2D.OverlapCircle_Internal(this, point, radius, contactFilter);
		}

		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		[NativeMethod("OverlapCircle_Binding")]
		private static Collider2D OverlapCircle_Internal(PhysicsScene2D physicsScene, Vector2 point, float radius, ContactFilter2D contactFilter)
		{
			return PhysicsScene2D.OverlapCircle_Internal_Injected(ref physicsScene, ref point, radius, ref contactFilter);
		}

		public int OverlapCircle(Vector2 point, float radius, Collider2D[] results, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return PhysicsScene2D.OverlapCircleNonAlloc_Internal(this, point, radius, contactFilter2D, results);
		}

		public int OverlapCircle(Vector2 point, float radius, ContactFilter2D contactFilter, Collider2D[] results)
		{
			return PhysicsScene2D.OverlapCircleNonAlloc_Internal(this, point, radius, contactFilter, results);
		}

		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		[NativeMethod("OverlapCircleNonAlloc_Binding")]
		private static int OverlapCircleNonAlloc_Internal(PhysicsScene2D physicsScene, Vector2 point, float radius, ContactFilter2D contactFilter, [Out] Collider2D[] results)
		{
			return PhysicsScene2D.OverlapCircleNonAlloc_Internal_Injected(ref physicsScene, ref point, radius, ref contactFilter, results);
		}

		public Collider2D OverlapBox(Vector2 point, Vector2 size, float angle, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return PhysicsScene2D.OverlapBox_Internal(this, point, size, angle, contactFilter2D);
		}

		public Collider2D OverlapBox(Vector2 point, Vector2 size, float angle, ContactFilter2D contactFilter)
		{
			return PhysicsScene2D.OverlapBox_Internal(this, point, size, angle, contactFilter);
		}

		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		[NativeMethod("OverlapBox_Binding")]
		private static Collider2D OverlapBox_Internal(PhysicsScene2D physicsScene, Vector2 point, Vector2 size, float angle, ContactFilter2D contactFilter)
		{
			return PhysicsScene2D.OverlapBox_Internal_Injected(ref physicsScene, ref point, ref size, angle, ref contactFilter);
		}

		public int OverlapBox(Vector2 point, Vector2 size, float angle, Collider2D[] results, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return PhysicsScene2D.OverlapBoxNonAlloc_Internal(this, point, size, angle, contactFilter2D, results);
		}

		public int OverlapBox(Vector2 point, Vector2 size, float angle, ContactFilter2D contactFilter, Collider2D[] results)
		{
			return PhysicsScene2D.OverlapBoxNonAlloc_Internal(this, point, size, angle, contactFilter, results);
		}

		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		[NativeMethod("OverlapBoxNonAlloc_Binding")]
		private static int OverlapBoxNonAlloc_Internal(PhysicsScene2D physicsScene, Vector2 point, Vector2 size, float angle, ContactFilter2D contactFilter, [Out] Collider2D[] results)
		{
			return PhysicsScene2D.OverlapBoxNonAlloc_Internal_Injected(ref physicsScene, ref point, ref size, angle, ref contactFilter, results);
		}

		public Collider2D OverlapArea(Vector2 pointA, Vector2 pointB, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return this.OverlapAreaToBox_Internal(pointA, pointB, contactFilter2D);
		}

		public Collider2D OverlapArea(Vector2 pointA, Vector2 pointB, ContactFilter2D contactFilter)
		{
			return this.OverlapAreaToBox_Internal(pointA, pointB, contactFilter);
		}

		private Collider2D OverlapAreaToBox_Internal(Vector2 pointA, Vector2 pointB, ContactFilter2D contactFilter)
		{
			Vector2 vector = (pointA + pointB) * 0.5f;
			Vector2 vector2 = new Vector2(Mathf.Abs(pointA.x - pointB.x), Math.Abs(pointA.y - pointB.y));
			return this.OverlapBox(vector, vector2, 0f, contactFilter);
		}

		public int OverlapArea(Vector2 pointA, Vector2 pointB, Collider2D[] results, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return this.OverlapAreaNonAllocToBox_Internal(pointA, pointB, contactFilter2D, results);
		}

		public int OverlapArea(Vector2 pointA, Vector2 pointB, ContactFilter2D contactFilter, Collider2D[] results)
		{
			return this.OverlapAreaNonAllocToBox_Internal(pointA, pointB, contactFilter, results);
		}

		private int OverlapAreaNonAllocToBox_Internal(Vector2 pointA, Vector2 pointB, ContactFilter2D contactFilter, Collider2D[] results)
		{
			Vector2 vector = (pointA + pointB) * 0.5f;
			Vector2 vector2 = new Vector2(Mathf.Abs(pointA.x - pointB.x), Math.Abs(pointA.y - pointB.y));
			return this.OverlapBox(vector, vector2, 0f, contactFilter, results);
		}

		public Collider2D OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return PhysicsScene2D.OverlapCapsule_Internal(this, point, size, direction, angle, contactFilter2D);
		}

		public Collider2D OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, ContactFilter2D contactFilter)
		{
			return PhysicsScene2D.OverlapCapsule_Internal(this, point, size, direction, angle, contactFilter);
		}

		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		[NativeMethod("OverlapCapsule_Binding")]
		private static Collider2D OverlapCapsule_Internal(PhysicsScene2D physicsScene, Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, ContactFilter2D contactFilter)
		{
			return PhysicsScene2D.OverlapCapsule_Internal_Injected(ref physicsScene, ref point, ref size, direction, angle, ref contactFilter);
		}

		public int OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, Collider2D[] results, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return PhysicsScene2D.OverlapCapsuleNonAlloc_Internal(this, point, size, direction, angle, contactFilter2D, results);
		}

		public int OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, ContactFilter2D contactFilter, Collider2D[] results)
		{
			return PhysicsScene2D.OverlapCapsuleNonAlloc_Internal(this, point, size, direction, angle, contactFilter, results);
		}

		[NativeMethod("OverlapCapsuleNonAlloc_Binding")]
		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		private static int OverlapCapsuleNonAlloc_Internal(PhysicsScene2D physicsScene, Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, ContactFilter2D contactFilter, [Out] Collider2D[] results)
		{
			return PhysicsScene2D.OverlapCapsuleNonAlloc_Internal_Injected(ref physicsScene, ref point, ref size, direction, angle, ref contactFilter, results);
		}

		public static int OverlapCollider(Collider2D collider, Collider2D[] results, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return PhysicsScene2D.OverlapCollider_Internal(collider, contactFilter2D, results);
		}

		public static int OverlapCollider(Collider2D collider, ContactFilter2D contactFilter, Collider2D[] results)
		{
			return PhysicsScene2D.OverlapCollider_Internal(collider, contactFilter, results);
		}

		[NativeMethod("OverlapCollider_Binding")]
		[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
		private static int OverlapCollider_Internal([NotNull] Collider2D collider, ContactFilter2D contactFilter, [Out] Collider2D[] results)
		{
			return PhysicsScene2D.OverlapCollider_Internal_Injected(collider, ref contactFilter, results);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsValid_Internal_Injected(ref PhysicsScene2D physicsScene);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsEmpty_Internal_Injected(ref PhysicsScene2D physicsScene);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Linecast_Internal_Injected(ref PhysicsScene2D physicsScene, ref Vector2 start, ref Vector2 end, ref ContactFilter2D contactFilter, out RaycastHit2D ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int LinecastNonAlloc_Internal_Injected(ref PhysicsScene2D physicsScene, ref Vector2 start, ref Vector2 end, ref ContactFilter2D contactFilter, [Out] RaycastHit2D[] results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Raycast_Internal_Injected(ref PhysicsScene2D physicsScene, ref Vector2 origin, ref Vector2 direction, float distance, ref ContactFilter2D contactFilter, out RaycastHit2D ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int RaycastNonAlloc_Internal_Injected(ref PhysicsScene2D physicsScene, ref Vector2 origin, ref Vector2 direction, float distance, ref ContactFilter2D contactFilter, [Out] RaycastHit2D[] results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CircleCast_Internal_Injected(ref PhysicsScene2D physicsScene, ref Vector2 origin, float radius, ref Vector2 direction, float distance, ref ContactFilter2D contactFilter, out RaycastHit2D ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int CircleCastNonAlloc_Internal_Injected(ref PhysicsScene2D physicsScene, ref Vector2 origin, float radius, ref Vector2 direction, float distance, ref ContactFilter2D contactFilter, [Out] RaycastHit2D[] results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BoxCast_Internal_Injected(ref PhysicsScene2D physicsScene, ref Vector2 origin, ref Vector2 size, float angle, ref Vector2 direction, float distance, ref ContactFilter2D contactFilter, out RaycastHit2D ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int BoxCastNonAlloc_Internal_Injected(ref PhysicsScene2D physicsScene, ref Vector2 origin, ref Vector2 size, float angle, ref Vector2 direction, float distance, ref ContactFilter2D contactFilter, [Out] RaycastHit2D[] results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CapsuleCast_Internal_Injected(ref PhysicsScene2D physicsScene, ref Vector2 origin, ref Vector2 size, CapsuleDirection2D capsuleDirection, float angle, ref Vector2 direction, float distance, ref ContactFilter2D contactFilter, out RaycastHit2D ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int CapsuleCastNonAlloc_Internal_Injected(ref PhysicsScene2D physicsScene, ref Vector2 origin, ref Vector2 size, CapsuleDirection2D capsuleDirection, float angle, ref Vector2 direction, float distance, ref ContactFilter2D contactFilter, [Out] RaycastHit2D[] results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetRayIntersection_Internal_Injected(ref PhysicsScene2D physicsScene, ref Vector3 origin, ref Vector3 direction, float distance, int layerMask, out RaycastHit2D ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetRayIntersectionNonAlloc_Internal_Injected(ref PhysicsScene2D physicsScene, ref Vector3 origin, ref Vector3 direction, float distance, int layerMask, [Out] RaycastHit2D[] results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Collider2D OverlapPoint_Internal_Injected(ref PhysicsScene2D physicsScene, ref Vector2 point, ref ContactFilter2D contactFilter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int OverlapPointNonAlloc_Internal_Injected(ref PhysicsScene2D physicsScene, ref Vector2 point, ref ContactFilter2D contactFilter, [Out] Collider2D[] results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Collider2D OverlapCircle_Internal_Injected(ref PhysicsScene2D physicsScene, ref Vector2 point, float radius, ref ContactFilter2D contactFilter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int OverlapCircleNonAlloc_Internal_Injected(ref PhysicsScene2D physicsScene, ref Vector2 point, float radius, ref ContactFilter2D contactFilter, [Out] Collider2D[] results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Collider2D OverlapBox_Internal_Injected(ref PhysicsScene2D physicsScene, ref Vector2 point, ref Vector2 size, float angle, ref ContactFilter2D contactFilter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int OverlapBoxNonAlloc_Internal_Injected(ref PhysicsScene2D physicsScene, ref Vector2 point, ref Vector2 size, float angle, ref ContactFilter2D contactFilter, [Out] Collider2D[] results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Collider2D OverlapCapsule_Internal_Injected(ref PhysicsScene2D physicsScene, ref Vector2 point, ref Vector2 size, CapsuleDirection2D direction, float angle, ref ContactFilter2D contactFilter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int OverlapCapsuleNonAlloc_Internal_Injected(ref PhysicsScene2D physicsScene, ref Vector2 point, ref Vector2 size, CapsuleDirection2D direction, float angle, ref ContactFilter2D contactFilter, [Out] Collider2D[] results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int OverlapCollider_Internal_Injected(Collider2D collider, ref ContactFilter2D contactFilter, [Out] Collider2D[] results);

		private int m_Handle;
	}
}
