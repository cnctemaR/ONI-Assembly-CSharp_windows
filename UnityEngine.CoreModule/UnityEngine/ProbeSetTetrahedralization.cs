using System;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeType("Runtime/Graphics/ProbeSetTetrahedralization.h")]
	internal sealed class ProbeSetTetrahedralization
	{
		internal Vector3[] hullRays { get; set; }

		internal Tetrahedron[] tetrahedra { get; set; }
	}
}
