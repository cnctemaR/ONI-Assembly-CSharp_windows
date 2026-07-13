using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.TextCore.LowLevel;

namespace UnityEngine.TextCore.Text
{
	[VisibleToOtherModules(new string[] { "UnityEditor.CoreModule", "UnityEngine.UIElementsModule" })]
	[global::System.Runtime.CompilerServices.Nullable(0)]
	[global::System.Runtime.CompilerServices.NullableContext(1)]
	internal class FontAssetFactory
	{
		[return: global::System.Runtime.CompilerServices.Nullable(2)]
		public static FontAsset CloneFontAssetWithBitmapRendering(FontAsset baseFontAsset, int fontSize, bool isRaster)
		{
			FontAssetFactory.visitedFontAssets.Clear();
			return FontAssetFactory.CloneFontAssetWithBitmapRenderingInternal(baseFontAsset, fontSize, isRaster);
		}

		[return: global::System.Runtime.CompilerServices.Nullable(2)]
		private static FontAsset CloneFontAssetWithBitmapRenderingInternal(FontAsset baseFontAsset, int fontSize, bool isRaster)
		{
			FontAssetFactory.visitedFontAssets.Add(baseFontAsset);
			FontAsset fontAsset = FontAssetFactory.CloneFontAssetWithBitmapSettings(baseFontAsset, fontSize, isRaster);
			bool flag = fontAsset != null;
			if (flag)
			{
				FontAssetFactory.ProcessFontWeights(fontAsset, baseFontAsset, fontSize, isRaster);
				FontAssetFactory.ProcessFallbackFonts(fontAsset, baseFontAsset, fontSize, isRaster);
			}
			return fontAsset;
		}

		[return: global::System.Runtime.CompilerServices.Nullable(2)]
		private static FontAsset CloneFontAssetWithBitmapSettings(FontAsset source, int size, bool isRaster)
		{
			bool flag = source.atlasRenderMode != GlyphRenderMode.SDFAA || !source.IsEditorFont || source.sourceFontFile == null;
			bool flag2 = source.atlasPopulationMode == AtlasPopulationMode.DynamicOS;
			FontAsset fontAsset;
			if (flag2)
			{
				fontAsset = FontAsset.CreateFontAsset(source.faceInfo.familyName, source.faceInfo.styleName, size, 6, isRaster ? GlyphRenderMode.RASTER_HINTED : GlyphRenderMode.SMOOTH_HINTED);
				bool flag3 = fontAsset != null;
				if (flag3)
				{
					FontAssetFactory.SetupFontAssetForBitmapSettings(fontAsset);
				}
			}
			else
			{
				bool flag4 = flag;
				if (flag4)
				{
					fontAsset = Object.Instantiate<FontAsset>(source);
					bool flag5 = fontAsset != null;
					if (flag5)
					{
						fontAsset.fallbackFontAssetTable = new List<FontAsset>();
						fontAsset.m_IsClone = true;
						fontAsset.IsEditorFont = true;
						FontAssetFactory.SetHideFlags(fontAsset);
					}
				}
				else
				{
					fontAsset = FontAsset.CreateFontAsset(source.sourceFontFile, size, 6, isRaster ? GlyphRenderMode.RASTER_HINTED : GlyphRenderMode.SMOOTH_HINTED, source.atlasWidth, source.atlasHeight, AtlasPopulationMode.Dynamic, true);
					bool flag6 = fontAsset != null;
					if (flag6)
					{
						FontAssetFactory.SetupFontAssetForBitmapSettings(fontAsset);
					}
				}
			}
			return fontAsset;
		}

