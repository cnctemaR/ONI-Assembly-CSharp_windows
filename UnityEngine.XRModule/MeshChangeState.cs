using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.XR
{
	[UsedByNativeCode]
	[NativeHeader("Modules/XR/Subsystems/Meshing/XRMeshBindings.h")]
	public enum MeshChangeState
	{
		Added,
		Updated,
		Removed,
		Unchanged
	}
}
