using System;

namespace UnityEngine.TextCore.Text
{
	[Serializable]
	public class TextStyle
	{
		public static TextStyle NormalStyle
		{
			get
			{
				bool flag = TextStyle.k_NormalStyle == null;
				if (flag)
				{
					TextStyle.k_NormalStyle = new TextStyle("Normal", string.Empty, string.Empty);
				}
				return TextStyle.k_NormalStyle;
			}
		}

		public string name
		{
			get
			{
				return this.m_Name;
			}
			set
			{
				bool flag = value != this.m_Name;
				if (flag)
				{
					this.m_Name = value;
				}
			}
		}

		public int hashCode
		{
			get
			{
				return this.m_HashCode;
			}
			set
			{
				bool flag = value != this.m_HashCode;
				if (flag)
				{
					this.m_HashCode = value;
				}
			}
		}

		public string styleOpeningDefinition
		{
			get
			{
				return this.m_OpeningDefinition;
			}
		}

		public string styleClosingDefinition
		{
			get
			{
				return this.m_ClosingDefinition;
			}
		}

		public uint[] styleOpeningTagArray
		{
			get
			{
				return this.m_OpeningTagArray;
			}
		}

		public uint[] styleClosingTagArray
		{
			get
			{
				return this.m_ClosingTagArray;
			}
		}

		internal TextStyle(string styleName, string styleOpeningDefinition, string styleClosingDefinition)
		{
			this.m_Name = styleName;
			this.m_HashCode = TextUtilities.GetHashCodeCaseInSensitive(styleName);
			this.m_OpeningDefinition = styleOpeningDefinition;
			this.m_ClosingDefinition = styleClosingDefinition;
			this.RefreshStyle();
		}

		public void RefreshStyle()
		{
			this.m_HashCode = TextUtilities.GetHashCodeCaseInSensitive(this.m_Name);
			int length = this.m_OpeningDefinition.Length;
			this.m_OpeningTagArray = new uint[length];
			this.m_OpeningTagUnicodeArray = new uint[length];
			for (int i = 0; i < length; i++)
			{
				this.m_OpeningTagArray[i] = (uint)this.m_OpeningDefinition[i];
				this.m_OpeningTagUnicodeArray[i] = (uint)this.m_OpeningDefinition[i];
			}
			int length2 = this.m_ClosingDefinition.Length;
			this.m_ClosingTagArray = new uint[length2];
			this.m_ClosingTagUnicodeArray = new uint[length2];
			for (int j = 0; j < length2; j++)
			{
				this.m_ClosingTagArray[j] = (uint)this.m_ClosingDefinition[j];
				this.m_ClosingTagUnicodeArray[j] = (uint)this.m_ClosingDefinition[j];
			}
		}

		internal static TextStyle k_NormalStyle;

		[SerializeField]
		private string m_Name;

		[SerializeField]
		private int m_HashCode;

		[SerializeField]
		private string m_OpeningDefinition;

		[SerializeField]
		private string m_ClosingDefinition;

		[SerializeField]
		private uint[] m_OpeningTagArray;

		[SerializeField]
		private uint[] m_ClosingTagArray;

		[SerializeField]
		internal uint[] m_OpeningTagUnicodeArray;

		[SerializeField]
		internal uint[] m_ClosingTagUnicodeArray;
	}
}
