using System;
using System.Linq;
using UnityEngine.Scripting;

namespace UnityEngine.Accessibility
{
	/// <summary>
	///   <para>A class containing methods to assist with accessibility for users with different vision capabilities.</para>
	/// </summary>
	[UsedByNativeCode]
	public static class VisionUtility
	{
		internal static float ComputePerceivedLuminance(Color color)
		{
			color = color.linear;
			return Mathf.LinearToGammaSpace(0.2126f * color.r + 0.7152f * color.g + 0.0722f * color.b);
		}

		/// <summary>
		///   <para>Gets a palette of colors that should be distinguishable for normal vision, deuteranopia, protanopia, and tritanopia.</para>
		/// </summary>
		/// <param name="palette">An array of colors to populate with a palette.</param>
		/// <param name="minimumLuminance">Minimum allowable perceived luminance from 0 to 1. A value of 0.2 or greater is recommended for dark backgrounds.</param>
		/// <param name="maximumLuminance">Maximum allowable perceived luminance from 0 to 1. A value of 0.8 or less is recommended for light backgrounds.</param>
		/// <returns>
		///   <para>The number of unambiguous colors in the palette.</para>
		/// </returns>
		public static int GetColorBlindSafePalette(Color[] palette, float minimumLuminance, float maximumLuminance)
		{
			if (palette == null)
			{
				throw new ArgumentNullException("palette");
			}
			Color[] array = (from i in Enumerable.Range(0, VisionUtility.s_ColorBlindSafePalette.Length)
				where VisionUtility.s_ColorBlindSafePaletteLuminanceValues[i] >= minimumLuminance && VisionUtility.s_ColorBlindSafePaletteLuminanceValues[i] <= maximumLuminance
				select VisionUtility.s_ColorBlindSafePalette[i]).ToArray<Color>();
			int num = Mathf.Min(palette.Length, array.Length);
			if (num > 0)
			{
				int k = 0;
				int num2 = palette.Length;
				while (k < num2)
				{
					palette[k] = array[k % num];
					k++;
				}
			}
			else
			{
				int j = 0;
				int num3 = palette.Length;
				while (j < num3)
				{
					palette[j] = default(Color);
					j++;
				}
			}
			return num;
		}

		private static readonly Color[] s_ColorBlindSafePalette = new Color[]
		{
			new Color32(0, 0, 0, byte.MaxValue),
			new Color32(73, 0, 146, byte.MaxValue),
			new Color32(7, 71, 81, byte.MaxValue),
			new Color32(0, 146, 146, byte.MaxValue),
			new Color32(182, 109, byte.MaxValue, byte.MaxValue),
			new Color32(byte.MaxValue, 109, 182, byte.MaxValue),
			new Color32(109, 182, byte.MaxValue, byte.MaxValue),
			new Color32(36, byte.MaxValue, 36, byte.MaxValue),
			new Color32(byte.MaxValue, 182, 219, byte.MaxValue),
			new Color32(182, 219, byte.MaxValue, byte.MaxValue),
			new Color32(byte.MaxValue, byte.MaxValue, 109, byte.MaxValue)
		};

		private static readonly float[] s_ColorBlindSafePaletteLuminanceValues = VisionUtility.s_ColorBlindSafePalette.Select<Color, float>((Color c) => VisionUtility.ComputePerceivedLuminance(c)).ToArray<float>();
	}
}
