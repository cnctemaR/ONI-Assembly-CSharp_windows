using System;
using UnityEngine.Bindings;

namespace UnityEngine.XR.WSA
{
	[NativeHeader("Runtime/VR/HoloLens/PerceptionRemoting.h")]
	internal enum EmulationMode
	{
		None,
		RemoteDevice,
		Simulated
	}
}
