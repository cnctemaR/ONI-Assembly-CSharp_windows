using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.TextCore;
using UnityEngine.TextCore.LowLevel;

namespace TMPro
{
	[ExcludeFromPreset]
	[Serializable]
	public class TMP_FontAsset : TMP_Asset
	{
		public FontAssetCreationSettings creationSettings
		{
			get
			{
				return this.m_CreationSettings;
			}
			set
			{
				this.m_CreationSettings = value;
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

		internal int familyNameHashCode
		{
			get
			{
				if (this.m_FamilyNameHashCode == 0)
				{
					this.m_FamilyNameHashCode = TMP_TextUtilities.GetHashCode(this.m_FaceInfo.familyName);
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
				if (this.m_StyleNameHashCode == 0)
				{
					this.m_StyleNameHashCode = TMP_TextUtilities.GetHashCode(this.m_FaceInfo.styleName);
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
				if (this.m_GlyphLookupDictionary == null)
				{
					this.ReadFontAssetDefinition();
				}
				return this.m_GlyphLookupDictionary;
			}
		}

		public List<TMP_Character> characterTable
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

		public Dictionary<uint, TMP_Character> characterLookupTable
		{
			get
			{
				if (this.m_CharacterLookupDictionary == null)
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
				if (this.m_AtlasTexture == null)
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
				Texture2D[] atlasTextures = this.m_AtlasTextures;
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

		public TMP_FontFeatureTable fontFeatureTable
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

		public List<TMP_FontAsset> fallbackFontAssetTable
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

		public TMP_FontWeightPair[] fontWeightTable
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

		[Obsolete("The fontInfo property and underlying type is now obsolete. Please use the faceInfo property and FaceInfo type instead.")]
		public FaceInfo_Legacy fontInfo
		{
			get
			{
				return this.m_fontInfo;
			}
		}

		public static TMP_FontAsset CreateFontAsset(string familyName, string styleName, int pointSize = 90)
		{
			FontReference fontReference;
			if (FontEngine.TryGetSystemFontReference(familyName, styleName, out fontReference))
			{
				return TMP_FontAsset.CreateFontAsset(fontReference.filePath, fontReference.faceIndex, pointSize, 9, GlyphRenderMode.SDFAA, 1024, 1024, AtlasPopulationMode.DynamicOS, true);
			}
			Debug.Log(string.Concat(new string[] { "Unable to find a font file with the specified Family Name [", familyName, "] and Style [", styleName, "]." }));
			return null;
		}

		public static TMP_FontAsset CreateFontAsset(string fontFilePath, int faceIndex, int samplingPointSize, int atlasPadding, GlyphRenderMode renderMode, int atlasWidth, int atlasHeight)
		{
			return TMP_FontAsset.CreateFontAsset(fontFilePath, faceIndex, samplingPointSize, atlasPadding, renderMode, atlasWidth, atlasHeight, AtlasPopulationMode.Dynamic, true);
		}

		private static TMP_FontAsset CreateFontAsset(string fontFilePath, int faceIndex, int samplingPointSize, int atlasPadding, GlyphRenderMode renderMode, int atlasWidth, int atlasHeight, AtlasPopulationMode atlasPopulationMode, bool enableMultiAtlasSupport = true)
		{
			if (FontEngine.LoadFontFace(fontFilePath, (float)samplingPointSize, faceIndex) != FontEngineError.Success)
			{
				Debug.Log("Unable to load font face from [" + fontFilePath + "].");
				return null;
			}
			TMP_FontAsset tmp_FontAsset = TMP_FontAsset.CreateFontAssetInstance(null, atlasPadding, renderMode, atlasWidth, atlasHeight, atlasPopulationMode, enableMultiAtlasSupport);
			tmp_FontAsset.m_SourceFontFilePath = fontFilePath;
			return tmp_FontAsset;
		}

		public static TMP_FontAsset CreateFontAsset(Font font)
		{
			return TMP_FontAsset.CreateFontAsset(font, 90, 9, GlyphRenderMode.SDFAA, 1024, 1024, AtlasPopulationMode.Dynamic, true);
		}

		public static TMP_FontAsset CreateFontAsset(Font font, int samplingPointSize, int atlasPadding, GlyphRenderMode renderMode, int atlasWidth, int atlasHeight, AtlasPopulationMode atlasPopulationMode = AtlasPopulationMode.Dynamic, bool enableMultiAtlasSupport = true)
		{
			return TMP_FontAsset.CreateFontAsset(font, 0, samplingPointSize, atlasPadding, renderMode, atlasWidth, atlasHeight, atlasPopulationMode, enableMultiAtlasSupport);
		}

		private static TMP_FontAsset CreateFontAsset(Font font, int faceIndex, int samplingPointSize, int atlasPadding, GlyphRenderMode renderMode, int atlasWidth, int atlasHeight, AtlasPopulationMode atlasPopulationMode = AtlasPopulationMode.Dynamic, bool enableMultiAtlasSupport = true)
		{
			if (FontEngine.LoadFontFace(font, (float)samplingPointSize, faceIndex) != FontEngineError.Success)
			{
				Debug.LogWarning("Unable to load font face for [" + font.name + "]. Make sure \"Include Font Data\" is enabled in the Font Import Settings.", font);
				return null;
			}
			return TMP_FontAsset.CreateFontAssetInstance(font, atlasPadding, renderMode, atlasWidth, atlasHeight, atlasPopulationMode, enableMultiAtlasSupport);
		}

		private static TMP_FontAsset CreateFontAssetInstance(Font font, int atlasPadding, GlyphRenderMode renderMode, int atlasWidth, int atlasHeight, AtlasPopulationMode atlasPopulationMode, bool enableMultiAtlasSupport)
		{
			TMP_FontAsset tmp_FontAsset = ScriptableObject.CreateInstance<TMP_FontAsset>();
			tmp_FontAsset.m_Version = "1.1.0";
			tmp_FontAsset.faceInfo = FontEngine.GetFaceInfo();
			if (atlasPopulationMode == AtlasPopulationMode.Dynamic && font != null)
			{
				tmp_FontAsset.sourceFontFile = font;
			}
			tmp_FontAsset.atlasPopulationMode = atlasPopulationMode;
			tmp_FontAsset.clearDynamicDataOnBuild = TMP_Settings.clearDynamicDataOnBuild;
			tmp_FontAsset.atlasWidth = atlasWidth;
			tmp_FontAsset.atlasHeight = atlasHeight;
			tmp_FontAsset.atlasPadding = atlasPadding;
			tmp_FontAsset.atlasRenderMode = renderMode;
			tmp_FontAsset.atlasTextures = new Texture2D[1];
			TextureFormat textureFormat = (((renderMode & (GlyphRenderMode)65536) == (GlyphRenderMode)65536) ? TextureFormat.RGBA32 : TextureFormat.Alpha8);
			Texture2D texture2D = new Texture2D(1, 1, textureFormat, false);
			tmp_FontAsset.atlasTextures[0] = texture2D;
			tmp_FontAsset.isMultiAtlasTexturesEnabled = enableMultiAtlasSupport;
			int num;
			if ((renderMode & (GlyphRenderMode)16) == (GlyphRenderMode)16)
			{
				num = 0;
				Material material;
				if (textureFormat == TextureFormat.Alpha8)
				{
					material = new Material(ShaderUtilities.ShaderRef_MobileBitmap);
				}
				else
				{
					material = new Material(Shader.Find("TextMeshPro/Sprite"));
				}
				material.SetTexture(ShaderUtilities.ID_MainTex, texture2D);
				material.SetFloat(ShaderUtilities.ID_TextureWidth, (float)atlasWidth);
				material.SetFloat(ShaderUtilities.ID_TextureHeight, (float)atlasHeight);
				tmp_FontAsset.material = material;
			}
			else
			{
				num = 1;
				Material material2 = new Material(ShaderUtilities.ShaderRef_MobileSDF);
				material2.SetTexture(ShaderUtilities.ID_MainTex, texture2D);
				material2.SetFloat(ShaderUtilities.ID_TextureWidth, (float)atlasWidth);
				material2.SetFloat(ShaderUtilities.ID_TextureHeight, (float)atlasHeight);
				material2.SetFloat(ShaderUtilities.ID_GradientScale, (float)(atlasPadding + num));
				material2.SetFloat(ShaderUtilities.ID_WeightNormal, tmp_FontAsset.normalStyle);
				material2.SetFloat(ShaderUtilities.ID_WeightBold, tmp_FontAsset.boldStyle);
				tmp_FontAsset.material = material2;
			}
			tmp_FontAsset.freeGlyphRects = new List<GlyphRect>(8)
			{
				new GlyphRect(0, 0, atlasWidth - num, atlasHeight - num)
			};
			tmp_FontAsset.usedGlyphRects = new List<GlyphRect>(8);
			tmp_FontAsset.ReadFontAssetDefinition();
			return tmp_FontAsset;
		}

		private void RegisterCallbackInstance(TMP_FontAsset instance)
		{
			for (int i = 0; i < TMP_FontAsset.s_CallbackInstances.Count; i++)
			{
				TMP_FontAsset tmp_FontAsset;
				if (TMP_FontAsset.s_CallbackInstances[i].TryGetTarget(out tmp_FontAsset) && tmp_FontAsset == instance)
				{
					return;
				}
			}
			for (int j = 0; j < TMP_FontAsset.s_CallbackInstances.Count; j++)
			{
				TMP_FontAsset tmp_FontAsset2;
				if (!TMP_FontAsset.s_CallbackInstances[j].TryGetTarget(out tmp_FontAsset2))
				{
					TMP_FontAsset.s_CallbackInstances[j] = new WeakReference<TMP_FontAsset>(instance);
					return;
				}
			}
			TMP_FontAsset.s_CallbackInstances.Add(new WeakReference<TMP_FontAsset>(this));
		}

		private void OnDestroy()
		{
			this.DestroyAtlasTextures();
			global::UnityEngine.Object.DestroyImmediate(this.m_Material);
		}

		public void ReadFontAssetDefinition()
		{
			this.InitializeDictionaryLookupTables();
			this.AddSynthesizedCharactersAndFaceMetrics();
			if (this.m_FaceInfo.capLine == 0f && this.m_CharacterLookupDictionary.ContainsKey(88U))
			{
				uint glyphIndex = this.m_CharacterLookupDictionary[88U].glyphIndex;
				this.m_FaceInfo.capLine = this.m_GlyphLookupDictionary[glyphIndex].metrics.horizontalBearingY;
			}
			if (this.m_FaceInfo.meanLine == 0f && this.m_CharacterLookupDictionary.ContainsKey(120U))
			{
				uint glyphIndex2 = this.m_CharacterLookupDictionary[120U].glyphIndex;
				this.m_FaceInfo.meanLine = this.m_GlyphLookupDictionary[glyphIndex2].metrics.horizontalBearingY;
			}
			if (this.m_FaceInfo.scale == 0f)
			{
				this.m_FaceInfo.scale = 1f;
			}
			if (this.m_FaceInfo.strikethroughOffset == 0f)
			{
				this.m_FaceInfo.strikethroughOffset = this.m_FaceInfo.capLine / 2.5f;
			}
			if (this.m_AtlasPadding == 0 && base.material.HasProperty(ShaderUtilities.ID_GradientScale))
			{
				this.m_AtlasPadding = (int)base.material.GetFloat(ShaderUtilities.ID_GradientScale) - 1;
			}
			if (this.m_FaceInfo.unitsPerEM == 0 && this.atlasPopulationMode != AtlasPopulationMode.Static)
			{
				if (!JobsUtility.IsExecutingJob)
				{
					this.m_FaceInfo.unitsPerEM = FontEngine.GetFaceInfo().unitsPerEM;
					Debug.Log(string.Concat(new string[]
					{
						"Font Asset [",
						base.name,
						"] Units Per EM set to ",
						this.m_FaceInfo.unitsPerEM.ToString(),
						". Please commit the newly serialized value."
					}));
				}
				else
				{
					Debug.LogError(string.Concat(new string[] { "Font Asset [", base.name, "] is missing Units Per EM. Please select the 'Reset FaceInfo' menu item on Font Asset [", base.name, "] to ensure proper serialization." }));
				}
			}
			base.hashCode = TMP_TextUtilities.GetHashCode(base.name);
			this.familyNameHashCode = TMP_TextUtilities.GetHashCode(this.m_FaceInfo.familyName);
			this.styleNameHashCode = TMP_TextUtilities.GetHashCode(this.m_FaceInfo.styleName);
			base.materialHashCode = TMP_TextUtilities.GetSimpleHashCode(base.name + TMP_FontAsset.s_DefaultMaterialSuffix);
			TMP_ResourceManager.AddFontAsset(this);
			this.IsFontAssetLookupTablesDirty = false;
			this.RegisterCallbackInstance(this);
		}

		internal void InitializeDictionaryLookupTables()
		{
			this.InitializeGlyphLookupDictionary();
			this.InitializeCharacterLookupDictionary();
			if ((this.m_AtlasPopulationMode == AtlasPopulationMode.Dynamic || this.m_AtlasPopulationMode == AtlasPopulationMode.DynamicOS) && this.m_ShouldReimportFontFeatures)
			{
				this.ImportFontFeatures();
			}
			this.InitializeLigatureSubstitutionLookupDictionary();
			this.InitializeGlyphPaidAdjustmentRecordsLookupDictionary();
			this.InitializeMarkToBaseAdjustmentRecordsLookupDictionary();
			this.InitializeMarkToMarkAdjustmentRecordsLookupDictionary();
		}

		internal void InitializeGlyphLookupDictionary()
		{
			if (this.m_GlyphLookupDictionary == null)
			{
				this.m_GlyphLookupDictionary = new Dictionary<uint, Glyph>();
			}
			else
			{
				this.m_GlyphLookupDictionary.Clear();
			}
			if (this.m_GlyphIndexList == null)
			{
				this.m_GlyphIndexList = new List<uint>();
			}
			else
			{
				this.m_GlyphIndexList.Clear();
			}
			if (this.m_GlyphIndexListNewlyAdded == null)
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
				if (!this.m_GlyphLookupDictionary.ContainsKey(index))
				{
					this.m_GlyphLookupDictionary.Add(index, glyph);
					this.m_GlyphIndexList.Add(index);
				}
			}
		}

		internal void InitializeCharacterLookupDictionary()
		{
			if (this.m_CharacterLookupDictionary == null)
			{
				this.m_CharacterLookupDictionary = new Dictionary<uint, TMP_Character>();
			}
			else
			{
				this.m_CharacterLookupDictionary.Clear();
			}
			for (int i = 0; i < this.m_CharacterTable.Count; i++)
			{
				TMP_Character tmp_Character = this.m_CharacterTable[i];
				uint unicode = tmp_Character.unicode;
				uint glyphIndex = tmp_Character.glyphIndex;
				if (!this.m_CharacterLookupDictionary.ContainsKey(unicode))
				{
					this.m_CharacterLookupDictionary.Add(unicode, tmp_Character);
					tmp_Character.textAsset = this;
					tmp_Character.glyph = this.m_GlyphLookupDictionary[glyphIndex];
				}
			}
			if (this.m_MissingUnicodesFromFontFile != null)
			{
				this.m_MissingUnicodesFromFontFile.Clear();
			}
		}

		internal void ClearFallbackCharacterTable()
		{
			List<uint> list = new List<uint>();
			foreach (KeyValuePair<uint, TMP_Character> keyValuePair in this.m_CharacterLookupDictionary)
			{
				if (keyValuePair.Value.textAsset != this)
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
			if (this.m_FontFeatureTable.m_LigatureSubstitutionRecordLookup == null)
			{
				this.m_FontFeatureTable.m_LigatureSubstitutionRecordLookup = new Dictionary<uint, List<LigatureSubstitutionRecord>>();
			}
			else
			{
				this.m_FontFeatureTable.m_LigatureSubstitutionRecordLookup.Clear();
			}
			List<LigatureSubstitutionRecord> ligatureSubstitutionRecords = this.m_FontFeatureTable.m_LigatureSubstitutionRecords;
			if (ligatureSubstitutionRecords != null)
			{
				for (int i = 0; i < ligatureSubstitutionRecords.Count; i++)
				{
					LigatureSubstitutionRecord ligatureSubstitutionRecord = ligatureSubstitutionRecords[i];
					if (ligatureSubstitutionRecord.componentGlyphIDs != null && ligatureSubstitutionRecord.componentGlyphIDs.Length != 0)
					{
						uint num = ligatureSubstitutionRecord.componentGlyphIDs[0];
						if (!this.m_FontFeatureTable.m_LigatureSubstitutionRecordLookup.ContainsKey(num))
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
			if (this.m_KerningTable != null && this.m_KerningTable.kerningPairs != null && this.m_KerningTable.kerningPairs.Count > 0)
			{
				this.UpgradeGlyphAdjustmentTableToFontFeatureTable();
			}
			if (this.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup == null)
			{
				this.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup = new Dictionary<uint, GlyphPairAdjustmentRecord>();
			}
			else
			{
				this.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup.Clear();
			}
			List<GlyphPairAdjustmentRecord> glyphPairAdjustmentRecords = this.m_FontFeatureTable.m_GlyphPairAdjustmentRecords;
			if (glyphPairAdjustmentRecords != null)
			{
				for (int i = 0; i < glyphPairAdjustmentRecords.Count; i++)
				{
					GlyphPairAdjustmentRecord glyphPairAdjustmentRecord = glyphPairAdjustmentRecords[i];
					uint num = (glyphPairAdjustmentRecord.secondAdjustmentRecord.glyphIndex << 16) | glyphPairAdjustmentRecord.firstAdjustmentRecord.glyphIndex;
					if (!this.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup.ContainsKey(num))
					{
						this.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup.Add(num, glyphPairAdjustmentRecord);
					}
				}
			}
		}

		internal void InitializeMarkToBaseAdjustmentRecordsLookupDictionary()
		{
			if (this.m_FontFeatureTable.m_MarkToBaseAdjustmentRecordLookup == null)
			{
				this.m_FontFeatureTable.m_MarkToBaseAdjustmentRecordLookup = new Dictionary<uint, MarkToBaseAdjustmentRecord>();
			}
			else
			{
				this.m_FontFeatureTable.m_MarkToBaseAdjustmentRecordLookup.Clear();
			}
			List<MarkToBaseAdjustmentRecord> markToBaseAdjustmentRecords = this.m_FontFeatureTable.m_MarkToBaseAdjustmentRecords;
			if (markToBaseAdjustmentRecords != null)
			{
				for (int i = 0; i < markToBaseAdjustmentRecords.Count; i++)
				{
					MarkToBaseAdjustmentRecord markToBaseAdjustmentRecord = markToBaseAdjustmentRecords[i];
					uint num = (markToBaseAdjustmentRecord.markGlyphID << 16) | markToBaseAdjustmentRecord.baseGlyphID;
					if (!this.m_FontFeatureTable.m_MarkToBaseAdjustmentRecordLookup.ContainsKey(num))
					{
						this.m_FontFeatureTable.m_MarkToBaseAdjustmentRecordLookup.Add(num, markToBaseAdjustmentRecord);
					}
				}
			}
		}

		internal void InitializeMarkToMarkAdjustmentRecordsLookupDictionary()
		{
			if (this.m_FontFeatureTable.m_MarkToMarkAdjustmentRecordLookup == null)
			{
				this.m_FontFeatureTable.m_MarkToMarkAdjustmentRecordLookup = new Dictionary<uint, MarkToMarkAdjustmentRecord>();
			}
			else
			{
				this.m_FontFeatureTable.m_MarkToMarkAdjustmentRecordLookup.Clear();
			}
			List<MarkToMarkAdjustmentRecord> markToMarkAdjustmentRecords = this.m_FontFeatureTable.m_MarkToMarkAdjustmentRecords;
			if (markToMarkAdjustmentRecords != null)
			{
				for (int i = 0; i < markToMarkAdjustmentRecords.Count; i++)
				{
					MarkToMarkAdjustmentRecord markToMarkAdjustmentRecord = markToMarkAdjustmentRecords[i];
					uint num = (markToMarkAdjustmentRecord.combiningMarkGlyphID << 16) | markToMarkAdjustmentRecord.baseMarkGlyphID;
					if (!this.m_FontFeatureTable.m_MarkToMarkAdjustmentRecordLookup.ContainsKey(num))
					{
						this.m_FontFeatureTable.m_MarkToMarkAdjustmentRecordLookup.Add(num, markToMarkAdjustmentRecord);
					}
				}
			}
		}

		internal void AddSynthesizedCharactersAndFaceMetrics()
		{
			bool flag = false;
			if (this.m_AtlasPopulationMode == AtlasPopulationMode.Dynamic || this.m_AtlasPopulationMode == AtlasPopulationMode.DynamicOS)
			{
				flag = this.LoadFontFace() == FontEngineError.Success;
				if (!flag && !this.InternalDynamicOS && TMP_Settings.warningsDisabled)
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
			if (this.m_CharacterLookupDictionary.ContainsKey(unicode))
			{
				return;
			}
			Glyph glyph;
			if (!isFontFaceLoaded || FontEngine.GetGlyphIndex(unicode) == 0U)
			{
				glyph = new Glyph(0U, new GlyphMetrics(0f, 0f, 0f, 0f, 0f), GlyphRect.zero, 1f, 0);
				this.m_CharacterLookupDictionary.Add(unicode, new TMP_Character(unicode, this, glyph));
				return;
			}
			if (!addImmediately)
			{
				return;
			}
			GlyphLoadFlags glyphLoadFlags = (((this.m_AtlasRenderMode & (GlyphRenderMode)4) == (GlyphRenderMode)4) ? (GlyphLoadFlags.LOAD_NO_HINTING | GlyphLoadFlags.LOAD_NO_BITMAP) : GlyphLoadFlags.LOAD_NO_BITMAP);
			if (FontEngine.TryGetGlyphWithUnicodeValue(unicode, glyphLoadFlags, out glyph))
			{
				this.m_CharacterLookupDictionary.Add(unicode, new TMP_Character(unicode, this, glyph));
			}
		}

		internal void AddCharacterToLookupCache(uint unicode, TMP_Character character, FontStyles fontStyle = FontStyles.Normal, FontWeight fontWeight = FontWeight.Regular, bool isAlternativeTypeface = false)
		{
			uint num = unicode;
			if (fontStyle != FontStyles.Normal || fontWeight != FontWeight.Regular)
			{
				num = (uint)(((uint)((isAlternativeTypeface ? FontStyles.Superscript : FontStyles.Normal) | ((uint)fontStyle << 4) | (FontStyles)(fontWeight / FontWeight.Thin)) << 24) | (FontStyles)unicode);
			}
			this.m_CharacterLookupDictionary.TryAdd(num, character);
		}

		internal FontEngineError LoadFontFace()
		{
			if (this.m_AtlasPopulationMode != AtlasPopulationMode.Dynamic)
			{
				return FontEngine.LoadFontFace(this.m_FaceInfo.familyName, this.m_FaceInfo.styleName, this.m_FaceInfo.pointSize);
			}
			if (FontEngine.LoadFontFace(this.m_SourceFontFile, this.m_FaceInfo.pointSize, this.m_FaceInfo.faceIndex) == FontEngineError.Success)
			{
				return FontEngineError.Success;
			}
			if (!string.IsNullOrEmpty(this.m_SourceFontFilePath))
			{
				return FontEngine.LoadFontFace(this.m_SourceFontFilePath, this.m_FaceInfo.pointSize, this.m_FaceInfo.faceIndex);
			}
			return FontEngineError.Invalid_Face;
		}

		internal void SortCharacterTable()
		{
			if (this.m_CharacterTable != null && this.m_CharacterTable.Count > 0)
			{
				this.m_CharacterTable = this.m_CharacterTable.OrderBy<TMP_Character, uint>((TMP_Character c) => c.unicode).ToList<TMP_Character>();
			}
		}

		internal void SortGlyphTable()
		{
			if (this.m_GlyphTable != null && this.m_GlyphTable.Count > 0)
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
			return this.characterLookupTable != null && this.m_CharacterLookupDictionary.ContainsKey((uint)character);
		}

		public bool HasCharacter(char character, bool searchFallbacks = false, bool tryAddCharacter = false)
		{
			if (this.characterLookupTable == null)
			{
				return false;
			}
			if (this.m_CharacterLookupDictionary.ContainsKey((uint)character))
			{
				return true;
			}
			TMP_Character tmp_Character;
			if (tryAddCharacter && (this.m_AtlasPopulationMode == AtlasPopulationMode.Dynamic || this.m_AtlasPopulationMode == AtlasPopulationMode.DynamicOS) && this.TryAddCharacterInternal((uint)character, out tmp_Character))
			{
				return true;
			}
			if (searchFallbacks)
			{
				if (TMP_FontAsset.k_SearchedFontAssetLookup == null)
				{
					TMP_FontAsset.k_SearchedFontAssetLookup = new HashSet<int>();
				}
				else
				{
					TMP_FontAsset.k_SearchedFontAssetLookup.Clear();
				}
				TMP_FontAsset.k_SearchedFontAssetLookup.Add(base.GetInstanceID());
				if (this.fallbackFontAssetTable != null && this.fallbackFontAssetTable.Count > 0)
				{
					int num = 0;
					while (num < this.fallbackFontAssetTable.Count && this.fallbackFontAssetTable[num] != null)
					{
						TMP_FontAsset tmp_FontAsset = this.fallbackFontAssetTable[num];
						int instanceID = tmp_FontAsset.GetInstanceID();
						if (TMP_FontAsset.k_SearchedFontAssetLookup.Add(instanceID) && tmp_FontAsset.HasCharacter_Internal((uint)character, true, tryAddCharacter))
						{
							return true;
						}
						num++;
					}
				}
				if (TMP_Settings.fallbackFontAssets != null && TMP_Settings.fallbackFontAssets.Count > 0)
				{
					int num2 = 0;
					while (num2 < TMP_Settings.fallbackFontAssets.Count && TMP_Settings.fallbackFontAssets[num2] != null)
					{
						TMP_FontAsset tmp_FontAsset2 = TMP_Settings.fallbackFontAssets[num2];
						int instanceID2 = tmp_FontAsset2.GetInstanceID();
						if (TMP_FontAsset.k_SearchedFontAssetLookup.Add(instanceID2) && tmp_FontAsset2.HasCharacter_Internal((uint)character, true, tryAddCharacter))
						{
							return true;
						}
						num2++;
					}
				}
				if (TMP_Settings.defaultFontAsset != null)
				{
					TMP_FontAsset defaultFontAsset = TMP_Settings.defaultFontAsset;
					int instanceID3 = defaultFontAsset.GetInstanceID();
					if (TMP_FontAsset.k_SearchedFontAssetLookup.Add(instanceID3) && defaultFontAsset.HasCharacter_Internal((uint)character, true, tryAddCharacter))
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool HasCharacter_Internal(uint character, bool searchFallbacks = false, bool tryAddCharacter = false)
		{
			if (this.m_CharacterLookupDictionary == null)
			{
				this.ReadFontAssetDefinition();
				if (this.m_CharacterLookupDictionary == null)
				{
					return false;
				}
			}
			if (this.m_CharacterLookupDictionary.ContainsKey(character))
			{
				return true;
			}
			TMP_Character tmp_Character;
			if (tryAddCharacter && (this.atlasPopulationMode == AtlasPopulationMode.Dynamic || this.m_AtlasPopulationMode == AtlasPopulationMode.DynamicOS) && this.TryAddCharacterInternal(character, out tmp_Character))
			{
				return true;
			}
			if (searchFallbacks)
			{
				if (this.fallbackFontAssetTable == null || this.fallbackFontAssetTable.Count == 0)
				{
					return false;
				}
				int num = 0;
				while (num < this.fallbackFontAssetTable.Count && this.fallbackFontAssetTable[num] != null)
				{
					TMP_FontAsset tmp_FontAsset = this.fallbackFontAssetTable[num];
					int instanceID = tmp_FontAsset.GetInstanceID();
					if (TMP_FontAsset.k_SearchedFontAssetLookup.Add(instanceID) && tmp_FontAsset.HasCharacter_Internal(character, true, tryAddCharacter))
					{
						return true;
					}
					num++;
				}
			}
			return false;
		}

		public bool HasCharacters(string text, out List<char> missingCharacters)
		{
			if (this.characterLookupTable == null)
			{
				missingCharacters = null;
				return false;
			}
			missingCharacters = new List<char>();
			for (int i = 0; i < text.Length; i++)
			{
				uint codePoint = TMP_FontAssetUtilities.GetCodePoint(text, ref i);
				if (!this.m_CharacterLookupDictionary.ContainsKey(codePoint))
				{
					missingCharacters.Add((char)codePoint);
				}
			}
			return missingCharacters.Count == 0;
		}

		public bool HasCharacters(string text, out uint[] missingCharacters, bool searchFallbacks = false, bool tryAddCharacter = false)
		{
			missingCharacters = null;
			if (this.characterLookupTable == null)
			{
				return false;
			}
			this.s_MissingCharacterList.Clear();
			for (int i = 0; i < text.Length; i++)
			{
				bool flag = true;
				uint codePoint = TMP_FontAssetUtilities.GetCodePoint(text, ref i);
				TMP_Character tmp_Character;
				if (!this.m_CharacterLookupDictionary.ContainsKey(codePoint) && (!tryAddCharacter || (this.atlasPopulationMode != AtlasPopulationMode.Dynamic && this.m_AtlasPopulationMode != AtlasPopulationMode.DynamicOS) || !this.TryAddCharacterInternal(codePoint, out tmp_Character)))
				{
					if (searchFallbacks)
					{
						if (TMP_FontAsset.k_SearchedFontAssetLookup == null)
						{
							TMP_FontAsset.k_SearchedFontAssetLookup = new HashSet<int>();
						}
						else
						{
							TMP_FontAsset.k_SearchedFontAssetLookup.Clear();
						}
						TMP_FontAsset.k_SearchedFontAssetLookup.Add(base.GetInstanceID());
						if (this.fallbackFontAssetTable != null && this.fallbackFontAssetTable.Count > 0)
						{
							int num = 0;
							while (num < this.fallbackFontAssetTable.Count && this.fallbackFontAssetTable[num] != null)
							{
								TMP_FontAsset tmp_FontAsset = this.fallbackFontAssetTable[num];
								int instanceID = tmp_FontAsset.GetInstanceID();
								if (TMP_FontAsset.k_SearchedFontAssetLookup.Add(instanceID) && tmp_FontAsset.HasCharacter_Internal(codePoint, true, tryAddCharacter))
								{
									flag = false;
									break;
								}
								num++;
							}
						}
						if (flag && TMP_Settings.fallbackFontAssets != null && TMP_Settings.fallbackFontAssets.Count > 0)
						{
							int num2 = 0;
							while (num2 < TMP_Settings.fallbackFontAssets.Count && TMP_Settings.fallbackFontAssets[num2] != null)
							{
								TMP_FontAsset tmp_FontAsset2 = TMP_Settings.fallbackFontAssets[num2];
								int instanceID2 = tmp_FontAsset2.GetInstanceID();
								if (TMP_FontAsset.k_SearchedFontAssetLookup.Add(instanceID2) && tmp_FontAsset2.HasCharacter_Internal(codePoint, true, tryAddCharacter))
								{
									flag = false;
									break;
								}
								num2++;
							}
						}
						if (flag && TMP_Settings.defaultFontAsset != null)
						{
							TMP_FontAsset defaultFontAsset = TMP_Settings.defaultFontAsset;
							int instanceID3 = defaultFontAsset.GetInstanceID();
							if (TMP_FontAsset.k_SearchedFontAssetLookup.Add(instanceID3) && defaultFontAsset.HasCharacter_Internal(codePoint, true, tryAddCharacter))
							{
								flag = false;
							}
						}
					}
					if (flag)
					{
						this.s_MissingCharacterList.Add(codePoint);
					}
				}
			}
			if (this.s_MissingCharacterList.Count > 0)
			{
				missingCharacters = this.s_MissingCharacterList.ToArray();
				return false;
			}
			return true;
		}

		public bool HasCharacters(string text)
		{
			if (this.characterLookupTable == null)
			{
				return false;
			}
			for (int i = 0; i < text.Length; i++)
			{
				uint codePoint = TMP_FontAssetUtilities.GetCodePoint(text, ref i);
				if (!this.m_CharacterLookupDictionary.ContainsKey(codePoint))
				{
					return false;
				}
			}
			return true;
		}

		public static string GetCharacters(TMP_FontAsset fontAsset)
		{
			string text = string.Empty;
			for (int i = 0; i < fontAsset.characterTable.Count; i++)
			{
				text += ((char)fontAsset.characterTable[i].unicode).ToString();
			}
			return text;
		}

		public static int[] GetCharactersArray(TMP_FontAsset fontAsset)
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
			if (this.m_CharacterLookupDictionary.ContainsKey(unicode))
			{
				return this.m_CharacterLookupDictionary[unicode].glyphIndex;
			}
			if (this.LoadFontFace() != FontEngineError.Success)
			{
				return 0U;
			}
			return FontEngine.GetGlyphIndex(unicode);
		}

		internal uint GetGlyphVariantIndex(uint unicode, uint variantSelectorUnicode)
		{
			if (this.LoadFontFace() != FontEngineError.Success)
			{
				return 0U;
			}
			return FontEngine.GetVariantGlyphIndex(unicode, variantSelectorUnicode);
		}

		internal static void RegisterFontAssetForFontFeatureUpdate(TMP_FontAsset fontAsset)
		{
			int instanceID = fontAsset.instanceID;
			if (TMP_FontAsset.k_FontAssets_FontFeaturesUpdateQueueLookup.Add(instanceID))
			{
				TMP_FontAsset.k_FontAssets_FontFeaturesUpdateQueue.Add(fontAsset);
			}
		}

		internal static void UpdateFontFeaturesForFontAssetsInQueue()
		{
			int count = TMP_FontAsset.k_FontAssets_FontFeaturesUpdateQueue.Count;
			for (int i = 0; i < count; i++)
			{
				TMP_FontAsset.k_FontAssets_FontFeaturesUpdateQueue[i].UpdateGPOSFontFeaturesForNewlyAddedGlyphs();
			}
			if (count > 0)
			{
				TMP_FontAsset.k_FontAssets_FontFeaturesUpdateQueue.Clear();
				TMP_FontAsset.k_FontAssets_FontFeaturesUpdateQueueLookup.Clear();
			}
		}

		internal static void RegisterAtlasTextureForApply(Texture2D texture)
		{
			int instanceID = texture.GetInstanceID();
			if (TMP_FontAsset.k_FontAssets_AtlasTexturesUpdateQueueLookup.Add(instanceID))
			{
				TMP_FontAsset.k_FontAssets_AtlasTexturesUpdateQueue.Add(texture);
			}
		}

		internal static void UpdateAtlasTexturesInQueue()
		{
			int count = TMP_FontAsset.k_FontAssets_AtlasTexturesUpdateQueueLookup.Count;
			for (int i = 0; i < count; i++)
			{
				TMP_FontAsset.k_FontAssets_AtlasTexturesUpdateQueue[i].Apply(false, false);
			}
			if (count > 0)
			{
				TMP_FontAsset.k_FontAssets_AtlasTexturesUpdateQueue.Clear();
				TMP_FontAsset.k_FontAssets_AtlasTexturesUpdateQueueLookup.Clear();
			}
		}

		internal static void UpdateFontAssetsInUpdateQueue()
		{
			TMP_FontAsset.UpdateAtlasTexturesInQueue();
			TMP_FontAsset.UpdateFontFeaturesForFontAssetsInQueue();
		}

		public bool TryAddCharacters(uint[] unicodes, bool includeFontFeatures = false)
		{
			uint[] array;
			return this.TryAddCharacters(unicodes, out array, includeFontFeatures);
		}

		public bool TryAddCharacters(uint[] unicodes, out uint[] missingUnicodes, bool includeFontFeatures = false)
		{
			if (unicodes == null || unicodes.Length == 0 || this.m_AtlasPopulationMode == AtlasPopulationMode.Static)
			{
				if (this.m_AtlasPopulationMode == AtlasPopulationMode.Static)
				{
					Debug.LogWarning("Unable to add characters to font asset [" + base.name + "] because its AtlasPopulationMode is set to Static.", this);
				}
				else
				{
					Debug.LogWarning("Unable to add characters to font asset [" + base.name + "] because the provided Unicode list is Null or Empty.", this);
				}
				missingUnicodes = null;
				return false;
			}
			if (this.LoadFontFace() != FontEngineError.Success)
			{
				missingUnicodes = unicodes.ToArray<uint>();
				return false;
			}
			if (this.m_CharacterLookupDictionary == null || this.m_GlyphLookupDictionary == null)
			{
				this.ReadFontAssetDefinition();
			}
			this.m_GlyphsToAdd.Clear();
			this.m_GlyphsToAddLookup.Clear();
			this.m_CharactersToAdd.Clear();
			this.m_CharactersToAddLookup.Clear();
			this.s_MissingCharacterList.Clear();
			bool flag = false;
			int num = unicodes.Length;
			for (int i = 0; i < num; i++)
			{
				uint codePoint = TMP_FontAssetUtilities.GetCodePoint(unicodes, ref i);
				if (!this.m_CharacterLookupDictionary.ContainsKey(codePoint))
				{
					uint num2 = FontEngine.GetGlyphIndex(codePoint);
					if (num2 == 0U)
					{
						if (codePoint != 160U)
						{
							if (codePoint == 173U || codePoint == 8209U)
							{
								num2 = FontEngine.GetGlyphIndex(45U);
							}
						}
						else
						{
							num2 = FontEngine.GetGlyphIndex(32U);
						}
						if (num2 == 0U)
						{
							this.s_MissingCharacterList.Add(codePoint);
							flag = true;
							goto IL_01BF;
						}
					}
					TMP_Character tmp_Character = new TMP_Character(codePoint, num2);
					if (this.m_GlyphLookupDictionary.ContainsKey(num2))
					{
						tmp_Character.glyph = this.m_GlyphLookupDictionary[num2];
						tmp_Character.textAsset = this;
						this.m_CharacterTable.Add(tmp_Character);
						this.m_CharacterLookupDictionary.Add(codePoint, tmp_Character);
					}
					else
					{
						if (this.m_GlyphsToAddLookup.Add(num2))
						{
							this.m_GlyphsToAdd.Add(num2);
						}
						if (this.m_CharactersToAddLookup.Add(codePoint))
						{
							this.m_CharactersToAdd.Add(tmp_Character);
						}
					}
				}
				IL_01BF:;
			}
			if (this.m_GlyphsToAdd.Count == 0)
			{
				missingUnicodes = unicodes;
				return false;
			}
			if (this.m_AtlasTextures[this.m_AtlasTextureIndex].width <= 1 || this.m_AtlasTextures[this.m_AtlasTextureIndex].height <= 1)
			{
				this.m_AtlasTextures[this.m_AtlasTextureIndex].Reinitialize(this.m_AtlasWidth, this.m_AtlasHeight);
				FontEngine.ResetAtlasTexture(this.m_AtlasTextures[this.m_AtlasTextureIndex]);
			}
			Glyph[] array;
			bool flag2 = FontEngine.TryAddGlyphsToTexture(this.m_GlyphsToAdd, this.m_AtlasPadding, GlyphPackingMode.BestShortSideFit, this.m_FreeGlyphRects, this.m_UsedGlyphRects, this.m_AtlasRenderMode, this.m_AtlasTextures[this.m_AtlasTextureIndex], out array);
			int num3 = 0;
			while (num3 < array.Length && array[num3] != null)
			{
				Glyph glyph = array[num3];
				uint index = glyph.index;
				glyph.atlasIndex = this.m_AtlasTextureIndex;
				this.m_GlyphTable.Add(glyph);
				this.m_GlyphLookupDictionary.Add(index, glyph);
				this.m_GlyphIndexListNewlyAdded.Add(index);
				this.m_GlyphIndexList.Add(index);
				num3++;
			}
			this.m_GlyphsToAdd.Clear();
			for (int j = 0; j < this.m_CharactersToAdd.Count; j++)
			{
				TMP_Character tmp_Character2 = this.m_CharactersToAdd[j];
				Glyph glyph2;
				if (!this.m_GlyphLookupDictionary.TryGetValue(tmp_Character2.glyphIndex, out glyph2))
				{
					this.m_GlyphsToAdd.Add(tmp_Character2.glyphIndex);
				}
				else
				{
					tmp_Character2.glyph = glyph2;
					tmp_Character2.textAsset = this;
					this.m_CharacterTable.Add(tmp_Character2);
					this.m_CharacterLookupDictionary.Add(tmp_Character2.unicode, tmp_Character2);
					this.m_CharactersToAdd.RemoveAt(j);
					j--;
				}
			}
			if (this.m_IsMultiAtlasTexturesEnabled && !flag2)
			{
				while (!flag2)
				{
					flag2 = this.TryAddGlyphsToNewAtlasTexture();
				}
			}
			if (includeFontFeatures)
			{
				this.UpdateFontFeaturesForNewlyAddedGlyphs();
			}
			for (int k = 0; k < this.m_CharactersToAdd.Count; k++)
			{
				TMP_Character tmp_Character3 = this.m_CharactersToAdd[k];
				this.s_MissingCharacterList.Add(tmp_Character3.unicode);
			}
			missingUnicodes = null;
			if (this.s_MissingCharacterList.Count > 0)
			{
				missingUnicodes = this.s_MissingCharacterList.ToArray();
			}
			return flag2 && !flag;
		}

		public bool TryAddCharacters(string characters, bool includeFontFeatures = false)
		{
			string text;
			return this.TryAddCharacters(characters, out text, includeFontFeatures);
		}

		public bool TryAddCharacters(string characters, out string missingCharacters, bool includeFontFeatures = false)
		{
			if (string.IsNullOrEmpty(characters) || this.m_AtlasPopulationMode == AtlasPopulationMode.Static)
			{
				if (this.m_AtlasPopulationMode == AtlasPopulationMode.Static)
				{
					Debug.LogWarning("Unable to add characters to font asset [" + base.name + "] because its AtlasPopulationMode is set to Static.", this);
				}
				else
				{
					Debug.LogWarning("Unable to add characters to font asset [" + base.name + "] because the provided character list is Null or Empty.", this);
				}
				missingCharacters = characters;
				return false;
			}
			if (this.LoadFontFace() != FontEngineError.Success)
			{
				missingCharacters = characters;
				return false;
			}
			if (this.m_CharacterLookupDictionary == null || this.m_GlyphLookupDictionary == null)
			{
				this.ReadFontAssetDefinition();
			}
			this.m_GlyphsToAdd.Clear();
			this.m_GlyphsToAddLookup.Clear();
			this.m_CharactersToAdd.Clear();
			this.m_CharactersToAddLookup.Clear();
			this.s_MissingCharacterList.Clear();
			bool flag = false;
			int length = characters.Length;
			for (int i = 0; i < length; i++)
			{
				uint num = (uint)characters[i];
				if (!this.m_CharacterLookupDictionary.ContainsKey(num))
				{
					uint num2 = FontEngine.GetGlyphIndex(num);
					if (num2 == 0U)
					{
						if (num != 160U)
						{
							if (num == 173U || num == 8209U)
							{
								num2 = FontEngine.GetGlyphIndex(45U);
							}
						}
						else
						{
							num2 = FontEngine.GetGlyphIndex(32U);
						}
						if (num2 == 0U)
						{
							this.s_MissingCharacterList.Add(num);
							flag = true;
							goto IL_01BE;
						}
					}
					TMP_Character tmp_Character = new TMP_Character(num, num2);
					if (this.m_GlyphLookupDictionary.ContainsKey(num2))
					{
						tmp_Character.glyph = this.m_GlyphLookupDictionary[num2];
						tmp_Character.textAsset = this;
						this.m_CharacterTable.Add(tmp_Character);
						this.m_CharacterLookupDictionary.Add(num, tmp_Character);
					}
					else
					{
						if (this.m_GlyphsToAddLookup.Add(num2))
						{
							this.m_GlyphsToAdd.Add(num2);
						}
						if (this.m_CharactersToAddLookup.Add(num))
						{
							this.m_CharactersToAdd.Add(tmp_Character);
						}
					}
				}
				IL_01BE:;
			}
			if (this.m_GlyphsToAdd.Count == 0)
			{
				missingCharacters = characters;
				return false;
			}
			if (this.m_AtlasTextures[this.m_AtlasTextureIndex].width <= 1 || this.m_AtlasTextures[this.m_AtlasTextureIndex].height <= 1)
			{
				this.m_AtlasTextures[this.m_AtlasTextureIndex].Reinitialize(this.m_AtlasWidth, this.m_AtlasHeight);
				FontEngine.ResetAtlasTexture(this.m_AtlasTextures[this.m_AtlasTextureIndex]);
			}
			Glyph[] array;
			bool flag2 = FontEngine.TryAddGlyphsToTexture(this.m_GlyphsToAdd, this.m_AtlasPadding, GlyphPackingMode.BestShortSideFit, this.m_FreeGlyphRects, this.m_UsedGlyphRects, this.m_AtlasRenderMode, this.m_AtlasTextures[this.m_AtlasTextureIndex], out array);
			int num3 = 0;
			while (num3 < array.Length && array[num3] != null)
			{
				Glyph glyph = array[num3];
				uint index = glyph.index;
				glyph.atlasIndex = this.m_AtlasTextureIndex;
				this.m_GlyphTable.Add(glyph);
				this.m_GlyphLookupDictionary.Add(index, glyph);
				this.m_GlyphIndexListNewlyAdded.Add(index);
				this.m_GlyphIndexList.Add(index);
				num3++;
			}
			this.m_GlyphsToAdd.Clear();
			for (int j = 0; j < this.m_CharactersToAdd.Count; j++)
			{
				TMP_Character tmp_Character2 = this.m_CharactersToAdd[j];
				Glyph glyph2;
				if (!this.m_GlyphLookupDictionary.TryGetValue(tmp_Character2.glyphIndex, out glyph2))
				{
					this.m_GlyphsToAdd.Add(tmp_Character2.glyphIndex);
				}
				else
				{
					tmp_Character2.glyph = glyph2;
					tmp_Character2.textAsset = this;
					this.m_CharacterTable.Add(tmp_Character2);
					this.m_CharacterLookupDictionary.Add(tmp_Character2.unicode, tmp_Character2);
					this.m_CharactersToAdd.RemoveAt(j);
					j--;
				}
			}
			if (this.m_IsMultiAtlasTexturesEnabled && !flag2)
			{
				while (!flag2)
				{
					flag2 = this.TryAddGlyphsToNewAtlasTexture();
				}
			}
			if (includeFontFeatures)
			{
				this.UpdateFontFeaturesForNewlyAddedGlyphs();
			}
			missingCharacters = string.Empty;
			for (int k = 0; k < this.m_CharactersToAdd.Count; k++)
			{
				TMP_Character tmp_Character3 = this.m_CharactersToAdd[k];
				this.s_MissingCharacterList.Add(tmp_Character3.unicode);
			}
			if (this.s_MissingCharacterList.Count > 0)
			{
				missingCharacters = this.s_MissingCharacterList.UintToString();
			}
			return flag2 && !flag;
		}

		internal bool AddGlyphInternal(uint glyphIndex)
		{
			Glyph glyph;
			return this.TryAddGlyphInternal(glyphIndex, out glyph);
		}

		internal bool TryAddGlyphInternal(uint glyphIndex, out Glyph glyph)
		{
			glyph = null;
			if (this.m_GlyphLookupDictionary.ContainsKey(glyphIndex))
			{
				glyph = this.m_GlyphLookupDictionary[glyphIndex];
				return true;
			}
			if (this.m_AtlasPopulationMode == AtlasPopulationMode.Static)
			{
				return false;
			}
			if (this.LoadFontFace() != FontEngineError.Success)
			{
				return false;
			}
			if (!this.m_AtlasTextures[this.m_AtlasTextureIndex].isReadable)
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
			if (this.m_AtlasTextures[this.m_AtlasTextureIndex].width <= 1 || this.m_AtlasTextures[this.m_AtlasTextureIndex].height <= 1)
			{
				this.m_AtlasTextures[this.m_AtlasTextureIndex].Reinitialize(this.m_AtlasWidth, this.m_AtlasHeight);
				FontEngine.ResetAtlasTexture(this.m_AtlasTextures[this.m_AtlasTextureIndex]);
			}
			if (FontEngine.TryAddGlyphToTexture(glyphIndex, this.m_AtlasPadding, GlyphPackingMode.BestShortSideFit, this.m_FreeGlyphRects, this.m_UsedGlyphRects, this.m_AtlasRenderMode, this.m_AtlasTextures[this.m_AtlasTextureIndex], out glyph))
			{
				glyph.atlasIndex = this.m_AtlasTextureIndex;
				this.m_GlyphTable.Add(glyph);
				this.m_GlyphLookupDictionary.Add(glyphIndex, glyph);
				this.m_GlyphIndexList.Add(glyphIndex);
				this.m_GlyphIndexListNewlyAdded.Add(glyphIndex);
				if (this.m_GetFontFeatures && TMP_Settings.getFontFeaturesAtRuntime)
				{
					this.UpdateGSUBFontFeaturesForNewGlyphIndex(glyphIndex);
					TMP_FontAsset.RegisterFontAssetForFontFeatureUpdate(this);
				}
				return true;
			}
			if (this.m_IsMultiAtlasTexturesEnabled && this.m_UsedGlyphRects.Count > 0)
			{
				this.SetupNewAtlasTexture();
				if (FontEngine.TryAddGlyphToTexture(glyphIndex, this.m_AtlasPadding, GlyphPackingMode.BestShortSideFit, this.m_FreeGlyphRects, this.m_UsedGlyphRects, this.m_AtlasRenderMode, this.m_AtlasTextures[this.m_AtlasTextureIndex], out glyph))
				{
					glyph.atlasIndex = this.m_AtlasTextureIndex;
					this.m_GlyphTable.Add(glyph);
					this.m_GlyphLookupDictionary.Add(glyphIndex, glyph);
					this.m_GlyphIndexList.Add(glyphIndex);
					this.m_GlyphIndexListNewlyAdded.Add(glyphIndex);
					if (this.m_GetFontFeatures && TMP_Settings.getFontFeaturesAtRuntime)
					{
						this.UpdateGSUBFontFeaturesForNewGlyphIndex(glyphIndex);
						TMP_FontAsset.RegisterFontAssetForFontFeatureUpdate(this);
					}
					return true;
				}
			}
			return false;
		}

		internal bool TryAddCharacterInternal(uint unicode, out TMP_Character character)
		{
			character = null;
			if (this.m_MissingUnicodesFromFontFile.Contains(unicode))
			{
				return false;
			}
			if (this.LoadFontFace() != FontEngineError.Success)
			{
				return false;
			}
			uint num = FontEngine.GetGlyphIndex(unicode);
			if (num == 0U)
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
				if (num == 0U)
				{
					this.m_MissingUnicodesFromFontFile.Add(unicode);
					return false;
				}
			}
			if (this.m_GlyphLookupDictionary.ContainsKey(num))
			{
				character = new TMP_Character(unicode, this, this.m_GlyphLookupDictionary[num]);
				this.m_CharacterTable.Add(character);
				this.m_CharacterLookupDictionary.Add(unicode, character);
				return true;
			}
			Glyph glyph = null;
			if (!this.m_AtlasTextures[this.m_AtlasTextureIndex].isReadable)
			{
				Debug.LogWarning(string.Concat(new string[]
				{
					"Unable to add the requested character to font asset [",
					base.name,
					"]'s atlas texture. Please make the texture [",
					this.m_AtlasTextures[this.m_AtlasTextureIndex].name,
					"] readable."
				}), this.m_AtlasTextures[this.m_AtlasTextureIndex]);
				return false;
			}
			if (this.m_AtlasTextures[this.m_AtlasTextureIndex].width <= 1 || this.m_AtlasTextures[this.m_AtlasTextureIndex].height <= 1)
			{
				this.m_AtlasTextures[this.m_AtlasTextureIndex].Reinitialize(this.m_AtlasWidth, this.m_AtlasHeight);
				FontEngine.ResetAtlasTexture(this.m_AtlasTextures[this.m_AtlasTextureIndex]);
			}
			if (FontEngine.TryAddGlyphToTexture(num, this.m_AtlasPadding, GlyphPackingMode.BestShortSideFit, this.m_FreeGlyphRects, this.m_UsedGlyphRects, this.m_AtlasRenderMode, this.m_AtlasTextures[this.m_AtlasTextureIndex], out glyph))
			{
				glyph.atlasIndex = this.m_AtlasTextureIndex;
				this.m_GlyphTable.Add(glyph);
				this.m_GlyphLookupDictionary.Add(num, glyph);
				character = new TMP_Character(unicode, this, glyph);
				this.m_CharacterTable.Add(character);
				this.m_CharacterLookupDictionary.Add(unicode, character);
				this.m_GlyphIndexList.Add(num);
				this.m_GlyphIndexListNewlyAdded.Add(num);
				if (this.m_GetFontFeatures && TMP_Settings.getFontFeaturesAtRuntime)
				{
					this.UpdateGSUBFontFeaturesForNewGlyphIndex(num);
					TMP_FontAsset.RegisterFontAssetForFontFeatureUpdate(this);
				}
				return true;
			}
			if (this.m_IsMultiAtlasTexturesEnabled && this.m_UsedGlyphRects.Count > 0)
			{
				this.SetupNewAtlasTexture();
				if (FontEngine.TryAddGlyphToTexture(num, this.m_AtlasPadding, GlyphPackingMode.BestShortSideFit, this.m_FreeGlyphRects, this.m_UsedGlyphRects, this.m_AtlasRenderMode, this.m_AtlasTextures[this.m_AtlasTextureIndex], out glyph))
				{
					glyph.atlasIndex = this.m_AtlasTextureIndex;
					this.m_GlyphTable.Add(glyph);
					this.m_GlyphLookupDictionary.Add(num, glyph);
					character = new TMP_Character(unicode, this, glyph);
					this.m_CharacterTable.Add(character);
					this.m_CharacterLookupDictionary.Add(unicode, character);
					this.m_GlyphIndexList.Add(num);
					this.m_GlyphIndexListNewlyAdded.Add(num);
					if (this.m_GetFontFeatures && TMP_Settings.getFontFeaturesAtRuntime)
					{
						this.UpdateGSUBFontFeaturesForNewGlyphIndex(num);
						TMP_FontAsset.RegisterFontAssetForFontFeatureUpdate(this);
					}
					return true;
				}
			}
			return false;
		}

		internal bool TryGetCharacter_and_QueueRenderToTexture(uint unicode, out TMP_Character character)
		{
			character = null;
			if (this.m_MissingUnicodesFromFontFile.Contains(unicode))
			{
				return false;
			}
			if (this.LoadFontFace() != FontEngineError.Success)
			{
				return false;
			}
			uint num = FontEngine.GetGlyphIndex(unicode);
			if (num == 0U)
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
				if (num == 0U)
				{
					this.m_MissingUnicodesFromFontFile.Add(unicode);
					return false;
				}
			}
			if (this.m_GlyphLookupDictionary.ContainsKey(num))
			{
				character = new TMP_Character(unicode, this, this.m_GlyphLookupDictionary[num]);
				this.m_CharacterTable.Add(character);
				this.m_CharacterLookupDictionary.Add(unicode, character);
				return true;
			}
			GlyphLoadFlags glyphLoadFlags = ((((GlyphRenderMode)4 & this.m_AtlasRenderMode) == (GlyphRenderMode)4) ? (GlyphLoadFlags.LOAD_NO_HINTING | GlyphLoadFlags.LOAD_NO_BITMAP) : GlyphLoadFlags.LOAD_NO_BITMAP);
			Glyph glyph = null;
			if (FontEngine.TryGetGlyphWithIndexValue(num, glyphLoadFlags, out glyph))
			{
				this.m_GlyphTable.Add(glyph);
				this.m_GlyphLookupDictionary.Add(num, glyph);
				character = new TMP_Character(unicode, this, glyph);
				this.m_CharacterTable.Add(character);
				this.m_CharacterLookupDictionary.Add(unicode, character);
				this.m_GlyphIndexList.Add(num);
				this.m_GlyphIndexListNewlyAdded.Add(num);
				if (this.m_GetFontFeatures && TMP_Settings.getFontFeaturesAtRuntime)
				{
					this.UpdateGSUBFontFeaturesForNewGlyphIndex(num);
					TMP_FontAsset.RegisterFontAssetForFontFeatureUpdate(this);
				}
				this.m_GlyphsToRender.Add(glyph);
				return true;
			}
			return false;
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
				TMP_Character tmp_Character = this.m_CharactersToAdd[i];
				Glyph glyph2;
				if (!this.m_GlyphLookupDictionary.TryGetValue(tmp_Character.glyphIndex, out glyph2))
				{
					this.m_GlyphsToAdd.Add(tmp_Character.glyphIndex);
				}
				else
				{
					tmp_Character.glyph = glyph2;
					tmp_Character.textAsset = this;
					this.m_CharacterTable.Add(tmp_Character);
					this.m_CharacterLookupDictionary.Add(tmp_Character.unicode, tmp_Character);
					this.m_CharactersToAdd.RemoveAt(i);
					i--;
				}
			}
			return flag;
		}

		private void SetupNewAtlasTexture()
		{
			this.m_AtlasTextureIndex++;
			if (this.m_AtlasTextures.Length == this.m_AtlasTextureIndex)
			{
				Array.Resize<Texture2D>(ref this.m_AtlasTextures, this.m_AtlasTextures.Length * 2);
			}
			TextureFormat textureFormat = (((this.m_AtlasRenderMode & (GlyphRenderMode)65536) == (GlyphRenderMode)65536) ? TextureFormat.RGBA32 : TextureFormat.Alpha8);
			this.m_AtlasTextures[this.m_AtlasTextureIndex] = new Texture2D(this.m_AtlasWidth, this.m_AtlasHeight, textureFormat, false);
			FontEngine.ResetAtlasTexture(this.m_AtlasTextures[this.m_AtlasTextureIndex]);
			int num = (((this.m_AtlasRenderMode & (GlyphRenderMode)16) == (GlyphRenderMode)16) ? 0 : 1);
			this.m_FreeGlyphRects.Clear();
			this.m_FreeGlyphRects.Add(new GlyphRect(0, 0, this.m_AtlasWidth - num, this.m_AtlasHeight - num));
			this.m_UsedGlyphRects.Clear();
		}

		internal void UpdateAtlasTexture()
		{
			if (this.m_GlyphsToRender.Count == 0)
			{
				return;
			}
			if (this.m_AtlasTextures[this.m_AtlasTextureIndex].width <= 1 || this.m_AtlasTextures[this.m_AtlasTextureIndex].height <= 1)
			{
				this.m_AtlasTextures[this.m_AtlasTextureIndex].Reinitialize(this.m_AtlasWidth, this.m_AtlasHeight);
				FontEngine.ResetAtlasTexture(this.m_AtlasTextures[this.m_AtlasTextureIndex]);
			}
			this.m_AtlasTextures[this.m_AtlasTextureIndex].Apply(false, false);
		}

		private void UpdateFontFeaturesForNewlyAddedGlyphs()
		{
			this.UpdateLigatureSubstitutionRecords();
			this.UpdateGlyphAdjustmentRecords();
			this.UpdateDiacriticalMarkAdjustmentRecords();
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
			if (this.LoadFontFace() != FontEngineError.Success)
			{
				return;
			}
			GlyphPairAdjustmentRecord[] allPairAdjustmentRecords = FontEngine.GetAllPairAdjustmentRecords();
			if (allPairAdjustmentRecords != null)
			{
				this.AddPairAdjustmentRecords(allPairAdjustmentRecords);
			}
			MarkToBaseAdjustmentRecord[] allMarkToBaseAdjustmentRecords = FontEngine.GetAllMarkToBaseAdjustmentRecords();
			if (allMarkToBaseAdjustmentRecords != null)
			{
				this.AddMarkToBaseAdjustmentRecords(allMarkToBaseAdjustmentRecords);
			}
			MarkToMarkAdjustmentRecord[] allMarkToMarkAdjustmentRecords = FontEngine.GetAllMarkToMarkAdjustmentRecords();
			if (allMarkToMarkAdjustmentRecords != null)
			{
				this.AddMarkToMarkAdjustmentRecords(allMarkToMarkAdjustmentRecords);
			}
			LigatureSubstitutionRecord[] allLigatureSubstitutionRecords = FontEngine.GetAllLigatureSubstitutionRecords();
			if (allLigatureSubstitutionRecords != null)
			{
				this.AddLigatureSubstitutionRecords(allLigatureSubstitutionRecords);
			}
			this.m_ShouldReimportFontFeatures = false;
		}

		private void UpdateGSUBFontFeaturesForNewGlyphIndex(uint glyphIndex)
		{
			LigatureSubstitutionRecord[] ligatureSubstitutionRecords = FontEngine.GetLigatureSubstitutionRecords(glyphIndex);
			if (ligatureSubstitutionRecords != null)
			{
				this.AddLigatureSubstitutionRecords(ligatureSubstitutionRecords);
			}
		}

		internal void UpdateLigatureSubstitutionRecords()
		{
			LigatureSubstitutionRecord[] ligatureSubstitutionRecords = FontEngine.GetLigatureSubstitutionRecords(this.m_GlyphIndexListNewlyAdded);
			if (ligatureSubstitutionRecords != null)
			{
				this.AddLigatureSubstitutionRecords(ligatureSubstitutionRecords);
			}
		}

		private void AddLigatureSubstitutionRecords(LigatureSubstitutionRecord[] records)
		{
			for (int i = 0; i < records.Length; i++)
			{
				LigatureSubstitutionRecord ligatureSubstitutionRecord = records[i];
				if (records[i].componentGlyphIDs == null || records[i].ligatureGlyphID == 0U)
				{
					return;
				}
				uint num = ligatureSubstitutionRecord.componentGlyphIDs[0];
				LigatureSubstitutionRecord ligatureSubstitutionRecord2 = new LigatureSubstitutionRecord
				{
					componentGlyphIDs = ligatureSubstitutionRecord.componentGlyphIDs,
					ligatureGlyphID = ligatureSubstitutionRecord.ligatureGlyphID
				};
				List<LigatureSubstitutionRecord> list;
				if (this.m_FontFeatureTable.m_LigatureSubstitutionRecordLookup.TryGetValue(num, out list))
				{
					foreach (LigatureSubstitutionRecord ligatureSubstitutionRecord3 in list)
					{
						if (ligatureSubstitutionRecord2 == ligatureSubstitutionRecord3)
						{
							return;
						}
					}
					this.m_FontFeatureTable.m_LigatureSubstitutionRecordLookup[num].Add(ligatureSubstitutionRecord2);
				}
				else
				{
					this.m_FontFeatureTable.m_LigatureSubstitutionRecordLookup.Add(num, new List<LigatureSubstitutionRecord> { ligatureSubstitutionRecord2 });
				}
				this.m_FontFeatureTable.m_LigatureSubstitutionRecords.Add(ligatureSubstitutionRecord2);
			}
		}

		internal void UpdateGlyphAdjustmentRecords()
		{
			GlyphPairAdjustmentRecord[] pairAdjustmentRecords = FontEngine.GetPairAdjustmentRecords(this.m_GlyphIndexListNewlyAdded);
			if (pairAdjustmentRecords != null)
			{
				this.AddPairAdjustmentRecords(pairAdjustmentRecords);
			}
		}

		private void AddPairAdjustmentRecords(GlyphPairAdjustmentRecord[] records)
		{
			float num = this.m_FaceInfo.pointSize / (float)this.m_FaceInfo.unitsPerEM;
			foreach (GlyphPairAdjustmentRecord glyphPairAdjustmentRecord in records)
			{
				GlyphAdjustmentRecord firstAdjustmentRecord = glyphPairAdjustmentRecord.firstAdjustmentRecord;
				GlyphAdjustmentRecord secondAdjustmentRecord = glyphPairAdjustmentRecord.secondAdjustmentRecord;
				uint glyphIndex = firstAdjustmentRecord.glyphIndex;
				uint glyphIndex2 = secondAdjustmentRecord.glyphIndex;
				if (glyphIndex == 0U && glyphIndex2 == 0U)
				{
					return;
				}
				uint num2 = (glyphIndex2 << 16) | glyphIndex;
				if (!this.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup.ContainsKey(num2))
				{
					GlyphValueRecord glyphValueRecord = firstAdjustmentRecord.glyphValueRecord;
					glyphValueRecord.xAdvance *= num;
					glyphPairAdjustmentRecord.firstAdjustmentRecord = new GlyphAdjustmentRecord(glyphIndex, glyphValueRecord);
					this.m_FontFeatureTable.m_GlyphPairAdjustmentRecords.Add(glyphPairAdjustmentRecord);
					this.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup.Add(num2, glyphPairAdjustmentRecord);
				}
			}
		}

		internal void UpdateGlyphAdjustmentRecords(uint[] glyphIndexes)
		{
			GlyphPairAdjustmentRecord[] glyphPairAdjustmentTable = FontEngine.GetGlyphPairAdjustmentTable(glyphIndexes);
			if (glyphPairAdjustmentTable == null || glyphPairAdjustmentTable.Length == 0)
			{
				return;
			}
			if (this.m_FontFeatureTable == null)
			{
				this.m_FontFeatureTable = new TMP_FontFeatureTable();
			}
			int num = 0;
			while (num < glyphPairAdjustmentTable.Length && glyphPairAdjustmentTable[num].firstAdjustmentRecord.glyphIndex != 0U)
			{
				uint num2 = (glyphPairAdjustmentTable[num].secondAdjustmentRecord.glyphIndex << 16) | glyphPairAdjustmentTable[num].firstAdjustmentRecord.glyphIndex;
				if (!this.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup.ContainsKey(num2))
				{
					GlyphPairAdjustmentRecord glyphPairAdjustmentRecord = glyphPairAdjustmentTable[num];
					this.m_FontFeatureTable.m_GlyphPairAdjustmentRecords.Add(glyphPairAdjustmentRecord);
					this.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup.Add(num2, glyphPairAdjustmentRecord);
				}
				num++;
			}
		}

		internal void UpdateDiacriticalMarkAdjustmentRecords()
		{
			MarkToBaseAdjustmentRecord[] markToBaseAdjustmentRecords = FontEngine.GetMarkToBaseAdjustmentRecords(this.m_GlyphIndexListNewlyAdded);
			if (markToBaseAdjustmentRecords != null)
			{
				this.AddMarkToBaseAdjustmentRecords(markToBaseAdjustmentRecords);
			}
			MarkToMarkAdjustmentRecord[] markToMarkAdjustmentRecords = FontEngine.GetMarkToMarkAdjustmentRecords(this.m_GlyphIndexListNewlyAdded);
			if (markToMarkAdjustmentRecords != null)
			{
				this.AddMarkToMarkAdjustmentRecords(markToMarkAdjustmentRecords);
			}
		}

		private void AddMarkToBaseAdjustmentRecords(MarkToBaseAdjustmentRecord[] records)
		{
			float num = this.m_FaceInfo.pointSize / (float)this.m_FaceInfo.unitsPerEM;
			for (int i = 0; i < records.Length; i++)
			{
				MarkToBaseAdjustmentRecord markToBaseAdjustmentRecord = records[i];
				if (records[i].baseGlyphID == 0U || records[i].markGlyphID == 0U)
				{
					return;
				}
				uint num2 = (markToBaseAdjustmentRecord.markGlyphID << 16) | markToBaseAdjustmentRecord.baseGlyphID;
				if (!this.m_FontFeatureTable.m_MarkToBaseAdjustmentRecordLookup.ContainsKey(num2))
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
				if (records[i].baseMarkGlyphID == 0U || records[i].combiningMarkGlyphID == 0U)
				{
					return;
				}
				uint num2 = (markToMarkAdjustmentRecord.combiningMarkGlyphID << 16) | markToMarkAdjustmentRecord.baseMarkGlyphID;
				if (!this.m_FontFeatureTable.m_MarkToMarkAdjustmentRecordLookup.ContainsKey(num2))
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

		private void CopyListDataToArray<T>(List<T> srcList, ref T[] dstArray)
		{
			int count = srcList.Count;
			if (dstArray == null)
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
			if (array.Length != 0)
			{
				this.TryAddCharacters(array, this.m_GetFontFeatures && TMP_Settings.getFontFeaturesAtRuntime);
			}
		}

		public void ClearFontAssetData(bool setAtlasSizeToZero = false)
		{
			this.ClearCharacterAndGlyphTables();
			this.ClearFontFeaturesTables();
			this.ClearAtlasTextures(setAtlasSizeToZero);
			this.ReadFontAssetDefinition();
			for (int i = 0; i < TMP_FontAsset.s_CallbackInstances.Count; i++)
			{
				TMP_FontAsset tmp_FontAsset;
				if (TMP_FontAsset.s_CallbackInstances[i].TryGetTarget(out tmp_FontAsset) && tmp_FontAsset != this)
				{
					tmp_FontAsset.ClearFallbackCharacterTable();
				}
			}
			TMPro_EventManager.ON_FONT_PROPERTY_CHANGED(true, this);
		}

		internal void ClearCharacterAndGlyphTablesInternal()
		{
			this.ClearCharacterAndGlyphTables();
			this.ClearAtlasTextures(true);
			this.ReadFontAssetDefinition();
		}

		internal void ClearFontFeaturesInternal()
		{
			this.ClearFontFeaturesTables();
			this.ReadFontAssetDefinition();
		}

		private void ClearCharacterAndGlyphTables()
		{
			if (this.m_GlyphTable != null)
			{
				this.m_GlyphTable.Clear();
			}
			if (this.m_CharacterTable != null)
			{
				this.m_CharacterTable.Clear();
			}
			if (this.m_UsedGlyphRects != null)
			{
				this.m_UsedGlyphRects.Clear();
			}
			if (this.m_FreeGlyphRects != null)
			{
				int num = (((this.m_AtlasRenderMode & (GlyphRenderMode)16) == (GlyphRenderMode)16) ? 0 : 1);
				this.m_FreeGlyphRects.Clear();
				this.m_FreeGlyphRects.Add(new GlyphRect(0, 0, this.m_AtlasWidth - num, this.m_AtlasHeight - num));
			}
			if (this.m_GlyphsToRender != null)
			{
				this.m_GlyphsToRender.Clear();
			}
			if (this.m_GlyphsRendered != null)
			{
				this.m_GlyphsRendered.Clear();
			}
		}

		private void ClearFontFeaturesTables()
		{
			if (this.m_FontFeatureTable != null && this.m_FontFeatureTable.m_LigatureSubstitutionRecords != null)
			{
				this.m_FontFeatureTable.m_LigatureSubstitutionRecords.Clear();
			}
			if (this.m_FontFeatureTable != null && this.m_FontFeatureTable.m_GlyphPairAdjustmentRecords != null)
			{
				this.m_FontFeatureTable.m_GlyphPairAdjustmentRecords.Clear();
			}
			if (this.m_FontFeatureTable != null && this.m_FontFeatureTable.m_MarkToBaseAdjustmentRecords != null)
			{
				this.m_FontFeatureTable.m_MarkToBaseAdjustmentRecords.Clear();
			}
			if (this.m_FontFeatureTable != null && this.m_FontFeatureTable.m_MarkToMarkAdjustmentRecords != null)
			{
				this.m_FontFeatureTable.m_MarkToMarkAdjustmentRecords.Clear();
			}
		}

		internal void ClearAtlasTextures(bool setAtlasSizeToZero = false)
		{
			this.m_AtlasTextureIndex = 0;
			if (this.m_AtlasTextures == null)
			{
				return;
			}
			Texture2D texture2D;
			for (int i = 1; i < this.m_AtlasTextures.Length; i++)
			{
				texture2D = this.m_AtlasTextures[i];
				if (!(texture2D == null))
				{
					global::UnityEngine.Object.DestroyImmediate(texture2D, true);
				}
			}
			Array.Resize<Texture2D>(ref this.m_AtlasTextures, 1);
			texture2D = (this.m_AtlasTexture = this.m_AtlasTextures[0]);
			bool isReadable = texture2D.isReadable;
			TextureFormat textureFormat = (((this.m_AtlasRenderMode & (GlyphRenderMode)65536) == (GlyphRenderMode)65536) ? TextureFormat.RGBA32 : TextureFormat.Alpha8);
			if (setAtlasSizeToZero)
			{
				texture2D.Reinitialize(1, 1, textureFormat, false);
			}
			else if (texture2D.width != this.m_AtlasWidth || texture2D.height != this.m_AtlasHeight)
			{
				texture2D.Reinitialize(this.m_AtlasWidth, this.m_AtlasHeight, textureFormat, false);
			}
			FontEngine.ResetAtlasTexture(texture2D);
			texture2D.Apply();
		}

		private void DestroyAtlasTextures()
		{
			if (this.m_AtlasTextures == null)
			{
				return;
			}
			for (int i = 0; i < this.m_AtlasTextures.Length; i++)
			{
				Texture2D texture2D = this.m_AtlasTextures[i];
				if (texture2D != null)
				{
					global::UnityEngine.Object.DestroyImmediate(texture2D);
				}
			}
		}

		private void UpgradeGlyphAdjustmentTableToFontFeatureTable()
		{
			Debug.Log("Upgrading font asset [" + base.name + "] Glyph Adjustment Table.", this);
			if (this.m_FontFeatureTable == null)
			{
				this.m_FontFeatureTable = new TMP_FontFeatureTable();
			}
			int count = this.m_KerningTable.kerningPairs.Count;
			this.m_FontFeatureTable.m_GlyphPairAdjustmentRecords = new List<GlyphPairAdjustmentRecord>(count);
			for (int i = 0; i < count; i++)
			{
				KerningPair kerningPair = this.m_KerningTable.kerningPairs[i];
				uint num = 0U;
				TMP_Character tmp_Character;
				if (this.m_CharacterLookupDictionary.TryGetValue(kerningPair.firstGlyph, out tmp_Character))
				{
					num = tmp_Character.glyphIndex;
				}
				uint num2 = 0U;
				TMP_Character tmp_Character2;
				if (this.m_CharacterLookupDictionary.TryGetValue(kerningPair.secondGlyph, out tmp_Character2))
				{
					num2 = tmp_Character2.glyphIndex;
				}
				GlyphAdjustmentRecord glyphAdjustmentRecord = new GlyphAdjustmentRecord(num, new GlyphValueRecord(kerningPair.firstGlyphAdjustments.xPlacement, kerningPair.firstGlyphAdjustments.yPlacement, kerningPair.firstGlyphAdjustments.xAdvance, kerningPair.firstGlyphAdjustments.yAdvance));
				GlyphAdjustmentRecord glyphAdjustmentRecord2 = new GlyphAdjustmentRecord(num2, new GlyphValueRecord(kerningPair.secondGlyphAdjustments.xPlacement, kerningPair.secondGlyphAdjustments.yPlacement, kerningPair.secondGlyphAdjustments.xAdvance, kerningPair.secondGlyphAdjustments.yAdvance));
				GlyphPairAdjustmentRecord glyphPairAdjustmentRecord = new GlyphPairAdjustmentRecord(glyphAdjustmentRecord, glyphAdjustmentRecord2);
				this.m_FontFeatureTable.m_GlyphPairAdjustmentRecords.Add(glyphPairAdjustmentRecord);
			}
			this.m_KerningTable.kerningPairs = null;
			this.m_KerningTable = null;
		}

		[SerializeField]
		internal string m_SourceFontFileGUID;

		[SerializeField]
		internal FontAssetCreationSettings m_CreationSettings;

		[SerializeField]
		private Font m_SourceFontFile;

		[SerializeField]
		private string m_SourceFontFilePath;

		[SerializeField]
		private AtlasPopulationMode m_AtlasPopulationMode;

		[SerializeField]
		internal bool InternalDynamicOS;

		private int m_FamilyNameHashCode;

		private int m_StyleNameHashCode;

		[SerializeField]
		internal List<Glyph> m_GlyphTable = new List<Glyph>();

		internal Dictionary<uint, Glyph> m_GlyphLookupDictionary;

		[SerializeField]
		internal List<TMP_Character> m_CharacterTable = new List<TMP_Character>();

		internal Dictionary<uint, TMP_Character> m_CharacterLookupDictionary;

		internal Texture2D m_AtlasTexture;

		[SerializeField]
		internal Texture2D[] m_AtlasTextures;

		[SerializeField]
		internal int m_AtlasTextureIndex;

		[SerializeField]
		private bool m_IsMultiAtlasTexturesEnabled;

		[SerializeField]
		private bool m_GetFontFeatures = true;

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
		internal TMP_FontFeatureTable m_FontFeatureTable = new TMP_FontFeatureTable();

		[SerializeField]
		internal bool m_ShouldReimportFontFeatures;

		[SerializeField]
		internal List<TMP_FontAsset> m_FallbackFontAssetTable;

		[SerializeField]
		private TMP_FontWeightPair[] m_FontWeightTable = new TMP_FontWeightPair[10];

		[SerializeField]
		private TMP_FontWeightPair[] fontWeights;

		public float normalStyle;

		public float normalSpacingOffset;

		public float boldStyle = 0.75f;

		public float boldSpacing = 7f;

		public byte italicStyle = 35;

		public byte tabSize = 10;

		internal bool IsFontAssetLookupTablesDirty;

		[SerializeField]
		private FaceInfo_Legacy m_fontInfo;

		[SerializeField]
		internal List<TMP_Glyph> m_glyphInfoList;

		[SerializeField]
		[FormerlySerializedAs("m_kerningInfo")]
		internal KerningTable m_KerningTable = new KerningTable();

		[SerializeField]
		private List<TMP_FontAsset> fallbackFontAssets;

		[SerializeField]
		public Texture2D atlas;

		private static readonly List<WeakReference<TMP_FontAsset>> s_CallbackInstances = new List<WeakReference<TMP_FontAsset>>();

		private static ProfilerMarker k_ReadFontAssetDefinitionMarker = new ProfilerMarker("TMP.ReadFontAssetDefinition");

		private static ProfilerMarker k_AddSynthesizedCharactersMarker = new ProfilerMarker("TMP.AddSynthesizedCharacters");

		private static ProfilerMarker k_TryAddGlyphMarker = new ProfilerMarker("TMP.TryAddGlyph");

		private static ProfilerMarker k_TryAddCharacterMarker = new ProfilerMarker("TMP.TryAddCharacter");

		private static ProfilerMarker k_TryAddCharactersMarker = new ProfilerMarker("TMP.TryAddCharacters");

		private static ProfilerMarker k_UpdateLigatureSubstitutionRecordsMarker = new ProfilerMarker("TMP.UpdateLigatureSubstitutionRecords");

		private static ProfilerMarker k_UpdateGlyphAdjustmentRecordsMarker = new ProfilerMarker("TMP.UpdateGlyphAdjustmentRecords");

		private static ProfilerMarker k_UpdateDiacriticalMarkAdjustmentRecordsMarker = new ProfilerMarker("TMP.UpdateDiacriticalAdjustmentRecords");

		private static ProfilerMarker k_ClearFontAssetDataMarker = new ProfilerMarker("TMP.ClearFontAssetData");

		private static ProfilerMarker k_UpdateFontAssetDataMarker = new ProfilerMarker("TMP.UpdateFontAssetData");

		private static string s_DefaultMaterialSuffix = " Atlas Material";

		private static HashSet<int> k_SearchedFontAssetLookup;

		private static List<TMP_FontAsset> k_FontAssets_FontFeaturesUpdateQueue = new List<TMP_FontAsset>();

		private static HashSet<int> k_FontAssets_FontFeaturesUpdateQueueLookup = new HashSet<int>();

		private static List<Texture2D> k_FontAssets_AtlasTexturesUpdateQueue = new List<Texture2D>();

		private static HashSet<int> k_FontAssets_AtlasTexturesUpdateQueueLookup = new HashSet<int>();

		private List<Glyph> m_GlyphsToRender = new List<Glyph>();

		private List<Glyph> m_GlyphsRendered = new List<Glyph>();

		private List<uint> m_GlyphIndexList = new List<uint>();

		private List<uint> m_GlyphIndexListNewlyAdded = new List<uint>();

		internal List<uint> m_GlyphsToAdd = new List<uint>();

		internal HashSet<uint> m_GlyphsToAddLookup = new HashSet<uint>();

		internal List<TMP_Character> m_CharactersToAdd = new List<TMP_Character>();

		internal HashSet<uint> m_CharactersToAddLookup = new HashSet<uint>();

		internal List<uint> s_MissingCharacterList = new List<uint>();

		internal HashSet<uint> m_MissingUnicodesFromFontFile = new HashSet<uint>();

		internal static uint[] k_GlyphIndexArray;
	}
}
