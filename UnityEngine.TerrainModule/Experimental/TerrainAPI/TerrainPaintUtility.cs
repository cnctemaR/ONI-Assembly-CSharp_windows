using System;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.TerrainAPI
{
	public static class TerrainPaintUtility
	{
		public static Material GetBuiltinPaintMaterial()
		{
			if (TerrainPaintUtility.s_BuiltinPaintMaterial == null)
			{
				TerrainPaintUtility.s_BuiltinPaintMaterial = new Material(Shader.Find("Hidden/TerrainEngine/PaintHeight"));
			}
			return TerrainPaintUtility.s_BuiltinPaintMaterial;
		}

		public static BrushTransform CalculateBrushTransform(Terrain terrain, Vector2 brushCenterTerrainUV, float brushSize, float brushRotationDegrees)
		{
			float num = brushRotationDegrees * 0.017453292f;
			float num2 = Mathf.Cos(num);
			float num3 = Mathf.Sin(num);
			Vector2 vector = new Vector2(num2, -num3) * brushSize;
			Vector2 vector2 = new Vector2(num3, num2) * brushSize;
			Vector3 size = terrain.terrainData.size;
			Vector2 vector3 = brushCenterTerrainUV * new Vector2(size.x, size.z);
			Vector2 vector4 = vector3 - 0.5f * vector - 0.5f * vector2;
			BrushTransform brushTransform = new BrushTransform(vector4, vector, vector2);
			return brushTransform;
		}

		public static void BuildTransformPaintContextUVToPaintContextUV(PaintContext src, PaintContext dst, out Vector4 scaleOffset)
		{
			float num = ((float)src.pixelRect.xMin - 0.5f) * src.pixelSize.x;
			float num2 = ((float)src.pixelRect.yMin - 0.5f) * src.pixelSize.y;
			float num3 = (float)src.pixelRect.width * src.pixelSize.x;
			float num4 = (float)src.pixelRect.height * src.pixelSize.y;
			float num5 = ((float)dst.pixelRect.xMin - 0.5f) * dst.pixelSize.x;
			float num6 = ((float)dst.pixelRect.yMin - 0.5f) * dst.pixelSize.y;
			float num7 = (float)dst.pixelRect.width * dst.pixelSize.x;
			float num8 = (float)dst.pixelRect.height * dst.pixelSize.y;
			scaleOffset = new Vector4(num3 / num7, num4 / num8, (num - num5) / num7, (num2 - num6) / num8);
		}

		public static void SetupTerrainToolMaterialProperties(PaintContext paintContext, BrushTransform brushXform, Material material)
		{
			float num = ((float)paintContext.pixelRect.xMin - 0.5f) * paintContext.pixelSize.x;
			float num2 = ((float)paintContext.pixelRect.yMin - 0.5f) * paintContext.pixelSize.y;
			float num3 = (float)paintContext.pixelRect.width * paintContext.pixelSize.x;
			float num4 = (float)paintContext.pixelRect.height * paintContext.pixelSize.y;
			Vector2 vector = num3 * brushXform.targetX;
			Vector2 vector2 = num4 * brushXform.targetY;
			Vector2 vector3 = brushXform.targetOrigin + num * brushXform.targetX + num2 * brushXform.targetY;
			material.SetVector("_PCUVToBrushUVScales", new Vector4(vector.x, vector.y, vector2.x, vector2.y));
			material.SetVector("_PCUVToBrushUVOffset", new Vector4(vector3.x, vector3.y, 0f, 0f));
		}

		internal static bool paintTextureUsesCopyTexture
		{
			get
			{
				return (SystemInfo.copyTextureSupport & (CopyTextureSupport.TextureToRT | CopyTextureSupport.RTToTexture)) == (CopyTextureSupport.TextureToRT | CopyTextureSupport.RTToTexture);
			}
		}

		private static PaintContext InitializePaintContext(Terrain terrain, Texture target, RenderTextureFormat pcFormat, Rect boundsInTerrainSpace, int extraBorderPixels = 0)
		{
			PaintContext paintContext = PaintContext.CreateFromBounds(terrain, boundsInTerrainSpace, target.width, target.height, extraBorderPixels);
			paintContext.CreateRenderTargets(pcFormat);
			return paintContext;
		}

		public static void ReleaseContextResources(PaintContext ctx)
		{
			ctx.Cleanup(true);
		}

		public static PaintContext BeginPaintHeightmap(Terrain terrain, Rect boundsInTerrainSpace, int extraBorderPixels = 0)
		{
			RenderTexture heightmapTexture = terrain.terrainData.heightmapTexture;
			PaintContext paintContext = TerrainPaintUtility.InitializePaintContext(terrain, heightmapTexture, heightmapTexture.format, boundsInTerrainSpace, extraBorderPixels);
			paintContext.GatherHeightmap();
			return paintContext;
		}

		public static void EndPaintHeightmap(PaintContext ctx, string editorUndoName)
		{
			ctx.ScatterHeightmap(editorUndoName);
			ctx.Cleanup(true);
		}

		public static PaintContext CollectNormals(Terrain terrain, Rect boundsInTerrainSpace, int extraBorderPixels = 0)
		{
			RenderTexture normalmapTexture = terrain.normalmapTexture;
			PaintContext paintContext = TerrainPaintUtility.InitializePaintContext(terrain, normalmapTexture, normalmapTexture.format, boundsInTerrainSpace, extraBorderPixels);
			paintContext.GatherNormals();
			return paintContext;
		}

		public static PaintContext BeginPaintTexture(Terrain terrain, Rect boundsInTerrainSpace, TerrainLayer inputLayer, int extraBorderPixels = 0)
		{
			PaintContext paintContext;
			if (inputLayer == null)
			{
				paintContext = null;
			}
			else
			{
				int num = TerrainPaintUtility.FindTerrainLayerIndex(terrain, inputLayer);
				if (num == -1)
				{
					num = TerrainPaintUtility.AddTerrainLayer(terrain, inputLayer);
				}
				Texture2D terrainAlphaMapChecked = TerrainPaintUtility.GetTerrainAlphaMapChecked(terrain, num >> 2);
				PaintContext paintContext2 = TerrainPaintUtility.InitializePaintContext(terrain, terrainAlphaMapChecked, RenderTextureFormat.R8, boundsInTerrainSpace, extraBorderPixels);
				paintContext2.GatherAlphamap(inputLayer, true);
				paintContext = paintContext2;
			}
			return paintContext;
		}

		public static void EndPaintTexture(PaintContext ctx, string editorUndoName)
		{
			ctx.ScatterAlphamap(editorUndoName);
			ctx.Cleanup(true);
		}

		public static Material GetBlitMaterial()
		{
			if (!TerrainPaintUtility.m_BlitMaterial)
			{
				TerrainPaintUtility.m_BlitMaterial = new Material(Shader.Find("Hidden/BlitCopy"));
			}
			return TerrainPaintUtility.m_BlitMaterial;
		}

		public static Material GetCopyTerrainLayerMaterial()
		{
			if (!TerrainPaintUtility.m_CopyTerrainLayerMaterial)
			{
				TerrainPaintUtility.m_CopyTerrainLayerMaterial = new Material(Shader.Find("Hidden/TerrainEngine/TerrainLayerUtils"));
			}
			return TerrainPaintUtility.m_CopyTerrainLayerMaterial;
		}

		internal static void DrawQuad(RectInt destinationPixels, RectInt sourcePixels, Texture sourceTexture)
		{
			if (destinationPixels.width > 0 && destinationPixels.height > 0)
			{
				Rect rect = new Rect((float)sourcePixels.x / (float)sourceTexture.width, (float)sourcePixels.y / (float)sourceTexture.height, (float)sourcePixels.width / (float)sourceTexture.width, (float)sourcePixels.height / (float)sourceTexture.height);
				GL.Begin(7);
				GL.Color(new Color(1f, 1f, 1f, 1f));
				GL.TexCoord2(rect.x, rect.y);
				GL.Vertex3((float)destinationPixels.x, (float)destinationPixels.y, 0f);
				GL.TexCoord2(rect.x, rect.yMax);
				GL.Vertex3((float)destinationPixels.x, (float)destinationPixels.yMax, 0f);
				GL.TexCoord2(rect.xMax, rect.yMax);
				GL.Vertex3((float)destinationPixels.xMax, (float)destinationPixels.yMax, 0f);
				GL.TexCoord2(rect.xMax, rect.y);
				GL.Vertex3((float)destinationPixels.xMax, (float)destinationPixels.y, 0f);
				GL.End();
			}
		}

		internal static RectInt CalcPixelRectFromBounds(Terrain terrain, Rect boundsInTerrainSpace, int textureWidth, int textureHeight, int extraBorderPixels)
		{
			float num = ((float)textureWidth - 1f) / terrain.terrainData.size.x;
			float num2 = ((float)textureHeight - 1f) / terrain.terrainData.size.z;
			int num3 = Mathf.FloorToInt(boundsInTerrainSpace.xMin * num) - extraBorderPixels;
			int num4 = Mathf.FloorToInt(boundsInTerrainSpace.yMin * num2) - extraBorderPixels;
			int num5 = Mathf.CeilToInt(boundsInTerrainSpace.xMax * num) + extraBorderPixels;
			int num6 = Mathf.CeilToInt(boundsInTerrainSpace.yMax * num2) + extraBorderPixels;
			return new RectInt(num3, num4, num5 - num3 + 1, num6 - num4 + 1);
		}

		public static Texture2D GetTerrainAlphaMapChecked(Terrain terrain, int mapIndex)
		{
			if (mapIndex >= terrain.terrainData.alphamapTextureCount)
			{
				throw new ArgumentException("Trying to access out-of-bounds terrain alphamap information.");
			}
			return terrain.terrainData.alphamapTextures[mapIndex];
		}

		public static int FindTerrainLayerIndex(Terrain terrain, TerrainLayer inputLayer)
		{
			for (int i = 0; i < terrain.terrainData.terrainLayers.Length; i++)
			{
				if (terrain.terrainData.terrainLayers[i] == inputLayer)
				{
					return i;
				}
			}
			return -1;
		}

		internal static int AddTerrainLayer(Terrain terrain, TerrainLayer inputLayer)
		{
			int num = terrain.terrainData.terrainLayers.Length;
			TerrainLayer[] array = new TerrainLayer[num + 1];
			Array.Copy(terrain.terrainData.terrainLayers, 0, array, 0, num);
			array[num] = inputLayer;
			terrain.terrainData.terrainLayers = array;
			return num;
		}

		private static Material s_BuiltinPaintMaterial = null;

		private static Material m_BlitMaterial = null;

		private static Material m_CopyTerrainLayerMaterial = null;

		public enum BuiltinPaintMaterialPasses
		{
			RaiseLowerHeight,
			StampHeight,
			SetHeights,
			SmoothHeights,
			PaintTexture
		}
	}
}
