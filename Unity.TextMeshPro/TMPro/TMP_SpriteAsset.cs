using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.TextCore;

namespace TMPro
{
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/TextMeshPro/Sprites.html")]
	[ExcludeFromPreset]
	public class TMP_SpriteAsset : TMP_Asset
	{
		public List<TMP_SpriteCharacter> spriteCharacterTable
		{
			get
			{
				if (this.m_GlyphIndexLookup == null)
				{
					this.UpdateLookupTables();
				}
				return this.m_SpriteCharacterTable;
			}
			internal set
			{
				this.m_SpriteCharacterTable = value;
			}
		}

		public Dictionary<uint, TMP_SpriteCharacter> spriteCharacterLookupTable
		{
			get
			{
				if (this.m_SpriteCharacterLookup == null)
				{
					this.UpdateLookupTables();
				}
				return this.m_SpriteCharacterLookup;
			}
			internal set
			{
				this.m_SpriteCharacterLookup = value;
			}
		}

		public List<TMP_SpriteGlyph> spriteGlyphTable
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

		private void Awake()
		{
			if (base.material != null && string.IsNullOrEmpty(this.m_Version))
			{
				this.UpgradeSpriteAsset();
			}
		}

		private Material GetDefaultSpriteMaterial()
		{
			ShaderUtilities.GetShaderPropertyIDs();
			Material material = new Material(Shader.Find("TextMeshPro/Sprite"));
			material.SetTexture(ShaderUtilities.ID_MainTex, this.spriteSheet);
			return material;
		}

		public void UpdateLookupTables()
		{
			if (base.material != null && string.IsNullOrEmpty(this.m_Version))
			{
				this.UpgradeSpriteAsset();
			}
			if (this.m_GlyphIndexLookup == null)
			{
				this.m_GlyphIndexLookup = new Dictionary<uint, int>();
			}
			else
			{
				this.m_GlyphIndexLookup.Clear();
			}
			if (this.m_SpriteGlyphLookup == null)
			{
				this.m_SpriteGlyphLookup = new Dictionary<uint, TMP_SpriteGlyph>();
			}
			else
			{
				this.m_SpriteGlyphLookup.Clear();
			}
			for (int i = 0; i < this.m_GlyphTable.Count; i++)
			{
				TMP_SpriteGlyph tmp_SpriteGlyph = this.m_GlyphTable[i];
				uint index = tmp_SpriteGlyph.index;
				if (!this.m_GlyphIndexLookup.ContainsKey(index))
				{
					this.m_GlyphIndexLookup.Add(index, i);
				}
				if (!this.m_SpriteGlyphLookup.ContainsKey(index))
				{
					this.m_SpriteGlyphLookup.Add(index, tmp_SpriteGlyph);
				}
			}
			if (this.m_NameLookup == null)
			{
				this.m_NameLookup = new Dictionary<int, int>();
			}
			else
			{
				this.m_NameLookup.Clear();
			}
			if (this.m_SpriteCharacterLookup == null)
			{
				this.m_SpriteCharacterLookup = new Dictionary<uint, TMP_SpriteCharacter>();
			}
			else
			{
				this.m_SpriteCharacterLookup.Clear();
			}
			for (int j = 0; j < this.m_SpriteCharacterTable.Count; j++)
			{
				TMP_SpriteCharacter tmp_SpriteCharacter = this.m_SpriteCharacterTable[j];
				if (tmp_SpriteCharacter != null)
				{
					uint glyphIndex = tmp_SpriteCharacter.glyphIndex;
					if (this.m_SpriteGlyphLookup.ContainsKey(glyphIndex))
					{
						tmp_SpriteCharacter.glyph = this.m_SpriteGlyphLookup[glyphIndex];
						tmp_SpriteCharacter.textAsset = this;
						int hashCode = TMP_TextUtilities.GetHashCode(this.m_SpriteCharacterTable[j].name);
						if (!this.m_NameLookup.ContainsKey(hashCode))
						{
							this.m_NameLookup.Add(hashCode, j);
						}
						uint unicode = this.m_SpriteCharacterTable[j].unicode;
						if (unicode != 65534U && !this.m_SpriteCharacterLookup.ContainsKey(unicode))
						{
							this.m_SpriteCharacterLookup.Add(unicode, tmp_SpriteCharacter);
						}
					}
				}
			}
			this.m_IsSpriteAssetLookupTablesDirty = false;
		}

