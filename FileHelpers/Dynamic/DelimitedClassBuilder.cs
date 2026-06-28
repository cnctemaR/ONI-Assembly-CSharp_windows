using System;
using System.Data;
using System.Diagnostics;
using System.Xml;

namespace FileHelpers.Dynamic
{
	public class DelimitedClassBuilder : ClassBuilder
	{
		public string Delimiter
		{
			get
			{
				return this.mDelimiter;
			}
			set
			{
				this.mDelimiter = value;
			}
		}

		public new DelimitedFieldBuilder FieldByIndex(int index)
		{
			return (DelimitedFieldBuilder)base.FieldByIndex(index);
		}

		public new DelimitedFieldBuilder[] Fields
		{
			get
			{
				return (DelimitedFieldBuilder[])this.mFields.ToArray(typeof(DelimitedFieldBuilder));
			}
		}

		public DelimitedClassBuilder(string className, string delimiter)
			: base(className)
		{
			this.mDelimiter = delimiter;
		}

		public DelimitedClassBuilder(string className)
			: this(className, string.Empty)
		{
		}

		public DelimitedClassBuilder(string className, string delimiter, DataTable dt)
			: this(className, delimiter)
		{
			foreach (object obj in dt.Columns)
			{
				DataColumn dataColumn = (DataColumn)obj;
				this.AddField(StringHelper.ToValidIdentifier(dataColumn.ColumnName), dataColumn.DataType);
			}
		}

		public virtual DelimitedFieldBuilder AddField(string fieldName, string fieldType)
		{
			DelimitedFieldBuilder delimitedFieldBuilder = new DelimitedFieldBuilder(fieldName, fieldType);
			base.AddFieldInternal(delimitedFieldBuilder);
			return delimitedFieldBuilder;
		}

		public DelimitedFieldBuilder AddField(string fieldName, Type fieldType)
		{
			return this.AddField(fieldName, ClassBuilder.TypeToString(fieldType));
		}

		public virtual DelimitedFieldBuilder AddField(string fieldName)
		{
			return this.AddField(fieldName, "System.String");
		}

		public DelimitedFieldBuilder AddField(DelimitedFieldBuilder field)
		{
			base.AddFieldInternal(field);
			return field;
		}

		public DelimitedFieldBuilder LastField
		{
			get
			{
				if (this.mFields.Count == 0)
				{
					return null;
				}
				return (DelimitedFieldBuilder)this.mFields[this.mFields.Count - 1];
			}
		}

		internal override void AddAttributesCode(AttributesBuilder attbs, NetLanguage lang)
		{
			if (this.mDelimiter == string.Empty)
			{
				throw new BadUsageException("The Delimiter of the DelimiterClassBuilder can't be null or empty.");
			}
			attbs.AddAttribute("DelimitedRecord(" + DelimitedClassBuilder.GetDelimiter(this.mDelimiter, lang) + ")");
		}

		private static string GetDelimiter(string delimiter, NetLanguage lang)
		{
			switch (lang)
			{
			case NetLanguage.CSharp:
				if (delimiter == "\t")
				{
					return "\"\\t\"";
				}
				return "\"" + delimiter + "\"";
			case NetLanguage.VbNet:
				if (delimiter == "\t")
				{
					return "VbTab";
				}
				return "\"" + delimiter + "\"";
			default:
				throw new ArgumentOutOfRangeException("lang");
			}
		}

		internal override void WriteHeaderElement(XmlHelper writer)
		{
			writer.Writer.WriteStartElement("DelimitedClass");
			writer.Writer.WriteStartAttribute("Delimiter", "");
			writer.Writer.WriteString(this.Delimiter);
			writer.Writer.WriteEndAttribute();
		}

		internal override void WriteExtraElements(XmlHelper writer)
		{
		}

		internal static DelimitedClassBuilder LoadXmlInternal(XmlDocument document)
		{
			string value = document.SelectNodes("/DelimitedClass")[0].Attributes["Delimiter"].Value;
			string innerText = document.SelectNodes("/DelimitedClass/ClassName")[0].InnerText;
			return new DelimitedClassBuilder(innerText, value);
		}

		internal override void ReadClassElements(XmlDocument document)
		{
		}

		internal override void ReadField(XmlNode node)
		{
			this.AddField(node.Attributes.Item(0).InnerText, node.Attributes.Item(1).InnerText).ReadField(node);
		}

		public virtual void AddFields(int numberOfFields)
		{
			for (int i = 0; i < numberOfFields; i++)
			{
				this.AddField("Field" + (i + 1).ToString().PadLeft(4, '0'));
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string mDelimiter = string.Empty;
	}
}
