using System;
using UnityEngine;

public static class TextureUtil
{
	public static int GetBytesPerPixel(TextureFormat format)
	{
		int num;
		switch (format)
		{
		case TextureFormat.Alpha8:
			num = 1;
			break;
		default:
			if (format != TextureFormat.RGFloat)
			{
				if (format != TextureFormat.RGBAFloat)
				{
					throw new ArgumentOutOfRangeException();
				}
				num = 16;
			}
			else
			{
				num = 8;
			}
			break;
		case TextureFormat.RGB24:
			num = 3;
			break;
		case TextureFormat.RGBA32:
			num = 4;
			break;
		case TextureFormat.ARGB32:
			num = 4;
			break;
		}
		return num;
	}

	public static RenderTextureFormat GetRenderTextureFormat(TextureFormat format)
	{
		RenderTextureFormat renderTextureFormat;
		switch (format)
		{
		case TextureFormat.Alpha8:
			renderTextureFormat = RenderTextureFormat.ARGB32;
			break;
		default:
			if (format != TextureFormat.RGFloat)
			{
				if (format != TextureFormat.RGBAFloat)
				{
					throw new ArgumentOutOfRangeException();
				}
				renderTextureFormat = RenderTextureFormat.ARGBHalf;
			}
			else
			{
				renderTextureFormat = RenderTextureFormat.RGFloat;
			}
			break;
		case TextureFormat.RGB24:
			renderTextureFormat = RenderTextureFormat.ARGB32;
			break;
		case TextureFormat.RGBA32:
			renderTextureFormat = RenderTextureFormat.ARGB32;
			break;
		case TextureFormat.ARGB32:
			renderTextureFormat = RenderTextureFormat.ARGB32;
			break;
		}
		return renderTextureFormat;
	}
}
