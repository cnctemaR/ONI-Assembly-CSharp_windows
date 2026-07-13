using System;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine.TextCore.LowLevel;

namespace UnityEngine.TextCore.Text
{
	[ExcludeFromPreset]
	[ExcludeFromObjectFactory]
	[Serializable]
	public class TextSettings : ScriptableObject
	{
		public string version
		{
			get
			{
				return this.m_Version;
			}
			internal set
			{
				this.m_Version = value;
			}
		}

		public FontAsset defaultFontAsset
		{
			get
			{
				return this.m_DefaultFontAsset;
			}
			set
			{
				this.m_DefaultFontAsset = value;
			}
		}

		public string defaultFontAssetPath
		{
			get
			{
				return this.m_DefaultFontAssetPath;
			}
			set
			{
				this.m_DefaultFontAssetPath = value;
			}
		}

		public List<FontAsset> fallbackFontAssets
		{
			get
			{
				return this.m_FallbackFontAssets;
			}
			set
			{
				this.m_FallbackFontAssets = value;
			}
		}

		public bool matchMaterialPreset
		{
			get
			{
				return this.m_MatchMaterialPreset;
			}
			set
			{
				this.m_MatchMaterialPreset = value;
			}
		}

		public int missingCharacterUnicode
		{
			get
			{
				return this.m_MissingCharacterUnicode;
			}
			set
			{
				this.m_MissingCharacterUnicode = value;
			}
		}

		public bool clearDynamicDataOnBuild
		{
			get
			{
				return this.m_ClearDynamicDataOnBuild;
			}
			set
			{
				this.m_ClearDynamicDataOnBuild = value;
			}
		}

		public SpriteAsset defaultSpriteAsset
		{
			get
			{
				return this.m_DefaultSpriteAsset;
			}
			set
			{
				this.m_DefaultSpriteAsset = value;
			}
		}

		public string defaultSpriteAssetPath
		{
			get
			{
				return this.m_DefaultSpriteAssetPath;
			}
			set
			{
				this.m_DefaultSpriteAssetPath = value;
			}
		}

		public List<SpriteAsset> fallbackSpriteAssets
		{
			get
			{
				return this.m_FallbackSpriteAssets;
			}
			set
			{
				this.m_FallbackSpriteAssets = value;
			}
		}

		public uint missingSpriteCharacterUnicode
		{
			get
			{
				return this.m_MissingSpriteCharacterUnicode;
			}
			set
			{
				this.m_MissingSpriteCharacterUnicode = value;
			}
		}

		public TextStyleSheet defaultStyleSheet
		{
			get
			{
				return this.m_DefaultStyleSheet;
			}
			set
			{
				this.m_DefaultStyleSheet = value;
			}
		}

		public string styleSheetsResourcePath
		{
			get
			{
				return this.m_StyleSheetsResourcePath;
			}
			set
			{
				this.m_StyleSheetsResourcePath = value;
			}
		}

		public string defaultColorGradientPresetsPath
		{
			get
			{
				return this.m_DefaultColorGradientPresetsPath;
			}
			set
			{
				this.m_DefaultColorGradientPresetsPath = value;
			}
		}

		public UnicodeLineBreakingRules lineBreakingRules
		{
			get
			{
				bool flag = this.m_UnicodeLineBreakingRules == null;
				if (flag)
				{
					this.m_UnicodeLineBreakingRules = new UnicodeLineBreakingRules();
					this.m_UnicodeLineBreakingRules.LoadLineBreakingRules();
				}
				return this.m_UnicodeLineBreakingRules;
			}
			set
			{
				this.m_UnicodeLineBreakingRules = value;
			}
		}

		public bool useModernHangulLineBreakingRules
		{
			get
			{
				return this.m_UseModernHangulLineBreakingRules;
			}
			set
			{
				this.m_UseModernHangulLineBreakingRules = value;
			}
		}

		public bool displayWarnings
		{
			get
			{
				return this.m_DisplayWarnings;
			}
			set
			{
				this.m_DisplayWarnings = value;
			}
		}

		private void OnEnable()
		{
			this.lineBreakingRules.LoadLineBreakingRules();
		}

		protected void InitializeFontReferenceLookup()
		{
			bool flag = this.m_FontReferences == null;
			if (flag)
			{
				this.m_FontReferences = new List<TextSettings.FontReferenceMap>();
			}
			for (int i = 0; i < this.m_FontReferences.Count; i++)
			{
				TextSettings.FontReferenceMap fontReferenceMap = this.m_FontReferences[i];
				bool flag2 = fontReferenceMap.font == null || fontReferenceMap.fontAsset == null;
				if (flag2)
				{
					Debug.Log("Deleting invalid font reference.");
					this.m_FontReferences.RemoveAt(i);
					i--;
				}
				else
				{
					int instanceID = fontReferenceMap.font.GetInstanceID();
					bool flag3 = !this.m_FontLookup.ContainsKey(instanceID);
					if (flag3)
					{
						this.m_FontLookup.Add(instanceID, fontReferenceMap.fontAsset);
					}
				}
			}
		}