		private static void ProcessFontWeights(FontAsset resultFontAsset, FontAsset baseFontAsset, int fontSize, bool isRaster)
		{
			for (int i = 0; i < baseFontAsset.fontWeightTable.Length; i++)
			{
				FontWeightPair fontWeightPair = baseFontAsset.fontWeightTable[i];
				bool flag = fontWeightPair.regularTypeface != null;
				if (flag)
				{
					resultFontAsset.fontWeightTable[i].regularTypeface = FontAssetFactory.CloneFontAssetWithBitmapSettings(fontWeightPair.regularTypeface, fontSize, isRaster);
				}
				bool flag2 = fontWeightPair.italicTypeface != null;
				if (flag2)
				{
					resultFontAsset.fontWeightTable[i].italicTypeface = FontAssetFactory.CloneFontAssetWithBitmapSettings(fontWeightPair.italicTypeface, fontSize, isRaster);
				}
			}
		}

		private static void ProcessFallbackFonts(FontAsset resultFontAsset, FontAsset baseFontAsset, int fontSize, bool isRaster)
		{
			bool flag = baseFontAsset.fallbackFontAssetTable == null;
			if (!flag)
			{
				foreach (FontAsset fontAsset in baseFontAsset.fallbackFontAssetTable)
				{
					bool flag2 = fontAsset != null && !FontAssetFactory.visitedFontAssets.Contains(fontAsset);
					if (flag2)
					{
						FontAssetFactory.visitedFontAssets.Add(fontAsset);
						if (resultFontAsset.fallbackFontAssetTable == null)
						{
							resultFontAsset.fallbackFontAssetTable = new List<FontAsset>();
						}
						FontAsset fontAsset2 = FontAssetFactory.CloneFontAssetWithBitmapRenderingInternal(fontAsset, fontSize, isRaster);
						bool flag3 = fontAsset2;
						if (flag3)
						{
							resultFontAsset.fallbackFontAssetTable.Add(fontAsset2);
						}
					}
				}
			}
		}

		[return: global::System.Runtime.CompilerServices.Nullable(2)]
		internal static FontAsset ConvertFontToFontAsset(Font font)
		{
			bool flag = font == null;
			FontAsset fontAsset;
			if (flag)
			{
				fontAsset = null;
			}
			else
			{
				FontAsset fontAsset2 = FontAsset.CreateFontAsset(font, 90, 9, GlyphRenderMode.DEFAULT, 1024, 1024, AtlasPopulationMode.Dynamic, true);
				bool flag2 = fontAsset2 != null;
				if (flag2)
				{
					FontAssetFactory.SetupFontAssetSettings(fontAsset2);
				}
				fontAsset = fontAsset2;
			}
			return fontAsset;
		}

		internal static void SetupFontAssetSettings(FontAsset fontAsset)
		{
			bool flag = !fontAsset;
			if (!flag)
			{
				FontAssetFactory.SetHideFlags(fontAsset);
				fontAsset.isMultiAtlasTexturesEnabled = true;
				fontAsset.IsEditorFont = true;
			}
		}

		private static void SetupFontAssetForBitmapSettings(FontAsset fontAsset)
		{
			bool flag = !fontAsset;
			if (!flag)
			{
				FontAssetFactory.SetHideFlags(fontAsset);
				fontAsset.IsEditorFont = true;
				fontAsset.isMultiAtlasTexturesEnabled = true;
				fontAsset.atlasTexture.filterMode = (TextGenerator.EnableCheckerboardPattern ? FilterMode.Bilinear : FilterMode.Point);
			}
		}

		public static void SetHideFlags(FontAsset fontAsset)
		{
			bool flag = !fontAsset;
			if (!flag)
			{
				fontAsset.hideFlags = HideFlags.DontSave;
				fontAsset.atlasTextures[0].hideFlags = HideFlags.DontSave;
				fontAsset.material.hideFlags = HideFlags.DontSave;
			}
		}

		internal const GlyphRenderMode k_SmoothEditorBitmapGlyphRenderMode = GlyphRenderMode.SMOOTH_HINTED;

		internal const GlyphRenderMode k_RasterEditorBitmapGlyphRenderMode = GlyphRenderMode.RASTER_HINTED;

		private static readonly HashSet<FontAsset> visitedFontAssets = new HashSet<FontAsset>();
	}
}
