using System;

namespace UnityEngine.TextCore.Text
{
	internal struct MarkupAttribute
	{
		public int NameHashCode
		{
			get
			{
				return this.m_NameHashCode;
			}
			set
			{
				this.m_NameHashCode = value;
			}
		}

		public int ValueHashCode
		{
			get
			{
				return this.m_ValueHashCode;
			}
			set
			{
				this.m_ValueHashCode = value;
			}
		}

		public int ValueStartIndex
		{
			get
			{
				return this.m_ValueStartIndex;
			}
			set
			{
				this.m_ValueStartIndex = value;
			}
		}

		public int ValueLength
		{
			get
			{
				return this.m_ValueLength;
			}
			set
			{
				this.m_ValueLength = value;
			}
		}

		private int m_NameHashCode;

		private int m_ValueHashCode;

		private int m_ValueStartIndex;

		private int m_ValueLength;
	}
}
