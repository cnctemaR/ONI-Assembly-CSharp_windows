using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.TextCore;

namespace TMPro
{
	[ExcludeFromPreset]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/TextMeshPro/Settings.html")]
	[Serializable]
	public class TMP_Settings : ScriptableObject
	{
		public static string version
		{
			get
			{
				return "1.4.0";
			}
		}

		internal void SetAssetVersion()
		{
			this.assetVersion = TMP_Settings.s_CurrentAssetVersion;
		}

		public static TextWrappingModes textWrappingMode
		{
			get
			{
				return TMP_Settings.instance.m_TextWrappingMode;
			}
		}

		[Obsolete("The \"enableKerning\" property has been deprecated. Use the \"fontFeatures\" property to control what features are enabled by default on newly created text components.")]
		public static bool enableKerning
		{
			get
			{
				if (TMP_Settings.instance.m_ActiveFontFeatures != null)
				{
					return TMP_Settings.instance.m_ActiveFontFeatures.Contains(OTL_FeatureTag.kern);
				}
				return TMP_Settings.instance.m_enableKerning;
			}
		}

		public static List<OTL_FeatureTag> fontFeatures
		{
			get
			{
				return TMP_Settings.instance.m_ActiveFontFeatures;
			}
		}

		public static bool enableExtraPadding
		{
			get
			{
				return TMP_Settings.instance.m_enableExtraPadding;
			}
		}

		public static bool enableTintAllSprites
		{
			get
			{
				return TMP_Settings.instance.m_enableTintAllSprites;
			}
		}

		public static bool enableParseEscapeCharacters
		{
			get
			{
				return TMP_Settings.instance.m_enableParseEscapeCharacters;
			}
		}

		public static bool enableRaycastTarget
		{
			get
			{
				return TMP_Settings.instance.m_EnableRaycastTarget;
			}
		}

		public static bool getFontFeaturesAtRuntime
		{
			get
			{
				return TMP_Settings.instance.m_GetFontFeaturesAtRuntime;
			}
		}

		public static int missingGlyphCharacter
		{
			get
			{
				return TMP_Settings.instance.m_missingGlyphCharacter;
			}
			set
			{
				TMP_Settings.instance.m_missingGlyphCharacter = value;
			}
		}

		public static bool clearDynamicDataOnBuild
		{
			get
			{
				return TMP_Settings.instance.m_ClearDynamicDataOnBuild;
			}
		}

		public static bool warningsDisabled
		{
			get
			{
				return TMP_Settings.instance.m_warningsDisabled;
			}
		}

		public static TMP_FontAsset defaultFontAsset
		{
			get
			{
				return TMP_Settings.instance.m_defaultFontAsset;
			}
			set
			{
				TMP_Settings.instance.m_defaultFontAsset = value;
			}
		}

		public static string defaultFontAssetPath
		{
			get
			{
				return TMP_Settings.instance.m_defaultFontAssetPath;
			}
		}

		public static float defaultFontSize
		{
			get
			{
				return TMP_Settings.instance.m_defaultFontSize;
			}
		}

		public static float defaultTextAutoSizingMinRatio
		{
			get
			{
				return TMP_Settings.instance.m_defaultAutoSizeMinRatio;
			}
		}

		public static float defaultTextAutoSizingMaxRatio
		{
			get
			{
				return TMP_Settings.instance.m_defaultAutoSizeMaxRatio;
			}
		}

		public static Vector2 defaultTextMeshProTextContainerSize
		{
			get
			{
				return TMP_Settings.instance.m_defaultTextMeshProTextContainerSize;
			}
		}

		public static Vector2 defaultTextMeshProUITextContainerSize
		{
			get
			{
				return TMP_Settings.instance.m_defaultTextMeshProUITextContainerSize;
			}
		}

		public static bool autoSizeTextContainer
		{
			get
			{
				return TMP_Settings.instance.m_autoSizeTextContainer;
			}
		}

		public static bool isTextObjectScaleStatic
		{
			get
			{
				return TMP_Settings.instance.m_IsTextObjectScaleStatic;
			}
			set
			{
				TMP_Settings.instance.m_IsTextObjectScaleStatic = value;
			}
		}

		public static List<TMP_FontAsset> fallbackFontAssets
		{
			get
			{
				return TMP_Settings.instance.m_fallbackFontAssets;
			}
			set
			{
				TMP_Settings.instance.m_fallbackFontAssets = value;
			}
		}

		public static bool matchMaterialPreset
		{
			get
			{
				return TMP_Settings.instance.m_matchMaterialPreset;
			}
		}

