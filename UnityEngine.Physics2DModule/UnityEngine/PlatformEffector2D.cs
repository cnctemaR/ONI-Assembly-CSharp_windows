using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Modules/Physics2D/PlatformEffector2D.h")]
	public class PlatformEffector2D : Effector2D
	{
		public bool useOneWay
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlatformEffector2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return PlatformEffector2D.get_useOneWay_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlatformEffector2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				PlatformEffector2D.set_useOneWay_Injected(intPtr, value);
			}
		}

		public bool useOneWayGrouping
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlatformEffector2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return PlatformEffector2D.get_useOneWayGrouping_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlatformEffector2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				PlatformEffector2D.set_useOneWayGrouping_Injected(intPtr, value);
			}
		}

		public bool useSideFriction
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlatformEffector2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return PlatformEffector2D.get_useSideFriction_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlatformEffector2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				PlatformEffector2D.set_useSideFriction_Injected(intPtr, value);
			}
		}

		public bool useSideBounce
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlatformEffector2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return PlatformEffector2D.get_useSideBounce_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlatformEffector2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				PlatformEffector2D.set_useSideBounce_Injected(intPtr, value);
			}
		}

		public float surfaceArc
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlatformEffector2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return PlatformEffector2D.get_surfaceArc_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlatformEffector2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				PlatformEffector2D.set_surfaceArc_Injected(intPtr, value);
			}
		}

		public float sideArc
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlatformEffector2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return PlatformEffector2D.get_sideArc_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlatformEffector2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				PlatformEffector2D.set_sideArc_Injected(intPtr, value);
			}
		}

		public float rotationalOffset
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlatformEffector2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return PlatformEffector2D.get_rotationalOffset_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<PlatformEffector2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				PlatformEffector2D.set_rotationalOffset_Injected(intPtr, value);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("PlatformEffector2D.oneWay has been obsolete. Use PlatformEffector2D.useOneWay instead (UnityUpgradable) -> useOneWay", true)]
		public bool oneWay
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
		[Obsolete("PlatformEffector2D.sideFriction has been obsolete. Use PlatformEffector2D.useSideFriction instead (UnityUpgradable) -> useSideFriction", true)]
		public bool sideFriction
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
		[Obsolete("PlatformEffector2D.sideBounce has been obsolete. Use PlatformEffector2D.useSideBounce instead (UnityUpgradable) -> useSideBounce", true)]
		public bool sideBounce
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

		[Obsolete("PlatformEffector2D.sideAngleVariance has been obsolete. Use PlatformEffector2D.sideArc instead (UnityUpgradable) -> sideArc", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public float sideAngleVariance
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
		private static extern bool get_useOneWay_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useOneWay_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useOneWayGrouping_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useOneWayGrouping_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useSideFriction_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useSideFriction_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useSideBounce_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useSideBounce_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_surfaceArc_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_surfaceArc_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_sideArc_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sideArc_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_rotationalOffset_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_rotationalOffset_Injected(IntPtr _unity_self, float value);
	}
}
