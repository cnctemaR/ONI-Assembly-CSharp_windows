using System;
using System.Data;
using System.Diagnostics;
using System.Xml;

namespace FileHelpers.Dynamic
{
	public sealed class FixedLengthClassBuilder : ClassBuilder
	{
		public new FixedFieldBuilder FieldByIndex(int index)
		{
			return (FixedFieldBuilder)base.FieldByIndex(index);
		}

		public new FixedFieldBuilder[] Fields
		{
			get
			{
				return (FixedFieldBuilder[])this.mFields.ToArray(typeof(FixedFieldBuilder));
			}
		}

		public FixedLengthClassBuilder(string className)
			: base(className)
		{
		}

		public FixedLengthClassBuilder(string className, params int[] lengths)
			: base(className)
		{
			for (int i = 0; i < lengths.Length; i++)
			{
				this.AddField("Field" + (i + 1).ToString(), lengths[i], typeof(string));
			}
		}

		public FixedLengthClassBuilder(string className, DataTable dt, int defaultLength)
			: this(className)
		{
			foreach (object obj in dt.Columns)
			{
				DataColumn dataColumn = (DataColumn)obj;
				this.AddField(StringHelper.ToValidIdentifier(dataColumn.ColumnName), defaultLength, dataColumn.DataType);
			}
		}

		public FixedLengthClassBuilder(string className, FixedMode mode)
			: base(className)
		{
			this.mFixedMode = mode;
		}

		public FixedFieldBuilder AddField(string fieldName, int length, string fieldType)
		{
			FixedFieldBuilder fixedFieldBuilder = new FixedFieldBuilder(fieldName, length, fieldType);
			base.AddFieldInternal(fixedFieldBuilder);
			return fixedFieldBuilder;
		}

		public FixedFieldBuilder AddField(string fieldName, int length, Type fieldType)
		{
			return this.AddField(fieldName, length, ClassBuilder.TypeToString(fieldType));
		}

		public FixedFieldBuilder AddField(FixedFieldBuilder field)
		{
			base.AddFieldInternal(field);
			return field;
		}

		public FixedFieldBuilder LastField
		{
			get
			{
				if (this.mFields.Count == 0)
				{
					return null;
				}
				return (FixedFieldBuilder)this.mFields[this.mFields.Count - 1];
			}
		}

		internal override void AddAttributesCode(AttributesBuilder attbs, NetLanguage lang)
		{
			attbs.AddAttribute("FixedLengthRecord(FixedMode." + this.mFixedMode.ToString() + ")");
		}

		internal override void WriteHeaderElement(XmlHelper writer)
		{
			writer.Writer.WriteStartElement("FixedLengthClass");
			writer.Writer.WriteStartAttribute("FixedMode", "");
			writer.Writer.WriteString(this.mFixedMode.ToString());
			writer.Writer.WriteEndAttribute();
		}

		internal override void WriteExtraElements(XmlHelper writer)
		{
		}

		public void SetFieldsLength(params int[] lengths)
		{
			if (lengths.Length != this.mFields.Count)
			{
				throw new BadUsageException(string.Format("The number of elements is {0} and you pass {1}. This method require the same number of values than fields", this.mFields.Count, lengths.Length));
			}
			for (int i = 0; i < this.mFields.Count; i++)
			{
				this.FieldByIndex(i).FieldLength = lengths[i];
			}
		}

		public FixedMode FixedMode
		{
			get
			{
				return this.mFixedMode;
			}
			set
			{
				this.mFixedMode = value;
			}
		}

		internal override void ReadClassElements(XmlDocument document)
		{
		}

		internal override void ReadField(XmlNode node)
		{
			this.AddField(node.Attributes.Item(0).InnerText, int.Parse(node.Attributes.Item(2).InnerText), node.Attributes.Item(1).InnerText).ReadField(node);
		}

		internal static FixedLengthClassBuilder LoadXmlInternal(XmlDocument document)
		{
			FixedMode fixedMode = (FixedMode)Enum.Parse(typeof(FixedMode), document.SelectNodes("/FixedLengthClass")[0].Attributes["FixedMode"].Value);
			string innerText = document.SelectNodes("/FixedLengthClass/ClassName")[0].InnerText;
			return new FixedLengthClassBuilder(innerText, fixedMode);
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private FixedMode mFixedMode;
	}
}
