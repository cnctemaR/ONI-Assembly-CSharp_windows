using System;
using System.Collections.Generic;

namespace UnityEngine.TextCore
{
	internal static class FontUtilities
	{
		internal static Character GetCharacterFromFontAsset(uint unicode, FontAsset sourceFontAsset, bool includeFallbacks, FontStyles fontStyle, FontWeight fontWeight, out bool isAlternativeTypeface, out FontAsset fontAsset)
		{
			if (includeFallbacks)
			{
				bool flag = FontUtilities.s_SearchedFontAssets == null;
				if (flag)
				{
					FontUtilities.s_SearchedFontAssets = new List<int>();
				}
				else
				{
					FontUtilities.s_SearchedFontAssets.Clear();
				}
			}
			return FontUtilities.GetCharacterFromFontAsset_Internal(unicode, sourceFontAsset, includeFallbacks, fontStyle, fontWeight, out isAlternativeTypeface, out fontAsset);
		}

		internal static Character GetCharacterFromFontAssets(uint unicode, List<FontAsset> fontAssets, bool includeFallbacks, FontStyles fontStyle, FontWeight fontWeight, out bool isAlternativeTypeface, out FontAsset fontAsset)
		{
			isAlternativeTypeface = false;
			bool flag = fontAssets == null || fontAssets.Count == 0;
			Character character;
			if (flag)
			{
				fontAsset = null;
				character = null;
			}
			else
			{
				if (includeFallbacks)
				{
					bool flag2 = FontUtilities.s_SearchedFontAssets == null;
					if (flag2)
					{
						FontUtilities.s_SearchedFontAssets = new List<int>();
					}
					else
					{
						FontUtilities.s_SearchedFontAssets.Clear();
					}
				}
				int count = fontAssets.Count;
				for (int i = 0; i < count; i++)
				{
					bool flag3 = fontAssets[i] == null;
					if (!flag3)
					{
						Character characterFromFontAsset_Internal = FontUtilities.GetCharacterFromFontAsset_Internal(unicode, fontAssets[i], includeFallbacks, fontStyle, fontWeight, out isAlternativeTypeface, out fontAsset);
						bool flag4 = characterFromFontAsset_Internal != null;
						if (flag4)
						{
							return characterFromFontAsset_Internal;
						}
					}
				}
				fontAsset = null;
				character = null;
			}
			return character;
		}

		private static Character GetCharacterFromFontAsset_Internal(uint unicode, FontAsset sourceFontAsset, bool includeFallbacks, FontStyles fontStyle, FontWeight fontWeight, out bool isAlternativeTypeface, out FontAsset fontAsset)
		{
			fontAsset = null;
			isAlternativeTypeface = false;
			Character character = null;
			bool flag = (fontStyle & FontStyles.Italic) == FontStyles.Italic;
			bool flag2 = flag || fontWeight != FontWeight.Regular;
			if (flag2)
			{
				FontWeights[] fontWeightTable = sourceFontAsset.fontWeightTable;
				if (fontWeight <= FontWeight.Regular)
				{
					if (fontWeight <= FontWeight.ExtraLight)
					{
						if (fontWeight != FontWeight.Thin)
						{
							if (fontWeight == FontWeight.ExtraLight)
							{
								fontAsset = (flag ? fontWeightTable[2].italicTypeface : fontWeightTable[2].regularTypeface);
							}
						}
						else
						{
							fontAsset = (flag ? fontWeightTable[1].italicTypeface : fontWeightTable[1].regularTypeface);
						}
					}
					else if (fontWeight != FontWeight.Light)
					{
						if (fontWeight == FontWeight.Regular)
						{
							fontAsset = (flag ? fontWeightTable[4].italicTypeface : fontWeightTable[4].regularTypeface);
						}
					}
					else
					{
						fontAsset = (flag ? fontWeightTable[3].italicTypeface : fontWeightTable[3].regularTypeface);
					}
				}
				else if (fontWeight <= FontWeight.SemiBold)
				{
					if (fontWeight != FontWeight.Medium)
					{
						if (fontWeight == FontWeight.SemiBold)
						{
							fontAsset = (flag ? fontWeightTable[6].italicTypeface : fontWeightTable[6].regularTypeface);
						}
					}
					else
					{
						fontAsset = (flag ? fontWeightTable[5].italicTypeface : fontWeightTable[5].regularTypeface);
					}
				}
				else if (fontWeight != FontWeight.Bold)
				{
					if (fontWeight != FontWeight.Heavy)
					{
						if (fontWeight == FontWeight.Black)
						{
							fontAsset = (flag ? fontWeightTable[9].italicTypeface : fontWeightTable[9].regularTypeface);
						}
					}
					else
					{
						fontAsset = (flag ? fontWeightTable[8].italicTypeface : fontWeightTable[8].regularTypeface);
					}
				}
				else
				{
					fontAsset = (flag ? fontWeightTable[7].italicTypeface : fontWeightTable[7].regularTypeface);
				}
				bool flag3 = fontAsset != null;
				if (flag3)
				{
					bool flag4 = fontAsset.characterLookupTable.TryGetValue(unicode, out character);
					if (flag4)
					{
						isAlternativeTypeface = true;
						return character;
					}
					bool flag5 = fontAsset.atlasPopulationMode == FontAsset.AtlasPopulationMode.Dynamic;
					if (flag5)
					{
						bool flag6 = fontAsset.TryAddCharacter(unicode, out character);
						if (flag6)
						{
							isAlternativeTypeface = true;
							return character;
						}
					}
				}
			}
			bool flag7 = sourceFontAsset.characterLookupTable.TryGetValue(unicode, out character);
			Character character2;
			if (flag7)
			{
				fontAsset = sourceFontAsset;
				character2 = character;
			}
			else
			{
				bool flag8 = sourceFontAsset.atlasPopulationMode == FontAsset.AtlasPopulationMode.Dynamic;
				if (flag8)
				{
					bool flag9 = sourceFontAsset.TryAddCharacter(unicode, out character);
					if (flag9)
					{
						fontAsset = sourceFontAsset;
						return character;
					}
				}
				bool flag10 = character == null && includeFallbacks && sourceFontAsset.fallbackFontAssetTable != null;
				if (flag10)
				{
					List<FontAsset> fallbackFontAssetTable = sourceFontAsset.fallbackFontAssetTable;
					int count = fallbackFontAssetTable.Count;
					bool flag11 = fallbackFontAssetTable != null && count > 0;
					if (flag11)
					{
						int num = 0;
						while (num < count && character == null)
						{
							FontAsset fontAsset2 = fallbackFontAssetTable[num];
							bool flag12 = fontAsset2 == null;
							if (!flag12)
							{
								int instanceID = fontAsset2.GetInstanceID();
								bool flag13 = FontUtilities.s_SearchedFontAssets.Contains(instanceID);
								if (!flag13)
								{
									FontUtilities.s_SearchedFontAssets.Add(instanceID);
									character = FontUtilities.GetCharacterFromFontAsset_Internal(unicode, fontAsset2, includeFallbacks, fontStyle, fontWeight, out isAlternativeTypeface, out fontAsset);
									bool flag14 = character != null;
									if (flag14)
									{
										return character;
									}
								}
							}
							num++;
						}
					}
				}
				character2 = null;
			}
			return character2;
		}

		private static List<int> s_SearchedFontAssets;
	}
}
