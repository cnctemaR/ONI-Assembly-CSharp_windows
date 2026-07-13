using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[RequireComponent(typeof(Transform))]
	[NativeHeader("Modules/Physics/ArticulationBody.h")]
	[NativeClass("Physics::ArticulationBody")]
	public class ArticulationBody : Behaviour
	{
		public ArticulationJointType jointType
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ArticulationBody.get_jointType_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_jointType_Injected(intPtr, value);
			}
		}

		public Vector3 anchorPosition
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				ArticulationBody.get_anchorPosition_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_anchorPosition_Injected(intPtr, ref value);
			}
		}

		public Vector3 parentAnchorPosition
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				ArticulationBody.get_parentAnchorPosition_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_parentAnchorPosition_Injected(intPtr, ref value);
			}
		}

		public Quaternion anchorRotation
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Quaternion quaternion;
				ArticulationBody.get_anchorRotation_Injected(intPtr, out quaternion);
				return quaternion;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_anchorRotation_Injected(intPtr, ref value);
			}
		}

		public Quaternion parentAnchorRotation
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Quaternion quaternion;
				ArticulationBody.get_parentAnchorRotation_Injected(intPtr, out quaternion);
				return quaternion;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_parentAnchorRotation_Injected(intPtr, ref value);
			}
		}

		public bool isRoot
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ArticulationBody.get_isRoot_Injected(intPtr);
			}
		}

		public bool matchAnchors
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ArticulationBody.get_matchAnchors_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_matchAnchors_Injected(intPtr, value);
			}
		}

		public ArticulationDofLock linearLockX
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ArticulationBody.get_linearLockX_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_linearLockX_Injected(intPtr, value);
			}
		}

		public ArticulationDofLock linearLockY
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ArticulationBody.get_linearLockY_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_linearLockY_Injected(intPtr, value);
			}
		}

		public ArticulationDofLock linearLockZ
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ArticulationBody.get_linearLockZ_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_linearLockZ_Injected(intPtr, value);
			}
		}

		public ArticulationDofLock swingYLock
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ArticulationBody.get_swingYLock_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_swingYLock_Injected(intPtr, value);
			}
		}

		public ArticulationDofLock swingZLock
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ArticulationBody.get_swingZLock_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_swingZLock_Injected(intPtr, value);
			}
		}

		public ArticulationDofLock twistLock
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ArticulationBody.get_twistLock_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_twistLock_Injected(intPtr, value);
			}
		}

		public ArticulationDrive xDrive
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationDrive articulationDrive;
				ArticulationBody.get_xDrive_Injected(intPtr, out articulationDrive);
				return articulationDrive;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_xDrive_Injected(intPtr, ref value);
			}
		}

		public ArticulationDrive yDrive
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationDrive articulationDrive;
				ArticulationBody.get_yDrive_Injected(intPtr, out articulationDrive);
				return articulationDrive;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_yDrive_Injected(intPtr, ref value);
			}
		}

		public ArticulationDrive zDrive
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationDrive articulationDrive;
				ArticulationBody.get_zDrive_Injected(intPtr, out articulationDrive);
				return articulationDrive;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_zDrive_Injected(intPtr, ref value);
			}
		}

		public bool immovable
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ArticulationBody.get_immovable_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_immovable_Injected(intPtr, value);
			}
		}

		public bool useGravity
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ArticulationBody.get_useGravity_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_useGravity_Injected(intPtr, value);
			}
		}

		public float linearDamping
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ArticulationBody.get_linearDamping_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_linearDamping_Injected(intPtr, value);
			}
		}

		public float angularDamping
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ArticulationBody.get_angularDamping_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_angularDamping_Injected(intPtr, value);
			}
		}

		public float jointFriction
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ArticulationBody.get_jointFriction_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_jointFriction_Injected(intPtr, value);
			}
		}

		public LayerMask excludeLayers
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LayerMask layerMask;
				ArticulationBody.get_excludeLayers_Injected(intPtr, out layerMask);
				return layerMask;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_excludeLayers_Injected(intPtr, ref value);
			}
		}

		public LayerMask includeLayers
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LayerMask layerMask;
				ArticulationBody.get_includeLayers_Injected(intPtr, out layerMask);
				return layerMask;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_includeLayers_Injected(intPtr, ref value);
			}
		}

		public Vector3 GetAccumulatedForce([DefaultValue("Time.fixedDeltaTime")] float step)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 vector;
			ArticulationBody.GetAccumulatedForce_Injected(intPtr, step, out vector);
			return vector;
		}

		[ExcludeFromDocs]
		public Vector3 GetAccumulatedForce()
		{
			return this.GetAccumulatedForce(Time.fixedDeltaTime);
		}

		public Vector3 GetAccumulatedTorque([DefaultValue("Time.fixedDeltaTime")] float step)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 vector;
			ArticulationBody.GetAccumulatedTorque_Injected(intPtr, step, out vector);
			return vector;
		}

		[ExcludeFromDocs]
		public Vector3 GetAccumulatedTorque()
		{
			return this.GetAccumulatedTorque(Time.fixedDeltaTime);
		}

		public void AddForce(Vector3 force, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ArticulationBody.AddForce_Injected(intPtr, ref force, mode);
		}

		[ExcludeFromDocs]
		public void AddForce(Vector3 force)
		{
			this.AddForce(force, ForceMode.Force);
		}

		public void AddRelativeForce(Vector3 force, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ArticulationBody.AddRelativeForce_Injected(intPtr, ref force, mode);
		}

		[ExcludeFromDocs]
		public void AddRelativeForce(Vector3 force)
		{
			this.AddRelativeForce(force, ForceMode.Force);
		}

		public void AddTorque(Vector3 torque, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ArticulationBody.AddTorque_Injected(intPtr, ref torque, mode);
		}

		[ExcludeFromDocs]
		public void AddTorque(Vector3 torque)
		{
			this.AddTorque(torque, ForceMode.Force);
		}

		public void AddRelativeTorque(Vector3 torque, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ArticulationBody.AddRelativeTorque_Injected(intPtr, ref torque, mode);
		}

		[ExcludeFromDocs]
		public void AddRelativeTorque(Vector3 torque)
		{
			this.AddRelativeTorque(torque, ForceMode.Force);
		}

		public void AddForceAtPosition(Vector3 force, Vector3 position, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ArticulationBody.AddForceAtPosition_Injected(intPtr, ref force, ref position, mode);
		}

		[ExcludeFromDocs]
		public void AddForceAtPosition(Vector3 force, Vector3 position)
		{
			this.AddForceAtPosition(force, position, ForceMode.Force);
		}

		public Vector3 linearVelocity
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				ArticulationBody.get_linearVelocity_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_linearVelocity_Injected(intPtr, ref value);
			}
		}

		public Vector3 angularVelocity
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				ArticulationBody.get_angularVelocity_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_angularVelocity_Injected(intPtr, ref value);
			}
		}

		public float mass
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ArticulationBody.get_mass_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_mass_Injected(intPtr, value);
			}
		}

		public bool automaticCenterOfMass
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ArticulationBody.get_automaticCenterOfMass_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_automaticCenterOfMass_Injected(intPtr, value);
			}
		}

		public Vector3 centerOfMass
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				ArticulationBody.get_centerOfMass_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_centerOfMass_Injected(intPtr, ref value);
			}
		}

		public Vector3 worldCenterOfMass
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				ArticulationBody.get_worldCenterOfMass_Injected(intPtr, out vector);
				return vector;
			}
		}

		public bool automaticInertiaTensor
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ArticulationBody.get_automaticInertiaTensor_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_automaticInertiaTensor_Injected(intPtr, value);
			}
		}

		public Vector3 inertiaTensor
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				ArticulationBody.get_inertiaTensor_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_inertiaTensor_Injected(intPtr, ref value);
			}
		}

		internal Matrix4x4 worldInertiaTensorMatrix
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Matrix4x4 matrix4x;
				ArticulationBody.get_worldInertiaTensorMatrix_Injected(intPtr, out matrix4x);
				return matrix4x;
			}
		}

		public Quaternion inertiaTensorRotation
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Quaternion quaternion;
				ArticulationBody.get_inertiaTensorRotation_Injected(intPtr, out quaternion);
				return quaternion;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_inertiaTensorRotation_Injected(intPtr, ref value);
			}
		}

		public void ResetCenterOfMass()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ArticulationBody.ResetCenterOfMass_Injected(intPtr);
		}

		public void ResetInertiaTensor()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ArticulationBody.ResetInertiaTensor_Injected(intPtr);
		}

		public void Sleep()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ArticulationBody.Sleep_Injected(intPtr);
		}

		public bool IsSleeping()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return ArticulationBody.IsSleeping_Injected(intPtr);
		}

		public void WakeUp()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ArticulationBody.WakeUp_Injected(intPtr);
		}

		public float sleepThreshold
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ArticulationBody.get_sleepThreshold_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_sleepThreshold_Injected(intPtr, value);
			}
		}

		public int solverIterations
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ArticulationBody.get_solverIterations_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_solverIterations_Injected(intPtr, value);
			}
		}

		public int solverVelocityIterations
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ArticulationBody.get_solverVelocityIterations_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_solverVelocityIterations_Injected(intPtr, value);
			}
		}

		public float maxAngularVelocity
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ArticulationBody.get_maxAngularVelocity_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_maxAngularVelocity_Injected(intPtr, value);
			}
		}

		public float maxLinearVelocity
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ArticulationBody.get_maxLinearVelocity_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_maxLinearVelocity_Injected(intPtr, value);
			}
		}

		public float maxJointVelocity
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ArticulationBody.get_maxJointVelocity_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_maxJointVelocity_Injected(intPtr, value);
			}
		}

		public float maxDepenetrationVelocity
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ArticulationBody.get_maxDepenetrationVelocity_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_maxDepenetrationVelocity_Injected(intPtr, value);
			}
		}

		public ArticulationReducedSpace jointPosition
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationReducedSpace articulationReducedSpace;
				ArticulationBody.get_jointPosition_Injected(intPtr, out articulationReducedSpace);
				return articulationReducedSpace;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_jointPosition_Injected(intPtr, ref value);
			}
		}

		public ArticulationReducedSpace jointVelocity
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationReducedSpace articulationReducedSpace;
				ArticulationBody.get_jointVelocity_Injected(intPtr, out articulationReducedSpace);
				return articulationReducedSpace;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_jointVelocity_Injected(intPtr, ref value);
			}
		}

		public ArticulationReducedSpace jointAcceleration
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationReducedSpace articulationReducedSpace;
				ArticulationBody.get_jointAcceleration_Injected(intPtr, out articulationReducedSpace);
				return articulationReducedSpace;
			}
			[Obsolete("Setting joint accelerations is not supported in forward kinematics. To have inverse dynamics take acceleration into account, use GetJointForcesForAcceleration instead", true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_jointAcceleration_Injected(intPtr, ref value);
			}
		}

		public ArticulationReducedSpace jointForce
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationReducedSpace articulationReducedSpace;
				ArticulationBody.get_jointForce_Injected(intPtr, out articulationReducedSpace);
				return articulationReducedSpace;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_jointForce_Injected(intPtr, ref value);
			}
		}

		public ArticulationReducedSpace driveForce
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationReducedSpace articulationReducedSpace;
				ArticulationBody.get_driveForce_Injected(intPtr, out articulationReducedSpace);
				return articulationReducedSpace;
			}
		}

		public int dofCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ArticulationBody.get_dofCount_Injected(intPtr);
			}
		}

		public int index
		{
			[NativeMethod("GetBodyIndex")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ArticulationBody.get_index_Injected(intPtr);
			}
		}

		public void TeleportRoot(Vector3 position, Quaternion rotation)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ArticulationBody.TeleportRoot_Injected(intPtr, ref position, ref rotation);
		}

		public Vector3 GetClosestPoint(Vector3 point)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 vector;
			ArticulationBody.GetClosestPoint_Injected(intPtr, ref point, out vector);
			return vector;
		}

		public Vector3 GetRelativePointVelocity(Vector3 relativePoint)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 vector;
			ArticulationBody.GetRelativePointVelocity_Injected(intPtr, ref relativePoint, out vector);
			return vector;
		}

		public Vector3 GetPointVelocity(Vector3 worldPoint)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 vector;
			ArticulationBody.GetPointVelocity_Injected(intPtr, ref worldPoint, out vector);
			return vector;
		}

		[NativeMethod("GetDenseJacobian")]
		private int GetDenseJacobian_Internal(ref ArticulationJacobian jacobian)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return ArticulationBody.GetDenseJacobian_Internal_Injected(intPtr, ref jacobian);
		}

		public int GetDenseJacobian(ref ArticulationJacobian jacobian)
		{
			bool flag = jacobian.elements == null;
			if (flag)
			{
				jacobian.elements = new List<float>();
			}
			return this.GetDenseJacobian_Internal(ref jacobian);
		}

		public unsafe int GetJointPositions(List<float> positions)
		{
			int jointPositions_Injected;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableListWrapper blittableListWrapper;
				if (positions != null)
				{
					fixed (float[] array = NoAllocHelpers.ExtractArrayFromList<float>(positions))
					{
						BlittableArrayWrapper blittableArrayWrapper;
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
						blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, positions.Count);
					}
				}
				jointPositions_Injected = ArticulationBody.GetJointPositions_Injected(intPtr, ref blittableListWrapper);
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<float>(positions);
			}
			return jointPositions_Injected;
		}

		public unsafe void SetJointPositions(List<float> positions)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableListWrapper blittableListWrapper;
				if (positions != null)
				{
					fixed (float[] array = NoAllocHelpers.ExtractArrayFromList<float>(positions))
					{
						BlittableArrayWrapper blittableArrayWrapper;
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
						blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, positions.Count);
					}
				}
				ArticulationBody.SetJointPositions_Injected(intPtr, ref blittableListWrapper);
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<float>(positions);
			}
		}

		public unsafe int GetJointVelocities(List<float> velocities)
		{
			int jointVelocities_Injected;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableListWrapper blittableListWrapper;
				if (velocities != null)
				{
					fixed (float[] array = NoAllocHelpers.ExtractArrayFromList<float>(velocities))
					{
						BlittableArrayWrapper blittableArrayWrapper;
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
						blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, velocities.Count);
					}
				}
				jointVelocities_Injected = ArticulationBody.GetJointVelocities_Injected(intPtr, ref blittableListWrapper);
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<float>(velocities);
			}
			return jointVelocities_Injected;
		}

		public unsafe void SetJointVelocities(List<float> velocities)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableListWrapper blittableListWrapper;
				if (velocities != null)
				{
					fixed (float[] array = NoAllocHelpers.ExtractArrayFromList<float>(velocities))
					{
						BlittableArrayWrapper blittableArrayWrapper;
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
						blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, velocities.Count);
					}
				}
				ArticulationBody.SetJointVelocities_Injected(intPtr, ref blittableListWrapper);
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<float>(velocities);
			}
		}

		public unsafe int GetJointAccelerations(List<float> accelerations)
		{
			int jointAccelerations_Injected;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableListWrapper blittableListWrapper;
				if (accelerations != null)
				{
					fixed (float[] array = NoAllocHelpers.ExtractArrayFromList<float>(accelerations))
					{
						BlittableArrayWrapper blittableArrayWrapper;
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
						blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, accelerations.Count);
					}
				}
				jointAccelerations_Injected = ArticulationBody.GetJointAccelerations_Injected(intPtr, ref blittableListWrapper);
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<float>(accelerations);
			}
			return jointAccelerations_Injected;
		}

		public unsafe int GetJointForces(List<float> forces)
		{
			int jointForces_Injected;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableListWrapper blittableListWrapper;
				if (forces != null)
				{
					fixed (float[] array = NoAllocHelpers.ExtractArrayFromList<float>(forces))
					{
						BlittableArrayWrapper blittableArrayWrapper;
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
						blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, forces.Count);
					}
				}
				jointForces_Injected = ArticulationBody.GetJointForces_Injected(intPtr, ref blittableListWrapper);
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<float>(forces);
			}
			return jointForces_Injected;
		}

		public unsafe void SetJointForces(List<float> forces)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableListWrapper blittableListWrapper;
				if (forces != null)
				{
					fixed (float[] array = NoAllocHelpers.ExtractArrayFromList<float>(forces))
					{
						BlittableArrayWrapper blittableArrayWrapper;
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
						blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, forces.Count);
					}
				}
				ArticulationBody.SetJointForces_Injected(intPtr, ref blittableListWrapper);
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<float>(forces);
			}
		}

		public ArticulationReducedSpace GetJointForcesForAcceleration(ArticulationReducedSpace acceleration)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ArticulationReducedSpace articulationReducedSpace;
			ArticulationBody.GetJointForcesForAcceleration_Injected(intPtr, ref acceleration, out articulationReducedSpace);
			return articulationReducedSpace;
		}

		public unsafe int GetDriveForces(List<float> forces)
		{
			int driveForces_Injected;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableListWrapper blittableListWrapper;
				if (forces != null)
				{
					fixed (float[] array = NoAllocHelpers.ExtractArrayFromList<float>(forces))
					{
						BlittableArrayWrapper blittableArrayWrapper;
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
						blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, forces.Count);
					}
				}
				driveForces_Injected = ArticulationBody.GetDriveForces_Injected(intPtr, ref blittableListWrapper);
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<float>(forces);
			}
			return driveForces_Injected;
		}

		public unsafe int GetJointGravityForces(List<float> forces)
		{
			int jointGravityForces_Injected;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableListWrapper blittableListWrapper;
				if (forces != null)
				{
					fixed (float[] array = NoAllocHelpers.ExtractArrayFromList<float>(forces))
					{
						BlittableArrayWrapper blittableArrayWrapper;
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
						blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, forces.Count);
					}
				}
				jointGravityForces_Injected = ArticulationBody.GetJointGravityForces_Injected(intPtr, ref blittableListWrapper);
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<float>(forces);
			}
			return jointGravityForces_Injected;
		}

		public unsafe int GetJointCoriolisCentrifugalForces(List<float> forces)
		{
			int jointCoriolisCentrifugalForces_Injected;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableListWrapper blittableListWrapper;
				if (forces != null)
				{
					fixed (float[] array = NoAllocHelpers.ExtractArrayFromList<float>(forces))
					{
						BlittableArrayWrapper blittableArrayWrapper;
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
						blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, forces.Count);
					}
				}
				jointCoriolisCentrifugalForces_Injected = ArticulationBody.GetJointCoriolisCentrifugalForces_Injected(intPtr, ref blittableListWrapper);
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<float>(forces);
			}
			return jointCoriolisCentrifugalForces_Injected;
		}

		public unsafe int GetJointExternalForces(List<float> forces, float step)
		{
			int jointExternalForces_Injected;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableListWrapper blittableListWrapper;
				if (forces != null)
				{
					fixed (float[] array = NoAllocHelpers.ExtractArrayFromList<float>(forces))
					{
						BlittableArrayWrapper blittableArrayWrapper;
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
						blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, forces.Count);
					}
				}
				jointExternalForces_Injected = ArticulationBody.GetJointExternalForces_Injected(intPtr, ref blittableListWrapper, step);
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<float>(forces);
			}
			return jointExternalForces_Injected;
		}

		public unsafe int GetDriveTargets(List<float> targets)
		{
			int driveTargets_Injected;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableListWrapper blittableListWrapper;
				if (targets != null)
				{
					fixed (float[] array = NoAllocHelpers.ExtractArrayFromList<float>(targets))
					{
						BlittableArrayWrapper blittableArrayWrapper;
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
						blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, targets.Count);
					}
				}
				driveTargets_Injected = ArticulationBody.GetDriveTargets_Injected(intPtr, ref blittableListWrapper);
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<float>(targets);
			}
			return driveTargets_Injected;
		}

		public unsafe void SetDriveTargets(List<float> targets)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableListWrapper blittableListWrapper;
				if (targets != null)
				{
					fixed (float[] array = NoAllocHelpers.ExtractArrayFromList<float>(targets))
					{
						BlittableArrayWrapper blittableArrayWrapper;
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
						blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, targets.Count);
					}
				}
				ArticulationBody.SetDriveTargets_Injected(intPtr, ref blittableListWrapper);
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<float>(targets);
			}
		}

		public unsafe int GetDriveTargetVelocities(List<float> targetVelocities)
		{
			int driveTargetVelocities_Injected;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableListWrapper blittableListWrapper;
				if (targetVelocities != null)
				{
					fixed (float[] array = NoAllocHelpers.ExtractArrayFromList<float>(targetVelocities))
					{
						BlittableArrayWrapper blittableArrayWrapper;
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
						blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, targetVelocities.Count);
					}
				}
				driveTargetVelocities_Injected = ArticulationBody.GetDriveTargetVelocities_Injected(intPtr, ref blittableListWrapper);
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<float>(targetVelocities);
			}
			return driveTargetVelocities_Injected;
		}

		public unsafe void SetDriveTargetVelocities(List<float> targetVelocities)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableListWrapper blittableListWrapper;
				if (targetVelocities != null)
				{
					fixed (float[] array = NoAllocHelpers.ExtractArrayFromList<float>(targetVelocities))
					{
						BlittableArrayWrapper blittableArrayWrapper;
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
						blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, targetVelocities.Count);
					}
				}
				ArticulationBody.SetDriveTargetVelocities_Injected(intPtr, ref blittableListWrapper);
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<float>(targetVelocities);
			}
		}

		public unsafe int GetDofStartIndices(List<int> dofStartIndices)
		{
			int dofStartIndices_Injected;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableListWrapper blittableListWrapper;
				if (dofStartIndices != null)
				{
					fixed (int[] array = NoAllocHelpers.ExtractArrayFromList<int>(dofStartIndices))
					{
						BlittableArrayWrapper blittableArrayWrapper;
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
						blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, dofStartIndices.Count);
					}
				}
				dofStartIndices_Injected = ArticulationBody.GetDofStartIndices_Injected(intPtr, ref blittableListWrapper);
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<int>(dofStartIndices);
			}
			return dofStartIndices_Injected;
		}

		public void SetDriveTarget(ArticulationDriveAxis axis, float value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ArticulationBody.SetDriveTarget_Injected(intPtr, axis, value);
		}

		public void SetDriveTargetVelocity(ArticulationDriveAxis axis, float value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ArticulationBody.SetDriveTargetVelocity_Injected(intPtr, axis, value);
		}

		public void SetDriveLimits(ArticulationDriveAxis axis, float lower, float upper)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ArticulationBody.SetDriveLimits_Injected(intPtr, axis, lower, upper);
		}

		public void SetDriveStiffness(ArticulationDriveAxis axis, float value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ArticulationBody.SetDriveStiffness_Injected(intPtr, axis, value);
		}

		public void SetDriveDamping(ArticulationDriveAxis axis, float value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ArticulationBody.SetDriveDamping_Injected(intPtr, axis, value);
		}

		public void SetDriveForceLimit(ArticulationDriveAxis axis, float value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ArticulationBody.SetDriveForceLimit_Injected(intPtr, axis, value);
		}

		public CollisionDetectionMode collisionDetectionMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ArticulationBody.get_collisionDetectionMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ArticulationBody.set_collisionDetectionMode_Injected(intPtr, value);
			}
		}

		public void PublishTransform()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ArticulationBody.PublishTransform_Injected(intPtr);
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

		[Obsolete("Please use ArticulationBody.linearVelocity instead. (UnityUpgradable) -> linearVelocity")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Vector3 velocity
		{
			get
			{
				return this.linearVelocity;
			}
			set
			{
				this.linearVelocity = value;
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
		public unsafe void SetJointAccelerations(List<float> accelerations)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ArticulationBody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableListWrapper blittableListWrapper;
				if (accelerations != null)
				{
					fixed (float[] array = NoAllocHelpers.ExtractArrayFromList<float>(accelerations))
					{
						BlittableArrayWrapper blittableArrayWrapper;
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
						blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, accelerations.Count);
					}
				}
				ArticulationBody.SetJointAccelerations_Injected(intPtr, ref blittableListWrapper);
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<float>(accelerations);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ArticulationJointType get_jointType_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_jointType_Injected(IntPtr _unity_self, ArticulationJointType value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_anchorPosition_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_anchorPosition_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_parentAnchorPosition_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_parentAnchorPosition_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_anchorRotation_Injected(IntPtr _unity_self, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_anchorRotation_Injected(IntPtr _unity_self, [In] ref Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_parentAnchorRotation_Injected(IntPtr _unity_self, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_parentAnchorRotation_Injected(IntPtr _unity_self, [In] ref Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isRoot_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_matchAnchors_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_matchAnchors_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ArticulationDofLock get_linearLockX_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_linearLockX_Injected(IntPtr _unity_self, ArticulationDofLock value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ArticulationDofLock get_linearLockY_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_linearLockY_Injected(IntPtr _unity_self, ArticulationDofLock value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ArticulationDofLock get_linearLockZ_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_linearLockZ_Injected(IntPtr _unity_self, ArticulationDofLock value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ArticulationDofLock get_swingYLock_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_swingYLock_Injected(IntPtr _unity_self, ArticulationDofLock value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ArticulationDofLock get_swingZLock_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_swingZLock_Injected(IntPtr _unity_self, ArticulationDofLock value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ArticulationDofLock get_twistLock_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_twistLock_Injected(IntPtr _unity_self, ArticulationDofLock value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_xDrive_Injected(IntPtr _unity_self, out ArticulationDrive ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_xDrive_Injected(IntPtr _unity_self, [In] ref ArticulationDrive value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_yDrive_Injected(IntPtr _unity_self, out ArticulationDrive ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_yDrive_Injected(IntPtr _unity_self, [In] ref ArticulationDrive value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_zDrive_Injected(IntPtr _unity_self, out ArticulationDrive ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_zDrive_Injected(IntPtr _unity_self, [In] ref ArticulationDrive value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_immovable_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_immovable_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useGravity_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useGravity_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_linearDamping_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_linearDamping_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_angularDamping_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_angularDamping_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_jointFriction_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_jointFriction_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_excludeLayers_Injected(IntPtr _unity_self, out LayerMask ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_excludeLayers_Injected(IntPtr _unity_self, [In] ref LayerMask value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_includeLayers_Injected(IntPtr _unity_self, out LayerMask ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_includeLayers_Injected(IntPtr _unity_self, [In] ref LayerMask value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetAccumulatedForce_Injected(IntPtr _unity_self, [DefaultValue("Time.fixedDeltaTime")] float step, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetAccumulatedTorque_Injected(IntPtr _unity_self, [DefaultValue("Time.fixedDeltaTime")] float step, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddForce_Injected(IntPtr _unity_self, [In] ref Vector3 force, [DefaultValue("ForceMode.Force")] ForceMode mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddRelativeForce_Injected(IntPtr _unity_self, [In] ref Vector3 force, [DefaultValue("ForceMode.Force")] ForceMode mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddTorque_Injected(IntPtr _unity_self, [In] ref Vector3 torque, [DefaultValue("ForceMode.Force")] ForceMode mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddRelativeTorque_Injected(IntPtr _unity_self, [In] ref Vector3 torque, [DefaultValue("ForceMode.Force")] ForceMode mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddForceAtPosition_Injected(IntPtr _unity_self, [In] ref Vector3 force, [In] ref Vector3 position, [DefaultValue("ForceMode.Force")] ForceMode mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_linearVelocity_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_linearVelocity_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_angularVelocity_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_angularVelocity_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_mass_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_mass_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_automaticCenterOfMass_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_automaticCenterOfMass_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_centerOfMass_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_centerOfMass_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_worldCenterOfMass_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_automaticInertiaTensor_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_automaticInertiaTensor_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_inertiaTensor_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_inertiaTensor_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_worldInertiaTensorMatrix_Injected(IntPtr _unity_self, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_inertiaTensorRotation_Injected(IntPtr _unity_self, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_inertiaTensorRotation_Injected(IntPtr _unity_self, [In] ref Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResetCenterOfMass_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResetInertiaTensor_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Sleep_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsSleeping_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void WakeUp_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_sleepThreshold_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sleepThreshold_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_solverIterations_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_solverIterations_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_solverVelocityIterations_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_solverVelocityIterations_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_maxAngularVelocity_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_maxAngularVelocity_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_maxLinearVelocity_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_maxLinearVelocity_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_maxJointVelocity_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_maxJointVelocity_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_maxDepenetrationVelocity_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_maxDepenetrationVelocity_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_jointPosition_Injected(IntPtr _unity_self, out ArticulationReducedSpace ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_jointPosition_Injected(IntPtr _unity_self, [In] ref ArticulationReducedSpace value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_jointVelocity_Injected(IntPtr _unity_self, out ArticulationReducedSpace ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_jointVelocity_Injected(IntPtr _unity_self, [In] ref ArticulationReducedSpace value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_jointAcceleration_Injected(IntPtr _unity_self, out ArticulationReducedSpace ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_jointAcceleration_Injected(IntPtr _unity_self, [In] ref ArticulationReducedSpace value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_jointForce_Injected(IntPtr _unity_self, out ArticulationReducedSpace ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_jointForce_Injected(IntPtr _unity_self, [In] ref ArticulationReducedSpace value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_driveForce_Injected(IntPtr _unity_self, out ArticulationReducedSpace ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_dofCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_index_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void TeleportRoot_Injected(IntPtr _unity_self, [In] ref Vector3 position, [In] ref Quaternion rotation);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetClosestPoint_Injected(IntPtr _unity_self, [In] ref Vector3 point, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetRelativePointVelocity_Injected(IntPtr _unity_self, [In] ref Vector3 relativePoint, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPointVelocity_Injected(IntPtr _unity_self, [In] ref Vector3 worldPoint, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetDenseJacobian_Internal_Injected(IntPtr _unity_self, ref ArticulationJacobian jacobian);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetJointPositions_Injected(IntPtr _unity_self, ref BlittableListWrapper positions);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetJointPositions_Injected(IntPtr _unity_self, ref BlittableListWrapper positions);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetJointVelocities_Injected(IntPtr _unity_self, ref BlittableListWrapper velocities);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetJointVelocities_Injected(IntPtr _unity_self, ref BlittableListWrapper velocities);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetJointAccelerations_Injected(IntPtr _unity_self, ref BlittableListWrapper accelerations);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetJointForces_Injected(IntPtr _unity_self, ref BlittableListWrapper forces);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetJointForces_Injected(IntPtr _unity_self, ref BlittableListWrapper forces);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetJointForcesForAcceleration_Injected(IntPtr _unity_self, [In] ref ArticulationReducedSpace acceleration, out ArticulationReducedSpace ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetDriveForces_Injected(IntPtr _unity_self, ref BlittableListWrapper forces);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetJointGravityForces_Injected(IntPtr _unity_self, ref BlittableListWrapper forces);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetJointCoriolisCentrifugalForces_Injected(IntPtr _unity_self, ref BlittableListWrapper forces);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetJointExternalForces_Injected(IntPtr _unity_self, ref BlittableListWrapper forces, float step);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetDriveTargets_Injected(IntPtr _unity_self, ref BlittableListWrapper targets);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetDriveTargets_Injected(IntPtr _unity_self, ref BlittableListWrapper targets);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetDriveTargetVelocities_Injected(IntPtr _unity_self, ref BlittableListWrapper targetVelocities);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetDriveTargetVelocities_Injected(IntPtr _unity_self, ref BlittableListWrapper targetVelocities);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetDofStartIndices_Injected(IntPtr _unity_self, ref BlittableListWrapper dofStartIndices);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetDriveTarget_Injected(IntPtr _unity_self, ArticulationDriveAxis axis, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetDriveTargetVelocity_Injected(IntPtr _unity_self, ArticulationDriveAxis axis, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetDriveLimits_Injected(IntPtr _unity_self, ArticulationDriveAxis axis, float lower, float upper);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetDriveStiffness_Injected(IntPtr _unity_self, ArticulationDriveAxis axis, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetDriveDamping_Injected(IntPtr _unity_self, ArticulationDriveAxis axis, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetDriveForceLimit_Injected(IntPtr _unity_self, ArticulationDriveAxis axis, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern CollisionDetectionMode get_collisionDetectionMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_collisionDetectionMode_Injected(IntPtr _unity_self, CollisionDetectionMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PublishTransform_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetJointAccelerations_Injected(IntPtr _unity_self, ref BlittableListWrapper accelerations);
	}
}
