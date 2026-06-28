using System;
using System.Diagnostics;
using System.Xml;

namespace FileHelpers.Dynamic
{
	public sealed class FixedFieldBuilder : FieldBuilder
	{
		internal FixedFieldBuilder(string fieldName, int length, Type fieldType)
			: this(fieldName, length, ClassBuilder.TypeToString(fieldType))
		{
		}

		internal FixedFieldBuilder(string fieldName, int length, string fieldType)
			: base(fieldName, fieldType)
		{
			this.mFieldLength = length;
		}

		public int FieldLength
		{
			get
			{
				return this.mFieldLength;
			}
			set
			{
				this.mFieldLength = value;
			}
		}

		public AlignMode AlignMode
		{
			get
			{
				return this.mAlignMode;
			}
			set
			{
				this.mAlignMode = value;
			}
		}

		public char AlignChar
		{
			get
			{
				return this.mAlignChar;
			}
			set
			{
				this.mAlignChar = value;
			}
		}

		internal override void AddAttributesCode(AttributesBuilder attbs, NetLanguage lang)
		{
			if (this.mFieldLength <= 0)
			{
				throw new BadUsageException("The Length of each field must be grater than 0");
			}
			attbs.AddAttribute("FieldFixedLength(" + this.mFieldLength.ToString() + ")");
			if (this.mAlignMode != AlignMode.Left)
			{
				if (lang == NetLanguage.CSharp)
				{
					attbs.AddAttribute(string.Concat(new string[]
					{
						"FieldAlign(AlignMode.",
						this.mAlignMode.ToString(),
						", '",
						this.mAlignChar.ToString(),
						"')"
					}));
					return;
				}
				if (lang == NetLanguage.VbNet)
				{
					attbs.AddAttribute(string.Concat(new string[]
					{
						"FieldAlign(AlignMode.",
						this.mAlignMode.ToString(),
						", \"",
						this.mAlignChar.ToString(),
						"\"c)"
					}));
				}
			}
		}

		internal override void WriteHeaderAttributes(XmlHelper writer)
		{
			writer.Writer.WriteStartAttribute("Length", "");
			writer.Writer.WriteString(this.mFieldLength.ToString());
			writer.Writer.WriteEndAttribute();
		}

		internal override void WriteExtraElements(XmlHelper writer)
		{
			writer.WriteElement("AlignMode", this.AlignMode.ToString(), "Left");
			writer.WriteElement("AlignChar", this.AlignChar.ToString(), " ");
		}

		internal override void ReadFieldInternal(XmlNode node)
		{
			XmlNode xmlNode = node["AlignChar"];
			if (xmlNode != null)
			{
				this.AlignChar = xmlNode.InnerText[0];
			}
			xmlNode = node["AlignMode"];
			if (xmlNode != null)
			{
				this.AlignMode = (AlignMode)Enum.Parse(typeof(AlignMode), xmlNode.InnerText);
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int mFieldLength;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private AlignMode mAlignMode;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private char mAlignChar = ' ';
	}
}
