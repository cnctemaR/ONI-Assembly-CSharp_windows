using System;

namespace TMPro
{
	internal struct CharacterElement
	{
		public uint Unicode
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

		public CharacterElement(TMP_TextElement textElement)
		{
			this.m_Unicode = textElement.unicode;
			this.m_TextElement = textElement;
		}

		private uint m_Unicode;

		private TMP_TextElement m_TextElement;
	}
}
