using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal enum StyleValueKeyword
	{
		Inherit,
		Initial,
		Auto,
		Unset,
		True,
		False,
		None,
		Cover,
		Contain
	}
}
