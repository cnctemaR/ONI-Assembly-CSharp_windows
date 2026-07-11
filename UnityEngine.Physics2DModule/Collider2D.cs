using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode(Optional = true)]
	[NativeHeader("Modules/Physics2D/Public/Collider2D.h")]
	[RequireComponent(typeof(Transform))]
	public class Collider2D : Behaviour
	{
		public extern float density
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool isTrigger
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool usedByEffector
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool usedByComposite
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern CompositeCollider2D composite
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public Vector2 offset
		{
			get
			{
				Vector2 vector;
				this.get_offset_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_offset_Injected(ref value);
			}
		}

		public extern Rigidbody2D attachedRigidbody
		{
			[NativeMethod("GetAttachedRigidbody_Binding")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int shapeCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public Bounds bounds
		{
			get
			{
				Bounds bounds;
				this.get_bounds_Injected(out bounds);
				return bounds;
			}
		}

		internal extern ColliderErrorState2D errorState
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern bool compositeCapable
		{
			[NativeMethod("GetCompositeCapable_Binding")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern PhysicsMaterial2D sharedMaterial
		{
			[NativeMethod("GetMaterial")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeMethod("SetMaterial")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float friction
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float bounciness
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsTouching([Writable] [NotNull] Collider2D collider);

		public bool IsTouching([Writable] Collider2D collider, ContactFilter2D contactFilter)
		{
			return this.IsTouching_OtherColliderWithFilter(collider, contactFilter);
		}

		[NativeMethod("IsTouching")]
		private bool IsTouching_OtherColliderWithFilter([NotNull] [Writable] Collider2D collider, ContactFilter2D contactFilter)
		{
			return this.IsTouching_OtherColliderWithFilter_Injected(collider, ref contactFilter);
		}

		public bool IsTouching(ContactFilter2D contactFilter)
		{
			return this.IsTouching_AnyColliderWithFilter(contactFilter);
		}

		[NativeMethod("IsTouching")]
		private bool IsTouching_AnyColliderWithFilter(ContactFilter2D contactFilter)
		{
			return this.IsTouching_AnyColliderWithFilter_Injected(ref contactFilter);
		}

		[ExcludeFromDocs]
		public bool IsTouchingLayers()
		{
			return this.IsTouchingLayers(-1);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsTouchingLayers([DefaultValue("Physics2D.AllLayers")] int layerMask);

		public bool OverlapPoint(Vector2 point)
		{
			return this.OverlapPoint_Injected(ref point);
		}

		public ColliderDistance2D Distance([Writable] Collider2D collider)
		{
			return Physics2D.Distance(this, collider);
		}

		public int OverlapCollider(ContactFilter2D contactFilter, Collider2D[] results)
		{
			return PhysicsScene2D.OverlapCollider(this, contactFilter, results);
		}

		public int GetContacts(ContactPoint2D[] contacts)
		{
			return Physics2D.GetContacts(this, default(ContactFilter2D).NoFilter(), contacts);
		}

		public int GetContacts(ContactFilter2D contactFilter, ContactPoint2D[] contacts)
		{
			return Physics2D.GetContacts(this, contactFilter, contacts);
		}

		public int GetContacts(Collider2D[] colliders)
		{
			return Physics2D.GetContacts(this, default(ContactFilter2D).NoFilter(), colliders);
		}

		public int GetContacts(ContactFilter2D contactFilter, Collider2D[] colliders)
		{
			return Physics2D.GetContacts(this, contactFilter, colliders);
		}

		[ExcludeFromDocs]
		public int Cast(Vector2 direction, RaycastHit2D[] results)
		{
			ContactFilter2D contactFilter2D = default(ContactFilter2D);
			contactFilter2D.useTriggers = Physics2D.queriesHitTriggers;
			contactFilter2D.SetLayerMask(Physics2D.GetLayerCollisionMask(base.gameObject.layer));
			return this.Cast_Internal(direction, float.PositiveInfinity, contactFilter2D, true, results);
		}

		[ExcludeFromDocs]
		public int Cast(Vector2 direction, RaycastHit2D[] results, float distance)
		{
			ContactFilter2D contactFilter2D = default(ContactFilter2D);
			contactFilter2D.useTriggers = Physics2D.queriesHitTriggers;
			contactFilter2D.SetLayerMask(Physics2D.GetLayerCollisionMask(base.gameObject.layer));
			return this.Cast_Internal(direction, distance, contactFilter2D, true, results);
		}

		public int Cast(Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("true")] bool ignoreSiblingColliders)
		{
			ContactFilter2D contactFilter2D = default(ContactFilter2D);
			contactFilter2D.useTriggers = Physics2D.queriesHitTriggers;
			contactFilter2D.SetLayerMask(Physics2D.GetLayerCollisionMask(base.gameObject.layer));
			return this.Cast_Internal(direction, distance, contactFilter2D, ignoreSiblingColliders, results);
		}

		[ExcludeFromDocs]
		public int Cast(Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results)
		{
			return this.Cast_Internal(direction, float.PositiveInfinity, contactFilter, true, results);
		}

		[ExcludeFromDocs]
		public int Cast(Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, float distance)
		{
			return this.Cast_Internal(direction, distance, contactFilter, true, results);
		}

		public int Cast(Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("true")] bool ignoreSiblingColliders)
		{
			return this.Cast_Internal(direction, distance, contactFilter, ignoreSiblingColliders, results);
		}

		[NativeMethod("Cast_Binding")]
		private int Cast_Internal(Vector2 direction, float distance, ContactFilter2D contactFilter, bool ignoreSiblingColliders, [Out] RaycastHit2D[] results)
		{
			return this.Cast_Internal_Injected(ref direction, distance, ref contactFilter, ignoreSiblingColliders, results);
		}

		[ExcludeFromDocs]
		public int Raycast(Vector2 direction, RaycastHit2D[] results)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(-1, float.NegativeInfinity, float.PositiveInfinity);
			return this.Raycast_Internal(direction, float.PositiveInfinity, contactFilter2D, results);
		}

		[ExcludeFromDocs]
		public int Raycast(Vector2 direction, RaycastHit2D[] results, float distance)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(-1, float.NegativeInfinity, float.PositiveInfinity);
			return this.Raycast_Internal(direction, distance, contactFilter2D, results);
		}

		[ExcludeFromDocs]
		public int Raycast(Vector2 direction, RaycastHit2D[] results, float distance, int layerMask)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
			return this.Raycast_Internal(direction, distance, contactFilter2D, results);
		}

		[ExcludeFromDocs]
		public int Raycast(Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
			return this.Raycast_Internal(direction, distance, contactFilter2D, results);
		}

		public int Raycast(Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("Physics2D.AllLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
		{
			ContactFilter2D contactFilter2D = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
			return this.Raycast_Internal(direction, distance, contactFilter2D, results);
		}

		[ExcludeFromDocs]
		public int Raycast(Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results)
		{
			return this.Raycast_Internal(direction, float.PositiveInfinity, contactFilter, results);
		}

		public int Raycast(Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance)
		{
			return this.Raycast_Internal(direction, distance, contactFilter, results);
		}

		[NativeMethod("Raycast_Binding")]
		private int Raycast_Internal(Vector2 direction, float distance, ContactFilter2D contactFilter, [Out] RaycastHit2D[] results)
		{
			return this.Raycast_Internal_Injected(ref direction, distance, ref contactFilter, results);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_offset_Injected(out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_offset_Injected(ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_bounds_Injected(out Bounds ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool IsTouching_OtherColliderWithFilter_Injected([Writable] Collider2D collider, ref ContactFilter2D contactFilter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool IsTouching_AnyColliderWithFilter_Injected(ref ContactFilter2D contactFilter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool OverlapPoint_Injected(ref Vector2 point);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int Cast_Internal_Injected(ref Vector2 direction, float distance, ref ContactFilter2D contactFilter, bool ignoreSiblingColliders, [Out] RaycastHit2D[] results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int Raycast_Internal_Injected(ref Vector2 direction, float distance, ref ContactFilter2D contactFilter, [Out] RaycastHit2D[] results);
	}
}
