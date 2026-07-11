using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.XR.WSA
{
	[NativeHeader("Runtime/VR/HoloLens/HolographicEmulation/HolographicEmulationManager.h")]
	[StaticAccessor("HolographicEmulation::HolographicEmulationManager::Get()", StaticAccessorType.Dot)]
	internal class HolographicEmulationHelper
	{
		[NativeConditional("ENABLE_HOLOLENS_MODULE", StubReturnStatement = "HolographicEmulation::EmulationMode_None")]
		[NativeName("GetEmulationMode")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern EmulationMode GetEmulationMode();
	}
}
