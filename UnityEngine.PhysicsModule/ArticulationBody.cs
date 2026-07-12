using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeHeader("Modules/Physics/ArticulationBody.h")]
	[NativeClass("Unity::ArticulationBody")]
	public class ArticulationBody : Behaviour
	{
		public extern ArticulationJointType jointType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Vector3 anchorPosition
		{
			get
			{
				Vector3 vector;
				this.get_anchorPosition_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_anchorPosition_Injected(ref value);
			}
		}

		public Vector3 parentAnchorPosition
		{
			get
			{
				Vector3 vector;
				this.get_parentAnchorPosition_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_parentAnchorPosition_Injected(ref value);
			}
		}

		public Quaternion anchorRotation
		{
			get
			{
				Quaternion quaternion;
				this.get_anchorRotation_Injected(out quaternion);
				return quaternion;
			}
			set
			{
				this.set_anchorRotation_Injected(ref value);
			}
		}

		public Quaternion parentAnchorRotation
		{
			get
			{
				Quaternion quaternion;
				this.get_parentAnchorRotation_Injected(out quaternion);
				return quaternion;
			}
			set
			{
				this.set_parentAnchorRotation_Injected(ref value);
			}
		}

		public extern bool isRoot
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool matchAnchors
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ArticulationDofLock linearLockX
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ArticulationDofLock linearLockY
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ArticulationDofLock linearLockZ
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ArticulationDofLock swingYLock
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ArticulationDofLock swingZLock
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ArticulationDofLock twistLock
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public ArticulationDrive xDrive
		{
			get
			{
				ArticulationDrive articulationDrive;
				this.get_xDrive_Injected(out articulationDrive);
				return articulationDrive;
			}
			set
			{
				this.set_xDrive_Injected(ref value);
			}
		}

		public ArticulationDrive yDrive
		{
			get
			{
				ArticulationDrive articulationDrive;
				this.get_yDrive_Injected(out articulationDrive);
				return articulationDrive;
			}
			set
			{
				this.set_yDrive_Injected(ref value);
			}
		}

		public ArticulationDrive zDrive
		{
			get
			{
				ArticulationDrive articulationDrive;
				this.get_zDrive_Injected(out articulationDrive);
				return articulationDrive;
			}
			set
			{
				this.set_zDrive_Injected(ref value);
			}
		}

		public extern bool immovable
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool useGravity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float linearDamping
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float angularDamping
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float jointFriction
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public LayerMask excludeLayers
		{
			get
			{
				LayerMask layerMask;
				this.get_excludeLayers_Injected(out layerMask);
				return layerMask;
			}
			set
			{
				this.set_excludeLayers_Injected(ref value);
			}
		}

		public LayerMask includeLayers
		{
			get
			{
				LayerMask layerMask;
				this.get_includeLayers_Injected(out layerMask);
				return layerMask;
			}
			set
			{
				this.set_includeLayers_Injected(ref value);
			}
		}

		public Vector3 GetAccumulatedForce([DefaultValue("Time.fixedDeltaTime")] float step)
		{
			Vector3 vector;
			this.GetAccumulatedForce_Injected(step, out vector);
			return vector;
		}

		[ExcludeFromDocs]
		public Vector3 GetAccumulatedForce()
		{
			return this.GetAccumulatedForce(Time.fixedDeltaTime);
		}

		public Vector3 GetAccumulatedTorque([DefaultValue("Time.fixedDeltaTime")] float step)
		{
			Vector3 vector;
			this.GetAccumulatedTorque_Injected(step, out vector);
			return vector;
		}

		[ExcludeFromDocs]
		public Vector3 GetAccumulatedTorque()
		{
			return this.GetAccumulatedTorque(Time.fixedDeltaTime);
		}

		public void AddForce(Vector3 force, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			this.AddForce_Injected(ref force, mode);
		}

		[ExcludeFromDocs]
		public void AddForce(Vector3 force)
		{
			this.AddForce(force, ForceMode.Force);
		}

		public void AddRelativeForce(Vector3 force, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			this.AddRelativeForce_Injected(ref force, mode);
		}

		[ExcludeFromDocs]
		public void AddRelativeForce(Vector3 force)
		{
			this.AddRelativeForce(force, ForceMode.Force);
		}

		public void AddTorque(Vector3 torque, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			this.AddTorque_Injected(ref torque, mode);
		}

		[ExcludeFromDocs]
		public void AddTorque(Vector3 torque)
		{
			this.AddTorque(torque, ForceMode.Force);
		}

		public void AddRelativeTorque(Vector3 torque, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			this.AddRelativeTorque_Injected(ref torque, mode);
		}

		[ExcludeFromDocs]
		public void AddRelativeTorque(Vector3 torque)
		{
			this.AddRelativeTorque(torque, ForceMode.Force);
		}

		public void AddForceAtPosition(Vector3 force, Vector3 position, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			this.AddForceAtPosition_Injected(ref force, ref position, mode);
		}

		[ExcludeFromDocs]
		public void AddForceAtPosition(Vector3 force, Vector3 position)
		{
			this.AddForceAtPosition(force, position, ForceMode.Force);
		}

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

		public extern float mass
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool automaticCenterOfMass
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

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

		public Vector3 worldCenterOfMass
		{
			get
			{
				Vector3 vector;
				this.get_worldCenterOfMass_Injected(out vector);
				return vector;
			}
		}

		public extern bool automaticInertiaTensor
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

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

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ResetCenterOfMass();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ResetInertiaTensor();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Sleep();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsSleeping();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void WakeUp();

		public extern float sleepThreshold
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int solverIterations
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int solverVelocityIterations
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float maxAngularVelocity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float maxLinearVelocity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float maxJointVelocity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float maxDepenetrationVelocity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public ArticulationReducedSpace jointPosition
		{
			get
			{
				ArticulationReducedSpace articulationReducedSpace;
				this.get_jointPosition_Injected(out articulationReducedSpace);
				return articulationReducedSpace;
			}
			set
			{
				this.set_jointPosition_Injected(ref value);
			}
		}

		public ArticulationReducedSpace jointVelocity
		{
			get
			{
				ArticulationReducedSpace articulationReducedSpace;
				this.get_jointVelocity_Injected(out articulationReducedSpace);
				return articulationReducedSpace;
			}
			set
			{
				this.set_jointVelocity_Injected(ref value);
			}
		}

		public ArticulationReducedSpace jointAcceleration
		{
			get
			{
				ArticulationReducedSpace articulationReducedSpace;
				this.get_jointAcceleration_Injected(out articulationReducedSpace);
				return articulationReducedSpace;
			}
			[Obsolete("Setting joint accelerations is not supported in forward kinematics. To have inverse dynamics take acceleration into account, use GetJointForcesForAcceleration instead", true)]
			set
			{
				this.set_jointAcceleration_Injected(ref value);
			}
		}

		public ArticulationReducedSpace jointForce
		{
			get
			{
				ArticulationReducedSpace articulationReducedSpace;
				this.get_jointForce_Injected(out articulationReducedSpace);
				return articulationReducedSpace;
			}
			set
			{
				this.set_jointForce_Injected(ref value);
			}
		}

		public ArticulationReducedSpace driveForce
		{
			get
			{
				ArticulationReducedSpace articulationReducedSpace;
				this.get_driveForce_Injected(out articulationReducedSpace);
				return articulationReducedSpace;
			}
		}

		public extern int dofCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int index
		{
			[NativeMethod("GetBodyIndex")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public void TeleportRoot(Vector3 position, Quaternion rotation)
		{
			this.TeleportRoot_Injected(ref position, ref rotation);
		}

		public Vector3 GetClosestPoint(Vector3 point)
		{
			Vector3 vector;
			this.GetClosestPoint_Injected(ref point, out vector);
			return vector;
		}

		public Vector3 GetRelativePointVelocity(Vector3 relativePoint)
		{
			Vector3 vector;
			this.GetRelativePointVelocity_Injected(ref relativePoint, out vector);
			return vector;
		}

		public Vector3 GetPointVelocity(Vector3 worldPoint)
		{
			Vector3 vector;
			this.GetPointVelocity_Injected(ref worldPoint, out vector);
			return vector;
		}

		[NativeMethod("GetDenseJacobian")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetDenseJacobian_Internal(ref ArticulationJacobian jacobian);

		public int GetDenseJacobian(ref ArticulationJacobian jacobian)
		{
			bool flag = jacobian.elements == null;
			if (flag)
			{
				jacobian.elements = new List<float>();
			}
			return this.GetDenseJacobian_Internal(ref jacobian);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetJointPositions(List<float> positions);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetJointPositions(List<float> positions);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetJointVelocities(List<float> velocities);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetJointVelocities(List<float> velocities);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetJointAccelerations(List<float> accelerations);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetJointForces(List<float> forces);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetJointForces(List<float> forces);

		public ArticulationReducedSpace GetJointForcesForAcceleration(ArticulationReducedSpace acceleration)
		{
			ArticulationReducedSpace articulationReducedSpace;
			this.GetJointForcesForAcceleration_Injected(ref acceleration, out articulationReducedSpace);
			return articulationReducedSpace;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetDriveForces(List<float> forces);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetJointGravityForces(List<float> forces);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetJointCoriolisCentrifugalForces(List<float> forces);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetJointExternalForces(List<float> forces, float step);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetDriveTargets(List<float> targets);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetDriveTargets(List<float> targets);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetDriveTargetVelocities(List<float> targetVelocities);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetDriveTargetVelocities(List<float> targetVelocities);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetDofStartIndices(List<int> dofStartIndices);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetDriveTarget(ArticulationDriveAxis axis, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetDriveTargetVelocity(ArticulationDriveAxis axis, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetDriveLimits(ArticulationDriveAxis axis, float lower, float upper);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetDriveStiffness(ArticulationDriveAxis axis, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetDriveDamping(ArticulationDriveAxis axis, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetDriveForceLimit(ArticulationDriveAxis axis, float value);

		public extern CollisionDetectionMode collisionDetectionMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public void SnapAnchorToClosestContact()
		{
			bool flag = !base.transform.parent;
			if (!flag)
			{
				ArticulationBody articulationBody = base.transform.parent.GetComponentInParent<ArticulationBody>();
				while (articulationBody && !articulationBody.enabled)
				{
					articulationBody = articulationBody.transform.parent.GetComponentInParent<ArticulationBody>();
				}
				bool flag2 = !articulationBody;
				if (!flag2)
				{
					Vector3 worldCenterOfMass = articulationBody.worldCenterOfMass;
					Vector3 closestPoint = this.GetClosestPoint(worldCenterOfMass);
					this.anchorPosition = base.transform.InverseTransformPoint(closestPoint);
					this.anchorRotation = Quaternion.FromToRotation(Vector3.right, base.transform.InverseTransformDirection(worldCenterOfMass - closestPoint).normalized);
				}
			}
		}

		[Obsolete("computeParentAnchor has been renamed to matchAnchors (UnityUpgradable) -> matchAnchors")]
		public bool computeParentAnchor
		{
			get
			{
				return this.matchAnchors;
			}
			set
			{
				this.matchAnchors = value;
			}
		}

		[Obsolete("Setting joint accelerations is not supported in forward kinematics. To have inverse dynamics take acceleration into account, use GetJointForcesForAcceleration instead", true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetJointAccelerations(List<float> accelerations);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_anchorPosition_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_anchorPosition_Injected(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_parentAnchorPosition_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_parentAnchorPosition_Injected(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_anchorRotation_Injected(out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_anchorRotation_Injected(ref Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_parentAnchorRotation_Injected(out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_parentAnchorRotation_Injected(ref Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_xDrive_Injected(out ArticulationDrive ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_xDrive_Injected(ref ArticulationDrive value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_yDrive_Injected(out ArticulationDrive ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_yDrive_Injected(ref ArticulationDrive value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_zDrive_Injected(out ArticulationDrive ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_zDrive_Injected(ref ArticulationDrive value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_excludeLayers_Injected(out LayerMask ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_excludeLayers_Injected(ref LayerMask value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_includeLayers_Injected(out LayerMask ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_includeLayers_Injected(ref LayerMask value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetAccumulatedForce_Injected([DefaultValue("Time.fixedDeltaTime")] float step, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetAccumulatedTorque_Injected([DefaultValue("Time.fixedDeltaTime")] float step, out Vector3 ret);

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
		private extern void get_inertiaTensor_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_inertiaTensor_Injected(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_inertiaTensorRotation_Injected(out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_inertiaTensorRotation_Injected(ref Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_jointPosition_Injected(out ArticulationReducedSpace ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_jointPosition_Injected(ref ArticulationReducedSpace value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_jointVelocity_Injected(out ArticulationReducedSpace ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_jointVelocity_Injected(ref ArticulationReducedSpace value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_jointAcceleration_Injected(out ArticulationReducedSpace ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_jointAcceleration_Injected(ref ArticulationReducedSpace value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_jointForce_Injected(out ArticulationReducedSpace ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_jointForce_Injected(ref ArticulationReducedSpace value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_driveForce_Injected(out ArticulationReducedSpace ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void TeleportRoot_Injected(ref Vector3 position, ref Quaternion rotation);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetClosestPoint_Injected(ref Vector3 point, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetRelativePointVelocity_Injected(ref Vector3 relativePoint, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetPointVelocity_Injected(ref Vector3 worldPoint, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetJointForcesForAcceleration_Injected(ref ArticulationReducedSpace acceleration, out ArticulationReducedSpace ret);
	}
}
