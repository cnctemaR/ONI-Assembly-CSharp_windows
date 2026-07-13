using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.AI
{
	[MovedFrom("UnityEngine")]
	[Obsolete("The OffMeshLink component is no longer supported and will be removed. Use NavMeshLink instead.")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.ai.navigation@2.0/manual/OffMeshLink.html")]
	public sealed class OffMeshLink : Behaviour
	{
		[Obsolete("activated has been deprecated together with the class. Declare the object as NavMeshLink and use activated as before.")]
		public bool activated
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<OffMeshLink>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return OffMeshLink.get_activated_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<OffMeshLink>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				OffMeshLink.set_activated_Injected(intPtr, value);
			}
		}

		[Obsolete("occupied has been deprecated together with the class. Declare the object as NavMeshLink and use occupied as before.")]
		public bool occupied
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<OffMeshLink>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return OffMeshLink.get_occupied_Injected(intPtr);
			}
		}

		[Obsolete("costOverride has been deprecated together with the class. Declare the object as NavMeshLink and use costModifier instead.")]
		public float costOverride
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<OffMeshLink>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return OffMeshLink.get_costOverride_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<OffMeshLink>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				OffMeshLink.set_costOverride_Injected(intPtr, value);
			}
		}

		[Obsolete("biDirectional has been deprecated together with the class. Declare the object as NavMeshLink and use bidirectional instead.")]
		public bool biDirectional
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<OffMeshLink>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return OffMeshLink.get_biDirectional_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<OffMeshLink>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				OffMeshLink.set_biDirectional_Injected(intPtr, value);
			}
		}

		[Obsolete("UpdatePositions() has been deprecated together with the class. Declare the object as NavMeshLink and use UpdateLink() instead.")]
		public void UpdatePositions()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<OffMeshLink>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			OffMeshLink.UpdatePositions_Injected(intPtr);
		}

		[Obsolete("navMeshLayer has been deprecated together with the class. Declare the object as NavMeshLink and use area instead. (UnityUpgradable) -> area")]
		public int navMeshLayer
		{
			get
			{
				return this.area;
			}
			set
			{
				this.area = value;
			}
		}

		[Obsolete("area has been deprecated together with the class. Declare the object as NavMeshLink and use area as before.")]
		public int area
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<OffMeshLink>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return OffMeshLink.get_area_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<OffMeshLink>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				OffMeshLink.set_area_Injected(intPtr, value);
			}
		}

		[Obsolete("autoUpdatePositions has been deprecated together with the class. Declare the object as NavMeshLink and use autoUpdate instead.")]
		public bool autoUpdatePositions
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<OffMeshLink>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return OffMeshLink.get_autoUpdatePositions_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<OffMeshLink>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				OffMeshLink.set_autoUpdatePositions_Injected(intPtr, value);
			}
		}

		[Obsolete("startTransform has been deprecated together with the class. Declare the object as NavMeshLink and use startTransform as before.")]
		public Transform startTransform
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<OffMeshLink>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Transform>(OffMeshLink.get_startTransform_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<OffMeshLink>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				OffMeshLink.set_startTransform_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Transform>(value));
			}
		}

		[Obsolete("endTransform has been deprecated together with the class. Declare the object as NavMeshLink and use endTransform as before.")]
		public Transform endTransform
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<OffMeshLink>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Transform>(OffMeshLink.get_endTransform_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<OffMeshLink>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				OffMeshLink.set_endTransform_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Transform>(value));
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_activated_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_activated_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_occupied_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_costOverride_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_costOverride_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_biDirectional_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_biDirectional_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UpdatePositions_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_area_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_area_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_autoUpdatePositions_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_autoUpdatePositions_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_startTransform_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_startTransform_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_endTransform_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_endTransform_Injected(IntPtr _unity_self, IntPtr value);
	}
}
