using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public static class TextureUtil
{
	public static GraphicsFormat TextureFormatToGraphicsFormat(TextureFormat format)
	{
		switch (format)
		{
		case TextureFormat.Alpha8:
			return GraphicsFormat.R8_UNorm;
		default:
			if (format == TextureFormat.RGFloat)
			{
				return GraphicsFormat.R32G32_SFloat;
			}
			if (format != TextureFormat.RGBAFloat)
			{
				global::Debug.LogError("Unspecfied graphics format for texture format: " + format.ToString(), null);
				throw new ArgumentOutOfRangeException();
			}
			return GraphicsFormat.R32G32B32A32_SFloat;
		case TextureFormat.RGB24:
			return GraphicsFormat.R8G8B8_SRGB;
		case TextureFormat.RGBA32:
			return GraphicsFormat.R8G8B8A8_SRGB;
		}
	}

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
