using System;

namespace UnityEngine.Shaders
{
	[Flags]
	public enum ShaderTypeFlags
	{
		None = 0,
		Graphics = 1,
		Compute = 2,
		RayTracing = 4,
		Any = 7
	}
}
