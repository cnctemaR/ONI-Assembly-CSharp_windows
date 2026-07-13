using System;
using System.Collections.Generic;
using UnityEngine;

namespace TMPro
{
	public class TMP_ResourceManager
	{
		internal static TMP_Settings GetTextSettings()
		{
			if (TMP_ResourceManager.s_TextSettings == null)
			{
				TMP_ResourceManager.s_TextSettings = Resources.Load<TMP_Settings>("TextSettings");
			}
			return TMP_ResourceManager.s_TextSettings;
		}

		public static void AddFontAsset(TMP_FontAsset fontAsset)
		{
			int instanceID = fontAsset.instanceID;
			if (!TMP_ResourceManager.s_FontAssetReferences.ContainsKey(instanceID))
			{
				TMP_ResourceManager.FontAssetRef fontAssetRef = new TMP_ResourceManager.FontAssetRef(fontAsset.hashCode, fontAsset.familyNameHashCode, fontAsset.styleNameHashCode, fontAsset);
				TMP_ResourceManager.s_FontAssetReferences.Add(instanceID, fontAssetRef);
				if (!TMP_ResourceManager.s_FontAssetNameReferenceLookup.ContainsKey(fontAssetRef.nameHashCode))
				{
					TMP_ResourceManager.s_FontAssetNameReferenceLookup.Add(fontAssetRef.nameHashCode, fontAsset);
				}
				if (!TMP_ResourceManager.s_FontAssetFamilyNameAndStyleReferenceLookup.ContainsKey(fontAssetRef.familyNameAndStyleHashCode))
				{
					TMP_ResourceManager.s_FontAssetFamilyNameAndStyleReferenceLookup.Add(fontAssetRef.familyNameAndStyleHashCode, fontAsset);
					return;
				}
			}
			else
			{
				TMP_ResourceManager.FontAssetRef fontAssetRef2 = TMP_ResourceManager.s_FontAssetReferences[instanceID];
				if (fontAssetRef2.nameHashCode == fontAsset.hashCode && fontAssetRef2.familyNameHashCode == fontAsset.familyNameHashCode && fontAssetRef2.styleNameHashCode == fontAsset.styleNameHashCode)
				{
					return;
				}
				if (fontAssetRef2.nameHashCode != fontAsset.hashCode)
				{
					TMP_ResourceManager.s_FontAssetNameReferenceLookup.Remove(fontAssetRef2.nameHashCode);
					fontAssetRef2.nameHashCode = fontAsset.hashCode;
					if (!TMP_ResourceManager.s_FontAssetNameReferenceLookup.ContainsKey(fontAssetRef2.nameHashCode))
					{
						TMP_ResourceManager.s_FontAssetNameReferenceLookup.Add(fontAssetRef2.nameHashCode, fontAsset);
					}
				}
				if (fontAssetRef2.familyNameHashCode != fontAsset.familyNameHashCode || fontAssetRef2.styleNameHashCode != fontAsset.styleNameHashCode)
				{
					TMP_ResourceManager.s_FontAssetFamilyNameAndStyleReferenceLookup.Remove(fontAssetRef2.familyNameAndStyleHashCode);
					fontAssetRef2.familyNameHashCode = fontAsset.familyNameHashCode;
					fontAssetRef2.styleNameHashCode = fontAsset.styleNameHashCode;
					fontAssetRef2.familyNameAndStyleHashCode = ((long)fontAsset.styleNameHashCode << 32) | (long)((ulong)fontAsset.familyNameHashCode);
					if (!TMP_ResourceManager.s_FontAssetFamilyNameAndStyleReferenceLookup.ContainsKey(fontAssetRef2.familyNameAndStyleHashCode))
					{
						TMP_ResourceManager.s_FontAssetFamilyNameAndStyleReferenceLookup.Add(fontAssetRef2.familyNameAndStyleHashCode, fontAsset);
					}
				}
				TMP_ResourceManager.s_FontAssetReferences[instanceID] = fontAssetRef2;
			}
		}

