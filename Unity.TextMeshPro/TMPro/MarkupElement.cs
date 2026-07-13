using System;

namespace TMPro
{
	internal struct MarkupElement
	{
		public int NameHashCode
		{
			get
			{
				if (this.m_Attributes != null)
				{
					return this.m_Attributes[0].NameHashCode;
				}
				return 0;
			}
			set
			{
				if (this.m_Attributes == null)
				{
					this.m_Attributes = new MarkupAttribute[8];
				}
				this.m_Attributes[0].NameHashCode = value;
			}
		}

		public int ValueHashCode
		{
			get
			{
				if (this.m_Attributes != null)
				{
					return this.m_Attributes[0].ValueHashCode;
				}
				return 0;
			}
			set
			{
				this.m_Attributes[0].ValueHashCode = value;
			}
		}

		public int ValueStartIndex
		{
			get
			{
				if (this.m_Attributes != null)
				{
					return this.m_Attributes[0].ValueStartIndex;
				}
				return 0;
			}
			set
			{
				this.m_Attributes[0].ValueStartIndex = value;
			}
		}

		public int ValueLength
		{
			get
			{
				if (this.m_Attributes != null)
				{
					return this.m_Attributes[0].ValueLength;
				}
				return 0;
			}
			set
			{
				this.m_Attributes[0].ValueLength = value;
			}
		}

		public MarkupAttribute[] Attributes
		{
			get
			{
				return this.m_Attributes;
			}
			set
			{
				this.m_Attributes = value;
			}
		}

		public MarkupElement(int nameHashCode, int startIndex, int length)
		{
			this.m_Attributes = new MarkupAttribute[8];
			this.m_Attributes[0].NameHashCode = nameHashCode;
			this.m_Attributes[0].ValueStartIndex = startIndex;
			this.m_Attributes[0].ValueLength = length;
		}

		private MarkupAttribute[] m_Attributes;
	}
}
