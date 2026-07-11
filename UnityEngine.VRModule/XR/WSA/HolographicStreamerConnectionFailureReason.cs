using System;
using UnityEngine.Bindings;

namespace UnityEngine.XR.WSA
{
	[NativeHeader("Runtime/VR/HoloLens/PerceptionRemoting.h")]
	public enum HolographicStreamerConnectionFailureReason
	{
		None,
		Unknown,
		Unreachable,
		HandshakeFailed,
		ProtocolVersionMismatch,
		ConnectionLost
	}
}
