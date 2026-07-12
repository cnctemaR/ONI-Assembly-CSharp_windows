using System;

namespace UnityEngine.NVIDIA
{
	internal struct GraphicsDeviceDebugInfo
	{
		public uint NVDeviceVersion;

		public uint NGXVersion;

		public unsafe DLSSDebugFeatureInfos* dlssInfos;

		public uint dlssInfosCount;
	}
}
