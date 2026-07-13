using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeHeader("Modules/Physics2D/Public/Rigidbody2D.h")]
	[RequireComponent(typeof(Transform))]
	public sealed class Rigidbody2D : Component
	{
		public Vector2 position
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				Rigidbody2D.get_position_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody2D.set_position_Injected(intPtr, ref value);
			}
		}

		public float rotation
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody2D.get_rotation_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody2D.set_rotation_Injected(intPtr, value);
			}
		}

		public void SetRotation(float angle)
		{
			this.SetRotation_Angle(angle);
		}

		[NativeMethod("SetRotation")]
		private void SetRotation_Angle(float angle)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Rigidbody2D.SetRotation_Angle_Injected(intPtr, angle);
		}

		public void SetRotation(Quaternion rotation)
		{
			this.SetRotation_Quaternion(rotation);
		}

		[NativeMethod("SetRotation")]
		private void SetRotation_Quaternion(Quaternion rotation)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Rigidbody2D.SetRotation_Quaternion_Injected(intPtr, ref rotation);
		}

		public void MovePosition(Vector2 position)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Rigidbody2D.MovePosition_Injected(intPtr, ref position);
		}

		public void MoveRotation(float angle)
		{
			this.MoveRotation_Angle(angle);
		}

		[NativeMethod("MoveRotation")]
		private void MoveRotation_Angle(float angle)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Rigidbody2D.MoveRotation_Angle_Injected(intPtr, angle);
		}

		public void MoveRotation(Quaternion rotation)
		{
			this.MoveRotation_Quaternion(rotation);
		}

		[NativeMethod("MoveRotation")]
		private void MoveRotation_Quaternion(Quaternion rotation)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Rigidbody2D.MoveRotation_Quaternion_Injected(intPtr, ref rotation);
		}

		[NativeMethod("MovePositionAndRotation")]
		public void MovePositionAndRotation(Vector2 position, float angle)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Rigidbody2D.MovePositionAndRotation_Injected(intPtr, ref position, angle);
		}

		public void MovePositionAndRotation(Vector2 position, Quaternion rotation)
		{
			this.MovePositionAndRotation_Quaternion(position, rotation);
		}

		[NativeMethod("MovePositionAndRotation")]
		private void MovePositionAndRotation_Quaternion(Vector2 position, Quaternion rotation)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Rigidbody2D.MovePositionAndRotation_Quaternion_Injected(intPtr, ref position, ref rotation);
		}

		public Rigidbody2D.SlideResults Slide(Vector2 velocity, float deltaTime, Rigidbody2D.SlideMovement slideMovement)
		{
			bool flag = deltaTime < 0f;
			if (flag)
			{
				throw new ArgumentException(string.Format("Time cannot be negative. It is {0}.", deltaTime), "deltaTime");
			}
			bool flag2 = Mathf.Approximately(deltaTime, 0f);
			Rigidbody2D.SlideResults slideResults;
			if (flag2)
			{
				slideResults = new Rigidbody2D.SlideResults
				{
					position = (slideMovement.useStartPosition ? slideMovement.startPosition : this.position),
					remainingVelocity = velocity
				};
			}
			else
			{
				bool flag3 = slideMovement.useSimulationMove && this.bodyType == RigidbodyType2D.Static;
				if (flag3)
				{
					throw new ArgumentException(string.Format("Cannot use simulation move when the body type is Static. It is {0}.", slideMovement.useSimulationMove), "SlideMovement.useSimulationMove");
				}
				bool flag4 = slideMovement.useNoMove && slideMovement.useSimulationMove;
				if (flag4)
				{
					throw new ArgumentException(string.Format("Cannot use no move and simulation move at the same time; the two are conflicting options. It is {0}.", slideMovement.useNoMove), "SlideMovement.useNoMove");
				}
				bool flag5 = slideMovement.maxIterations < 1;
				if (flag5)
				{
					throw new ArgumentException(string.Format("Maximum Iterations must be greater than zero. It is {0}.", slideMovement.maxIterations), "SlideMovement.maxIterations");
				}
				bool flag6 = !float.IsFinite(slideMovement.surfaceSlideAngle) || slideMovement.surfaceSlideAngle < 0f || slideMovement.surfaceSlideAngle > 90f;
				if (flag6)
				{
					throw new ArgumentException(string.Format("Surface Slide Angle must be in the range of 0 to 90 degrees. It is {0}.", slideMovement.surfaceSlideAngle), "SlideMovement.surfaceSlideAngle");
				}
				bool flag7 = !float.IsFinite(slideMovement.gravitySlipAngle) || slideMovement.gravitySlipAngle < 0f || slideMovement.gravitySlipAngle > 90f;
				if (flag7)
				{
					throw new ArgumentException(string.Format("Gravity Slip Angle must be in the range of 0 to 90 degrees. It is {0}.", slideMovement.gravitySlipAngle), "SlideMovement.gravitySlipAngle");
				}
				bool flag8 = !float.IsFinite(slideMovement.surfaceUp.x) || !float.IsFinite(slideMovement.surfaceUp.y);
				if (flag8)
				{
					throw new ArgumentException(string.Format("Surface Up is invalid. It is {0}.", slideMovement.surfaceUp), "SlideMovement.surfaceUp");
				}
				bool flag9 = !float.IsFinite(slideMovement.surfaceAnchor.x) || !float.IsFinite(slideMovement.surfaceAnchor.y);
				if (flag9)
				{
					throw new ArgumentException(string.Format("Surface Anchor is invalid. It is {0}.", slideMovement.surfaceAnchor), "SlideMovement.surfaceAnchor");
				}
				bool flag10 = !float.IsFinite(slideMovement.gravity.x) || !float.IsFinite(slideMovement.gravity.y);
				if (flag10)
				{
					throw new ArgumentException(string.Format("Gravity is invalid. It is {0}.", slideMovement.gravity), "SlideMovement.gravity");
				}
				bool flag11 = !float.IsFinite(slideMovement.startPosition.x) || !float.IsFinite(slideMovement.startPosition.y);
				if (flag11)
				{
					throw new ArgumentException(string.Format("Start Position is invalid. It is {0}.", slideMovement.gravity), "SlideMovement.startPosition");
				}
				bool flag12 = slideMovement.selectedCollider && slideMovement.selectedCollider.attachedRigidbody != this;
				if (flag12)
				{
					throw new ArgumentException(string.Format("Selected Collider must be attached to the Slide Rigidbody2D. It is {0}.", slideMovement.selectedCollider), "SlideMovement.selectedCollider");
				}
				slideResults = this.Slide_Internal(velocity, deltaTime, slideMovement);
			}
			return slideResults;
		}

		[NativeMethod("Slide")]
		private Rigidbody2D.SlideResults Slide_Internal(Vector2 velocity, float deltaTime, Rigidbody2D.SlideMovement slideMovement)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Rigidbody2D.SlideResults slideResults;
			Rigidbody2D.Slide_Internal_Injected(intPtr, ref velocity, deltaTime, ref slideMovement, out slideResults);
			return slideResults;
		}

		public Vector2 linearVelocity
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				Rigidbody2D.get_linearVelocity_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody2D.set_linearVelocity_Injected(intPtr, ref value);
			}
		}

		public float linearVelocityX
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody2D.get_linearVelocityX_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody2D.set_linearVelocityX_Injected(intPtr, value);
			}
		}

		public float linearVelocityY
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody2D.get_linearVelocityY_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody2D.set_linearVelocityY_Injected(intPtr, value);
			}
		}

		public float angularVelocity
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody2D.get_angularVelocity_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody2D.set_angularVelocity_Injected(intPtr, value);
			}
		}

		public bool useAutoMass
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody2D.get_useAutoMass_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody2D.set_useAutoMass_Injected(intPtr, value);
			}
		}

		public float mass
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody2D.get_mass_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody2D.set_mass_Injected(intPtr, value);
			}
		}

		[NativeMethod("Material")]
		public PhysicsMaterial2D sharedMaterial
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<PhysicsMaterial2D>(Rigidbody2D.get_sharedMaterial_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody2D.set_sharedMaterial_Injected(intPtr, Object.MarshalledUnityObject.Marshal<PhysicsMaterial2D>(value));
			}
		}

		public Vector2 centerOfMass
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				Rigidbody2D.get_centerOfMass_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody2D.set_centerOfMass_Injected(intPtr, ref value);
			}
		}

		public Vector2 worldCenterOfMass
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				Rigidbody2D.get_worldCenterOfMass_Injected(intPtr, out vector);
				return vector;
			}
		}

		public float inertia
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody2D.get_inertia_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody2D.set_inertia_Injected(intPtr, value);
			}
		}

		public float linearDamping
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody2D.get_linearDamping_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody2D.set_linearDamping_Injected(intPtr, value);
			}
		}

		public float angularDamping
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody2D.get_angularDamping_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody2D.set_angularDamping_Injected(intPtr, value);
			}
		}

		public float gravityScale
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody2D.get_gravityScale_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody2D.set_gravityScale_Injected(intPtr, value);
			}
		}

		public RigidbodyType2D bodyType
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody2D.get_bodyType_Injected(intPtr);
			}
			[NativeMethod("SetBodyType_Binding")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody2D.set_bodyType_Injected(intPtr, value);
			}
		}

		internal void SetDragBehaviour(bool dragged)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Rigidbody2D.SetDragBehaviour_Injected(intPtr, dragged);
		}

		public bool useFullKinematicContacts
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody2D.get_useFullKinematicContacts_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody2D.set_useFullKinematicContacts_Injected(intPtr, value);
			}
		}

		public bool freezeRotation
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody2D.get_freezeRotation_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody2D.set_freezeRotation_Injected(intPtr, value);
			}
		}

		public RigidbodyConstraints2D constraints
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody2D.get_constraints_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody2D.set_constraints_Injected(intPtr, value);
			}
		}

		public bool IsSleeping()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Rigidbody2D.IsSleeping_Injected(intPtr);
		}

		public bool IsAwake()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Rigidbody2D.IsAwake_Injected(intPtr);
		}

		public void Sleep()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Rigidbody2D.Sleep_Injected(intPtr);
		}

		[NativeMethod("Wake")]
		public void WakeUp()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Rigidbody2D.WakeUp_Injected(intPtr);
		}

		public bool simulated
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody2D.get_simulated_Injected(intPtr);
			}
			[NativeMethod("SetSimulated_Binding")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody2D.set_simulated_Injected(intPtr, value);
			}
		}

		public RigidbodyInterpolation2D interpolation
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody2D.get_interpolation_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody2D.set_interpolation_Injected(intPtr, value);
			}
		}

		public RigidbodySleepMode2D sleepMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody2D.get_sleepMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody2D.set_sleepMode_Injected(intPtr, value);
			}
		}

		public CollisionDetectionMode2D collisionDetectionMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody2D.get_collisionDetectionMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody2D.set_collisionDetectionMode_Injected(intPtr, value);
			}
		}

		public int attachedColliderCount
		{
			get
			{
				return this.GetAttachedColliderCount_Internal(true);
			}
		}

		[NativeMethod("GetAttachedColliderCount")]
		private int GetAttachedColliderCount_Internal(bool findTriggers)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Rigidbody2D.GetAttachedColliderCount_Internal_Injected(intPtr, findTriggers);
		}

		public Vector2 totalForce
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				Rigidbody2D.get_totalForce_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody2D.set_totalForce_Injected(intPtr, ref value);
			}
		}

		public float totalTorque
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Rigidbody2D.get_totalTorque_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody2D.set_totalTorque_Injected(intPtr, value);
			}
		}

		public LayerMask excludeLayers
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LayerMask layerMask;
				Rigidbody2D.get_excludeLayers_Injected(intPtr, out layerMask);
				return layerMask;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody2D.set_excludeLayers_Injected(intPtr, ref value);
			}
		}

		public LayerMask includeLayers
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LayerMask layerMask;
				Rigidbody2D.get_includeLayers_Injected(intPtr, out layerMask);
				return layerMask;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rigidbody2D.set_includeLayers_Injected(intPtr, ref value);
			}
		}

		public Matrix4x4 localToWorldMatrix
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Matrix4x4 matrix4x;
				Rigidbody2D.get_localToWorldMatrix_Injected(intPtr, out matrix4x);
				return matrix4x;
			}
		}

		public bool IsTouching([NotNull] Collider2D collider)
		{
			if (collider == null)
			{
				ThrowHelper.ThrowArgumentNullException(collider, "collider");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(collider);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(collider, "collider");
			}
			return Rigidbody2D.IsTouching_Injected(intPtr, intPtr2);
		}

		public bool IsTouching(Collider2D collider, ContactFilter2D contactFilter)
		{
			return this.IsTouching_OtherColliderWithFilter_Internal(collider, contactFilter);
		}

		[NativeMethod("IsTouching")]
		private bool IsTouching_OtherColliderWithFilter_Internal([NotNull] Collider2D collider, ContactFilter2D contactFilter)
		{
			if (collider == null)
			{
				ThrowHelper.ThrowArgumentNullException(collider, "collider");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(collider);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(collider, "collider");
			}
			return Rigidbody2D.IsTouching_OtherColliderWithFilter_Internal_Injected(intPtr, intPtr2, ref contactFilter);
		}

		public bool IsTouching(ContactFilter2D contactFilter)
		{
			return this.IsTouching_AnyColliderWithFilter_Internal(contactFilter);
		}

		[NativeMethod("IsTouching")]
		private bool IsTouching_AnyColliderWithFilter_Internal(ContactFilter2D contactFilter)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Rigidbody2D.IsTouching_AnyColliderWithFilter_Internal_Injected(intPtr, ref contactFilter);
		}

		[ExcludeFromDocs]
		public bool IsTouchingLayers()
		{
			return this.IsTouchingLayers(-1);
		}

		public bool IsTouchingLayers([DefaultValue("Physics2D.AllLayers")] int layerMask = -1)
		{
			ContactFilter2D contactFilter2D = default(ContactFilter2D);
			contactFilter2D.SetLayerMask(layerMask);
			contactFilter2D.useTriggers = Physics2D.queriesHitTriggers;
			return this.IsTouching(contactFilter2D);
		}

		public bool OverlapPoint(Vector2 point)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Rigidbody2D.OverlapPoint_Injected(intPtr, ref point);
		}

		public ColliderDistance2D Distance(Collider2D collider)
		{
			bool flag = collider == null;
			if (flag)
			{
				throw new ArgumentNullException("Collider cannot be null.");
			}
			bool flag2 = collider.attachedRigidbody == this;
			if (flag2)
			{
				throw new ArgumentException("The collider cannot be attached to the Rigidbody2D being searched.");
			}
			return this.Distance_Internal(collider);
		}

		public ColliderDistance2D Distance(Vector2 thisPosition, float thisAngle, Collider2D collider, Vector2 position, float angle)
		{
			bool flag = !collider.attachedRigidbody;
			if (flag)
			{
				throw new InvalidOperationException("Cannot perform a Collider Distance at a specific position and angle if the Collider is not attached to a Rigidbody2D.");
			}
			return this.DistanceFrom_Internal(thisPosition, thisAngle, collider, position, angle);
		}

		[NativeMethod("Distance")]
		private ColliderDistance2D Distance_Internal([NotNull] Collider2D collider)
		{
			if (collider == null)
			{
				ThrowHelper.ThrowArgumentNullException(collider, "collider");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(collider);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(collider, "collider");
			}
			ColliderDistance2D colliderDistance2D;
			Rigidbody2D.Distance_Internal_Injected(intPtr, intPtr2, out colliderDistance2D);
			return colliderDistance2D;
		}

		[NativeMethod("DistanceFrom")]
		private ColliderDistance2D DistanceFrom_Internal(Vector2 thisPosition, float thisAngle, [NotNull] Collider2D collider, Vector2 position, float angle)
		{
			if (collider == null)
			{
				ThrowHelper.ThrowArgumentNullException(collider, "collider");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Collider2D>(collider);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(collider, "collider");
			}
			ColliderDistance2D colliderDistance2D;
			Rigidbody2D.DistanceFrom_Internal_Injected(intPtr, ref thisPosition, thisAngle, intPtr2, ref position, angle, out colliderDistance2D);
			return colliderDistance2D;
		}

		public Vector2 ClosestPoint(Vector2 position)
		{
			return Physics2D.ClosestPoint(position, this);
		}

		[ExcludeFromDocs]
		public void AddForce(Vector2 force)
		{
			this.AddForce_Internal(force, ForceMode2D.Force);
		}

		public void AddForce(Vector2 force, [DefaultValue("ForceMode2D.Force")] ForceMode2D mode = ForceMode2D.Force)
		{
			this.AddForce_Internal(force, mode);
		}

		public void AddForceX(float force, [DefaultValue("ForceMode2D.Force")] ForceMode2D mode = ForceMode2D.Force)
		{
			this.AddForce_Internal(new Vector2(force, 0f), mode);
		}

		public void AddForceY(float force, [DefaultValue("ForceMode2D.Force")] ForceMode2D mode = ForceMode2D.Force)
		{
			this.AddForce_Internal(new Vector2(0f, force), mode);
		}

		[NativeMethod("AddForce")]
		private void AddForce_Internal(Vector2 force, [DefaultValue("ForceMode2D.Force")] ForceMode2D mode)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Rigidbody2D.AddForce_Internal_Injected(intPtr, ref force, mode);
		}

		[ExcludeFromDocs]
		public void AddRelativeForce(Vector2 relativeForce)
		{
			this.AddRelativeForce_Internal(relativeForce, ForceMode2D.Force);
		}

		public void AddRelativeForce(Vector2 relativeForce, [DefaultValue("ForceMode2D.Force")] ForceMode2D mode = ForceMode2D.Force)
		{
			this.AddRelativeForce_Internal(relativeForce, mode);
		}

		public void AddRelativeForceX(float force, [DefaultValue("ForceMode2D.Force")] ForceMode2D mode = ForceMode2D.Force)
		{
			this.AddRelativeForce_Internal(new Vector2(force, 0f), mode);
		}

		public void AddRelativeForceY(float force, [DefaultValue("ForceMode2D.Force")] ForceMode2D mode = ForceMode2D.Force)
		{
			this.AddRelativeForce_Internal(new Vector2(0f, force), mode);
		}

		[NativeMethod("AddRelativeForce")]
		private void AddRelativeForce_Internal(Vector2 relativeForce, [DefaultValue("ForceMode2D.Force")] ForceMode2D mode)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Rigidbody2D.AddRelativeForce_Internal_Injected(intPtr, ref relativeForce, mode);
		}

		[ExcludeFromDocs]
		public void AddForceAtPosition(Vector2 force, Vector2 position)
		{
			this.AddForceAtPosition(force, position, ForceMode2D.Force);
		}

		public void AddForceAtPosition(Vector2 force, Vector2 position, [DefaultValue("ForceMode2D.Force")] ForceMode2D mode)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Rigidbody2D.AddForceAtPosition_Injected(intPtr, ref force, ref position, mode);
		}

		[ExcludeFromDocs]
		public void AddTorque(float torque)
		{
			this.AddTorque(torque, ForceMode2D.Force);
		}

		public void AddTorque(float torque, [DefaultValue("ForceMode2D.Force")] ForceMode2D mode)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Rigidbody2D.AddTorque_Injected(intPtr, torque, mode);
		}

		public Vector2 GetPoint(Vector2 point)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector2 vector;
			Rigidbody2D.GetPoint_Injected(intPtr, ref point, out vector);
			return vector;
		}

		public Vector2 GetRelativePoint(Vector2 relativePoint)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector2 vector;
			Rigidbody2D.GetRelativePoint_Injected(intPtr, ref relativePoint, out vector);
			return vector;
		}

		public Vector2 GetVector(Vector2 vector)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector2 vector2;
			Rigidbody2D.GetVector_Injected(intPtr, ref vector, out vector2);
			return vector2;
		}

		public Vector2 GetRelativeVector(Vector2 relativeVector)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector2 vector;
			Rigidbody2D.GetRelativeVector_Injected(intPtr, ref relativeVector, out vector);
			return vector;
		}

		public Vector2 GetPointVelocity(Vector2 point)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector2 vector;
			Rigidbody2D.GetPointVelocity_Injected(intPtr, ref point, out vector);
			return vector;
		}

		public Vector2 GetRelativePointVelocity(Vector2 relativePoint)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector2 vector;
			Rigidbody2D.GetRelativePointVelocity_Injected(intPtr, ref relativePoint, out vector);
			return vector;
		}

		public int GetContacts(ContactPoint2D[] contacts)
		{
			return Physics2D.GetContacts(this, ContactFilter2D.noFilter, contacts);
		}

		public int GetContacts(List<ContactPoint2D> contacts)
		{
			return Physics2D.GetContacts(this, ContactFilter2D.noFilter, contacts);
		}

		public int GetContacts(ContactFilter2D contactFilter, ContactPoint2D[] contacts)
		{
			return Physics2D.GetContacts(this, contactFilter, contacts);
		}

		public int GetContacts(ContactFilter2D contactFilter, List<ContactPoint2D> contacts)
		{
			return Physics2D.GetContacts(this, contactFilter, contacts);
		}

		public int GetContacts(Collider2D[] colliders)
		{
			return Physics2D.GetContacts(this, ContactFilter2D.noFilter, colliders);
		}

		public int GetContacts(List<Collider2D> colliders)
		{
			return Physics2D.GetContacts(this, ContactFilter2D.noFilter, colliders);
		}

		public int GetContacts(ContactFilter2D contactFilter, Collider2D[] colliders)
		{
			return Physics2D.GetContacts(this, contactFilter, colliders);
		}

		public int GetContacts(ContactFilter2D contactFilter, List<Collider2D> colliders)
		{
			return Physics2D.GetContacts(this, contactFilter, colliders);
		}

		[ExcludeFromDocs]
		public int GetAttachedColliders([Out] Collider2D[] results)
		{
			return this.GetAttachedCollidersArray_Internal(results, true);
		}

		[ExcludeFromDocs]
		public int GetAttachedColliders(List<Collider2D> results)
		{
			return this.GetAttachedCollidersList_Internal(results, true);
		}

		public int GetAttachedColliders([Out] Collider2D[] results, [DefaultValue("true")] bool findTriggers = true)
		{
			return this.GetAttachedCollidersArray_Internal(results, findTriggers);
		}

		public int GetAttachedColliders(List<Collider2D> results, [DefaultValue("true")] bool findTriggers = true)
		{
			return this.GetAttachedCollidersList_Internal(results, findTriggers);
		}

		public int GetShapes(PhysicsShapeGroup2D physicsShapeGroup)
		{
			return this.GetShapes_Internal(ref physicsShapeGroup.m_GroupState);
		}

		[ExcludeFromDocs]
		public int Cast(Vector2 direction, RaycastHit2D[] results)
		{
			return this.CastArray_Internal(direction, float.PositiveInfinity, false, results);
		}

		public int Cast(Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance)
		{
			return this.CastArray_Internal(direction, distance, false, results);
		}

		public int Cast(Vector2 direction, List<RaycastHit2D> results, [DefaultValue("Mathf.Infinity")] float distance = float.PositiveInfinity)
		{
			return this.CastList_Internal(direction, distance, false, results);
		}

		[ExcludeFromDocs]
		public int Cast(Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results)
		{
			return this.CastFilteredArray_Internal(direction, float.PositiveInfinity, false, contactFilter, results);
		}

		public int Cast(Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance = float.PositiveInfinity)
		{
			return this.CastFilteredArray_Internal(direction, distance, false, contactFilter, results);
		}

		public int Cast(Vector2 direction, ContactFilter2D contactFilter, List<RaycastHit2D> results, [DefaultValue("Mathf.Infinity")] float distance = float.PositiveInfinity)
		{
			return this.CastFilteredList_Internal(direction, distance, false, contactFilter, results);
		}

		public int Cast(Vector2 position, float angle, Vector2 direction, List<RaycastHit2D> results, [DefaultValue("Mathf.Infinity")] float distance = float.PositiveInfinity)
		{
			return this.CastFrom_Internal(position, angle, direction, distance, false, results);
		}

		public int Cast(Vector2 position, float angle, Vector2 direction, ContactFilter2D contactFilter, List<RaycastHit2D> results, [DefaultValue("Mathf.Infinity")] float distance = float.PositiveInfinity)
		{
			return this.CastFromFiltered_Internal(position, angle, direction, distance, false, contactFilter, results);
		}

		public int Overlap(ContactFilter2D contactFilter, [Out] Collider2D[] results)
		{
			return this.OverlapArray_Internal(contactFilter, results);
		}

		public int Overlap(List<Collider2D> results)
		{
			return this.OverlapList_Internal(results);
		}

		public int Overlap(ContactFilter2D contactFilter, List<Collider2D> results)
		{
			return this.OverlapFilteredList_Internal(contactFilter, results);
		}

		public int Overlap(Vector2 position, float angle, List<Collider2D> results)
		{
			return this.OverlapFromList_Internal(position, angle, results);
		}

		public int Overlap(Vector2 position, float angle, ContactFilter2D contactFilter, List<Collider2D> results)
		{
			return this.OverlapFromFilteredList_Internal(position, angle, contactFilter, results);
		}

		[NativeMethod("GetAttachedCollidersArray_Binding")]
		private int GetAttachedCollidersArray_Internal([UnityMarshalAs(NativeType.ScriptingObjectPtr)] [NotNull] Collider2D[] results, bool findTriggers)
		{
			if (results == null)
			{
				ThrowHelper.ThrowArgumentNullException(results, "results");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Rigidbody2D.GetAttachedCollidersArray_Internal_Injected(intPtr, results, findTriggers);
		}

		[NativeMethod("GetAttachedCollidersList_Binding")]
		private int GetAttachedCollidersList_Internal([NotNull] List<Collider2D> results, bool findTriggers)
		{
			if (results == null)
			{
				ThrowHelper.ThrowArgumentNullException(results, "results");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Rigidbody2D.GetAttachedCollidersList_Internal_Injected(intPtr, results, findTriggers);
		}

		[NativeMethod("GetShapes_Binding")]
		private int GetShapes_Internal(ref PhysicsShapeGroup2D.GroupState physicsShapeGroupState)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Rigidbody2D.GetShapes_Internal_Injected(intPtr, ref physicsShapeGroupState);
		}

		[NativeMethod("CastArray_Binding")]
		private unsafe int CastArray_Internal(Vector2 direction, float distance, bool checkIgnoreColliders, [NotNull] RaycastHit2D[] results)
		{
			if (results == null)
			{
				ThrowHelper.ThrowArgumentNullException(results, "results");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<RaycastHit2D> span = new Span<RaycastHit2D>(results);
			int num;
			fixed (RaycastHit2D* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				num = Rigidbody2D.CastArray_Internal_Injected(intPtr, ref direction, distance, checkIgnoreColliders, ref managedSpanWrapper);
			}
			return num;
		}

		[NativeMethod("CastList_Binding")]
		private unsafe int CastList_Internal(Vector2 direction, float distance, bool checkIgnoreColliders, [NotNull] List<RaycastHit2D> results)
		{
			if (results == null)
			{
				ThrowHelper.ThrowArgumentNullException(results, "results");
			}
			int num;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				fixed (RaycastHit2D[] array = NoAllocHelpers.ExtractArrayFromList<RaycastHit2D>(results))
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					BlittableListWrapper blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, results.Count);
					num = Rigidbody2D.CastList_Internal_Injected(intPtr, ref direction, distance, checkIgnoreColliders, ref blittableListWrapper);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<RaycastHit2D>(results);
			}
			return num;
		}

		[NativeMethod("CastFilteredArray_Binding")]
		private unsafe int CastFilteredArray_Internal(Vector2 direction, float distance, bool checkIgnoreColliders, ContactFilter2D contactFilter, [NotNull] RaycastHit2D[] results)
		{
			if (results == null)
			{
				ThrowHelper.ThrowArgumentNullException(results, "results");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<RaycastHit2D> span = new Span<RaycastHit2D>(results);
			int num;
			fixed (RaycastHit2D* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				num = Rigidbody2D.CastFilteredArray_Internal_Injected(intPtr, ref direction, distance, checkIgnoreColliders, ref contactFilter, ref managedSpanWrapper);
			}
			return num;
		}

		[NativeMethod("CastFilteredList_Binding")]
		private unsafe int CastFilteredList_Internal(Vector2 direction, float distance, bool checkIgnoreColliders, ContactFilter2D contactFilter, [NotNull] List<RaycastHit2D> results)
		{
			if (results == null)
			{
				ThrowHelper.ThrowArgumentNullException(results, "results");
			}
			int num;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				fixed (RaycastHit2D[] array = NoAllocHelpers.ExtractArrayFromList<RaycastHit2D>(results))
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					BlittableListWrapper blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, results.Count);
					num = Rigidbody2D.CastFilteredList_Internal_Injected(intPtr, ref direction, distance, checkIgnoreColliders, ref contactFilter, ref blittableListWrapper);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<RaycastHit2D>(results);
			}
			return num;
		}

		[NativeMethod("CastFrom_Binding")]
		private unsafe int CastFrom_Internal(Vector2 position, float angle, Vector2 direction, float distance, bool checkIgnoreColliders, [NotNull] List<RaycastHit2D> results)
		{
			if (results == null)
			{
				ThrowHelper.ThrowArgumentNullException(results, "results");
			}
			int num;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				fixed (RaycastHit2D[] array = NoAllocHelpers.ExtractArrayFromList<RaycastHit2D>(results))
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					BlittableListWrapper blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, results.Count);
					num = Rigidbody2D.CastFrom_Internal_Injected(intPtr, ref position, angle, ref direction, distance, checkIgnoreColliders, ref blittableListWrapper);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<RaycastHit2D>(results);
			}
			return num;
		}

		[NativeMethod("CastFromFiltered_Binding")]
		private unsafe int CastFromFiltered_Internal(Vector2 position, float angle, Vector2 direction, float distance, bool checkIgnoreColliders, ContactFilter2D contactFilter, [NotNull] List<RaycastHit2D> results)
		{
			if (results == null)
			{
				ThrowHelper.ThrowArgumentNullException(results, "results");
			}
			int num;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				fixed (RaycastHit2D[] array = NoAllocHelpers.ExtractArrayFromList<RaycastHit2D>(results))
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					BlittableListWrapper blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, results.Count);
					num = Rigidbody2D.CastFromFiltered_Internal_Injected(intPtr, ref position, angle, ref direction, distance, checkIgnoreColliders, ref contactFilter, ref blittableListWrapper);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<RaycastHit2D>(results);
			}
			return num;
		}

		[NativeMethod("OverlapArray_Binding")]
		private int OverlapArray_Internal(ContactFilter2D contactFilter, [NotNull] [UnityMarshalAs(NativeType.ScriptingObjectPtr)] Collider2D[] results)
		{
			if (results == null)
			{
				ThrowHelper.ThrowArgumentNullException(results, "results");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Rigidbody2D.OverlapArray_Internal_Injected(intPtr, ref contactFilter, results);
		}

		[NativeMethod("OverlapList_Binding")]
		private int OverlapList_Internal([NotNull] List<Collider2D> results)
		{
			if (results == null)
			{
				ThrowHelper.ThrowArgumentNullException(results, "results");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Rigidbody2D.OverlapList_Internal_Injected(intPtr, results);
		}

		[NativeMethod("OverlapFilteredList_Binding")]
		private int OverlapFilteredList_Internal(ContactFilter2D contactFilter, [NotNull] List<Collider2D> results)
		{
			if (results == null)
			{
				ThrowHelper.ThrowArgumentNullException(results, "results");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Rigidbody2D.OverlapFilteredList_Internal_Injected(intPtr, ref contactFilter, results);
		}

		[NativeMethod("OverlapFromList_Binding")]
		private int OverlapFromList_Internal(Vector2 position, float angle, [NotNull] List<Collider2D> results)
		{
			if (results == null)
			{
				ThrowHelper.ThrowArgumentNullException(results, "results");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Rigidbody2D.OverlapFromList_Internal_Injected(intPtr, ref position, angle, results);
		}

		[NativeMethod("OverlapFromFilteredList_Binding")]
		private int OverlapFromFilteredList_Internal(Vector2 position, float angle, ContactFilter2D contactFilter, [NotNull] List<Collider2D> results)
		{
			if (results == null)
			{
				ThrowHelper.ThrowArgumentNullException(results, "results");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Rigidbody2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Rigidbody2D.OverlapFromFilteredList_Internal_Injected(intPtr, ref position, angle, ref contactFilter, results);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("OverlapCollider has been deprecated. Please use Overlap (UnityUpgradable) -> Overlap(*)", false)]
		[ExcludeFromDocs]
		public int OverlapCollider(ContactFilter2D contactFilter, [Out] Collider2D[] results)
		{
			return this.Overlap(contactFilter, results);
		}

		[ExcludeFromDocs]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("OverlapCollider has been deprecated. Please use Overlap (UnityUpgradable) -> Overlap(*)", false)]
		public int OverlapCollider(ContactFilter2D contactFilter, List<Collider2D> results)
		{
			return this.Overlap(contactFilter, results);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Rigidbody2D.fixedAngle is obsolete. Use Rigidbody2D.constraints instead.", true)]
		[ExcludeFromDocs]
		public bool fixedAngle
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[ExcludeFromDocs]
		[Obsolete("isKinematic has been deprecated. Please use bodyType.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool isKinematic
		{
			get
			{
				return this.bodyType == RigidbodyType2D.Kinematic;
			}
			set
			{
				this.bodyType = (value ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("drag has been deprecated. Please use linearDamping. (UnityUpgradable) -> linearDamping", false)]
		[ExcludeFromDocs]
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

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("angularDrag has been deprecated. Please use angularDamping. (UnityUpgradable) -> angularDamping", false)]
		[ExcludeFromDocs]
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
		[Obsolete("velocity has been deprecated. Please use linearVelocity. (UnityUpgradable) -> linearVelocity", false)]
		[ExcludeFromDocs]
		public Vector2 velocity
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

		[Obsolete("velocityX has been deprecated. Please use linearVelocityX. (UnityUpgradable) -> linearVelocityX", false)]
		[ExcludeFromDocs]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public float velocityX
		{
			get
			{
				return this.linearVelocityX;
			}
			set
			{
				this.linearVelocityX = value;
			}
		}

		[ExcludeFromDocs]
		[Obsolete("velocityY has been deprecated. Please use linearVelocityY (UnityUpgradable) -> linearVelocityY", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public float velocityY
		{
			get
			{
				return this.linearVelocityY;
			}
			set
			{
				this.linearVelocityY = value;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_position_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_position_Injected(IntPtr _unity_self, [In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_rotation_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_rotation_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetRotation_Angle_Injected(IntPtr _unity_self, float angle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetRotation_Quaternion_Injected(IntPtr _unity_self, [In] ref Quaternion rotation);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MovePosition_Injected(IntPtr _unity_self, [In] ref Vector2 position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MoveRotation_Angle_Injected(IntPtr _unity_self, float angle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MoveRotation_Quaternion_Injected(IntPtr _unity_self, [In] ref Quaternion rotation);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MovePositionAndRotation_Injected(IntPtr _unity_self, [In] ref Vector2 position, float angle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MovePositionAndRotation_Quaternion_Injected(IntPtr _unity_self, [In] ref Vector2 position, [In] ref Quaternion rotation);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Slide_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 velocity, float deltaTime, [In] ref Rigidbody2D.SlideMovement slideMovement, out Rigidbody2D.SlideResults ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_linearVelocity_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_linearVelocity_Injected(IntPtr _unity_self, [In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_linearVelocityX_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_linearVelocityX_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_linearVelocityY_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_linearVelocityY_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_angularVelocity_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_angularVelocity_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useAutoMass_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useAutoMass_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_mass_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_mass_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_sharedMaterial_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sharedMaterial_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_centerOfMass_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_centerOfMass_Injected(IntPtr _unity_self, [In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_worldCenterOfMass_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_inertia_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_inertia_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_linearDamping_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_linearDamping_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_angularDamping_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_angularDamping_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_gravityScale_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_gravityScale_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RigidbodyType2D get_bodyType_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_bodyType_Injected(IntPtr _unity_self, RigidbodyType2D value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetDragBehaviour_Injected(IntPtr _unity_self, bool dragged);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useFullKinematicContacts_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useFullKinematicContacts_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_freezeRotation_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_freezeRotation_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RigidbodyConstraints2D get_constraints_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_constraints_Injected(IntPtr _unity_self, RigidbodyConstraints2D value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsSleeping_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsAwake_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Sleep_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void WakeUp_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_simulated_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_simulated_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RigidbodyInterpolation2D get_interpolation_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_interpolation_Injected(IntPtr _unity_self, RigidbodyInterpolation2D value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RigidbodySleepMode2D get_sleepMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sleepMode_Injected(IntPtr _unity_self, RigidbodySleepMode2D value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern CollisionDetectionMode2D get_collisionDetectionMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_collisionDetectionMode_Injected(IntPtr _unity_self, CollisionDetectionMode2D value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetAttachedColliderCount_Internal_Injected(IntPtr _unity_self, bool findTriggers);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_totalForce_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_totalForce_Injected(IntPtr _unity_self, [In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_totalTorque_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_totalTorque_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_excludeLayers_Injected(IntPtr _unity_self, out LayerMask ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_excludeLayers_Injected(IntPtr _unity_self, [In] ref LayerMask value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_includeLayers_Injected(IntPtr _unity_self, out LayerMask ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_includeLayers_Injected(IntPtr _unity_self, [In] ref LayerMask value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_localToWorldMatrix_Injected(IntPtr _unity_self, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsTouching_Injected(IntPtr _unity_self, IntPtr collider);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsTouching_OtherColliderWithFilter_Internal_Injected(IntPtr _unity_self, IntPtr collider, [In] ref ContactFilter2D contactFilter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsTouching_AnyColliderWithFilter_Internal_Injected(IntPtr _unity_self, [In] ref ContactFilter2D contactFilter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool OverlapPoint_Injected(IntPtr _unity_self, [In] ref Vector2 point);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Distance_Internal_Injected(IntPtr _unity_self, IntPtr collider, out ColliderDistance2D ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DistanceFrom_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 thisPosition, float thisAngle, IntPtr collider, [In] ref Vector2 position, float angle, out ColliderDistance2D ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddForce_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 force, [DefaultValue("ForceMode2D.Force")] ForceMode2D mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddRelativeForce_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 relativeForce, [DefaultValue("ForceMode2D.Force")] ForceMode2D mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddForceAtPosition_Injected(IntPtr _unity_self, [In] ref Vector2 force, [In] ref Vector2 position, [DefaultValue("ForceMode2D.Force")] ForceMode2D mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddTorque_Injected(IntPtr _unity_self, float torque, [DefaultValue("ForceMode2D.Force")] ForceMode2D mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPoint_Injected(IntPtr _unity_self, [In] ref Vector2 point, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetRelativePoint_Injected(IntPtr _unity_self, [In] ref Vector2 relativePoint, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetVector_Injected(IntPtr _unity_self, [In] ref Vector2 vector, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetRelativeVector_Injected(IntPtr _unity_self, [In] ref Vector2 relativeVector, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPointVelocity_Injected(IntPtr _unity_self, [In] ref Vector2 point, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetRelativePointVelocity_Injected(IntPtr _unity_self, [In] ref Vector2 relativePoint, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetAttachedCollidersArray_Internal_Injected(IntPtr _unity_self, Collider2D[] results, bool findTriggers);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetAttachedCollidersList_Internal_Injected(IntPtr _unity_self, List<Collider2D> results, bool findTriggers);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetShapes_Internal_Injected(IntPtr _unity_self, ref PhysicsShapeGroup2D.GroupState physicsShapeGroupState);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int CastArray_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 direction, float distance, bool checkIgnoreColliders, ref ManagedSpanWrapper results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int CastList_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 direction, float distance, bool checkIgnoreColliders, ref BlittableListWrapper results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int CastFilteredArray_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 direction, float distance, bool checkIgnoreColliders, [In] ref ContactFilter2D contactFilter, ref ManagedSpanWrapper results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int CastFilteredList_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 direction, float distance, bool checkIgnoreColliders, [In] ref ContactFilter2D contactFilter, ref BlittableListWrapper results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int CastFrom_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 position, float angle, [In] ref Vector2 direction, float distance, bool checkIgnoreColliders, ref BlittableListWrapper results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int CastFromFiltered_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 position, float angle, [In] ref Vector2 direction, float distance, bool checkIgnoreColliders, [In] ref ContactFilter2D contactFilter, ref BlittableListWrapper results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int OverlapArray_Internal_Injected(IntPtr _unity_self, [In] ref ContactFilter2D contactFilter, Collider2D[] results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int OverlapList_Internal_Injected(IntPtr _unity_self, List<Collider2D> results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int OverlapFilteredList_Internal_Injected(IntPtr _unity_self, [In] ref ContactFilter2D contactFilter, List<Collider2D> results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int OverlapFromList_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 position, float angle, List<Collider2D> results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int OverlapFromFilteredList_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 position, float angle, [In] ref ContactFilter2D contactFilter, List<Collider2D> results);

		[NativeHeader(Header = "Modules/Physics2D/Public/Rigidbody2D.h")]
		[Serializable]
		public struct SlideMovement
		{
			public SlideMovement()
			{
				this.maxIterations = 3;
				this.surfaceSlideAngle = 90f;
				this.gravitySlipAngle = 90f;
				this.surfaceUp = Vector2.up;
				this.surfaceAnchor = Vector2.down;
				this.gravity = new Vector2(0f, -9.81f);
				this.startPosition = Vector2.zero;
				this.selectedCollider = null;
				this.useStartPosition = false;
				this.useNoMove = false;
				this.useSimulationMove = false;
				this.useAttachedTriggers = false;
				this.useLayerMask = false;
				this.layerMask = -1;
			}

			public int maxIterations { readonly get; set; }

			public float surfaceSlideAngle { readonly get; set; }

			public float gravitySlipAngle { readonly get; set; }

			public Vector2 surfaceUp { readonly get; set; }

			public Vector2 surfaceAnchor { readonly get; set; }

			public Vector2 gravity { readonly get; set; }

			public Vector2 startPosition { readonly get; set; }

			public Collider2D selectedCollider { readonly get; set; }

			public LayerMask layerMask { readonly get; set; }

			public bool useLayerMask { readonly get; set; }

			public bool useStartPosition { readonly get; set; }

			public bool useNoMove { readonly get; set; }

			public bool useSimulationMove { readonly get; set; }

			public bool useAttachedTriggers { readonly get; set; }

			public void SetLayerMask(LayerMask mask)
			{
				this.layerMask = mask;
				this.useLayerMask = true;
			}

			public void SetStartPosition(Vector2 position)
			{
				this.startPosition = position;
				this.useStartPosition = true;
			}
		}

		[NativeHeader(Header = "Modules/Physics2D/Public/Rigidbody2D.h")]
		[Serializable]
		public struct SlideResults
		{
			public Vector2 remainingVelocity { readonly get; set; }

			public Vector2 position { readonly get; set; }

			public int iterationsUsed { readonly get; set; }

			public RaycastHit2D slideHit { readonly get; set; }

			public RaycastHit2D surfaceHit { readonly get; set; }
		}
	}
}
