using System;

namespace UnityEngine.UIElements.UIR
{
	internal struct CopyMeshJobData
	{
		public IntPtr vertSrc;

		public IntPtr vertDst;

		public int vertCount;

		public IntPtr indexSrc;

		public IntPtr indexDst;

		public int indexCount;

		public int indexOffset;
	}
}
