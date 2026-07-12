using System;
using System.Collections.Generic;

namespace UnityEngine.TextCore.Text
{
	internal static class FontAssetUtilities
	{
		internal static Character GetCharacterFromFontAsset(uint unicode, FontAsset sourceFontAsset, bool includeFallbacks, FontStyles fontStyle, TextFontWeight fontWeight, out bool isAlternativeTypeface)
		{
			if (includeFallbacks)
			{
				bool flag = FontAssetUtilities.k_SearchedAssets == null;
				if (flag)
				{
					FontAssetUtilities.k_SearchedAssets = new HashSet<int>();
				}
				else
				{
					FontAssetUtilities.k_SearchedAssets.Clear();
				}
			}
			return FontAssetUtilities.GetCharacterFromFontAsset_Internal(unicode, sourceFontAsset, includeFallbacks, fontStyle, fontWeight, out isAlternativeTypeface);
		}

		private static Character GetCharacterFromFontAsset_Internal(uint unicode, FontAsset sourceFontAsset, bool includeFallbacks, FontStyles fontStyle, TextFontWeight fontWeight, out bool isAlternativeTypeface)
		{
			isAlternativeTypeface = false;
			Character character = null;
			bool flag = (fontStyle & FontStyles.Italic) == FontStyles.Italic;
			bool flag2 = flag || fontWeight != TextFontWeight.Regular;
			if (flag2)
			{
				FontWeightPair[] fontWeightTable = sourceFontAsset.fontWeightTable;
				int num = 4;
				if (fontWeight <= TextFontWeight.Regular)
				{
					if (fontWeight <= TextFontWeight.ExtraLight)
					{
						if (fontWeight != TextFontWeight.Thin)
						{
							if (fontWeight == TextFontWeight.ExtraLight)
							{
								num = 2;
							}
						}
						else
						{
							num = 1;
						}
					}
					else if (fontWeight != TextFontWeight.Light)
					{
						if (fontWeight == TextFontWeight.Regular)
						{
							num = 4;
						}
					}
					else
					{
						num = 3;
					}
				}
				else if (fontWeight <= TextFontWeight.SemiBold)
				{
					if (fontWeight != TextFontWeight.Medium)
					{
						if (fontWeight == TextFontWeight.SemiBold)
						{
							num = 6;
						}
					}
					else
					{
						num = 5;
					}
				}
				else if (fontWeight != TextFontWeight.Bold)
				{
					if (fontWeight != TextFontWeight.Heavy)
					{
						if (fontWeight == TextFontWeight.Black)
						{
							num = 9;
						}
					}
					else
					{
						num = 8;
					}
				}
				else
				{
					num = 7;
				}
				FontAsset fontAsset = (flag ? fontWeightTable[num].italicTypeface : fontWeightTable[num].regularTypeface);
				bool flag3 = fontAsset != null;
				if (flag3)
				{
					bool flag4 = fontAsset.characterLookupTable.TryGetValue(unicode, out character);
					if (flag4)
					{
						bool flag5 = character.textAsset != null;
						if (flag5)
						{
							isAlternativeTypeface = true;
							return character;
						}
						fontAsset.characterLookupTable.Remove(unicode);
					}
					bool flag6 = fontAsset.atlasPopulationMode == AtlasPopulationMode.Dynamic || fontAsset.atlasPopulationMode == AtlasPopulationMode.DynamicOS;
					if (flag6)
					{
						bool flag7 = fontAsset.TryAddCharacterInternal(unicode, out character, false);
						if (flag7)
						{
							isAlternativeTypeface = true;
							return character;
						}
					}
				}
			}
			bool flag8 = sourceFontAsset.characterLookupTable.TryGetValue(unicode, out character);
			if (flag8)
			{
				bool flag9 = character.textAsset != null;
				if (flag9)
				{
					return character;
				}
				sourceFontAsset.characterLookupTable.Remove(unicode);
			}
			bool flag10 = sourceFontAsset.atlasPopulationMode == AtlasPopulationMode.Dynamic || sourceFontAsset.atlasPopulationMode == AtlasPopulationMode.DynamicOS;
			if (flag10)
			{
				bool flag11 = sourceFontAsset.TryAddCharacterInternal(unicode, out character, false);
				if (flag11)
				{
					return character;
				}
			}
			bool flag12 = character == null && includeFallbacks && sourceFontAsset.fallbackFontAssetTable != null;
			if (flag12)
			{
				List<FontAsset> fallbackFontAssetTable = sourceFontAsset.fallbackFontAssetTable;
				int count = fallbackFontAssetTable.Count;
				bool flag13 = count == 0;
				if (flag13)
				{
					return null;
				}
				for (int i = 0; i < count; i++)
				{
					FontAsset fontAsset2 = fallbackFontAssetTable[i];
					bool flag14 = fontAsset2 == null;
					if (!flag14)
					{
						int instanceID = fontAsset2.instanceID;
						bool flag15 = !FontAssetUtilities.k_SearchedAssets.Add(instanceID);
						if (!flag15)
						{
							character = FontAssetUtilities.GetCharacterFromFontAsset_Internal(unicode, fontAsset2, true, fontStyle, fontWeight, out isAlternativeTypeface);
							bool flag16 = character != null;
							if (flag16)
							{
								return character;
							}
						}
					}
				}
			}
			return null;
		}

