using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Profiling;
using UnityEngine.Serialization;
using UnityEngine.TextCore.LowLevel;

namespace UnityEngine.TextCore.Text
{
	[ExcludeFromPreset]
	[Serializable]
	public class FontAsset : TextAsset
	{
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

		public List<Glyph> glyphTable
		{
			get
			{
				return this.m_GlyphTable;
			}
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
				bool flag = this.m_AtlasTextures == null;
				if (flag)
				{
				}
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
			FontReference fontReference;
			bool flag = FontEngine.TryGetSystemFontReference(familyName, styleName, out fontReference);
			FontAsset fontAsset;
			if (flag)
			{
				fontAsset = FontAsset.CreateFontAsset(fontReference.filePath, fontReference.faceIndex, pointSize, 9, GlyphRenderMode.SDFAA, 1024, 1024, AtlasPopulationMode.DynamicOS, true);
			}
			else
			{
				Debug.Log(string.Concat(new string[] { "Unable to find a font file with the specified Family Name [", familyName, "] and Style [", styleName, "]." }));
				fontAsset = null;
			}
			return fontAsset;
		}

		public static FontAsset CreateFontAsset(string fontFilePath, int faceIndex, int samplingPointSize, int atlasPadding, GlyphRenderMode renderMode, int atlasWidth, int atlasHeight)
		{
			return FontAsset.CreateFontAsset(fontFilePath, faceIndex, samplingPointSize, atlasPadding, renderMode, atlasWidth, atlasHeight, AtlasPopulationMode.Dynamic, true);
		}

