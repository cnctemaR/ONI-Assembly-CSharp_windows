using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Net.Mail;
using System.Text;

namespace System.Net.Mime
{
	public class ContentDisposition
	{
		static ContentDisposition()
		{
			ContentDisposition.validators.Add("creation-date", ContentDisposition.dateParser);
			ContentDisposition.validators.Add("modification-date", ContentDisposition.dateParser);
			ContentDisposition.validators.Add("read-date", ContentDisposition.dateParser);
			ContentDisposition.validators.Add("size", ContentDisposition.longParser);
		}

		public ContentDisposition()
		{
			this.isChanged = true;
			this.dispositionType = "attachment";
			this.disposition = this.dispositionType;
		}

		public ContentDisposition(string disposition)
		{
			if (disposition == null)
			{
				throw new ArgumentNullException("disposition");
			}
			this.isChanged = true;
			this.disposition = disposition;
			this.ParseValue();
		}

		internal DateTime GetDateParameter(string parameterName)
		{
			SmtpDateTime smtpDateTime = ((TrackingValidationObjectDictionary)this.Parameters).InternalGet(parameterName) as SmtpDateTime;
			if (smtpDateTime == null)
			{
				return DateTime.MinValue;
			}
			return smtpDateTime.Date;
		}

		public string DispositionType
		{
			get
			{
				return this.dispositionType;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value == string.Empty)
				{
					throw new ArgumentException(global::SR.GetString("This property cannot be set to an empty string."), "value");
				}
				this.isChanged = true;
				this.dispositionType = value;
			}
		}

		public StringDictionary Parameters
		{
			get
			{
				if (this.parameters == null)
				{
					this.parameters = new TrackingValidationObjectDictionary(ContentDisposition.validators);
				}
				return this.parameters;
			}
		}

