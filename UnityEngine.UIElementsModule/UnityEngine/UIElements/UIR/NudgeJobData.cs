using System;

namespace UnityEngine.UIElements.UIR
{
	internal struct NudgeJobData
	{
		public IntPtr headSrc;

		public IntPtr headDst;

		public int headCount;

		public IntPtr tailSrc;

		public IntPtr tailDst;

		public int tailCount;

		public Matrix4x4 transform;
	}
}
