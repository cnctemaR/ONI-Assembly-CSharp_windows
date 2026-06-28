using System;
using UnityEngine;

public static class TextureUtil
{
	public static int GetBytesPerPixel(TextureFormat format)
	{
		switch (format)
		{
		case TextureFormat.Alpha8:
			return 1;
		default:
			if (format == TextureFormat.RGFloat)
			{
				return 8;
			}
			if (format != TextureFormat.RGBAFloat)
			{
				throw new ArgumentOutOfRangeException();
			}
			return 16;
		case TextureFormat.RGB24:
			return 3;
		case TextureFormat.RGBA32:
			return 4;
		case TextureFormat.ARGB32:
			return 4;
		}
	}

	public static RenderTextureFormat GetRenderTextureFormat(TextureFormat format)
	{
		switch (format)
		{
		case TextureFormat.Alpha8:
			return RenderTextureFormat.ARGB32;
		default:
			if (format == TextureFormat.RGFloat)
			{
				return RenderTextureFormat.RGFloat;
			}
			if (format != TextureFormat.RGBAFloat)
			{
				throw new ArgumentOutOfRangeException();
			}
			return RenderTextureFormat.ARGBHalf;
		case TextureFormat.RGB24:
			return RenderTextureFormat.ARGB32;
		case TextureFormat.RGBA32:
			return RenderTextureFormat.ARGB32;
		case TextureFormat.ARGB32:
			return RenderTextureFormat.ARGB32;
		}
	}
}
