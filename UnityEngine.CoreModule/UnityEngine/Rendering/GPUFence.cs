using System;

namespace UnityEngine.Rendering
{
	[Obsolete("GPUFence has been deprecated. Use GraphicsFence instead (UnityUpgradable) -> GraphicsFence", false)]
	public struct GPUFence
	{
		public bool passed
		{
			get
			{
				return true;
			}
		}
	}
}
