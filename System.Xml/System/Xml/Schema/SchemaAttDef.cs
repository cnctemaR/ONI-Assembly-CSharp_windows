using System;

namespace System.Xml.Schema
{
	internal sealed class SchemaAttDef : SchemaDeclBase, IDtdDefaultAttributeInfo, IDtdAttributeInfo
	{
		public SchemaAttDef(XmlQualifiedName name, string prefix)
			: base(name, prefix)
		{
		}

		public SchemaAttDef(XmlQualifiedName name)
			: base(name, null)
		{
		}

		private SchemaAttDef()
		{
		}

		string IDtdAttributeInfo.Prefix
		{
			get
			{
				return this.Prefix;
			}
		}

		string IDtdAttributeInfo.LocalName
		{
			get
			{
				return this.Name.Name;
			}
		}

		int IDtdAttributeInfo.LineNumber
		{
			get
			{
				return this.LineNumber;
			}
		}

		int IDtdAttributeInfo.LinePosition
		{
			get
			{
				return this.LinePosition;
			}
		}

		bool IDtdAttributeInfo.IsNonCDataType
		{
			get
			{
				return this.TokenizedType > XmlTokenizedType.CDATA;
			}
		}

		bool IDtdAttributeInfo.IsDeclaredInExternal
		{
			get
			{
				return this.IsDeclaredInExternal;
			}
		}

		bool IDtdAttributeInfo.IsXmlAttribute
		{
			get
			{
				return this.Reserved > SchemaAttDef.Reserve.None;
			}
		}

		string IDtdDefaultAttributeInfo.DefaultValueExpanded
		{
			get
			{
				return this.DefaultValueExpanded;
			}
		}

		object IDtdDefaultAttributeInfo.DefaultValueTyped
		{
			get
			{
				return this.DefaultValueTyped;
			}
		}

		int IDtdDefaultAttributeInfo.ValueLineNumber
		{
			get
			{
				return this.ValueLineNumber;
			}
		}

		int IDtdDefaultAttributeInfo.ValueLinePosition
		{
			get
			{
				return this.ValueLinePosition;
			}
		}

		internal int LinePosition
		{
			get
			{
				return this.linePos;
			}
			set
			{
				this.linePos = value;
			}
		}

		internal int LineNumber
		{
			get
			{
				return this.lineNum;
			}
			set
			{
				this.lineNum = value;
			}
		}

		internal int ValueLinePosition
		{
			get
			{
				return this.valueLinePos;
			}
			set
			{
				this.valueLinePos = value;
			}
		}

		internal int ValueLineNumber
		{
			get
			{
				return this.valueLineNum;
			}
			set
			{
				this.valueLineNum = value;
			}
		}

		internal string DefaultValueExpanded
		{
			get
			{
				if (this.defExpanded == null)
				{
					return string.Empty;
				}
				return this.defExpanded;
			}
			set
			{
				this.defExpanded = value;
			}
		}

		internal XmlTokenizedType TokenizedType
		{
			get
			{
				return base.Datatype.TokenizedType;
			}
			set
			{
				base.Datatype = XmlSchemaDatatype.FromXmlTokenizedType(value);
			}
		}

		internal SchemaAttDef.Reserve Reserved
		{
			get
			{
				return this.reserved;
			}
			set
			{
				this.reserved = value;
			}
		}

		internal bool DefaultValueChecked
		{
			get
			{
				return this.defaultValueChecked;
			}
		}

		internal bool HasEntityRef
		{
			get
			{
				return this.hasEntityRef;
			}
			set
			{
				this.hasEntityRef = value;
			}
		}

		internal XmlSchemaAttribute SchemaAttribute
		{
			get
			{
				return this.schemaAttribute;
			}
			set
			{
				this.schemaAttribute = value;
			}
		}

		internal void CheckXmlSpace(IValidationEventHandling validationEventHandling)
		{
			if (this.datatype.TokenizedType == XmlTokenizedType.ENUMERATION && this.values != null && this.values.Count <= 2)
			{
				string text = this.values[0].ToString();
				if (this.values.Count == 2)
				{
					string text2 = this.values[1].ToString();
					if ((text == "default" || text2 == "default") && (text == "preserve" || text2 == "preserve"))
					{
						return;
					}
				}
				else if (text == "default" || text == "preserve")
				{
					return;
				}
			}
			validationEventHandling.SendEvent(new XmlSchemaException("Invalid xml:space syntax.", string.Empty), XmlSeverityType.Error);
		}

		internal SchemaAttDef Clone()
		{
			return (SchemaAttDef)base.MemberwiseClone();
		}

		private string defExpanded;

		private int lineNum;

		private int linePos;

		private int valueLineNum;

		private int valueLinePos;

		private SchemaAttDef.Reserve reserved;

		private bool defaultValueChecked;

		private bool hasEntityRef;

		private XmlSchemaAttribute schemaAttribute;

		public static readonly SchemaAttDef Empty = new SchemaAttDef();

		internal enum Reserve
		{
			None,
			XmlSpace,
			XmlLang
		}
	}
}
