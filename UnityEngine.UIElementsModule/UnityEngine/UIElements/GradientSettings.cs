using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEngine.VectorGraphicsModule", "UnityEditor.VectorGraphicsModule" })]
	[Serializable]
	internal struct GradientSettings
	{
		public GradientType gradientType;

		public AddressMode addressMode;

		public Vector2 radialFocus;

		public RectInt location;
	}
}
