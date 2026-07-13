using System;
using UnityEngine;

namespace TMPro
{
	[Serializable]
	public class TMP_Style
	{
		public static TMP_Style NormalStyle
		{
			get
			{
				if (TMP_Style.k_NormalStyle == null)
				{
					TMP_Style.k_NormalStyle = new TMP_Style("Normal", string.Empty, string.Empty);
				}
				return TMP_Style.k_NormalStyle;
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
				if (value != this.m_Name)
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
				if (value != this.m_HashCode)
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

		public TMP_Style(string styleName, string styleOpeningDefinition, string styleClosingDefinition)
		{
			this.m_Name = styleName;
			this.m_HashCode = TMP_TextParsingUtilities.GetHashCode(styleName);
			this.m_OpeningDefinition = styleOpeningDefinition;
			this.m_ClosingDefinition = styleClosingDefinition;
			this.RefreshStyle();
		}

		public void RefreshStyle()
		{
			this.m_HashCode = TMP_TextParsingUtilities.GetHashCode(this.m_Name);
			int length = this.m_OpeningDefinition.Length;
			this.m_OpeningTagArray = new uint[length];
			for (int i = 0; i < length; i++)
			{
				this.m_OpeningTagArray[i] = (uint)this.m_OpeningDefinition[i];
			}
			int length2 = this.m_ClosingDefinition.Length;
			this.m_ClosingTagArray = new uint[length2];
			for (int j = 0; j < length2; j++)
			{
				this.m_ClosingTagArray[j] = (uint)this.m_ClosingDefinition[j];
			}
		}

		internal static TMP_Style k_NormalStyle;

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
	}
}
