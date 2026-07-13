using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("PhysicsScriptingClasses.h")]
	[NativeHeader("Modules/Vehicles/WheelCollider.h")]
	public class WheelCollider : Collider
	{
		public Vector3 center
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				WheelCollider.get_center_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				WheelCollider.set_center_Injected(intPtr, ref value);
			}
		}

		public float radius
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return WheelCollider.get_radius_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				WheelCollider.set_radius_Injected(intPtr, value);
			}
		}

		public float suspensionDistance
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return WheelCollider.get_suspensionDistance_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				WheelCollider.set_suspensionDistance_Injected(intPtr, value);
			}
		}

		public JointSpring suspensionSpring
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				JointSpring jointSpring;
				WheelCollider.get_suspensionSpring_Injected(intPtr, out jointSpring);
				return jointSpring;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				WheelCollider.set_suspensionSpring_Injected(intPtr, ref value);
			}
		}

		public bool suspensionExpansionLimited
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return WheelCollider.get_suspensionExpansionLimited_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				WheelCollider.set_suspensionExpansionLimited_Injected(intPtr, value);
			}
		}

		public float forceAppPointDistance
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return WheelCollider.get_forceAppPointDistance_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				WheelCollider.set_forceAppPointDistance_Injected(intPtr, value);
			}
		}

		public float mass
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return WheelCollider.get_mass_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				WheelCollider.set_mass_Injected(intPtr, value);
			}
		}

		public float wheelDampingRate
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return WheelCollider.get_wheelDampingRate_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				WheelCollider.set_wheelDampingRate_Injected(intPtr, value);
			}
		}

		public WheelFrictionCurve forwardFriction
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				WheelFrictionCurve wheelFrictionCurve;
				WheelCollider.get_forwardFriction_Injected(intPtr, out wheelFrictionCurve);
				return wheelFrictionCurve;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				WheelCollider.set_forwardFriction_Injected(intPtr, ref value);
			}
		}

		public WheelFrictionCurve sidewaysFriction
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				WheelFrictionCurve wheelFrictionCurve;
				WheelCollider.get_sidewaysFriction_Injected(intPtr, out wheelFrictionCurve);
				return wheelFrictionCurve;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				WheelCollider.set_sidewaysFriction_Injected(intPtr, ref value);
			}
		}

		public float motorTorque
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return WheelCollider.get_motorTorque_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				WheelCollider.set_motorTorque_Injected(intPtr, value);
			}
		}

		public float brakeTorque
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return WheelCollider.get_brakeTorque_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				WheelCollider.set_brakeTorque_Injected(intPtr, value);
			}
		}

		public float steerAngle
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return WheelCollider.get_steerAngle_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				WheelCollider.set_steerAngle_Injected(intPtr, value);
			}
		}

		public bool isGrounded
		{
			[NativeName("IsGrounded")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return WheelCollider.get_isGrounded_Injected(intPtr);
			}
		}

		public float rpm
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return WheelCollider.get_rpm_Injected(intPtr);
			}
		}

		public float sprungMass
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return WheelCollider.get_sprungMass_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				WheelCollider.set_sprungMass_Injected(intPtr, value);
			}
		}

		public float rotationSpeed
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return WheelCollider.get_rotationSpeed_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				WheelCollider.set_rotationSpeed_Injected(intPtr, value);
			}
		}

		public void ResetSprungMasses()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			WheelCollider.ResetSprungMasses_Injected(intPtr);
		}

		public void ConfigureVehicleSubsteps(float speedThreshold, int stepsBelowThreshold, int stepsAboveThreshold)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			WheelCollider.ConfigureVehicleSubsteps_Injected(intPtr, speedThreshold, stepsBelowThreshold, stepsAboveThreshold);
		}

		public void GetWorldPose(out Vector3 pos, out Quaternion quat)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			WheelCollider.GetWorldPose_Injected(intPtr, out pos, out quat);
		}

		public bool GetGroundHit(out WheelHit hit)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return WheelCollider.GetGroundHit_Injected(intPtr, out hit);
		}

		internal bool isSupported
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return WheelCollider.get_isSupported_Injected(intPtr);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_center_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_center_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_radius_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_radius_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_suspensionDistance_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_suspensionDistance_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_suspensionSpring_Injected(IntPtr _unity_self, out JointSpring ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_suspensionSpring_Injected(IntPtr _unity_self, [In] ref JointSpring value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_suspensionExpansionLimited_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_suspensionExpansionLimited_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_forceAppPointDistance_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_forceAppPointDistance_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_mass_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_mass_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_wheelDampingRate_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_wheelDampingRate_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_forwardFriction_Injected(IntPtr _unity_self, out WheelFrictionCurve ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_forwardFriction_Injected(IntPtr _unity_self, [In] ref WheelFrictionCurve value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_sidewaysFriction_Injected(IntPtr _unity_self, out WheelFrictionCurve ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sidewaysFriction_Injected(IntPtr _unity_self, [In] ref WheelFrictionCurve value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_motorTorque_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_motorTorque_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_brakeTorque_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_brakeTorque_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_steerAngle_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_steerAngle_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isGrounded_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_rpm_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_sprungMass_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sprungMass_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_rotationSpeed_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_rotationSpeed_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResetSprungMasses_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ConfigureVehicleSubsteps_Injected(IntPtr _unity_self, float speedThreshold, int stepsBelowThreshold, int stepsAboveThreshold);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetWorldPose_Injected(IntPtr _unity_self, out Vector3 pos, out Quaternion quat);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetGroundHit_Injected(IntPtr _unity_self, out WheelHit hit);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isSupported_Injected(IntPtr _unity_self);
	}
}
