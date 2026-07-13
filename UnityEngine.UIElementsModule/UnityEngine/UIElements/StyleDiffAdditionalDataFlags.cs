using System;

namespace UnityEngine.UIElements
{
	[Flags]
	internal enum StyleDiffAdditionalDataFlags
	{
		None = 0,
		UxmlInlineProperties = 1,
		Bindings = 2,
		Selectors = 4,
		All = 7
	}
}
