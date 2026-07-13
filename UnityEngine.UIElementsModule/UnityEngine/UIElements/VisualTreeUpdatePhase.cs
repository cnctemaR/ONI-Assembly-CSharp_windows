using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal enum VisualTreeUpdatePhase
	{
		Bindings,
		DataBinding,
		Animation,
		Styles,
		Layout,
		TransformClip,
		Repaint,
		Authoring,
		Count
	}
}
