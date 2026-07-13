using System;

namespace UnityEngine.UIElements.UIR
{
	[Flags]
	internal enum RenderDataFlags
	{
		IsGroupTransform = 1,
		IsIgnoringDynamicColorHint = 2,
		HasExtraData = 4,
		HasExtraMeshes = 8,
		IsSubTreeQuad = 16,
		IsNestedRenderTreeRoot = 32,
		IsClippingRectDirty = 64
	}
}
