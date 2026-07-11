using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	/// <summary>
	///   <para>Rigidbody physics component for 2D sprites.</para>
	/// </summary>
	[RequireComponent(typeof(Transform))]
	[NativeHeader("Modules/Physics2D/Public/Rigidbody2D.h")]
	public sealed class Rigidbody2D : Component
	{
		/// <summary>
		///   <para>The position of the rigidbody.</para>
		/// </summary>
		public Vector2 position
		{
			get
			{
				Vector2 vector;
				this.get_position_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_position_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>The rotation of the rigidbody.</para>
		/// </summary>
		public extern float rotation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Moves the rigidbody to position.</para>
		/// </summary>
		/// <param name="position">The new position for the Rigidbody object.</param>
		public void MovePosition(Vector2 position)
		{
			this.MovePosition_Injected(ref position);
		}

		/// <summary>
		///   <para>Rotates the rigidbody to angle (given in degrees).</para>
		/// </summary>
		/// <param name="angle">The new rotation angle for the Rigidbody object.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void MoveRotation(float angle);

		/// <summary>
		///   <para>Linear velocity of the rigidbody.</para>
		/// </summary>
		public Vector2 velocity
		{
			get
			{
				Vector2 vector;
				this.get_velocity_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_velocity_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Angular velocity in degrees per second.</para>
		/// </summary>
		public extern float angularVelocity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Should the total rigid-body mass be automatically calculated from the Collider2D.density of attached colliders?</para>
		/// </summary>
		public extern bool useAutoMass
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Mass of the Rigidbody.</para>
		/// </summary>
		public extern float mass
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The PhysicsMaterial2D that is applied to all Collider2D attached to this Rigidbody2D.</para>
		/// </summary>
		[NativeMethod("Material")]
		public extern PhysicsMaterial2D sharedMaterial
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The center of mass of the rigidBody in local space.</para>
		/// </summary>
		public Vector2 centerOfMass
		{
			get
			{
				Vector2 vector;
				this.get_centerOfMass_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_centerOfMass_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Gets the center of mass of the rigidBody in global space.</para>
		/// </summary>
		public Vector2 worldCenterOfMass
		{
			get
			{
				Vector2 vector;
				this.get_worldCenterOfMass_Injected(out vector);
				return vector;
			}
		}

		/// <summary>
		///   <para>The rigidBody rotational inertia.</para>
		/// </summary>
		public extern float inertia
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Coefficient of drag.</para>
		/// </summary>
		public extern float drag
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Coefficient of angular drag.</para>
		/// </summary>
		public extern float angularDrag
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The degree to which this object is affected by gravity.</para>
		/// </summary>
		public extern float gravityScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The physical behaviour type of the Rigidbody2D.</para>
		/// </summary>
		public extern RigidbodyType2D bodyType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeMethod("SetBodyType_Binding")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetDragBehaviour(bool dragged);

		/// <summary>
		///   <para>Should kinematickinematic and kinematicstatic collisions be allowed?</para>
		/// </summary>
		public extern bool useFullKinematicContacts
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Should this rigidbody be taken out of physics control?</para>
		/// </summary>
		public bool isKinematic
		{
			get
			{
				return this.bodyType == RigidbodyType2D.Kinematic;
			}
			set
			{
				this.bodyType = ((!value) ? RigidbodyType2D.Dynamic : RigidbodyType2D.Kinematic);
			}
		}

		/// <summary>
		///   <para>Should the rigidbody be prevented from rotating?</para>
		/// </summary>
		[Obsolete("'fixedAngle' is no longer supported. Use constraints instead.", false)]
		[NativeMethod("FreezeRotation")]
		public extern bool fixedAngle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Controls whether physics will change the rotation of the object.</para>
		/// </summary>
		public extern bool freezeRotation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Controls which degrees of freedom are allowed for the simulation of this Rigidbody2D.</para>
		/// </summary>
		public extern RigidbodyConstraints2D constraints
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Is the rigidbody "sleeping"?</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsSleeping();

		/// <summary>
		///   <para>Is the rigidbody "awake"?</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsAwake();

		/// <summary>
		///   <para>Make the rigidbody "sleep".</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Sleep();

		/// <summary>
		///   <para>Disables the "sleeping" state of a rigidbody.</para>
		/// </summary>
		[NativeMethod("Wake")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void WakeUp();

		/// <summary>
		///   <para>Indicates whether the rigid body should be simulated or not by the physics system.</para>
		/// </summary>
		public extern bool simulated
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeMethod("SetSimulated_Binding")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Physics interpolation used between updates.</para>
		/// </summary>
		public extern RigidbodyInterpolation2D interpolation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The sleep state that the rigidbody will initially be in.</para>
		/// </summary>
		public extern RigidbodySleepMode2D sleepMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The method used by the physics engine to check if two objects have collided.</para>
		/// </summary>
		public extern CollisionDetectionMode2D collisionDetectionMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Returns the number of Collider2D attached to this Rigidbody2D.</para>
		/// </summary>
		public extern int attachedColliderCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Checks whether the collider  is touching any of the collider(s) attached to this rigidbody or not.</para>
		/// </summary>
		/// <param name="collider">The collider to check if it is touching any of the collider(s) attached to this rigidbody.</param>
		/// <returns>
		///   <para>Whether the collider is touching any of the collider(s) attached to this rigidbody or not.</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsTouching([NotNull] [Writable] Collider2D collider);

		/// <summary>
		///   <para>Checks whether the collider  is touching any of the collider(s) attached to this rigidbody or not with the results filtered by the ContactFilter2D.</para>
		/// </summary>
		/// <param name="collider">The collider to check if it is touching any of the collider(s) attached to this rigidbody.</param>
		/// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth, or normal angle.</param>
		/// <returns>
		///   <para>Whether the collider is touching any of the collider(s) attached to this rigidbody or not.</para>
		/// </returns>
		public bool IsTouching([Writable] Collider2D collider, ContactFilter2D contactFilter)
		{
			return this.IsTouching_OtherColliderWithFilter_Internal(collider, contactFilter);
		}

		[NativeMethod("IsTouching")]
		private bool IsTouching_OtherColliderWithFilter_Internal([NotNull] [Writable] Collider2D collider, ContactFilter2D contactFilter)
		{
			return this.IsTouching_OtherColliderWithFilter_Internal_Injected(collider, ref contactFilter);
		}

		/// <summary>
		///   <para>Checks whether any collider is touching any of the collider(s) attached to this rigidbody or not with the results filtered by the ContactFilter2D.</para>
		/// </summary>
		/// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth, or normal angle.</param>
		/// <returns>
		///   <para>Whether any collider is touching any of the collider(s) attached to this rigidbody or not.</para>
		/// </returns>
		public bool IsTouching(ContactFilter2D contactFilter)
		{
			return this.IsTouching_AnyColliderWithFilter_Internal(contactFilter);
		}

		[NativeMethod("IsTouching")]
		private bool IsTouching_AnyColliderWithFilter_Internal(ContactFilter2D contactFilter)
		{
			return this.IsTouching_AnyColliderWithFilter_Internal_Injected(ref contactFilter);
		}

		[ExcludeFromDocs]
		public bool IsTouchingLayers()
		{
			return this.IsTouchingLayers(-1);
		}

		/// <summary>
		///   <para>Checks whether any of the collider(s) attached to this rigidbody are touching any colliders on the specified layerMask or not.</para>
		/// </summary>
		/// <param name="layerMask">Any colliders on any of these layers count as touching.</param>
		/// <returns>
		///   <para>Whether any of the collider(s) attached to this rigidbody are touching any colliders on the specified layerMask or not.</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsTouchingLayers([DefaultValue("Physics2D.AllLayers")] int layerMask);

		/// <summary>
		///   <para>Check if any of the Rigidbody2D colliders overlap a point in space.</para>
		/// </summary>
		/// <param name="point">A point in world space.</param>
		/// <returns>
		///   <para>Whether the point overlapped any of the Rigidbody2D colliders.</para>
		/// </returns>
		public bool OverlapPoint(Vector2 point)
		{
			return this.OverlapPoint_Injected(ref point);
		}

		/// <summary>
		///   <para>Calculates the minimum distance of this collider against all Collider2D attached to this Rigidbody2D.</para>
		/// </summary>
		/// <param name="collider">A collider used to calculate the minimum distance against all colliders attached to this Rigidbody2D.</param>
		/// <returns>
		///   <para>The minimum distance of collider against all colliders attached to this Rigidbody2D.</para>
		/// </returns>
		public ColliderDistance2D Distance([Writable] Collider2D collider)
		{
			if (collider == null)
			{
				throw new ArgumentNullException("Collider cannot be null.");
			}
			if (collider.attachedRigidbody == this)
			{
				throw new ArgumentException("The collider cannot be attached to the Rigidbody2D being searched.");
			}
			return this.Distance_Internal(collider);
		}

		[NativeMethod("Distance")]
		private ColliderDistance2D Distance_Internal([NotNull] [Writable] Collider2D collider)
		{
			ColliderDistance2D colliderDistance2D;
			this.Distance_Internal_Injected(collider, out colliderDistance2D);
			return colliderDistance2D;
		}

		[ExcludeFromDocs]
		public void AddForce(Vector2 force)
		{
			this.AddForce(force, ForceMode2D.Force);
		}

		/// <summary>
		///   <para>Apply a force to the rigidbody.</para>
		/// </summary>
		/// <param name="force">Components of the force in the X and Y axes.</param>
		/// <param name="mode">The method used to apply the specified force.</param>
		public void AddForce(Vector2 force, [DefaultValue("ForceMode2D.Force")] ForceMode2D mode)
		{
			this.AddForce_Injected(ref force, mode);
		}

		[ExcludeFromDocs]
		public void AddRelativeForce(Vector2 relativeForce)
		{
			this.AddRelativeForce(relativeForce, ForceMode2D.Force);
		}

		/// <summary>
		///   <para>Adds a force to the rigidbody2D relative to its coordinate system.</para>
		/// </summary>
		/// <param name="relativeForce">Components of the force in the X and Y axes.</param>
		/// <param name="mode">The method used to apply the specified force.</param>
		public void AddRelativeForce(Vector2 relativeForce, [DefaultValue("ForceMode2D.Force")] ForceMode2D mode)
		{
			this.AddRelativeForce_Injected(ref relativeForce, mode);
		}

		[ExcludeFromDocs]
		public void AddForceAtPosition(Vector2 force, Vector2 position)
		{
			this.AddForceAtPosition(force, position, ForceMode2D.Force);
		}

		/// <summary>
		///   <para>Apply a force at a given position in space.</para>
		/// </summary>
		/// <param name="force">Components of the force in the X and Y axes.</param>
		/// <param name="position">Position in world space to apply the force.</param>
		/// <param name="mode">The method used to apply the specified force.</param>
		public void AddForceAtPosition(Vector2 force, Vector2 position, [DefaultValue("ForceMode2D.Force")] ForceMode2D mode)
		{
			this.AddForceAtPosition_Injected(ref force, ref position, mode);
		}

		[ExcludeFromDocs]
		public void AddTorque(float torque)
		{
			this.AddTorque(torque, ForceMode2D.Force);
		}

		/// <summary>
		///   <para>Apply a torque at the rigidbody's centre of mass.</para>
		/// </summary>
		/// <param name="torque">Torque to apply.</param>
		/// <param name="mode">The force mode to use.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void AddTorque(float torque, [DefaultValue("ForceMode2D.Force")] ForceMode2D mode);

		/// <summary>
		///   <para>Get a local space point given the point point in rigidBody global space.</para>
		/// </summary>
		/// <param name="point">The global space point to transform into local space.</param>
		public Vector2 GetPoint(Vector2 point)
		{
			Vector2 vector;
			this.GetPoint_Injected(ref point, out vector);
			return vector;
		}

		/// <summary>
		///   <para>Get a global space point given the point relativePoint in rigidBody local space.</para>
		/// </summary>
		/// <param name="relativePoint">The local space point to transform into global space.</param>
		public Vector2 GetRelativePoint(Vector2 relativePoint)
		{
			Vector2 vector;
			this.GetRelativePoint_Injected(ref relativePoint, out vector);
			return vector;
		}

		/// <summary>
		///   <para>Get a local space vector given the vector vector in rigidBody global space.</para>
		/// </summary>
		/// <param name="vector">The global space vector to transform into a local space vector.</param>
		public Vector2 GetVector(Vector2 vector)
		{
			Vector2 vector2;
			this.GetVector_Injected(ref vector, out vector2);
			return vector2;
		}

		/// <summary>
		///   <para>Get a global space vector given the vector relativeVector in rigidBody local space.</para>
		/// </summary>
		/// <param name="relativeVector">The local space vector to transform into a global space vector.</param>
		public Vector2 GetRelativeVector(Vector2 relativeVector)
		{
			Vector2 vector;
			this.GetRelativeVector_Injected(ref relativeVector, out vector);
			return vector;
		}

		/// <summary>
		///   <para>The velocity of the rigidbody at the point Point in global space.</para>
		/// </summary>
		/// <param name="point">The global space point to calculate velocity for.</param>
		public Vector2 GetPointVelocity(Vector2 point)
		{
			Vector2 vector;
			this.GetPointVelocity_Injected(ref point, out vector);
			return vector;
		}

		/// <summary>
		///   <para>The velocity of the rigidbody at the point Point in local space.</para>
		/// </summary>
		/// <param name="relativePoint">The local space point to calculate velocity for.</param>
		public Vector2 GetRelativePointVelocity(Vector2 relativePoint)
		{
			Vector2 vector;
			this.GetRelativePointVelocity_Injected(ref relativePoint, out vector);
			return vector;
		}

		/// <summary>
		///   <para>Get a list of all colliders that overlap all colliders attached to this Rigidbody2D.</para>
		/// </summary>
		/// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth.  Note that normal angle is not used for overlap testing.</param>
		/// <param name="results">The array to receive results.  The size of the array determines the maximum number of results that can be returned.</param>
		/// <returns>
		///   <para>Returns the number of results placed in the results array.</para>
		/// </returns>
		[NativeMethod("OverlapCollider_Binding")]
		public int OverlapCollider(ContactFilter2D contactFilter, [Out] Collider2D[] results)
		{
			return this.OverlapCollider_Injected(ref contactFilter, results);
		}

		/// <summary>
		///   <para>Retrieves all contact points for all of the collider(s) attached to this rigidbody.</para>
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
		///   <para>Retrieves all contact points for all of the collider(s) attached to this rigidbody, with the results filtered by the ContactFilter2D.</para>
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
		///   <para>Retrieves all colliders in contact with any of the collider(s) attached to this rigidbody.</para>
		/// </summary>
		/// <param name="colliders">An array of Collider2D used to receive the results.</param>
		/// <returns>
		///   <para>Returns the number of colliders placed in the colliders array.</para>
		/// </returns>
		public int GetContacts(Collider2D[] colliders)
		{
			return Physics2D.GetContacts(this, default(ContactFilter2D).NoFilter(), colliders);
		}

		/// <summary>
		///   <para>Retrieves all colliders in contact with any of the collider(s) attached to this rigidbody, with the results filtered by the ContactFilter2D.</para>
		/// </summary>
		/// <param name="contactFilter">The contact filter used to filter the results differently, such as by layer mask, Z depth, or normal angle.</param>
		/// <param name="colliders">An array of Collider2D used to receive the results.</param>
		/// <returns>
		///   <para>Returns the number of colliders placed in the colliders array.</para>
		/// </returns>
		public int GetContacts(ContactFilter2D contactFilter, Collider2D[] colliders)
		{
			return Physics2D.GetContacts(this, contactFilter, colliders);
		}

		/// <summary>
		///   <para>Returns all Collider2D that are attached to this Rigidbody2D.</para>
		/// </summary>
		/// <param name="results">An array of Collider2D used to receive the results.</param>
		/// <returns>
		///   <para>Returns the number of Collider2D placed in the results array.</para>
		/// </returns>
		[NativeMethod("GetAttachedColliders_Binding")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetAttachedColliders([Out] Collider2D[] results);

		[ExcludeFromDocs]
		public int Cast(Vector2 direction, RaycastHit2D[] results)
		{
			return this.Cast_Internal(direction, float.PositiveInfinity, results);
		}

		/// <summary>
		///   <para>All the Collider2D shapes attached to the Rigidbody2D are cast into the scene starting at each collider position ignoring the colliders attached to the same Rigidbody2D.</para>
		/// </summary>
		/// <param name="direction">Vector representing the direction to cast each Collider2D shape.</param>
		/// <param name="results">Array to receive results.</param>
		/// <param name="distance">Maximum distance over which to cast the shape(s).</param>
		/// <returns>
		///   <para>The number of results returned.</para>
		/// </returns>
		public int Cast(Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance)
		{
			return this.Cast_Internal(direction, distance, results);
		}

		[ExcludeFromDocs]
		public int Cast(Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results)
		{
			return this.CastFiltered_Internal(direction, float.PositiveInfinity, contactFilter, results);
		}

		/// <summary>
		///   <para>All the Collider2D shapes attached to the Rigidbody2D are cast into the scene starting at each collider position ignoring the colliders attached to the same Rigidbody2D.</para>
		/// </summary>
		/// <param name="direction">Vector representing the direction to cast each Collider2D shape.</param>
		/// <param name="contactFilter">Filter results defined by the contact filter.</param>
		/// <param name="results">Array to receive results.</param>
		/// <param name="distance">Maximum distance over which to cast the shape(s).</param>
		/// <returns>
		///   <para>The number of results returned.</para>
		/// </returns>
		public int Cast(Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance)
		{
			return this.CastFiltered_Internal(direction, distance, contactFilter, results);
		}

		[NativeMethod("Cast_Binding")]
		private int Cast_Internal(Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [Out] RaycastHit2D[] results)
		{
			return this.Cast_Internal_Injected(ref direction, distance, results);
		}

		[NativeMethod("CastFiltered_Binding")]
		private int CastFiltered_Internal(Vector2 direction, float distance, ContactFilter2D contactFilter, [Out] RaycastHit2D[] results)
		{
			return this.CastFiltered_Internal_Injected(ref direction, distance, ref contactFilter, results);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_position_Injected(out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_position_Injected(ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void MovePosition_Injected(ref Vector2 position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_velocity_Injected(out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_velocity_Injected(ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_centerOfMass_Injected(out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_centerOfMass_Injected(ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_worldCenterOfMass_Injected(out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool IsTouching_OtherColliderWithFilter_Internal_Injected([Writable] Collider2D collider, ref ContactFilter2D contactFilter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool IsTouching_AnyColliderWithFilter_Internal_Injected(ref ContactFilter2D contactFilter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool OverlapPoint_Injected(ref Vector2 point);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Distance_Internal_Injected([Writable] Collider2D collider, out ColliderDistance2D ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void AddForce_Injected(ref Vector2 force, [DefaultValue("ForceMode2D.Force")] ForceMode2D mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void AddRelativeForce_Injected(ref Vector2 relativeForce, [DefaultValue("ForceMode2D.Force")] ForceMode2D mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void AddForceAtPosition_Injected(ref Vector2 force, ref Vector2 position, [DefaultValue("ForceMode2D.Force")] ForceMode2D mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetPoint_Injected(ref Vector2 point, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetRelativePoint_Injected(ref Vector2 relativePoint, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetVector_Injected(ref Vector2 vector, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetRelativeVector_Injected(ref Vector2 relativeVector, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetPointVelocity_Injected(ref Vector2 point, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetRelativePointVelocity_Injected(ref Vector2 relativePoint, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int OverlapCollider_Injected(ref ContactFilter2D contactFilter, [Out] Collider2D[] results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int Cast_Internal_Injected(ref Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [Out] RaycastHit2D[] results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int CastFiltered_Internal_Injected(ref Vector2 direction, float distance, ref ContactFilter2D contactFilter, [Out] RaycastHit2D[] results);
	}
}
