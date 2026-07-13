using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.VFX
{
	[RejectDragAndDropMaterial]
	[NativeType(Header = "Modules/VFX/Public/VFXRenderer.h")]
	[RequiredByNativeCode]
	public sealed class VFXRenderer : Renderer
	{
		[RequiredMember]
		public VFXRenderer()
		{
		}
	}
}
