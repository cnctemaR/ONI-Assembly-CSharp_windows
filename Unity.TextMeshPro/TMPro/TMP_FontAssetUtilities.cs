using System;
using System.Collections.Generic;

namespace TMPro
{
	public class TMP_FontAssetUtilities
	{
		public static TMP_FontAssetUtilities instance
		{
			get
			{
				return TMP_FontAssetUtilities.s_Instance;
			}
		}

		public static TMP_Character GetCharacterFromFontAsset(uint unicode, TMP_FontAsset sourceFontAsset, bool includeFallbacks, FontStyles fontStyle, FontWeight fontWeight, out bool isAlternativeTypeface)
		{
			if (includeFallbacks)
			{
				if (TMP_FontAssetUtilities.k_SearchedAssets == null)
				{
					TMP_FontAssetUtilities.k_SearchedAssets = new HashSet<int>();
				}
				else
				{
					TMP_FontAssetUtilities.k_SearchedAssets.Clear();
				}
			}
			return TMP_FontAssetUtilities.GetCharacterFromFontAsset_Internal(unicode, sourceFontAsset, includeFallbacks, fontStyle, fontWeight, out isAlternativeTypeface);
		}

		private static TMP_Character GetCharacterFromFontAsset_Internal(uint unicode, TMP_FontAsset sourceFontAsset, bool includeFallbacks, FontStyles fontStyle, FontWeight fontWeight, out bool isAlternativeTypeface)
		{
			isAlternativeTypeface = false;
			bool flag = (fontStyle & FontStyles.Italic) == FontStyles.Italic;
			TMP_Character tmp_Character;
			AtlasPopulationMode atlasPopulationMode;
			if (flag || fontWeight != FontWeight.Regular)
			{
				uint num = (uint)(((uint)(FontStyles.Superscript | ((uint)fontStyle << 4) | (FontStyles)(fontWeight / FontWeight.Thin)) << 24) | (FontStyles)unicode);
				if (sourceFontAsset.characterLookupTable.TryGetValue(num, out tmp_Character))
				{
					isAlternativeTypeface = true;
					if (tmp_Character.textAsset != null)
					{
						return tmp_Character;
					}
					sourceFontAsset.characterLookupTable.Remove(unicode);
				}
				TMP_FontWeightPair[] fontWeightTable = sourceFontAsset.fontWeightTable;
				int num2 = 4;
				if (fontWeight <= FontWeight.Regular)
				{
					if (fontWeight <= FontWeight.ExtraLight)
					{
						if (fontWeight != FontWeight.Thin)
						{
							if (fontWeight == FontWeight.ExtraLight)
							{
								num2 = 2;
							}
						}
						else
						{
							num2 = 1;
						}
					}
					else if (fontWeight != FontWeight.Light)
					{
						if (fontWeight == FontWeight.Regular)
						{
							num2 = 4;
						}
					}
					else
					{
						num2 = 3;
					}
				}
				else if (fontWeight <= FontWeight.SemiBold)
				{
					if (fontWeight != FontWeight.Medium)
					{
						if (fontWeight == FontWeight.SemiBold)
						{
							num2 = 6;
						}
					}
					else
					{
						num2 = 5;
					}
				}
				else if (fontWeight != FontWeight.Bold)
				{
					if (fontWeight != FontWeight.Heavy)
					{
						if (fontWeight == FontWeight.Black)
						{
							num2 = 9;
						}
					}
					else
					{
						num2 = 8;
					}
				}
				else
				{
					num2 = 7;
				}
				TMP_FontAsset tmp_FontAsset = (flag ? fontWeightTable[num2].italicTypeface : fontWeightTable[num2].regularTypeface);
				if (tmp_FontAsset != null)
				{
					if (tmp_FontAsset.characterLookupTable.TryGetValue(unicode, out tmp_Character))
					{
						if (tmp_Character.textAsset != null)
						{
							isAlternativeTypeface = true;
							return tmp_Character;
						}
						tmp_FontAsset.characterLookupTable.Remove(unicode);
					}
					atlasPopulationMode = tmp_FontAsset.atlasPopulationMode;
					if ((atlasPopulationMode == AtlasPopulationMode.Dynamic || atlasPopulationMode == AtlasPopulationMode.DynamicOS) && tmp_FontAsset.TryAddCharacterInternal(unicode, out tmp_Character))
					{
						isAlternativeTypeface = true;
						return tmp_Character;
					}
				}
				if (includeFallbacks)
				{
					List<TMP_FontAsset> list = sourceFontAsset.fallbackFontAssetTable;
					if (list != null && list.Count > 0)
					{
						return TMP_FontAssetUtilities.SearchFallbacksForCharacter(unicode, sourceFontAsset, fontStyle, fontWeight, out isAlternativeTypeface);
					}
				}
				return null;
			}
			if (sourceFontAsset.characterLookupTable.TryGetValue(unicode, out tmp_Character))
			{
				if (tmp_Character.textAsset != null)
				{
					return tmp_Character;
				}
				sourceFontAsset.characterLookupTable.Remove(unicode);
			}
			atlasPopulationMode = sourceFontAsset.atlasPopulationMode;
			if ((atlasPopulationMode == AtlasPopulationMode.Dynamic || atlasPopulationMode == AtlasPopulationMode.DynamicOS) && sourceFontAsset.TryAddCharacterInternal(unicode, out tmp_Character))
			{
				return tmp_Character;
			}
			if (includeFallbacks)
			{
				List<TMP_FontAsset> list = sourceFontAsset.fallbackFontAssetTable;
				if (list != null && list.Count > 0)
				{
					return TMP_FontAssetUtilities.SearchFallbacksForCharacter(unicode, sourceFontAsset, fontStyle, fontWeight, out isAlternativeTypeface);
				}
			}
			return null;
		}

