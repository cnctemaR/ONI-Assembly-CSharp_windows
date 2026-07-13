using System;
using UnityEngine.Bindings;

namespace UnityEngine.TextCore.Text
{
	[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
	internal enum TextWrappingMode
	{
		NoWrap,
		Normal,
		PreserveWhitespace,
		PreserveWhitespaceNoWrap
	}
}
