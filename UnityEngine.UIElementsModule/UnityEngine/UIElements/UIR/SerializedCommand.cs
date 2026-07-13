using System;

namespace UnityEngine.UIElements.UIR
{
	internal struct SerializedCommand
	{
		public SerializedCommandType type;

		public IntPtr vertexBuffer;

		public IntPtr indexBuffer;

		public int firstRange;

		public int rangeCount;

		public int textureName;

		public IntPtr texturePtr;

		public int gpuDataOffset;

		public Vector4 gpuData0;

		public Vector4 gpuData1;

		public MaterialPropertyBlock userProps;
	}
}
