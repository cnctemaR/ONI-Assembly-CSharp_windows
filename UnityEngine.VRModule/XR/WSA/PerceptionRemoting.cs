using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.XR.WSA
{
	[NativeHeader("Runtime/VR/HoloLens/PerceptionRemoting.h")]
	[NativeConditional("ENABLE_HOLOLENS_MODULE")]
	internal class PerceptionRemoting
	{
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
