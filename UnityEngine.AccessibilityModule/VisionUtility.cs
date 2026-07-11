using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Accessibility
{
	[UsedByNativeCode]
	public static class VisionUtility
	{
		internal static float ComputePerceivedLuminance(Color color)
		{
			color = color.linear;
			return Mathf.LinearToGammaSpace(0.2126f * color.r + 0.7152f * color.g + 0.0722f * color.b);
		}

		internal static void GetLuminanceValuesForPalette(Color[] palette, ref float[] outLuminanceValues)
		{
			Debug.Assert(palette != null && outLuminanceValues != null, "Passed in arrays can't be null.");
			Debug.Assert(palette.Length == outLuminanceValues.Length, "Passed in arrays need to be of the same length.");
			for (int i = 0; i < palette.Length; i++)
			{
				outLuminanceValues[i] = VisionUtility.ComputePerceivedLuminance(palette[i]);
			}
		}

		public unsafe static int GetColorBlindSafePalette(Color[] palette, float minimumLuminance, float maximumLuminance)
		{
			bool flag = palette == null;
			if (flag)
			{
				throw new ArgumentNullException("palette");
			}
			Color* ptr;
			if (palette == null || palette.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &palette[0];
			}
			return VisionUtility.GetColorBlindSafePaletteInternal((void*)ptr, palette.Length, minimumLuminance, maximumLuminance, false);
		}

		internal unsafe static int GetColorBlindSafePalette(Color32[] palette, float minimumLuminance, float maximumLuminance)
		{
			bool flag = palette == null;
			if (flag)
			{
				throw new ArgumentNullException("palette");
			}
			Color32* ptr;
			if (palette == null || palette.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &palette[0];
			}
			return VisionUtility.GetColorBlindSafePaletteInternal((void*)ptr, palette.Length, minimumLuminance, maximumLuminance, true);
		}

		[MethodImpl((MethodImplOptions)256)]
		private unsafe static int GetColorBlindSafePaletteInternal(void* palette, int paletteLength, float minimumLuminance, float maximumLuminance, bool useColor32)
		{
			bool flag = palette == null;
			if (flag)
			{
				throw new ArgumentNullException("palette");
			}
			Color[] array = (from i in Enumerable.Range(0, VisionUtility.s_ColorBlindSafePalette.Length)
				where VisionUtility.s_ColorBlindSafePaletteLuminanceValues[i] >= minimumLuminance && VisionUtility.s_ColorBlindSafePaletteLuminanceValues[i] <= maximumLuminance
				select VisionUtility.s_ColorBlindSafePalette[i]).ToArray<Color>();
			int num = Mathf.Min(paletteLength, array.Length);
			bool flag2 = num > 0;
			if (flag2)
			{
				for (int k = 0; k < paletteLength; k++)
				{
					if (useColor32)
					{
						*(Color32*)((byte*)palette + (IntPtr)k * (IntPtr)sizeof(Color32)) = array[k % num];
					}
					else
					{
						*(Color*)((byte*)palette + (IntPtr)k * (IntPtr)sizeof(Color)) = array[k % num];
					}
				}
			}
			else
			{
				for (int j = 0; j < paletteLength; j++)
				{
					if (useColor32)
					{
						*(Color32*)((byte*)palette + (IntPtr)j * (IntPtr)sizeof(Color32)) = default(Color32);
					}
					else
					{
						*(Color*)((byte*)palette + (IntPtr)j * (IntPtr)sizeof(Color)) = default(Color);
					}
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
			new Color32(byte.MaxValue, byte.MaxValue, 109, byte.MaxValue),
			new Color32(30, 92, 92, byte.MaxValue),
			new Color32(74, 154, 87, byte.MaxValue),
			new Color32(113, 66, 183, byte.MaxValue),
			new Color32(162, 66, 183, byte.MaxValue),
			new Color32(178, 92, 25, byte.MaxValue),
			new Color32(100, 100, 100, byte.MaxValue),
			new Color32(80, 203, 181, byte.MaxValue),
			new Color32(82, 205, 242, byte.MaxValue)
		};

		private static readonly float[] s_ColorBlindSafePaletteLuminanceValues = VisionUtility.s_ColorBlindSafePalette.Select<Color, float>((Color c) => VisionUtility.ComputePerceivedLuminance(c)).ToArray<float>();
	}
}