		private static FontAsset CreateFontAsset(string fontFilePath, int faceIndex, int samplingPointSize, int atlasPadding, GlyphRenderMode renderMode, int atlasWidth, int atlasHeight, AtlasPopulationMode atlasPopulationMode = AtlasPopulationMode.DynamicOS, bool enableMultiAtlasSupport = true)
		{
			bool flag = FontEngine.LoadFontFace(fontFilePath, samplingPointSize, faceIndex) > FontEngineError.Success;
			FontAsset fontAsset;
			if (flag)
			{
				Debug.Log("Unable to load font face from [" + fontFilePath + "].");
				fontAsset = null;
			}
			else
			{
				FontAsset fontAsset2 = FontAsset.CreateFontAssetInstance(null, atlasPadding, renderMode, atlasWidth, atlasHeight, atlasPopulationMode, enableMultiAtlasSupport);
				fontAsset2.m_SourceFontFilePath = fontFilePath;
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
			bool flag = FontEngine.LoadFontFace(font, samplingPointSize, faceIndex) > FontEngineError.Success;
			FontAsset fontAsset;
			if (flag)
			{
				bool flag2 = font.name == "LegacyRuntime";
				if (flag2)
				{
					fontAsset = FontAsset.CreateFontAsset("Arial", "Regular", 90);
				}
				else
				{
					Debug.LogWarning("Unable to load font face for [" + font.name + "]. Make sure \"Include Font Data\" is enabled in the Font Import Settings.", font);
					fontAsset = null;
				}
			}
			else
			{
				fontAsset = FontAsset.CreateFontAssetInstance(font, atlasPadding, renderMode, atlasWidth, atlasHeight, atlasPopulationMode, enableMultiAtlasSupport);
			}
			return fontAsset;
		}

		private static FontAsset CreateFontAssetInstance(Font font, int atlasPadding, GlyphRenderMode renderMode, int atlasWidth, int atlasHeight, AtlasPopulationMode atlasPopulationMode, bool enableMultiAtlasSupport)
		{
			FontAsset fontAsset = ScriptableObject.CreateInstance<FontAsset>();
			fontAsset.m_Version = "1.1.0";
			fontAsset.faceInfo = FontEngine.GetFaceInfo();
			bool flag = atlasPopulationMode == AtlasPopulationMode.Dynamic && font != null;
			if (flag)
			{
				fontAsset.sourceFontFile = font;
			}
			fontAsset.atlasPopulationMode = atlasPopulationMode;
			fontAsset.atlasWidth = atlasWidth;
			fontAsset.atlasHeight = atlasHeight;
			fontAsset.atlasPadding = atlasPadding;
			fontAsset.atlasRenderMode = renderMode;
			fontAsset.atlasTextures = new Texture2D[1];
			Texture2D texture2D = new Texture2D(1, 1, TextureFormat.Alpha8, false);
			fontAsset.atlasTextures[0] = texture2D;
			fontAsset.isMultiAtlasTexturesEnabled = enableMultiAtlasSupport;
			bool flag2 = (renderMode & (GlyphRenderMode)16) == (GlyphRenderMode)16;
			int num;
			if (flag2)
			{
				num = 0;
				Material material = new Material(TextShaderUtilities.ShaderRef_MobileBitmap);
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

		private void Awake()
		{
		}

		private void OnDestroy()
		{
			this.DestroyAtlasTextures();
			Object.DestroyImmediate(this.m_Material);
		}

		public void ReadFontAssetDefinition()
		{
			this.InitializeDictionaryLookupTables();
			this.AddSynthesizedCharactersAndFaceMetrics();
			bool flag = this.m_FaceInfo.capLine == 0f && this.m_CharacterLookupDictionary.ContainsKey(88U);
			if (flag)
			{
				uint glyphIndex = this.m_CharacterLookupDictionary[88U].glyphIndex;
				this.m_FaceInfo.capLine = this.m_GlyphLookupDictionary[glyphIndex].metrics.horizontalBearingY;
			}
			bool flag2 = this.m_FaceInfo.meanLine == 0f && this.m_CharacterLookupDictionary.ContainsKey(120U);
			if (flag2)
			{
				uint glyphIndex2 = this.m_CharacterLookupDictionary[120U].glyphIndex;
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
			base.hashCode = TextUtilities.GetHashCodeCaseInSensitive(base.name);
			this.familyNameHashCode = TextUtilities.GetHashCodeCaseInSensitive(this.m_FaceInfo.familyName);
			this.styleNameHashCode = TextUtilities.GetHashCodeCaseInSensitive(this.m_FaceInfo.styleName);
			base.materialHashCode = TextUtilities.GetHashCodeCaseInSensitive(base.name + FontAsset.s_DefaultMaterialSuffix);
			TextResourceManager.AddFontAsset(this);
			this.IsFontAssetLookupTablesDirty = false;
		}

		internal void InitializeDictionaryLookupTables()
		{
			this.InitializeGlyphLookupDictionary();
			this.InitializeCharacterLookupDictionary();
			this.InitializeLigatureSubstitutionLookupDictionary();
			this.InitializeGlyphPaidAdjustmentRecordsLookupDictionary();
			this.InitializeMarkToBaseAdjustmentRecordsLookupDictionary();
			this.InitializeMarkToMarkAdjustmentRecordsLookupDictionary();
		}

		internal void InitializeGlyphLookupDictionary()
		{
			bool flag = this.m_GlyphLookupDictionary == null;
			if (flag)
			{
				this.m_GlyphLookupDictionary = new Dictionary<uint, Glyph>();
			}
			else
			{
				this.m_GlyphLookupDictionary.Clear();
			}
			bool flag2 = this.m_GlyphIndexList == null;
			if (flag2)
			{
				this.m_GlyphIndexList = new List<uint>();
			}
			else
			{
				this.m_GlyphIndexList.Clear();
			}
			bool flag3 = this.m_GlyphIndexListNewlyAdded == null;
			if (flag3)
			{
				this.m_GlyphIndexListNewlyAdded = new List<uint>();
			}
			else
			{
				this.m_GlyphIndexListNewlyAdded.Clear();
			}
			int count = this.m_GlyphTable.Count;
			for (int i = 0; i < count; i++)
			{
				Glyph glyph = this.m_GlyphTable[i];
				uint index = glyph.index;
				bool flag4 = !this.m_GlyphLookupDictionary.ContainsKey(index);
				if (flag4)
				{
					this.m_GlyphLookupDictionary.Add(index, glyph);
					this.m_GlyphIndexList.Add(index);
				}
			}
		}

		internal void InitializeCharacterLookupDictionary()
		{
			bool flag = this.m_CharacterLookupDictionary == null;
			if (flag)
			{
				this.m_CharacterLookupDictionary = new Dictionary<uint, Character>();
			}
			else
			{
				this.m_CharacterLookupDictionary.Clear();
			}
			for (int i = 0; i < this.m_CharacterTable.Count; i++)
			{
				Character character = this.m_CharacterTable[i];
				uint unicode = character.unicode;
				uint glyphIndex = character.glyphIndex;
				bool flag2 = !this.m_CharacterLookupDictionary.ContainsKey(unicode);
				if (flag2)
				{
					this.m_CharacterLookupDictionary.Add(unicode, character);
					character.textAsset = this;
					character.glyph = this.m_GlyphLookupDictionary[glyphIndex];
				}
			}
		}

		internal void InitializeLigatureSubstitutionLookupDictionary()
		{
			bool flag = this.m_FontFeatureTable.m_LigatureSubstitutionRecordLookup == null;
			if (flag)
			{
				this.m_FontFeatureTable.m_LigatureSubstitutionRecordLookup = new Dictionary<uint, List<LigatureSubstitutionRecord>>();
			}
			else
			{
				this.m_FontFeatureTable.m_LigatureSubstitutionRecordLookup.Clear();
			}
			List<LigatureSubstitutionRecord> ligatureSubstitutionRecords = this.m_FontFeatureTable.m_LigatureSubstitutionRecords;
			bool flag2 = ligatureSubstitutionRecords != null;
			if (flag2)
			{
				for (int i = 0; i < ligatureSubstitutionRecords.Count; i++)
				{
					LigatureSubstitutionRecord ligatureSubstitutionRecord = ligatureSubstitutionRecords[i];
					bool flag3 = ligatureSubstitutionRecord.componentGlyphIDs == null || ligatureSubstitutionRecord.componentGlyphIDs.Length == 0;
					if (!flag3)
					{
						uint num = ligatureSubstitutionRecord.componentGlyphIDs[0];
						bool flag4 = !this.m_FontFeatureTable.m_LigatureSubstitutionRecordLookup.ContainsKey(num);
						if (flag4)
						{
							this.m_FontFeatureTable.m_LigatureSubstitutionRecordLookup.Add(num, new List<LigatureSubstitutionRecord> { ligatureSubstitutionRecord });
						}
						else
						{
							this.m_FontFeatureTable.m_LigatureSubstitutionRecordLookup[num].Add(ligatureSubstitutionRecord);
						}
					}
				}
			}
		}

		internal void InitializeGlyphPaidAdjustmentRecordsLookupDictionary()
		{
			bool flag = this.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup == null;
			if (flag)
			{
				this.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup = new Dictionary<uint, GlyphPairAdjustmentRecord>();
			}
			else
			{
				this.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup.Clear();
			}
			List<GlyphPairAdjustmentRecord> glyphPairAdjustmentRecords = this.m_FontFeatureTable.m_GlyphPairAdjustmentRecords;
			bool flag2 = glyphPairAdjustmentRecords != null;
			if (flag2)
			{
				for (int i = 0; i < glyphPairAdjustmentRecords.Count; i++)
				{
					GlyphPairAdjustmentRecord glyphPairAdjustmentRecord = glyphPairAdjustmentRecords[i];
					uint num = (glyphPairAdjustmentRecord.secondAdjustmentRecord.glyphIndex << 16) | glyphPairAdjustmentRecord.firstAdjustmentRecord.glyphIndex;
					bool flag3 = !this.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup.ContainsKey(num);
					if (flag3)
					{
						this.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup.Add(num, glyphPairAdjustmentRecord);
					}
				}
			}
		}

		internal void InitializeMarkToBaseAdjustmentRecordsLookupDictionary()
		{
			bool flag = this.m_FontFeatureTable.m_MarkToBaseAdjustmentRecordLookup == null;
			if (flag)
			{
				this.m_FontFeatureTable.m_MarkToBaseAdjustmentRecordLookup = new Dictionary<uint, MarkToBaseAdjustmentRecord>();
			}
			else
			{
				this.m_FontFeatureTable.m_MarkToBaseAdjustmentRecordLookup.Clear();
			}
			List<MarkToBaseAdjustmentRecord> markToBaseAdjustmentRecords = this.m_FontFeatureTable.m_MarkToBaseAdjustmentRecords;
			bool flag2 = markToBaseAdjustmentRecords != null;
			if (flag2)
			{
				for (int i = 0; i < markToBaseAdjustmentRecords.Count; i++)
				{
					MarkToBaseAdjustmentRecord markToBaseAdjustmentRecord = markToBaseAdjustmentRecords[i];
					uint num = (markToBaseAdjustmentRecord.markGlyphID << 16) | markToBaseAdjustmentRecord.baseGlyphID;
					bool flag3 = !this.m_FontFeatureTable.m_MarkToBaseAdjustmentRecordLookup.ContainsKey(num);
					if (flag3)
					{
						this.m_FontFeatureTable.m_MarkToBaseAdjustmentRecordLookup.Add(num, markToBaseAdjustmentRecord);
					}
				}
			}
		}

		internal void InitializeMarkToMarkAdjustmentRecordsLookupDictionary()
		{
			bool flag = this.m_FontFeatureTable.m_MarkToMarkAdjustmentRecordLookup == null;
			if (flag)
			{
				this.m_FontFeatureTable.m_MarkToMarkAdjustmentRecordLookup = new Dictionary<uint, MarkToMarkAdjustmentRecord>();
			}
			else
			{
				this.m_FontFeatureTable.m_MarkToMarkAdjustmentRecordLookup.Clear();
			}
			List<MarkToMarkAdjustmentRecord> markToMarkAdjustmentRecords = this.m_FontFeatureTable.m_MarkToMarkAdjustmentRecords;
			bool flag2 = markToMarkAdjustmentRecords != null;
			if (flag2)
			{
				for (int i = 0; i < markToMarkAdjustmentRecords.Count; i++)
				{
					MarkToMarkAdjustmentRecord markToMarkAdjustmentRecord = markToMarkAdjustmentRecords[i];
					uint num = (markToMarkAdjustmentRecord.combiningMarkGlyphID << 16) | markToMarkAdjustmentRecord.baseMarkGlyphID;
					bool flag3 = !this.m_FontFeatureTable.m_MarkToMarkAdjustmentRecordLookup.ContainsKey(num);
					if (flag3)
					{
						this.m_FontFeatureTable.m_MarkToMarkAdjustmentRecordLookup.Add(num, markToMarkAdjustmentRecord);
					}
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
							this.m_CharacterLookupDictionary.Add(unicode, new Character(unicode, this, glyph));
						}
						return;
					}
				}
				glyph = new Glyph(0U, new GlyphMetrics(0f, 0f, 0f, 0f, 0f), GlyphRect.zero, 1f, 0);
				this.m_CharacterLookupDictionary.Add(unicode, new Character(unicode, this, glyph));
			}
		}

		internal void AddCharacterToLookupCache(uint unicode, Character character)
		{
			this.m_CharacterLookupDictionary.Add(unicode, character);
		}

		private FontEngineError LoadFontFace()
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
			bool flag = this.m_CharacterLookupDictionary == null;
			return !flag && this.m_CharacterLookupDictionary.ContainsKey((uint)character);
		}

		public bool HasCharacter(char character, bool searchFallbacks = false, bool tryAddCharacter = false)
		{
			return this.HasCharacter((uint)character, searchFallbacks, tryAddCharacter);
		}

		public bool HasCharacter(uint character, bool searchFallbacks = false, bool tryAddCharacter = false)
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
			bool flag3 = this.m_CharacterLookupDictionary.ContainsKey(character);
			bool flag4;
			if (flag3)
			{
				flag4 = true;
			}
			else
			{
				bool flag5 = tryAddCharacter && (this.m_AtlasPopulationMode == AtlasPopulationMode.Dynamic || this.m_AtlasPopulationMode == AtlasPopulationMode.DynamicOS);
				if (flag5)
				{
					Character character2;
					bool flag6 = this.TryAddCharacterInternal(character, out character2, false);
					if (flag6)
					{
						return true;
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
								bool flag10 = fontAsset.HasCharacter_Internal(character, true, tryAddCharacter);
								if (flag10)
								{
									return true;
								}
							}
							num++;
						}
					}
				}
				flag4 = false;
			}
			return flag4;
		}

		private bool HasCharacter_Internal(uint character, bool searchFallbacks = false, bool tryAddCharacter = false)
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
			bool flag3 = this.m_CharacterLookupDictionary.ContainsKey(character);
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
					bool flag6 = this.TryAddCharacterInternal(character, out character2, false);
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
							bool flag9 = fontAsset.HasCharacter_Internal(character, true, tryAddCharacter);
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
			bool flag = this.m_CharacterLookupDictionary == null;
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
					bool flag3 = !this.m_CharacterLookupDictionary.ContainsKey((uint)text[i]);
					if (flag3)
					{
						missingCharacters.Add(text[i]);
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
			this.s_MissingCharacterList.Clear();
			for (int i = 0; i < text.Length; i++)
			{
				bool flag3 = true;
				uint num = (uint)text[i];
				bool flag4 = this.m_CharacterLookupDictionary.ContainsKey(num);
				if (!flag4)
				{
					bool flag5 = tryAddCharacter && (this.atlasPopulationMode == AtlasPopulationMode.Dynamic || this.m_AtlasPopulationMode == AtlasPopulationMode.DynamicOS);
					if (flag5)
					{
						Character character;
						bool flag6 = this.TryAddCharacterInternal(num, out character, false);
						if (flag6)
						{
							goto IL_019C;
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
							int num2 = 0;
							while (num2 < this.fallbackFontAssetTable.Count && this.fallbackFontAssetTable[num2] != null)
							{
								FontAsset fontAsset = this.fallbackFontAssetTable[num2];
								int instanceID = fontAsset.GetInstanceID();
								bool flag9 = FontAsset.k_SearchedFontAssetLookup.Add(instanceID);
								if (flag9)
								{
									bool flag10 = !fontAsset.HasCharacter_Internal(num, true, tryAddCharacter);
									if (!flag10)
									{
										flag3 = false;
										break;
									}
								}
								num2++;
							}
						}
					}
					bool flag11 = flag3;
					if (flag11)
					{
						this.s_MissingCharacterList.Add(num);
					}
				}
				IL_019C:;
			}
			bool flag12 = this.s_MissingCharacterList.Count > 0;
			bool flag13;
			if (flag12)
			{
				missingCharacters = this.s_MissingCharacterList.ToArray();
				flag13 = false;
			}
			else
			{
				flag13 = true;
			}
			return flag13;
		}

		public bool HasCharacters(string text)
		{
			bool flag = this.m_CharacterLookupDictionary == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				for (int i = 0; i < text.Length; i++)
				{
					bool flag3 = !this.m_CharacterLookupDictionary.ContainsKey((uint)text[i]);
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
			bool flag = this.m_CharacterLookupDictionary.ContainsKey(unicode);
			uint num;
			if (flag)
			{
				num = this.m_CharacterLookupDictionary[unicode].glyphIndex;
			}
			else
			{
				num = ((this.LoadFontFace() == FontEngineError.Success) ? FontEngine.GetGlyphIndex(unicode) : 0U);
			}
			return num;
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

		internal static void UpdateFontFeaturesForFontAssetsInQueue()
		{
			int count = FontAsset.k_FontAssets_FontFeaturesUpdateQueue.Count;
			for (int i = 0; i < count; i++)
			{
				FontAsset.k_FontAssets_FontFeaturesUpdateQueue[i].UpdateAllFontFeatures();
			}
			bool flag = count > 0;
			if (flag)
			{
				FontAsset.k_FontAssets_FontFeaturesUpdateQueue.Clear();
				FontAsset.k_FontAssets_FontFeaturesUpdateQueueLookup.Clear();
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
			bool flag = unicodes == null || unicodes.Length == 0 || this.m_AtlasPopulationMode == AtlasPopulationMode.Static;
			bool flag3;
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
					missingUnicodes = unicodes.ToArray<uint>();
					flag3 = false;
				}
				else
				{
					bool flag5 = this.m_CharacterLookupDictionary == null || this.m_GlyphLookupDictionary == null;
					if (flag5)
					{
						this.ReadFontAssetDefinition();
					}
					this.m_GlyphsToAdd.Clear();
					this.m_GlyphsToAddLookup.Clear();
					this.m_CharactersToAdd.Clear();
					this.m_CharactersToAddLookup.Clear();
					this.s_MissingCharacterList.Clear();
					bool flag6 = false;
					int num = unicodes.Length;
					for (int i = 0; i < num; i++)
					{
						uint num2 = unicodes[i];
						bool flag7 = this.m_CharacterLookupDictionary.ContainsKey(num2);
						if (!flag7)
						{
							uint num3 = FontEngine.GetGlyphIndex(num2);
							bool flag8 = num3 == 0U;
							if (flag8)
							{
								uint num4 = num2;
								uint num5 = num4;
								if (num5 != 160U)
								{
									if (num5 == 173U || num5 == 8209U)
									{
										num3 = FontEngine.GetGlyphIndex(45U);
									}
								}
								else
								{
									num3 = FontEngine.GetGlyphIndex(32U);
								}
								bool flag9 = num3 == 0U;
								if (flag9)
								{
									this.s_MissingCharacterList.Add(num2);
									flag6 = true;
									goto IL_022F;
								}
							}
							Character character = new Character(num2, num3);
							bool flag10 = this.m_GlyphLookupDictionary.ContainsKey(num3);
							if (flag10)
							{
								character.glyph = this.m_GlyphLookupDictionary[num3];
								character.textAsset = this;
								this.m_CharacterTable.Add(character);
								this.m_CharacterLookupDictionary.Add(num2, character);
							}
							else
							{
								bool flag11 = this.m_GlyphsToAddLookup.Add(num3);
								if (flag11)
								{
									this.m_GlyphsToAdd.Add(num3);
								}
								bool flag12 = this.m_CharactersToAddLookup.Add(num2);
								if (flag12)
								{
									this.m_CharactersToAdd.Add(character);
								}
							}
						}
						IL_022F:;
					}
					bool flag13 = this.m_GlyphsToAdd.Count == 0;
					if (flag13)
					{
						missingUnicodes = unicodes;
						flag3 = false;
					}
					else
					{
						bool flag14 = this.m_AtlasTextures[this.m_AtlasTextureIndex].width != this.m_AtlasWidth || this.m_AtlasTextures[this.m_AtlasTextureIndex].height != this.m_AtlasHeight;
						if (flag14)
						{
							this.m_AtlasTextures[this.m_AtlasTextureIndex].Reinitialize(this.m_AtlasWidth, this.m_AtlasHeight);
							FontEngine.ResetAtlasTexture(this.m_AtlasTextures[this.m_AtlasTextureIndex]);
						}
						Glyph[] array;
						bool flag15 = FontEngine.TryAddGlyphsToTexture(this.m_GlyphsToAdd, this.m_AtlasPadding, GlyphPackingMode.BestShortSideFit, this.m_FreeGlyphRects, this.m_UsedGlyphRects, this.m_AtlasRenderMode, this.m_AtlasTextures[this.m_AtlasTextureIndex], out array);
						int num6 = 0;
						while (num6 < array.Length && array[num6] != null)
						{
							Glyph glyph = array[num6];
							uint index = glyph.index;
							glyph.atlasIndex = this.m_AtlasTextureIndex;
							this.m_GlyphTable.Add(glyph);
							this.m_GlyphLookupDictionary.Add(index, glyph);
							this.m_GlyphIndexListNewlyAdded.Add(index);
							this.m_GlyphIndexList.Add(index);
							num6++;
						}
						this.m_GlyphsToAdd.Clear();
						for (int j = 0; j < this.m_CharactersToAdd.Count; j++)
						{
							Character character2 = this.m_CharactersToAdd[j];
							Glyph glyph2;
							bool flag16 = !this.m_GlyphLookupDictionary.TryGetValue(character2.glyphIndex, out glyph2);
							if (flag16)
							{
								this.m_GlyphsToAdd.Add(character2.glyphIndex);
							}
							else
							{
								character2.glyph = glyph2;
								character2.textAsset = this;
								this.m_CharacterTable.Add(character2);
								this.m_CharacterLookupDictionary.Add(character2.unicode, character2);
								this.m_CharactersToAdd.RemoveAt(j);
								j--;
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
						if (includeFontFeatures)
						{
							this.UpdateAllFontFeatures();
						}
						for (int k = 0; k < this.m_CharactersToAdd.Count; k++)
						{
							Character character3 = this.m_CharactersToAdd[k];
							this.s_MissingCharacterList.Add(character3.unicode);
						}
						missingUnicodes = null;
						bool flag18 = this.s_MissingCharacterList.Count > 0;
						if (flag18)
						{
							missingUnicodes = this.s_MissingCharacterList.ToArray();
						}
						flag3 = flag15 && !flag6;
					}
				}
			}
			return flag3;
		}

		public bool TryAddCharacters(string characters, bool includeFontFeatures = false)
		{
			string text;
			return this.TryAddCharacters(characters, out text, includeFontFeatures);
		}

		public bool TryAddCharacters(string characters, out string missingCharacters, bool includeFontFeatures = false)
		{
			bool flag = string.IsNullOrEmpty(characters) || this.m_AtlasPopulationMode == AtlasPopulationMode.Static;
			bool flag3;
			if (flag)
			{
				bool flag2 = this.m_AtlasPopulationMode == AtlasPopulationMode.Static;
				if (flag2)
				{
					Debug.LogWarning("Unable to add characters to font asset [" + base.name + "] because its AtlasPopulationMode is set to Static.", this);
				}
				else
				{
					Debug.LogWarning("Unable to add characters to font asset [" + base.name + "] because the provided character list is Null or Empty.", this);
				}
				missingCharacters = characters;
				flag3 = false;
			}
			else
			{
				bool flag4 = this.LoadFontFace() > FontEngineError.Success;
				if (flag4)
				{
					missingCharacters = characters;
					flag3 = false;
				}
				else
				{
					bool flag5 = this.m_CharacterLookupDictionary == null || this.m_GlyphLookupDictionary == null;
					if (flag5)
					{
						this.ReadFontAssetDefinition();
					}
					this.m_GlyphsToAdd.Clear();
					this.m_GlyphsToAddLookup.Clear();
					this.m_CharactersToAdd.Clear();
					this.m_CharactersToAddLookup.Clear();
					this.s_MissingCharacterList.Clear();
					bool flag6 = false;
					int length = characters.Length;
					for (int i = 0; i < length; i++)
					{
						uint num = (uint)characters[i];
						bool flag7 = this.m_CharacterLookupDictionary.ContainsKey(num);
						if (!flag7)
						{
							uint num2 = FontEngine.GetGlyphIndex(num);
							bool flag8 = num2 == 0U;
							if (flag8)
							{
								uint num3 = num;
								uint num4 = num3;
								if (num4 != 160U)
								{
									if (num4 == 173U || num4 == 8209U)
									{
										num2 = FontEngine.GetGlyphIndex(45U);
									}
								}
								else
								{
									num2 = FontEngine.GetGlyphIndex(32U);
								}
								bool flag9 = num2 == 0U;
								if (flag9)
								{
									this.s_MissingCharacterList.Add(num);
									flag6 = true;
									goto IL_0234;
								}
							}
							Character character = new Character(num, num2);
							bool flag10 = this.m_GlyphLookupDictionary.ContainsKey(num2);
							if (flag10)
							{
								character.glyph = this.m_GlyphLookupDictionary[num2];
								character.textAsset = this;
								this.m_CharacterTable.Add(character);
								this.m_CharacterLookupDictionary.Add(num, character);
							}
							else
							{
								bool flag11 = this.m_GlyphsToAddLookup.Add(num2);
								if (flag11)
								{
									this.m_GlyphsToAdd.Add(num2);
								}
								bool flag12 = this.m_CharactersToAddLookup.Add(num);
								if (flag12)
								{
									this.m_CharactersToAdd.Add(character);
								}
							}
						}
						IL_0234:;
					}
					bool flag13 = this.m_GlyphsToAdd.Count == 0;
					if (flag13)
					{
						missingCharacters = characters;
						flag3 = false;
					}
					else
					{
						bool flag14 = this.m_AtlasTextures[this.m_AtlasTextureIndex].width != this.m_AtlasWidth || this.m_AtlasTextures[this.m_AtlasTextureIndex].height != this.m_AtlasHeight;
						if (flag14)
						{
							this.m_AtlasTextures[this.m_AtlasTextureIndex].Reinitialize(this.m_AtlasWidth, this.m_AtlasHeight);
							FontEngine.ResetAtlasTexture(this.m_AtlasTextures[this.m_AtlasTextureIndex]);
						}
						Glyph[] array;
						bool flag15 = FontEngine.TryAddGlyphsToTexture(this.m_GlyphsToAdd, this.m_AtlasPadding, GlyphPackingMode.BestShortSideFit, this.m_FreeGlyphRects, this.m_UsedGlyphRects, this.m_AtlasRenderMode, this.m_AtlasTextures[this.m_AtlasTextureIndex], out array);
						int num5 = 0;
						while (num5 < array.Length && array[num5] != null)
						{
							Glyph glyph = array[num5];
							uint index = glyph.index;
							glyph.atlasIndex = this.m_AtlasTextureIndex;
							this.m_GlyphTable.Add(glyph);
							this.m_GlyphLookupDictionary.Add(index, glyph);
							this.m_GlyphIndexListNewlyAdded.Add(index);
							this.m_GlyphIndexList.Add(index);
							num5++;
						}
						this.m_GlyphsToAdd.Clear();
						for (int j = 0; j < this.m_CharactersToAdd.Count; j++)
						{
							Character character2 = this.m_CharactersToAdd[j];
							Glyph glyph2;
							bool flag16 = !this.m_GlyphLookupDictionary.TryGetValue(character2.glyphIndex, out glyph2);
							if (flag16)
							{
								this.m_GlyphsToAdd.Add(character2.glyphIndex);
							}
							else
							{
								character2.glyph = glyph2;
								character2.textAsset = this;
								this.m_CharacterTable.Add(character2);
								this.m_CharacterLookupDictionary.Add(character2.unicode, character2);
								this.m_CharactersToAdd.RemoveAt(j);
								j--;
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
						if (includeFontFeatures)
						{
							this.UpdateAllFontFeatures();
						}
						missingCharacters = string.Empty;
						for (int k = 0; k < this.m_CharactersToAdd.Count; k++)
						{
							Character character3 = this.m_CharactersToAdd[k];
							this.s_MissingCharacterList.Add(character3.unicode);
						}
						bool flag18 = this.s_MissingCharacterList.Count > 0;
						if (flag18)
						{
							missingCharacters = this.s_MissingCharacterList.UintToString();
						}
						flag3 = flag15 && !flag6;
					}
				}
			}
			return flag3;
		}

		internal bool TryAddGlyphInternal(uint glyphIndex, out Glyph glyph)
		{
			using (FontAsset.k_TryAddGlyphMarker.Auto())
			{
				glyph = null;
				bool flag = this.m_GlyphLookupDictionary.ContainsKey(glyphIndex);
				if (flag)
				{
					glyph = this.m_GlyphLookupDictionary[glyphIndex];
					return true;
				}
				bool flag2 = this.LoadFontFace() > FontEngineError.Success;
				if (flag2)
				{
					return false;
				}
				bool flag3 = !this.m_AtlasTextures[this.m_AtlasTextureIndex].isReadable;
				if (flag3)
				{
					Debug.LogWarning(string.Concat(new string[]
					{
						"Unable to add the requested glyph to font asset [",
						base.name,
						"]'s atlas texture. Please make the texture [",
						this.m_AtlasTextures[this.m_AtlasTextureIndex].name,
						"] readable."
					}), this.m_AtlasTextures[this.m_AtlasTextureIndex]);
					return false;
				}
				bool flag4 = this.m_AtlasTextures[this.m_AtlasTextureIndex].width != this.m_AtlasWidth || this.m_AtlasTextures[this.m_AtlasTextureIndex].height != this.m_AtlasHeight;
				if (flag4)
				{
					this.m_AtlasTextures[this.m_AtlasTextureIndex].Reinitialize(this.m_AtlasWidth, this.m_AtlasHeight);
					FontEngine.ResetAtlasTexture(this.m_AtlasTextures[this.m_AtlasTextureIndex]);
				}
				FontEngine.SetTextureUploadMode(false);
				bool flag5 = FontEngine.TryAddGlyphToTexture(glyphIndex, this.m_AtlasPadding, GlyphPackingMode.BestShortSideFit, this.m_FreeGlyphRects, this.m_UsedGlyphRects, this.m_AtlasRenderMode, this.m_AtlasTextures[this.m_AtlasTextureIndex], out glyph);
				if (flag5)
				{
					glyph.atlasIndex = this.m_AtlasTextureIndex;
					this.m_GlyphTable.Add(glyph);
					this.m_GlyphLookupDictionary.Add(glyphIndex, glyph);
					this.m_GlyphIndexList.Add(glyphIndex);
					this.m_GlyphIndexListNewlyAdded.Add(glyphIndex);
					return true;
				}
				bool isMultiAtlasTexturesEnabled = this.m_IsMultiAtlasTexturesEnabled;
				if (isMultiAtlasTexturesEnabled)
				{
					this.SetupNewAtlasTexture();
					bool flag6 = FontEngine.TryAddGlyphToTexture(glyphIndex, this.m_AtlasPadding, GlyphPackingMode.BestShortSideFit, this.m_FreeGlyphRects, this.m_UsedGlyphRects, this.m_AtlasRenderMode, this.m_AtlasTextures[this.m_AtlasTextureIndex], out glyph);
					if (flag6)
					{
						glyph.atlasIndex = this.m_AtlasTextureIndex;
						this.m_GlyphTable.Add(glyph);
						this.m_GlyphLookupDictionary.Add(glyphIndex, glyph);
						this.m_GlyphIndexList.Add(glyphIndex);
						this.m_GlyphIndexListNewlyAdded.Add(glyphIndex);
						return true;
					}
				}
			}
			return false;
		}

		internal bool TryAddCharacterInternal(uint unicode, out Character character, bool shouldGetFontFeatures = false)
		{
			character = null;
			bool flag = this.m_MissingUnicodesFromFontFile.Contains(unicode);
			bool flag2;
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
					bool flag6 = this.m_GlyphLookupDictionary.ContainsKey(num);
					if (flag6)
					{
						character = new Character(unicode, this, this.m_GlyphLookupDictionary[num]);
						this.m_CharacterTable.Add(character);
						this.m_CharacterLookupDictionary.Add(unicode, character);
						flag2 = true;
					}
					else
					{
						Glyph glyph = null;
						bool flag7 = !this.m_AtlasTextures[this.m_AtlasTextureIndex].isReadable;
						if (flag7)
						{
							Debug.LogWarning(string.Concat(new string[]
							{
								"Unable to add the requested character to font asset [",
								base.name,
								"]'s atlas texture. Please make the texture [",
								this.m_AtlasTextures[this.m_AtlasTextureIndex].name,
								"] readable."
							}), this.m_AtlasTextures[this.m_AtlasTextureIndex]);
							flag2 = false;
						}
						else
						{
							bool flag8 = this.m_AtlasTextures[this.m_AtlasTextureIndex].width != this.m_AtlasWidth || this.m_AtlasTextures[this.m_AtlasTextureIndex].height != this.m_AtlasHeight;
							if (flag8)
							{
								this.m_AtlasTextures[this.m_AtlasTextureIndex].Reinitialize(this.m_AtlasWidth, this.m_AtlasHeight);
								FontEngine.ResetAtlasTexture(this.m_AtlasTextures[this.m_AtlasTextureIndex]);
							}
							FontEngine.SetTextureUploadMode(false);
							bool flag9 = FontEngine.TryAddGlyphToTexture(num, this.m_AtlasPadding, GlyphPackingMode.BestShortSideFit, this.m_FreeGlyphRects, this.m_UsedGlyphRects, this.m_AtlasRenderMode, this.m_AtlasTextures[this.m_AtlasTextureIndex], out glyph);
							if (flag9)
							{
								glyph.atlasIndex = this.m_AtlasTextureIndex;
								this.m_GlyphTable.Add(glyph);
								this.m_GlyphLookupDictionary.Add(num, glyph);
								character = new Character(unicode, this, glyph);
								this.m_CharacterTable.Add(character);
								this.m_CharacterLookupDictionary.Add(unicode, character);
								this.m_GlyphIndexList.Add(num);
								this.m_GlyphIndexListNewlyAdded.Add(num);
								if (shouldGetFontFeatures)
								{
									FontAsset.RegisterFontAssetForFontFeatureUpdate(this);
								}
								FontAsset.RegisterAtlasTextureForApply(this.m_AtlasTextures[this.m_AtlasTextureIndex]);
								FontEngine.SetTextureUploadMode(true);
								flag2 = true;
							}
							else
							{
								bool isMultiAtlasTexturesEnabled = this.m_IsMultiAtlasTexturesEnabled;
								if (isMultiAtlasTexturesEnabled)
								{
									this.SetupNewAtlasTexture();
									bool flag10 = FontEngine.TryAddGlyphToTexture(num, this.m_AtlasPadding, GlyphPackingMode.BestShortSideFit, this.m_FreeGlyphRects, this.m_UsedGlyphRects, this.m_AtlasRenderMode, this.m_AtlasTextures[this.m_AtlasTextureIndex], out glyph);
									if (flag10)
									{
										glyph.atlasIndex = this.m_AtlasTextureIndex;
										this.m_GlyphTable.Add(glyph);
										this.m_GlyphLookupDictionary.Add(num, glyph);
										character = new Character(unicode, this, glyph);
										this.m_CharacterTable.Add(character);
										this.m_CharacterLookupDictionary.Add(unicode, character);
										this.m_GlyphIndexList.Add(num);
										this.m_GlyphIndexListNewlyAdded.Add(num);
										if (shouldGetFontFeatures)
										{
											FontAsset.RegisterFontAssetForFontFeatureUpdate(this);
										}
										FontAsset.RegisterAtlasTextureForApply(this.m_AtlasTextures[this.m_AtlasTextureIndex]);
										FontEngine.SetTextureUploadMode(true);
										return true;
									}
								}
								flag2 = false;
							}
						}
					}
				}
			}
			return flag2;
		}

		internal bool TryGetCharacter_and_QueueRenderToTexture(uint unicode, out Character character, bool shouldGetFontFeatures = false)
		{
			character = null;
			bool flag = this.m_MissingUnicodesFromFontFile.Contains(unicode);
			bool flag2;
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
					bool flag6 = this.m_GlyphLookupDictionary.ContainsKey(num);
					if (flag6)
					{
						character = new Character(unicode, this, this.m_GlyphLookupDictionary[num]);
						this.m_CharacterTable.Add(character);
						this.m_CharacterLookupDictionary.Add(unicode, character);
						flag2 = true;
					}
					else
					{
						GlyphLoadFlags glyphLoadFlags = ((((GlyphRenderMode)4 & this.m_AtlasRenderMode) == (GlyphRenderMode)4) ? (GlyphLoadFlags.LOAD_NO_HINTING | GlyphLoadFlags.LOAD_NO_BITMAP) : GlyphLoadFlags.LOAD_NO_BITMAP);
						Glyph glyph = null;
						bool flag7 = FontEngine.TryGetGlyphWithIndexValue(num, glyphLoadFlags, out glyph);
						if (flag7)
						{
							this.m_GlyphTable.Add(glyph);
							this.m_GlyphLookupDictionary.Add(num, glyph);
							character = new Character(unicode, this, glyph);
							this.m_CharacterTable.Add(character);
							this.m_CharacterLookupDictionary.Add(unicode, character);
							this.m_GlyphIndexList.Add(num);
							this.m_GlyphIndexListNewlyAdded.Add(num);
							if (shouldGetFontFeatures)
							{
								FontAsset.RegisterFontAssetForFontFeatureUpdate(this);
							}
							this.m_GlyphsToRender.Add(glyph);
							flag2 = true;
						}
						else
						{
							flag2 = false;
						}
					}
				}
			}
			return flag2;
		}

		internal void TryAddGlyphsToAtlasTextures()
		{
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
			this.m_AtlasTextures[this.m_AtlasTextureIndex] = new Texture2D(this.m_AtlasWidth, this.m_AtlasHeight, TextureFormat.Alpha8, false);
			FontEngine.ResetAtlasTexture(this.m_AtlasTextures[this.m_AtlasTextureIndex]);
			int num = (((this.m_AtlasRenderMode & (GlyphRenderMode)16) == (GlyphRenderMode)16) ? 0 : 1);
			this.m_FreeGlyphRects.Clear();
			this.m_FreeGlyphRects.Add(new GlyphRect(0, 0, this.m_AtlasWidth - num, this.m_AtlasHeight - num));
			this.m_UsedGlyphRects.Clear();
		}

		private void UpdateAllFontFeatures()
		{
			this.UpdateGlyphAdjustmentRecords();
			this.m_GlyphIndexListNewlyAdded.Clear();
		}

		internal void UpdateGlyphAdjustmentRecords()
		{
			using (FontAsset.k_UpdateGlyphAdjustmentRecordsMarker.Auto())
			{
				int num;
				GlyphPairAdjustmentRecord[] glyphPairAdjustmentRecords = FontEngine.GetGlyphPairAdjustmentRecords(this.m_GlyphIndexList, out num);
				this.m_GlyphIndexListNewlyAdded.Clear();
				bool flag = glyphPairAdjustmentRecords == null || glyphPairAdjustmentRecords.Length == 0;
				if (!flag)
				{
					bool flag2 = this.m_FontFeatureTable == null;
					if (flag2)
					{
						this.m_FontFeatureTable = new FontFeatureTable();
					}
					int num2 = 0;
					while (num2 < glyphPairAdjustmentRecords.Length && glyphPairAdjustmentRecords[num2].firstAdjustmentRecord.glyphIndex > 0U)
					{
						uint num3 = (glyphPairAdjustmentRecords[num2].secondAdjustmentRecord.glyphIndex << 16) | glyphPairAdjustmentRecords[num2].firstAdjustmentRecord.glyphIndex;
						bool flag3 = this.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup.ContainsKey(num3);
						if (!flag3)
						{
							GlyphPairAdjustmentRecord glyphPairAdjustmentRecord = glyphPairAdjustmentRecords[num2];
							this.m_FontFeatureTable.m_GlyphPairAdjustmentRecords.Add(glyphPairAdjustmentRecord);
							this.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup.Add(num3, glyphPairAdjustmentRecord);
						}
						num2++;
					}
				}
			}
		}

		internal void UpdateGlyphAdjustmentRecords(uint[] glyphIndexes)
		{
			using (FontAsset.k_UpdateGlyphAdjustmentRecordsMarker.Auto())
			{
				GlyphPairAdjustmentRecord[] glyphPairAdjustmentTable = FontEngine.GetGlyphPairAdjustmentTable(glyphIndexes);
				bool flag = glyphPairAdjustmentTable == null || glyphPairAdjustmentTable.Length == 0;
				if (!flag)
				{
					bool flag2 = this.m_FontFeatureTable == null;
					if (flag2)
					{
						this.m_FontFeatureTable = new FontFeatureTable();
					}
					int num = 0;
					while (num < glyphPairAdjustmentTable.Length && glyphPairAdjustmentTable[num].firstAdjustmentRecord.glyphIndex > 0U)
					{
						uint num2 = (glyphPairAdjustmentTable[num].secondAdjustmentRecord.glyphIndex << 16) | glyphPairAdjustmentTable[num].firstAdjustmentRecord.glyphIndex;
						bool flag3 = this.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup.ContainsKey(num2);
						if (!flag3)
						{
							GlyphPairAdjustmentRecord glyphPairAdjustmentRecord = glyphPairAdjustmentTable[num];
							this.m_FontFeatureTable.m_GlyphPairAdjustmentRecords.Add(glyphPairAdjustmentRecord);
							this.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup.Add(num2, glyphPairAdjustmentRecord);
						}
						num++;
					}
				}
			}
		}

		internal void UpdateGlyphAdjustmentRecords(List<uint> glyphIndexes)
		{
		}

		internal void UpdateGlyphAdjustmentRecords(List<uint> newGlyphIndexes, List<uint> allGlyphIndexes)
		{
		}

		private void CopyListDataToArray<T>(List<T> srcList, ref T[] dstArray)
		{
			int count = srcList.Count;
			bool flag = dstArray == null;
			if (flag)
			{
				dstArray = new T[count];
			}
			else
			{
				Array.Resize<T>(ref dstArray, count);
			}
			for (int i = 0; i < count; i++)
			{
				dstArray[i] = srcList[i];
			}
		}

		public void ClearFontAssetData(bool setAtlasSizeToZero = false)
		{
			using (FontAsset.k_ClearFontAssetDataMarker.Auto())
			{
				this.ClearFontAssetTables(true);
				this.ClearAtlasTextures(setAtlasSizeToZero);
				this.ReadFontAssetDefinition();
			}
		}

		internal void ClearFontAssetDataInternal(bool clearFontFeatures = false)
		{
			this.ClearFontAssetTables(clearFontFeatures);
			this.ClearAtlasTextures(true);
		}

		internal void UpdateFontAssetData()
		{
			using (FontAsset.k_UpdateFontAssetDataMarker.Auto())
			{
				uint[] array = new uint[this.m_CharacterTable.Count];
				for (int i = 0; i < this.m_CharacterTable.Count; i++)
				{
					array[i] = this.m_CharacterTable[i].unicode;
				}
				this.ClearFontAssetTables(true);
				this.ClearAtlasTextures(true);
				this.ReadFontAssetDefinition();
				bool flag = array.Length != 0;
				if (flag)
				{
					this.TryAddCharacters(array, true);
				}
			}
		}

		internal void ClearFontAssetTables(bool clearFontFeatures)
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
			if (clearFontFeatures)
			{
				bool flag7 = this.m_FontFeatureTable != null && this.m_FontFeatureTable.m_LigatureSubstitutionRecords != null;
				if (flag7)
				{
					this.m_FontFeatureTable.m_LigatureSubstitutionRecords.Clear();
				}
				bool flag8 = this.m_FontFeatureTable != null && this.m_FontFeatureTable.m_GlyphPairAdjustmentRecords != null;
				if (flag8)
				{
					this.m_FontFeatureTable.m_GlyphPairAdjustmentRecords.Clear();
				}
				bool flag9 = this.m_FontFeatureTable != null && this.m_FontFeatureTable.m_MarkToBaseAdjustmentRecords != null;
				if (flag9)
				{
					this.m_FontFeatureTable.m_MarkToBaseAdjustmentRecords.Clear();
				}
				bool flag10 = this.m_FontFeatureTable != null && this.m_FontFeatureTable.m_MarkToMarkAdjustmentRecords != null;
				if (flag10)
				{
					this.m_FontFeatureTable.m_MarkToMarkAdjustmentRecords.Clear();
				}
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
					bool flag2 = texture2D == null;
					if (!flag2)
					{
						Object.DestroyImmediate(texture2D, true);
					}
				}
				Array.Resize<Texture2D>(ref this.m_AtlasTextures, 1);
				texture2D = (this.m_AtlasTexture = this.m_AtlasTextures[0]);
				bool flag3 = !texture2D.isReadable;
				if (flag3)
				{
				}
				if (setAtlasSizeToZero)
				{
					texture2D.Reinitialize(1, 1, TextureFormat.Alpha8, false);
				}
				else
				{
					bool flag4 = texture2D.width != this.m_AtlasWidth || texture2D.height != this.m_AtlasHeight;
					if (flag4)
					{
						texture2D.Reinitialize(this.m_AtlasWidth, this.m_AtlasHeight, TextureFormat.Alpha8, false);
					}
				}
				FontEngine.ResetAtlasTexture(texture2D);
				texture2D.Apply();
			}
		}

		private void DestroyAtlasTextures()
		{
			bool flag = this.m_AtlasTextures == null;
			if (!flag)
			{
				for (int i = 0; i < this.m_AtlasTextures.Length; i++)
				{
					Texture2D texture2D = this.m_AtlasTextures[i];
					bool flag2 = texture2D != null;
					if (flag2)
					{
						Object.DestroyImmediate(texture2D);
					}
				}
			}
		}

		[SerializeField]
		internal string m_SourceFontFileGUID;

		[SerializeField]
		internal FontAssetCreationEditorSettings m_fontAssetCreationEditorSettings;

		[SerializeField]
		private Font m_SourceFontFile;

		[SerializeField]
		private string m_SourceFontFilePath;

		[SerializeField]
		private AtlasPopulationMode m_AtlasPopulationMode;

		[SerializeField]
		internal bool InternalDynamicOS;

		[SerializeField]
		internal FaceInfo m_FaceInfo;

		private int m_FamilyNameHashCode;

		private int m_StyleNameHashCode;

		[SerializeField]
		internal List<Glyph> m_GlyphTable = new List<Glyph>();

		internal Dictionary<uint, Glyph> m_GlyphLookupDictionary;

		[SerializeField]
		internal List<Character> m_CharacterTable = new List<Character>();

		internal Dictionary<uint, Character> m_CharacterLookupDictionary;

		internal Texture2D m_AtlasTexture;

		[SerializeField]
		internal Texture2D[] m_AtlasTextures;

		[SerializeField]
		internal int m_AtlasTextureIndex;

		[SerializeField]
		private bool m_IsMultiAtlasTexturesEnabled;

		[SerializeField]
		private bool m_ClearDynamicDataOnBuild;

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
		internal List<FontAsset> m_FallbackFontAssetTable;

		[SerializeField]
		private FontWeightPair[] m_FontWeightTable = new FontWeightPair[10];

		[SerializeField]
		[FormerlySerializedAs("normalStyle")]
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

		[SerializeField]
		[FormerlySerializedAs("italicStyle")]
		internal byte m_ItalicStyleSlant = 35;

		[SerializeField]
		[FormerlySerializedAs("tabSize")]
		internal byte m_TabMultiple = 10;

		internal bool IsFontAssetLookupTablesDirty;

		private static ProfilerMarker k_ReadFontAssetDefinitionMarker = new ProfilerMarker("FontAsset.ReadFontAssetDefinition");

		private static ProfilerMarker k_AddSynthesizedCharactersMarker = new ProfilerMarker("FontAsset.AddSynthesizedCharacters");

		private static ProfilerMarker k_TryAddCharacterMarker = new ProfilerMarker("FontAsset.TryAddCharacter");

		private static ProfilerMarker k_TryAddCharactersMarker = new ProfilerMarker("FontAsset.TryAddCharacters");

		private static ProfilerMarker k_UpdateGlyphAdjustmentRecordsMarker = new ProfilerMarker("FontAsset.UpdateGlyphAdjustmentRecords");

		private static ProfilerMarker k_UpdateDiacriticalMarkAdjustmentRecordsMarker = new ProfilerMarker("FontAsset.UpdateDiacriticalAdjustmentRecords");

		private static ProfilerMarker k_ClearFontAssetDataMarker = new ProfilerMarker("FontAsset.ClearFontAssetData");

		private static ProfilerMarker k_UpdateFontAssetDataMarker = new ProfilerMarker("FontAsset.UpdateFontAssetData");

		private static ProfilerMarker k_TryAddGlyphMarker = new ProfilerMarker("FontAsset.TryAddGlyphMarker");

		private static string s_DefaultMaterialSuffix = " Atlas Material";

		private static HashSet<int> k_SearchedFontAssetLookup;

		private static List<FontAsset> k_FontAssets_FontFeaturesUpdateQueue = new List<FontAsset>();

		private static HashSet<int> k_FontAssets_FontFeaturesUpdateQueueLookup = new HashSet<int>();

		private static List<Texture2D> k_FontAssets_AtlasTexturesUpdateQueue = new List<Texture2D>();

		private static HashSet<int> k_FontAssets_AtlasTexturesUpdateQueueLookup = new HashSet<int>();

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

		internal static uint[] k_GlyphIndexArray;
	}
}
