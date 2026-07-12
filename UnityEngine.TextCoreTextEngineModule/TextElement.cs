using System;

namespace UnityEngine.TextCore.Text
{
	[Serializable]
	public abstract class TextElement
	{
		public TextElementType elementType
		{
			get
			{
				return this.m_ElementType;
			}
		}

		public uint unicode
		{
			get
			{
				return this.m_Unicode;
			}
			set
			{
				this.m_Unicode = value;
			}
		}

		public TextAsset textAsset
		{
			get
			{
				return this.m_TextAsset;
			}
			set
			{
				this.m_TextAsset = value;
			}
		}

		public Glyph glyph
		{
			get
			{
				return this.m_Glyph;
			}
			set
			{
				this.m_Glyph = value;
			}
		}

		public uint glyphIndex
		{
			get
			{
				return this.m_GlyphIndex;
			}
			set
			{
				this.m_GlyphIndex = value;
			}
		}

		public float scale
		{
			get
			{
				return this.m_Scale;
			}
			set
			{
				this.m_Scale = value;
			}
		}

		[SerializeField]
		protected TextElementType m_ElementType;

		[SerializeField]
		internal uint m_Unicode;

		internal TextAsset m_TextAsset;

		internal Glyph m_Glyph;

		[SerializeField]
		internal uint m_GlyphIndex;

		[SerializeField]
		internal float m_Scale;
	}
}
