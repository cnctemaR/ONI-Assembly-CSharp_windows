using System;

namespace UnityEngine.AMD
{
	public struct FSR2CommandExecutionData
	{
		public float jitterOffsetX;

		public float jitterOffsetY;

		public float MVScaleX;

		public float MVScaleY;

		public uint renderSizeWidth;

		public uint renderSizeHeight;

		public int enableSharpening;

		public float sharpness;

		public float frameTimeDelta;

		public float preExposure;

		public int reset;

		public float cameraNear;

		public float cameraFar;

		public float cameraFovAngleVertical;

		internal uint featureSlot;

		internal enum Textures
		{
			ColorInput,
			ColorOutput,
			Depth,
			MotionVectors,
			TransparencyMask,
			ExposureTexture,
			ReactiveMask,
			BiasColorMask
		}
	}
}
