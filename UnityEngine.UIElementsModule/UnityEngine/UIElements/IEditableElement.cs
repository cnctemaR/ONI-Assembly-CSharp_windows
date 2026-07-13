using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal interface IEditableElement
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		Action editingStarted { get; set; }

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		Action editingEnded { get; set; }
	}
}
