using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Serialization;

namespace UnityEngine.TextCore.Text
{
	[ExcludeFromObjectFactory]
	[ExcludeFromPreset]
	[NativeHeader("Modules/TextCoreTextEngine/Native/TextSettings.h")]
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
				this.m_IsNativeTextSettingsDirty = true;
			}
		}

		internal List<FontAsset> fallbackOSFontAssets
		{
			[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
			get
			{
				bool flag = this.GetStaticFallbackOSFontAsset() == null;
				if (flag)
				{
					this.SetStaticFallbackOSFontAsset(this.GetOSFontAssetList());
				}
				return this.GetStaticFallbackOSFontAsset();
			}
		}

		internal virtual List<FontAsset> GetStaticFallbackOSFontAsset()
		{
			return TextSettings.s_FallbackOSFontAssetInternal;
		}

		internal virtual void SetStaticFallbackOSFontAsset(List<FontAsset> fontAssets)
		{
			TextSettings.s_FallbackOSFontAssetInternal = fontAssets;
		}

		internal virtual List<FontAsset> GetFallbackFontAssets(bool isRaster, int textPixelSize = -1)
		{
			return this.fallbackFontAssets;
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

		public bool enableEmojiSupport
		{
			get
			{
				return this.m_EnableEmojiSupport;
			}
			set
			{
				this.m_EnableEmojiSupport = value;
			}
		}

		public List<TextAsset> emojiFallbackTextAssets
		{
			get
			{
				return this.m_EmojiFallbackTextAssets;
			}
			set
			{
				this.m_EmojiFallbackTextAssets = value;
				this.m_IsNativeTextSettingsDirty = true;
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

		[Obsolete("The Fallback Sprite Assets list is now obsolete. Use the emojiFallbackTextAssets instead.", true)]
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

		internal static SpriteAsset s_GlobalSpriteAsset { get; private set; }

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

		[Obsolete("styleSheetsResourcePath is no longer used and will be removed in a future version.", false)]
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
			this.SetStaticFallbackOSFontAsset(null);
			bool flag = TextSettings.s_GlobalSpriteAsset == null;
			if (flag)
			{
				TextSettings.s_GlobalSpriteAsset = Resources.Load<SpriteAsset>("Sprite Assets/Default Sprite Asset");
			}
		}

		private void OnDestroy()
		{
			bool flag = this.m_NativeTextSettings != IntPtr.Zero;
			if (flag)
			{
				TextSettings.DestroyNativeObject(this.m_NativeTextSettings, Object.MarshalledUnityObject.MarshalNotNull<TextSettings>(this));
			}
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
					Debug.LogWarning("Deleting invalid font reference.");
					this.m_FontReferences.RemoveAt(i);
					i--;
				}
				else
				{
					int hashCode = fontReferenceMap.font.GetHashCode();
					bool flag3 = !this.m_FontLookup.ContainsKey(hashCode);
					if (flag3)
					{
						this.m_FontLookup.Add(hashCode, fontReferenceMap.fontAsset);
					}
				}
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
		internal FontAsset GetCachedFontAsset(Font font)
		{
			bool flag = font == null;
			FontAsset fontAsset;
			if (flag)
			{
				fontAsset = null;
			}
			else
			{
				bool flag2 = this.m_FontLookup == null;
				if (flag2)
				{
					this.m_FontLookup = new Dictionary<int, FontAsset>();
					this.InitializeFontReferenceLookup();
				}
				int hashCode = font.GetHashCode();
				bool flag3 = this.m_FontLookup.ContainsKey(hashCode);
				if (flag3)
				{
					fontAsset = this.m_FontLookup[hashCode];
				}
				else
				{
					bool isExecutingJob = TextGenerator.IsExecutingJob;
					if (isExecutingJob)
					{
						fontAsset = null;
					}
					else
					{
						FontAsset fontAsset2 = FontAssetFactory.ConvertFontToFontAsset(font);
						bool flag4 = fontAsset2 != null;
						if (flag4)
						{
							this.m_FontReferences.Add(new TextSettings.FontReferenceMap(font, fontAsset2));
							this.m_FontLookup.Add(hashCode, fontAsset2);
						}
						fontAsset = fontAsset2;
					}
				}
			}
			return fontAsset;
		}

		private List<FontAsset> GetOSFontAssetList()
		{
			string[] osfallbacks = Font.GetOSFallbacks();
			return FontAsset.CreateFontAssetOSFallbackList(osfallbacks, 90);
		}

		[NativeMethod(Name = "TextSettings::Create")]
		private unsafe static IntPtr CreateNativeObject(IntPtr[] fallbacks, IntPtr managedObject)
		{
			Span<IntPtr> span = new Span<IntPtr>(fallbacks);
			IntPtr intPtr;
			fixed (IntPtr* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				intPtr = TextSettings.CreateNativeObject_Injected(ref managedSpanWrapper, managedObject);
			}
			return intPtr;
		}

		[NativeMethod(Name = "TextSettings::Destroy")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DestroyNativeObject(IntPtr m_NativeTextSettings, IntPtr managedObject);

		private unsafe static void UpdateFallbacks(IntPtr ptr, IntPtr[] fallbacks)
		{
			Span<IntPtr> span = new Span<IntPtr>(fallbacks);
			fixed (IntPtr* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				TextSettings.UpdateFallbacks_Injected(ptr, ref managedSpanWrapper);
			}
		}

		internal IntPtr nativeTextSettings
		{
			[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
			get
			{
				this.UpdateNativeTextSettings();
				return this.m_NativeTextSettings;
			}
		}

		private IntPtr[] GetGlobalFallbacks()
		{
			List<IntPtr> globalFontAssetFallbacks = new List<IntPtr>();
			List<FontAsset> fallbackFontAssets = this.fallbackFontAssets;
			if (fallbackFontAssets != null)
			{
				fallbackFontAssets.ForEach(delegate(FontAsset fallback)
				{
					bool flag = fallback == null;
					if (!flag)
					{
						bool flag2 = fallback.atlasPopulationMode == AtlasPopulationMode.Static && fallback.characterTable.Count > 0;
						if (flag2)
						{
							Debug.LogWarning("Advanced text system cannot use static font asset " + fallback.name + " as fallback.");
						}
						else
						{
							globalFontAssetFallbacks.Add(fallback.nativeFontAsset);
						}
					}
				});
			}
			List<TextAsset> emojiFallbackTextAssets = this.emojiFallbackTextAssets;
			if (emojiFallbackTextAssets != null)
			{
				emojiFallbackTextAssets.ForEach(delegate(TextAsset fallback)
				{
					FontAsset fontAsset = fallback as FontAsset;
					bool flag3 = fontAsset != null;
					if (flag3)
					{
						bool flag4 = fontAsset.atlasPopulationMode == AtlasPopulationMode.Static && fontAsset.characterTable.Count > 0;
						if (flag4)
						{
							Debug.LogWarning("Advanced text system cannot use static font asset " + fallback.name + " as fallback.");
						}
						else
						{
							globalFontAssetFallbacks.Add(fontAsset.nativeFontAsset);
						}
					}
				});
			}
			List<FontAsset> fallbackOSFontAssets = this.fallbackOSFontAssets;
			if (fallbackOSFontAssets != null)
			{
				fallbackOSFontAssets.ForEach(delegate(FontAsset fallback)
				{
					bool flag5 = fallback == null;
					if (!flag5)
					{
						bool flag6 = fallback.atlasPopulationMode == AtlasPopulationMode.Static && fallback.characterTable.Count > 0;
						if (flag6)
						{
							Debug.LogWarning("Advanced text system cannot use static font asset " + fallback.name + " as fallback.");
						}
						else
						{
							globalFontAssetFallbacks.Add(fallback.nativeFontAsset);
						}
					}
				});
			}
			List<TextAsset> emojiFallbackTextAssets2 = this.emojiFallbackTextAssets;
			if (emojiFallbackTextAssets2 != null)
			{
				emojiFallbackTextAssets2.ForEach(delegate(TextAsset fallback)
				{
					FontAsset fontAsset2 = fallback as FontAsset;
					bool flag7 = fontAsset2 != null;
					if (flag7)
					{
						bool flag8 = fontAsset2 == null;
						if (!flag8)
						{
							bool flag9 = fontAsset2.atlasPopulationMode == AtlasPopulationMode.Static && fontAsset2.characterTable.Count > 0;
							if (flag9)
							{
								Debug.LogWarning("Advanced text system cannot use static font asset " + fallback.name + " as fallback.");
							}
							else
							{
								globalFontAssetFallbacks.Add(fontAsset2.nativeFontAsset);
							}
						}
					}
				});
			}
			return globalFontAssetFallbacks.ToArray();
		}

		internal void SetNativeTextSettingsDirty()
		{
			this.m_IsNativeTextSettingsDirty = true;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal void UpdateNativeTextSettings()
		{
			bool flag = this.m_NativeTextSettings == IntPtr.Zero;
			if (flag)
			{
				this.m_NativeTextSettings = TextSettings.CreateNativeObject(this.GetGlobalFallbacks(), Object.MarshalledUnityObject.MarshalNotNull<TextSettings>(this));
				this.m_IsNativeTextSettingsDirty = false;
			}
			else
			{
				bool flag2 = this.m_IsNativeTextSettingsDirty && this.m_NativeTextSettings != IntPtr.Zero;
				if (flag2)
				{
					TextSettings.UpdateFallbacks(this.m_NativeTextSettings, this.GetGlobalFallbacks());
					this.m_IsNativeTextSettingsDirty = false;
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr CreateNativeObject_Injected(ref ManagedSpanWrapper fallbacks, IntPtr managedObject);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UpdateFallbacks_Injected(IntPtr ptr, ref ManagedSpanWrapper fallbacks);

		[SerializeField]
		protected string m_Version;

		[FormerlySerializedAs("m_defaultFontAsset")]
		[SerializeField]
		protected FontAsset m_DefaultFontAsset;

		[FormerlySerializedAs("m_defaultFontAssetPath")]
		[SerializeField]
		protected string m_DefaultFontAssetPath = "Fonts & Materials/";

		[SerializeField]
		[FormerlySerializedAs("m_fallbackFontAssets")]
		protected List<FontAsset> m_FallbackFontAssets;

		private static List<FontAsset> s_FallbackOSFontAssetInternal;

		[SerializeField]
		[FormerlySerializedAs("m_matchMaterialPreset")]
		protected bool m_MatchMaterialPreset;

		[SerializeField]
		[FormerlySerializedAs("m_missingGlyphCharacter")]
		protected int m_MissingCharacterUnicode;

		[SerializeField]
		protected bool m_ClearDynamicDataOnBuild = true;

		[SerializeField]
		private bool m_EnableEmojiSupport;

		[SerializeField]
		private List<TextAsset> m_EmojiFallbackTextAssets;

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

		[SerializeField]
		[FormerlySerializedAs("m_defaultStyleSheet")]
		protected TextStyleSheet m_DefaultStyleSheet;

		private string m_StyleSheetsResourcePath = "Text Style Sheets/";

		[SerializeField]
		[FormerlySerializedAs("m_defaultColorGradientPresetsPath")]
		protected string m_DefaultColorGradientPresetsPath = "Text Color Gradients/";

		[SerializeField]
		protected UnicodeLineBreakingRules m_UnicodeLineBreakingRules;

		[FormerlySerializedAs("m_warningsDisabled")]
		[SerializeField]
		protected bool m_DisplayWarnings = false;

		internal Dictionary<int, FontAsset> m_FontLookup;

		internal List<TextSettings.FontReferenceMap> m_FontReferences = new List<TextSettings.FontReferenceMap>();

		private IntPtr m_NativeTextSettings = IntPtr.Zero;

		private bool m_IsNativeTextSettingsDirty = true;

		[Serializable]
		internal struct FontReferenceMap
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
