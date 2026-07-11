using System;

namespace UnityEngine.Experimental.VFX
{
	[Flags]
	internal enum VFXCullingFlags
	{
		CullNone = 0,
		CullSimulation = 1,
		CullBoundsUpdate = 2,
		CullDefault = 3
	}
}
