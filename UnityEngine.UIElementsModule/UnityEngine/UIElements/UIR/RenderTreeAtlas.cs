using System;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.UIElements.UIR
{
	internal static class RenderTreeAtlas
	{
		public static bool ReserveSize(int width, int height, out RenderTreeAtlas.AtlasBlock block)
		{
			int num = width + 2 + 2;
			int num2 = height + 2 + 2;
			RectInt rectInt = new RectInt(2, 2, width, height);
			Rect rect = new Rect(2f / (float)num, 2f / (float)num2, (float)width / (float)num, (float)height / (float)num2);
			rectInt.y = num2 - (rectInt.y + rectInt.height);
			rect.y = 1f - rect.yMax;
			block = new RenderTreeAtlas.AtlasBlock(width, height, rectInt, rect);
			return true;
		}

		public static bool CreateTextureForAtlasBlock(ref RenderTreeAtlas.AtlasBlock block, bool forceGammaRendering, out bool allocatedNewTexture)
		{
			Debug.Assert(block.texture == null, "Entry already has a texture assigned.");
			Debug.Assert(block.width > 0 && block.height > 0, "Invalid texture size requested.");
			int num = block.width + 2 + 2;
			int num2 = block.height + 2 + 2;
			ColorSpace colorSpace = QualitySettings.activeColorSpace;
			if (forceGammaRendering)
			{
				colorSpace = ColorSpace.Gamma;
			}
			GraphicsFormat graphicsFormat = ((colorSpace == ColorSpace.Linear) ? GraphicsFormat.R8G8B8A8_SRGB : GraphicsFormat.R8G8B8A8_UNorm);
			block.texture = RenderTexture.GetTemporary(new RenderTextureDescriptor(num, num2, graphicsFormat, GraphicsFormat.D24_UNorm_S8_UInt)
			{
				useMipMap = false
			});
			allocatedNewTexture = true;
			bool flag = block.texture == null;
			bool flag2;
			if (flag)
			{
				Debug.LogError(string.Format("Failed to allocate RenderTexture of size {0}x{1}.", block.width, block.height));
				flag2 = false;
			}
			else
			{
				RenderTexture active = RenderTexture.active;
				RenderTexture.active = block.texture;
				GL.Clear(true, true, Color.clear, 1f);
				RenderTexture.active = active;
				flag2 = true;
			}
			return flag2;
		}

		private const int k_LeftMargin = 2;

		private const int k_TopMargin = 2;

		private const int k_RightMargin = 2;

		private const int k_BottomMargin = 2;

		public struct AtlasBlock
		{
			public AtlasBlock(int w, int h, RectInt r, Rect uv)
			{
				this.width = w;
				this.height = h;
				this.rect = r;
				this.uvRect = uv;
				this.texture = null;
			}

			public int width;

			public int height;

			public RectInt rect;

			public Rect uvRect;

			public RenderTexture texture;
		}
	}
}