		protected FontAsset GetCachedFontAssetInternal(Font font)
		{
			bool flag = this.m_FontLookup == null;
			if (flag)
			{
				this.m_FontLookup = new Dictionary<int, FontAsset>();
				this.InitializeFontReferenceLookup();
			}
			int instanceID = font.GetInstanceID();
			bool flag2 = this.m_FontLookup.ContainsKey(instanceID);
			FontAsset fontAsset;
			if (flag2)
			{
				fontAsset = this.m_FontLookup[instanceID];
			}
			else
			{
				bool flag3 = font.name == "System Normal";
				FontAsset fontAsset2;
				if (flag3)
				{
					fontAsset2 = FontAsset.CreateFontAsset("Lucida Grande", "Regular", 90);
				}
				else
				{
					fontAsset2 = FontAsset.CreateFontAsset(font, 90, 9, GlyphRenderMode.SDFAA, 1024, 1024, AtlasPopulationMode.Dynamic, true);
				}
				bool flag4 = fontAsset2 != null;
				if (flag4)
				{
					fontAsset2.hideFlags = HideFlags.DontSave;
					fontAsset2.atlasTextures[0].hideFlags = HideFlags.DontSave;
					fontAsset2.material.hideFlags = HideFlags.DontSave;
					fontAsset2.isMultiAtlasTexturesEnabled = true;
					this.m_FontReferences.Add(new TextSettings.FontReferenceMap(font, fontAsset2));
					this.m_FontLookup.Add(instanceID, fontAsset2);
				}
				fontAsset = fontAsset2;
			}
			return fontAsset;
		}

		[SerializeField]
		protected string m_Version;

		[FormerlySerializedAs("m_defaultFontAsset")]
		[SerializeField]
		protected FontAsset m_DefaultFontAsset;

		[FormerlySerializedAs("m_defaultFontAssetPath")]
		[SerializeField]
		protected string m_DefaultFontAssetPath = "Fonts & Materials/";

		[FormerlySerializedAs("m_fallbackFontAssets")]
		[SerializeField]
		protected List<FontAsset> m_FallbackFontAssets;

		[SerializeField]
		[FormerlySerializedAs("m_matchMaterialPreset")]
		protected bool m_MatchMaterialPreset;

		[FormerlySerializedAs("m_missingGlyphCharacter")]
		[SerializeField]
		protected int m_MissingCharacterUnicode;

		[SerializeField]
		protected bool m_ClearDynamicDataOnBuild = true;

		[FormerlySerializedAs("m_defaultSpriteAsset")]
		[SerializeField]
		protected SpriteAsset m_DefaultSpriteAsset;

		[FormerlySerializedAs("m_defaultSpriteAssetPath")]
		[SerializeField]
		protected string m_DefaultSpriteAssetPath = "Sprite Assets/";

		[SerializeField]
		protected List<SpriteAsset> m_FallbackSpriteAssets;

		[SerializeField]
		protected uint m_MissingSpriteCharacterUnicode;

		[FormerlySerializedAs("m_defaultStyleSheet")]
		[SerializeField]
		protected TextStyleSheet m_DefaultStyleSheet;

		[SerializeField]
		protected string m_StyleSheetsResourcePath = "Text Style Sheets/";

		[FormerlySerializedAs("m_defaultColorGradientPresetsPath")]
		[SerializeField]
		protected string m_DefaultColorGradientPresetsPath = "Text Color Gradients/";

		[SerializeField]
		protected UnicodeLineBreakingRules m_UnicodeLineBreakingRules;

		[SerializeField]
		private bool m_UseModernHangulLineBreakingRules;

		[FormerlySerializedAs("m_warningsDisabled")]
		[SerializeField]
		protected bool m_DisplayWarnings = false;

		internal Dictionary<int, FontAsset> m_FontLookup;

		private List<TextSettings.FontReferenceMap> m_FontReferences = new List<TextSettings.FontReferenceMap>();

		[Serializable]
		private struct FontReferenceMap
		{
			public FontReferenceMap(Font font, FontAsset fontAsset)
			{
				this.font = font;
				this.fontAsset = fontAsset;
			}

			public Font font;

			public FontAsset fontAsset;
		}
	}
}
