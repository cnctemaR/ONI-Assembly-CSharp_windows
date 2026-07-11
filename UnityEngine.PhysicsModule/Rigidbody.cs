using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	/// <summary>
	///   <para>Control of an object's position through physics simulation.</para>
	/// </summary>
	[RequireComponent(typeof(Transform))]
	[NativeHeader("Runtime/Dynamics/Rigidbody.h")]
	public class Rigidbody : Component
	{
		/// <summary>
		///   <para>The velocity vector of the rigidbody.</para>
		/// </summary>
		public Vector3 velocity
		{
			get
			{
				Vector3 vector;
				this.get_velocity_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_velocity_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>The angular velocity vector of the rigidbody measured in radians per second.</para>
		/// </summary>
		public Vector3 angularVelocity
		{
			get
			{
				Vector3 vector;
				this.get_angularVelocity_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_angularVelocity_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>The drag of the object.</para>
		/// </summary>
		public extern float drag
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The angular drag of the object.</para>
		/// </summary>
		public extern float angularDrag
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The mass of the rigidbody.</para>
		/// </summary>
		public extern float mass
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Sets the mass based on the attached colliders assuming a constant density.</para>
		/// </summary>
		/// <param name="density"></param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetDensity(float density);

		/// <summary>
		///   <para>Controls whether gravity affects this rigidbody.</para>
		/// </summary>
		public extern bool useGravity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Maximum velocity of a rigidbody when moving out of penetrating state.</para>
		/// </summary>
		public extern float maxDepenetrationVelocity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Controls whether physics affects the rigidbody.</para>
		/// </summary>
		public extern bool isKinematic
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
		///   <para>Controls which degrees of freedom are allowed for the simulation of this Rigidbody.</para>
		/// </summary>
		public extern RigidbodyConstraints constraints
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The Rigidbody's collision detection mode.</para>
		/// </summary>
		public extern CollisionDetectionMode collisionDetectionMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The center of mass relative to the transform's origin.</para>
		/// </summary>
		public Vector3 centerOfMass
		{
			get
			{
				Vector3 vector;
				this.get_centerOfMass_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_centerOfMass_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>The center of mass of the rigidbody in world space (Read Only).</para>
		/// </summary>
		public Vector3 worldCenterOfMass
		{
			get
			{
				Vector3 vector;
				this.get_worldCenterOfMass_Injected(out vector);
				return vector;
			}
		}

		/// <summary>
		///   <para>The rotation of the inertia tensor.</para>
		/// </summary>
		public Quaternion inertiaTensorRotation
		{
			get
			{
				Quaternion quaternion;
				this.get_inertiaTensorRotation_Injected(out quaternion);
				return quaternion;
			}
			set
			{
				this.set_inertiaTensorRotation_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>The diagonal inertia tensor of mass relative to the center of mass.</para>
		/// </summary>
		public Vector3 inertiaTensor
		{
			get
			{
				Vector3 vector;
				this.get_inertiaTensor_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_inertiaTensor_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Should collision detection be enabled? (By default always enabled).</para>
		/// </summary>
		public extern bool detectCollisions
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The position of the rigidbody.</para>
		/// </summary>
		public Vector3 position
		{
			get
			{
				Vector3 vector;
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
		public Quaternion rotation
		{
			get
			{
				Quaternion quaternion;
				this.get_rotation_Injected(out quaternion);
				return quaternion;
			}
			set
			{
				this.set_rotation_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Interpolation allows you to smooth out the effect of running physics at a fixed frame rate.</para>
		/// </summary>
		public extern RigidbodyInterpolation interpolation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The solverIterations determines how accurately Rigidbody joints and collision contacts are resolved. Overrides Physics.defaultSolverIterations. Must be positive.</para>
		/// </summary>
		public extern int solverIterations
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The mass-normalized energy threshold, below which objects start going to sleep.</para>
		/// </summary>
		public extern float sleepThreshold
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The maximimum angular velocity of the rigidbody. (Default 7) range { 0, infinity }.</para>
		/// </summary>
		public extern float maxAngularVelocity
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
		public void MovePosition(Vector3 position)
		{
			this.MovePosition_Injected(ref position);
		}

		/// <summary>
		///   <para>Rotates the rigidbody to rotation.</para>
		/// </summary>
		/// <param name="rot">The new rotation for the Rigidbody.</param>
		public void MoveRotation(Quaternion rot)
		{
			this.MoveRotation_Injected(ref rot);
		}

		/// <summary>
		///   <para>Forces a rigidbody to sleep at least one frame.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Sleep();

		/// <summary>
		///   <para>Is the rigidbody sleeping?</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsSleeping();

		/// <summary>
		///   <para>Forces a rigidbody to wake up.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void WakeUp();

		/// <summary>
		///   <para>Reset the center of mass of the rigidbody.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ResetCenterOfMass();

		/// <summary>
		///   <para>Reset the inertia tensor value and rotation.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ResetInertiaTensor();

		/// <summary>
		///   <para>The velocity relative to the rigidbody at the point relativePoint.</para>
		/// </summary>
		/// <param name="relativePoint"></param>
		public Vector3 GetRelativePointVelocity(Vector3 relativePoint)
		{
			Vector3 vector;
			this.GetRelativePointVelocity_Injected(ref relativePoint, out vector);
			return vector;
		}

		/// <summary>
		///   <para>The velocity of the rigidbody at the point worldPoint in global space.</para>
		/// </summary>
		/// <param name="worldPoint"></param>
		public Vector3 GetPointVelocity(Vector3 worldPoint)
		{
			Vector3 vector;
			this.GetPointVelocity_Injected(ref worldPoint, out vector);
			return vector;
		}

		/// <summary>
		///   <para>The solverVelocityIterations affects how how accurately Rigidbody joints and collision contacts are resolved. Overrides Physics.defaultSolverVelocityIterations. Must be positive.</para>
		/// </summary>
		public extern int solverVelocityIterations
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The linear velocity below which objects start going to sleep. (Default 0.14) range { 0, infinity }.</para>
		/// </summary>
		[Obsolete("The sleepVelocity is no longer supported. Use sleepThreshold. Note that sleepThreshold is energy but not velocity.")]
		public extern float sleepVelocity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The angular velocity below which objects start going to sleep.  (Default 0.14) range { 0, infinity }.</para>
		/// </summary>
		[Obsolete("The sleepAngularVelocity is no longer supported. Set Use sleepThreshold to specify energy.")]
		public extern float sleepAngularVelocity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Use Rigidbody.maxAngularVelocity instead.")]
		public void SetMaxAngularVelocity(float a)
		{
			this.maxAngularVelocity = a;
		}

		/// <summary>
		///   <para>Force cone friction to be used for this rigidbody.</para>
		/// </summary>
		[Obsolete("Cone friction is no longer supported.")]
		public bool useConeFriction
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		[Obsolete("Please use Rigidbody.solverIterations instead. (UnityUpgradable) -> solverIterations")]
		public int solverIterationCount
		{
			get
			{
				return this.solverIterations;
			}
			set
			{
				this.solverIterations = value;
			}
		}

		[Obsolete("Please use Rigidbody.solverVelocityIterations instead. (UnityUpgradable) -> solverVelocityIterations")]
		public int solverVelocityIterationCount
		{
			get
			{
				return this.solverVelocityIterations;
			}
			set
			{
				this.solverVelocityIterations = value;
			}
		}

		/// <summary>
		///   <para>Adds a force to the Rigidbody.</para>
		/// </summary>
		/// <param name="force">Force vector in world coordinates.</param>
		/// <param name="mode">Type of force to apply.</param>
		public void AddForce(Vector3 force, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			this.AddForce_Injected(ref force, mode);
		}

		/// <summary>
		///   <para>Adds a force to the Rigidbody.</para>
		/// </summary>
		/// <param name="force">Force vector in world coordinates.</param>
		/// <param name="mode">Type of force to apply.</param>
		[ExcludeFromDocs]
		public void AddForce(Vector3 force)
		{
			this.AddForce(force, ForceMode.Force);
		}

		/// <summary>
		///   <para>Adds a force to the Rigidbody.</para>
		/// </summary>
		/// <param name="x">Size of force along the world x-axis.</param>
		/// <param name="y">Size of force along the world y-axis.</param>
		/// <param name="z">Size of force along the world z-axis.</param>
		/// <param name="mode">Type of force to apply.</param>
		public void AddForce(float x, float y, float z, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			this.AddForce(new Vector3(x, y, z), mode);
		}

		/// <summary>
		///   <para>Adds a force to the Rigidbody.</para>
		/// </summary>
		/// <param name="x">Size of force along the world x-axis.</param>
		/// <param name="y">Size of force along the world y-axis.</param>
		/// <param name="z">Size of force along the world z-axis.</param>
		/// <param name="mode">Type of force to apply.</param>
		[ExcludeFromDocs]
		public void AddForce(float x, float y, float z)
		{
			this.AddForce(new Vector3(x, y, z), ForceMode.Force);
		}

		/// <summary>
		///   <para>Adds a force to the rigidbody relative to its coordinate system.</para>
		/// </summary>
		/// <param name="force">Force vector in local coordinates.</param>
		/// <param name="mode"></param>
		public void AddRelativeForce(Vector3 force, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			this.AddRelativeForce_Injected(ref force, mode);
		}

		/// <summary>
		///   <para>Adds a force to the rigidbody relative to its coordinate system.</para>
		/// </summary>
		/// <param name="force">Force vector in local coordinates.</param>
		/// <param name="mode"></param>
		[ExcludeFromDocs]
		public void AddRelativeForce(Vector3 force)
		{
			this.AddRelativeForce(force, ForceMode.Force);
		}

		/// <summary>
		///   <para>Adds a force to the rigidbody relative to its coordinate system.</para>
		/// </summary>
		/// <param name="x">Size of force along the local x-axis.</param>
		/// <param name="y">Size of force along the local y-axis.</param>
		/// <param name="z">Size of force along the local z-axis.</param>
		/// <param name="mode"></param>
		public void AddRelativeForce(float x, float y, float z, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			this.AddRelativeForce(new Vector3(x, y, z), mode);
		}

		/// <summary>
		///   <para>Adds a force to the rigidbody relative to its coordinate system.</para>
		/// </summary>
		/// <param name="x">Size of force along the local x-axis.</param>
		/// <param name="y">Size of force along the local y-axis.</param>
		/// <param name="z">Size of force along the local z-axis.</param>
		/// <param name="mode"></param>
		[ExcludeFromDocs]
		public void AddRelativeForce(float x, float y, float z)
		{
			this.AddRelativeForce(new Vector3(x, y, z), ForceMode.Force);
		}

		/// <summary>
		///   <para>Adds a torque to the rigidbody.</para>
		/// </summary>
		/// <param name="torque">Torque vector in world coordinates.</param>
		/// <param name="mode"></param>
		public void AddTorque(Vector3 torque, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			this.AddTorque_Injected(ref torque, mode);
		}

		/// <summary>
		///   <para>Adds a torque to the rigidbody.</para>
		/// </summary>
		/// <param name="torque">Torque vector in world coordinates.</param>
		/// <param name="mode"></param>
		[ExcludeFromDocs]
		public void AddTorque(Vector3 torque)
		{
			this.AddTorque(torque, ForceMode.Force);
		}

		/// <summary>
		///   <para>Adds a torque to the rigidbody.</para>
		/// </summary>
		/// <param name="x">Size of torque along the world x-axis.</param>
		/// <param name="y">Size of torque along the world y-axis.</param>
		/// <param name="z">Size of torque along the world z-axis.</param>
		/// <param name="mode"></param>
		public void AddTorque(float x, float y, float z, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			this.AddTorque(new Vector3(x, y, z), mode);
		}

		/// <summary>
		///   <para>Adds a torque to the rigidbody.</para>
		/// </summary>
		/// <param name="x">Size of torque along the world x-axis.</param>
		/// <param name="y">Size of torque along the world y-axis.</param>
		/// <param name="z">Size of torque along the world z-axis.</param>
		/// <param name="mode"></param>
		[ExcludeFromDocs]
		public void AddTorque(float x, float y, float z)
		{
			this.AddTorque(new Vector3(x, y, z), ForceMode.Force);
		}

		/// <summary>
		///   <para>Adds a torque to the rigidbody relative to its coordinate system.</para>
		/// </summary>
		/// <param name="torque">Torque vector in local coordinates.</param>
		/// <param name="mode"></param>
		public void AddRelativeTorque(Vector3 torque, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			this.AddRelativeTorque_Injected(ref torque, mode);
		}

		/// <summary>
		///   <para>Adds a torque to the rigidbody relative to its coordinate system.</para>
		/// </summary>
		/// <param name="torque">Torque vector in local coordinates.</param>
		/// <param name="mode"></param>
		[ExcludeFromDocs]
		public void AddRelativeTorque(Vector3 torque)
		{
			this.AddRelativeTorque(torque, ForceMode.Force);
		}

		/// <summary>
		///   <para>Adds a torque to the rigidbody relative to its coordinate system.</para>
		/// </summary>
		/// <param name="x">Size of torque along the local x-axis.</param>
		/// <param name="y">Size of torque along the local y-axis.</param>
		/// <param name="z">Size of torque along the local z-axis.</param>
		/// <param name="mode"></param>
		public void AddRelativeTorque(float x, float y, float z, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			this.AddRelativeTorque(new Vector3(x, y, z), mode);
		}

		/// <summary>
		///   <para>Adds a torque to the rigidbody relative to its coordinate system.</para>
		/// </summary>
		/// <param name="x">Size of torque along the local x-axis.</param>
		/// <param name="y">Size of torque along the local y-axis.</param>
		/// <param name="z">Size of torque along the local z-axis.</param>
		/// <param name="mode"></param>
		[ExcludeFromDocs]
		public void AddRelativeTorque(float x, float y, float z)
		{
			this.AddRelativeTorque(x, y, z, ForceMode.Force);
		}

		/// <summary>
		///   <para>Applies force at position. As a result this will apply a torque and force on the object.</para>
		/// </summary>
		/// <param name="force">Force vector in world coordinates.</param>
		/// <param name="position">Position in world coordinates.</param>
		/// <param name="mode"></param>
		public void AddForceAtPosition(Vector3 force, Vector3 position, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			this.AddForceAtPosition_Injected(ref force, ref position, mode);
		}

		/// <summary>
		///   <para>Applies force at position. As a result this will apply a torque and force on the object.</para>
		/// </summary>
		/// <param name="force">Force vector in world coordinates.</param>
		/// <param name="position">Position in world coordinates.</param>
		/// <param name="mode"></param>
		[ExcludeFromDocs]
		public void AddForceAtPosition(Vector3 force, Vector3 position)
		{
			this.AddForceAtPosition(force, position, ForceMode.Force);
		}

		/// <summary>
		///   <para>Applies a force to a rigidbody that simulates explosion effects.</para>
		/// </summary>
		/// <param name="explosionForce">The force of the explosion (which may be modified by distance).</param>
		/// <param name="explosionPosition">The centre of the sphere within which the explosion has its effect.</param>
		/// <param name="explosionRadius">The radius of the sphere within which the explosion has its effect.</param>
		/// <param name="upwardsModifier">Adjustment to the apparent position of the explosion to make it seem to lift objects.</param>
		/// <param name="mode">The method used to apply the force to its targets.</param>
		public void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius, [DefaultValue("0.0f")] float upwardsModifier, [DefaultValue("ForceMode.Force)")] ForceMode mode)
		{
			this.AddExplosionForce_Injected(explosionForce, ref explosionPosition, explosionRadius, upwardsModifier, mode);
		}

		/// <summary>
		///   <para>Applies a force to a rigidbody that simulates explosion effects.</para>
		/// </summary>
		/// <param name="explosionForce">The force of the explosion (which may be modified by distance).</param>
		/// <param name="explosionPosition">The centre of the sphere within which the explosion has its effect.</param>
		/// <param name="explosionRadius">The radius of the sphere within which the explosion has its effect.</param>
		/// <param name="upwardsModifier">Adjustment to the apparent position of the explosion to make it seem to lift objects.</param>
		/// <param name="mode">The method used to apply the force to its targets.</param>
		[ExcludeFromDocs]
		public void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius, float upwardsModifier)
		{
			this.AddExplosionForce(explosionForce, explosionPosition, explosionRadius, upwardsModifier, ForceMode.Force);
		}

		/// <summary>
		///   <para>Applies a force to a rigidbody that simulates explosion effects.</para>
		/// </summary>
		/// <param name="explosionForce">The force of the explosion (which may be modified by distance).</param>
		/// <param name="explosionPosition">The centre of the sphere within which the explosion has its effect.</param>
		/// <param name="explosionRadius">The radius of the sphere within which the explosion has its effect.</param>
		/// <param name="upwardsModifier">Adjustment to the apparent position of the explosion to make it seem to lift objects.</param>
		/// <param name="mode">The method used to apply the force to its targets.</param>
		[ExcludeFromDocs]
		public void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius)
		{
			this.AddExplosionForce(explosionForce, explosionPosition, explosionRadius, 0f, ForceMode.Force);
		}

		[NativeName("ClosestPointOnBounds")]
		private void Internal_ClosestPointOnBounds(Vector3 point, ref Vector3 outPos, ref float distance)
		{
			this.Internal_ClosestPointOnBounds_Injected(ref point, ref outPos, ref distance);
		}

		/// <summary>
		///   <para>The closest point to the bounding box of the attached colliders.</para>
		/// </summary>
		/// <param name="position"></param>
		public Vector3 ClosestPointOnBounds(Vector3 position)
		{
			float num = 0f;
			Vector3 zero = Vector3.zero;
			this.Internal_ClosestPointOnBounds(position, ref zero, ref num);
			return zero;
		}

		private RaycastHit SweepTest(Vector3 direction, float maxDistance, QueryTriggerInteraction queryTriggerInteraction, ref bool hasHit)
		{
			RaycastHit raycastHit;
			this.SweepTest_Injected(ref direction, maxDistance, queryTriggerInteraction, ref hasHit, out raycastHit);
			return raycastHit;
		}

		public bool SweepTest(Vector3 direction, out RaycastHit hitInfo, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			float magnitude = direction.magnitude;
			bool flag2;
			if (magnitude > 1E-45f)
			{
				Vector3 vector = direction / magnitude;
				bool flag = false;
				hitInfo = this.SweepTest(vector, maxDistance, queryTriggerInteraction, ref flag);
				flag2 = flag;
			}
			else
			{
				hitInfo = default(RaycastHit);
				flag2 = false;
			}
			return flag2;
		}

		[ExcludeFromDocs]
		public bool SweepTest(Vector3 direction, out RaycastHit hitInfo, float maxDistance)
		{
			return this.SweepTest(direction, out hitInfo, maxDistance, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public bool SweepTest(Vector3 direction, out RaycastHit hitInfo)
		{
			return this.SweepTest(direction, out hitInfo, float.PositiveInfinity, QueryTriggerInteraction.UseGlobal);
		}

		[NativeName("SweepTestAll")]
		private RaycastHit[] Internal_SweepTestAll(Vector3 direction, float maxDistance, QueryTriggerInteraction queryTriggerInteraction)
		{
			return this.Internal_SweepTestAll_Injected(ref direction, maxDistance, queryTriggerInteraction);
		}

		/// <summary>
		///   <para>Like Rigidbody.SweepTest, but returns all hits.</para>
		/// </summary>
		/// <param name="direction">The direction into which to sweep the rigidbody.</param>
		/// <param name="maxDistance">The length of the sweep.</param>
		/// <param name="queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
		/// <returns>
		///   <para>An array of all colliders hit in the sweep.</para>
		/// </returns>
		public RaycastHit[] SweepTestAll(Vector3 direction, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			float magnitude = direction.magnitude;
			RaycastHit[] array;
			if (magnitude > 1E-45f)
			{
				Vector3 vector = direction / magnitude;
				array = this.Internal_SweepTestAll(vector, maxDistance, queryTriggerInteraction);
			}
			else
			{
				array = new RaycastHit[0];
			}
			return array;
		}

		[ExcludeFromDocs]
		public RaycastHit[] SweepTestAll(Vector3 direction, float maxDistance)
		{
			return this.SweepTestAll(direction, maxDistance, QueryTriggerInteraction.UseGlobal);
		}

		[ExcludeFromDocs]
		public RaycastHit[] SweepTestAll(Vector3 direction)
		{
			return this.SweepTestAll(direction, float.PositiveInfinity, QueryTriggerInteraction.UseGlobal);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_velocity_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_velocity_Injected(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_angularVelocity_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_angularVelocity_Injected(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_centerOfMass_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_centerOfMass_Injected(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_worldCenterOfMass_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_inertiaTensorRotation_Injected(out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_inertiaTensorRotation_Injected(ref Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_inertiaTensor_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_inertiaTensor_Injected(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_position_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_position_Injected(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_rotation_Injected(out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_rotation_Injected(ref Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void MovePosition_Injected(ref Vector3 position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void MoveRotation_Injected(ref Quaternion rot);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetRelativePointVelocity_Injected(ref Vector3 relativePoint, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetPointVelocity_Injected(ref Vector3 worldPoint, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void AddForce_Injected(ref Vector3 force, [DefaultValue("ForceMode.Force")] ForceMode mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void AddRelativeForce_Injected(ref Vector3 force, [DefaultValue("ForceMode.Force")] ForceMode mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void AddTorque_Injected(ref Vector3 torque, [DefaultValue("ForceMode.Force")] ForceMode mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void AddRelativeTorque_Injected(ref Vector3 torque, [DefaultValue("ForceMode.Force")] ForceMode mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void AddForceAtPosition_Injected(ref Vector3 force, ref Vector3 position, [DefaultValue("ForceMode.Force")] ForceMode mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void AddExplosionForce_Injected(float explosionForce, ref Vector3 explosionPosition, float explosionRadius, [DefaultValue("0.0f")] float upwardsModifier, [DefaultValue("ForceMode.Force)")] ForceMode mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_ClosestPointOnBounds_Injected(ref Vector3 point, ref Vector3 outPos, ref float distance);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SweepTest_Injected(ref Vector3 direction, float maxDistance, QueryTriggerInteraction queryTriggerInteraction, ref bool hasHit, out RaycastHit ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern RaycastHit[] Internal_SweepTestAll_Injected(ref Vector3 direction, float maxDistance, QueryTriggerInteraction queryTriggerInteraction);
	}
}
