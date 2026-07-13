using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements.StyleSheets
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal struct StylePropertyValue
	{
		public StyleSheet sheet;

		public StyleValueHandle handle;
	}
}
