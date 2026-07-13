using System;
using System.Diagnostics;

namespace TMPro
{
	[DebuggerDisplay("{DebuggerDisplay()}")]
	internal struct TextProcessingElement
	{
		public TextProcessingElementType ElementType
		{
			get
			{
				return this.m_ElementType;
			}
			set
			{
				this.m_ElementType = value;
			}
		}

		public int StartIndex
		{
			get
			{
				return this.m_StartIndex;
			}
			set
			{
				this.m_StartIndex = value;
			}
		}

		public int Length
		{
			get
			{
				return this.m_Length;
			}
			set
			{
				this.m_Length = value;
			}
		}

		public CharacterElement CharacterElement
		{
			get
			{
				return this.m_CharacterElement;
			}
		}

		public MarkupElement MarkupElement
		{
			get
			{
				return this.m_MarkupElement;
			}
			set
			{
				this.m_MarkupElement = value;
			}
		}

		public TextProcessingElement(TextProcessingElementType elementType, int startIndex, int length)
		{
			this.m_ElementType = elementType;
			this.m_StartIndex = startIndex;
			this.m_Length = length;
			this.m_CharacterElement = default(CharacterElement);
			this.m_MarkupElement = default(MarkupElement);
		}

		public TextProcessingElement(TMP_TextElement textElement, int startIndex, int length)
		{
			this.m_ElementType = TextProcessingElementType.TextCharacterElement;
			this.m_StartIndex = startIndex;
			this.m_Length = length;
			this.m_CharacterElement = new CharacterElement(textElement);
			this.m_MarkupElement = default(MarkupElement);
		}

		public TextProcessingElement(CharacterElement characterElement, int startIndex, int length)
		{
			this.m_ElementType = TextProcessingElementType.TextCharacterElement;
			this.m_StartIndex = startIndex;
			this.m_Length = length;
			this.m_CharacterElement = characterElement;
			this.m_MarkupElement = default(MarkupElement);
		}

		public TextProcessingElement(MarkupElement markupElement)
		{
			this.m_ElementType = TextProcessingElementType.TextMarkupElement;
			this.m_StartIndex = markupElement.ValueStartIndex;
			this.m_Length = markupElement.ValueLength;
			this.m_CharacterElement = default(CharacterElement);
			this.m_MarkupElement = markupElement;
		}

		public static TextProcessingElement Undefined
		{
			get
			{
				return new TextProcessingElement
				{
					ElementType = TextProcessingElementType.Undefined
				};
			}
		}

		private string DebuggerDisplay()
		{
			if (this.m_ElementType != TextProcessingElementType.TextCharacterElement)
			{
				return string.Format("Markup = {0}", (MarkupTag)this.m_MarkupElement.NameHashCode);
			}
			return string.Format("Unicode ({0})   '{1}' ", this.m_CharacterElement.Unicode, (char)this.m_CharacterElement.Unicode);
		}

		private TextProcessingElementType m_ElementType;

		private int m_StartIndex;

		private int m_Length;

		private CharacterElement m_CharacterElement;

		private MarkupElement m_MarkupElement;
	}
}
