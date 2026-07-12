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
		DirtyOffset = 5,
		DirtyGroupTransform = 32,
		DirtyBoneTransform = 64,
		DirtyClipWithScissors = 128,
		DirtyMaskContainer = 256,
		DirtyDynamicColor = 512,
		DirtyAll = 992
	}
}
