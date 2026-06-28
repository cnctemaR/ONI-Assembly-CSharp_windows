using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Xml;

namespace FileHelpers.Dynamic
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class FieldBuilder
	{
		internal FieldBuilder(string fieldName, Type fieldType)
		{
			fieldName = fieldName.Trim();
			if (!ValidIdentifierValidator.ValidIdentifier(fieldName))
			{
				throw new FileHelpersException(Messages.Errors.InvalidIdentifier.Identifier(fieldName).Text);
			}
			this.mFieldName = fieldName;
			this.mFieldType = ClassBuilder.TypeToString(fieldType);
		}

		internal FieldBuilder(string fieldName, string fieldType)
		{
			fieldName = fieldName.Trim();
			if (!ValidIdentifierValidator.ValidIdentifier(fieldName))
			{
				throw new FileHelpersException(Messages.Errors.InvalidIdentifier.Identifier(fieldName).Text);
			}
			if (!ValidIdentifierValidator.ValidIdentifier(fieldType, true))
			{
				throw new FileHelpersException(Messages.Errors.InvalidIdentifier.Identifier(fieldType).Text);
			}
			this.mFieldName = fieldName;
			this.mFieldType = fieldType;
		}

		public TrimMode TrimMode
		{
			get
			{
				return this.mTrimMode;
			}
			set
			{
				this.mTrimMode = value;
			}
		}

		public string TrimChars
		{
			get
			{
				return this.mTrimChars;
			}
			set
			{
				this.mTrimChars = value;
			}
		}

		public int FieldIndex
		{
			get
			{
				return this.mFieldIndex;
			}
		}

		public bool FieldInNewLine
		{
			get
			{
				return this.mFieldInNewLine;
			}
			set
			{
				this.mFieldInNewLine = value;
			}
		}

		public bool FieldNotInFile
		{
			get
			{
				return this.mFieldNotInFile;
			}
			set
			{
				this.mFieldNotInFile = value;
			}
		}

		[Obsolete("Use [FieldNotInFile] instead")]
		public bool FieldIgnored
		{
			get
			{
				return this.mFieldNotInFile;
			}
			set
			{
				this.mFieldNotInFile = value;
			}
		}

		public bool FieldValueDiscarded
		{
			get
			{
				return this.mFieldValueDiscarded;
			}
			set
			{
				this.mFieldValueDiscarded = value;
			}
		}

		public bool FieldOptional
		{
			get
			{
				return this.mFieldOptional;
			}
			set
			{
				this.mFieldOptional = value;
			}
		}

		public ConverterBuilder Converter
		{
			get
			{
				return this.mConverter;
			}
		}

		public string FieldName
		{
			get
			{
				return this.mFieldName;
			}
			set
			{
				this.mFieldName = value;
			}
		}

		public string FieldType
		{
			get
			{
				return this.mFieldType;
			}
			set
			{
				this.mFieldType = value;
			}
		}

		public object FieldNullValue
		{
			get
			{
				return this.mFieldNullValue;
			}
			set
			{
				this.mFieldNullValue = value;
			}
		}

		public bool FieldNotEmpty
		{
			get
			{
				return this.mFieldNotEmpty;
			}
			set
			{
				this.mFieldNotEmpty = value;
			}
		}

		internal string GetFieldCode(NetLanguage lang)
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			AttributesBuilder attributesBuilder = new AttributesBuilder(lang);
			this.AddAttributesInternal(attributesBuilder, lang);
			this.AddAttributesCode(attributesBuilder, lang);
			stringBuilder.Append(attributesBuilder.GetAttributesCode());
			NetVisibility netVisibility = this.mVisibility;
			string text = this.mFieldName;
			if (this.mClassBuilder.GenerateProperties)
			{
				netVisibility = NetVisibility.Private;
				text = "m" + this.mFieldName;
			}
			switch (lang)
			{
			case NetLanguage.CSharp:
				stringBuilder.Append(string.Concat(new string[]
				{
					ClassBuilder.GetVisibility(lang, netVisibility),
					this.mFieldType,
					" ",
					text,
					";"
				}));
				break;
			case NetLanguage.VbNet:
				stringBuilder.Append(ClassBuilder.GetVisibility(lang, netVisibility) + text + " As " + this.mFieldType);
				break;
			}
			stringBuilder.Append(StringHelper.NewLine);
			if (this.mClassBuilder.GenerateProperties)
			{
				stringBuilder.Append(StringHelper.NewLine);
				switch (lang)
				{
				case NetLanguage.CSharp:
					stringBuilder.Append("public " + this.mFieldType + " " + this.mFieldName);
					stringBuilder.Append(StringHelper.NewLine);
					stringBuilder.Append("{");
					stringBuilder.Append(StringHelper.NewLine);
					stringBuilder.Append("   get { return m" + this.mFieldName + "; }");
					stringBuilder.Append(StringHelper.NewLine);
					stringBuilder.Append("   set { m" + this.mFieldName + " = value; }");
					stringBuilder.Append(StringHelper.NewLine);
					stringBuilder.Append("}");
					break;
				case NetLanguage.VbNet:
					stringBuilder.Append("Public Property " + this.mFieldName + " As " + this.mFieldType);
					stringBuilder.Append(StringHelper.NewLine);
					stringBuilder.Append("   Get");
					stringBuilder.Append(StringHelper.NewLine);
					stringBuilder.Append("      Return m" + this.mFieldName);
					stringBuilder.Append(StringHelper.NewLine);
					stringBuilder.Append("   End Get");
					stringBuilder.Append(StringHelper.NewLine);
					stringBuilder.Append("   Set (value As " + this.mFieldType + ")");
					stringBuilder.Append(StringHelper.NewLine);
					stringBuilder.Append("      m" + this.mFieldName + " = value");
					stringBuilder.Append(StringHelper.NewLine);
					stringBuilder.Append("   End Set");
					stringBuilder.Append(StringHelper.NewLine);
					stringBuilder.Append("End Property");
					break;
				}
				stringBuilder.Append(StringHelper.NewLine);
				stringBuilder.Append(StringHelper.NewLine);
			}
			return stringBuilder.ToString();
		}

		internal abstract void AddAttributesCode(AttributesBuilder attbs, NetLanguage lang);

		private void AddAttributesInternal(AttributesBuilder attbs, NetLanguage lang)
		{
			if (this.mFieldOptional)
			{
				attbs.AddAttribute("FieldOptional()");
			}
			if (this.mFieldNotInFile)
			{
				attbs.AddAttribute("FieldNotInFile()");
			}
			if (this.mFieldValueDiscarded)
			{
				attbs.AddAttribute("FieldValueDiscarded()");
			}
			if (this.mFieldInNewLine)
			{
				attbs.AddAttribute("FieldInNewLine()");
			}
			if (this.mFieldNotEmpty)
			{
				attbs.AddAttribute("FieldNotEmpty()");
			}
			if (this.mFieldNullValue != null)
			{
				if (this.mFieldNullValue is string)
				{
					attbs.AddAttribute("FieldNullValue(\"" + this.mFieldNullValue.ToString() + "\")");
				}
				else
				{
					string text = ClassBuilder.TypeToString(this.mFieldNullValue.GetType());
					string text2 = string.Empty;
					if (lang == NetLanguage.CSharp)
					{
						text2 = "typeof(" + text + ")";
					}
					else if (lang == NetLanguage.VbNet)
					{
						text2 = "GetType(" + text + ")";
					}
					attbs.AddAttribute(string.Concat(new string[]
					{
						"FieldNullValue(",
						text2,
						", \"",
						this.mFieldNullValue.ToString(),
						"\")"
					}));
				}
			}
			attbs.AddAttribute(this.mConverter.GetConverterCode(lang));
			if (this.mTrimMode != TrimMode.None)
			{
				if (" \t" == this.mTrimChars)
				{
					attbs.AddAttribute("FieldTrim(TrimMode." + this.mTrimMode.ToString() + ")");
					return;
				}
				attbs.AddAttribute(string.Concat(new string[]
				{
					"FieldTrim(TrimMode.",
					this.mTrimMode.ToString(),
					", \"",
					this.mTrimChars.ToString(),
					"\")"
				}));
			}
		}

		public NetVisibility Visibility
		{
			get
			{
				return this.mVisibility;
			}
			set
			{
				this.mVisibility = value;
			}
		}

		internal void SaveToXml(XmlHelper writer)
		{
			writer.Writer.WriteStartElement("Field");
			writer.Writer.WriteStartAttribute("Name", "");
			writer.Writer.WriteString(this.mFieldName);
			writer.Writer.WriteEndAttribute();
			writer.Writer.WriteStartAttribute("Type", "");
			writer.Writer.WriteString(this.mFieldType);
			writer.Writer.WriteEndAttribute();
			this.WriteHeaderAttributes(writer);
			this.Converter.WriteXml(writer);
			writer.WriteElement("Visibility", this.Visibility.ToString(), "Public");
			writer.WriteElement("FieldNotInFile", this.FieldNotInFile);
			writer.WriteElement("FieldOptional", this.FieldOptional);
			writer.WriteElement("FieldValueDiscarded", this.FieldValueDiscarded);
			writer.WriteElement("FieldInNewLine", this.FieldInNewLine);
			writer.WriteElement("TrimChars", this.TrimChars, " \t");
			writer.WriteElement("TrimMode", this.TrimMode.ToString(), "None");
			if (this.FieldNullValue != null)
			{
				writer.Writer.WriteStartElement("FieldNullValue");
				writer.Writer.WriteStartAttribute("Type", "");
				writer.Writer.WriteString(ClassBuilder.TypeToString(this.mFieldNullValue.GetType()));
				writer.Writer.WriteEndAttribute();
				writer.Writer.WriteString(this.mFieldNullValue.ToString());
				writer.Writer.WriteEndElement();
			}
			this.WriteExtraElements(writer);
			writer.Writer.WriteEndElement();
		}

		internal abstract void WriteHeaderAttributes(XmlHelper writer);

		internal abstract void WriteExtraElements(XmlHelper writer);

		internal void ReadField(XmlNode node)
		{
			XmlNode xmlNode = node["Visibility"];
			if (xmlNode != null)
			{
				this.Visibility = (NetVisibility)Enum.Parse(typeof(NetVisibility), xmlNode.InnerText);
			}
			this.FieldNotInFile = node["FieldIgnored"] != null || node["FieldNotInFile"] != null;
			this.FieldValueDiscarded = node["FieldValueDiscarded"] != null;
			this.FieldOptional = node["FieldOptional"] != null;
			this.FieldInNewLine = node["FieldInNewLine"] != null;
			xmlNode = node["TrimChars"];
			if (xmlNode != null)
			{
				this.TrimChars = xmlNode.InnerText;
			}
			xmlNode = node["TrimMode"];
			if (xmlNode != null)
			{
				this.TrimMode = (TrimMode)Enum.Parse(typeof(TrimMode), xmlNode.InnerText);
			}
			xmlNode = node["FieldNullValue"];
			if (xmlNode != null)
			{
				this.FieldNullValue = Convert.ChangeType(xmlNode.InnerText, Type.GetType(xmlNode.Attributes["Type"].InnerText));
			}
			xmlNode = node["Converter"];
			if (xmlNode != null)
			{
				this.Converter.LoadXml(xmlNode);
			}
			this.ReadFieldInternal(node);
		}

		internal abstract void ReadFieldInternal(XmlNode node);

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string mFieldName;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string mFieldType;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private TrimMode mTrimMode;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string mTrimChars = " \t";

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal int mFieldIndex = -1;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool mFieldInNewLine;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool mFieldNotInFile;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool mFieldValueDiscarded;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool mFieldOptional;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private object mFieldNullValue;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool mFieldNotEmpty;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ConverterBuilder mConverter = new ConverterBuilder();

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal ClassBuilder mClassBuilder;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private NetVisibility mVisibility;
	}
}
