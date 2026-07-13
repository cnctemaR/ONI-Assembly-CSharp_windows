using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal enum StyleValueFunction
	{
		Unknown,
		Var,
		Env,
		LinearGradient,
		NoneFilter,
		CustomFilter,
		FilterTint,
		FilterOpacity,
		FilterInvert,
		FilterGrayscale,
		FilterSepia,
		FilterBlur,
		FilterContrast,
		FilterHueRotate,
		MaterialProperty
	}
}
