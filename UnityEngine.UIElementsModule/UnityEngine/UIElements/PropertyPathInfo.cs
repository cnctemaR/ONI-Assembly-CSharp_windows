using System;
using Unity.Properties;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal readonly struct PropertyPathInfo
	{
		internal PropertyPathInfo(in PropertyPath propertyPath, Type type)
		{
			this.propertyPath = propertyPath;
			this.type = type;
		}

		public readonly PropertyPath propertyPath;

		public readonly Type type;
	}
}
