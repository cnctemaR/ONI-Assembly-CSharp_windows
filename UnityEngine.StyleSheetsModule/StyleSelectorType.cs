using System;
using UnityEngine.Bindings;

namespace UnityEngine.StyleSheets
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal enum StyleSelectorType
	{
		Unknown,
		Wildcard,
		Type,
		Class,
		PseudoClass,
		RecursivePseudoClass,
		ID,
		Predicate
	}
}
