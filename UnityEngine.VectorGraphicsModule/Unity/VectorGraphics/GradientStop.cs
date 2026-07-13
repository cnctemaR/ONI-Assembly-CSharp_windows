using System;
using UnityEngine;

namespace Unity.VectorGraphics
{
	public struct GradientStop
	{
		public Color Color { readonly get; set; }

		public float StopPercentage { readonly get; set; }
	}
}
