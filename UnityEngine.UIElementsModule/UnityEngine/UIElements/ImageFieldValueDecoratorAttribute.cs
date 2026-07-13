using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal class ImageFieldValueDecoratorAttribute : PropertyAttribute
	{
		public ImageFieldValueDecoratorAttribute(string fieldName)
		{
			this.name = fieldName;
		}

		public string name;
	}
}
