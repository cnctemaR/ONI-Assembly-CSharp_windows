using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.TextCore.LowLevel;

namespace UnityEngine.TextCore
{
	[Serializable]
	internal class FontAsset : ScriptableObject
	{
		public string version
		{
			get
			{
				return this.m_Version;
			}
			set
			{
				this.m_Version = value;
			}
		}

		public int hashCode
		{
			get
			{
				return this.m_HashCode;
			}
			set
			{
				this.m_HashCode = value;
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

		public Font sourceFontFile
		{
			get
			{
				return this.m_SourceFontFile;
			}
		}

		public FontAsset.AtlasPopulationMode atlasPopulationMode
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

		public List<Glyph> glyphTable
		{
			get
			{
				return this.m_GlyphTable;
			}
			set
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
			set
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

		public int atlasWidth
		{
			get
			{
				return this.m_AtlasWidth;
			}
			set
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
			set
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
			set
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
			set
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

		public Material material
		{
			get
			{
				return this.m_Material;
			}
			set
			{
				this.m_Material = value;
				this.m_MaterialHashCode = TextUtilities.GetHashCodeCaseInSensitive(this.m_Material.name);
			}
		}

		public int materialHashCode
		{
			get
			{
				return this.m_MaterialHashCode;
			}
			set
			{
				bool flag = this.m_MaterialHashCode == 0;
				if (flag)
				{
					this.m_MaterialHashCode = TextUtilities.GetHashCodeCaseInSensitive(this.m_Material.name);
				}
				this.m_MaterialHashCode = value;
			}
		}

		public KerningTable kerningTable
		{
			get
			{
				return this.m_KerningTable;
			}
			set
			{
				this.m_KerningTable = value;
			}
		}

		public Dictionary<int, KerningPair> kerningLookupDictionary
		{
			get
			{
				return this.m_KerningLookupDictionary;
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

		public FontAssetCreationSettings fontAssetCreationSettings
		{
			get
			{
				return this.m_FontAssetCreationSettings;
			}
			set
			{
				this.m_FontAssetCreationSettings = value;
			}
		}

		public FontWeights[] fontWeightTable
		{
			get
			{
				return this.m_FontWeightTable;
			}
			set
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

		public static FontAsset CreateFontAsset(Font font)
		{
			return FontAsset.CreateFontAsset(font, 90, 9, GlyphRenderMode.SDFAA, 1024, 1024, FontAsset.AtlasPopulationMode.Dynamic);
		}

		public static FontAsset CreateFontAsset(Font font, int samplingPointSize, int atlasPadding, GlyphRenderMode renderMode, int atlasWidth, int atlasHeight, FontAsset.AtlasPopulationMode atlasPopulationMode = FontAsset.AtlasPopulationMode.Dynamic)
		{
			FontAsset fontAsset = ScriptableObject.CreateInstance<FontAsset>();
			FontEngine.InitializeFontEngine();
			FontEngine.LoadFontFace(font, samplingPointSize);
			fontAsset.faceInfo = FontEngine.GetFaceInfo();
			bool flag = atlasPopulationMode == FontAsset.AtlasPopulationMode.Dynamic;
			if (flag)
			{
				fontAsset.m_SourceFontFile = font;
			}
			fontAsset.atlasPopulationMode = atlasPopulationMode;
			fontAsset.atlasWidth = atlasWidth;
			fontAsset.atlasHeight = atlasHeight;
			fontAsset.atlasPadding = atlasPadding;
			fontAsset.atlasRenderMode = renderMode;
			fontAsset.atlasTextures = new Texture2D[1];
			Texture2D texture2D = new Texture2D(0, 0, TextureFormat.Alpha8, false);
			fontAsset.atlasTextures[0] = texture2D;
			bool flag2 = (renderMode & (GlyphRenderMode)16) == (GlyphRenderMode)16;
			int num;
			if (flag2)
			{
				num = 0;
				Material material = new Material(ShaderUtilities.ShaderRef_MobileBitmap);
				material.SetTexture(ShaderUtilities.ID_MainTex, texture2D);
				material.SetFloat(ShaderUtilities.ID_TextureWidth, (float)atlasWidth);
				material.SetFloat(ShaderUtilities.ID_TextureHeight, (float)atlasHeight);
				fontAsset.material = material;
			}
			else
			{
				num = 1;
				Material material2 = new Material(ShaderUtilities.ShaderRef_MobileSDF);
				material2.SetTexture(ShaderUtilities.ID_MainTex, texture2D);
				material2.SetFloat(ShaderUtilities.ID_TextureWidth, (float)atlasWidth);
				material2.SetFloat(ShaderUtilities.ID_TextureHeight, (float)atlasHeight);
				material2.SetFloat(ShaderUtilities.ID_GradientScale, (float)(atlasPadding + num));
				material2.SetFloat(ShaderUtilities.ID_WeightNormal, fontAsset.regularStyleWeight);
				material2.SetFloat(ShaderUtilities.ID_WeightBold, fontAsset.boldStyleWeight);
				fontAsset.material = material2;
			}
			fontAsset.freeGlyphRects = new List<GlyphRect>
			{
				new GlyphRect(0, 0, atlasWidth - num, atlasHeight - num)
			};
			fontAsset.usedGlyphRects = new List<GlyphRect>();
			fontAsset.ReadFontAssetDefinition();
			return fontAsset;
		}

		private void Awake()
		{
			this.m_HashCode = TextUtilities.GetHashCodeCaseInSensitive(base.name);
			bool flag = this.m_Material != null;
			if (flag)
			{
				this.m_MaterialHashCode = TextUtilities.GetHashCodeCaseInSensitive(this.m_Material.name);
			}
		}

		internal void InitializeDictionaryLookupTables()
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
			for (int i = 0; i < this.m_GlyphTable.Count; i++)
			{
				Glyph glyph = this.m_GlyphTable[i];
				uint index = glyph.index;
				bool flag2 = !this.m_GlyphLookupDictionary.ContainsKey(index);
				if (flag2)
				{
					this.m_GlyphLookupDictionary.Add(index, glyph);
				}
			}
			bool flag3 = this.m_CharacterLookupDictionary == null;
			if (flag3)
			{
				this.m_CharacterLookupDictionary = new Dictionary<uint, Character>();
			}
			else
			{
				this.m_CharacterLookupDictionary.Clear();
			}
			for (int j = 0; j < this.m_CharacterTable.Count; j++)
			{
				Character character = this.m_CharacterTable[j];
				uint unicode = character.unicode;
				bool flag4 = !this.m_CharacterLookupDictionary.ContainsKey(unicode);
				if (flag4)
				{
					this.m_CharacterLookupDictionary.Add(unicode, character);
				}
				bool flag5 = this.m_GlyphLookupDictionary.ContainsKey(character.glyphIndex);
				if (flag5)
				{
					character.glyph = this.m_GlyphLookupDictionary[character.glyphIndex];
				}
			}
			bool flag6 = this.m_KerningLookupDictionary == null;
			if (flag6)
			{
				this.m_KerningLookupDictionary = new Dictionary<int, KerningPair>();
			}
			else
			{
				this.m_KerningLookupDictionary.Clear();
			}
			List<KerningPair> kerningPairs = this.m_KerningTable.kerningPairs;
			bool flag7 = kerningPairs != null;
			if (flag7)
			{
				for (int k = 0; k < kerningPairs.Count; k++)
				{
					KerningPair kerningPair = kerningPairs[k];
					KerningPairKey kerningPairKey = new KerningPairKey(kerningPair.firstGlyph, kerningPair.secondGlyph);
					bool flag8 = !this.m_KerningLookupDictionary.ContainsKey((int)kerningPairKey.key);
					if (flag8)
					{
						this.m_KerningLookupDictionary.Add((int)kerningPairKey.key, kerningPair);
					}
					else
					{
						bool flag9 = !TextSettings.warningsDisabled;
						if (flag9)
						{
							Debug.LogWarning(string.Concat(new string[]
							{
								"Kerning Key for [",
								kerningPairKey.ascii_Left.ToString(),
								"] and [",
								kerningPairKey.ascii_Right.ToString(),
								"] already exists."
							}));
						}
					}
				}
			}
		}

		internal void ReadFontAssetDefinition()
		{
			this.InitializeDictionaryLookupTables();
			bool flag = !this.m_CharacterLookupDictionary.ContainsKey(9U);
			if (flag)
			{
				Glyph glyph = new Glyph(0U, new GlyphMetrics(0f, 0f, 0f, 0f, this.m_FaceInfo.tabWidth * (float)this.tabMultiple), GlyphRect.zero, 1f, 0);
				this.m_CharacterLookupDictionary.Add(9U, new Character(9U, glyph));
			}
			bool flag2 = !this.m_CharacterLookupDictionary.ContainsKey(10U);
			if (flag2)
			{
				Glyph glyph2 = new Glyph(0U, new GlyphMetrics(10f, 0f, 0f, 0f, 0f), GlyphRect.zero, 1f, 0);
				this.m_CharacterLookupDictionary.Add(10U, new Character(10U, glyph2));
				bool flag3 = !this.m_CharacterLookupDictionary.ContainsKey(13U);
				if (flag3)
				{
					this.m_CharacterLookupDictionary.Add(13U, new Character(13U, glyph2));
				}
			}
			bool flag4 = !this.m_CharacterLookupDictionary.ContainsKey(8203U);
			if (flag4)
			{
				Glyph glyph3 = new Glyph(0U, new GlyphMetrics(0f, 0f, 0f, 0f, 0f), GlyphRect.zero, 1f, 0);
				this.m_CharacterLookupDictionary.Add(8203U, new Character(8203U, glyph3));
			}
			bool flag5 = !this.m_CharacterLookupDictionary.ContainsKey(8288U);
			if (flag5)
			{
				Glyph glyph4 = new Glyph(0U, new GlyphMetrics(0f, 0f, 0f, 0f, 0f), GlyphRect.zero, 1f, 0);
				this.m_CharacterLookupDictionary.Add(8288U, new Character(8288U, glyph4));
			}
			bool flag6 = this.m_FaceInfo.capLine == 0f && this.m_CharacterLookupDictionary.ContainsKey(72U);
			if (flag6)
			{
				this.m_FaceInfo.capLine = this.m_CharacterLookupDictionary[72U].glyph.metrics.horizontalBearingY;
			}
			bool flag7 = this.m_FaceInfo.scale == 0f;
			if (flag7)
			{
				this.m_FaceInfo.scale = 1f;
			}
			bool flag8 = this.m_FaceInfo.strikethroughOffset == 0f;
			if (flag8)
			{
				this.m_FaceInfo.strikethroughOffset = this.m_FaceInfo.capLine / 2.5f;
			}
			bool flag9 = this.m_AtlasPadding == 0;
			if (flag9)
			{
				bool flag10 = this.material.HasProperty(ShaderUtilities.ID_GradientScale);
				if (flag10)
				{
					this.m_AtlasPadding = (int)this.material.GetFloat(ShaderUtilities.ID_GradientScale) - 1;
				}
			}
			this.m_HashCode = TextUtilities.GetHashCodeCaseInSensitive(base.name);
			this.m_MaterialHashCode = TextUtilities.GetHashCodeCaseInSensitive(this.material.name);
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

		internal void SortGlyphAndCharacterTables()
		{
			this.SortGlyphTable();
			this.SortCharacterTable();
		}

		internal bool HasCharacter(int character)
		{
			bool flag = this.m_CharacterLookupDictionary == null;
			return !flag && this.m_CharacterLookupDictionary.ContainsKey((uint)character);
		}

		internal bool HasCharacter(char character)
		{
			bool flag = this.m_CharacterLookupDictionary == null;
			return !flag && this.m_CharacterLookupDictionary.ContainsKey((uint)character);
		}

		internal bool HasCharacter(char character, bool searchFallbacks)
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
			bool flag3 = this.m_CharacterLookupDictionary.ContainsKey((uint)character);
			bool flag4;
			if (flag3)
			{
				flag4 = true;
			}
			else
			{
				if (searchFallbacks)
				{
					bool flag5 = this.fallbackFontAssetTable != null && this.fallbackFontAssetTable.Count > 0;
					if (flag5)
					{
						int num = 0;
						while (num < this.fallbackFontAssetTable.Count && this.fallbackFontAssetTable[num] != null)
						{
							bool flag6 = this.fallbackFontAssetTable[num].HasCharacter_Internal(character, searchFallbacks);
							if (flag6)
							{
								return true;
							}
							num++;
						}
					}
					bool flag7 = TextSettings.fallbackFontAssets != null && TextSettings.fallbackFontAssets.Count > 0;
					if (flag7)
					{
						int num2 = 0;
						while (num2 < TextSettings.fallbackFontAssets.Count && TextSettings.fallbackFontAssets[num2] != null)
						{
							bool flag8 = TextSettings.fallbackFontAssets[num2].m_CharacterLookupDictionary == null;
							if (flag8)
							{
								TextSettings.fallbackFontAssets[num2].ReadFontAssetDefinition();
							}
							bool flag9 = TextSettings.fallbackFontAssets[num2].m_CharacterLookupDictionary != null && TextSettings.fallbackFontAssets[num2].HasCharacter_Internal(character, searchFallbacks);
							if (flag9)
							{
								return true;
							}
							num2++;
						}
					}
					bool flag10 = TextSettings.defaultFontAsset != null;
					if (flag10)
					{
						bool flag11 = TextSettings.defaultFontAsset.m_CharacterLookupDictionary == null;
						if (flag11)
						{
							TextSettings.defaultFontAsset.ReadFontAssetDefinition();
						}
						bool flag12 = TextSettings.defaultFontAsset.m_CharacterLookupDictionary != null && TextSettings.defaultFontAsset.HasCharacter_Internal(character, searchFallbacks);
						if (flag12)
						{
							return true;
						}
					}
				}
				flag4 = false;
			}
			return flag4;
		}

		private bool HasCharacter_Internal(char character, bool searchFallbacks)
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
			bool flag3 = this.m_CharacterLookupDictionary.ContainsKey((uint)character);
			bool flag4;
			if (flag3)
			{
				flag4 = true;
			}
			else
			{
				if (searchFallbacks)
				{
					bool flag5 = this.fallbackFontAssetTable != null && this.fallbackFontAssetTable.Count > 0;
					if (flag5)
					{
						int num = 0;
						while (num < this.fallbackFontAssetTable.Count && this.fallbackFontAssetTable[num] != null)
						{
							bool flag6 = this.fallbackFontAssetTable[num].HasCharacter_Internal(character, searchFallbacks);
							if (flag6)
							{
								return true;
							}
							num++;
						}
					}
				}
				flag4 = false;
			}
			return flag4;
		}

		internal bool HasCharacters(string text, out List<char> missingCharacters)
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
				flag2 = missingCharacters.Count == 0;
			}
			return flag2;
		}

		internal bool HasCharacters(string text)
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

		internal static string GetCharacters(FontAsset fontAsset)
		{
			string text = string.Empty;
			for (int i = 0; i < fontAsset.characterTable.Count; i++)
			{
				text += ((char)fontAsset.characterTable[i].unicode).ToString();
			}
			return text;
		}

		internal static int[] GetCharactersArray(FontAsset fontAsset)
		{
			int[] array = new int[fontAsset.characterTable.Count];
			for (int i = 0; i < fontAsset.characterTable.Count; i++)
			{
				array[i] = (int)fontAsset.characterTable[i].unicode;
			}
			return array;
		}

		internal Character AddCharacter_Internal(uint unicode, Glyph glyph)
		{
			bool flag = this.m_CharacterLookupDictionary.ContainsKey(unicode);
			Character character;
			if (flag)
			{
				character = this.m_CharacterLookupDictionary[unicode];
			}
			else
			{
				uint index = glyph.index;
				bool flag2 = !this.m_GlyphLookupDictionary.ContainsKey(index);
				if (flag2)
				{
					bool flag3 = glyph.glyphRect.width == 0 || glyph.glyphRect.height == 0;
					if (flag3)
					{
						this.m_GlyphTable.Add(glyph);
					}
					else
					{
						bool flag4 = !FontEngine.TryPackGlyphInAtlas(glyph, this.m_AtlasPadding, GlyphPackingMode.ContactPointRule, this.m_AtlasRenderMode, this.m_AtlasWidth, this.m_AtlasHeight, this.m_FreeGlyphRects, this.m_UsedGlyphRects);
						if (flag4)
						{
							return null;
						}
						this.m_GlyphsToRender.Add(glyph);
					}
				}
				Character character2 = new Character(unicode, glyph);
				this.m_CharacterTable.Add(character2);
				this.m_CharacterLookupDictionary.Add(unicode, character2);
				this.UpdateAtlasTexture();
				character = character2;
			}
			return character;
		}

		internal bool TryAddCharacter(uint unicode, out Character character)
		{
			bool flag = this.m_CharacterLookupDictionary.ContainsKey(unicode);
			bool flag2;
			if (flag)
			{
				character = this.m_CharacterLookupDictionary[unicode];
				flag2 = true;
			}
			else
			{
				character = null;
				bool flag3 = FontEngine.LoadFontFace(this.sourceFontFile, this.m_FaceInfo.pointSize) > FontEngineError.Success;
				if (flag3)
				{
					flag2 = false;
				}
				else
				{
					uint glyphIndex = FontEngine.GetGlyphIndex(unicode);
					bool flag4 = glyphIndex == 0U;
					if (flag4)
					{
						flag2 = false;
					}
					else
					{
						bool flag5 = this.m_GlyphLookupDictionary.ContainsKey(glyphIndex);
						if (flag5)
						{
							character = new Character(unicode, this.m_GlyphLookupDictionary[glyphIndex]);
							this.m_CharacterTable.Add(character);
							this.m_CharacterLookupDictionary.Add(unicode, character);
							flag2 = true;
						}
						else
						{
							bool flag6 = this.m_AtlasTextures[this.m_AtlasTextureIndex].width == 0 || this.m_AtlasTextures[this.m_AtlasTextureIndex].height == 0;
							if (flag6)
							{
								this.m_AtlasTextures[this.m_AtlasTextureIndex].Resize(this.m_AtlasWidth, this.m_AtlasHeight);
								FontEngine.ResetAtlasTexture(this.m_AtlasTextures[this.m_AtlasTextureIndex]);
							}
							Glyph glyph;
							bool flag7 = FontEngine.TryAddGlyphToTexture(glyphIndex, this.m_AtlasPadding, GlyphPackingMode.BestShortSideFit, this.m_FreeGlyphRects, this.m_UsedGlyphRects, this.m_AtlasRenderMode, this.m_AtlasTextures[this.m_AtlasTextureIndex], out glyph);
							if (flag7)
							{
								this.m_GlyphTable.Add(glyph);
								this.m_GlyphLookupDictionary.Add(glyphIndex, glyph);
								character = new Character(unicode, glyph);
								this.m_CharacterTable.Add(character);
								this.m_CharacterLookupDictionary.Add(unicode, character);
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

		internal void UpdateAtlasTexture()
		{
			bool flag = this.m_GlyphsToRender.Count == 0;
			if (!flag)
			{
				FontEngine.RenderGlyphsToTexture(this.m_GlyphsToRender, this.m_AtlasPadding, this.m_AtlasRenderMode, this.m_AtlasTextures[this.m_AtlasTextureIndex]);
				this.m_AtlasTextures[this.m_AtlasTextureIndex].Apply(false, false);
				for (int i = 0; i < this.m_GlyphsToRender.Count; i++)
				{
					Glyph glyph = this.m_GlyphsToRender[i];
					glyph.atlasIndex = this.m_AtlasTextureIndex;
					this.m_GlyphTable.Add(glyph);
					this.m_GlyphLookupDictionary.Add(glyph.index, glyph);
				}
				this.m_GlyphsPacked.Clear();
				this.m_GlyphsToRender.Clear();
				bool flag2 = this.m_GlyphsToPack.Count > 0;
				if (flag2)
				{
				}
			}
		}

		public bool TryAddCharacters(uint[] unicodes)
		{
			bool flag = false;
			this.m_GlyphIndexes.Clear();
			this.s_GlyphLookupMap.Clear();
			FontEngine.LoadFontFace(this.m_SourceFontFile, this.m_FaceInfo.pointSize);
			foreach (uint num in unicodes)
			{
				bool flag2 = this.m_CharacterLookupDictionary.ContainsKey(num);
				if (!flag2)
				{
					uint glyphIndex = FontEngine.GetGlyphIndex(num);
					bool flag3 = glyphIndex == 0U;
					if (flag3)
					{
						flag = true;
					}
					else
					{
						bool flag4 = this.m_GlyphLookupDictionary.ContainsKey(glyphIndex);
						if (flag4)
						{
							Character character = new Character(num, this.m_GlyphLookupDictionary[glyphIndex]);
							this.m_CharacterTable.Add(character);
							this.m_CharacterLookupDictionary.Add(num, character);
						}
						else
						{
							bool flag5 = this.s_GlyphLookupMap.ContainsKey(glyphIndex);
							if (flag5)
							{
								this.s_GlyphLookupMap[glyphIndex].Add(num);
							}
							else
							{
								this.s_GlyphLookupMap.Add(glyphIndex, new List<uint> { num });
								this.m_GlyphIndexes.Add(glyphIndex);
							}
						}
					}
				}
			}
			bool flag6 = this.m_GlyphIndexes == null || this.m_GlyphIndexes.Count == 0;
			bool flag7;
			if (flag6)
			{
				flag7 = true;
			}
			else
			{
				bool flag8 = this.m_AtlasTextures[this.m_AtlasTextureIndex].width == 0 || this.m_AtlasTextures[this.m_AtlasTextureIndex].height == 0;
				if (flag8)
				{
					this.m_AtlasTextures[this.m_AtlasTextureIndex].Resize(this.m_AtlasWidth, this.m_AtlasHeight);
					FontEngine.ResetAtlasTexture(this.m_AtlasTextures[this.m_AtlasTextureIndex]);
				}
				Glyph[] array;
				bool flag9 = FontEngine.TryAddGlyphsToTexture(this.m_GlyphIndexes, this.m_AtlasPadding, GlyphPackingMode.BestShortSideFit, this.m_FreeGlyphRects, this.m_UsedGlyphRects, this.m_AtlasRenderMode, this.m_AtlasTextures[this.m_AtlasTextureIndex], out array);
				int num2 = 0;
				while (num2 < array.Length && array[num2] != null)
				{
					Glyph glyph = array[num2];
					uint index = glyph.index;
					this.m_GlyphTable.Add(glyph);
					this.m_GlyphLookupDictionary.Add(index, glyph);
					foreach (uint num3 in this.s_GlyphLookupMap[index])
					{
						Character character2 = new Character(num3, glyph);
						this.m_CharacterTable.Add(character2);
						this.m_CharacterLookupDictionary.Add(num3, character2);
					}
					num2++;
				}
				flag7 = flag9 && !flag;
			}
			return flag7;
		}

		public bool TryAddCharacters(string characters)
		{
			bool flag = string.IsNullOrEmpty(characters) || this.m_AtlasPopulationMode == FontAsset.AtlasPopulationMode.Static;
			bool flag3;
			if (flag)
			{
				bool flag2 = this.m_AtlasPopulationMode == FontAsset.AtlasPopulationMode.Static;
				if (flag2)
				{
					Debug.LogWarning("Unable to add characters to font asset [" + base.name + "] because its AtlasPopulationMode is set to Static.", this);
				}
				else
				{
					Debug.LogWarning("Unable to add characters to font asset [" + base.name + "] because the provided character list is Null or Empty.", this);
				}
				flag3 = false;
			}
			else
			{
				bool flag4 = FontEngine.LoadFontFace(this.m_SourceFontFile, this.m_FaceInfo.pointSize) > FontEngineError.Success;
				if (flag4)
				{
					flag3 = false;
				}
				else
				{
					bool flag5 = false;
					int length = characters.Length;
					this.m_GlyphIndexes.Clear();
					this.s_GlyphLookupMap.Clear();
					for (int i = 0; i < length; i++)
					{
						uint num = (uint)characters[i];
						bool flag6 = this.m_CharacterLookupDictionary.ContainsKey(num);
						if (!flag6)
						{
							uint glyphIndex = FontEngine.GetGlyphIndex(num);
							bool flag7 = glyphIndex == 0U;
							if (flag7)
							{
								flag5 = true;
							}
							else
							{
								bool flag8 = this.m_GlyphLookupDictionary.ContainsKey(glyphIndex);
								if (flag8)
								{
									Character character = new Character(num, this.m_GlyphLookupDictionary[glyphIndex]);
									this.m_CharacterTable.Add(character);
									this.m_CharacterLookupDictionary.Add(num, character);
								}
								else
								{
									bool flag9 = this.s_GlyphLookupMap.ContainsKey(glyphIndex);
									if (flag9)
									{
										bool flag10 = this.s_GlyphLookupMap[glyphIndex].Contains(num);
										if (!flag10)
										{
											this.s_GlyphLookupMap[glyphIndex].Add(num);
										}
									}
									else
									{
										this.s_GlyphLookupMap.Add(glyphIndex, new List<uint> { num });
										this.m_GlyphIndexes.Add(glyphIndex);
									}
								}
							}
						}
					}
					bool flag11 = this.m_GlyphIndexes == null || this.m_GlyphIndexes.Count == 0;
					if (flag11)
					{
						Debug.LogWarning("No characters will be added to font asset [" + base.name + "] either because they are already present in the font asset or missing from the font file.");
						flag3 = true;
					}
					else
					{
						bool flag12 = this.m_AtlasTextures[this.m_AtlasTextureIndex].width == 0 || this.m_AtlasTextures[this.m_AtlasTextureIndex].height == 0;
						if (flag12)
						{
							this.m_AtlasTextures[this.m_AtlasTextureIndex].Resize(this.m_AtlasWidth, this.m_AtlasHeight);
							FontEngine.ResetAtlasTexture(this.m_AtlasTextures[this.m_AtlasTextureIndex]);
						}
						Glyph[] array;
						bool flag13 = FontEngine.TryAddGlyphsToTexture(this.m_GlyphIndexes, this.m_AtlasPadding, GlyphPackingMode.BestShortSideFit, this.m_FreeGlyphRects, this.m_UsedGlyphRects, this.m_AtlasRenderMode, this.m_AtlasTextures[this.m_AtlasTextureIndex], out array);
						int num2 = 0;
						while (num2 < array.Length && array[num2] != null)
						{
							Glyph glyph = array[num2];
							uint index = glyph.index;
							this.m_GlyphTable.Add(glyph);
							this.m_GlyphLookupDictionary.Add(index, glyph);
							List<uint> list = this.s_GlyphLookupMap[index];
							int count = list.Count;
							for (int j = 0; j < count; j++)
							{
								uint num3 = list[j];
								Character character2 = new Character(num3, glyph);
								this.m_CharacterTable.Add(character2);
								this.m_CharacterLookupDictionary.Add(num3, character2);
							}
							num2++;
						}
						flag3 = flag13 && !flag5;
					}
				}
			}
			return flag3;
		}

		internal void ClearFontAssetData()
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
				this.m_FreeGlyphRects = new List<GlyphRect>
				{
					new GlyphRect(0, 0, this.m_AtlasWidth - num, this.m_AtlasHeight - num)
				};
			}
			bool flag5 = this.m_GlyphsToPack != null;
			if (flag5)
			{
				this.m_GlyphsToPack.Clear();
			}
			bool flag6 = this.m_GlyphsPacked != null;
			if (flag6)
			{
				this.m_GlyphsPacked.Clear();
			}
			bool flag7 = this.m_KerningTable != null && this.m_KerningTable.kerningPairs != null;
			if (flag7)
			{
				this.m_KerningTable.kerningPairs.Clear();
			}
			this.m_AtlasTextureIndex = 0;
			bool flag8 = this.m_AtlasTextures != null;
			if (flag8)
			{
				for (int i = 0; i < this.m_AtlasTextures.Length; i++)
				{
					Texture2D texture2D = this.m_AtlasTextures[i];
					bool flag9 = texture2D == null;
					if (!flag9)
					{
						bool flag10 = texture2D.width != this.m_AtlasWidth || texture2D.height != this.m_AtlasHeight;
						if (flag10)
						{
							texture2D.Resize(this.m_AtlasWidth, this.m_AtlasHeight, TextureFormat.Alpha8, false);
						}
						FontEngine.ResetAtlasTexture(texture2D);
						texture2D.Apply();
						bool flag11 = i == 0;
						if (flag11)
						{
							this.m_AtlasTexture = texture2D;
						}
						this.m_AtlasTextures[i] = texture2D;
					}
				}
			}
			this.ReadFontAssetDefinition();
		}

		[SerializeField]
		private string m_Version = "1.1.0";

		[SerializeField]
		private int m_HashCode;

		[SerializeField]
		private FaceInfo m_FaceInfo;

		[SerializeField]
		internal string m_SourceFontFileGUID;

		[SerializeField]
		internal Font m_SourceFontFile_EditorRef;

		[SerializeField]
		internal Font m_SourceFontFile;

		[SerializeField]
		private FontAsset.AtlasPopulationMode m_AtlasPopulationMode;

		[SerializeField]
		private List<Glyph> m_GlyphTable = new List<Glyph>();

		private Dictionary<uint, Glyph> m_GlyphLookupDictionary;

		[SerializeField]
		private List<Character> m_CharacterTable = new List<Character>();

		private Dictionary<uint, Character> m_CharacterLookupDictionary;

		private Texture2D m_AtlasTexture;

		[SerializeField]
		private Texture2D[] m_AtlasTextures;

		[SerializeField]
		internal int m_AtlasTextureIndex;

		[SerializeField]
		private int m_AtlasWidth;

		[SerializeField]
		private int m_AtlasHeight;

		[SerializeField]
		private int m_AtlasPadding;

		[SerializeField]
		private GlyphRenderMode m_AtlasRenderMode;

		[SerializeField]
		private List<GlyphRect> m_UsedGlyphRects;

		[SerializeField]
		private List<GlyphRect> m_FreeGlyphRects;

		private List<uint> m_GlyphIndexes = new List<uint>();

		private Dictionary<uint, List<uint>> s_GlyphLookupMap = new Dictionary<uint, List<uint>>();

		[SerializeField]
		private Material m_Material;

		[SerializeField]
		internal int m_MaterialHashCode;

		[SerializeField]
		internal KerningTable m_KerningTable = new KerningTable();

		private Dictionary<int, KerningPair> m_KerningLookupDictionary;

		[SerializeField]
		internal KerningPair m_EmptyKerningPair;

		[SerializeField]
		internal List<FontAsset> m_FallbackFontAssetTable;

		[SerializeField]
		internal FontAssetCreationSettings m_FontAssetCreationSettings;

		[SerializeField]
		internal FontWeights[] m_FontWeightTable = new FontWeights[10];

		[SerializeField]
		private float m_RegularStyleWeight = 0f;

		[SerializeField]
		private float m_RegularStyleSpacing = 0f;

		[SerializeField]
		private float m_BoldStyleWeight = 0.75f;

		[SerializeField]
		private float m_BoldStyleSpacing = 7f;

		[SerializeField]
		private byte m_ItalicStyleSlant = 35;

		[SerializeField]
		private byte m_TabMultiple = 10;

		internal bool m_IsFontAssetLookupTablesDirty = false;

		private List<Glyph> m_GlyphsToPack = new List<Glyph>();

		private List<Glyph> m_GlyphsPacked = new List<Glyph>();

		private List<Glyph> m_GlyphsToRender = new List<Glyph>();

		internal enum AtlasPopulationMode
		{
			Static,
			Dynamic
		}
	}
}
