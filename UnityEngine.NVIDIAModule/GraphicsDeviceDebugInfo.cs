using System;

namespace UnityEngine.NVIDIA
{
	internal struct GraphicsDeviceDebugInfo
	{
		public uint NVDeviceVersion;

		public uint NGXVersion;

		public IntPtr outDlssInfoBuffer;

		public uint outDlssInfoBufferCapacity;

		public uint dlssInfoCount;
	}
}
