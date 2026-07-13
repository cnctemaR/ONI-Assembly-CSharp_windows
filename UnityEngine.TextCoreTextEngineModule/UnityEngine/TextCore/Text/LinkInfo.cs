using System;
using UnityEngine.Bindings;

namespace UnityEngine.TextCore.Text
{
	[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
	internal struct LinkInfo
	{
		internal void SetLinkId(char[] text, int startIndex, int length)
		{
			bool flag = this.linkId == null || this.linkId.Length < length;
			if (flag)
			{
				this.linkId = new char[length];
			}
			for (int i = 0; i < length; i++)
			{
				this.linkId[i] = text[startIndex + i];
			}
			this.linkIdLength = length;
			this.m_LinkIdString = null;
			this.m_LinkTextString = null;
		}

		public string GetLinkText(TextInfo textInfo)
		{
			bool flag = string.IsNullOrEmpty(this.m_LinkTextString);
			if (flag)
			{
				for (int i = this.linkTextfirstCharacterIndex; i < this.linkTextfirstCharacterIndex + this.linkTextLength; i++)
				{
					this.m_LinkTextString += ((char)textInfo.textElementInfo[i].character).ToString();
				}
			}
			return this.m_LinkTextString;
		}

		public string GetLinkId()
		{
			bool flag = string.IsNullOrEmpty(this.m_LinkIdString);
			if (flag)
			{
				this.m_LinkIdString = new string(this.linkId, 0, this.linkIdLength);
			}
			return this.m_LinkIdString;
		}

		public int hashCode;

		public int linkIdFirstCharacterIndex;

		public int linkIdLength;

		public int linkTextfirstCharacterIndex;

		public int linkTextLength;

		[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
		internal char[] linkId;

		private string m_LinkIdString;

		private string m_LinkTextString;
	}
}
