using System;

namespace UnityEngine.UIElements
{
	[Flags]
	internal enum RenderHints
	{
		None = 0,
		GroupTransform = 1,
		BoneTransform = 2,
		ClipWithScissors = 4,
		MaskContainer = 8,
		DynamicColor = 16,
		DynamicPostProcessing = 32,
		LargePixelCoverage = 64,
		DirtyOffset = 7,
		DirtyGroupTransform = 128,
		DirtyBoneTransform = 256,
		DirtyClipWithScissors = 512,
		DirtyMaskContainer = 1024,
		DirtyDynamicColor = 2048,
		DirtyDynamicPostProcessing = 4096,
		DirtyLargePixelCoverage = 8192,
		DirtyAll = 16256
	}
}
