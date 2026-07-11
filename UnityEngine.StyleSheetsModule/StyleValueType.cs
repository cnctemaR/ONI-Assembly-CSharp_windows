using System;
using UnityEngine.Bindings;

namespace UnityEngine.StyleSheets
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal enum StyleValueType
	{
		Keyword,
		Float,
		Color,
		ResourcePath,
		AssetReference,
		Enum,
		String
	}
}
