using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeHeader("Modules/Physics/Rigidbody.h")]
	[RequireComponent(typeof(Transform))]
	public class Rigidbody : Component
	{
		public Vector3 linearVelocity
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				Rigidbody.get_linearVelocity_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody.set_linearVelocity_Injected(intPtr, ref value);
			}
		}

		public Vector3 angularVelocity
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				Rigidbody.get_angularVelocity_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody.set_angularVelocity_Injected(intPtr, ref value);
			}
		}

		public float linearDamping
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody.get_linearDamping_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody.set_linearDamping_Injected(intPtr, value);
			}
		}

		public float angularDamping
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody.get_angularDamping_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody.set_angularDamping_Injected(intPtr, value);
			}
		}

		public float mass
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody.get_mass_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody.set_mass_Injected(intPtr, value);
			}
		}

		public bool useGravity
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody.get_useGravity_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody.set_useGravity_Injected(intPtr, value);
			}
		}

		public float maxDepenetrationVelocity
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody.get_maxDepenetrationVelocity_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody.set_maxDepenetrationVelocity_Injected(intPtr, value);
			}
		}

		public bool isKinematic
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody.get_isKinematic_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody.set_isKinematic_Injected(intPtr, value);
			}
		}

		public bool freezeRotation
		{
			get
			{
				return this.constraints.HasFlag(RigidbodyConstraints.FreezeRotation);
			}
			set
			{
				if (value)
				{
					this.constraints |= RigidbodyConstraints.FreezeRotation;
				}
				else
				{
					this.constraints &= RigidbodyConstraints.FreezePosition;
				}
			}
		}

		public RigidbodyConstraints constraints
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody.get_constraints_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody.set_constraints_Injected(intPtr, value);
			}
		}

		public CollisionDetectionMode collisionDetectionMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody.get_collisionDetectionMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody.set_collisionDetectionMode_Injected(intPtr, value);
			}
		}

		public bool automaticCenterOfMass
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody.get_automaticCenterOfMass_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody.set_automaticCenterOfMass_Injected(intPtr, value);
			}
		}

		public Vector3 centerOfMass
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				Rigidbody.get_centerOfMass_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody.set_centerOfMass_Injected(intPtr, ref value);
			}
		}

		public Vector3 worldCenterOfMass
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				Rigidbody.get_worldCenterOfMass_Injected(intPtr, out vector);
				return vector;
			}
		}

		public bool automaticInertiaTensor
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody.get_automaticInertiaTensor_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody.set_automaticInertiaTensor_Injected(intPtr, value);
			}
		}

		public Quaternion inertiaTensorRotation
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Quaternion quaternion;
				Rigidbody.get_inertiaTensorRotation_Injected(intPtr, out quaternion);
				return quaternion;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody.set_inertiaTensorRotation_Injected(intPtr, ref value);
			}
		}

		public Vector3 inertiaTensor
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				Rigidbody.get_inertiaTensor_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody.set_inertiaTensor_Injected(intPtr, ref value);
			}
		}

		internal Matrix4x4 worldInertiaTensorMatrix
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Matrix4x4 matrix4x;
				Rigidbody.get_worldInertiaTensorMatrix_Injected(intPtr, out matrix4x);
				return matrix4x;
			}
		}

		public bool detectCollisions
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody.get_detectCollisions_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody.set_detectCollisions_Injected(intPtr, value);
			}
		}

		public Vector3 position
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				Rigidbody.get_position_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody.set_position_Injected(intPtr, ref value);
			}
		}

		public Quaternion rotation
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Quaternion quaternion;
				Rigidbody.get_rotation_Injected(intPtr, out quaternion);
				return quaternion;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody.set_rotation_Injected(intPtr, ref value);
			}
		}

		public RigidbodyInterpolation interpolation
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody.get_interpolation_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody.set_interpolation_Injected(intPtr, value);
			}
		}

		public int solverIterations
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody.get_solverIterations_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody.set_solverIterations_Injected(intPtr, value);
			}
		}

		public float sleepThreshold
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody.get_sleepThreshold_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody.set_sleepThreshold_Injected(intPtr, value);
			}
		}

		public float maxAngularVelocity
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody.get_maxAngularVelocity_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody.set_maxAngularVelocity_Injected(intPtr, value);
			}
		}

		public float maxLinearVelocity
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody.get_maxLinearVelocity_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody.set_maxLinearVelocity_Injected(intPtr, value);
			}
		}

		public void MovePosition(Vector3 position)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Rigidbody.MovePosition_Injected(intPtr, ref position);
		}

		public void MoveRotation(Quaternion rotation)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Rigidbody.MoveRotation_Injected(intPtr, ref rotation);
		}

		public void Move(Vector3 position, Quaternion rotation)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Rigidbody.Move_Injected(intPtr, ref position, ref rotation);
		}

		public void Sleep()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Rigidbody.Sleep_Injected(intPtr);
		}

		public bool IsSleeping()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Rigidbody.IsSleeping_Injected(intPtr);
		}

		public void WakeUp()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Rigidbody.WakeUp_Injected(intPtr);
		}

		public void ResetCenterOfMass()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Rigidbody.ResetCenterOfMass_Injected(intPtr);
		}

		public void ResetInertiaTensor()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Rigidbody.ResetInertiaTensor_Injected(intPtr);
		}

		public Vector3 GetRelativePointVelocity(Vector3 relativePoint)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 vector;
			Rigidbody.GetRelativePointVelocity_Injected(intPtr, ref relativePoint, out vector);
			return vector;
		}

		public Vector3 GetPointVelocity(Vector3 worldPoint)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 vector;
			Rigidbody.GetPointVelocity_Injected(intPtr, ref worldPoint, out vector);
			return vector;
		}

		public int solverVelocityIterations
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody.get_solverVelocityIterations_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody.set_solverVelocityIterations_Injected(intPtr, value);
			}
		}

		public void PublishTransform()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Rigidbody.PublishTransform_Injected(intPtr);
		}

		public LayerMask excludeLayers
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LayerMask layerMask;
				Rigidbody.get_excludeLayers_Injected(intPtr, out layerMask);
				return layerMask;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody.set_excludeLayers_Injected(intPtr, ref value);
			}
		}

		public LayerMask includeLayers
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LayerMask layerMask;
				Rigidbody.get_includeLayers_Injected(intPtr, out layerMask);
				return layerMask;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody.set_includeLayers_Injected(intPtr, ref value);
			}
		}

		public Vector3 GetAccumulatedForce([DefaultValue("Time.fixedDeltaTime")] float step)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 vector;
			Rigidbody.GetAccumulatedForce_Injected(intPtr, step, out vector);
			return vector;
		}

		[ExcludeFromDocs]
		public Vector3 GetAccumulatedForce()
		{
			return this.GetAccumulatedForce(Time.fixedDeltaTime);
		}

		public Vector3 GetAccumulatedTorque([DefaultValue("Time.fixedDeltaTime")] float step)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 vector;
			Rigidbody.GetAccumulatedTorque_Injected(intPtr, step, out vector);
			return vector;
		}

		[ExcludeFromDocs]
		public Vector3 GetAccumulatedTorque()
		{
			return this.GetAccumulatedTorque(Time.fixedDeltaTime);
		}

		public void AddForce(Vector3 force, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Rigidbody.AddForce_Injected(intPtr, ref force, mode);
		}

		[ExcludeFromDocs]
		public void AddForce(Vector3 force)
		{
			this.AddForce(force, ForceMode.Force);
		}

		public void AddForce(float x, float y, float z, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			this.AddForce(new Vector3(x, y, z), mode);
		}

		[ExcludeFromDocs]
		public void AddForce(float x, float y, float z)
		{
			this.AddForce(new Vector3(x, y, z), ForceMode.Force);
		}

		public void AddRelativeForce(Vector3 force, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Rigidbody.AddRelativeForce_Injected(intPtr, ref force, mode);
		}

		[ExcludeFromDocs]
		public void AddRelativeForce(Vector3 force)
		{
			this.AddRelativeForce(force, ForceMode.Force);
		}

		public void AddRelativeForce(float x, float y, float z, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			this.AddRelativeForce(new Vector3(x, y, z), mode);
		}

		[ExcludeFromDocs]
		public void AddRelativeForce(float x, float y, float z)
		{
			this.AddRelativeForce(new Vector3(x, y, z), ForceMode.Force);
		}

		public void AddTorque(Vector3 torque, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Rigidbody.AddTorque_Injected(intPtr, ref torque, mode);
		}

		[ExcludeFromDocs]
		public void AddTorque(Vector3 torque)
		{
			this.AddTorque(torque, ForceMode.Force);
		}

		public void AddTorque(float x, float y, float z, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			this.AddTorque(new Vector3(x, y, z), mode);
		}

		[ExcludeFromDocs]
		public void AddTorque(float x, float y, float z)
		{
			this.AddTorque(new Vector3(x, y, z), ForceMode.Force);
		}

		public void AddRelativeTorque(Vector3 torque, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Rigidbody.AddRelativeTorque_Injected(intPtr, ref torque, mode);
		}

		[ExcludeFromDocs]
		public void AddRelativeTorque(Vector3 torque)
		{
			this.AddRelativeTorque(torque, ForceMode.Force);
		}

		public void AddRelativeTorque(float x, float y, float z, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			this.AddRelativeTorque(new Vector3(x, y, z), mode);
		}

		[ExcludeFromDocs]
		public void AddRelativeTorque(float x, float y, float z)
		{
			this.AddRelativeTorque(x, y, z, ForceMode.Force);
		}

		public void AddForceAtPosition(Vector3 force, Vector3 position, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Rigidbody.AddForceAtPosition_Injected(intPtr, ref force, ref position, mode);
		}

		[ExcludeFromDocs]
		public void AddForceAtPosition(Vector3 force, Vector3 position)
		{
			this.AddForceAtPosition(force, position, ForceMode.Force);
		}

		public void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius, [DefaultValue("0.0f")] float upwardsModifier, [DefaultValue("ForceMode.Force)")] ForceMode mode)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Rigidbody.AddExplosionForce_Injected(intPtr, explosionForce, ref explosionPosition, explosionRadius, upwardsModifier, mode);
		}

		[ExcludeFromDocs]
		public void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius, float upwardsModifier)
		{
			this.AddExplosionForce(explosionForce, explosionPosition, explosionRadius, upwardsModifier, ForceMode.Force);
		}

		[ExcludeFromDocs]
		public void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius)
		{
			this.AddExplosionForce(explosionForce, explosionPosition, explosionRadius, 0f, ForceMode.Force);
		}

		[NativeName("ClosestPointOnBounds")]
		private void Internal_ClosestPointOnBounds(Vector3 point, ref Vector3 outPos, ref float distance)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Rigidbody.Internal_ClosestPointOnBounds_Injected(intPtr, ref point, ref outPos, ref distance);
		}

		public Vector3 ClosestPointOnBounds(Vector3 position)
		{
			float num = 0f;
			Vector3 zero = Vector3.zero;
			this.Internal_ClosestPointOnBounds(position, ref zero, ref num);
			return zero;
		}

		private RaycastHit SweepTest(Vector3 direction, float maxDistance, QueryTriggerInteraction queryTriggerInteraction, ref bool hasHit)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RaycastHit raycastHit;
			Rigidbody.SweepTest_Injected(intPtr, ref direction, maxDistance, queryTriggerInteraction, ref hasHit, out raycastHit);
			return raycastHit;
		}

		public bool SweepTest(Vector3 direction, out RaycastHit hitInfo, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			float magnitude = direction.magnitude;
			bool flag = magnitude > float.Epsilon;
			bool flag3;
			if (flag)
			{
				Vector3 vector = direction / magnitude;
				bool flag2 = false;
				hitInfo = this.SweepTest(vector, maxDistance, queryTriggerInteraction, ref flag2);
				flag3 = flag2;
			}
			else
			{
				hitInfo = default(RaycastHit);
				flag3 = false;
			}
			return flag3;
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
			RaycastHit[] array2;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				Rigidbody.Internal_SweepTestAll_Injected(intPtr, ref direction, maxDistance, queryTriggerInteraction, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				RaycastHit[] array;
				blittableArrayWrapper.Unmarshal<RaycastHit>(ref array);
				array2 = array;
			}
			return array2;
		}

		public RaycastHit[] SweepTestAll(Vector3 direction, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
		{
			float magnitude = direction.magnitude;
			bool flag = magnitude > float.Epsilon;
			RaycastHit[] array;
			if (flag)
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

		[Obsolete("Please use Rigidbody.linearDamping instead. (UnityUpgradable) -> linearDamping")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public float drag
		{
			get
			{
				return this.linearDamping;
			}
			set
			{
				this.linearDamping = value;
			}
		}

		[Obsolete("Please use Rigidbody.angularDamping instead. (UnityUpgradable) -> angularDamping")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public float angularDrag
		{
			get
			{
				return this.angularDamping;
			}
			set
			{
				this.angularDamping = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Please use Rigidbody.linearVelocity instead. (UnityUpgradable) -> linearVelocity")]
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

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Please use Rigidbody.mass instead. Setting density on a Rigidbody no longer has any effect.", false)]
		public void SetDensity(float density)
		{
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_linearVelocity_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_linearVelocity_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_angularVelocity_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_angularVelocity_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_linearDamping_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_linearDamping_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_angularDamping_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_angularDamping_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_mass_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_mass_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useGravity_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useGravity_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_maxDepenetrationVelocity_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_maxDepenetrationVelocity_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isKinematic_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_isKinematic_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RigidbodyConstraints get_constraints_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_constraints_Injected(IntPtr _unity_self, RigidbodyConstraints value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern CollisionDetectionMode get_collisionDetectionMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_collisionDetectionMode_Injected(IntPtr _unity_self, CollisionDetectionMode value);

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
		private static extern void get_inertiaTensorRotation_Injected(IntPtr _unity_self, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_inertiaTensorRotation_Injected(IntPtr _unity_self, [In] ref Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_inertiaTensor_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_inertiaTensor_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_worldInertiaTensorMatrix_Injected(IntPtr _unity_self, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_detectCollisions_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_detectCollisions_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_position_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_position_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_rotation_Injected(IntPtr _unity_self, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_rotation_Injected(IntPtr _unity_self, [In] ref Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RigidbodyInterpolation get_interpolation_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_interpolation_Injected(IntPtr _unity_self, RigidbodyInterpolation value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_solverIterations_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_solverIterations_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_sleepThreshold_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sleepThreshold_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_maxAngularVelocity_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_maxAngularVelocity_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_maxLinearVelocity_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_maxLinearVelocity_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MovePosition_Injected(IntPtr _unity_self, [In] ref Vector3 position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MoveRotation_Injected(IntPtr _unity_self, [In] ref Quaternion rotation);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Move_Injected(IntPtr _unity_self, [In] ref Vector3 position, [In] ref Quaternion rotation);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Sleep_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsSleeping_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void WakeUp_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResetCenterOfMass_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResetInertiaTensor_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetRelativePointVelocity_Injected(IntPtr _unity_self, [In] ref Vector3 relativePoint, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPointVelocity_Injected(IntPtr _unity_self, [In] ref Vector3 worldPoint, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_solverVelocityIterations_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_solverVelocityIterations_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PublishTransform_Injected(IntPtr _unity_self);

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
		private static extern void AddExplosionForce_Injected(IntPtr _unity_self, float explosionForce, [In] ref Vector3 explosionPosition, float explosionRadius, [DefaultValue("0.0f")] float upwardsModifier, [DefaultValue("ForceMode.Force)")] ForceMode mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_ClosestPointOnBounds_Injected(IntPtr _unity_self, [In] ref Vector3 point, ref Vector3 outPos, ref float distance);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SweepTest_Injected(IntPtr _unity_self, [In] ref Vector3 direction, float maxDistance, QueryTriggerInteraction queryTriggerInteraction, ref bool hasHit, out RaycastHit ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SweepTestAll_Injected(IntPtr _unity_self, [In] ref Vector3 direction, float maxDistance, QueryTriggerInteraction queryTriggerInteraction, out BlittableArrayWrapper ret);
	}
}
