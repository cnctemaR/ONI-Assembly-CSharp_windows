using System;

namespace UnityEngine.UIElements.UIR
{
	[Flags]
	internal enum CommandFlags
	{
		None = 0,
		IsPremultiplied = 1,
		ForceRenderTypeBitOffset = 1,
		ForceRenderTypeSolid = 2,
		ForceRenderTypeTextured = 4,
		ForceRenderTypeText = 6,
		ForceRenderTypeSvgGradient = 8,
		ForceRenderTypeBits = 14,
		ForceSingleTextureSlot = 16
	}
}
