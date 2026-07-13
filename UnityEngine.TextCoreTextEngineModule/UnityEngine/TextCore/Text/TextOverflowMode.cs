using System;
using UnityEngine.Bindings;

namespace UnityEngine.TextCore.Text
{
	[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
	internal enum TextOverflowMode
	{
		Overflow,
		Ellipsis,
		Masking,
		Truncate,
		ScrollRect,
		Linked = 6
	}
}
