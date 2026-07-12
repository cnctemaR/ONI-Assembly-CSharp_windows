using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.VFX
{
	[RejectDragAndDropMaterial]
	[NativeType(Header = "Modules/VFX/Public/VFXRenderer.h")]
	[UsedByNativeCode]
	internal sealed class VFXRenderer : Renderer
	{
	}
}