		public int GetSpriteIndexFromHashcode(int hashCode)
		{
			if (this.m_NameLookup == null)
			{
				this.UpdateLookupTables();
			}
			int num;
			if (this.m_NameLookup.TryGetValue(hashCode, out num))
			{
				return num;
			}
			return -1;
		}

		public int GetSpriteIndexFromUnicode(uint unicode)
		{
			if (this.m_SpriteCharacterLookup == null)
			{
				this.UpdateLookupTables();
			}
			TMP_SpriteCharacter tmp_SpriteCharacter;
			if (this.m_SpriteCharacterLookup.TryGetValue(unicode, out tmp_SpriteCharacter))
			{
				return (int)tmp_SpriteCharacter.glyphIndex;
			}
			return -1;
		}

		public int GetSpriteIndexFromName(string name)
		{
			if (this.m_NameLookup == null)
			{
				this.UpdateLookupTables();
			}
			int hashCode = TMP_TextUtilities.GetHashCode(name);
			return this.GetSpriteIndexFromHashcode(hashCode);
		}

		public static TMP_SpriteAsset SearchForSpriteByUnicode(TMP_SpriteAsset spriteAsset, uint unicode, bool includeFallbacks, out int spriteIndex)
		{
			if (spriteAsset == null)
			{
				spriteIndex = -1;
				return null;
			}
			spriteIndex = spriteAsset.GetSpriteIndexFromUnicode(unicode);
			if (spriteIndex != -1)
			{
				return spriteAsset;
			}
			if (TMP_SpriteAsset.k_searchedSpriteAssets == null)
			{
				TMP_SpriteAsset.k_searchedSpriteAssets = new HashSet<int>();
			}
			else
			{
				TMP_SpriteAsset.k_searchedSpriteAssets.Clear();
			}
			int instanceID = spriteAsset.GetInstanceID();
			TMP_SpriteAsset.k_searchedSpriteAssets.Add(instanceID);
			if (includeFallbacks && spriteAsset.fallbackSpriteAssets != null && spriteAsset.fallbackSpriteAssets.Count > 0)
			{
				return TMP_SpriteAsset.SearchForSpriteByUnicodeInternal(spriteAsset.fallbackSpriteAssets, unicode, true, out spriteIndex);
			}
			if (includeFallbacks && TMP_Settings.defaultSpriteAsset != null)
			{
				return TMP_SpriteAsset.SearchForSpriteByUnicodeInternal(TMP_Settings.defaultSpriteAsset, unicode, true, out spriteIndex);
			}
			spriteIndex = -1;
			return null;
		}

		private static TMP_SpriteAsset SearchForSpriteByUnicodeInternal(List<TMP_SpriteAsset> spriteAssets, uint unicode, bool includeFallbacks, out int spriteIndex)
		{
			for (int i = 0; i < spriteAssets.Count; i++)
			{
				TMP_SpriteAsset tmp_SpriteAsset = spriteAssets[i];
				if (!(tmp_SpriteAsset == null))
				{
					int instanceID = tmp_SpriteAsset.GetInstanceID();
					if (TMP_SpriteAsset.k_searchedSpriteAssets.Add(instanceID))
					{
						tmp_SpriteAsset = TMP_SpriteAsset.SearchForSpriteByUnicodeInternal(tmp_SpriteAsset, unicode, includeFallbacks, out spriteIndex);
						if (tmp_SpriteAsset != null)
						{
							return tmp_SpriteAsset;
						}
					}
				}
			}
			spriteIndex = -1;
			return null;
		}

		private static TMP_SpriteAsset SearchForSpriteByUnicodeInternal(TMP_SpriteAsset spriteAsset, uint unicode, bool includeFallbacks, out int spriteIndex)
		{
			spriteIndex = spriteAsset.GetSpriteIndexFromUnicode(unicode);
			if (spriteIndex != -1)
			{
				return spriteAsset;
			}
			if (includeFallbacks && spriteAsset.fallbackSpriteAssets != null && spriteAsset.fallbackSpriteAssets.Count > 0)
			{
				return TMP_SpriteAsset.SearchForSpriteByUnicodeInternal(spriteAsset.fallbackSpriteAssets, unicode, true, out spriteIndex);
			}
			spriteIndex = -1;
			return null;
		}

