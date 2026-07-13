using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements.StyleSheets.Syntax
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal enum ExpressionType
	{
		Unknown,
		Data,
		Keyword,
		Combinator
	}
}
