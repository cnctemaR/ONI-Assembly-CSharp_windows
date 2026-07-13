using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements.StyleSheets.Syntax
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal enum DataType
	{
		None,
		Number,
		Integer,
		Length,
		Percentage,
		Color,
		Resource,
		Url,
		Time,
		FilterFunction,
		Prop,
		Angle,
		CustomIdent,
		Ratio
	}
}
