using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TMPro
{
	[Serializable]
	public class TMP_FontAsset : TMP_Asset
	{
		public static TMP_FontAsset defaultFontAsset
		{
			get
			{
				if (TMP_FontAsset.s_defaultFontAsset == null)
				{
					TMP_FontAsset.s_defaultFontAsset = Resources.Load<TMP_FontAsset>("Fonts & Materials/ARIAL SDF");
				}
				return TMP_FontAsset.s_defaultFontAsset;
			}
		}

		public FaceInfo fontInfo
		{
			get
			{
				return this.m_fontInfo;
			}
		}

		public Dictionary<int, TMP_Glyph> characterDictionary
		{
			get
			{
				if (this.m_characterDictionary == null)
				{
					this.ReadFontDefinition();
				}
				return this.m_characterDictionary;
			}
		}

		public Dictionary<int, KerningPair> kerningDictionary
		{
			get
			{
				return this.m_kerningDictionary;
			}
		}

		public KerningTable kerningInfo
		{
			get
			{
				return this.m_kerningInfo;
			}
		}

		private void OnEnable()
		{
		}

		private void OnDisable()
		{
		}

		public void AddFaceInfo(FaceInfo faceInfo)
		{
			this.m_fontInfo = faceInfo;
		}

		public void AddGlyphInfo(TMP_Glyph[] glyphInfo)
		{
			this.m_glyphInfoList = new List<TMP_Glyph>();
			int num = glyphInfo.Length;
			this.m_fontInfo.CharacterCount = num;
			this.m_characterSet = new int[num];
			for (int i = 0; i < num; i++)
			{
				TMP_Glyph tmp_Glyph = new TMP_Glyph();
				tmp_Glyph.id = glyphInfo[i].id;
				tmp_Glyph.x = glyphInfo[i].x;
				tmp_Glyph.y = glyphInfo[i].y;
				tmp_Glyph.width = glyphInfo[i].width;
				tmp_Glyph.height = glyphInfo[i].height;
				tmp_Glyph.xOffset = glyphInfo[i].xOffset;
				tmp_Glyph.yOffset = glyphInfo[i].yOffset;
				tmp_Glyph.xAdvance = glyphInfo[i].xAdvance;
				this.m_glyphInfoList.Add(tmp_Glyph);
				this.m_characterSet[i] = tmp_Glyph.id;
			}
			this.m_glyphInfoList = this.m_glyphInfoList.OrderBy<TMP_Glyph, int>((TMP_Glyph s) => s.id).ToList<TMP_Glyph>();
		}

		public void AddKerningInfo(KerningTable kerningTable)
		{
			this.m_kerningInfo = kerningTable;
		}

		public void ReadFontDefinition()
		{
			if (this.m_fontInfo == null)
			{
				return;
			}
			this.m_characterDictionary = new Dictionary<int, TMP_Glyph>();
			foreach (TMP_Glyph tmp_Glyph in this.m_glyphInfoList)
			{
				if (!this.m_characterDictionary.ContainsKey(tmp_Glyph.id))
				{
					this.m_characterDictionary.Add(tmp_Glyph.id, tmp_Glyph);
				}
			}
			TMP_Glyph tmp_Glyph2 = new TMP_Glyph();
			if (this.m_characterDictionary.ContainsKey(32))
			{
				this.m_characterDictionary[32].width = this.m_characterDictionary[32].xAdvance;
				this.m_characterDictionary[32].height = this.m_fontInfo.Ascender - this.m_fontInfo.Descender;
				this.m_characterDictionary[32].yOffset = this.m_fontInfo.Ascender;
			}
			else
			{
				tmp_Glyph2 = new TMP_Glyph();
				tmp_Glyph2.id = 32;
				tmp_Glyph2.x = 0f;
				tmp_Glyph2.y = 0f;
				tmp_Glyph2.width = this.m_fontInfo.Ascender / 5f;
				tmp_Glyph2.height = this.m_fontInfo.Ascender - this.m_fontInfo.Descender;
				tmp_Glyph2.xOffset = 0f;
				tmp_Glyph2.yOffset = this.m_fontInfo.Ascender;
				tmp_Glyph2.xAdvance = this.m_fontInfo.PointSize / 4f;
				this.m_characterDictionary.Add(32, tmp_Glyph2);
			}
			if (!this.m_characterDictionary.ContainsKey(160))
			{
				tmp_Glyph2 = TMP_Glyph.Clone(this.m_characterDictionary[32]);
				this.m_characterDictionary.Add(160, tmp_Glyph2);
			}
			if (!this.m_characterDictionary.ContainsKey(8203))
			{
				tmp_Glyph2 = TMP_Glyph.Clone(this.m_characterDictionary[32]);
				tmp_Glyph2.width = 0f;
				tmp_Glyph2.xAdvance = 0f;
				this.m_characterDictionary.Add(8203, tmp_Glyph2);
			}
			if (!this.m_characterDictionary.ContainsKey(8288))
			{
				tmp_Glyph2 = TMP_Glyph.Clone(this.m_characterDictionary[32]);
				tmp_Glyph2.width = 0f;
				tmp_Glyph2.xAdvance = 0f;
				this.m_characterDictionary.Add(8288, tmp_Glyph2);
			}
			if (!this.m_characterDictionary.ContainsKey(10))
			{
				tmp_Glyph2 = new TMP_Glyph();
				tmp_Glyph2.id = 10;
				tmp_Glyph2.x = 0f;
				tmp_Glyph2.y = 0f;
				tmp_Glyph2.width = 10f;
				tmp_Glyph2.height = this.m_characterDictionary[32].height;
				tmp_Glyph2.xOffset = 0f;
				tmp_Glyph2.yOffset = this.m_characterDictionary[32].yOffset;
				tmp_Glyph2.xAdvance = 0f;
				this.m_characterDictionary.Add(10, tmp_Glyph2);
				if (!this.m_characterDictionary.ContainsKey(13))
				{
					this.m_characterDictionary.Add(13, tmp_Glyph2);
				}
			}
			if (!this.m_characterDictionary.ContainsKey(9))
			{
				tmp_Glyph2 = new TMP_Glyph();
				tmp_Glyph2.id = 9;
				tmp_Glyph2.x = this.m_characterDictionary[32].x;
				tmp_Glyph2.y = this.m_characterDictionary[32].y;
				tmp_Glyph2.width = this.m_characterDictionary[32].width * (float)this.tabSize + (this.m_characterDictionary[32].xAdvance - this.m_characterDictionary[32].width) * (float)(this.tabSize - 1);
				tmp_Glyph2.height = this.m_characterDictionary[32].height;
				tmp_Glyph2.xOffset = this.m_characterDictionary[32].xOffset;
				tmp_Glyph2.yOffset = this.m_characterDictionary[32].yOffset;
				tmp_Glyph2.xAdvance = this.m_characterDictionary[32].xAdvance * (float)this.tabSize;
				this.m_characterDictionary.Add(9, tmp_Glyph2);
			}
			this.m_fontInfo.TabWidth = this.m_characterDictionary[9].xAdvance;
			if (this.m_fontInfo.Scale == 0f)
			{
				this.m_fontInfo.Scale = 1f;
			}
			this.m_kerningDictionary = new Dictionary<int, KerningPair>();
			List<KerningPair> kerningPairs = this.m_kerningInfo.kerningPairs;
			for (int i = 0; i < kerningPairs.Count; i++)
			{
				KerningPair kerningPair = kerningPairs[i];
				KerningPairKey kerningPairKey = new KerningPairKey(kerningPair.AscII_Left, kerningPair.AscII_Right);
				if (!this.m_kerningDictionary.ContainsKey(kerningPairKey.key))
				{
					this.m_kerningDictionary.Add(kerningPairKey.key, kerningPair);
				}
				else if (!TMP_Settings.warningsDisabled)
				{
					Debug.LogWarning(string.Concat(new object[] { "Kerning Key for [", kerningPairKey.ascii_Left, "] and [", kerningPairKey.ascii_Right, "] already exists." }));
				}
			}
			this.hashCode = TMP_TextUtilities.GetSimpleHashCode(base.name);
			this.materialHashCode = TMP_TextUtilities.GetSimpleHashCode(this.material.name);
		}

		public bool HasCharacter(int character)
		{
			return this.m_characterDictionary != null && this.m_characterDictionary.ContainsKey(character);
		}

		public bool HasCharacter(char character)
		{
			return this.m_characterDictionary != null && this.m_characterDictionary.ContainsKey((int)character);
		}

		public bool HasCharacters(string text, out List<char> missingCharacters)
		{
			if (this.m_characterDictionary == null)
			{
				missingCharacters = null;
				return false;
			}
			missingCharacters = new List<char>();
			for (int i = 0; i < text.Length; i++)
			{
				if (!this.m_characterDictionary.ContainsKey((int)text[i]))
				{
					missingCharacters.Add(text[i]);
				}
			}
			return missingCharacters.Count == 0;
		}

		public static string GetCharacters(TMP_FontAsset fontAsset)
		{
			string text = string.Empty;
			for (int i = 0; i < fontAsset.m_glyphInfoList.Count; i++)
			{
				text += (char)fontAsset.m_glyphInfoList[i].id;
			}
			return text;
		}

		public static int[] GetCharactersArray(TMP_FontAsset fontAsset)
		{
			int[] array = new int[fontAsset.m_glyphInfoList.Count];
			for (int i = 0; i < fontAsset.m_glyphInfoList.Count; i++)
			{
				array[i] = fontAsset.m_glyphInfoList[i].id;
			}
			return array;
		}

		private static TMP_FontAsset s_defaultFontAsset;

		public TMP_FontAsset.FontAssetTypes fontAssetType;

		[SerializeField]
		private FaceInfo m_fontInfo;

		[SerializeField]
		public Texture2D atlas;

		[SerializeField]
		private List<TMP_Glyph> m_glyphInfoList;

		private Dictionary<int, TMP_Glyph> m_characterDictionary;

		private Dictionary<int, KerningPair> m_kerningDictionary;

		[SerializeField]
		private KerningTable m_kerningInfo;

		[SerializeField]
		private KerningPair m_kerningPair;

		[SerializeField]
		public List<TMP_FontAsset> fallbackFontAssets;

		[SerializeField]
		public FontCreationSetting fontCreationSettings;

		public TMP_FontWeights[] fontWeights = new TMP_FontWeights[10];

		private int[] m_characterSet;

		public float normalStyle;

		public float normalSpacingOffset;

		public float boldStyle = 0.75f;

		public float boldSpacing = 7f;

		public byte italicStyle = 35;

		public byte tabSize = 10;

		private byte m_oldTabSize;

		public enum FontAssetTypes
		{
			None,
			SDF,
			Bitmap
		}
	}
}
