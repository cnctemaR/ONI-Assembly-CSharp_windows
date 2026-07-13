using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[NativeType(Header = "Modules/UIElements/VisualNodeTextData.h")]
	internal struct VisualNodeTextData
	{
		internal LanguageDirection LanguageDirection;

		internal LanguageDirection LocalLanguageDirection;
	}
}
