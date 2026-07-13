using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule", "UnityEngine.HierarchyModule" })]
	internal enum DispatchMode
	{
		Default = 1,
		Queued = 1,
		Immediate
	}
}