		private static TMP_Character SearchFallbacksForCharacter(uint unicode, TMP_FontAsset sourceFontAsset, FontStyles fontStyle, FontWeight fontWeight, out bool isAlternativeTypeface)
		{
			isAlternativeTypeface = false;
			List<TMP_FontAsset> fallbackFontAssetTable = sourceFontAsset.fallbackFontAssetTable;
			int count = fallbackFontAssetTable.Count;
			if (count == 0)
			{
				return null;
			}
			for (int i = 0; i < count; i++)
			{
				TMP_FontAsset tmp_FontAsset = fallbackFontAssetTable[i];
				if (!(tmp_FontAsset == null))
				{
					int instanceID = tmp_FontAsset.instanceID;
					if (TMP_FontAssetUtilities.k_SearchedAssets.Add(instanceID))
					{
						TMP_Character characterFromFontAsset_Internal = TMP_FontAssetUtilities.GetCharacterFromFontAsset_Internal(unicode, tmp_FontAsset, true, fontStyle, fontWeight, out isAlternativeTypeface);
						if (characterFromFontAsset_Internal != null)
						{
							return characterFromFontAsset_Internal;
						}
					}
				}
			}
			return null;
		}

		public static TMP_Character GetCharacterFromFontAssets(uint unicode, TMP_FontAsset sourceFontAsset, List<TMP_FontAsset> fontAssets, bool includeFallbacks, FontStyles fontStyle, FontWeight fontWeight, out bool isAlternativeTypeface)
		{
			isAlternativeTypeface = false;
			if (fontAssets == null || fontAssets.Count == 0)
			{
				return null;
			}
			if (includeFallbacks)
			{
				if (TMP_FontAssetUtilities.k_SearchedAssets == null)
				{
					TMP_FontAssetUtilities.k_SearchedAssets = new HashSet<int>();
				}
				else
				{
					TMP_FontAssetUtilities.k_SearchedAssets.Clear();
				}
			}
			int count = fontAssets.Count;
			for (int i = 0; i < count; i++)
			{
				TMP_FontAsset tmp_FontAsset = fontAssets[i];
				if (!(tmp_FontAsset == null))
				{
					TMP_Character characterFromFontAsset_Internal = TMP_FontAssetUtilities.GetCharacterFromFontAsset_Internal(unicode, tmp_FontAsset, includeFallbacks, fontStyle, fontWeight, out isAlternativeTypeface);
					if (characterFromFontAsset_Internal != null)
					{
						return characterFromFontAsset_Internal;
					}
				}
			}
			return null;
		}

		internal static TMP_TextElement GetTextElementFromTextAssets(uint unicode, TMP_FontAsset sourceFontAsset, List<TMP_Asset> textAssets, bool includeFallbacks, FontStyles fontStyle, FontWeight fontWeight, out bool isAlternativeTypeface)
		{
			isAlternativeTypeface = false;
			if (textAssets == null || textAssets.Count == 0)
			{
				return null;
			}
			if (includeFallbacks)
			{
				if (TMP_FontAssetUtilities.k_SearchedAssets == null)
				{
					TMP_FontAssetUtilities.k_SearchedAssets = new HashSet<int>();
				}
				else
				{
					TMP_FontAssetUtilities.k_SearchedAssets.Clear();
				}
			}
			int count = textAssets.Count;
			for (int i = 0; i < count; i++)
			{
				TMP_Asset tmp_Asset = textAssets[i];
				if (!(tmp_Asset == null))
				{
					if (tmp_Asset.GetType() == typeof(TMP_FontAsset))
					{
						TMP_FontAsset tmp_FontAsset = tmp_Asset as TMP_FontAsset;
						TMP_Character characterFromFontAsset_Internal = TMP_FontAssetUtilities.GetCharacterFromFontAsset_Internal(unicode, tmp_FontAsset, includeFallbacks, fontStyle, fontWeight, out isAlternativeTypeface);
						if (characterFromFontAsset_Internal != null)
						{
							return characterFromFontAsset_Internal;
						}
					}
					else
					{
						TMP_SpriteAsset tmp_SpriteAsset = tmp_Asset as TMP_SpriteAsset;
						TMP_SpriteCharacter spriteCharacterFromSpriteAsset_Internal = TMP_FontAssetUtilities.GetSpriteCharacterFromSpriteAsset_Internal(unicode, tmp_SpriteAsset, true);
						if (spriteCharacterFromSpriteAsset_Internal != null)
						{
							return spriteCharacterFromSpriteAsset_Internal;
						}
					}
				}
			}
			return null;
		}

