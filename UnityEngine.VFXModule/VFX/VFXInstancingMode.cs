using System;

namespace UnityEngine.VFX
{
	internal enum VFXInstancingMode
	{
		Disabled = -1,
		[InspectorName("Automatic batch capacity")]
		Auto,
		[InspectorName("Custom batch capacity")]
		Custom
	}
}