		public static bool hideSubTextObjects
		{
			get
			{
				return TMP_Settings.instance.m_HideSubTextObjects;
			}
		}

		public static TMP_SpriteAsset defaultSpriteAsset
		{
			get
			{
				return TMP_Settings.instance.m_defaultSpriteAsset;
			}
			set
			{
				TMP_Settings.instance.m_defaultSpriteAsset = value;
			}
		}

		public static string defaultSpriteAssetPath
		{
			get
			{
				return TMP_Settings.instance.m_defaultSpriteAssetPath;
			}
		}

		public static bool enableEmojiSupport
		{
			get
			{
				return TMP_Settings.instance.m_enableEmojiSupport;
			}
			set
			{
				TMP_Settings.instance.m_enableEmojiSupport = value;
			}
		}

		public static uint missingCharacterSpriteUnicode
		{
			get
			{
				return TMP_Settings.instance.m_MissingCharacterSpriteUnicode;
			}
			set
			{
				TMP_Settings.instance.m_MissingCharacterSpriteUnicode = value;
			}
		}

		public static List<TMP_Asset> emojiFallbackTextAssets
		{
			get
			{
				return TMP_Settings.instance.m_EmojiFallbackTextAssets;
			}
			set
			{
				TMP_Settings.instance.m_EmojiFallbackTextAssets = value;
			}
		}

		public static string defaultColorGradientPresetsPath
		{
			get
			{
				return TMP_Settings.instance.m_defaultColorGradientPresetsPath;
			}
		}

		public static TMP_StyleSheet defaultStyleSheet
		{
			get
			{
				return TMP_Settings.instance.m_defaultStyleSheet;
			}
			set
			{
				TMP_Settings.instance.m_defaultStyleSheet = value;
			}
		}

		public static string styleSheetsResourcePath
		{
			get
			{
				return TMP_Settings.instance.m_StyleSheetsResourcePath;
			}
		}

		public static TextAsset leadingCharacters
		{
			get
			{
				return TMP_Settings.instance.m_leadingCharacters;
			}
		}

		public static TextAsset followingCharacters
		{
			get
			{
				return TMP_Settings.instance.m_followingCharacters;
			}
		}

		public static TMP_Settings.LineBreakingTable linebreakingRules
		{
			get
			{
				if (TMP_Settings.instance.m_linebreakingRules == null)
				{
					TMP_Settings.LoadLinebreakingRules();
				}
				return TMP_Settings.instance.m_linebreakingRules;
			}
		}

		public static bool useModernHangulLineBreakingRules
		{
			get
			{
				return TMP_Settings.instance.m_UseModernHangulLineBreakingRules;
			}
			set
			{
				TMP_Settings.instance.m_UseModernHangulLineBreakingRules = value;
			}
		}

		public static TMP_Settings instance
		{
			get
			{
				if (TMP_Settings.isTMPSettingsNull)
				{
					TMP_Settings.s_Instance = Resources.Load<TMP_Settings>("TMP Settings");
					if (!TMP_Settings.isTMPSettingsNull && TMP_Settings.s_Instance.m_ActiveFontFeatures.Count == 1 && TMP_Settings.s_Instance.m_ActiveFontFeatures[0] == (OTL_FeatureTag)0U)
					{
						TMP_Settings.s_Instance.m_ActiveFontFeatures.Clear();
						if (TMP_Settings.s_Instance.m_enableKerning)
						{
							TMP_Settings.s_Instance.m_ActiveFontFeatures.Add(OTL_FeatureTag.kern);
						}
					}
				}
				return TMP_Settings.s_Instance;
			}
		}

		internal static bool isTMPSettingsNull
		{
			get
			{
				return TMP_Settings.s_Instance == null;
			}
		}

		public static TMP_Settings LoadDefaultSettings()
		{
			if (TMP_Settings.s_Instance == null)
			{
				TMP_Settings tmp_Settings = Resources.Load<TMP_Settings>("TMP Settings");
				if (tmp_Settings != null)
				{
					TMP_Settings.s_Instance = tmp_Settings;
				}
			}
			return TMP_Settings.s_Instance;
		}

		public static TMP_Settings GetSettings()
		{
			if (TMP_Settings.instance == null)
			{
				return null;
			}
			return TMP_Settings.instance;
		}

		public static TMP_FontAsset GetFontAsset()
		{
			if (TMP_Settings.instance == null)
			{
				return null;
			}
			return TMP_Settings.instance.m_defaultFontAsset;
		}

