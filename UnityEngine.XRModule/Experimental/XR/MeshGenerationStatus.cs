using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR
{
	[NativeHeader("Modules/XR/Subsystems/Meshing/XRMeshBindings.h")]
	[RequiredByNativeCode]
	public enum MeshGenerationStatus
	{
		Success,
		InvalidMeshId,
		GenerationAlreadyInProgress,
		Canceled,
		UnknownError
	}
}
