using System;
using UnityEngine;

namespace TMPro
{
	[Serializable]
	public class TMP_SpriteCharacter : TMP_TextElement
	{
		public string name
		{
			get
			{
				return this.m_Name;
			}
			set
			{
				this.m_Name = value;
			}
		}

		public TMP_SpriteCharacter()
		{
			this.m_ElementType = TextElementType.Sprite;
		}

		public TMP_SpriteCharacter(uint unicode, TMP_SpriteGlyph glyph)
		{
			this.m_ElementType = TextElementType.Sprite;
			base.unicode = unicode;
			base.glyphIndex = glyph.index;
			base.glyph = glyph;
			base.scale = 1f;
		}

		public TMP_SpriteCharacter(uint unicode, TMP_SpriteAsset spriteAsset, TMP_SpriteGlyph glyph)
		{
			this.m_ElementType = TextElementType.Sprite;
			base.unicode = unicode;
			base.textAsset = spriteAsset;
			base.glyph = glyph;
			base.glyphIndex = glyph.index;
			base.scale = 1f;
		}

		internal TMP_SpriteCharacter(uint unicode, uint glyphIndex)
		{
			this.m_ElementType = TextElementType.Sprite;
			base.unicode = unicode;
			base.textAsset = null;
			base.glyph = null;
			base.glyphIndex = glyphIndex;
			base.scale = 1f;
		}

		[SerializeField]
		private string m_Name;
	}
}
