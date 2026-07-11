using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.XR.WSA
{
	[NativeConditional("ENABLE_HOLOLENS_MODULE")]
	[NativeHeader("Runtime/VR/HoloLens/PerceptionRemoting.h")]
	internal class PerceptionRemoting
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetRemoteDeviceVersion(RemoteDeviceVersion remoteDeviceVersion);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Connect(string clientName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Disconnect();

		[NativeConditional("ENABLE_HOLOLENS_MODULE", StubReturnStatement = "HolographicEmulation::None")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern HolographicStreamerConnectionFailureReason CheckForDisconnect();

		[NativeConditional("ENABLE_HOLOLENS_MODULE", StubReturnStatement = "HolographicEmulation::Disconnected")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern HolographicStreamerConnectionState GetConnectionState();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetEnableAudio(bool enable);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetEnableVideo(bool enable);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetVideoEncodingParameters(int maxBitRate);
	}
}
