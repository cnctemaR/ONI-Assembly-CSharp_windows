using System;
using System.Text;

namespace FileHelpers.Dynamic
{
	internal sealed class AttributesBuilder
	{
		public AttributesBuilder(NetLanguage lang)
		{
			this.mLang = lang;
		}

		public void AddAttribute(string attribute)
		{
			if (string.IsNullOrEmpty(attribute))
			{
				return;
			}
			if (this.mFirst)
			{
				switch (this.mLang)
				{
				case NetLanguage.CSharp:
					this.mSb.Append("[");
					break;
				case NetLanguage.VbNet:
					this.mSb.Append("<");
					break;
				}
				this.mFirst = false;
			}
			else
			{
				switch (this.mLang)
				{
				case NetLanguage.CSharp:
					this.mSb.Append("[");
					break;
				case NetLanguage.VbNet:
					this.mSb.Append(", _");
					this.mSb.Append(StringHelper.NewLine);
					this.mSb.Append(" ");
					break;
				}
			}
			this.mSb.Append(attribute);
			switch (this.mLang)
			{
			case NetLanguage.CSharp:
				this.mSb.Append("]");
				this.mSb.Append(StringHelper.NewLine);
				break;
			case NetLanguage.VbNet:
				break;
			default:
				return;
			}
		}

		public string GetAttributesCode()
		{
			if (this.mFirst)
			{
				return string.Empty;
			}
			NetLanguage netLanguage = this.mLang;
			if (netLanguage == NetLanguage.VbNet)
			{
				this.mSb.Append("> _");
				this.mSb.Append(StringHelper.NewLine);
			}
			return this.mSb.ToString();
		}

		private readonly StringBuilder mSb = new StringBuilder(250);

		private readonly NetLanguage mLang;

		private bool mFirst = true;
	}
}
