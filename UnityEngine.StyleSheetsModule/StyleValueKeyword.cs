using System;
using UnityEngine.Bindings;

namespace UnityEngine.StyleSheets
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal enum StyleValueKeyword
	{
		Inherit,
		Auto,
		Unset,
		True,
		False,
		None
	}
}
