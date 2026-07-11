using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.XR.WSA;

namespace UnityEngineInternal.XR.WSA
{
	[NativeConditional("ENABLE_HOLOLENS_MODULE")]
	[NativeHeader("Modules/VR/HoloLens/PerceptionRemoting.h")]
	public class RemoteSpeechAccess
	{
		[Obsolete("Support for built-in VR will be removed in Unity 2020.1. Please update to the new Unity XR Plugin System. More information about the new XR Plugin System can be found at https://docs.unity3d.com/2019.3/Documentation/Manual/XR.html.", false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void EnableRemoteSpeech(RemoteDeviceVersion remoteDeviceVersion);

		[Obsolete("Support for built-in VR will be removed in Unity 2020.1. Please update to the new Unity XR Plugin System. More information about the new XR Plugin System can be found at https://docs.unity3d.com/2019.3/Documentation/Manual/XR.html.", false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DisableRemoteSpeech();
	}
}
