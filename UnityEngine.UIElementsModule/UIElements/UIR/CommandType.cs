using System;

namespace UnityEngine.UIElements.UIR
{
	internal enum CommandType
	{
		Draw,
		ImmediateCull,
		Immediate,
		PushView,
		PopView,
		PushScissor,
		PopScissor,
		PushRenderTexture,
		PopRenderTexture,
		BlitToPreviousRT,
		PushDefaultMaterial,
		PopDefaultMaterial
	}
}