		public static void RemoveFontAsset(TMP_FontAsset fontAsset)
		{
			int instanceID = fontAsset.instanceID;
			TMP_ResourceManager.FontAssetRef fontAssetRef;
			if (TMP_ResourceManager.s_FontAssetReferences.TryGetValue(instanceID, out fontAssetRef))
			{
				TMP_ResourceManager.s_FontAssetNameReferenceLookup.Remove(fontAssetRef.nameHashCode);
				TMP_ResourceManager.s_FontAssetFamilyNameAndStyleReferenceLookup.Remove(fontAssetRef.familyNameAndStyleHashCode);
				TMP_ResourceManager.s_FontAssetReferences.Remove(instanceID);
			}
		}

		internal static bool TryGetFontAssetByName(int nameHashcode, out TMP_FontAsset fontAsset)
		{
			fontAsset = null;
			return TMP_ResourceManager.s_FontAssetNameReferenceLookup.TryGetValue(nameHashcode, out fontAsset);
		}

		internal static bool TryGetFontAssetByFamilyName(int familyNameHashCode, int styleNameHashCode, out TMP_FontAsset fontAsset)
		{
			fontAsset = null;
			if (styleNameHashCode == 0)
			{
				styleNameHashCode = TMP_ResourceManager.k_RegularStyleHashCode;
			}
			long num = ((long)styleNameHashCode << 32) | (long)((ulong)familyNameHashCode);
			return TMP_ResourceManager.s_FontAssetFamilyNameAndStyleReferenceLookup.TryGetValue(num, out fontAsset);
		}

		public static void ClearFontAssetGlyphCache()
		{
			TMP_ResourceManager.RebuildFontAssetCache();
		}

		internal static void RebuildFontAssetCache()
		{
			foreach (KeyValuePair<int, TMP_ResourceManager.FontAssetRef> keyValuePair in TMP_ResourceManager.s_FontAssetReferences)
			{
				TMP_ResourceManager.FontAssetRef value = keyValuePair.Value;
				TMP_FontAsset fontAsset = value.fontAsset;
				if (fontAsset == null)
				{
					TMP_ResourceManager.s_FontAssetNameReferenceLookup.Remove(value.nameHashCode);
					TMP_ResourceManager.s_FontAssetFamilyNameAndStyleReferenceLookup.Remove(value.familyNameAndStyleHashCode);
					TMP_ResourceManager.s_FontAssetRemovalList.Add(keyValuePair.Key);
				}
				else
				{
					fontAsset.InitializeCharacterLookupDictionary();
					fontAsset.AddSynthesizedCharactersAndFaceMetrics();
				}
			}
			for (int i = 0; i < TMP_ResourceManager.s_FontAssetRemovalList.Count; i++)
			{
				TMP_ResourceManager.s_FontAssetReferences.Remove(TMP_ResourceManager.s_FontAssetRemovalList[i]);
			}
			TMP_ResourceManager.s_FontAssetRemovalList.Clear();
			TMPro_EventManager.ON_FONT_PROPERTY_CHANGED(true, null);
		}

		private static TMP_Settings s_TextSettings;

		private static readonly Dictionary<int, TMP_ResourceManager.FontAssetRef> s_FontAssetReferences = new Dictionary<int, TMP_ResourceManager.FontAssetRef>();

		private static readonly Dictionary<int, TMP_FontAsset> s_FontAssetNameReferenceLookup = new Dictionary<int, TMP_FontAsset>();

		private static readonly Dictionary<long, TMP_FontAsset> s_FontAssetFamilyNameAndStyleReferenceLookup = new Dictionary<long, TMP_FontAsset>();

		private static readonly List<int> s_FontAssetRemovalList = new List<int>(16);

		private static readonly int k_RegularStyleHashCode = TMP_TextUtilities.GetHashCode("Regular");

		private struct FontAssetRef
		{
			public FontAssetRef(int nameHashCode, int familyNameHashCode, int styleNameHashCode, TMP_FontAsset fontAsset)
			{
				this.nameHashCode = ((nameHashCode != 0) ? nameHashCode : familyNameHashCode);
				this.familyNameHashCode = familyNameHashCode;
				this.styleNameHashCode = styleNameHashCode;
				this.familyNameAndStyleHashCode = ((long)styleNameHashCode << 32) | (long)((ulong)familyNameHashCode);
				this.fontAsset = fontAsset;
			}

			public int nameHashCode;

			public int familyNameHashCode;

			public int styleNameHashCode;

			public long familyNameAndStyleHashCode;

			public readonly TMP_FontAsset fontAsset;
		}
	}
}
