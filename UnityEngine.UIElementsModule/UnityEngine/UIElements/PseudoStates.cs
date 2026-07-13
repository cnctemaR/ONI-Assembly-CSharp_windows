using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[Flags]
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal enum PseudoStates
	{
		None = 0,
		Active = 1,
		Hover = 2,
		Checked = 8,
		Disabled = 32,
		Focus = 64,
		Root = 128
	}
}
