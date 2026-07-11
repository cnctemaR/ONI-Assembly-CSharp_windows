using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Parent class for collider types used with 2D gameplay.</para>
	/// </summary>
	[RequiredByNativeCode(Optional = true)]
	[NativeHeader("Modules/Physics2D/Public/Collider2D.h")]
	[RequireComponent(typeof(Transform))]
	public class Collider2D : Behaviour
	{
		/// <summary>
		///   <para>The density of the collider used to calculate its mass (when auto mass is enabled).</para>
		/// </summary>
		public extern float density
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Is this collider configured as a trigger?</para>
		/// </summary>
		public extern bool isTrigger
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Whether the collider is used by an attached effector or not.</para>
		/// </summary>
		public extern bool usedByEffector
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Sets whether the Collider will be used or not used by a CompositeCollider2D.</para>
		/// </summary>
		public extern bool usedByComposite
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Get the CompositeCollider2D that is available to be attached to the collider.</para>
		/// </summary>
		public extern CompositeCollider2D composite
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The local offset of the collider geometry.</para>
		/// </summary>
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

		/// <summary>
		///   <para>The Rigidbody2D attached to the Collider2D.</para>
		/// </summary>
		public extern Rigidbody2D attachedRigidbody
		{
			[NativeMethod("GetAttachedRigidbody_Binding")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The number of separate shaped regions in the collider.</para>
		/// </summary>
		public extern int shapeCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The world space bounding area of the collider.</para>
		/// </summary>
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

		/// <summary>
		///   <para>The PhysicsMaterial2D that is applied to this collider.</para>
		/// </summary>
		public extern PhysicsMaterial2D sharedMaterial
		{
			[NativeMethod("GetMaterial")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeMethod("SetMaterial")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Get the friction used by the collider.</para>
		/// </summary>
		public extern float friction
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Get the bounciness used by the collider.</para>
		/// </summary>
		public extern float bounciness
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Check whether this collider is touching the collider or not.</para>
		/// </summary>
		/// <param name="collider">The collider to check if it is touching this collider.</param>
		/// <returns>
		///   <para>Whether this collider is touching the collider or not.</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsTouching([NotNull] [Writable] Collider2D collider);

		/// <summary>
		///   <para>Check whether this collider is touching the collider or not with the results filtered by the ContactFilter2D.</para>
		/// </summary>
		/// <param name="collider">The collider to check if it is touching this collider.</param>
		/// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth, or normal angle.</param>
		/// <returns>
		///   <para>Whether this collider is touching the collider or not.</para>
		/// </returns>
		public bool IsTouching([Writable] Collider2D collider, ContactFilter2D contactFilter)
		{
			return this.IsTouching_OtherColliderWithFilter(collider, contactFilter);
		}

		[NativeMethod("IsTouching")]
		private bool IsTouching_OtherColliderWithFilter([NotNull] [Writable] Collider2D collider, ContactFilter2D contactFilter)
		{
			return this.IsTouching_OtherColliderWithFilter_Injected(collider, ref contactFilter);
		}

		/// <summary>
		///   <para>Check whether this collider is touching other colliders or not with the results filtered by the ContactFilter2D.</para>
		/// </summary>
		/// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth, or normal angle.</param>
		/// <returns>
		///   <para>Whether this collider is touching the collider or not.</para>
		/// </returns>
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

		/// <summary>
		///   <para>Checks whether this collider is touching any colliders on the specified layerMask or not.</para>
		/// </summary>
		/// <param name="layerMask">Any colliders on any of these layers count as touching.</param>
		/// <returns>
		///   <para>Whether this collider is touching any collider on the specified layerMask or not.</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsTouchingLayers([DefaultValue("Physics2D.AllLayers")] int layerMask);

		/// <summary>
		///   <para>Check if a collider overlaps a point in space.</para>
		/// </summary>
		/// <param name="point">A point in world space.</param>
		/// <returns>
		///   <para>Does point overlap the collider?</para>
		/// </returns>
		public bool OverlapPoint(Vector2 point)
		{
			return this.OverlapPoint_Injected(ref point);
		}

		/// <summary>
		///   <para>Calculates the minimum separation of this collider against another collider.</para>
		/// </summary>
		/// <param name="collider">A collider used to calculate the minimum separation against this collider.</param>
		/// <returns>
		///   <para>The minimum separation of collider and this collider.</para>
		/// </returns>
		public ColliderDistance2D Distance([Writable] Collider2D collider)
		{
			return Physics2D.Distance(this, collider);
		}

		/// <summary>
		///   <para>Get a list of all colliders that overlap this collider.</para>
		/// </summary>
		/// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth.  Note that normal angle is not used for overlap testing.</param>
		/// <param name="results">The array to receive results.  The size of the array determines the maximum number of results that can be returned.</param>
		/// <returns>
		///   <para>Returns the number of results placed in the results array.</para>
		/// </returns>
		public int OverlapCollider(ContactFilter2D contactFilter, Collider2D[] results)
		{
			return Physics2D.OverlapCollider(this, contactFilter, results);
		}

		/// <summary>
		///   <para>Retrieves all contact points for this collider.</para>
		/// </summary>
		/// <param name="contacts">An array of ContactPoint2D used to receive the results.</param>
		/// <returns>
		///   <para>Returns the number of contacts placed in the contacts array.</para>
		/// </returns>
		public int GetContacts(ContactPoint2D[] contacts)
		{
			return Physics2D.GetContacts(this, default(ContactFilter2D).NoFilter(), contacts);
		}

		/// <summary>
		///   <para>Retrieves all contact points for this collider, with the results filtered by the ContactFilter2D.</para>
		/// </summary>
		/// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth, or normal angle.</param>
		/// <param name="contacts">An array of ContactPoint2D used to receive the results.</param>
		/// <returns>
		///   <para>Returns the number of contacts placed in the contacts array.</para>
		/// </returns>
		public int GetContacts(ContactFilter2D contactFilter, ContactPoint2D[] contacts)
		{
			return Physics2D.GetContacts(this, contactFilter, contacts);
		}

		/// <summary>
		///   <para>Retrieves all colliders in contact with this collider.</para>
		/// </summary>
		/// <param name="colliders">An array of Collider2D used to receive the results.</param>
		/// <returns>
		///   <para>Returns the number of contacts placed in the colliders array.</para>
		/// </returns>
		public int GetContacts(Collider2D[] colliders)
		{
			return Physics2D.GetContacts(this, default(ContactFilter2D).NoFilter(), colliders);
		}

		/// <summary>
		///   <para>Retrieves all colliders in contact with this collider, with the results filtered by the ContactFilter2D.</para>
		/// </summary>
		/// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth, or normal angle.</param>
		/// <param name="colliders">An array of Collider2D used to receive the results.</param>
		/// <returns>
		///   <para>Returns the number of collidersplaced in the colliders array.</para>
		/// </returns>
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

		/// <summary>
		///   <para>Casts the collider shape into the scene starting at the collider position ignoring the collider itself.</para>
		/// </summary>
		/// <param name="direction">Vector representing the direction to cast the shape.</param>
		/// <param name="results">Array to receive results.</param>
		/// <param name="distance">Maximum distance over which to cast the shape.</param>
		/// <param name="ignoreSiblingColliders">Should colliders attached to the same Rigidbody2D (known as sibling colliders) be ignored?</param>
		/// <returns>
		///   <para>The number of results returned.</para>
		/// </returns>
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

		/// <summary>
		///   <para>Casts the collider shape into the scene starting at the collider position ignoring the collider itself.</para>
		/// </summary>
		/// <param name="direction">Vector representing the direction to cast the shape.</param>
		/// <param name="contactFilter">Filter results defined by the contact filter.</param>
		/// <param name="results">Array to receive results.</param>
		/// <param name="distance">Maximum distance over which to cast the shape.</param>
		/// <param name="ignoreSiblingColliders">Should colliders attached to the same Rigidbody2D (known as sibling colliders) be ignored?</param>
		/// <returns>
		///   <para>The number of results returned.</para>
		/// </returns>
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

		/// <summary>
		///   <para>Casts a ray into the scene starting at the collider position ignoring the collider itself.</para>
		/// </summary>
		/// <param name="direction">Vector representing the direction of the ray.</param>
		/// <param name="results">Array to receive results.</param>
		/// <param name="distance">Maximum distance over which to cast the ray.</param>
		/// <param name="layerMask">Filter to check objects only on specific layers.</param>
		/// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than this value.</param>
		/// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than this value.</param>
		/// <param name="contactFilter">Filter results defined by the contact filter.</param>
		/// <returns>
		///   <para>The number of results returned.</para>
		/// </returns>
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

		/// <summary>
		///   <para>Casts a ray into the scene starting at the collider position ignoring the collider itself.</para>
		/// </summary>
		/// <param name="direction">Vector representing the direction of the ray.</param>
		/// <param name="results">Array to receive results.</param>
		/// <param name="distance">Maximum distance over which to cast the ray.</param>
		/// <param name="layerMask">Filter to check objects only on specific layers.</param>
		/// <param name="minDepth">Only include objects with a Z coordinate (depth) greater than this value.</param>
		/// <param name="maxDepth">Only include objects with a Z coordinate (depth) less than this value.</param>
		/// <param name="contactFilter">Filter results defined by the contact filter.</param>
		/// <returns>
		///   <para>The number of results returned.</para>
		/// </returns>
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
