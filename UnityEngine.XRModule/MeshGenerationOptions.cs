using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.XR
{
	[NativeHeader("Modules/XR/Subsystems/Meshing/XRMeshBindings.h")]
	[Flags]
	[UsedByNativeCode]
	public enum MeshGenerationOptions
	{
		None = 0,
		ConsumeTransform = 1
	}
}