		public string FileName
		{
			get
			{
				return this.Parameters["filename"];
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.Parameters.Remove("filename");
					return;
				}
				this.Parameters["filename"] = value;
			}
		}

		public DateTime CreationDate
		{
			get
			{
				return this.GetDateParameter("creation-date");
			}
			set
			{
				SmtpDateTime smtpDateTime = new SmtpDateTime(value);
				((TrackingValidationObjectDictionary)this.Parameters).InternalSet("creation-date", smtpDateTime);
			}
		}

		public DateTime ModificationDate
		{
			get
			{
				return this.GetDateParameter("modification-date");
			}
			set
			{
				SmtpDateTime smtpDateTime = new SmtpDateTime(value);
				((TrackingValidationObjectDictionary)this.Parameters).InternalSet("modification-date", smtpDateTime);
			}
		}

		public bool Inline
		{
			get
			{
				return this.dispositionType == "inline";
			}
			set
			{
				this.isChanged = true;
				if (value)
				{
					this.dispositionType = "inline";
					return;
				}
				this.dispositionType = "attachment";
			}
		}

		public DateTime ReadDate
		{
			get
			{
				return this.GetDateParameter("read-date");
			}
			set
			{
				SmtpDateTime smtpDateTime = new SmtpDateTime(value);
				((TrackingValidationObjectDictionary)this.Parameters).InternalSet("read-date", smtpDateTime);
			}
		}

		public long Size
		{
			get
			{
				object obj = ((TrackingValidationObjectDictionary)this.Parameters).InternalGet("size");
				if (obj == null)
				{
					return -1L;
				}
				return (long)obj;
			}
			set
			{
				((TrackingValidationObjectDictionary)this.Parameters).InternalSet("size", value);
			}
		}

		internal void Set(string contentDisposition, HeaderCollection headers)
		{
			this.disposition = contentDisposition;
			this.ParseValue();
			headers.InternalSet(MailHeaderInfo.GetString(MailHeaderID.ContentDisposition), this.ToString());
			this.isPersisted = true;
		}

		internal void PersistIfNeeded(HeaderCollection headers, bool forcePersist)
		{
			if (this.IsChanged || !this.isPersisted || forcePersist)
			{
				headers.InternalSet(MailHeaderInfo.GetString(MailHeaderID.ContentDisposition), this.ToString());
				this.isPersisted = true;
			}
		}

		internal bool IsChanged
		{
			get
			{
				return this.isChanged || (this.parameters != null && this.parameters.IsChanged);
			}
		}

		public override string ToString()
		{
			if (this.disposition == null || this.isChanged || (this.parameters != null && this.parameters.IsChanged))
			{
				this.disposition = this.Encode(false);
				this.isChanged = false;
				this.parameters.IsChanged = false;
				this.isPersisted = false;
			}
			return this.disposition;
		}

		internal string Encode(bool allowUnicode)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.dispositionType);
			foreach (object obj in this.Parameters.Keys)
			{
				string text = (string)obj;
				stringBuilder.Append("; ");
				ContentDisposition.EncodeToBuffer(text, stringBuilder, allowUnicode);
				stringBuilder.Append('=');
				ContentDisposition.EncodeToBuffer(this.parameters[text], stringBuilder, allowUnicode);
			}
			return stringBuilder.ToString();
		}

		private static void EncodeToBuffer(string value, StringBuilder builder, bool allowUnicode)
		{
			Encoding encoding = MimeBasePart.DecodeEncoding(value);
			if (encoding != null)
			{
				builder.Append("\"" + value + "\"");
				return;
			}
			if ((allowUnicode && !MailBnfHelper.HasCROrLF(value)) || MimeBasePart.IsAscii(value, false))
			{
				MailBnfHelper.GetTokenOrQuotedString(value, builder, allowUnicode);
				return;
			}
			encoding = Encoding.GetEncoding("utf-8");
			builder.Append("\"" + MimeBasePart.EncodeHeaderValue(value, encoding, MimeBasePart.ShouldUseBase64Encoding(encoding)) + "\"");
		}

		public override bool Equals(object rparam)
		{
			return rparam != null && string.Compare(this.ToString(), rparam.ToString(), StringComparison.OrdinalIgnoreCase) == 0;
		}

		public override int GetHashCode()
		{
			return this.ToString().ToLowerInvariant().GetHashCode();
		}

		private void ParseValue()
		{
			int num = 0;
			try
			{
				this.dispositionType = MailBnfHelper.ReadToken(this.disposition, ref num, null);
				if (string.IsNullOrEmpty(this.dispositionType))
				{
					throw new FormatException(global::SR.GetString("The mail header is malformed."));
				}
				if (this.parameters == null)
				{
					this.parameters = new TrackingValidationObjectDictionary(ContentDisposition.validators);
				}
				else
				{
					this.parameters.Clear();
				}
				while (MailBnfHelper.SkipCFWS(this.disposition, ref num))
				{
					if (this.disposition[num++] != ';')
					{
						throw new FormatException(global::SR.GetString("An invalid character was found in the mail header: '{0}'.", new object[] { this.disposition[num - 1] }));
					}
					if (!MailBnfHelper.SkipCFWS(this.disposition, ref num))
					{
						break;
					}
					string text = MailBnfHelper.ReadParameterAttribute(this.disposition, ref num, null);
					if (this.disposition[num++] != '=')
					{
						throw new FormatException(global::SR.GetString("The mail header is malformed."));
					}
					if (!MailBnfHelper.SkipCFWS(this.disposition, ref num))
					{
						throw new FormatException(global::SR.GetString("The specified content disposition is invalid."));
					}
					string text2;
					if (this.disposition[num] == '"')
					{
						text2 = MailBnfHelper.ReadQuotedString(this.disposition, ref num, null);
					}
					else
					{
						text2 = MailBnfHelper.ReadToken(this.disposition, ref num, null);
					}
					if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text2))
					{
						throw new FormatException(global::SR.GetString("The specified content disposition is invalid."));
					}
					this.Parameters.Add(text, text2);
				}
			}
			catch (FormatException ex)
			{
				throw new FormatException(global::SR.GetString("The specified content disposition is invalid."), ex);
			}
			this.parameters.IsChanged = false;
		}

		private string dispositionType;

		private TrackingValidationObjectDictionary parameters;

		private bool isChanged;

		private bool isPersisted;

		private string disposition;

		private const string creationDate = "creation-date";

		private const string readDate = "read-date";

		private const string modificationDate = "modification-date";

		private const string size = "size";

		private const string fileName = "filename";

		private static readonly TrackingValidationObjectDictionary.ValidateAndParseValue dateParser = (object value) => new SmtpDateTime(value.ToString());

		private static readonly TrackingValidationObjectDictionary.ValidateAndParseValue longParser = delegate(object value)
		{
			long num;
			if (!long.TryParse(value.ToString(), NumberStyles.None, CultureInfo.InvariantCulture, out num))
			{
				throw new FormatException(global::SR.GetString("The specified content disposition is invalid."));
			}
			return num;
		};

		private static readonly IDictionary<string, TrackingValidationObjectDictionary.ValidateAndParseValue> validators = new Dictionary<string, TrackingValidationObjectDictionary.ValidateAndParseValue>();
	}
}
