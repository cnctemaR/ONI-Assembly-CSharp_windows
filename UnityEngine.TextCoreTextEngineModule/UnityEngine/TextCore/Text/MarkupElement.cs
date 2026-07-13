using System;

namespace UnityEngine.TextCore.Text
{
	internal struct MarkupElement
	{
		public int NameHashCode
		{
			get
			{
				return (this.m_Attributes == null) ? 0 : this.m_Attributes[0].NameHashCode;
			}
			set
			{
				bool flag = this.m_Attributes == null;
				if (flag)
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
				return (this.m_Attributes == null) ? 0 : this.m_Attributes[0].ValueHashCode;
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
				return (this.m_Attributes == null) ? 0 : this.m_Attributes[0].ValueStartIndex;
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
				return (this.m_Attributes == null) ? 0 : this.m_Attributes[0].ValueLength;
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
