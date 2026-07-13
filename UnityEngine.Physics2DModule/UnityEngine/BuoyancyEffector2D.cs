using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Modules/Physics2D/BuoyancyEffector2D.h")]
	public class BuoyancyEffector2D : Effector2D
	{
		public float surfaceLevel
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BuoyancyEffector2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return BuoyancyEffector2D.get_surfaceLevel_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BuoyancyEffector2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BuoyancyEffector2D.set_surfaceLevel_Injected(intPtr, value);
			}
		}

		public float density
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BuoyancyEffector2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return BuoyancyEffector2D.get_density_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BuoyancyEffector2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BuoyancyEffector2D.set_density_Injected(intPtr, value);
			}
		}

		public float linearDamping
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BuoyancyEffector2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return BuoyancyEffector2D.get_linearDamping_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BuoyancyEffector2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BuoyancyEffector2D.set_linearDamping_Injected(intPtr, value);
			}
		}

		public float angularDamping
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BuoyancyEffector2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return BuoyancyEffector2D.get_angularDamping_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BuoyancyEffector2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BuoyancyEffector2D.set_angularDamping_Injected(intPtr, value);
			}
		}

		public float flowAngle
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BuoyancyEffector2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return BuoyancyEffector2D.get_flowAngle_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BuoyancyEffector2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BuoyancyEffector2D.set_flowAngle_Injected(intPtr, value);
			}
		}

		public float flowMagnitude
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BuoyancyEffector2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return BuoyancyEffector2D.get_flowMagnitude_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BuoyancyEffector2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BuoyancyEffector2D.set_flowMagnitude_Injected(intPtr, value);
			}
		}

		public float flowVariation
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BuoyancyEffector2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return BuoyancyEffector2D.get_flowVariation_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BuoyancyEffector2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BuoyancyEffector2D.set_flowVariation_Injected(intPtr, value);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("BuoyancyEffector2D.drag has been obsolete. Use BuoyancyEffector2D.linearDamping instead (UnityUpgradable) -> linearDamping", true)]
		public float drag
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

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("BuoyancyEffector2D.angularDrag has been obsolete. Use BuoyancyEffector2D.angularDamping instead (UnityUpgradable) -> angularDamping", true)]
		public float angularDrag
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_surfaceLevel_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_surfaceLevel_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_density_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_density_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_linearDamping_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_linearDamping_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_angularDamping_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_angularDamping_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_flowAngle_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_flowAngle_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_flowMagnitude_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_flowMagnitude_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_flowVariation_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_flowVariation_Injected(IntPtr _unity_self, float value);
	}
}
