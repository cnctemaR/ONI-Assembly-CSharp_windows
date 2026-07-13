using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering
{
	[NativeHeader("Runtime/Graphics/ShadingRateImage.h")]
	public static class ShadingRateImage
	{
		[FreeFunction("ShadingRateImage::GetAllocSizeInternal")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void GetAllocSizeInternal(int pixelWidth, int pixelHeight, out int tileWidth, out int tileHeight);

		public static Vector2Int GetAllocTileSize(Vector2Int pixelSize)
		{
			return ShadingRateImage.GetAllocTileSize(pixelSize.x, pixelSize.y);
		}

		public static Vector2Int GetAllocTileSize(int pixelWidth, int pixelHeight)
		{
			int num;
			int num2;
			ShadingRateImage.GetAllocSizeInternal(pixelWidth, pixelHeight, out num, out num2);
			return new Vector2Int(num, num2);
		}

		public static RenderTexture AllocFromPixelSize(in RenderTextureDescriptor rtDesc)
		{
			Vector2Int allocTileSize = ShadingRateImage.GetAllocTileSize(rtDesc.width, rtDesc.height);
			RenderTextureDescriptor renderTextureDescriptor = rtDesc;
			renderTextureDescriptor.width = allocTileSize.x;
			renderTextureDescriptor.height = allocTileSize.y;
			return new RenderTexture(renderTextureDescriptor);
		}

		public static RenderTextureDescriptor GetRenderTextureDescriptor(int width, int height, int volumeDepth = 1, TextureDimension textureDimension = TextureDimension.Tex2D)
		{
			bool supportsPerImageTile = ShadingRateInfo.supportsPerImageTile;
			RenderTextureDescriptor renderTextureDescriptor2;
			if (supportsPerImageTile)
			{
				RenderTextureDescriptor renderTextureDescriptor = new RenderTextureDescriptor(width, height)
				{
					msaaSamples = 1,
					autoGenerateMips = false,
					volumeDepth = volumeDepth,
					dimension = textureDimension,
					graphicsFormat = ShadingRateInfo.graphicsFormat,
					enableRandomWrite = true,
					enableShadingRate = true
				};
				renderTextureDescriptor2 = renderTextureDescriptor;
			}
			else
			{
				RenderTextureDescriptor renderTextureDescriptor = new RenderTextureDescriptor(0, 0)
				{
					msaaSamples = 0,
					autoGenerateMips = false,
					volumeDepth = 0,
					dimension = TextureDimension.None,
					graphicsFormat = GraphicsFormat.None
				};
				renderTextureDescriptor2 = renderTextureDescriptor;
			}
			return renderTextureDescriptor2;
		}
	}
}
