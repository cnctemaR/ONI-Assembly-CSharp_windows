using System;
using UnityEngine.Bindings;

namespace UnityEngine.StyleSheets
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal enum StyleSelectorRelationship
	{
		None,
		Child,
		Descendent
	}
}
