using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR
{
	[NativeHeader("Modules/XR/XRPrefix.h")]
	[NativeHeader("Modules/XR/Subsystems/Raycast/XRRaycastSubsystem.h")]
	[UsedByNativeCode]
	[NativeConditional("ENABLE_XR")]
	public class XRRaycastSubsystem : IntegratedSubsystem<XRRaycastSubsystemDescriptor>
	{
		public bool Raycast(Vector3 screenPoint, List<XRRaycastHit> hitResults, TrackableType trackableTypeMask = TrackableType.All)
		{
			if (hitResults == null)
			{
				throw new ArgumentNullException("hitResults");
			}
			float num = Mathf.Clamp01(screenPoint.x / (float)Screen.width);
			float num2 = Mathf.Clamp01(screenPoint.y / (float)Screen.height);
			return this.Internal_ScreenRaycastAsList(num, num2, trackableTypeMask, hitResults);
		}

		public static void Raycast(Ray ray, XRDepthSubsystem depthSubsystem, XRPlaneSubsystem planeSubsystem, List<XRRaycastHit> hitResults, TrackableType trackableTypeMask = TrackableType.All, float pointCloudRaycastAngleInDegrees = 5f)
		{
			if (hitResults == null)
			{
				throw new ArgumentNullException("hitResults");
			}
			IntPtr intPtr = ((depthSubsystem != null) ? depthSubsystem.m_Ptr : IntPtr.Zero);
			IntPtr intPtr2 = ((planeSubsystem != null) ? planeSubsystem.m_Ptr : IntPtr.Zero);
			XRRaycastSubsystem.Internal_RaycastAsList(ray, pointCloudRaycastAngleInDegrees, intPtr, intPtr2, trackableTypeMask, hitResults);
		}

		private static void Internal_RaycastAsList(Ray ray, float pointCloudRaycastAngleInDegrees, IntPtr depthSubsystem, IntPtr planeSubsystem, TrackableType trackableTypeMask, List<XRRaycastHit> hitResultsOut)
		{
			XRRaycastSubsystem.Internal_RaycastAsList_Injected(ref ray, pointCloudRaycastAngleInDegrees, depthSubsystem, planeSubsystem, trackableTypeMask, hitResultsOut);
		}

		private static XRRaycastHit[] Internal_RaycastAsFixedArray(Ray ray, float pointCloudRaycastAngleInDegrees, IntPtr depthSubsystem, IntPtr planeSubsystem, TrackableType trackableTypeMask)
		{
			return XRRaycastSubsystem.Internal_RaycastAsFixedArray_Injected(ref ray, pointCloudRaycastAngleInDegrees, depthSubsystem, planeSubsystem, trackableTypeMask);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool Internal_ScreenRaycastAsList(float screenX, float screenY, TrackableType hitMask, List<XRRaycastHit> hitResultsOut);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern XRRaycastHit[] Internal_ScreenRaycastAsFixedArray(float screenX, float screenY, TrackableType hitMask);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_RaycastAsList_Injected(ref Ray ray, float pointCloudRaycastAngleInDegrees, IntPtr depthSubsystem, IntPtr planeSubsystem, TrackableType trackableTypeMask, List<XRRaycastHit> hitResultsOut);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern XRRaycastHit[] Internal_RaycastAsFixedArray_Injected(ref Ray ray, float pointCloudRaycastAngleInDegrees, IntPtr depthSubsystem, IntPtr planeSubsystem, TrackableType trackableTypeMask);
	}
}
