using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal interface IPrefixLabel
	{
		string label { get; }

		Label labelElement { get; }
	}
}