		public static TMP_SpriteAsset SearchForSpriteByHashCode(TMP_SpriteAsset spriteAsset, int hashCode, bool includeFallbacks, out int spriteIndex)
		{
			if (spriteAsset == null)
			{
				spriteIndex = -1;
				return null;
			}
			spriteIndex = spriteAsset.GetSpriteIndexFromHashcode(hashCode);
			if (spriteIndex != -1)
			{
				return spriteAsset;
			}
			if (TMP_SpriteAsset.k_searchedSpriteAssets == null)
			{
				TMP_SpriteAsset.k_searchedSpriteAssets = new HashSet<int>();
			}
			else
			{
				TMP_SpriteAsset.k_searchedSpriteAssets.Clear();
			}
			int instanceID = spriteAsset.instanceID;
			TMP_SpriteAsset.k_searchedSpriteAssets.Add(instanceID);
			if (includeFallbacks && spriteAsset.fallbackSpriteAssets != null && spriteAsset.fallbackSpriteAssets.Count > 0)
			{
				TMP_SpriteAsset tmp_SpriteAsset = TMP_SpriteAsset.SearchForSpriteByHashCodeInternal(spriteAsset.fallbackSpriteAssets, hashCode, true, out spriteIndex);
				if (spriteIndex != -1)
				{
					return tmp_SpriteAsset;
				}
			}
			if (includeFallbacks && TMP_Settings.defaultSpriteAsset != null)
			{
				TMP_SpriteAsset tmp_SpriteAsset = TMP_SpriteAsset.SearchForSpriteByHashCodeInternal(TMP_Settings.defaultSpriteAsset, hashCode, true, out spriteIndex);
				if (spriteIndex != -1)
				{
					return tmp_SpriteAsset;
				}
			}
			TMP_SpriteAsset.k_searchedSpriteAssets.Clear();
			uint missingCharacterSpriteUnicode = TMP_Settings.missingCharacterSpriteUnicode;
			spriteIndex = spriteAsset.GetSpriteIndexFromUnicode(missingCharacterSpriteUnicode);
			if (spriteIndex != -1)
			{
				return spriteAsset;
			}
			TMP_SpriteAsset.k_searchedSpriteAssets.Add(instanceID);
			if (includeFallbacks && spriteAsset.fallbackSpriteAssets != null && spriteAsset.fallbackSpriteAssets.Count > 0)
			{
				TMP_SpriteAsset tmp_SpriteAsset = TMP_SpriteAsset.SearchForSpriteByUnicodeInternal(spriteAsset.fallbackSpriteAssets, missingCharacterSpriteUnicode, true, out spriteIndex);
				if (spriteIndex != -1)
				{
					return tmp_SpriteAsset;
				}
			}
			if (includeFallbacks && TMP_Settings.defaultSpriteAsset != null)
			{
				TMP_SpriteAsset tmp_SpriteAsset = TMP_SpriteAsset.SearchForSpriteByUnicodeInternal(TMP_Settings.defaultSpriteAsset, missingCharacterSpriteUnicode, true, out spriteIndex);
				if (spriteIndex != -1)
				{
					return tmp_SpriteAsset;
				}
			}
			spriteIndex = -1;
			return null;
		}

		private static TMP_SpriteAsset SearchForSpriteByHashCodeInternal(List<TMP_SpriteAsset> spriteAssets, int hashCode, bool searchFallbacks, out int spriteIndex)
		{
			for (int i = 0; i < spriteAssets.Count; i++)
			{
				TMP_SpriteAsset tmp_SpriteAsset = spriteAssets[i];
				if (!(tmp_SpriteAsset == null))
				{
					int instanceID = tmp_SpriteAsset.instanceID;
					if (TMP_SpriteAsset.k_searchedSpriteAssets.Add(instanceID))
					{
						tmp_SpriteAsset = TMP_SpriteAsset.SearchForSpriteByHashCodeInternal(tmp_SpriteAsset, hashCode, searchFallbacks, out spriteIndex);
						if (tmp_SpriteAsset != null)
						{
							return tmp_SpriteAsset;
						}
					}
				}
			}
			spriteIndex = -1;
			return null;
		}

