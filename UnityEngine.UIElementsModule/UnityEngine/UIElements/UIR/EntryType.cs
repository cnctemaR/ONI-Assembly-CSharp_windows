using System;

namespace UnityEngine.UIElements.UIR
{
	internal enum EntryType : ushort
	{
		DrawSolidMesh,
		DrawTexturedMesh,
		DrawTexturedMeshSkipAtlas,
		DrawDynamicTexturedMesh,
		DrawTextMesh,
		DrawGradients,
		DrawImmediate,
		DrawImmediateCull,
		DrawChildren,
		BeginStencilMask,
		EndStencilMask,
		PopStencilMask,
		PushClippingRect,
		PopClippingRect,
		PushScissors,
		PopScissors,
		PushGroupMatrix,
		PopGroupMatrix,
		PushDefaultMaterial,
		PopDefaultMaterial,
		CutRenderChain,
		DedicatedPlaceholder
	}
}
