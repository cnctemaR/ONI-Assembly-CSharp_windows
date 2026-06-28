using System;
using System.Diagnostics;
using System.Xml;

namespace FileHelpers.Dynamic
{
	public sealed class DelimitedFieldBuilder : FieldBuilder
	{
		internal DelimitedFieldBuilder(string fieldName, string fieldType)
			: base(fieldName, fieldType)
		{
		}

		internal DelimitedFieldBuilder(string fieldName, Type fieldType)
			: base(fieldName, fieldType)
		{
		}

		public bool FieldQuoted
		{
			get
			{
				return this.mFieldQuoted;
			}
			set
			{
				this.mFieldQuoted = value;
			}
		}

		public char QuoteChar
		{
			get
			{
				return this.mQuoteChar;
			}
			set
			{
				this.mQuoteChar = value;
			}
		}

		public QuoteMode QuoteMode
		{
			get
			{
				return this.mQuoteMode;
			}
			set
			{
				this.mQuoteMode = value;
			}
		}

		public MultilineMode QuoteMultiline
		{
			get
			{
				return this.mQuoteMultiline;
			}
			set
			{
				this.mQuoteMultiline = value;
			}
		}

		internal override void AddAttributesCode(AttributesBuilder attbs, NetLanguage lang)
		{
			if (this.mFieldQuoted)
			{
				if (lang == NetLanguage.CSharp)
				{
					string text = this.mQuoteChar.ToString();
					if (this.mQuoteChar == '\'')
					{
						text = "\\'";
					}
					attbs.AddAttribute(string.Concat(new string[]
					{
						"FieldQuoted('",
						text,
						"', QuoteMode.",
						this.mQuoteMode.ToString(),
						", MultilineMode.",
						this.mQuoteMultiline.ToString(),
						")"
					}));
					return;
				}
				if (lang == NetLanguage.VbNet)
				{
					string text2 = this.mQuoteChar.ToString();
					if (this.mQuoteChar == '"')
					{
						text2 = "\"\"";
					}
					attbs.AddAttribute(string.Concat(new string[]
					{
						"FieldQuoted(\"",
						text2,
						"\"c, QuoteMode.",
						this.mQuoteMode.ToString(),
						", MultilineMode.",
						this.mQuoteMultiline.ToString(),
						")"
					}));
				}
			}
		}

		internal override void WriteHeaderAttributes(XmlHelper writer)
		{
		}

		internal override void WriteExtraElements(XmlHelper writer)
		{
			writer.WriteElement("FieldQuoted", this.FieldQuoted);
			writer.WriteElement("QuoteChar", this.QuoteChar.ToString(), "\"");
			writer.WriteElement("QuoteMode", this.QuoteMode.ToString(), "OptionalForRead");
			writer.WriteElement("QuoteMultiline", this.QuoteMultiline.ToString(), "AllowForRead");
		}

		internal override void ReadFieldInternal(XmlNode node)
		{
			this.FieldQuoted = node["FieldQuoted"] != null;
			XmlNode xmlNode = node["QuoteChar"];
			if (xmlNode != null && xmlNode.InnerText.Length > 0)
			{
				this.QuoteChar = xmlNode.InnerText[0];
			}
			xmlNode = node["QuoteMode"];
			if (xmlNode != null && xmlNode.InnerText.Length > 0)
			{
				this.QuoteMode = (QuoteMode)Enum.Parse(typeof(QuoteMode), xmlNode.InnerText);
			}
			xmlNode = node["QuoteMultiline"];
			if (xmlNode != null && xmlNode.InnerText.Length > 0)
			{
				this.QuoteMultiline = (MultilineMode)Enum.Parse(typeof(MultilineMode), xmlNode.InnerText);
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool mFieldQuoted;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private char mQuoteChar = '"';

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private QuoteMode mQuoteMode = QuoteMode.OptionalForRead;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private MultilineMode mQuoteMultiline = MultilineMode.AllowForRead;
	}
}
