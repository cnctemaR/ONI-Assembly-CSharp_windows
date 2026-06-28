using System;

namespace TMPro
{
	public struct TMP_LinkInfo
	{
		internal void SetLinkID(char[] text, int startIndex, int length)
		{
			if (this.linkID == null || this.linkID.Length < length)
			{
				this.linkID = new char[length];
			}
			for (int i = 0; i < length; i++)
			{
				this.linkID[i] = text[startIndex + i];
			}
		}

		public string GetLinkText()
		{
			string text = string.Empty;
			TMP_TextInfo textInfo = this.textComponent.textInfo;
			for (int i = this.linkTextfirstCharacterIndex; i < this.linkTextfirstCharacterIndex + this.linkTextLength; i++)
			{
				text += textInfo.characterInfo[i].character;
			}
			return text;
		}

		public string GetLinkID()
		{
			string text;
			if (this.textComponent == null)
			{
				text = string.Empty;
			}
			else
			{
				text = new string(this.linkID);
			}
			return text;
		}

		public TMP_Text textComponent;

		public int hashCode;

		public int linkIdFirstCharacterIndex;

		public int linkIdLength;

		public int linkTextfirstCharacterIndex;

		public int linkTextLength;

		internal char[] linkID;
	}
}
