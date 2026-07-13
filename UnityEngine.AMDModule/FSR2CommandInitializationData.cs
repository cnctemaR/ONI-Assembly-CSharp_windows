using System;

namespace UnityEngine.AMD
{
	public struct FSR2CommandInitializationData
	{
		public void SetFlag(FfxFsr2InitializationFlags flag, bool value)
		{
			if (value)
			{
				this.ffxFsrFlags |= flag;
			}
			else
			{
				this.ffxFsrFlags &= ~flag;
			}
		}

		public bool GetFlag(FfxFsr2InitializationFlags flag)
		{
			return (this.ffxFsrFlags & flag) > (FfxFsr2InitializationFlags)0;
		}

		public uint maxRenderSizeWidth;

		public uint maxRenderSizeHeight;

		public uint displaySizeWidth;

		public uint displaySizeHeight;

		public FfxFsr2InitializationFlags ffxFsrFlags;

		internal uint featureSlot;
	}
}