		public static TMP_SpriteAsset GetSpriteAsset()
		{
			if (TMP_Settings.instance == null)
			{
				return null;
			}
			return TMP_Settings.instance.m_defaultSpriteAsset;
		}

		public static TMP_StyleSheet GetStyleSheet()
		{
			if (TMP_Settings.instance == null)
			{
				return null;
			}
			return TMP_Settings.instance.m_defaultStyleSheet;
		}

		public static void LoadLinebreakingRules()
		{
			if (TMP_Settings.instance == null)
			{
				return;
			}
			if (TMP_Settings.s_Instance.m_linebreakingRules == null)
			{
				TMP_Settings.s_Instance.m_linebreakingRules = new TMP_Settings.LineBreakingTable();
			}
			TMP_Settings.s_Instance.m_linebreakingRules.leadingCharacters = TMP_Settings.GetCharacters(TMP_Settings.s_Instance.m_leadingCharacters);
			TMP_Settings.s_Instance.m_linebreakingRules.followingCharacters = TMP_Settings.GetCharacters(TMP_Settings.s_Instance.m_followingCharacters);
		}

		private static HashSet<uint> GetCharacters(TextAsset file)
		{
			HashSet<uint> hashSet = new HashSet<uint>();
			string text = file.text;
			for (int i = 0; i < text.Length; i++)
			{
				hashSet.Add((uint)text[i]);
			}
			return hashSet;
		}

		private static TMP_Settings s_Instance;

		[SerializeField]
		internal string assetVersion;

		internal static string s_CurrentAssetVersion = "2";

		[FormerlySerializedAs("m_enableWordWrapping")]
		[SerializeField]
		private TextWrappingModes m_TextWrappingMode;

		[SerializeField]
		private bool m_enableKerning;

		[SerializeField]
		private List<OTL_FeatureTag> m_ActiveFontFeatures = new List<OTL_FeatureTag> { (OTL_FeatureTag)0U };

		[SerializeField]
		private bool m_enableExtraPadding;

		[SerializeField]
		private bool m_enableTintAllSprites;

		[SerializeField]
		private bool m_enableParseEscapeCharacters;

		[SerializeField]
		private bool m_EnableRaycastTarget = true;

		[SerializeField]
		private bool m_GetFontFeaturesAtRuntime = true;

		[SerializeField]
		private int m_missingGlyphCharacter;

		[SerializeField]
		private bool m_ClearDynamicDataOnBuild = true;

		[SerializeField]
		private bool m_warningsDisabled;

		[SerializeField]
		private TMP_FontAsset m_defaultFontAsset;

		[SerializeField]
		private string m_defaultFontAssetPath;

		[SerializeField]
		private float m_defaultFontSize;

		[SerializeField]
		private float m_defaultAutoSizeMinRatio;

		[SerializeField]
		private float m_defaultAutoSizeMaxRatio;

		[SerializeField]
		private Vector2 m_defaultTextMeshProTextContainerSize;

		[SerializeField]
		private Vector2 m_defaultTextMeshProUITextContainerSize;

		[SerializeField]
		private bool m_autoSizeTextContainer;

		[SerializeField]
		private bool m_IsTextObjectScaleStatic;

		[SerializeField]
		private List<TMP_FontAsset> m_fallbackFontAssets;

		[SerializeField]
		private bool m_matchMaterialPreset;

		[SerializeField]
		private bool m_HideSubTextObjects = true;

		[SerializeField]
		private TMP_SpriteAsset m_defaultSpriteAsset;

		[SerializeField]
		private string m_defaultSpriteAssetPath;

		[SerializeField]
		private bool m_enableEmojiSupport;

		[SerializeField]
		private uint m_MissingCharacterSpriteUnicode;

		[SerializeField]
		private List<TMP_Asset> m_EmojiFallbackTextAssets;

		[SerializeField]
		private string m_defaultColorGradientPresetsPath;

		[SerializeField]
		private TMP_StyleSheet m_defaultStyleSheet;

		[SerializeField]
		private string m_StyleSheetsResourcePath;

		[SerializeField]
		private TextAsset m_leadingCharacters;

		[SerializeField]
		private TextAsset m_followingCharacters;

		[SerializeField]
		private TMP_Settings.LineBreakingTable m_linebreakingRules;

		[SerializeField]
		private bool m_UseModernHangulLineBreakingRules;

		public class LineBreakingTable
		{
			public HashSet<uint> leadingCharacters;

			public HashSet<uint> followingCharacters;
		}
	}
}
