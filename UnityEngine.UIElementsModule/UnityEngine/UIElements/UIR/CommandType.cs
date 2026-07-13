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
		PushDefaultMaterial,
		PopDefaultMaterial,
		BeginDisable,
		EndDisable,
		CutRenderChain
	}
}
