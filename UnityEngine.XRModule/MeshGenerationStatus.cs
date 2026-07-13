using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.XR
{
	[RequiredByNativeCode]
	[NativeHeader("Modules/XR/Subsystems/Meshing/XRMeshBindings.h")]
	public enum MeshGenerationStatus
	{
		Success,
		InvalidMeshId,
		GenerationAlreadyInProgress,
		Canceled,
		UnknownError
	}
}
