using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace FileHelpers.Dynamic
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class ConverterBuilder
	{
		internal ConverterBuilder()
		{
		}

		public ConverterKind Kind
		{
			get
			{
				return this.mKind;
			}
			set
			{
				this.mKind = value;
			}
		}

		public string TypeName
		{
			get
			{
				return this.mTypeName;
			}
			set
			{
				this.mTypeName = value;
			}
		}

		public string Arg1
		{
			get
			{
				return this.mArg1;
			}
			set
			{
				this.mArg1 = value;
			}
		}

		public string Arg2
		{
			get
			{
				return this.mArg2;
			}
			set
			{
				this.mArg2 = value;
			}
		}

		public string Arg3
		{
			get
			{
				return this.mArg3;
			}
			set
			{
				this.mArg3 = value;
			}
		}

		internal void WriteXml(XmlHelper writer)
		{
			if (this.mKind == ConverterKind.None && this.mTypeName == string.Empty)
			{
				return;
			}
			writer.Writer.WriteStartElement("Converter");
			writer.WriteAttribute("Kind", this.Kind.ToString(), "None");
			writer.WriteAttribute("TypeName", this.mTypeName.ToString(), string.Empty);
			writer.WriteAttribute("Arg1", this.Arg1, string.Empty);
			writer.WriteAttribute("Arg2", this.Arg2, string.Empty);
			writer.WriteAttribute("Arg3", this.Arg3, string.Empty);
			writer.Writer.WriteEndElement();
		}

		internal void LoadXml(XmlNode node)
		{
			XmlAttribute xmlAttribute = node.Attributes["Kind"];
			if (xmlAttribute != null)
			{
				this.Kind = (ConverterKind)Enum.Parse(typeof(ConverterKind), xmlAttribute.InnerText);
			}
			xmlAttribute = node.Attributes["TypeName"];
			if (xmlAttribute != null)
			{
				this.TypeName = xmlAttribute.InnerText;
			}
			xmlAttribute = node.Attributes["Arg1"];
			if (xmlAttribute != null)
			{
				this.Arg1 = xmlAttribute.InnerText;
			}
			xmlAttribute = node.Attributes["Arg2"];
			if (xmlAttribute != null)
			{
				this.Arg2 = xmlAttribute.InnerText;
			}
			xmlAttribute = node.Attributes["Arg3"];
			if (xmlAttribute != null)
			{
				this.Arg3 = xmlAttribute.InnerText;
			}
		}

		internal string GetConverterCode(NetLanguage lang)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.mKind != ConverterKind.None)
			{
				stringBuilder.Append("FieldConverter(ConverterKind." + this.mKind.ToString());
			}
			else
			{
				if (!(this.mTypeName != string.Empty))
				{
					return string.Empty;
				}
				if (lang == NetLanguage.CSharp)
				{
					stringBuilder.Append("FieldConverter(typeof(" + this.mTypeName + ")");
				}
				else if (lang == NetLanguage.VbNet)
				{
					stringBuilder.Append("FieldConverter(GetType(" + this.mTypeName + ")");
				}
			}
			if (!string.IsNullOrEmpty(this.mArg1))
			{
				stringBuilder.Append(", \"" + this.mArg1 + "\"");
				if (!string.IsNullOrEmpty(this.mArg2))
				{
					stringBuilder.Append(", \"" + this.mArg2 + "\"");
					if (!string.IsNullOrEmpty(this.mArg3))
					{
						stringBuilder.Append(", \"" + this.mArg3 + "\"");
					}
				}
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		private ConverterKind mKind;

		private string mTypeName = string.Empty;

		private string mArg1 = string.Empty;

		private string mArg2 = string.Empty;

		private string mArg3 = string.Empty;
	}
}
