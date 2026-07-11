using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.XR.WSA
{
	[StaticAccessor("HolographicSettings::GetInstance()", StaticAccessorType.Dot)]
	[NativeHeader("Modules/VR/HoloLens/HolographicSettings.h")]
	public class HolographicSettings
	{
		[Obsolete("Support for built-in VR will be removed in Unity 2020.1. Please update to the new Unity XR Plugin System. More information about the new XR Plugin System can be found at https://docs.unity3d.com/2019.3/Documentation/Manual/XR.html.", false)]
		public static void SetFocusPointForFrame(Vector3 position)
		{
			HolographicSettings.InternalSetFocusPointForFrameP(position);
		}

		[Obsolete("Support for built-in VR will be removed in Unity 2020.1. Please update to the new Unity XR Plugin System. More information about the new XR Plugin System can be found at https://docs.unity3d.com/2019.3/Documentation/Manual/XR.html.", false)]
		public static void SetFocusPointForFrame(Vector3 position, Vector3 normal)
		{
			HolographicSettings.InternalSetFocusPointForFramePN(position, normal);
		}

		[Obsolete("Support for built-in VR will be removed in Unity 2020.1. Please update to the new Unity XR Plugin System. More information about the new XR Plugin System can be found at https://docs.unity3d.com/2019.3/Documentation/Manual/XR.html.", false)]
		public static void SetFocusPointForFrame(Vector3 position, Vector3 normal, Vector3 velocity)
		{
			HolographicSettings.InternalSetFocusPointForFramePNV(position, normal, velocity);
		}

		[NativeConditional("ENABLE_HOLOLENS_MODULE")]
		[NativeName("SetFocusPointForFrame")]
		private static void InternalSetFocusPointForFrameP(Vector3 position)
		{
			HolographicSettings.InternalSetFocusPointForFrameP_Injected(ref position);
		}

		[NativeConditional("ENABLE_HOLOLENS_MODULE")]
		[NativeName("SetFocusPointForFrame")]
		private static void InternalSetFocusPointForFramePN(Vector3 position, Vector3 normal)
		{
			HolographicSettings.InternalSetFocusPointForFramePN_Injected(ref position, ref normal);
		}

		[NativeConditional("ENABLE_HOLOLENS_MODULE")]
		[NativeName("SetFocusPointForFrame")]
		private static void InternalSetFocusPointForFramePNV(Vector3 position, Vector3 normal, Vector3 velocity)
		{
			HolographicSettings.InternalSetFocusPointForFramePNV_Injected(ref position, ref normal, ref velocity);
		}

		[Obsolete("Support for built-in VR will be removed in Unity 2020.1. Please update to the new Unity XR Plugin System. More information about the new XR Plugin System can be found at https://docs.unity3d.com/2019.3/Documentation/Manual/XR.html.", false)]
		public static bool IsDisplayOpaque
		{
			get
			{
				return true;
			}
		}

		[NativeConditional("ENABLE_HOLOLENS_MODULE")]
		[Obsolete("Support for built-in VR will be removed in Unity 2020.1. Please update to the new Unity XR Plugin System. More information about the new XR Plugin System can be found at https://docs.unity3d.com/2019.3/Documentation/Manual/XR.html.", false)]
		public static extern bool IsContentProtectionEnabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Support for built-in VR will be removed in Unity 2020.1. Please update to the new Unity XR Plugin System. More information about the new XR Plugin System can be found at https://docs.unity3d.com/2019.3/Documentation/Manual/XR.html.", false)]
		public static HolographicSettings.HolographicReprojectionMode ReprojectionMode
		{
			get
			{
				return HolographicSettings.HolographicReprojectionMode.Disabled;
			}
			set
			{
			}
		}

		[Obsolete("Support for toggling latent frame presentation has been removed", true)]
		public static void ActivateLatentFramePresentation(bool activated)
		{
		}

		[Obsolete("Support for toggling latent frame presentation has been removed, and IsLatentFramePresentation will always return true", false)]
		public static bool IsLatentFramePresentation
		{
			get
			{
				return true;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetFocusPointForFrameP_Injected(ref Vector3 position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetFocusPointForFramePN_Injected(ref Vector3 position, ref Vector3 normal);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetFocusPointForFramePNV_Injected(ref Vector3 position, ref Vector3 normal, ref Vector3 velocity);

		public enum HolographicReprojectionMode
		{
			PositionAndOrientation,
			OrientationOnly,
			Disabled
		}
	}
}
