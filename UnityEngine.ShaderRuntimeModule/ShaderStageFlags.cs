using System;

namespace UnityEngine.Shaders
{
	[Flags]
	public enum ShaderStageFlags
	{
		None = 0,
		Vertex = 1,
		Fragment = 2,
		Hull = 4,
		Domain = 8,
		Geometry = 16,
		Compute = 32,
		RayTracing = 64,
		Basic = 3,
		Tessellation = 12,
		Graphics = 31,
		Any = 127
	}
}
