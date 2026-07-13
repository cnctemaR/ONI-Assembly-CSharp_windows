using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal enum OverflowInternal
	{
		Visible,
		Hidden,
		Scroll
	}
}
