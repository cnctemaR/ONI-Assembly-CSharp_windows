using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace System.Xml
{
	internal class ContentTypeHeader : MimeHeader
	{
		public ContentTypeHeader(string value)
			: base("content-type", value)
		{
		}

		public string MediaType
		{
			get
			{
				if (this.mediaType == null && base.Value != null)
				{
					this.ParseValue();
				}
				return this.mediaType;
			}
		}

		public string MediaSubtype
		{
			get
			{
				if (this.subType == null && base.Value != null)
				{
					this.ParseValue();
				}
				return this.subType;
			}
		}

		public Dictionary<string, string> Parameters
		{
			get
			{
				if (this.parameters == null)
				{
					if (base.Value != null)
					{
						this.ParseValue();
					}
					else
					{
						this.parameters = new Dictionary<string, string>();
					}
				}
				return this.parameters;
			}
		}

		private void ParseValue()
		{
			if (this.parameters == null)
			{
				int num = 0;
				this.parameters = new Dictionary<string, string>();
				this.mediaType = MailBnfHelper.ReadToken(base.Value, ref num, null);
				if (num >= base.Value.Length || base.Value[num++] != '/')
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(global::System.Runtime.Serialization.SR.GetString("MIME content type header is invalid.")));
				}
				this.subType = MailBnfHelper.ReadToken(base.Value, ref num, null);
				while (MailBnfHelper.SkipCFWS(base.Value, ref num))
				{
					if (num >= base.Value.Length || base.Value[num++] != ';')
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(global::System.Runtime.Serialization.SR.GetString("MIME content type header is invalid.")));
					}
					if (!MailBnfHelper.SkipCFWS(base.Value, ref num))
					{
						break;
					}
					string text = MailBnfHelper.ReadParameterAttribute(base.Value, ref num, null);
					if (text == null || num >= base.Value.Length || base.Value[num++] != '=')
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(global::System.Runtime.Serialization.SR.GetString("MIME content type header is invalid.")));
					}
					string text2 = MailBnfHelper.ReadParameterValue(base.Value, ref num, null);
					this.parameters.Add(text.ToLowerInvariant(), text2);
				}
				if (this.parameters.ContainsKey(MtomGlobals.StartInfoParam))
				{
					string text3 = this.parameters[MtomGlobals.StartInfoParam];
					int num2 = text3.IndexOf(';');
					if (num2 > -1)
					{
						while (MailBnfHelper.SkipCFWS(text3, ref num2))
						{
							if (text3[num2] != ';')
							{
								throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(global::System.Runtime.Serialization.SR.GetString("MIME content type header is invalid.")));
							}
							num2++;
							string text4 = MailBnfHelper.ReadParameterAttribute(text3, ref num2, null);
							if (text4 == null || num2 >= text3.Length || text3[num2++] != '=')
							{
								throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(global::System.Runtime.Serialization.SR.GetString("MIME content type header is invalid.")));
							}
							string text5 = MailBnfHelper.ReadParameterValue(text3, ref num2, null);
							if (text4 == MtomGlobals.ActionParam)
							{
								this.parameters[MtomGlobals.ActionParam] = text5;
							}
						}
					}
				}
			}
		}

		public static readonly ContentTypeHeader Default = new ContentTypeHeader("application/octet-stream");

		private string mediaType;

		private string subType;

		private Dictionary<string, string> parameters;
	}
}
