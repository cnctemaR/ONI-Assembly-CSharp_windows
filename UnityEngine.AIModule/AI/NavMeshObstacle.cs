using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.AI
{
	[NativeHeader("Modules/AI/Components/NavMeshObstacle.bindings.h")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.ai.navigation@2.0/manual/NavMeshObstacle.html")]
	[MovedFrom("UnityEngine")]
	public sealed class NavMeshObstacle : Behaviour
	{
		public float height
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshObstacle>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return NavMeshObstacle.get_height_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshObstacle>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				NavMeshObstacle.set_height_Injected(intPtr, value);
			}
		}

		public float radius
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshObstacle>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return NavMeshObstacle.get_radius_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshObstacle>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				NavMeshObstacle.set_radius_Injected(intPtr, value);
			}
		}

		public Vector3 velocity
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshObstacle>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				NavMeshObstacle.get_velocity_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshObstacle>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				NavMeshObstacle.set_velocity_Injected(intPtr, ref value);
			}
		}

		public bool carving
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshObstacle>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return NavMeshObstacle.get_carving_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshObstacle>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				NavMeshObstacle.set_carving_Injected(intPtr, value);
			}
		}

		public bool carveOnlyStationary
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshObstacle>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return NavMeshObstacle.get_carveOnlyStationary_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshObstacle>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				NavMeshObstacle.set_carveOnlyStationary_Injected(intPtr, value);
			}
		}

		[NativeProperty("MoveThreshold")]
		public float carvingMoveThreshold
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshObstacle>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return NavMeshObstacle.get_carvingMoveThreshold_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshObstacle>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				NavMeshObstacle.set_carvingMoveThreshold_Injected(intPtr, value);
			}
		}

		[NativeProperty("TimeToStationary")]
		public float carvingTimeToStationary
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshObstacle>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return NavMeshObstacle.get_carvingTimeToStationary_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshObstacle>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				NavMeshObstacle.set_carvingTimeToStationary_Injected(intPtr, value);
			}
		}

		public NavMeshObstacleShape shape
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshObstacle>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return NavMeshObstacle.get_shape_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshObstacle>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				NavMeshObstacle.set_shape_Injected(intPtr, value);
			}
		}

		public Vector3 center
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshObstacle>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				NavMeshObstacle.get_center_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshObstacle>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				NavMeshObstacle.set_center_Injected(intPtr, ref value);
			}
		}

		public Vector3 size
		{
			[FreeFunction("NavMeshObstacleScriptBindings::GetSize", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshObstacle>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				NavMeshObstacle.get_size_Injected(intPtr, out vector);
				return vector;
			}
			[FreeFunction("NavMeshObstacleScriptBindings::SetSize", HasExplicitThis = true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshObstacle>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				NavMeshObstacle.set_size_Injected(intPtr, ref value);
			}
		}

		[FreeFunction("NavMeshObstacleScriptBindings::FitExtents", HasExplicitThis = true)]
		internal void FitExtents()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<NavMeshObstacle>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			NavMeshObstacle.FitExtents_Injected(intPtr);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_height_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_height_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_radius_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_radius_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_velocity_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_velocity_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_carving_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_carving_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_carveOnlyStationary_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_carveOnlyStationary_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_carvingMoveThreshold_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_carvingMoveThreshold_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_carvingTimeToStationary_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_carvingTimeToStationary_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern NavMeshObstacleShape get_shape_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_shape_Injected(IntPtr _unity_self, NavMeshObstacleShape value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_center_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_center_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_size_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_size_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FitExtents_Injected(IntPtr _unity_self);
	}
}
