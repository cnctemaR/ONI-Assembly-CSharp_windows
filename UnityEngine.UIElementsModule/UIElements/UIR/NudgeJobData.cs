using System;

namespace UnityEngine.UIElements.UIR
{
	internal struct NudgeJobData
	{
		public IntPtr src;

		public IntPtr dst;

		public int count;

		public IntPtr closingSrc;

		public IntPtr closingDst;

		public int closingCount;

		public Matrix4x4 transform;

		public int vertsBeforeUVDisplacement;

		public int vertsAfterUVDisplacement;
	}
}