		public static TMP_SpriteCharacter GetSpriteCharacterFromSpriteAsset(uint unicode, TMP_SpriteAsset spriteAsset, bool includeFallbacks)
		{
			if (spriteAsset == null)
			{
				return null;
			}
			TMP_SpriteCharacter spriteCharacterFromSpriteAsset_Internal;
			if (spriteAsset.spriteCharacterLookupTable.TryGetValue(unicode, out spriteCharacterFromSpriteAsset_Internal))
			{
				return spriteCharacterFromSpriteAsset_Internal;
			}
			if (includeFallbacks)
			{
				if (TMP_FontAssetUtilities.k_SearchedAssets == null)
				{
					TMP_FontAssetUtilities.k_SearchedAssets = new HashSet<int>();
				}
				else
				{
					TMP_FontAssetUtilities.k_SearchedAssets.Clear();
				}
				TMP_FontAssetUtilities.k_SearchedAssets.Add(spriteAsset.instanceID);
				List<TMP_SpriteAsset> fallbackSpriteAssets = spriteAsset.fallbackSpriteAssets;
				if (fallbackSpriteAssets != null && fallbackSpriteAssets.Count > 0)
				{
					int count = fallbackSpriteAssets.Count;
					for (int i = 0; i < count; i++)
					{
						TMP_SpriteAsset tmp_SpriteAsset = fallbackSpriteAssets[i];
						if (!(tmp_SpriteAsset == null))
						{
							int instanceID = tmp_SpriteAsset.instanceID;
							if (TMP_FontAssetUtilities.k_SearchedAssets.Add(instanceID))
							{
								spriteCharacterFromSpriteAsset_Internal = TMP_FontAssetUtilities.GetSpriteCharacterFromSpriteAsset_Internal(unicode, tmp_SpriteAsset, true);
								if (spriteCharacterFromSpriteAsset_Internal != null)
								{
									return spriteCharacterFromSpriteAsset_Internal;
								}
							}
						}
					}
				}
			}
			return null;
		}

		private static TMP_SpriteCharacter GetSpriteCharacterFromSpriteAsset_Internal(uint unicode, TMP_SpriteAsset spriteAsset, bool includeFallbacks)
		{
			TMP_SpriteCharacter spriteCharacterFromSpriteAsset_Internal;
			if (spriteAsset.spriteCharacterLookupTable.TryGetValue(unicode, out spriteCharacterFromSpriteAsset_Internal))
			{
				return spriteCharacterFromSpriteAsset_Internal;
			}
			if (includeFallbacks)
			{
				List<TMP_SpriteAsset> fallbackSpriteAssets = spriteAsset.fallbackSpriteAssets;
				if (fallbackSpriteAssets != null && fallbackSpriteAssets.Count > 0)
				{
					int count = fallbackSpriteAssets.Count;
					for (int i = 0; i < count; i++)
					{
						TMP_SpriteAsset tmp_SpriteAsset = fallbackSpriteAssets[i];
						if (!(tmp_SpriteAsset == null))
						{
							int instanceID = tmp_SpriteAsset.instanceID;
							if (TMP_FontAssetUtilities.k_SearchedAssets.Add(instanceID))
							{
								spriteCharacterFromSpriteAsset_Internal = TMP_FontAssetUtilities.GetSpriteCharacterFromSpriteAsset_Internal(unicode, tmp_SpriteAsset, true);
								if (spriteCharacterFromSpriteAsset_Internal != null)
								{
									return spriteCharacterFromSpriteAsset_Internal;
								}
							}
						}
					}
				}
			}
			return null;
		}

		internal static uint GetCodePoint(string text, ref int index)
		{
			char c = text[index];
			if (char.IsHighSurrogate(c) && index + 1 < text.Length && char.IsLowSurrogate(text[index + 1]))
			{
				uint num = (uint)char.ConvertToUtf32(c, text[index + 1]);
				index++;
				return num;
			}
			return (uint)c;
		}

		internal static uint GetCodePoint(uint[] codesPoints, ref int index)
		{
			char c = (char)codesPoints[index];
			if (char.IsHighSurrogate(c) && index + 1 < codesPoints.Length && char.IsLowSurrogate((char)codesPoints[index + 1]))
			{
				uint num = (uint)char.ConvertToUtf32(c, (char)codesPoints[index + 1]);
				index++;
				return num;
			}
			return (uint)c;
		}

		private static readonly TMP_FontAssetUtilities s_Instance = new TMP_FontAssetUtilities();

		private static HashSet<int> k_SearchedAssets;
	}
}
