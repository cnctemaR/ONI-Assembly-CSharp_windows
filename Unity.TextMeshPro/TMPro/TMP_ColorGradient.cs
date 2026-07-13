using System;
using UnityEngine;

namespace TMPro
{
	[ExcludeFromPreset]
	[Serializable]
	public class TMP_ColorGradient : ScriptableObject
	{
		public TMP_ColorGradient()
		{
			this.colorMode = ColorMode.FourCornersGradient;
			this.topLeft = TMP_ColorGradient.k_DefaultColor;
			this.topRight = TMP_ColorGradient.k_DefaultColor;
			this.bottomLeft = TMP_ColorGradient.k_DefaultColor;
			this.bottomRight = TMP_ColorGradient.k_DefaultColor;
		}

		public TMP_ColorGradient(Color color)
		{
			this.colorMode = ColorMode.FourCornersGradient;
			this.topLeft = color;
			this.topRight = color;
			this.bottomLeft = color;
			this.bottomRight = color;
		}

		public TMP_ColorGradient(Color color0, Color color1, Color color2, Color color3)
		{
			this.colorMode = ColorMode.FourCornersGradient;
			this.topLeft = color0;
			this.topRight = color1;
			this.bottomLeft = color2;
			this.bottomRight = color3;
		}

		public ColorMode colorMode = ColorMode.FourCornersGradient;

		public Color topLeft;

		public Color topRight;

		public Color bottomLeft;

		public Color bottomRight;

		private const ColorMode k_DefaultColorMode = ColorMode.FourCornersGradient;

		private static readonly Color k_DefaultColor = Color.white;
	}
}
