using System;
using System.Runtime.Serialization;
using System.Text;

namespace System.Xml
{
	internal class MimeVersionHeader : MimeHeader
	{
		public MimeVersionHeader(string value)
			: base("mime-version", value)
		{
		}

		public string Version
		{
			get
			{
				if (this.version == null && base.Value != null)
				{
					this.ParseValue();
				}
				return this.version;
			}
		}

		private void ParseValue()
		{
			if (base.Value == "1.0")
			{
				this.version = "1.0";
				return;
			}
			int num = 0;
			if (!MailBnfHelper.SkipCFWS(base.Value, ref num))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(global::System.Runtime.Serialization.SR.GetString("MIME version header is invalid.")));
			}
			StringBuilder stringBuilder = new StringBuilder();
			MailBnfHelper.ReadDigits(base.Value, ref num, stringBuilder);
			if (!MailBnfHelper.SkipCFWS(base.Value, ref num) || num >= base.Value.Length || base.Value[num++] != '.' || !MailBnfHelper.SkipCFWS(base.Value, ref num))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(global::System.Runtime.Serialization.SR.GetString("MIME version header is invalid.")));
			}
			stringBuilder.Append('.');
			MailBnfHelper.ReadDigits(base.Value, ref num, stringBuilder);
			this.version = stringBuilder.ToString();
		}

		public static readonly MimeVersionHeader Default = new MimeVersionHeader("1.0");

		private string version;
	}
}
