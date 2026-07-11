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
		case TextureFormat.ARGB4444:
			break;
		case TextureFormat.RGB24:
			return GraphicsFormat.R8G8B8_SRGB;
		case TextureFormat.RGBA32:
			return GraphicsFormat.R8G8B8A8_SRGB;
		default:
			if (format == TextureFormat.RGFloat)
			{
				return GraphicsFormat.R32G32_SFloat;
			}
			if (format == TextureFormat.RGBAFloat)
			{
				return GraphicsFormat.R32G32B32A32_SFloat;
			}
			break;
		}
		global::Debug.LogError("Unspecfied graphics format for texture format: " + format.ToString());
		throw new ArgumentOutOfRangeException();
	}

	public static int GetBytesPerPixel(TextureFormat format)
	{
		switch (format)
		{
		case TextureFormat.Alpha8:
			return 1;
		case TextureFormat.ARGB4444:
			break;
		case TextureFormat.RGB24:
			return 3;
		case TextureFormat.RGBA32:
			return 4;
		case TextureFormat.ARGB32:
			return 4;
		default:
			if (format == TextureFormat.RGFloat)
			{
				return 8;
			}
			if (format == TextureFormat.RGBAFloat)
			{
				return 16;
			}
			break;
		}
		throw new ArgumentOutOfRangeException();
	}

	public static RenderTextureFormat GetRenderTextureFormat(TextureFormat format)
	{
		switch (format)
		{
		case TextureFormat.Alpha8:
			return RenderTextureFormat.ARGB32;
		case TextureFormat.ARGB4444:
			break;
		case TextureFormat.RGB24:
			return RenderTextureFormat.ARGB32;
		case TextureFormat.RGBA32:
			return RenderTextureFormat.ARGB32;
		case TextureFormat.ARGB32:
			return RenderTextureFormat.ARGB32;
		default:
			if (format == TextureFormat.RGFloat)
			{
				return RenderTextureFormat.RGFloat;
			}
			if (format == TextureFormat.RGBAFloat)
			{
				return RenderTextureFormat.ARGBHalf;
			}
			break;
		}
		throw new ArgumentOutOfRangeException();
	}
}
