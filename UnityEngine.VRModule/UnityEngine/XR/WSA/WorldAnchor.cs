using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.XR.WSA
{
	[RequireComponent(typeof(Transform))]
	[MovedFrom("UnityEngine.VR.WSA")]
	[NativeHeader("Modules/VR/HoloLens/WorldAnchor/WorldAnchor.h")]
	[UsedByNativeCode]
	public class WorldAnchor : Component
	{
		private WorldAnchor()
		{
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event WorldAnchor.OnTrackingChangedDelegate OnTrackingChanged;

		[Obsolete("Support for built-in VR will be removed in Unity 2020.1. Please update to the new Unity XR Plugin System. More information about the new XR Plugin System can be found at https://docs.unity3d.com/2019.3/Documentation/Manual/XR.html.", false)]
		public extern bool isLocated
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[NativeConditional("ENABLE_HOLOLENS_MODULE")]
		[NativeName("SetSpatialAnchor_Internal")]
		[Obsolete("Support for built-in VR will be removed in Unity 2020.1. Please update to the new Unity XR Plugin System. More information about the new XR Plugin System can be found at https://docs.unity3d.com/2019.3/Documentation/Manual/XR.html.", false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetNativeSpatialAnchorPtr(IntPtr spatialAnchorPtr);

		[NativeConditional("ENABLE_HOLOLENS_MODULE")]
		[NativeName("GetSpatialAnchor_Internal")]
		[Obsolete("Support for built-in VR will be removed in Unity 2020.1. Please update to the new Unity XR Plugin System. More information about the new XR Plugin System can be found at https://docs.unity3d.com/2019.3/Documentation/Manual/XR.html.", false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern IntPtr GetNativeSpatialAnchorPtr();

		[RequiredByNativeCode]
		private static void Internal_TriggerEventOnTrackingLost(WorldAnchor worldAnchor, bool located)
		{
			bool flag = worldAnchor != null && worldAnchor.OnTrackingChanged != null;
			if (flag)
			{
				worldAnchor.OnTrackingChanged(worldAnchor, located);
			}
		}

		public delegate void OnTrackingChangedDelegate(WorldAnchor worldAnchor, bool located);
	}
}
