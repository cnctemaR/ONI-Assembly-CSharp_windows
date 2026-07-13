using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Bindings;
using UnityEngine.Serialization;

namespace UnityEngine.TextCore.Text
{
	[ExcludeFromPreset]
	[HelpURL("https://docs.unity3d.com/2023.3/Documentation/Manual/UIE-sprite.html")]
	public class SpriteAsset : TextAsset
	{
		public FaceInfo faceInfo
		{
			get
			{
				return this.m_FaceInfo;
			}
			internal set
			{
				this.m_FaceInfo = value;
			}
		}

		public Texture spriteSheet
		{
			get
			{
				return this.m_SpriteAtlasTexture;
			}
			internal set
			{
				this.m_SpriteAtlasTexture = value;
				this.width = (float)this.m_SpriteAtlasTexture.width;
				this.height = (float)this.m_SpriteAtlasTexture.height;
			}
		}

		internal float width { get; private set; }

		internal float height { get; private set; }

		public List<SpriteCharacter> spriteCharacterTable
		{
			get
			{
				bool flag = this.m_GlyphIndexLookup == null;
				if (flag)
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

		public Dictionary<uint, SpriteCharacter> spriteCharacterLookupTable
		{
			get
			{
				bool flag = this.m_SpriteCharacterLookup == null;
				if (flag)
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

		public List<SpriteGlyph> spriteGlyphTable
		{
			get
			{
				return this.m_SpriteGlyphTable;
			}
			internal set
			{
				this.m_SpriteGlyphTable = value;
			}
		}

		private void Awake()
		{
		}

		public void UpdateLookupTables()
		{
			bool flag = this.m_SpriteAtlasTexture == null;
			if (flag)
			{
				Debug.LogWarning("Invalid Sprite Atlas Texture assigned to sprite asset [" + base.name + "]. Cannot update lookup tables.");
			}
			else
			{
				this.width = (float)this.m_SpriteAtlasTexture.width;
				this.height = (float)this.m_SpriteAtlasTexture.height;
				bool flag2 = this.m_GlyphIndexLookup == null;
				if (flag2)
				{
					this.m_GlyphIndexLookup = new Dictionary<uint, int>();
				}
				else
				{
					this.m_GlyphIndexLookup.Clear();
				}
				bool flag3 = this.m_SpriteGlyphLookup == null;
				if (flag3)
				{
					this.m_SpriteGlyphLookup = new Dictionary<uint, SpriteGlyph>();
				}
				else
				{
					this.m_SpriteGlyphLookup.Clear();
				}
				for (int i = 0; i < this.m_SpriteGlyphTable.Count; i++)
				{
					SpriteGlyph spriteGlyph = this.m_SpriteGlyphTable[i];
					uint index = spriteGlyph.index;
					bool flag4 = !this.m_GlyphIndexLookup.ContainsKey(index);
					if (flag4)
					{
						this.m_GlyphIndexLookup.Add(index, i);
					}
					bool flag5 = !this.m_SpriteGlyphLookup.ContainsKey(index);
					if (flag5)
					{
						this.m_SpriteGlyphLookup.Add(index, spriteGlyph);
					}
				}
				bool flag6 = this.m_NameLookup == null;
				if (flag6)
				{
					this.m_NameLookup = new Dictionary<int, int>();
				}
				else
				{
					this.m_NameLookup.Clear();
				}
				bool flag7 = this.m_SpriteCharacterLookup == null;
				if (flag7)
				{
					this.m_SpriteCharacterLookup = new Dictionary<uint, SpriteCharacter>();
				}
				else
				{
					this.m_SpriteCharacterLookup.Clear();
				}
				for (int j = 0; j < this.m_SpriteCharacterTable.Count; j++)
				{
					SpriteCharacter spriteCharacter = this.m_SpriteCharacterTable[j];
					bool flag8 = spriteCharacter == null;
					if (!flag8)
					{
						uint glyphIndex = spriteCharacter.glyphIndex;
						bool flag9 = !this.m_SpriteGlyphLookup.ContainsKey(glyphIndex);
						if (!flag9)
						{
							spriteCharacter.glyph = this.m_SpriteGlyphLookup[glyphIndex];
							spriteCharacter.textAsset = this;
							int hashCodeCaseInSensitive = TextUtilities.GetHashCodeCaseInSensitive(this.m_SpriteCharacterTable[j].name);
							bool flag10 = !this.m_NameLookup.ContainsKey(hashCodeCaseInSensitive);
							if (flag10)
							{
								this.m_NameLookup.Add(hashCodeCaseInSensitive, j);
							}
							uint unicode = this.m_SpriteCharacterTable[j].unicode;
							bool flag11 = unicode != 65534U && !this.m_SpriteCharacterLookup.ContainsKey(unicode);
							if (flag11)
							{
								this.m_SpriteCharacterLookup.Add(unicode, spriteCharacter);
							}
						}
					}
				}
				this.m_IsSpriteAssetLookupTablesDirty = false;
			}
		}

		public int GetSpriteIndexFromHashcode(int hashCode)
		{
			bool flag = this.m_NameLookup == null;
			if (flag)
			{
				this.UpdateLookupTables();
			}
			int num;
			bool flag2 = this.m_NameLookup.TryGetValue(hashCode, out num);
			int num2;
			if (flag2)
			{
				num2 = num;
			}
			else
			{
				num2 = -1;
			}
			return num2;
		}

		public int GetSpriteIndexFromUnicode(uint unicode)
		{
			bool flag = this.m_SpriteCharacterLookup == null;
			if (flag)
			{
				this.UpdateLookupTables();
			}
			SpriteCharacter spriteCharacter;
			bool flag2 = this.m_SpriteCharacterLookup.TryGetValue(unicode, out spriteCharacter);
			int num;
			if (flag2)
			{
				num = (int)spriteCharacter.glyphIndex;
			}
			else
			{
				num = -1;
			}
			return num;
		}

		public int GetSpriteIndexFromName(string name)
		{
			bool flag = this.m_NameLookup == null;
			if (flag)
			{
				this.UpdateLookupTables();
			}
			int hashCodeCaseInSensitive = TextUtilities.GetHashCodeCaseInSensitive(name);
			return this.GetSpriteIndexFromHashcode(hashCodeCaseInSensitive);
		}

		public static SpriteAsset SearchForSpriteByUnicode(SpriteAsset spriteAsset, uint unicode, bool includeFallbacks, out int spriteIndex)
		{
			bool flag = spriteAsset == null;
			SpriteAsset spriteAsset2;
			if (flag)
			{
				spriteIndex = -1;
				spriteAsset2 = null;
			}
			else
			{
				spriteIndex = spriteAsset.GetSpriteIndexFromUnicode(unicode);
				bool flag2 = spriteIndex != -1;
				if (flag2)
				{
					spriteAsset2 = spriteAsset;
				}
				else
				{
					HashSet<int> hashSet = new HashSet<int>();
					int instanceID = spriteAsset.GetInstanceID();
					hashSet.Add(instanceID);
					bool flag3 = includeFallbacks && spriteAsset.fallbackSpriteAssets != null && spriteAsset.fallbackSpriteAssets.Count > 0;
					if (flag3)
					{
						spriteAsset2 = SpriteAsset.SearchForSpriteByUnicodeInternal(spriteAsset.fallbackSpriteAssets, unicode, true, hashSet, out spriteIndex);
					}
					else
					{
						spriteIndex = -1;
						spriteAsset2 = null;
					}
				}
			}
			return spriteAsset2;
		}

		private static SpriteAsset SearchForSpriteByUnicodeInternal(List<SpriteAsset> spriteAssets, uint unicode, bool includeFallbacks, HashSet<int> searchedSpriteAssets, out int spriteIndex)
		{
			for (int i = 0; i < spriteAssets.Count; i++)
			{
				SpriteAsset spriteAsset = spriteAssets[i];
				bool flag = spriteAsset == null;
				if (!flag)
				{
					int instanceID = spriteAsset.GetInstanceID();
					bool flag2 = !searchedSpriteAssets.Add(instanceID);
					if (!flag2)
					{
						spriteAsset = SpriteAsset.SearchForSpriteByUnicodeInternal(spriteAsset, unicode, includeFallbacks, searchedSpriteAssets, out spriteIndex);
						bool flag3 = spriteAsset != null;
						if (flag3)
						{
							return spriteAsset;
						}
					}
				}
			}
			spriteIndex = -1;
			return null;
		}

		private static SpriteAsset SearchForSpriteByUnicodeInternal(SpriteAsset spriteAsset, uint unicode, bool includeFallbacks, HashSet<int> searchedSpriteAssets, out int spriteIndex)
		{
			spriteIndex = spriteAsset.GetSpriteIndexFromUnicode(unicode);
			bool flag = spriteIndex != -1;
			SpriteAsset spriteAsset2;
			if (flag)
			{
				spriteAsset2 = spriteAsset;
			}
			else
			{
				bool flag2 = includeFallbacks && spriteAsset.fallbackSpriteAssets != null && spriteAsset.fallbackSpriteAssets.Count > 0;
				if (flag2)
				{
					spriteAsset2 = SpriteAsset.SearchForSpriteByUnicodeInternal(spriteAsset.fallbackSpriteAssets, unicode, true, searchedSpriteAssets, out spriteIndex);
				}
				else
				{
					spriteIndex = -1;
					spriteAsset2 = null;
				}
			}
			return spriteAsset2;
		}

		public static SpriteAsset SearchForSpriteByHashCode(SpriteAsset spriteAsset, int hashCode, bool includeFallbacks, out int spriteIndex, TextSettings textSettings = null)
		{
			bool flag = spriteAsset == null;
			SpriteAsset spriteAsset2;
			if (flag)
			{
				spriteIndex = -1;
				spriteAsset2 = null;
			}
			else
			{
				spriteIndex = spriteAsset.GetSpriteIndexFromHashcode(hashCode);
				bool flag2 = spriteIndex != -1;
				if (flag2)
				{
					spriteAsset2 = spriteAsset;
				}
				else
				{
					HashSet<int> hashSet = new HashSet<int>();
					int hashCode2 = spriteAsset.GetHashCode();
					hashSet.Add(hashCode2);
					bool flag3 = includeFallbacks && spriteAsset.fallbackSpriteAssets != null && spriteAsset.fallbackSpriteAssets.Count > 0;
					if (flag3)
					{
						SpriteAsset spriteAsset3 = SpriteAsset.SearchForSpriteByHashCodeInternal(spriteAsset.fallbackSpriteAssets, hashCode, true, hashSet, out spriteIndex);
						bool flag4 = spriteIndex != -1;
						if (flag4)
						{
							return spriteAsset3;
						}
					}
					bool flag5 = textSettings == null;
					if (flag5)
					{
						spriteIndex = -1;
						spriteAsset2 = null;
					}
					else
					{
						bool flag6 = includeFallbacks && textSettings.defaultSpriteAsset != null;
						if (flag6)
						{
							SpriteAsset spriteAsset3 = SpriteAsset.SearchForSpriteByHashCodeInternal(textSettings.defaultSpriteAsset, hashCode, true, hashSet, out spriteIndex);
							bool flag7 = spriteIndex != -1;
							if (flag7)
							{
								return spriteAsset3;
							}
						}
						hashSet.Clear();
						uint missingSpriteCharacterUnicode = textSettings.missingSpriteCharacterUnicode;
						spriteIndex = spriteAsset.GetSpriteIndexFromUnicode(missingSpriteCharacterUnicode);
						bool flag8 = spriteIndex != -1;
						if (flag8)
						{
							spriteAsset2 = spriteAsset;
						}
						else
						{
							hashSet.Add(hashCode2);
							bool flag9 = includeFallbacks && spriteAsset.fallbackSpriteAssets != null && spriteAsset.fallbackSpriteAssets.Count > 0;
							if (flag9)
							{
								SpriteAsset spriteAsset3 = SpriteAsset.SearchForSpriteByUnicodeInternal(spriteAsset.fallbackSpriteAssets, missingSpriteCharacterUnicode, true, hashSet, out spriteIndex);
								bool flag10 = spriteIndex != -1;
								if (flag10)
								{
									return spriteAsset3;
								}
							}
							bool flag11 = includeFallbacks && textSettings.defaultSpriteAsset != null;
							if (flag11)
							{
								SpriteAsset spriteAsset3 = SpriteAsset.SearchForSpriteByUnicodeInternal(textSettings.defaultSpriteAsset, missingSpriteCharacterUnicode, true, hashSet, out spriteIndex);
								bool flag12 = spriteIndex != -1;
								if (flag12)
								{
									return spriteAsset3;
								}
							}
							spriteIndex = -1;
							spriteAsset2 = null;
						}
					}
				}
			}
			return spriteAsset2;
		}

		private static SpriteAsset SearchForSpriteByHashCodeInternal(List<SpriteAsset> spriteAssets, int hashCode, bool searchFallbacks, HashSet<int> searchedSpriteAssets, out int spriteIndex)
		{
			for (int i = 0; i < spriteAssets.Count; i++)
			{
				SpriteAsset spriteAsset = spriteAssets[i];
				bool flag = spriteAsset == null;
				if (!flag)
				{
					int hashCode2 = spriteAsset.GetHashCode();
					bool flag2 = !searchedSpriteAssets.Add(hashCode2);
					if (!flag2)
					{
						spriteAsset = SpriteAsset.SearchForSpriteByHashCodeInternal(spriteAsset, hashCode, searchFallbacks, searchedSpriteAssets, out spriteIndex);
						bool flag3 = spriteAsset != null;
						if (flag3)
						{
							return spriteAsset;
						}
					}
				}
			}
			spriteIndex = -1;
			return null;
		}

		private static SpriteAsset SearchForSpriteByHashCodeInternal(SpriteAsset spriteAsset, int hashCode, bool searchFallbacks, HashSet<int> searchedSpriteAssets, out int spriteIndex)
		{
			spriteIndex = spriteAsset.GetSpriteIndexFromHashcode(hashCode);
			bool flag = spriteIndex != -1;
			SpriteAsset spriteAsset2;
			if (flag)
			{
				spriteAsset2 = spriteAsset;
			}
			else
			{
				bool flag2 = searchFallbacks && spriteAsset.fallbackSpriteAssets != null && spriteAsset.fallbackSpriteAssets.Count > 0;
				if (flag2)
				{
					spriteAsset2 = SpriteAsset.SearchForSpriteByHashCodeInternal(spriteAsset.fallbackSpriteAssets, hashCode, true, searchedSpriteAssets, out spriteIndex);
				}
				else
				{
					spriteIndex = -1;
					spriteAsset2 = null;
				}
			}
			return spriteAsset2;
		}

		public void SortGlyphTable()
		{
			bool flag = this.m_SpriteGlyphTable == null || this.m_SpriteGlyphTable.Count == 0;
			if (!flag)
			{
				this.m_SpriteGlyphTable = this.m_SpriteGlyphTable.OrderBy<SpriteGlyph, uint>((SpriteGlyph item) => item.index).ToList<SpriteGlyph>();
			}
		}

		internal void SortCharacterTable()
		{
			bool flag = this.m_SpriteCharacterTable != null && this.m_SpriteCharacterTable.Count > 0;
			if (flag)
			{
				this.m_SpriteCharacterTable = this.m_SpriteCharacterTable.OrderBy<SpriteCharacter, uint>((SpriteCharacter c) => c.unicode).ToList<SpriteCharacter>();
			}
		}

		internal void SortGlyphAndCharacterTables()
		{
			this.SortGlyphTable();
			this.SortCharacterTable();
		}

		internal Dictionary<int, int> m_NameLookup;

		internal Dictionary<uint, int> m_GlyphIndexLookup;

		[SerializeField]
		internal FaceInfo m_FaceInfo;

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		[FormerlySerializedAs("spriteSheet")]
		[SerializeField]
		internal Texture m_SpriteAtlasTexture;

		[SerializeField]
		private List<SpriteCharacter> m_SpriteCharacterTable = new List<SpriteCharacter>();

		internal Dictionary<uint, SpriteCharacter> m_SpriteCharacterLookup;

		[SerializeField]
		private List<SpriteGlyph> m_SpriteGlyphTable = new List<SpriteGlyph>();

		internal Dictionary<uint, SpriteGlyph> m_SpriteGlyphLookup;

		[SerializeField]
		public List<SpriteAsset> fallbackSpriteAssets;

		internal bool m_IsSpriteAssetLookupTablesDirty = false;
	}
}
