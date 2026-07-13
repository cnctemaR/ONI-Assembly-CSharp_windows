using System;
using UnityEngine.Bindings;

namespace UnityEngine.TextCore
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal enum WhiteSpace
	{
		Normal,
		NoWrap,
		Pre,
		PreWrap
	}
}