		private static TMP_SpriteAsset SearchForSpriteByHashCodeInternal(TMP_SpriteAsset spriteAsset, int hashCode, bool searchFallbacks, out int spriteIndex)
		{
			spriteIndex = spriteAsset.GetSpriteIndexFromHashcode(hashCode);
			if (spriteIndex != -1)
			{
				return spriteAsset;
			}
			if (searchFallbacks && spriteAsset.fallbackSpriteAssets != null && spriteAsset.fallbackSpriteAssets.Count > 0)
			{
				return TMP_SpriteAsset.SearchForSpriteByHashCodeInternal(spriteAsset.fallbackSpriteAssets, hashCode, true, out spriteIndex);
			}
			spriteIndex = -1;
			return null;
		}

		public void SortGlyphTable()
		{
			if (this.m_GlyphTable == null || this.m_GlyphTable.Count == 0)
			{
				return;
			}
			this.m_GlyphTable = this.m_GlyphTable.OrderBy<TMP_SpriteGlyph, uint>((TMP_SpriteGlyph item) => item.index).ToList<TMP_SpriteGlyph>();
		}

		internal void SortCharacterTable()
		{
			if (this.m_SpriteCharacterTable != null && this.m_SpriteCharacterTable.Count > 0)
			{
				this.m_SpriteCharacterTable = this.m_SpriteCharacterTable.OrderBy<TMP_SpriteCharacter, uint>((TMP_SpriteCharacter c) => c.unicode).ToList<TMP_SpriteCharacter>();
			}
		}

		internal void SortGlyphAndCharacterTables()
		{
			this.SortGlyphTable();
			this.SortCharacterTable();
		}

		private void UpgradeSpriteAsset()
		{
			this.m_Version = "1.1.0";
			Debug.Log(string.Concat(new string[] { "Upgrading sprite asset [", base.name, "] to version ", this.m_Version, "." }), this);
			this.m_SpriteCharacterTable.Clear();
			this.m_GlyphTable.Clear();
			for (int i = 0; i < this.spriteInfoList.Count; i++)
			{
				TMP_Sprite tmp_Sprite = this.spriteInfoList[i];
				TMP_SpriteGlyph tmp_SpriteGlyph = new TMP_SpriteGlyph();
				tmp_SpriteGlyph.index = (uint)i;
				tmp_SpriteGlyph.sprite = tmp_Sprite.sprite;
				tmp_SpriteGlyph.metrics = new GlyphMetrics(tmp_Sprite.width, tmp_Sprite.height, tmp_Sprite.xOffset, tmp_Sprite.yOffset, tmp_Sprite.xAdvance);
				tmp_SpriteGlyph.glyphRect = new GlyphRect((int)tmp_Sprite.x, (int)tmp_Sprite.y, (int)tmp_Sprite.width, (int)tmp_Sprite.height);
				tmp_SpriteGlyph.scale = 1f;
				tmp_SpriteGlyph.atlasIndex = 0;
				this.m_GlyphTable.Add(tmp_SpriteGlyph);
				TMP_SpriteCharacter tmp_SpriteCharacter = new TMP_SpriteCharacter();
				tmp_SpriteCharacter.glyph = tmp_SpriteGlyph;
				tmp_SpriteCharacter.unicode = (uint)((tmp_Sprite.unicode == 0) ? 65534 : tmp_Sprite.unicode);
				tmp_SpriteCharacter.name = tmp_Sprite.name;
				tmp_SpriteCharacter.scale = tmp_Sprite.scale;
				this.m_SpriteCharacterTable.Add(tmp_SpriteCharacter);
			}
			this.UpdateLookupTables();
		}

		internal Dictionary<int, int> m_NameLookup;

		internal Dictionary<uint, int> m_GlyphIndexLookup;

		public Texture spriteSheet;

		[SerializeField]
		private List<TMP_SpriteCharacter> m_SpriteCharacterTable = new List<TMP_SpriteCharacter>();

		internal Dictionary<uint, TMP_SpriteCharacter> m_SpriteCharacterLookup;

		[FormerlySerializedAs("m_SpriteGlyphTable")]
		[SerializeField]
		private List<TMP_SpriteGlyph> m_GlyphTable = new List<TMP_SpriteGlyph>();

		internal Dictionary<uint, TMP_SpriteGlyph> m_SpriteGlyphLookup;

		public List<TMP_Sprite> spriteInfoList;

		[SerializeField]
		public List<TMP_SpriteAsset> fallbackSpriteAssets;

		internal bool m_IsSpriteAssetLookupTablesDirty;

		private static HashSet<int> k_searchedSpriteAssets;
	}
}
