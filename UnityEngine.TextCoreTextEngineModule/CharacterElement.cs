using System;

namespace UnityEngine.TextCore.Text
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

		public CharacterElement(TextElement textElement)
		{
			this.m_Unicode = textElement.unicode;
			this.m_TextElement = textElement;
		}

		private uint m_Unicode;

		private TextElement m_TextElement;
	}
}
