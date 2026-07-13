using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
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
