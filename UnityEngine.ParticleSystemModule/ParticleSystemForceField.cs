using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Modules/ParticleSystem/ParticleSystemForceFieldManager.h")]
	[RequireComponent(typeof(Transform))]
	[NativeHeader("ParticleSystemScriptingClasses.h")]
	[NativeHeader("Modules/ParticleSystem/ParticleSystem.h")]
	[NativeHeader("Modules/ParticleSystem/ParticleSystemForceField.h")]
	[NativeHeader("Modules/ParticleSystem/ScriptBindings/ParticleSystemScriptBindings.h")]
	public class ParticleSystemForceField : Behaviour
	{
		[NativeName("ForceShape")]
		public ParticleSystemForceFieldShape shape
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystemForceField.get_shape_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemForceField.set_shape_Injected(intPtr, value);
			}
		}

		public float startRange
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystemForceField.get_startRange_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemForceField.set_startRange_Injected(intPtr, value);
			}
		}

		public float endRange
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystemForceField.get_endRange_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemForceField.set_endRange_Injected(intPtr, value);
			}
		}

		public float length
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystemForceField.get_length_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemForceField.set_length_Injected(intPtr, value);
			}
		}

		public float gravityFocus
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystemForceField.get_gravityFocus_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemForceField.set_gravityFocus_Injected(intPtr, value);
			}
		}

		public Vector2 rotationRandomness
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				ParticleSystemForceField.get_rotationRandomness_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemForceField.set_rotationRandomness_Injected(intPtr, ref value);
			}
		}

		public bool multiplyDragByParticleSize
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystemForceField.get_multiplyDragByParticleSize_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemForceField.set_multiplyDragByParticleSize_Injected(intPtr, value);
			}
		}

		public bool multiplyDragByParticleVelocity
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystemForceField.get_multiplyDragByParticleVelocity_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemForceField.set_multiplyDragByParticleVelocity_Injected(intPtr, value);
			}
		}

		public Texture3D vectorField
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Texture3D>(ParticleSystemForceField.get_vectorField_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemForceField.set_vectorField_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Texture3D>(value));
			}
		}

		public ParticleSystem.MinMaxCurve directionX
		{
			get
			{
				return this.directionXBlittable;
			}
			set
			{
				this.directionXBlittable = value;
			}
		}

		[NativeName("DirectionX")]
		private ParticleSystem.MinMaxCurveBlittable directionXBlittable
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
				ParticleSystemForceField.get_directionXBlittable_Injected(intPtr, out minMaxCurveBlittable);
				return minMaxCurveBlittable;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemForceField.set_directionXBlittable_Injected(intPtr, ref value);
			}
		}

		public ParticleSystem.MinMaxCurve directionY
		{
			get
			{
				return this.directionYBlittable;
			}
			set
			{
				this.directionYBlittable = value;
			}
		}

		[NativeName("DirectionY")]
		private ParticleSystem.MinMaxCurveBlittable directionYBlittable
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
				ParticleSystemForceField.get_directionYBlittable_Injected(intPtr, out minMaxCurveBlittable);
				return minMaxCurveBlittable;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemForceField.set_directionYBlittable_Injected(intPtr, ref value);
			}
		}

		public ParticleSystem.MinMaxCurve directionZ
		{
			get
			{
				return this.directionZBlittable;
			}
			set
			{
				this.directionZBlittable = value;
			}
		}

		[NativeName("DirectionZ")]
		private ParticleSystem.MinMaxCurveBlittable directionZBlittable
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
				ParticleSystemForceField.get_directionZBlittable_Injected(intPtr, out minMaxCurveBlittable);
				return minMaxCurveBlittable;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemForceField.set_directionZBlittable_Injected(intPtr, ref value);
			}
		}

		public ParticleSystem.MinMaxCurve gravity
		{
			get
			{
				return this.gravityBlittable;
			}
			set
			{
				this.gravityBlittable = value;
			}
		}

		[NativeName("Gravity")]
		private ParticleSystem.MinMaxCurveBlittable gravityBlittable
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
				ParticleSystemForceField.get_gravityBlittable_Injected(intPtr, out minMaxCurveBlittable);
				return minMaxCurveBlittable;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemForceField.set_gravityBlittable_Injected(intPtr, ref value);
			}
		}

		public ParticleSystem.MinMaxCurve rotationSpeed
		{
			get
			{
				return this.rotationSpeedBlittable;
			}
			set
			{
				this.rotationSpeedBlittable = value;
			}
		}

		[NativeName("RotationSpeed")]
		private ParticleSystem.MinMaxCurveBlittable rotationSpeedBlittable
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
				ParticleSystemForceField.get_rotationSpeedBlittable_Injected(intPtr, out minMaxCurveBlittable);
				return minMaxCurveBlittable;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemForceField.set_rotationSpeedBlittable_Injected(intPtr, ref value);
			}
		}

		public ParticleSystem.MinMaxCurve rotationAttraction
		{
			get
			{
				return this.rotationAttractionBlittable;
			}
			set
			{
				this.rotationAttractionBlittable = value;
			}
		}

		[NativeName("RotationAttraction")]
		private ParticleSystem.MinMaxCurveBlittable rotationAttractionBlittable
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
				ParticleSystemForceField.get_rotationAttractionBlittable_Injected(intPtr, out minMaxCurveBlittable);
				return minMaxCurveBlittable;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemForceField.set_rotationAttractionBlittable_Injected(intPtr, ref value);
			}
		}

		public ParticleSystem.MinMaxCurve drag
		{
			get
			{
				return this.dragBlittable;
			}
			set
			{
				this.dragBlittable = value;
			}
		}

		[NativeName("Drag")]
		private ParticleSystem.MinMaxCurveBlittable dragBlittable
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
				ParticleSystemForceField.get_dragBlittable_Injected(intPtr, out minMaxCurveBlittable);
				return minMaxCurveBlittable;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemForceField.set_dragBlittable_Injected(intPtr, ref value);
			}
		}

		public ParticleSystem.MinMaxCurve vectorFieldSpeed
		{
			get
			{
				return this.vectorFieldSpeedBlittable;
			}
			set
			{
				this.vectorFieldSpeedBlittable = value;
			}
		}

		[NativeName("VectorFieldSpeed")]
		private ParticleSystem.MinMaxCurveBlittable vectorFieldSpeedBlittable
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
				ParticleSystemForceField.get_vectorFieldSpeedBlittable_Injected(intPtr, out minMaxCurveBlittable);
				return minMaxCurveBlittable;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemForceField.set_vectorFieldSpeedBlittable_Injected(intPtr, ref value);
			}
		}

		public ParticleSystem.MinMaxCurve vectorFieldAttraction
		{
			get
			{
				return this.vectorFieldAttractionBlittable;
			}
			set
			{
				this.vectorFieldAttractionBlittable = value;
			}
		}

		[NativeName("VectorFieldAttraction")]
		private ParticleSystem.MinMaxCurveBlittable vectorFieldAttractionBlittable
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystem.MinMaxCurveBlittable minMaxCurveBlittable;
				ParticleSystemForceField.get_vectorFieldAttractionBlittable_Injected(intPtr, out minMaxCurveBlittable);
				return minMaxCurveBlittable;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemForceField>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemForceField.set_vectorFieldAttractionBlittable_Injected(intPtr, ref value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ParticleSystemForceFieldShape get_shape_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_shape_Injected(IntPtr _unity_self, ParticleSystemForceFieldShape value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_startRange_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_startRange_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_endRange_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_endRange_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_length_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_length_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_gravityFocus_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_gravityFocus_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_rotationRandomness_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_rotationRandomness_Injected(IntPtr _unity_self, [In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_multiplyDragByParticleSize_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_multiplyDragByParticleSize_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_multiplyDragByParticleVelocity_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_multiplyDragByParticleVelocity_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_vectorField_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_vectorField_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_directionXBlittable_Injected(IntPtr _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_directionXBlittable_Injected(IntPtr _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_directionYBlittable_Injected(IntPtr _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_directionYBlittable_Injected(IntPtr _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_directionZBlittable_Injected(IntPtr _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_directionZBlittable_Injected(IntPtr _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_gravityBlittable_Injected(IntPtr _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_gravityBlittable_Injected(IntPtr _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_rotationSpeedBlittable_Injected(IntPtr _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_rotationSpeedBlittable_Injected(IntPtr _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_rotationAttractionBlittable_Injected(IntPtr _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_rotationAttractionBlittable_Injected(IntPtr _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_dragBlittable_Injected(IntPtr _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_dragBlittable_Injected(IntPtr _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_vectorFieldSpeedBlittable_Injected(IntPtr _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_vectorFieldSpeedBlittable_Injected(IntPtr _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_vectorFieldAttractionBlittable_Injected(IntPtr _unity_self, out ParticleSystem.MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_vectorFieldAttractionBlittable_Injected(IntPtr _unity_self, [In] ref ParticleSystem.MinMaxCurveBlittable value);
	}
}
