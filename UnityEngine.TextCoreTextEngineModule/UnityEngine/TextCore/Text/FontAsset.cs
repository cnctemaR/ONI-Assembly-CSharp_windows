using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Profiling;
using UnityEngine.Bindings;
using UnityEngine.Serialization;
using UnityEngine.TextCore.LowLevel;

namespace UnityEngine.TextCore.Text
{
	[ExcludeFromPreset]
	[NativeHeader("Modules/TextCoreTextEngine/Native/FontAsset.h")]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class FontAsset : TextAsset
	{
		private static void EnsureAdditionalCapacity<T>(List<T> container, int additionalCapacity)
		{
			int num = container.Count + additionalCapacity;
			bool flag = container.Capacity < num;
			if (flag)
			{
				container.Capacity = num;
			}
		}

		private static void EnsureAdditionalCapacity<TKey, TValue>(Dictionary<TKey, TValue> container, int additionalCapacity)
		{
			int num = container.Count + additionalCapacity;
			container.EnsureCapacity(num);
		}

		public FontAssetCreationEditorSettings fontAssetCreationEditorSettings
		{
			get
			{
				return this.m_fontAssetCreationEditorSettings;
			}
			set
			{
				this.m_fontAssetCreationEditorSettings = value;
			}
		}

		public Font sourceFontFile
		{
			get
			{
				return this.m_SourceFontFile;
			}
			internal set
			{
				this.m_SourceFontFile = value;
			}
		}

		public AtlasPopulationMode atlasPopulationMode
		{
			get
			{
				return this.m_AtlasPopulationMode;
			}
			set
			{
				this.m_AtlasPopulationMode = value;
			}
		}

		public FaceInfo faceInfo
		{
			get
			{
				return this.m_FaceInfo;
			}
			set
			{
				this.m_FaceInfo = value;
				bool flag = this.m_NativeFontAsset != IntPtr.Zero;
				if (flag)
				{
					this.UpdateFaceInfo();
				}
			}
		}

		internal int familyNameHashCode
		{
			get
			{
				bool flag = this.m_FamilyNameHashCode == 0;
				if (flag)
				{
					this.m_FamilyNameHashCode = TextUtilities.GetHashCodeCaseInSensitive(this.m_FaceInfo.familyName);
				}
				return this.m_FamilyNameHashCode;
			}
			set
			{
				this.m_FamilyNameHashCode = value;
			}
		}

		internal int styleNameHashCode
		{
			get
			{
				bool flag = this.m_StyleNameHashCode == 0;
				if (flag)
				{
					this.m_StyleNameHashCode = TextUtilities.GetHashCodeCaseInSensitive(this.m_FaceInfo.styleName);
				}
				return this.m_StyleNameHashCode;
			}
			set
			{
				this.m_StyleNameHashCode = value;
			}
		}

		[global::System.Runtime.CompilerServices.Nullable(1)]
		public List<Glyph> glyphTable
		{
			[global::System.Runtime.CompilerServices.NullableContext(1)]
			get
			{
				return this.m_GlyphTable;
			}
			[global::System.Runtime.CompilerServices.NullableContext(1)]
			internal set
			{
				this.m_GlyphTable = value;
			}
		}

		public Dictionary<uint, Glyph> glyphLookupTable
		{
			get
			{
				bool flag = this.m_GlyphLookupDictionary == null;
				if (flag)
				{
					this.ReadFontAssetDefinition();
				}
				return this.m_GlyphLookupDictionary;
			}
		}

		public List<Character> characterTable
		{
			get
			{
				return this.m_CharacterTable;
			}
			internal set
			{
				this.m_CharacterTable = value;
			}
		}

		public Dictionary<uint, Character> characterLookupTable
		{
			get
			{
				bool flag = this.m_CharacterLookupDictionary == null;
				if (flag)
				{
					this.ReadFontAssetDefinition();
				}
				return this.m_CharacterLookupDictionary;
			}
		}

		public Texture2D atlasTexture
		{
			get
			{
				bool flag = this.m_AtlasTexture == null;
				if (flag)
				{
					this.m_AtlasTexture = this.atlasTextures[0];
				}
				return this.m_AtlasTexture;
			}
		}

		public Texture2D[] atlasTextures
		{
			get
			{
				return this.m_AtlasTextures;
			}
			set
			{
				this.m_AtlasTextures = value;
			}
		}

		public int atlasTextureCount
		{
			get
			{
				return this.m_AtlasTextureIndex + 1;
			}
		}

		public bool isMultiAtlasTexturesEnabled
		{
			get
			{
				return this.m_IsMultiAtlasTexturesEnabled;
			}
			set
			{
				this.m_IsMultiAtlasTexturesEnabled = value;
			}
		}

		public bool getFontFeatures
		{
			get
			{
				return this.m_GetFontFeatures;
			}
			set
			{
				this.m_GetFontFeatures = value;
			}
		}

		internal bool clearDynamicDataOnBuild
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

		public int atlasWidth
		{
			get
			{
				return this.m_AtlasWidth;
			}
			internal set
			{
				this.m_AtlasWidth = value;
			}
		}

		public int atlasHeight
		{
			get
			{
				return this.m_AtlasHeight;
			}
			internal set
			{
				this.m_AtlasHeight = value;
			}
		}

		public int atlasPadding
		{
			get
			{
				return this.m_AtlasPadding;
			}
			internal set
			{
				this.m_AtlasPadding = value;
			}
		}

		public GlyphRenderMode atlasRenderMode
		{
			get
			{
				return this.m_AtlasRenderMode;
			}
			internal set
			{
				this.m_AtlasRenderMode = value;
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
		internal bool IsBitmap()
		{
			return ((GlyphRasterModes)this.m_AtlasRenderMode).HasFlag(GlyphRasterModes.RASTER_MODE_BITMAP) && !((GlyphRasterModes)this.m_AtlasRenderMode).HasFlag(GlyphRasterModes.RASTER_MODE_COLOR);
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
		internal bool IsRaster()
		{
			return this.m_AtlasRenderMode == GlyphRenderMode.RASTER_HINTED;
		}

		internal bool IsColor()
		{
			return ((GlyphRasterModes)this.m_AtlasRenderMode).HasFlag(GlyphRasterModes.RASTER_MODE_COLOR);
		}

		internal List<GlyphRect> usedGlyphRects
		{
			get
			{
				return this.m_UsedGlyphRects;
			}
			set
			{
				this.m_UsedGlyphRects = value;
			}
		}

		internal List<GlyphRect> freeGlyphRects
		{
			get
			{
				return this.m_FreeGlyphRects;
			}
			set
			{
				this.m_FreeGlyphRects = value;
			}
		}

		public FontFeatureTable fontFeatureTable
		{
			get
			{
				return this.m_FontFeatureTable;
			}
			internal set
			{
				this.m_FontFeatureTable = value;
			}
		}

		public List<FontAsset> fallbackFontAssetTable
		{
			get
			{
				return this.m_FallbackFontAssetTable;
			}
			set
			{
				this.m_FallbackFontAssetTable = value;
			}
		}

		public FontWeightPair[] fontWeightTable
		{
			get
			{
				return this.m_FontWeightTable;
			}
			internal set
			{
				this.m_FontWeightTable = value;
			}
		}

		public float regularStyleWeight
		{
			get
			{
				return this.m_RegularStyleWeight;
			}
			set
			{
				this.m_RegularStyleWeight = value;
			}
		}

		public float regularStyleSpacing
		{
			get
			{
				return this.m_RegularStyleSpacing;
			}
			set
			{
				this.m_RegularStyleSpacing = value;
			}
		}

		public float boldStyleWeight
		{
			get
			{
				return this.m_BoldStyleWeight;
			}
			set
			{
				this.m_BoldStyleWeight = value;
			}
		}

		public float boldStyleSpacing
		{
			get
			{
				return this.m_BoldStyleSpacing;
			}
			set
			{
				this.m_BoldStyleSpacing = value;
			}
		}

		public byte italicStyleSlant
		{
			get
			{
				return this.m_ItalicStyleSlant;
			}
			set
			{
				this.m_ItalicStyleSlant = value;
			}
		}

		public byte tabMultiple
		{
			get
			{
				return this.m_TabMultiple;
			}
			set
			{
				this.m_TabMultiple = value;
			}
		}

		public static FontAsset CreateFontAsset(string familyName, string styleName, int pointSize = 90)
		{
			FontAsset fontAsset = FontAsset.CreateFontAssetInternal(familyName, styleName, pointSize);
			bool flag = fontAsset == null;
			FontAsset fontAsset2;
			if (flag)
			{
				Debug.Log(string.Concat(new string[] { "Unable to find a font file with the specified Family Name [", familyName, "] and Style [", styleName, "]." }));
				fontAsset2 = null;
			}
			else
			{
				fontAsset2 = fontAsset;
			}
			return fontAsset2;
		}

		[global::System.Runtime.CompilerServices.NullableContext(1)]
		[return: global::System.Runtime.CompilerServices.Nullable(2)]
		internal static FontAsset CreateFontAssetInternal(string familyName, string styleName, int pointSize = 90)
		{
			FontReference fontReference;
			bool flag = FontEngine.TryGetSystemFontReference(familyName, styleName, out fontReference);
			FontAsset fontAsset;
			if (flag)
			{
				fontAsset = FontAsset.CreateFontAsset(fontReference.filePath, fontReference.faceIndex, pointSize, 9, GlyphRenderMode.DEFAULT, 1024, 1024, AtlasPopulationMode.DynamicOS, true);
			}
			else
			{
				fontAsset = null;
			}
			return fontAsset;
		}

		[global::System.Runtime.CompilerServices.NullableContext(1)]
		[return: global::System.Runtime.CompilerServices.Nullable(2)]
		public static FontAsset CreateFontAsset(string familyName, string styleName, int pointSize, int padding, GlyphRenderMode renderMode)
		{
			FontReference fontReference;
			bool flag = FontEngine.TryGetSystemFontReference(familyName, styleName, out fontReference);
			FontAsset fontAsset;
			if (flag)
			{
				fontAsset = FontAsset.CreateFontAsset(fontReference.filePath, fontReference.faceIndex, pointSize, padding, renderMode, 1024, 1024, AtlasPopulationMode.DynamicOS, true);
			}
			else
			{
				fontAsset = null;
			}
			return fontAsset;
		}

		internal static List<FontAsset> CreateFontAssetOSFallbackList(string[] fallbacksFamilyNames, int pointSize = 90)
		{
			List<FontAsset> list = new List<FontAsset>();
			foreach (string text in fallbacksFamilyNames)
			{
				FontAsset fontAsset = FontAsset.CreateFontAssetFromFamilyName(text, pointSize);
				bool flag = fontAsset == null;
				if (!flag)
				{
					list.Add(fontAsset);
				}
			}
			return list;
		}

		internal static FontAsset CreateFontAssetWithOSFallbackList(string[] fallbacksFamilyNames, int pointSize = 90)
		{
			FontAsset fontAsset = null;
			foreach (string text in fallbacksFamilyNames)
			{
				FontAsset fontAsset2 = FontAsset.CreateFontAssetFromFamilyName(text, pointSize);
				bool flag = fontAsset2 == null;
				if (!flag)
				{
					bool flag2 = fontAsset == null;
					if (flag2)
					{
						fontAsset = fontAsset2;
					}
					bool flag3 = fontAsset.fallbackFontAssetTable == null;
					if (flag3)
					{
						fontAsset.fallbackFontAssetTable = new List<FontAsset>();
					}
					fontAsset.fallbackFontAssetTable.Add(fontAsset2);
				}
			}
			return fontAsset;
		}

		private static FontAsset CreateFontAssetFromFamilyName(string familyName, int pointSize = 90)
		{
			FontAsset fontAsset = null;
			FontReference fontReference;
			bool flag = FontEngine.TryGetSystemFontReference(familyName, null, out fontReference);
			if (flag)
			{
				fontAsset = FontAsset.CreateFontAsset(fontReference.filePath, fontReference.faceIndex, pointSize, 9, GlyphRenderMode.DEFAULT, 1024, 1024, AtlasPopulationMode.DynamicOS, true);
			}
			bool flag2 = fontAsset == null;
			FontAsset fontAsset2;
			if (flag2)
			{
				fontAsset2 = null;
			}
			else
			{
				FontAssetFactory.SetHideFlags(fontAsset);
				fontAsset.isMultiAtlasTexturesEnabled = true;
				fontAsset.InternalDynamicOS = true;
				fontAsset2 = fontAsset;
			}
			return fontAsset2;
		}

		public static FontAsset CreateFontAsset(string fontFilePath, int faceIndex, int samplingPointSize, int atlasPadding, GlyphRenderMode renderMode, int atlasWidth, int atlasHeight)
		{
			return FontAsset.CreateFontAsset(fontFilePath, faceIndex, samplingPointSize, atlasPadding, renderMode, atlasWidth, atlasHeight, AtlasPopulationMode.Dynamic, true);
		}

		private static FontAsset CreateFontAsset(string fontFilePath, int faceIndex, int samplingPointSize, int atlasPadding, GlyphRenderMode renderMode, int atlasWidth, int atlasHeight, AtlasPopulationMode atlasPopulationMode, bool enableMultiAtlasSupport = true)
		{
			bool flag = FontEngine.LoadFontFace(fontFilePath, (float)samplingPointSize, faceIndex) > FontEngineError.Success;
			FontAsset fontAsset;
			if (flag)
			{
				Debug.Log("Unable to load font face from [" + fontFilePath + "].");
				fontAsset = null;
			}
			else
			{
				FontAsset fontAsset2 = FontAsset.CreateFontAssetInstance(null, atlasPadding, renderMode, atlasWidth, atlasHeight, atlasPopulationMode, enableMultiAtlasSupport);
				bool flag2 = fontAsset2;
				if (flag2)
				{
					fontAsset2.m_SourceFontFilePath = fontFilePath;
				}
				fontAsset = fontAsset2;
			}
			return fontAsset;
		}

		public static FontAsset CreateFontAsset(Font font)
		{
			return FontAsset.CreateFontAsset(font, 90, 9, GlyphRenderMode.SDFAA, 1024, 1024, AtlasPopulationMode.Dynamic, true);
		}

		public static FontAsset CreateFontAsset(Font font, int samplingPointSize, int atlasPadding, GlyphRenderMode renderMode, int atlasWidth, int atlasHeight, AtlasPopulationMode atlasPopulationMode = AtlasPopulationMode.Dynamic, bool enableMultiAtlasSupport = true)
		{
			return FontAsset.CreateFontAsset(font, 0, samplingPointSize, atlasPadding, renderMode, atlasWidth, atlasHeight, atlasPopulationMode, enableMultiAtlasSupport);
		}

		private static FontAsset CreateFontAsset(Font font, int faceIndex, int samplingPointSize, int atlasPadding, GlyphRenderMode renderMode, int atlasWidth, int atlasHeight, AtlasPopulationMode atlasPopulationMode = AtlasPopulationMode.Dynamic, bool enableMultiAtlasSupport = true)
		{
			bool flag = font.name == "LegacyRuntime";
			if (flag)
			{
				string[] osfallbacks = Font.GetOSFallbacks();
				bool flag2 = FontEngine.LoadFontFace(font, (float)samplingPointSize, faceIndex) == FontEngineError.Success;
				if (flag2)
				{
					FontAsset fontAsset = FontAsset.CreateFontAssetInstance(font, atlasPadding, renderMode, atlasWidth, atlasHeight, atlasPopulationMode, enableMultiAtlasSupport);
					List<FontAsset> list = FontAsset.CreateFontAssetOSFallbackList(osfallbacks, samplingPointSize);
					fontAsset.fallbackFontAssetTable = list;
					return fontAsset;
				}
				FontAsset fontAsset2 = FontAsset.CreateFontAssetWithOSFallbackList(osfallbacks, samplingPointSize);
				bool flag3 = fontAsset2 != null;
				if (flag3)
				{
					return fontAsset2;
				}
			}
			bool flag4 = FontEngine.LoadFontFace(font, (float)samplingPointSize, faceIndex) > FontEngineError.Success;
			FontAsset fontAsset4;
			if (flag4)
			{
				FontAsset fontAsset3 = FontAsset.CreateFontAsset(font.name, "Regular", 90);
				bool flag5 = fontAsset3 != null;
				if (flag5)
				{
					fontAsset4 = fontAsset3;
				}
				else
				{
					Debug.LogWarning("Unable to load font face for [" + font.name + "]. Make sure \"Include Font Data\" is enabled in the Font Import Settings.", font);
					fontAsset4 = null;
				}
			}
			else
			{
				fontAsset4 = FontAsset.CreateFontAssetInstance(font, atlasPadding, renderMode, atlasWidth, atlasHeight, atlasPopulationMode, enableMultiAtlasSupport);
			}
			return fontAsset4;
		}

		private static FontAsset CreateFontAssetInstance(Font font, int atlasPadding, GlyphRenderMode renderMode, int atlasWidth, int atlasHeight, AtlasPopulationMode atlasPopulationMode, bool enableMultiAtlasSupport)
		{
			FontAsset fontAsset = ScriptableObject.CreateInstance<FontAsset>();
			fontAsset.m_Version = "1.1.0";
			fontAsset.faceInfo = FontEngine.GetFaceInfo();
			bool flag = renderMode == GlyphRenderMode.DEFAULT;
			if (flag)
			{
				renderMode = (FontEngine.IsColorFontFace() ? GlyphRenderMode.COLOR : GlyphRenderMode.SDFAA);
			}
			bool flag2 = atlasPopulationMode == AtlasPopulationMode.Dynamic && font != null;
			if (flag2)
			{
				fontAsset.sourceFontFile = font;
			}
			fontAsset.atlasPopulationMode = atlasPopulationMode;
			fontAsset.atlasWidth = atlasWidth;
			fontAsset.atlasHeight = atlasHeight;
			fontAsset.atlasPadding = atlasPadding;
			fontAsset.atlasRenderMode = renderMode;
			fontAsset.atlasTextures = new Texture2D[1];
			TextureFormat textureFormat = (((renderMode & (GlyphRenderMode)65536) == (GlyphRenderMode)65536) ? TextureFormat.RGBA32 : TextureFormat.Alpha8);
			Texture2D texture2D = new Texture2D(1, 1, textureFormat, false);
			fontAsset.atlasTextures[0] = texture2D;
			fontAsset.isMultiAtlasTexturesEnabled = enableMultiAtlasSupport;
			bool flag3 = (renderMode & (GlyphRenderMode)16) == (GlyphRenderMode)16;
			int num;
			if (flag3)
			{
				num = 0;
				bool flag4 = textureFormat == TextureFormat.Alpha8;
				Material material;
				if (flag4)
				{
					bool flag5 = TextShaderUtilities.ShaderRef_MobileBitmap;
					if (!flag5)
					{
						return null;
					}
					material = new Material(TextShaderUtilities.ShaderRef_MobileBitmap);
				}
				else
				{
					bool flag6 = TextShaderUtilities.ShaderRef_Sprite;
					if (!flag6)
					{
						return null;
					}
					material = new Material(TextShaderUtilities.ShaderRef_Sprite);
				}
				material.SetTexture(TextShaderUtilities.ID_MainTex, texture2D);
				material.SetFloat(TextShaderUtilities.ID_TextureWidth, (float)atlasWidth);
				material.SetFloat(TextShaderUtilities.ID_TextureHeight, (float)atlasHeight);
				fontAsset.material = material;
			}
			else
			{
				num = 1;
				Material material2 = new Material(TextShaderUtilities.ShaderRef_MobileSDF);
				material2.SetTexture(TextShaderUtilities.ID_MainTex, texture2D);
				material2.SetFloat(TextShaderUtilities.ID_TextureWidth, (float)atlasWidth);
				material2.SetFloat(TextShaderUtilities.ID_TextureHeight, (float)atlasHeight);
				material2.SetFloat(TextShaderUtilities.ID_GradientScale, (float)(atlasPadding + num));
				material2.SetFloat(TextShaderUtilities.ID_WeightNormal, fontAsset.regularStyleWeight);
				material2.SetFloat(TextShaderUtilities.ID_WeightBold, fontAsset.boldStyleWeight);
				fontAsset.material = material2;
			}
			fontAsset.freeGlyphRects = new List<GlyphRect>(8)
			{
				new GlyphRect(0, 0, atlasWidth - num, atlasHeight - num)
			};
			fontAsset.usedGlyphRects = new List<GlyphRect>(8);
			fontAsset.ReadFontAssetDefinition();
			return fontAsset;
		}

		private void RegisterCallbackInstance(FontAsset instance)
		{
			for (int i = 0; i < FontAsset.s_CallbackInstances.Count; i++)
			{
				FontAsset fontAsset;
				bool flag = FontAsset.s_CallbackInstances[i].TryGetTarget(out fontAsset) && fontAsset == instance;
				if (flag)
				{
					return;
				}
			}
			for (int j = 0; j < FontAsset.s_CallbackInstances.Count; j++)
			{
				FontAsset fontAsset2;
				bool flag2 = !FontAsset.s_CallbackInstances[j].TryGetTarget(out fontAsset2);
				if (flag2)
				{
					FontAsset.s_CallbackInstances[j] = new WeakReference<FontAsset>(instance);
					return;
				}
			}
			FontAsset.s_CallbackInstances.Add(new WeakReference<FontAsset>(this));
		}

		internal override void OnDestroy()
		{
			base.OnDestroy();
			bool flag = !this.m_IsClone;
			if (flag)
			{
				this.DestroyAtlasTextures();
				bool flag2 = this.m_Material;
				if (flag2)
				{
					Object.Destroy(this.m_Material);
				}
				this.m_Material = null;
			}
			bool flag3 = this.m_NativeFontAsset != IntPtr.Zero;
			if (flag3)
			{
				FontAsset.Destroy(this.m_NativeFontAsset, Object.MarshalledUnityObject.MarshalNotNull<FontAsset>(this));
				this.m_NativeFontAsset = IntPtr.Zero;
			}
		}

		public void ReadFontAssetDefinition()
		{
			this.InitializeDictionaryLookupTables();
			this.AddSynthesizedCharactersAndFaceMetrics();
			Character character;
			bool flag = this.m_FaceInfo.capLine == 0f && this.m_CharacterLookupDictionary.TryGetValue(88U, out character);
			if (flag)
			{
				uint glyphIndex = character.glyphIndex;
				this.m_FaceInfo.capLine = this.m_GlyphLookupDictionary[glyphIndex].metrics.horizontalBearingY;
			}
			bool flag2 = this.m_FaceInfo.meanLine == 0f && this.m_CharacterLookupDictionary.TryGetValue(88U, out character);
			if (flag2)
			{
				uint glyphIndex2 = character.glyphIndex;
				this.m_FaceInfo.meanLine = this.m_GlyphLookupDictionary[glyphIndex2].metrics.horizontalBearingY;
			}
			bool flag3 = this.m_FaceInfo.scale == 0f;
			if (flag3)
			{
				this.m_FaceInfo.scale = 1f;
			}
			bool flag4 = this.m_FaceInfo.strikethroughOffset == 0f;
			if (flag4)
			{
				this.m_FaceInfo.strikethroughOffset = this.m_FaceInfo.capLine / 2.5f;
			}
			bool flag5 = this.m_AtlasPadding == 0;
			if (flag5)
			{
				bool flag6 = base.material.HasProperty(TextShaderUtilities.ID_GradientScale);
				if (flag6)
				{
					this.m_AtlasPadding = (int)base.material.GetFloat(TextShaderUtilities.ID_GradientScale) - 1;
				}
			}
			bool flag7 = this.m_FaceInfo.unitsPerEM == 0 && this.atlasPopulationMode > AtlasPopulationMode.Static;
			if (flag7)
			{
				bool flag8 = !JobsUtility.IsExecutingJob;
				if (flag8)
				{
					this.m_FaceInfo.unitsPerEM = FontEngine.GetFaceInfo().unitsPerEM;
					Debug.Log(string.Concat(new string[]
					{
						"Font Asset [",
						base.name,
						"] Units Per EM set to ",
						this.m_FaceInfo.unitsPerEM.ToString(),
						". Please commit the newly serialized value."
					}), this);
				}
				else
				{
					Debug.LogError(string.Concat(new string[] { "Font Asset [", base.name, "] is missing Units Per EM. Please select the 'Reset FaceInfo' menu item on Font Asset [", base.name, "] to ensure proper serialization." }), this);
				}
			}
			base.hashCode = TextUtilities.GetHashCodeCaseInSensitive(base.name);
			this.familyNameHashCode = TextUtilities.GetHashCodeCaseInSensitive(this.m_FaceInfo.familyName);
			this.styleNameHashCode = TextUtilities.GetHashCodeCaseInSensitive(this.m_FaceInfo.styleName);
			base.materialHashCode = TextUtilities.GetHashCodeCaseInSensitive(base.name + FontAsset.s_DefaultMaterialSuffix);
			TextResourceManager.AddFontAsset(this);
			this.IsFontAssetLookupTablesDirty = false;
			this.RegisterCallbackInstance(this);
		}

		internal void InitializeDictionaryLookupTables()
		{
			this.InitializeGlyphLookupDictionary();
			this.InitializeCharacterLookupDictionary();
			bool flag = (this.m_AtlasPopulationMode == AtlasPopulationMode.Dynamic || this.m_AtlasPopulationMode == AtlasPopulationMode.DynamicOS) && this.m_ShouldReimportFontFeatures;
			if (flag)
			{
				this.ImportFontFeatures();
			}
			this.InitializeLigatureSubstitutionLookupDictionary();
			this.InitializeGlyphPairAdjustmentRecordsLookupDictionary();
			this.InitializeMarkToBaseAdjustmentRecordsLookupDictionary();
			this.InitializeMarkToMarkAdjustmentRecordsLookupDictionary();
		}

		private static void InitializeLookup<T>(ICollection source, ref Dictionary<uint, T> lookup, int defaultCapacity = 16)
		{
			int num = ((source != null) ? source.Count : defaultCapacity);
			bool flag = lookup == null;
			if (flag)
			{
				lookup = new Dictionary<uint, T>(num);
			}
			else
			{
				lookup.Clear();
				lookup.EnsureCapacity(num);
			}
		}

		private static void InitializeList<T>(ICollection source, ref List<T> list, int defaultCapacity = 16)
		{
			int num = ((source != null) ? source.Count : defaultCapacity);
			bool flag = list == null;
			if (flag)
			{
				list = new List<T>(num);
			}
			else
			{
				list.Clear();
				list.Capacity = num;
			}
		}

		internal void InitializeGlyphLookupDictionary()
		{
			FontAsset.InitializeLookup<Glyph>(this.m_GlyphTable, ref this.m_GlyphLookupDictionary, 16);
			FontAsset.InitializeList<uint>(this.m_GlyphTable, ref this.m_GlyphIndexList, 16);
			FontAsset.InitializeList<uint>(null, ref this.m_GlyphIndexListNewlyAdded, 16);
			foreach (Glyph glyph in this.m_GlyphTable)
			{
				uint index = glyph.index;
				bool flag = this.m_GlyphLookupDictionary.TryAdd(index, glyph);
				if (flag)
				{
					this.m_GlyphIndexList.Add(index);
				}
			}
		}

		internal void InitializeCharacterLookupDictionary()
		{
			FontAsset.InitializeLookup<Character>(this.m_CharacterTable, ref this.m_CharacterLookupDictionary, 16);
			foreach (Character character in this.m_CharacterTable)
			{
				uint unicode = character.unicode;
				uint glyphIndex = character.glyphIndex;
				bool flag = this.m_CharacterLookupDictionary.TryAdd(unicode, character);
				if (flag)
				{
					character.textAsset = this;
					character.glyph = this.m_GlyphLookupDictionary[glyphIndex];
				}
			}
			HashSet<uint> missingUnicodesFromFontFile = this.m_MissingUnicodesFromFontFile;
			if (missingUnicodesFromFontFile != null)
			{
				missingUnicodesFromFontFile.Clear();
			}
		}

		internal void ClearFallbackCharacterTable()
		{
			List<uint> list = new List<uint>();
			foreach (KeyValuePair<uint, Character> keyValuePair in this.m_CharacterLookupDictionary)
			{
				Character value = keyValuePair.Value;
				bool flag = value.textAsset != this;
				if (flag)
				{
					list.Add(keyValuePair.Key);
				}
			}
			foreach (uint num in list)
			{
				this.m_CharacterLookupDictionary.Remove(num);
			}
		}

		internal void InitializeLigatureSubstitutionLookupDictionary()
		{
			List<LigatureSubstitutionRecord> ligatureSubstitutionRecords = this.m_FontFeatureTable.m_LigatureSubstitutionRecords;
			FontAsset.InitializeLookup<List<LigatureSubstitutionRecord>>(ligatureSubstitutionRecords, ref this.m_FontFeatureTable.m_LigatureSubstitutionRecordLookup, 16);
			bool flag = ligatureSubstitutionRecords == null;
			if (!flag)
			{
				foreach (LigatureSubstitutionRecord ligatureSubstitutionRecord in ligatureSubstitutionRecords)
				{
					bool flag2 = ligatureSubstitutionRecord.componentGlyphIDs == null || ligatureSubstitutionRecord.componentGlyphIDs.Length == 0;
					if (!flag2)
					{
						uint num = ligatureSubstitutionRecord.componentGlyphIDs[0];
						List<LigatureSubstitutionRecord> list;
						bool flag3 = this.m_FontFeatureTable.m_LigatureSubstitutionRecordLookup.TryGetValue(num, out list);
						if (flag3)
						{
							list.Add(ligatureSubstitutionRecord);
						}
						else
						{
							this.m_FontFeatureTable.m_LigatureSubstitutionRecordLookup.Add(num, new List<LigatureSubstitutionRecord> { ligatureSubstitutionRecord });
						}
					}
				}
			}
		}

		internal void InitializeGlyphPairAdjustmentRecordsLookupDictionary()
		{
			List<GlyphPairAdjustmentRecord> glyphPairAdjustmentRecords = this.m_FontFeatureTable.glyphPairAdjustmentRecords;
			FontAsset.InitializeLookup<GlyphPairAdjustmentRecord>(glyphPairAdjustmentRecords, ref this.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup, 16);
			bool flag = glyphPairAdjustmentRecords == null;
			if (!flag)
			{
				foreach (GlyphPairAdjustmentRecord glyphPairAdjustmentRecord in glyphPairAdjustmentRecords)
				{
					uint num = (glyphPairAdjustmentRecord.secondAdjustmentRecord.glyphIndex << 16) | glyphPairAdjustmentRecord.firstAdjustmentRecord.glyphIndex;
					this.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup.TryAdd(num, glyphPairAdjustmentRecord);
				}
			}
		}

		internal void InitializeMarkToBaseAdjustmentRecordsLookupDictionary()
		{
			List<MarkToBaseAdjustmentRecord> markToBaseAdjustmentRecords = this.m_FontFeatureTable.m_MarkToBaseAdjustmentRecords;
			FontAsset.InitializeLookup<MarkToBaseAdjustmentRecord>(markToBaseAdjustmentRecords, ref this.m_FontFeatureTable.m_MarkToBaseAdjustmentRecordLookup, 16);
			bool flag = markToBaseAdjustmentRecords == null;
			if (!flag)
			{
				foreach (MarkToBaseAdjustmentRecord markToBaseAdjustmentRecord in markToBaseAdjustmentRecords)
				{
					uint num = (markToBaseAdjustmentRecord.markGlyphID << 16) | markToBaseAdjustmentRecord.baseGlyphID;
					this.m_FontFeatureTable.m_MarkToBaseAdjustmentRecordLookup.TryAdd(num, markToBaseAdjustmentRecord);
				}
			}
		}

		internal void InitializeMarkToMarkAdjustmentRecordsLookupDictionary()
		{
			List<MarkToMarkAdjustmentRecord> markToMarkAdjustmentRecords = this.m_FontFeatureTable.m_MarkToMarkAdjustmentRecords;
			FontAsset.InitializeLookup<MarkToMarkAdjustmentRecord>(markToMarkAdjustmentRecords, ref this.m_FontFeatureTable.m_MarkToMarkAdjustmentRecordLookup, 16);
			bool flag = markToMarkAdjustmentRecords == null;
			if (!flag)
			{
				foreach (MarkToMarkAdjustmentRecord markToMarkAdjustmentRecord in markToMarkAdjustmentRecords)
				{
					uint num = (markToMarkAdjustmentRecord.combiningMarkGlyphID << 16) | markToMarkAdjustmentRecord.baseMarkGlyphID;
					this.m_FontFeatureTable.m_MarkToMarkAdjustmentRecordLookup.TryAdd(num, markToMarkAdjustmentRecord);
				}
			}
		}

		internal void AddSynthesizedCharactersAndFaceMetrics()
		{
			bool flag = false;
			bool flag2 = this.m_AtlasPopulationMode == AtlasPopulationMode.Dynamic || this.m_AtlasPopulationMode == AtlasPopulationMode.DynamicOS;
			if (flag2)
			{
				flag = this.LoadFontFace() == FontEngineError.Success;
				bool flag3 = !flag && !this.InternalDynamicOS;
				if (flag3)
				{
					Debug.LogWarning("Unable to load font face for [" + base.name + "] font asset.", this);
				}
			}
			this.AddSynthesizedCharacter(3U, flag, true);
			this.AddSynthesizedCharacter(9U, flag, true);
			this.AddSynthesizedCharacter(10U, flag, false);
			this.AddSynthesizedCharacter(11U, flag, false);
			this.AddSynthesizedCharacter(13U, flag, false);
			this.AddSynthesizedCharacter(1564U, flag, false);
			this.AddSynthesizedCharacter(8203U, flag, false);
			this.AddSynthesizedCharacter(8206U, flag, false);
			this.AddSynthesizedCharacter(8207U, flag, false);
			this.AddSynthesizedCharacter(8232U, flag, false);
			this.AddSynthesizedCharacter(8233U, flag, false);
			this.AddSynthesizedCharacter(8288U, flag, false);
		}

		private void AddSynthesizedCharacter(uint unicode, bool isFontFaceLoaded, bool addImmediately = false)
		{
			bool flag = this.m_CharacterLookupDictionary.ContainsKey(unicode);
			if (!flag)
			{
				Glyph glyph;
				Character character;
				if (isFontFaceLoaded)
				{
					bool flag2 = FontEngine.GetGlyphIndex(unicode) > 0U;
					if (flag2)
					{
						bool flag3 = !addImmediately;
						if (flag3)
						{
							return;
						}
						GlyphLoadFlags glyphLoadFlags = (((this.m_AtlasRenderMode & (GlyphRenderMode)4) == (GlyphRenderMode)4) ? (GlyphLoadFlags.LOAD_NO_HINTING | GlyphLoadFlags.LOAD_NO_BITMAP) : GlyphLoadFlags.LOAD_NO_BITMAP);
						bool flag4 = FontEngine.TryGetGlyphWithUnicodeValue(unicode, glyphLoadFlags, out glyph);
						if (flag4)
						{
							character = new Character(unicode, this, glyph);
							foreach (object obj in Enum.GetValues(typeof(TextFontWeight)))
							{
								TextFontWeight textFontWeight = (TextFontWeight)obj;
								this.m_CharacterLookupDictionary.Add(this.CreateCompositeKey(unicode, FontStyles.Normal, textFontWeight), character);
								this.m_CharacterLookupDictionary.Add(this.CreateCompositeKey(unicode, FontStyles.Italic, textFontWeight), character);
							}
						}
						return;
					}
				}
				glyph = new Glyph(0U, new GlyphMetrics(0f, 0f, 0f, 0f, 0f), GlyphRect.zero, 1f, 0);
				character = new Character(unicode, this, glyph);
				foreach (object obj2 in Enum.GetValues(typeof(TextFontWeight)))
				{
					TextFontWeight textFontWeight2 = (TextFontWeight)obj2;
					this.m_CharacterLookupDictionary.Add(this.CreateCompositeKey(unicode, FontStyles.Normal, textFontWeight2), character);
					this.m_CharacterLookupDictionary.Add(this.CreateCompositeKey(unicode, FontStyles.Italic, textFontWeight2), character);
				}
			}
		}

		internal void AddCharacterToLookupCache(uint unicode, Character character)
		{
			this.AddCharacterToLookupCache(unicode, character, FontStyles.Normal, TextFontWeight.Regular);
		}

		internal void AddCharacterToLookupCache(uint unicode, Character character, FontStyles fontStyle, TextFontWeight fontWeight)
		{
			bool flag = this.m_CharacterLookupDictionary == null;
			if (flag)
			{
				this.ReadFontAssetDefinition();
			}
			this.m_CharacterLookupDictionary.TryAdd(this.CreateCompositeKey(unicode, fontStyle, fontWeight), character);
		}

		internal bool GetCharacterInLookupCache(uint unicode, FontStyles fontStyle, TextFontWeight fontWeight, out Character character)
		{
			bool flag = this.m_CharacterLookupDictionary == null;
			if (flag)
			{
				this.ReadFontAssetDefinition();
			}
			return this.m_CharacterLookupDictionary.TryGetValue(this.CreateCompositeKey(unicode, fontStyle, fontWeight), out character);
		}

		internal void RemoveCharacterInLookupCache(uint unicode, FontStyles fontStyle, TextFontWeight fontWeight)
		{
			bool flag = this.m_CharacterLookupDictionary == null;
			if (flag)
			{
				this.ReadFontAssetDefinition();
			}
			this.m_CharacterLookupDictionary.Remove(this.CreateCompositeKey(unicode, fontStyle, fontWeight));
		}

		internal bool ContainsCharacterInLookupCache(uint unicode, FontStyles fontStyle, TextFontWeight fontWeight)
		{
			bool flag = this.m_CharacterLookupDictionary == null;
			if (flag)
			{
				this.ReadFontAssetDefinition();
			}
			return this.m_CharacterLookupDictionary.ContainsKey(this.CreateCompositeKey(unicode, fontStyle, fontWeight));
		}

		private uint CreateCompositeKey(uint unicode, FontStyles fontStyle = FontStyles.Normal, TextFontWeight fontWeight = TextFontWeight.Regular)
		{
			bool flag = fontStyle == FontStyles.Normal && fontWeight == TextFontWeight.Regular;
			uint num;
			if (flag)
			{
				num = unicode;
			}
			else
			{
				bool flag2 = (fontStyle & FontStyles.Italic) == FontStyles.Italic;
				int num2 = 0;
				bool flag3 = fontWeight != TextFontWeight.Regular;
				if (flag3)
				{
					num2 = TextUtilities.GetTextFontWeightIndex(fontWeight);
				}
				uint num3 = unicode & 2097151U;
				uint num4 = (uint)((uint)(num2 & 15) << 21);
				uint num5 = (flag2 ? 33554432U : 0U);
				uint num6 = num3 | num4 | num5;
				num = num6;
			}
			return num;
		}

		internal FontEngineError LoadFontFace()
		{
			bool flag = this.m_AtlasPopulationMode == AtlasPopulationMode.Dynamic;
			FontEngineError fontEngineError;
			if (flag)
			{
				bool flag2 = FontEngine.LoadFontFace(this.m_SourceFontFile, this.m_FaceInfo.pointSize, this.m_FaceInfo.faceIndex) == FontEngineError.Success;
				if (flag2)
				{
					fontEngineError = FontEngineError.Success;
				}
				else
				{
					bool flag3 = !string.IsNullOrEmpty(this.m_SourceFontFilePath);
					if (flag3)
					{
						fontEngineError = FontEngine.LoadFontFace(this.m_SourceFontFilePath, this.m_FaceInfo.pointSize, this.m_FaceInfo.faceIndex);
					}
					else
					{
						fontEngineError = FontEngineError.Invalid_Face;
					}
				}
			}
			else
			{
				fontEngineError = FontEngine.LoadFontFace(this.m_FaceInfo.familyName, this.m_FaceInfo.styleName, this.m_FaceInfo.pointSize);
			}
			return fontEngineError;
		}

		internal void SortCharacterTable()
		{
			bool flag = this.m_CharacterTable != null && this.m_CharacterTable.Count > 0;
			if (flag)
			{
				this.m_CharacterTable = this.m_CharacterTable.OrderBy<Character, uint>((Character c) => c.unicode).ToList<Character>();
			}
		}

		internal void SortGlyphTable()
		{
			bool flag = this.m_GlyphTable != null && this.m_GlyphTable.Count > 0;
			if (flag)
			{
				this.m_GlyphTable = this.m_GlyphTable.OrderBy<Glyph, uint>((Glyph c) => c.index).ToList<Glyph>();
			}
		}

		internal void SortFontFeatureTable()
		{
			this.m_FontFeatureTable.SortGlyphPairAdjustmentRecords();
			this.m_FontFeatureTable.SortMarkToBaseAdjustmentRecords();
			this.m_FontFeatureTable.SortMarkToMarkAdjustmentRecords();
		}

		internal void SortAllTables()
		{
			this.SortGlyphTable();
			this.SortCharacterTable();
			this.SortFontFeatureTable();
		}

		public bool HasCharacter(int character)
		{
			bool flag = this.characterLookupTable == null;
			return !flag && this.m_CharacterLookupDictionary.ContainsKey((uint)character);
		}

		public bool HasCharacter(char character, bool searchFallbacks = false, bool tryAddCharacter = false)
		{
			return this.HasCharacter((uint)character, searchFallbacks, tryAddCharacter);
		}

		public bool HasCharacter(uint character, bool searchFallbacks = false, bool tryAddCharacter = false)
		{
			bool flag = this.characterLookupTable == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = this.m_CharacterLookupDictionary.ContainsKey(character);
				if (flag3)
				{
					flag2 = true;
				}
				else
				{
					bool flag4 = tryAddCharacter && (this.m_AtlasPopulationMode == AtlasPopulationMode.Dynamic || this.m_AtlasPopulationMode == AtlasPopulationMode.DynamicOS);
					if (flag4)
					{
						Character character2;
						bool flag5 = this.TryAddCharacterInternal(character, FontStyles.Normal, TextFontWeight.Regular, out character2, true);
						if (flag5)
						{
							return true;
						}
					}
					if (searchFallbacks)
					{
						bool flag6 = FontAsset.k_SearchedFontAssetLookup == null;
						if (flag6)
						{
							FontAsset.k_SearchedFontAssetLookup = new HashSet<int>();
						}
						else
						{
							FontAsset.k_SearchedFontAssetLookup.Clear();
						}
						FontAsset.k_SearchedFontAssetLookup.Add(base.GetInstanceID());
						bool flag7 = this.fallbackFontAssetTable != null && this.fallbackFontAssetTable.Count > 0;
						if (flag7)
						{
							int num = 0;
							while (num < this.fallbackFontAssetTable.Count && this.fallbackFontAssetTable[num] != null)
							{
								FontAsset fontAsset = this.fallbackFontAssetTable[num];
								int instanceID = fontAsset.GetInstanceID();
								bool flag8 = FontAsset.k_SearchedFontAssetLookup.Add(instanceID);
								if (flag8)
								{
									bool flag9 = fontAsset.HasCharacter_Internal(character, FontStyles.Normal, TextFontWeight.Regular, true, tryAddCharacter);
									if (flag9)
									{
										return true;
									}
								}
								num++;
							}
						}
					}
					flag2 = false;
				}
			}
			return flag2;
		}

		private bool HasCharacterWithStyle_Internal(uint character, FontStyles fontStyle, TextFontWeight fontWeight, bool searchFallbacks = false, bool tryAddCharacter = false)
		{
			return this.HasCharacter_Internal(character, fontStyle, fontWeight, searchFallbacks, tryAddCharacter);
		}

		private bool HasCharacter_Internal(uint character, FontStyles fontStyle = FontStyles.Normal, TextFontWeight fontWeight = TextFontWeight.Regular, bool searchFallbacks = false, bool tryAddCharacter = false)
		{
			bool flag = this.m_CharacterLookupDictionary == null;
			if (flag)
			{
				this.ReadFontAssetDefinition();
				bool flag2 = this.m_CharacterLookupDictionary == null;
				if (flag2)
				{
					return false;
				}
			}
			bool flag3 = this.ContainsCharacterInLookupCache(character, fontStyle, fontWeight);
			bool flag4;
			if (flag3)
			{
				flag4 = true;
			}
			else
			{
				bool flag5 = tryAddCharacter && (this.atlasPopulationMode == AtlasPopulationMode.Dynamic || this.m_AtlasPopulationMode == AtlasPopulationMode.DynamicOS);
				if (flag5)
				{
					Character character2;
					bool flag6 = this.TryAddCharacterInternal(character, fontStyle, fontWeight, out character2, true);
					if (flag6)
					{
						return true;
					}
				}
				if (searchFallbacks)
				{
					bool flag7 = this.fallbackFontAssetTable == null || this.fallbackFontAssetTable.Count == 0;
					if (flag7)
					{
						return false;
					}
					int num = 0;
					while (num < this.fallbackFontAssetTable.Count && this.fallbackFontAssetTable[num] != null)
					{
						FontAsset fontAsset = this.fallbackFontAssetTable[num];
						int instanceID = fontAsset.GetInstanceID();
						bool flag8 = FontAsset.k_SearchedFontAssetLookup.Add(instanceID);
						if (flag8)
						{
							bool flag9 = fontAsset.HasCharacter_Internal(character, fontStyle, fontWeight, true, tryAddCharacter);
							if (flag9)
							{
								return true;
							}
						}
						num++;
					}
				}
				flag4 = false;
			}
			return flag4;
		}

		public bool HasCharacters(string text, out List<char> missingCharacters)
		{
			bool flag = this.characterLookupTable == null;
			bool flag2;
			if (flag)
			{
				missingCharacters = null;
				flag2 = false;
			}
			else
			{
				missingCharacters = new List<char>();
				for (int i = 0; i < text.Length; i++)
				{
					uint codePoint = FontAssetUtilities.GetCodePoint(text, ref i);
					bool flag3 = !this.m_CharacterLookupDictionary.ContainsKey(codePoint);
					if (flag3)
					{
						missingCharacters.Add((char)codePoint);
					}
				}
				bool flag4 = missingCharacters.Count == 0;
				flag2 = flag4;
			}
			return flag2;
		}

		public bool HasCharacters(string text, out uint[] missingCharacters, bool searchFallbacks = false, bool tryAddCharacter = false)
		{
			missingCharacters = null;
			bool flag = this.characterLookupTable == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				this.s_MissingCharacterList.Clear();
				for (int i = 0; i < text.Length; i++)
				{
					bool flag3 = true;
					uint codePoint = FontAssetUtilities.GetCodePoint(text, ref i);
					bool flag4 = this.m_CharacterLookupDictionary.ContainsKey(codePoint);
					if (!flag4)
					{
						bool flag5 = tryAddCharacter && (this.atlasPopulationMode == AtlasPopulationMode.Dynamic || this.m_AtlasPopulationMode == AtlasPopulationMode.DynamicOS);
						if (flag5)
						{
							Character character;
							bool flag6 = this.TryAddCharacterInternal(codePoint, FontStyles.Normal, TextFontWeight.Regular, out character, true);
							if (flag6)
							{
								goto IL_0190;
							}
						}
						if (searchFallbacks)
						{
							bool flag7 = FontAsset.k_SearchedFontAssetLookup == null;
							if (flag7)
							{
								FontAsset.k_SearchedFontAssetLookup = new HashSet<int>();
							}
							else
							{
								FontAsset.k_SearchedFontAssetLookup.Clear();
							}
							FontAsset.k_SearchedFontAssetLookup.Add(base.GetInstanceID());
							bool flag8 = this.fallbackFontAssetTable != null && this.fallbackFontAssetTable.Count > 0;
							if (flag8)
							{
								int num = 0;
								while (num < this.fallbackFontAssetTable.Count && this.fallbackFontAssetTable[num] != null)
								{
									FontAsset fontAsset = this.fallbackFontAssetTable[num];
									int instanceID = fontAsset.GetInstanceID();
									bool flag9 = FontAsset.k_SearchedFontAssetLookup.Add(instanceID);
									if (flag9)
									{
										bool flag10 = !fontAsset.HasCharacter_Internal(codePoint, FontStyles.Normal, TextFontWeight.Regular, true, tryAddCharacter);
										if (!flag10)
										{
											flag3 = false;
											break;
										}
									}
									num++;
								}
							}
						}
						bool flag11 = flag3;
						if (flag11)
						{
							this.s_MissingCharacterList.Add(codePoint);
						}
					}
					IL_0190:;
				}
				bool flag12 = this.s_MissingCharacterList.Count > 0;
				if (flag12)
				{
					missingCharacters = this.s_MissingCharacterList.ToArray();
					flag2 = false;
				}
				else
				{
					flag2 = true;
				}
			}
			return flag2;
		}

		public bool HasCharacters(string text)
		{
			bool flag = this.characterLookupTable == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				for (int i = 0; i < text.Length; i++)
				{
					uint codePoint = FontAssetUtilities.GetCodePoint(text, ref i);
					bool flag3 = !this.m_CharacterLookupDictionary.ContainsKey(codePoint);
					if (flag3)
					{
						return false;
					}
				}
				flag2 = true;
			}
			return flag2;
		}

		public static string GetCharacters(FontAsset fontAsset)
		{
			string text = string.Empty;
			for (int i = 0; i < fontAsset.characterTable.Count; i++)
			{
				text += ((char)fontAsset.characterTable[i].unicode).ToString();
			}
			return text;
		}

		public static int[] GetCharactersArray(FontAsset fontAsset)
		{
			int[] array = new int[fontAsset.characterTable.Count];
			for (int i = 0; i < fontAsset.characterTable.Count; i++)
			{
				array[i] = (int)fontAsset.characterTable[i].unicode;
			}
			return array;
		}

		internal uint GetGlyphIndex(uint unicode)
		{
			bool flag;
			return this.GetGlyphIndex(unicode, out flag);
		}

		internal Glyph GetGlyphInCache(uint glyphID)
		{
			bool flag = this.m_GlyphLookupDictionary == null;
			Glyph glyph;
			if (flag)
			{
				glyph = null;
			}
			else
			{
				Glyph glyph2;
				bool flag2 = !this.glyphLookupTable.TryGetValue(glyphID, out glyph2);
				if (flag2)
				{
					glyph = null;
				}
				else
				{
					glyph = glyph2;
				}
			}
			return glyph;
		}

		internal uint GetGlyphIndex(uint unicode, out bool success)
		{
			success = true;
			Character character;
			bool flag = this.characterLookupTable.TryGetValue(unicode, out character);
			uint num;
			if (flag)
			{
				num = character.glyphIndex;
			}
			else
			{
				bool isExecutingJob = TextGenerator.IsExecutingJob;
				if (isExecutingJob)
				{
					success = false;
					num = 0U;
				}
				else
				{
					num = ((this.LoadFontFace() == FontEngineError.Success) ? FontEngine.GetGlyphIndex(unicode) : 0U);
				}
			}
			return num;
		}

		internal uint GetGlyphVariantIndex(uint unicode, uint variantSelectorUnicode)
		{
			return (this.LoadFontFace() == FontEngineError.Success) ? FontEngine.GetVariantGlyphIndex(unicode, variantSelectorUnicode) : 0U;
		}

		internal void UpdateFontAssetData()
		{
			uint[] array = new uint[this.m_CharacterTable.Count];
			for (int i = 0; i < this.m_CharacterTable.Count; i++)
			{
				array[i] = this.m_CharacterTable[i].unicode;
			}
			this.ClearCharacterAndGlyphTables();
			this.ClearFontFeaturesTables();
			this.ClearAtlasTextures(true);
			this.ReadFontAssetDefinition();
			bool flag = array.Length != 0;
			if (flag)
			{
				this.TryAddCharacters(array, this.m_GetFontFeatures);
			}
		}

		public void ClearFontAssetData(bool setAtlasSizeToZero = false)
		{
			using (FontAsset.k_ClearFontAssetDataMarker.Auto())
			{
				this.ClearCharacterAndGlyphTables();
				this.ClearFontFeaturesTables();
				this.ClearAtlasTextures(setAtlasSizeToZero);
				this.ReadFontAssetDefinition();
				for (int i = 0; i < FontAsset.s_CallbackInstances.Count; i++)
				{
					FontAsset fontAsset;
					bool flag = FontAsset.s_CallbackInstances[i].TryGetTarget(out fontAsset) && fontAsset != this;
					if (flag)
					{
						fontAsset.ClearFallbackCharacterTable();
					}
				}
				TextEventManager.ON_FONT_PROPERTY_CHANGED(true, this);
			}
		}

		internal void ClearCharacterAndGlyphTablesInternal()
		{
			this.ClearCharacterAndGlyphTables();
			this.ClearAtlasTextures(true);
			this.ReadFontAssetDefinition();
		}

		private void ClearCharacterAndGlyphTables()
		{
			bool flag = this.m_GlyphTable != null;
			if (flag)
			{
				this.m_GlyphTable.Clear();
			}
			bool flag2 = this.m_CharacterTable != null;
			if (flag2)
			{
				this.m_CharacterTable.Clear();
			}
			bool flag3 = this.m_UsedGlyphRects != null;
			if (flag3)
			{
				this.m_UsedGlyphRects.Clear();
			}
			bool flag4 = this.m_FreeGlyphRects != null;
			if (flag4)
			{
				int num = (((this.m_AtlasRenderMode & (GlyphRenderMode)16) == (GlyphRenderMode)16) ? 0 : 1);
				this.m_FreeGlyphRects.Clear();
				this.m_FreeGlyphRects.Add(new GlyphRect(0, 0, this.m_AtlasWidth - num, this.m_AtlasHeight - num));
			}
			bool flag5 = this.m_GlyphsToRender != null;
			if (flag5)
			{
				this.m_GlyphsToRender.Clear();
			}
			bool flag6 = this.m_GlyphsRendered != null;
			if (flag6)
			{
				this.m_GlyphsRendered.Clear();
			}
		}

		private void ClearFontFeaturesTables()
		{
			bool flag = this.m_FontFeatureTable != null && this.m_FontFeatureTable.m_LigatureSubstitutionRecords != null;
			if (flag)
			{
				this.m_FontFeatureTable.m_LigatureSubstitutionRecords.Clear();
			}
			bool flag2 = this.m_FontFeatureTable != null && this.m_FontFeatureTable.glyphPairAdjustmentRecords != null;
			if (flag2)
			{
				this.m_FontFeatureTable.glyphPairAdjustmentRecords.Clear();
			}
			bool flag3 = this.m_FontFeatureTable != null && this.m_FontFeatureTable.m_MarkToBaseAdjustmentRecords != null;
			if (flag3)
			{
				this.m_FontFeatureTable.m_MarkToBaseAdjustmentRecords.Clear();
			}
			bool flag4 = this.m_FontFeatureTable != null && this.m_FontFeatureTable.m_MarkToMarkAdjustmentRecords != null;
			if (flag4)
			{
				this.m_FontFeatureTable.m_MarkToMarkAdjustmentRecords.Clear();
			}
		}

		internal void ClearAtlasTextures(bool setAtlasSizeToZero = false)
		{
			this.m_AtlasTextureIndex = 0;
			bool flag = this.m_AtlasTextures == null;
			if (!flag)
			{
				Texture2D texture2D;
				for (int i = 1; i < this.m_AtlasTextures.Length; i++)
				{
					texture2D = this.m_AtlasTextures[i];
					bool flag2 = !texture2D;
					if (!flag2)
					{
						Object.Destroy(texture2D);
					}
				}
				Array.Resize<Texture2D>(ref this.m_AtlasTextures, 1);
				texture2D = (this.m_AtlasTexture = this.m_AtlasTextures[0]);
				bool flag3 = !texture2D.isReadable;
				if (flag3)
				{
				}
				TextureFormat textureFormat = (((this.m_AtlasRenderMode & (GlyphRenderMode)65536) == (GlyphRenderMode)65536) ? TextureFormat.RGBA32 : TextureFormat.Alpha8);
				if (setAtlasSizeToZero)
				{
					texture2D.Reinitialize(1, 1, textureFormat, false);
				}
				else
				{
					bool flag4 = texture2D.width != this.m_AtlasWidth || texture2D.height != this.m_AtlasHeight;
					if (flag4)
					{
						texture2D.Reinitialize(this.m_AtlasWidth, this.m_AtlasHeight, textureFormat, false);
					}
				}
				FontEngine.ResetAtlasTexture(texture2D);
				texture2D.Apply();
			}
		}

		private void DestroyAtlasTextures()
		{
			this.m_AtlasTexture = null;
			this.m_AtlasTextureIndex = -1;
			bool flag = this.m_AtlasTextures == null;
			if (!flag)
			{
				foreach (Texture2D texture2D in this.m_AtlasTextures)
				{
					bool flag2 = texture2D != null;
					if (flag2)
					{
						Object.Destroy(texture2D);
					}
				}
				this.m_AtlasTextures = null;
			}
		}

		internal static void RegisterFontAssetForFontFeatureUpdate(FontAsset fontAsset)
		{
			int instanceID = fontAsset.instanceID;
			bool flag = FontAsset.k_FontAssets_FontFeaturesUpdateQueueLookup.Add(instanceID);
			if (flag)
			{
				FontAsset.k_FontAssets_FontFeaturesUpdateQueue.Add(fontAsset);
			}
		}

		internal static void RegisterFontAssetForKerningUpdate(FontAsset fontAsset)
		{
			int instanceID = fontAsset.instanceID;
			bool flag = FontAsset.k_FontAssets_KerningUpdateQueueLookup.Add(instanceID);
			if (flag)
			{
				FontAsset.k_FontAssets_KerningUpdateQueue.Add(fontAsset);
			}
		}

		internal static void UpdateFontFeaturesForFontAssetsInQueue()
		{
			int num = FontAsset.k_FontAssets_FontFeaturesUpdateQueue.Count;
			for (int i = 0; i < num; i++)
			{
				FontAsset.k_FontAssets_FontFeaturesUpdateQueue[i].UpdateGPOSFontFeaturesForNewlyAddedGlyphs();
			}
			bool flag = num > 0;
			if (flag)
			{
				FontAsset.k_FontAssets_FontFeaturesUpdateQueue.Clear();
				FontAsset.k_FontAssets_FontFeaturesUpdateQueueLookup.Clear();
			}
			num = FontAsset.k_FontAssets_KerningUpdateQueue.Count;
			for (int j = 0; j < num; j++)
			{
				FontAsset.k_FontAssets_KerningUpdateQueue[j].UpdateGlyphAdjustmentRecordsForNewGlyphs();
			}
			bool flag2 = num > 0;
			if (flag2)
			{
				FontAsset.k_FontAssets_KerningUpdateQueue.Clear();
				FontAsset.k_FontAssets_KerningUpdateQueueLookup.Clear();
			}
		}

		internal static void RegisterAtlasTextureForApply(Texture2D texture)
		{
			int instanceID = texture.GetInstanceID();
			bool flag = FontAsset.k_FontAssets_AtlasTexturesUpdateQueueLookup.Add(instanceID);
			if (flag)
			{
				FontAsset.k_FontAssets_AtlasTexturesUpdateQueue.Add(texture);
			}
		}

		internal static void UpdateAtlasTexturesInQueue()
		{
			int count = FontAsset.k_FontAssets_AtlasTexturesUpdateQueueLookup.Count;
			for (int i = 0; i < count; i++)
			{
				FontAsset.k_FontAssets_AtlasTexturesUpdateQueue[i].Apply(false, false);
			}
			bool flag = count > 0;
			if (flag)
			{
				FontAsset.k_FontAssets_AtlasTexturesUpdateQueue.Clear();
				FontAsset.k_FontAssets_AtlasTexturesUpdateQueueLookup.Clear();
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
		internal static void UpdateFontAssetsInUpdateQueue()
		{
			FontAsset.UpdateAtlasTexturesInQueue();
			FontAsset.UpdateFontFeaturesForFontAssetsInQueue();
		}

		public bool TryAddCharacters(uint[] unicodes, bool includeFontFeatures = false)
		{
			uint[] array;
			return this.TryAddCharacters(unicodes, out array, includeFontFeatures);
		}

		public bool TryAddCharacters(uint[] unicodes, out uint[] missingUnicodes, bool includeFontFeatures = false)
		{
			bool flag3;
			using (FontAsset.k_TryAddCharactersMarker.Auto())
			{
				bool flag = unicodes == null || unicodes.Length == 0 || this.m_AtlasPopulationMode == AtlasPopulationMode.Static;
				if (flag)
				{
					bool flag2 = this.m_AtlasPopulationMode == AtlasPopulationMode.Static;
					if (flag2)
					{
						Debug.LogWarning("Unable to add characters to font asset [" + base.name + "] because its AtlasPopulationMode is set to Static.", this);
					}
					else
					{
						Debug.LogWarning("Unable to add characters to font asset [" + base.name + "] because the provided Unicode list is Null or Empty.", this);
					}
					missingUnicodes = null;
					flag3 = false;
				}
				else
				{
					bool flag4 = this.LoadFontFace() > FontEngineError.Success;
					if (flag4)
					{
						missingUnicodes = new uint[unicodes.Length];
						int num = 0;
						foreach (uint num2 in unicodes)
						{
							missingUnicodes[num++] = num2;
						}
						flag3 = false;
					}
					else
					{
						bool flag5 = this.m_CharacterLookupDictionary == null || this.m_GlyphLookupDictionary == null;
						if (flag5)
						{
							this.ReadFontAssetDefinition();
						}
						Dictionary<uint, Character> characterLookupDictionary = this.m_CharacterLookupDictionary;
						Dictionary<uint, Glyph> glyphLookupDictionary = this.m_GlyphLookupDictionary;
						this.m_GlyphsToAdd.Clear();
						this.m_GlyphsToAddLookup.Clear();
						this.m_CharactersToAdd.Clear();
						this.m_CharactersToAddLookup.Clear();
						this.s_MissingCharacterList.Clear();
						bool flag6 = false;
						int num3 = unicodes.Length;
						for (int j = 0; j < num3; j++)
						{
							uint codePoint = FontAssetUtilities.GetCodePoint(unicodes, ref j);
							bool flag7 = characterLookupDictionary.ContainsKey(codePoint);
							if (!flag7)
							{
								uint num4 = FontEngine.GetGlyphIndex(codePoint);
								bool flag8 = num4 == 0U;
								if (flag8)
								{
									uint num5 = codePoint;
									uint num6 = num5;
									if (num6 != 160U)
									{
										if (num6 == 173U || num6 == 8209U)
										{
											num4 = FontEngine.GetGlyphIndex(45U);
										}
									}
									else
									{
										num4 = FontEngine.GetGlyphIndex(32U);
									}
									bool flag9 = num4 == 0U;
									if (flag9)
									{
										this.s_MissingCharacterList.Add(codePoint);
										flag6 = true;
										goto IL_0266;
									}
								}
								Character character = new Character(codePoint, num4);
								Glyph glyph;
								bool flag10 = glyphLookupDictionary.TryGetValue(num4, out glyph);
								if (flag10)
								{
									character.glyph = glyph;
									character.textAsset = this;
									this.m_CharacterTable.Add(character);
									characterLookupDictionary.Add(codePoint, character);
								}
								else
								{
									bool flag11 = this.m_GlyphsToAddLookup.Add(num4);
									if (flag11)
									{
										this.m_GlyphsToAdd.Add(num4);
									}
									bool flag12 = this.m_CharactersToAddLookup.Add(codePoint);
									if (flag12)
									{
										this.m_CharactersToAdd.Add(character);
									}
								}
							}
							IL_0266:;
						}
						bool flag13 = this.m_GlyphsToAdd.Count == 0;
						if (flag13)
						{
							missingUnicodes = unicodes;
							flag3 = !flag6;
						}
						else
						{
							bool flag14 = this.m_AtlasTextures[this.m_AtlasTextureIndex].width <= 1 || this.m_AtlasTextures[this.m_AtlasTextureIndex].height <= 1;
							if (flag14)
							{
								this.m_AtlasTextures[this.m_AtlasTextureIndex].Reinitialize(this.m_AtlasWidth, this.m_AtlasHeight);
								FontEngine.ResetAtlasTexture(this.m_AtlasTextures[this.m_AtlasTextureIndex]);
							}
							Glyph[] array;
							bool flag15 = FontEngine.TryAddGlyphsToTexture(this.m_GlyphsToAdd, this.m_AtlasPadding, GlyphPackingMode.BestShortSideFit, this.m_FreeGlyphRects, this.m_UsedGlyphRects, this.m_AtlasRenderMode, this.m_AtlasTextures[this.m_AtlasTextureIndex], out array);
							int num7 = array.Length;
							FontAsset.EnsureAdditionalCapacity<Glyph>(this.m_GlyphTable, num7);
							FontAsset.EnsureAdditionalCapacity<uint, Glyph>(glyphLookupDictionary, num7);
							FontAsset.EnsureAdditionalCapacity<uint>(this.m_GlyphIndexListNewlyAdded, num7);
							FontAsset.EnsureAdditionalCapacity<uint>(this.m_GlyphIndexList, num7);
							int num8 = 0;
							while (num8 < array.Length && array[num8] != null)
							{
								Glyph glyph2 = array[num8];
								uint index = glyph2.index;
								glyph2.atlasIndex = this.m_AtlasTextureIndex;
								this.m_GlyphTable.Add(glyph2);
								glyphLookupDictionary.Add(index, glyph2);
								this.m_GlyphIndexListNewlyAdded.Add(index);
								this.m_GlyphIndexList.Add(index);
								num8++;
							}
							this.m_GlyphsToAdd.Clear();
							int count = this.m_CharactersToAdd.Count;
							FontAsset.EnsureAdditionalCapacity<uint>(this.m_GlyphsToAdd, count);
							FontAsset.EnsureAdditionalCapacity<Character>(this.m_CharacterTable, count);
							FontAsset.EnsureAdditionalCapacity<uint, Character>(characterLookupDictionary, count);
							for (int k = 0; k < this.m_CharactersToAdd.Count; k++)
							{
								Character character2 = this.m_CharactersToAdd[k];
								Glyph glyph3;
								bool flag16 = !glyphLookupDictionary.TryGetValue(character2.glyphIndex, out glyph3);
								if (flag16)
								{
									this.m_GlyphsToAdd.Add(character2.glyphIndex);
								}
								else
								{
									character2.glyph = glyph3;
									character2.textAsset = this;
									this.m_CharacterTable.Add(character2);
									characterLookupDictionary.Add(character2.unicode, character2);
									this.m_CharactersToAdd.RemoveAt(k);
									k--;
								}
							}
							bool flag17 = this.m_IsMultiAtlasTexturesEnabled && !flag15;
							if (flag17)
							{
								while (!flag15)
								{
									flag15 = this.TryAddGlyphsToNewAtlasTexture();
								}
							}
							else
							{
								bool flag18 = !flag15;
								if (flag18)
								{
									Debug.Log("Atlas is full, consider enabling multi-atlas textures in the Font Asset: " + base.name);
								}
							}
							if (includeFontFeatures)
							{
								this.UpdateFontFeaturesForNewlyAddedGlyphs();
							}
							foreach (Character character3 in this.m_CharactersToAdd)
							{
								this.s_MissingCharacterList.Add(character3.unicode);
							}
							missingUnicodes = null;
							bool flag19 = this.s_MissingCharacterList.Count > 0;
							if (flag19)
							{
								missingUnicodes = this.s_MissingCharacterList.ToArray();
							}
							flag3 = flag15 && !flag6;
						}
					}
				}
			}
			return flag3;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal bool TryAddGlyphs(List<uint> glyphsToAdd)
		{
			bool flag = this.LoadFontFace() > FontEngineError.Success;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = this.m_CharacterLookupDictionary == null || this.m_GlyphLookupDictionary == null;
				if (flag3)
				{
					this.ReadFontAssetDefinition();
					glyphsToAdd.RemoveAll((uint glyphId) => this.m_GlyphLookupDictionary.ContainsKey(glyphId));
					bool flag4 = glyphsToAdd.Count == 0;
					if (flag4)
					{
						return true;
					}
				}
				bool flag5 = this.m_AtlasTextures[this.m_AtlasTextureIndex].width <= 1 || this.m_AtlasTextures[this.m_AtlasTextureIndex].height <= 1;
				if (flag5)
				{
					this.m_AtlasTextures[this.m_AtlasTextureIndex].Reinitialize(this.m_AtlasWidth, this.m_AtlasHeight);
					FontEngine.ResetAtlasTexture(this.m_AtlasTextures[this.m_AtlasTextureIndex]);
				}
				bool flag6 = false;
				while (!flag6)
				{
					Glyph[] array;
					flag6 = FontEngine.TryAddGlyphsToTexture(glyphsToAdd, this.m_AtlasPadding, GlyphPackingMode.BestShortSideFit, this.m_FreeGlyphRects, this.m_UsedGlyphRects, this.m_AtlasRenderMode, this.m_AtlasTextures[this.m_AtlasTextureIndex], out array);
					int num = array.Length;
					FontAsset.EnsureAdditionalCapacity<Glyph>(this.m_GlyphTable, num);
					FontAsset.EnsureAdditionalCapacity<uint, Glyph>(this.m_GlyphLookupDictionary, num);
					FontAsset.EnsureAdditionalCapacity<uint>(this.m_GlyphIndexListNewlyAdded, num);
					FontAsset.EnsureAdditionalCapacity<uint>(this.m_GlyphIndexList, num);
					HashSet<uint> successfullyAddedGlyphIndices = new HashSet<uint>();
					int num2 = 0;
					while (num2 < array.Length && array[num2] != null)
					{
						Glyph glyph = array[num2];
						uint index = glyph.index;
						glyph.atlasIndex = this.m_AtlasTextureIndex;
						this.m_GlyphTable.Add(glyph);
						this.m_GlyphLookupDictionary.Add(index, glyph);
						this.m_GlyphIndexListNewlyAdded.Add(index);
						this.m_GlyphIndexList.Add(index);
						successfullyAddedGlyphIndices.Add(index);
						num2++;
					}
					bool flag7 = successfullyAddedGlyphIndices.Count > 0;
					if (flag7)
					{
						glyphsToAdd.RemoveAll((uint id) => successfullyAddedGlyphIndices.Contains(id));
					}
					FontAsset.RegisterAtlasTextureForApply(this.m_AtlasTextures[this.m_AtlasTextureIndex]);
					bool flag8 = !this.m_IsMultiAtlasTexturesEnabled && !flag6;
					if (flag8)
					{
						Debug.Log("Atlas is full, consider enabling multi-atlas textures in the Font Asset: " + base.name);
						break;
					}
					bool flag9 = !flag6;
					if (flag9)
					{
						this.SetupNewAtlasTexture();
					}
				}
				bool flag10 = this.m_GetFontFeatures && this.m_GlyphIndexListNewlyAdded.Count > 0;
				if (flag10)
				{
					FontAsset.RegisterFontAssetForKerningUpdate(this);
				}
				FontEngine.SetTextureUploadMode(true);
				flag2 = flag6;
			}
			return flag2;
		}

		public bool TryAddCharacters(string characters, bool includeFontFeatures = false)
		{
			string text;
			return this.TryAddCharacters(characters, out text, includeFontFeatures);
		}

		public bool TryAddCharacters(string characters, out string missingCharacters, bool includeFontFeatures = false)
		{
			uint[] array = new uint[characters.Length];
			for (int i = 0; i < characters.Length; i++)
			{
				array[i] = (uint)characters[i];
			}
			uint[] array2;
			bool flag = this.TryAddCharacters(array, out array2, includeFontFeatures);
			bool flag2 = array2 == null || array2.Length == 0;
			bool flag3;
			if (flag2)
			{
				missingCharacters = null;
				flag3 = flag;
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder(array2.Length);
				foreach (uint num in array2)
				{
					stringBuilder.Append((char)num);
				}
				missingCharacters = stringBuilder.ToString();
				flag3 = flag;
			}
			return flag3;
		}

		internal bool TryAddGlyphVariantIndexInternal(uint unicode, uint nextCharacter, uint variantGlyphIndex)
		{
			return this.m_VariantGlyphIndexes.TryAdd(new ValueTuple<uint, uint>(unicode, nextCharacter), variantGlyphIndex);
		}

		internal bool TryGetGlyphVariantIndexInternal(uint unicode, uint nextCharacter, out uint variantGlyphIndex)
		{
			return this.m_VariantGlyphIndexes.TryGetValue(new ValueTuple<uint, uint>(unicode, nextCharacter), out variantGlyphIndex);
		}

		internal bool TryAddGlyphInternal(uint glyphIndex, out Glyph glyph, bool populateLigatures = true)
		{
			bool flag2;
			using (FontAsset.k_TryAddGlyphMarker.Auto())
			{
				glyph = null;
				bool flag = this.glyphLookupTable.TryGetValue(glyphIndex, out glyph);
				if (flag)
				{
					flag2 = true;
				}
				else
				{
					bool flag3 = this.LoadFontFace() > FontEngineError.Success;
					if (flag3)
					{
						flag2 = false;
					}
					else
					{
						flag2 = this.TryAddGlyphToAtlas(glyphIndex, out glyph, populateLigatures);
					}
				}
			}
			return flag2;
		}

		internal bool TryAddCharacterInternal(uint unicode, out Character character)
		{
			return this.TryAddCharacterInternal(unicode, FontStyles.Normal, TextFontWeight.Regular, out character, true);
		}

		internal bool TryAddCharacterInternal(uint unicode, FontStyles fontStyle, TextFontWeight fontWeight, out Character character, bool populateLigatures = true)
		{
			bool flag2;
			using (FontAsset.k_TryAddCharacterMarker.Auto())
			{
				character = null;
				bool flag = this.m_MissingUnicodesFromFontFile.Contains(unicode);
				if (flag)
				{
					flag2 = false;
				}
				else
				{
					bool flag3 = this.LoadFontFace() > FontEngineError.Success;
					if (flag3)
					{
						flag2 = false;
					}
					else
					{
						uint num = FontEngine.GetGlyphIndex(unicode);
						bool flag4 = num == 0U;
						if (flag4)
						{
							if (unicode != 160U)
							{
								if (unicode == 173U || unicode == 8209U)
								{
									num = FontEngine.GetGlyphIndex(45U);
								}
							}
							else
							{
								num = FontEngine.GetGlyphIndex(32U);
							}
							bool flag5 = num == 0U;
							if (flag5)
							{
								this.m_MissingUnicodesFromFontFile.Add(unicode);
								return false;
							}
						}
						bool flag6 = this.glyphLookupTable.ContainsKey(num);
						if (flag6)
						{
							character = this.CreateCharacterAndAddToCache(unicode, this.m_GlyphLookupDictionary[num], fontStyle, fontWeight);
							flag2 = true;
						}
						else
						{
							Glyph glyph = null;
							bool flag7 = this.TryAddGlyphToAtlas(num, out glyph, populateLigatures);
							if (flag7)
							{
								character = this.CreateCharacterAndAddToCache(unicode, glyph, fontStyle, fontWeight);
								flag2 = true;
							}
							else
							{
								flag2 = false;
							}
						}
					}
				}
			}
			return flag2;
		}

		private bool TryAddGlyphToAtlas(uint glyphIndex, out Glyph glyph, bool populateLigatures = true)
		{
			glyph = null;
			bool flag = !this.m_AtlasTextures[this.m_AtlasTextureIndex].isReadable;
			bool flag2;
			if (flag)
			{
				Debug.LogWarning(string.Concat(new string[]
				{
					"Unable to add the requested glyph to font asset [",
					base.name,
					"]'s atlas texture. Please make the texture [",
					this.m_AtlasTextures[this.m_AtlasTextureIndex].name,
					"] readable."
				}), this.m_AtlasTextures[this.m_AtlasTextureIndex]);
				flag2 = false;
			}
			else
			{
				bool flag3 = this.m_AtlasTextures[this.m_AtlasTextureIndex].width <= 1 || this.m_AtlasTextures[this.m_AtlasTextureIndex].height <= 1;
				if (flag3)
				{
					this.m_AtlasTextures[this.m_AtlasTextureIndex].Reinitialize(this.m_AtlasWidth, this.m_AtlasHeight);
					FontEngine.ResetAtlasTexture(this.m_AtlasTextures[this.m_AtlasTextureIndex]);
				}
				FontEngine.SetTextureUploadMode(false);
				bool flag4 = this.TryAddGlyphToTexture(glyphIndex, out glyph, populateLigatures);
				if (flag4)
				{
					flag2 = true;
				}
				else
				{
					bool flag5 = this.m_IsMultiAtlasTexturesEnabled && this.m_UsedGlyphRects.Count > 0;
					if (flag5)
					{
						this.SetupNewAtlasTexture();
						FontEngine.SetTextureUploadMode(false);
						bool flag6 = this.TryAddGlyphToTexture(glyphIndex, out glyph, populateLigatures);
						if (flag6)
						{
							return true;
						}
					}
					else
					{
						bool flag7 = this.m_UsedGlyphRects.Count > 0;
						if (flag7)
						{
							Debug.Log("Atlas is full, consider enabling multi-atlas textures in the Font Asset: " + base.name);
						}
					}
					flag2 = false;
				}
			}
			return flag2;
		}

		private bool TryAddGlyphToTexture(uint glyphIndex, out Glyph glyph, bool populateLigatures = true)
		{
			bool flag = FontEngine.TryAddGlyphToTexture(glyphIndex, this.m_AtlasPadding, GlyphPackingMode.BestShortSideFit, this.m_FreeGlyphRects, this.m_UsedGlyphRects, this.m_AtlasRenderMode, this.m_AtlasTextures[this.m_AtlasTextureIndex], out glyph);
			bool flag2;
			if (flag)
			{
				glyph.atlasIndex = this.m_AtlasTextureIndex;
				this.m_GlyphTable.Add(glyph);
				this.m_GlyphLookupDictionary.Add(glyphIndex, glyph);
				this.m_GlyphIndexList.Add(glyphIndex);
				this.m_GlyphIndexListNewlyAdded.Add(glyphIndex);
				bool getFontFeatures = this.m_GetFontFeatures;
				if (getFontFeatures)
				{
					if (populateLigatures)
					{
						this.UpdateGSUBFontFeaturesForNewGlyphIndex(glyphIndex);
						FontAsset.RegisterFontAssetForFontFeatureUpdate(this);
					}
					else
					{
						FontAsset.RegisterFontAssetForKerningUpdate(this);
					}
				}
				FontAsset.RegisterAtlasTextureForApply(this.m_AtlasTextures[this.m_AtlasTextureIndex]);
				FontEngine.SetTextureUploadMode(true);
				flag2 = true;
			}
			else
			{
				flag2 = false;
			}
			return flag2;
		}

		private bool TryAddGlyphsToNewAtlasTexture()
		{
			this.SetupNewAtlasTexture();
			Glyph[] array;
			bool flag = FontEngine.TryAddGlyphsToTexture(this.m_GlyphsToAdd, this.m_AtlasPadding, GlyphPackingMode.BestShortSideFit, this.m_FreeGlyphRects, this.m_UsedGlyphRects, this.m_AtlasRenderMode, this.m_AtlasTextures[this.m_AtlasTextureIndex], out array);
			int num = 0;
			while (num < array.Length && array[num] != null)
			{
				Glyph glyph = array[num];
				uint index = glyph.index;
				glyph.atlasIndex = this.m_AtlasTextureIndex;
				this.m_GlyphTable.Add(glyph);
				this.m_GlyphLookupDictionary.Add(index, glyph);
				this.m_GlyphIndexListNewlyAdded.Add(index);
				this.m_GlyphIndexList.Add(index);
				num++;
			}
			this.m_GlyphsToAdd.Clear();
			for (int i = 0; i < this.m_CharactersToAdd.Count; i++)
			{
				Character character = this.m_CharactersToAdd[i];
				Glyph glyph2;
				bool flag2 = !this.m_GlyphLookupDictionary.TryGetValue(character.glyphIndex, out glyph2);
				if (flag2)
				{
					this.m_GlyphsToAdd.Add(character.glyphIndex);
				}
				else
				{
					character.glyph = glyph2;
					character.textAsset = this;
					this.m_CharacterTable.Add(character);
					this.m_CharacterLookupDictionary.Add(character.unicode, character);
					this.m_CharactersToAdd.RemoveAt(i);
					i--;
				}
			}
			return flag;
		}

		private void SetupNewAtlasTexture()
		{
			this.m_AtlasTextureIndex++;
			bool flag = this.m_AtlasTextures.Length == this.m_AtlasTextureIndex;
			if (flag)
			{
				Array.Resize<Texture2D>(ref this.m_AtlasTextures, this.m_AtlasTextures.Length * 2);
			}
			TextureFormat textureFormat = (((this.m_AtlasRenderMode & (GlyphRenderMode)65536) == (GlyphRenderMode)65536) ? TextureFormat.RGBA32 : TextureFormat.Alpha8);
			this.m_AtlasTextures[this.m_AtlasTextureIndex] = new Texture2D(this.m_AtlasWidth, this.m_AtlasHeight, textureFormat, false);
			this.m_AtlasTextures[this.m_AtlasTextureIndex].hideFlags = this.m_AtlasTextures[0].hideFlags;
			FontEngine.ResetAtlasTexture(this.m_AtlasTextures[this.m_AtlasTextureIndex]);
			int num = (((this.m_AtlasRenderMode & (GlyphRenderMode)16) == (GlyphRenderMode)16) ? 0 : 1);
			this.m_FreeGlyphRects.Clear();
			this.m_FreeGlyphRects.Add(new GlyphRect(0, 0, this.m_AtlasWidth - num, this.m_AtlasHeight - num));
			this.m_UsedGlyphRects.Clear();
		}

		private Character CreateCharacterAndAddToCache(uint unicode, Glyph glyph, FontStyles fontStyle, TextFontWeight fontWeight)
		{
			Character character;
			bool flag = !this.m_CharacterLookupDictionary.TryGetValue(unicode, out character);
			if (flag)
			{
				character = new Character(unicode, this, glyph);
				this.m_CharacterTable.Add(character);
				this.AddCharacterToLookupCache(unicode, character, FontStyles.Normal, TextFontWeight.Regular);
			}
			bool flag2 = fontStyle != FontStyles.Normal || fontWeight != TextFontWeight.Regular;
			if (flag2)
			{
				this.AddCharacterToLookupCache(unicode, character, fontStyle, fontWeight);
			}
			return character;
		}

		private void UpdateFontFeaturesForNewlyAddedGlyphs()
		{
			this.UpdateLigatureSubstitutionRecords();
			this.UpdateGlyphAdjustmentRecords();
			this.UpdateDiacriticalMarkAdjustmentRecords();
			this.m_GlyphIndexListNewlyAdded.Clear();
		}

		private void UpdateGlyphAdjustmentRecordsForNewGlyphs()
		{
			this.UpdateGlyphAdjustmentRecords();
			this.m_GlyphIndexListNewlyAdded.Clear();
		}

		private void UpdateGPOSFontFeaturesForNewlyAddedGlyphs()
		{
			this.UpdateGlyphAdjustmentRecords();
			this.UpdateDiacriticalMarkAdjustmentRecords();
			this.m_GlyphIndexListNewlyAdded.Clear();
		}

		internal void ImportFontFeatures()
		{
			bool flag = this.LoadFontFace() > FontEngineError.Success;
			if (!flag)
			{
				GlyphPairAdjustmentRecord[] allPairAdjustmentRecords = FontEngine.GetAllPairAdjustmentRecords();
				bool flag2 = allPairAdjustmentRecords != null;
				if (flag2)
				{
					this.AddPairAdjustmentRecords(allPairAdjustmentRecords);
				}
				MarkToBaseAdjustmentRecord[] allMarkToBaseAdjustmentRecords = FontEngine.GetAllMarkToBaseAdjustmentRecords();
				bool flag3 = allMarkToBaseAdjustmentRecords != null;
				if (flag3)
				{
					this.AddMarkToBaseAdjustmentRecords(allMarkToBaseAdjustmentRecords);
				}
				MarkToMarkAdjustmentRecord[] allMarkToMarkAdjustmentRecords = FontEngine.GetAllMarkToMarkAdjustmentRecords();
				bool flag4 = allMarkToMarkAdjustmentRecords != null;
				if (flag4)
				{
					this.AddMarkToMarkAdjustmentRecords(allMarkToMarkAdjustmentRecords);
				}
				LigatureSubstitutionRecord[] allLigatureSubstitutionRecords = FontEngine.GetAllLigatureSubstitutionRecords();
				bool flag5 = allLigatureSubstitutionRecords != null;
				if (flag5)
				{
					this.AddLigatureSubstitutionRecords(allLigatureSubstitutionRecords);
				}
				this.m_ShouldReimportFontFeatures = false;
			}
		}

		private void UpdateGSUBFontFeaturesForNewGlyphIndex(uint glyphIndex)
		{
			LigatureSubstitutionRecord[] ligatureSubstitutionRecords = FontEngine.GetLigatureSubstitutionRecords(glyphIndex);
			bool flag = ligatureSubstitutionRecords != null;
			if (flag)
			{
				this.AddLigatureSubstitutionRecords(ligatureSubstitutionRecords);
			}
		}

		internal void UpdateLigatureSubstitutionRecords()
		{
			LigatureSubstitutionRecord[] ligatureSubstitutionRecords = FontEngine.GetLigatureSubstitutionRecords(this.m_GlyphIndexListNewlyAdded);
			bool flag = ligatureSubstitutionRecords != null;
			if (flag)
			{
				this.AddLigatureSubstitutionRecords(ligatureSubstitutionRecords);
			}
		}

		private void AddLigatureSubstitutionRecords(LigatureSubstitutionRecord[] records)
		{
			Dictionary<uint, List<LigatureSubstitutionRecord>> ligatureSubstitutionRecordLookup = this.m_FontFeatureTable.m_LigatureSubstitutionRecordLookup;
			List<LigatureSubstitutionRecord> ligatureSubstitutionRecords = this.m_FontFeatureTable.m_LigatureSubstitutionRecords;
			FontAsset.EnsureAdditionalCapacity<uint, List<LigatureSubstitutionRecord>>(ligatureSubstitutionRecordLookup, records.Length);
			FontAsset.EnsureAdditionalCapacity<LigatureSubstitutionRecord>(ligatureSubstitutionRecords, records.Length);
			foreach (LigatureSubstitutionRecord ligatureSubstitutionRecord in records)
			{
				bool flag = ligatureSubstitutionRecord.componentGlyphIDs == null || ligatureSubstitutionRecord.ligatureGlyphID == 0U;
				if (flag)
				{
					break;
				}
				uint num = ligatureSubstitutionRecord.componentGlyphIDs[0];
				LigatureSubstitutionRecord ligatureSubstitutionRecord2 = new LigatureSubstitutionRecord
				{
					componentGlyphIDs = ligatureSubstitutionRecord.componentGlyphIDs,
					ligatureGlyphID = ligatureSubstitutionRecord.ligatureGlyphID
				};
				List<LigatureSubstitutionRecord> list;
				bool flag2 = ligatureSubstitutionRecordLookup.TryGetValue(num, out list);
				if (flag2)
				{
					foreach (LigatureSubstitutionRecord ligatureSubstitutionRecord3 in list)
					{
						bool flag3 = ligatureSubstitutionRecord2 == ligatureSubstitutionRecord3;
						if (flag3)
						{
							return;
						}
					}
					ligatureSubstitutionRecordLookup[num].Add(ligatureSubstitutionRecord2);
				}
				else
				{
					ligatureSubstitutionRecordLookup.Add(num, new List<LigatureSubstitutionRecord> { ligatureSubstitutionRecord2 });
				}
				ligatureSubstitutionRecords.Add(ligatureSubstitutionRecord2);
			}
		}

		internal void UpdateGlyphAdjustmentRecords()
		{
			this.LoadFontFace();
			GlyphPairAdjustmentRecord[] pairAdjustmentRecords = FontEngine.GetPairAdjustmentRecords(this.m_GlyphIndexListNewlyAdded);
			bool flag = pairAdjustmentRecords != null;
			if (flag)
			{
				this.AddPairAdjustmentRecords(pairAdjustmentRecords);
			}
		}

		private void AddPairAdjustmentRecords(GlyphPairAdjustmentRecord[] records)
		{
			float num = this.m_FaceInfo.pointSize / (float)this.m_FaceInfo.unitsPerEM;
			List<GlyphPairAdjustmentRecord> glyphPairAdjustmentRecords = this.m_FontFeatureTable.glyphPairAdjustmentRecords;
			Dictionary<uint, GlyphPairAdjustmentRecord> glyphPairAdjustmentRecordLookup = this.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup;
			FontAsset.EnsureAdditionalCapacity<uint, GlyphPairAdjustmentRecord>(glyphPairAdjustmentRecordLookup, records.Length);
			FontAsset.EnsureAdditionalCapacity<GlyphPairAdjustmentRecord>(glyphPairAdjustmentRecords, records.Length);
			foreach (GlyphPairAdjustmentRecord glyphPairAdjustmentRecord in records)
			{
				GlyphAdjustmentRecord firstAdjustmentRecord = glyphPairAdjustmentRecord.firstAdjustmentRecord;
				GlyphAdjustmentRecord secondAdjustmentRecord = glyphPairAdjustmentRecord.secondAdjustmentRecord;
				uint glyphIndex = firstAdjustmentRecord.glyphIndex;
				uint glyphIndex2 = secondAdjustmentRecord.glyphIndex;
				bool flag = glyphIndex == 0U && glyphIndex2 == 0U;
				if (flag)
				{
					break;
				}
				uint num2 = (glyphIndex2 << 16) | glyphIndex;
				GlyphPairAdjustmentRecord glyphPairAdjustmentRecord2 = glyphPairAdjustmentRecord;
				GlyphValueRecord glyphValueRecord = firstAdjustmentRecord.glyphValueRecord;
				glyphValueRecord.xAdvance *= num;
				glyphPairAdjustmentRecord2.firstAdjustmentRecord = new GlyphAdjustmentRecord(glyphIndex, glyphValueRecord);
				bool flag2 = glyphPairAdjustmentRecordLookup.TryAdd(num2, glyphPairAdjustmentRecord2);
				if (flag2)
				{
					glyphPairAdjustmentRecords.Add(glyphPairAdjustmentRecord2);
				}
			}
		}

		internal void UpdateDiacriticalMarkAdjustmentRecords()
		{
			using (FontAsset.k_UpdateDiacriticalMarkAdjustmentRecordsMarker.Auto())
			{
				MarkToBaseAdjustmentRecord[] markToBaseAdjustmentRecords = FontEngine.GetMarkToBaseAdjustmentRecords(this.m_GlyphIndexListNewlyAdded);
				bool flag = markToBaseAdjustmentRecords != null;
				if (flag)
				{
					this.AddMarkToBaseAdjustmentRecords(markToBaseAdjustmentRecords);
				}
				MarkToMarkAdjustmentRecord[] markToMarkAdjustmentRecords = FontEngine.GetMarkToMarkAdjustmentRecords(this.m_GlyphIndexListNewlyAdded);
				bool flag2 = markToMarkAdjustmentRecords != null;
				if (flag2)
				{
					this.AddMarkToMarkAdjustmentRecords(markToMarkAdjustmentRecords);
				}
			}
		}

		private void AddMarkToBaseAdjustmentRecords(MarkToBaseAdjustmentRecord[] records)
		{
			float num = this.m_FaceInfo.pointSize / (float)this.m_FaceInfo.unitsPerEM;
			foreach (MarkToBaseAdjustmentRecord markToBaseAdjustmentRecord in records)
			{
				bool flag = markToBaseAdjustmentRecord.baseGlyphID == 0U || markToBaseAdjustmentRecord.markGlyphID == 0U;
				if (flag)
				{
					break;
				}
				uint num2 = (markToBaseAdjustmentRecord.markGlyphID << 16) | markToBaseAdjustmentRecord.baseGlyphID;
				bool flag2 = this.m_FontFeatureTable.m_MarkToBaseAdjustmentRecordLookup.ContainsKey(num2);
				if (!flag2)
				{
					MarkToBaseAdjustmentRecord markToBaseAdjustmentRecord2 = new MarkToBaseAdjustmentRecord
					{
						baseGlyphID = markToBaseAdjustmentRecord.baseGlyphID,
						baseGlyphAnchorPoint = new GlyphAnchorPoint
						{
							xCoordinate = markToBaseAdjustmentRecord.baseGlyphAnchorPoint.xCoordinate * num,
							yCoordinate = markToBaseAdjustmentRecord.baseGlyphAnchorPoint.yCoordinate * num
						},
						markGlyphID = markToBaseAdjustmentRecord.markGlyphID,
						markPositionAdjustment = new MarkPositionAdjustment
						{
							xPositionAdjustment = markToBaseAdjustmentRecord.markPositionAdjustment.xPositionAdjustment * num,
							yPositionAdjustment = markToBaseAdjustmentRecord.markPositionAdjustment.yPositionAdjustment * num
						}
					};
					this.m_FontFeatureTable.MarkToBaseAdjustmentRecords.Add(markToBaseAdjustmentRecord2);
					this.m_FontFeatureTable.m_MarkToBaseAdjustmentRecordLookup.Add(num2, markToBaseAdjustmentRecord2);
				}
			}
		}

		private void AddMarkToMarkAdjustmentRecords(MarkToMarkAdjustmentRecord[] records)
		{
			float num = this.m_FaceInfo.pointSize / (float)this.m_FaceInfo.unitsPerEM;
			for (int i = 0; i < records.Length; i++)
			{
				MarkToMarkAdjustmentRecord markToMarkAdjustmentRecord = records[i];
				bool flag = records[i].baseMarkGlyphID == 0U || records[i].combiningMarkGlyphID == 0U;
				if (flag)
				{
					break;
				}
				uint num2 = (markToMarkAdjustmentRecord.combiningMarkGlyphID << 16) | markToMarkAdjustmentRecord.baseMarkGlyphID;
				bool flag2 = this.m_FontFeatureTable.m_MarkToMarkAdjustmentRecordLookup.ContainsKey(num2);
				if (!flag2)
				{
					MarkToMarkAdjustmentRecord markToMarkAdjustmentRecord2 = new MarkToMarkAdjustmentRecord
					{
						baseMarkGlyphID = markToMarkAdjustmentRecord.baseMarkGlyphID,
						baseMarkGlyphAnchorPoint = new GlyphAnchorPoint
						{
							xCoordinate = markToMarkAdjustmentRecord.baseMarkGlyphAnchorPoint.xCoordinate * num,
							yCoordinate = markToMarkAdjustmentRecord.baseMarkGlyphAnchorPoint.yCoordinate * num
						},
						combiningMarkGlyphID = markToMarkAdjustmentRecord.combiningMarkGlyphID,
						combiningMarkPositionAdjustment = new MarkPositionAdjustment
						{
							xPositionAdjustment = markToMarkAdjustmentRecord.combiningMarkPositionAdjustment.xPositionAdjustment * num,
							yPositionAdjustment = markToMarkAdjustmentRecord.combiningMarkPositionAdjustment.yPositionAdjustment * num
						}
					};
					this.m_FontFeatureTable.MarkToMarkAdjustmentRecords.Add(markToMarkAdjustmentRecord2);
					this.m_FontFeatureTable.m_MarkToMarkAdjustmentRecordLookup.Add(num2, markToMarkAdjustmentRecord2);
				}
			}
		}

		internal IntPtr nativeFontAsset
		{
			[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
			get
			{
				this.EnsureNativeFontAssetIsCreated();
				return this.m_NativeFontAsset;
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal void EnsureNativeFontAssetIsCreated()
		{
			bool flag = this.m_NativeFontAsset != IntPtr.Zero;
			if (!flag)
			{
				bool isExecutingJob = JobsUtility.IsExecutingJob;
				if (!isExecutingJob)
				{
					bool flag2 = this.atlasPopulationMode == AtlasPopulationMode.Static && this.characterTable.Count > 0;
					if (flag2)
					{
						Debug.LogWarning("Advanced text system cannot use static font asset " + base.name + ".");
					}
					else
					{
						bool flag3 = this.atlasPopulationMode == AtlasPopulationMode.Dynamic && this.sourceFontFile == null;
						if (flag3)
						{
							Debug.LogWarning(base.name + " FontAsset is invalid. Please assign a Source Font File.");
						}
						else
						{
							IntPtr[] fallbacks = this.GetFallbacks();
							ValueTuple<IntPtr[], IntPtr[]> weightFallbacks = this.GetWeightFallbacks();
							Font font = null;
							this.m_NativeFontAsset = FontAsset.Create(this.faceInfo, this.sourceFontFile, font, this.m_SourceFontFilePath, base.instanceID, fallbacks, weightFallbacks.Item1, weightFallbacks.Item2, this.m_AtlasRenderMode, Object.MarshalledUnityObject.MarshalNotNull<FontAsset>(this));
						}
					}
				}
			}
		}

		internal void UpdateFallbacks()
		{
			FontAsset.UpdateFallbacks(this.nativeFontAsset, this.GetFallbacks());
		}

		internal void UpdateWeightFallbacks()
		{
			ValueTuple<IntPtr[], IntPtr[]> weightFallbacks = this.GetWeightFallbacks();
			FontAsset.UpdateWeightFallbacks(this.nativeFontAsset, weightFallbacks.Item1, weightFallbacks.Item2);
		}

		internal void UpdateFaceInfo()
		{
			FontAsset.UpdateFaceInfo(this.nativeFontAsset, this.faceInfo);
		}

		internal void UpdateRenderMode()
		{
			FontAsset.UpdateRenderMode(this.nativeFontAsset, this.m_AtlasRenderMode);
		}

		internal IntPtr[] GetFallbacks()
		{
			List<IntPtr> list = new List<IntPtr>();
			bool flag = this.fallbackFontAssetTable == null;
			IntPtr[] array;
			if (flag)
			{
				array = list.ToArray();
			}
			else
			{
				foreach (FontAsset fontAsset in this.fallbackFontAssetTable)
				{
					bool flag2 = fontAsset == null;
					if (!flag2)
					{
						bool flag3 = fontAsset.atlasPopulationMode == AtlasPopulationMode.Static && fontAsset.characterTable.Count > 0;
						if (flag3)
						{
							Debug.LogWarning("Advanced text system cannot use static font asset " + fontAsset.name + " as fallback.");
						}
						else
						{
							bool flag4 = this.HasRecursion(fontAsset);
							if (!flag4)
							{
								list.Add(fontAsset.nativeFontAsset);
							}
						}
					}
				}
				array = list.ToArray();
			}
			return array;
		}

		private bool HasRecursion(FontAsset fontAsset)
		{
			FontAsset.visitedFontAssets.Clear();
			return this.HasRecursionInternal(fontAsset);
		}

		private bool HasRecursionInternal(FontAsset fontAsset)
		{
			bool flag = FontAsset.visitedFontAssets.Contains(fontAsset.instanceID);
			bool flag2;
			if (flag)
			{
				flag2 = true;
			}
			else
			{
				FontAsset.visitedFontAssets.Add(fontAsset.instanceID);
				bool flag3 = fontAsset.fallbackFontAssetTable != null;
				if (flag3)
				{
					foreach (FontAsset fontAsset2 in fontAsset.fallbackFontAssetTable)
					{
						bool flag4 = this.HasRecursionInternal(fontAsset2);
						if (flag4)
						{
							return true;
						}
					}
				}
				for (int i = 0; i < fontAsset.fontWeightTable.Length; i++)
				{
					FontWeightPair fontWeightPair = fontAsset.fontWeightTable[i];
					bool flag5 = fontWeightPair.regularTypeface != null;
					if (flag5)
					{
						bool flag6 = this.HasRecursionInternal(fontWeightPair.regularTypeface);
						if (flag6)
						{
							return true;
						}
					}
					bool flag7 = fontWeightPair.italicTypeface != null;
					if (flag7)
					{
						bool flag8 = this.HasRecursionInternal(fontWeightPair.italicTypeface);
						if (flag8)
						{
							return true;
						}
					}
				}
				FontAsset.visitedFontAssets.Remove(fontAsset.instanceID);
				flag2 = false;
			}
			return flag2;
		}

		private ValueTuple<IntPtr[], IntPtr[]> GetWeightFallbacks()
		{
			IntPtr[] array = new IntPtr[10];
			IntPtr[] array2 = new IntPtr[10];
			int i = 0;
			while (i < this.fontWeightTable.Length)
			{
				FontWeightPair fontWeightPair = this.fontWeightTable[i];
				bool flag = fontWeightPair.regularTypeface != null;
				if (!flag)
				{
					goto IL_00D2;
				}
				bool flag2 = fontWeightPair.regularTypeface.atlasPopulationMode == AtlasPopulationMode.Static && fontWeightPair.regularTypeface.characterTable.Count > 0;
				if (flag2)
				{
					Debug.LogWarning("Advanced text system cannot use static font asset " + fontWeightPair.regularTypeface.name + " as fallback.");
				}
				else
				{
					bool flag3 = this.HasRecursion(fontWeightPair.regularTypeface);
					if (!flag3)
					{
						array[i] = fontWeightPair.regularTypeface.nativeFontAsset;
						goto IL_00D2;
					}
					Debug.LogWarning("Circular reference detected. Cannot add " + fontWeightPair.regularTypeface.name + " to the fallbacks.");
				}
				IL_0179:
				i++;
				continue;
				IL_00D2:
				bool flag4 = fontWeightPair.italicTypeface != null;
				if (flag4)
				{
					bool flag5 = fontWeightPair.italicTypeface.atlasPopulationMode == AtlasPopulationMode.Static && fontWeightPair.italicTypeface.characterTable.Count > 0;
					if (flag5)
					{
						Debug.LogWarning("Advanced text system cannot use static font asset " + fontWeightPair.italicTypeface.name + " as fallback.");
					}
					else
					{
						bool flag6 = this.HasRecursion(fontWeightPair.italicTypeface);
						if (flag6)
						{
							Debug.LogWarning("Circular reference detected. Cannot add " + fontWeightPair.italicTypeface.name + " to the fallbacks.");
						}
						else
						{
							array2[i] = fontWeightPair.italicTypeface.nativeFontAsset;
						}
					}
				}
				goto IL_0179;
			}
			return new ValueTuple<IntPtr[], IntPtr[]>(array, array2);
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void CreateHbFaceIfNeeded();

		private unsafe static void UpdateFallbacks(IntPtr ptr, IntPtr[] fallbacks)
		{
			Span<IntPtr> span = new Span<IntPtr>(fallbacks);
			fixed (IntPtr* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				FontAsset.UpdateFallbacks_Injected(ptr, ref managedSpanWrapper);
			}
		}

		private unsafe static void UpdateWeightFallbacks(IntPtr ptr, IntPtr[] regularFallbacks, IntPtr[] italicFallbacks)
		{
			Span<IntPtr> span = new Span<IntPtr>(regularFallbacks);
			fixed (IntPtr* ptr2 = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr2, span.Length);
				Span<IntPtr> span2 = new Span<IntPtr>(italicFallbacks);
				fixed (IntPtr* pinnableReference = span2.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)pinnableReference, span2.Length);
					FontAsset.UpdateWeightFallbacks_Injected(ptr, ref managedSpanWrapper, ref managedSpanWrapper2);
					ptr2 = null;
				}
			}
		}

		private unsafe static IntPtr Create(FaceInfo faceInfo, Font sourceFontFile, Font sourceFont_EditorRef, string sourceFontFilePath, EntityId fontEntityId, IntPtr[] fallbacks, IntPtr[] weightFallbacks, IntPtr[] italicFallbacks, GlyphRenderMode renderMode, IntPtr managedObject)
		{
			IntPtr intPtr3;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.Marshal<Font>(sourceFontFile);
				IntPtr intPtr2 = Object.MarshalledUnityObject.Marshal<Font>(sourceFont_EditorRef);
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(sourceFontFilePath, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = sourceFontFilePath.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Span<IntPtr> span = new Span<IntPtr>(fallbacks);
				fixed (IntPtr* ptr2 = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, span.Length);
					Span<IntPtr> span2 = new Span<IntPtr>(weightFallbacks);
					fixed (IntPtr* ptr3 = span2.GetPinnableReference())
					{
						ManagedSpanWrapper managedSpanWrapper3 = new ManagedSpanWrapper((void*)ptr3, span2.Length);
						Span<IntPtr> span3 = new Span<IntPtr>(italicFallbacks);
						fixed (IntPtr* ptr4 = span3.GetPinnableReference())
						{
							ManagedSpanWrapper managedSpanWrapper4 = new ManagedSpanWrapper((void*)ptr4, span3.Length);
							intPtr3 = FontAsset.Create_Injected(ref faceInfo, intPtr, intPtr2, ref managedSpanWrapper, ref fontEntityId, ref managedSpanWrapper2, ref managedSpanWrapper3, ref managedSpanWrapper4, renderMode, managedObject);
						}
					}
				}
			}
			finally
			{
				char* ptr = null;
				IntPtr* ptr2 = null;
				IntPtr* ptr3 = null;
				IntPtr* ptr4 = null;
			}
			return intPtr3;
		}

		private static void UpdateFaceInfo(IntPtr ptr, FaceInfo faceInfo)
		{
			FontAsset.UpdateFaceInfo_Injected(ptr, ref faceInfo);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UpdateRenderMode(IntPtr ptr, GlyphRenderMode renderMode);

		[FreeFunction("FontAsset::Destroy")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Destroy(IntPtr ptr, IntPtr managedObject);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UpdateFallbacks_Injected(IntPtr ptr, ref ManagedSpanWrapper fallbacks);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UpdateWeightFallbacks_Injected(IntPtr ptr, ref ManagedSpanWrapper regularFallbacks, ref ManagedSpanWrapper italicFallbacks);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Create_Injected([In] ref FaceInfo faceInfo, IntPtr sourceFontFile, IntPtr sourceFont_EditorRef, ref ManagedSpanWrapper sourceFontFilePath, [In] ref EntityId fontEntityId, ref ManagedSpanWrapper fallbacks, ref ManagedSpanWrapper weightFallbacks, ref ManagedSpanWrapper italicFallbacks, GlyphRenderMode renderMode, IntPtr managedObject);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UpdateFaceInfo_Injected(IntPtr ptr, [In] ref FaceInfo faceInfo);

		[SerializeField]
		internal string m_SourceFontFileGUID;

		[SerializeField]
		internal FontAssetCreationEditorSettings m_fontAssetCreationEditorSettings;

		[SerializeField]
		private Font m_SourceFontFile;

		[SerializeField]
		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal string m_SourceFontFilePath;

		[SerializeField]
		private AtlasPopulationMode m_AtlasPopulationMode;

		[SerializeField]
		internal bool InternalDynamicOS;

		[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
		[SerializeField]
		internal bool IsEditorFont = false;

		[SerializeField]
		internal FaceInfo m_FaceInfo;

		private int m_FamilyNameHashCode;

		private int m_StyleNameHashCode;

		[global::System.Runtime.CompilerServices.Nullable(1)]
		[SerializeField]
		internal List<Glyph> m_GlyphTable = new List<Glyph>();

		internal Dictionary<uint, Glyph> m_GlyphLookupDictionary;

		[SerializeField]
		internal List<Character> m_CharacterTable = new List<Character>();

		[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
		internal Dictionary<uint, Character> m_CharacterLookupDictionary;

		internal Texture2D m_AtlasTexture;

		[SerializeField]
		internal Texture2D[] m_AtlasTextures;

		[SerializeField]
		internal int m_AtlasTextureIndex;

		[SerializeField]
		private bool m_IsMultiAtlasTexturesEnabled = true;

		[SerializeField]
		private bool m_GetFontFeatures = true;

		[SerializeField]
		private bool m_ClearDynamicDataOnBuild = true;

		[SerializeField]
		internal int m_AtlasWidth;

		[SerializeField]
		internal int m_AtlasHeight;

		[SerializeField]
		internal int m_AtlasPadding;

		[SerializeField]
		internal GlyphRenderMode m_AtlasRenderMode;

		[SerializeField]
		private List<GlyphRect> m_UsedGlyphRects;

		[SerializeField]
		private List<GlyphRect> m_FreeGlyphRects;

		[SerializeField]
		internal FontFeatureTable m_FontFeatureTable = new FontFeatureTable();

		[SerializeField]
		internal bool m_ShouldReimportFontFeatures;

		[SerializeField]
		internal List<FontAsset> m_FallbackFontAssetTable;

		[SerializeField]
		private FontWeightPair[] m_FontWeightTable = new FontWeightPair[10];

		[FormerlySerializedAs("normalStyle")]
		[SerializeField]
		internal float m_RegularStyleWeight = 0f;

		[FormerlySerializedAs("normalSpacingOffset")]
		[SerializeField]
		internal float m_RegularStyleSpacing = 0f;

		[FormerlySerializedAs("boldStyle")]
		[SerializeField]
		internal float m_BoldStyleWeight = 0.75f;

		[FormerlySerializedAs("boldSpacing")]
		[SerializeField]
		internal float m_BoldStyleSpacing = 7f;

		[FormerlySerializedAs("italicStyle")]
		[SerializeField]
		internal byte m_ItalicStyleSlant = 35;

		[FormerlySerializedAs("tabSize")]
		[SerializeField]
		internal byte m_TabMultiple = 10;

		internal bool IsFontAssetLookupTablesDirty;

		private IntPtr m_NativeFontAsset = IntPtr.Zero;

		private List<Glyph> m_GlyphsToRender = new List<Glyph>();

		private List<Glyph> m_GlyphsRendered = new List<Glyph>();

		private List<uint> m_GlyphIndexList = new List<uint>();

		private List<uint> m_GlyphIndexListNewlyAdded = new List<uint>();

		internal List<uint> m_GlyphsToAdd = new List<uint>();

		internal HashSet<uint> m_GlyphsToAddLookup = new HashSet<uint>();

		internal List<Character> m_CharactersToAdd = new List<Character>();

		internal HashSet<uint> m_CharactersToAddLookup = new HashSet<uint>();

		internal List<uint> s_MissingCharacterList = new List<uint>();

		internal HashSet<uint> m_MissingUnicodesFromFontFile = new HashSet<uint>();

		internal Dictionary<ValueTuple<uint, uint>, uint> m_VariantGlyphIndexes = new Dictionary<ValueTuple<uint, uint>, uint>();

		internal bool m_IsClone;

		private static readonly List<WeakReference<FontAsset>> s_CallbackInstances = new List<WeakReference<FontAsset>>();

		private static ProfilerMarker k_ReadFontAssetDefinitionMarker = new ProfilerMarker("FontAsset.ReadFontAssetDefinition");

		private static ProfilerMarker k_AddSynthesizedCharactersMarker = new ProfilerMarker("FontAsset.AddSynthesizedCharacters");

		private static ProfilerMarker k_TryAddGlyphMarker = new ProfilerMarker("FontAsset.TryAddGlyph");

		private static ProfilerMarker k_TryAddCharacterMarker = new ProfilerMarker("FontAsset.TryAddCharacter");

		private static ProfilerMarker k_TryAddCharactersMarker = new ProfilerMarker("FontAsset.TryAddCharacters");

		private static ProfilerMarker k_UpdateLigatureSubstitutionRecordsMarker = new ProfilerMarker("FontAsset.UpdateLigatureSubstitutionRecords");

		private static ProfilerMarker k_UpdateGlyphAdjustmentRecordsMarker = new ProfilerMarker("FontAsset.UpdateGlyphAdjustmentRecords");

		private static ProfilerMarker k_UpdateDiacriticalMarkAdjustmentRecordsMarker = new ProfilerMarker("FontAsset.UpdateDiacriticalAdjustmentRecords");

		private static ProfilerMarker k_ClearFontAssetDataMarker = new ProfilerMarker("FontAsset.ClearFontAssetData");

		private static ProfilerMarker k_UpdateFontAssetDataMarker = new ProfilerMarker("FontAsset.UpdateFontAssetData");

		private static string s_DefaultMaterialSuffix = " Atlas Material";

		private static HashSet<int> k_SearchedFontAssetLookup;

		private static List<FontAsset> k_FontAssets_FontFeaturesUpdateQueue = new List<FontAsset>();

		private static HashSet<int> k_FontAssets_FontFeaturesUpdateQueueLookup = new HashSet<int>();

		private static List<FontAsset> k_FontAssets_KerningUpdateQueue = new List<FontAsset>();

		private static HashSet<int> k_FontAssets_KerningUpdateQueueLookup = new HashSet<int>();

		private static List<Texture2D> k_FontAssets_AtlasTexturesUpdateQueue = new List<Texture2D>();

		private static HashSet<int> k_FontAssets_AtlasTexturesUpdateQueueLookup = new HashSet<int>();

		internal static uint[] k_GlyphIndexArray;

		private static HashSet<int> visitedFontAssets = new HashSet<int>();

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(FontAsset fontAsset)
			{
				return fontAsset.m_NativeFontAsset;
			}
		}
	}
}
