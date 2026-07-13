using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[NativeType("Runtime/Export/Graphics/GraphicsTexture.bindings.h")]
	[UsedByNativeCode]
	public enum GraphicsTextureState
	{
		Constructed,
		Initializing,
		InitializedOnRenderThread,
		DestroyQueued,
		Destroyed
	}
}
