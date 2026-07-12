using System;
using System.Collections.Generic;

namespace UnityEngine.TextCore.Text
{
	internal class TextResourceManager
	{
		internal static void AddFontAsset(FontAsset fontAsset)
		{
			int instanceID = fontAsset.instanceID;
			bool flag = !TextResourceManager.s_FontAssetReferences.ContainsKey(instanceID);
			if (flag)
			{
				TextResourceManager.FontAssetRef fontAssetRef = new TextResourceManager.FontAssetRef(fontAsset.hashCode, fontAsset.familyNameHashCode, fontAsset.styleNameHashCode, fontAsset);
				TextResourceManager.s_FontAssetReferences.Add(instanceID, fontAssetRef);
				bool flag2 = !TextResourceManager.s_FontAssetNameReferenceLookup.ContainsKey(fontAssetRef.nameHashCode);
				if (flag2)
				{
					TextResourceManager.s_FontAssetNameReferenceLookup.Add(fontAssetRef.nameHashCode, fontAsset);
				}
				bool flag3 = !TextResourceManager.s_FontAssetFamilyNameAndStyleReferenceLookup.ContainsKey(fontAssetRef.familyNameAndStyleHashCode);
				if (flag3)
				{
					TextResourceManager.s_FontAssetFamilyNameAndStyleReferenceLookup.Add(fontAssetRef.familyNameAndStyleHashCode, fontAsset);
				}
			}
			else
			{
				TextResourceManager.FontAssetRef fontAssetRef2 = TextResourceManager.s_FontAssetReferences[instanceID];
				bool flag4 = fontAssetRef2.nameHashCode == fontAsset.hashCode && fontAssetRef2.familyNameHashCode == fontAsset.familyNameHashCode && fontAssetRef2.styleNameHashCode == fontAsset.styleNameHashCode;
				if (!flag4)
				{
					bool flag5 = fontAssetRef2.nameHashCode != fontAsset.hashCode;
					if (flag5)
					{
						TextResourceManager.s_FontAssetNameReferenceLookup.Remove(fontAssetRef2.nameHashCode);
						fontAssetRef2.nameHashCode = fontAsset.hashCode;
						bool flag6 = !TextResourceManager.s_FontAssetNameReferenceLookup.ContainsKey(fontAssetRef2.nameHashCode);
						if (flag6)
						{
							TextResourceManager.s_FontAssetNameReferenceLookup.Add(fontAssetRef2.nameHashCode, fontAsset);
						}
					}
					bool flag7 = fontAssetRef2.familyNameHashCode != fontAsset.familyNameHashCode || fontAssetRef2.styleNameHashCode != fontAsset.styleNameHashCode;
					if (flag7)
					{
						TextResourceManager.s_FontAssetFamilyNameAndStyleReferenceLookup.Remove(fontAssetRef2.familyNameAndStyleHashCode);
						fontAssetRef2.familyNameHashCode = fontAsset.familyNameHashCode;
						fontAssetRef2.styleNameHashCode = fontAsset.styleNameHashCode;
						fontAssetRef2.familyNameAndStyleHashCode = ((long)fontAsset.styleNameHashCode << 32) | (long)((ulong)fontAsset.familyNameHashCode);
						bool flag8 = !TextResourceManager.s_FontAssetFamilyNameAndStyleReferenceLookup.ContainsKey(fontAssetRef2.familyNameAndStyleHashCode);
						if (flag8)
						{
							TextResourceManager.s_FontAssetFamilyNameAndStyleReferenceLookup.Add(fontAssetRef2.familyNameAndStyleHashCode, fontAsset);
						}
					}
					TextResourceManager.s_FontAssetReferences[instanceID] = fontAssetRef2;
				}
			}
		}

		public static void RemoveFontAsset(FontAsset fontAsset)
		{
			int instanceID = fontAsset.instanceID;
			TextResourceManager.FontAssetRef fontAssetRef;
			bool flag = TextResourceManager.s_FontAssetReferences.TryGetValue(instanceID, out fontAssetRef);
			if (flag)
			{
				TextResourceManager.s_FontAssetNameReferenceLookup.Remove(fontAssetRef.nameHashCode);
				TextResourceManager.s_FontAssetFamilyNameAndStyleReferenceLookup.Remove(fontAssetRef.familyNameAndStyleHashCode);
				TextResourceManager.s_FontAssetReferences.Remove(instanceID);
			}
		}

		internal static bool TryGetFontAssetByName(int nameHashcode, out FontAsset fontAsset)
		{
			fontAsset = null;
			return TextResourceManager.s_FontAssetNameReferenceLookup.TryGetValue(nameHashcode, out fontAsset);
		}

		internal static bool TryGetFontAssetByFamilyName(int familyNameHashCode, int styleNameHashCode, out FontAsset fontAsset)
		{
			fontAsset = null;
			bool flag = styleNameHashCode == 0;
			if (flag)
			{
				styleNameHashCode = TextResourceManager.k_RegularStyleHashCode;
			}
			long num = ((long)styleNameHashCode << 32) | (long)((ulong)familyNameHashCode);
			return TextResourceManager.s_FontAssetFamilyNameAndStyleReferenceLookup.TryGetValue(num, out fontAsset);
		}

		internal static void RebuildFontAssetCache()
		{
			foreach (KeyValuePair<int, TextResourceManager.FontAssetRef> keyValuePair in TextResourceManager.s_FontAssetReferences)
			{
				TextResourceManager.FontAssetRef value = keyValuePair.Value;
				FontAsset fontAsset = value.fontAsset;
				bool flag = fontAsset == null;
				if (flag)
				{
					TextResourceManager.s_FontAssetNameReferenceLookup.Remove(value.nameHashCode);
					TextResourceManager.s_FontAssetFamilyNameAndStyleReferenceLookup.Remove(value.familyNameAndStyleHashCode);
					TextResourceManager.s_FontAssetRemovalList.Add(keyValuePair.Key);
				}
				else
				{
					fontAsset.InitializeCharacterLookupDictionary();
					fontAsset.AddSynthesizedCharactersAndFaceMetrics();
				}
			}
			for (int i = 0; i < TextResourceManager.s_FontAssetRemovalList.Count; i++)
			{
				TextResourceManager.s_FontAssetReferences.Remove(TextResourceManager.s_FontAssetRemovalList[i]);
			}
			TextResourceManager.s_FontAssetRemovalList.Clear();
			TextEventManager.ON_FONT_PROPERTY_CHANGED(true, null);
		}

		private static readonly Dictionary<int, TextResourceManager.FontAssetRef> s_FontAssetReferences = new Dictionary<int, TextResourceManager.FontAssetRef>();

		private static readonly Dictionary<int, FontAsset> s_FontAssetNameReferenceLookup = new Dictionary<int, FontAsset>();

		private static readonly Dictionary<long, FontAsset> s_FontAssetFamilyNameAndStyleReferenceLookup = new Dictionary<long, FontAsset>();

		private static readonly List<int> s_FontAssetRemovalList = new List<int>(16);

		private static readonly int k_RegularStyleHashCode = TextUtilities.GetHashCodeCaseInSensitive("Regular");

		private struct FontAssetRef
		{
			public FontAssetRef(int nameHashCode, int familyNameHashCode, int styleNameHashCode, FontAsset fontAsset)
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

			public readonly FontAsset fontAsset;
		}
	}
}
