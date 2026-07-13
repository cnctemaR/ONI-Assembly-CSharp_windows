using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[UsedByNativeCode]
	internal struct GPUDrivenMeshLodInfo
	{
		public bool lodSelectionActive
		{
			get
			{
				return this.levelCount > 1;
			}
		}

		public int levelCount;

		public float lodSlope;

		public float lodBias;
	}
}
