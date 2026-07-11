using System;

namespace System.Xml.Schema
{
	internal class AutoValidator : BaseValidator
	{
		public AutoValidator(XmlValidatingReaderImpl reader, XmlSchemaCollection schemaCollection, IValidationEventHandling eventHandling)
			: base(reader, schemaCollection, eventHandling)
		{
			this.schemaInfo = new SchemaInfo();
		}

		public override bool PreserveWhitespace
		{
			get
			{
				return false;
			}
		}

		public override void Validate()
		{
			switch (this.DetectValidationType())
			{
			case ValidationType.Auto:
			case ValidationType.DTD:
				break;
			case ValidationType.XDR:
				this.reader.Validator = new XdrValidator(this);
				this.reader.Validator.Validate();
				return;
			case ValidationType.Schema:
				this.reader.Validator = new XsdValidator(this);
				this.reader.Validator.Validate();
				break;
			default:
				return;
			}
		}

		public override void CompleteValidation()
		{
		}

		public override object FindId(string name)
		{
			return null;
		}

		private ValidationType DetectValidationType()
		{
			if (this.reader.Schemas != null && this.reader.Schemas.Count > 0)
			{
				XmlSchemaCollectionEnumerator enumerator = this.reader.Schemas.GetEnumerator();
				while (enumerator.MoveNext())
				{
					SchemaInfo schemaInfo = enumerator.CurrentNode.SchemaInfo;
					if (schemaInfo.SchemaType == SchemaType.XSD)
					{
						return ValidationType.Schema;
					}
					if (schemaInfo.SchemaType == SchemaType.XDR)
					{
						return ValidationType.XDR;
					}
				}
			}
			if (this.reader.NodeType == XmlNodeType.Element)
			{
				SchemaType schemaType = base.SchemaNames.SchemaTypeFromRoot(this.reader.LocalName, this.reader.NamespaceURI);
				if (schemaType == SchemaType.XSD)
				{
					return ValidationType.Schema;
				}
				if (schemaType == SchemaType.XDR)
				{
					return ValidationType.XDR;
				}
				int attributeCount = this.reader.AttributeCount;
				for (int i = 0; i < attributeCount; i++)
				{
					this.reader.MoveToAttribute(i);
					string namespaceURI = this.reader.NamespaceURI;
					string localName = this.reader.LocalName;
					if (Ref.Equal(namespaceURI, base.SchemaNames.NsXmlNs))
					{
						if (XdrBuilder.IsXdrSchema(this.reader.Value))
						{
							this.reader.MoveToElement();
							return ValidationType.XDR;
						}
					}
					else
					{
						if (Ref.Equal(namespaceURI, base.SchemaNames.NsXsi))
						{
							this.reader.MoveToElement();
							return ValidationType.Schema;
						}
						if (Ref.Equal(namespaceURI, base.SchemaNames.QnDtDt.Namespace) && Ref.Equal(localName, base.SchemaNames.QnDtDt.Name))
						{
							this.reader.SchemaTypeObject = XmlSchemaDatatype.FromXdrName(this.reader.Value);
							this.reader.MoveToElement();
							return ValidationType.XDR;
						}
					}
				}
				if (attributeCount > 0)
				{
					this.reader.MoveToElement();
				}
			}
			return ValidationType.Auto;
		}

		private const string x_schema = "x-schema";
	}
}