		public static Character GetCharacterFromFontAssets(uint unicode, FontAsset sourceFontAsset, List<FontAsset> fontAssets, bool includeFallbacks, FontStyles fontStyle, TextFontWeight fontWeight, out bool isAlternativeTypeface)
		{
			isAlternativeTypeface = false;
			bool flag = fontAssets == null || fontAssets.Count == 0;
			Character character;
			if (flag)
			{
				character = null;
			}
			else
			{
				if (includeFallbacks)
				{
					bool flag2 = FontAssetUtilities.k_SearchedAssets == null;
					if (flag2)
					{
						FontAssetUtilities.k_SearchedAssets = new HashSet<int>();
					}
					else
					{
						FontAssetUtilities.k_SearchedAssets.Clear();
					}
				}
				int count = fontAssets.Count;
				for (int i = 0; i < count; i++)
				{
					FontAsset fontAsset = fontAssets[i];
					bool flag3 = fontAsset == null;
					if (!flag3)
					{
						Character characterFromFontAsset_Internal = FontAssetUtilities.GetCharacterFromFontAsset_Internal(unicode, fontAsset, includeFallbacks, fontStyle, fontWeight, out isAlternativeTypeface);
						bool flag4 = characterFromFontAsset_Internal != null;
						if (flag4)
						{
							return characterFromFontAsset_Internal;
						}
					}
				}
				character = null;
			}
			return character;
		}

		public static SpriteCharacter GetSpriteCharacterFromSpriteAsset(uint unicode, SpriteAsset spriteAsset, bool includeFallbacks)
		{
			bool flag = spriteAsset == null;
			SpriteCharacter spriteCharacter;
			if (flag)
			{
				spriteCharacter = null;
			}
			else
			{
				SpriteCharacter spriteCharacterFromSpriteAsset_Internal;
				bool flag2 = spriteAsset.spriteCharacterLookupTable.TryGetValue(unicode, out spriteCharacterFromSpriteAsset_Internal);
				if (flag2)
				{
					spriteCharacter = spriteCharacterFromSpriteAsset_Internal;
				}
				else
				{
					if (includeFallbacks)
					{
						bool flag3 = FontAssetUtilities.k_SearchedAssets == null;
						if (flag3)
						{
							FontAssetUtilities.k_SearchedAssets = new HashSet<int>();
						}
						else
						{
							FontAssetUtilities.k_SearchedAssets.Clear();
						}
						FontAssetUtilities.k_SearchedAssets.Add(spriteAsset.instanceID);
						List<SpriteAsset> fallbackSpriteAssets = spriteAsset.fallbackSpriteAssets;
						bool flag4 = fallbackSpriteAssets != null && fallbackSpriteAssets.Count > 0;
						if (flag4)
						{
							int count = fallbackSpriteAssets.Count;
							for (int i = 0; i < count; i++)
							{
								SpriteAsset spriteAsset2 = fallbackSpriteAssets[i];
								bool flag5 = spriteAsset2 == null;
								if (!flag5)
								{
									int instanceID = spriteAsset2.instanceID;
									bool flag6 = !FontAssetUtilities.k_SearchedAssets.Add(instanceID);
									if (!flag6)
									{
										spriteCharacterFromSpriteAsset_Internal = FontAssetUtilities.GetSpriteCharacterFromSpriteAsset_Internal(unicode, spriteAsset2, true);
										bool flag7 = spriteCharacterFromSpriteAsset_Internal != null;
										if (flag7)
										{
											return spriteCharacterFromSpriteAsset_Internal;
										}
									}
								}
							}
						}
					}
					spriteCharacter = null;
				}
			}
			return spriteCharacter;
		}

		private static SpriteCharacter GetSpriteCharacterFromSpriteAsset_Internal(uint unicode, SpriteAsset spriteAsset, bool includeFallbacks)
		{
			SpriteCharacter spriteCharacterFromSpriteAsset_Internal;
			bool flag = spriteAsset.spriteCharacterLookupTable.TryGetValue(unicode, out spriteCharacterFromSpriteAsset_Internal);
			SpriteCharacter spriteCharacter;
			if (flag)
			{
				spriteCharacter = spriteCharacterFromSpriteAsset_Internal;
			}
			else
			{
				if (includeFallbacks)
				{
					List<SpriteAsset> fallbackSpriteAssets = spriteAsset.fallbackSpriteAssets;
					bool flag2 = fallbackSpriteAssets != null && fallbackSpriteAssets.Count > 0;
					if (flag2)
					{
						int count = fallbackSpriteAssets.Count;
						for (int i = 0; i < count; i++)
						{
							SpriteAsset spriteAsset2 = fallbackSpriteAssets[i];
							bool flag3 = spriteAsset2 == null;
							if (!flag3)
							{
								int instanceID = spriteAsset2.instanceID;
								bool flag4 = !FontAssetUtilities.k_SearchedAssets.Add(instanceID);
								if (!flag4)
								{
									spriteCharacterFromSpriteAsset_Internal = FontAssetUtilities.GetSpriteCharacterFromSpriteAsset_Internal(unicode, spriteAsset2, true);
									bool flag5 = spriteCharacterFromSpriteAsset_Internal != null;
									if (flag5)
									{
										return spriteCharacterFromSpriteAsset_Internal;
									}
								}
							}
						}
					}
				}
				spriteCharacter = null;
			}
			return spriteCharacter;
		}

		private static HashSet<int> k_SearchedAssets;
	}
}
