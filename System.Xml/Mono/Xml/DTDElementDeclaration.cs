using System;
using System.Xml.Schema;

namespace Mono.Xml
{
	internal class DTDElementDeclaration : DTDNode
	{
		internal DTDElementDeclaration(DTDObjectModel root)
		{
			this.root = root;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.isEmpty;
			}
			set
			{
				this.isEmpty = value;
			}
		}

		public bool IsAny
		{
			get
			{
				return this.isAny;
			}
			set
			{
				this.isAny = value;
			}
		}

		public bool IsMixedContent
		{
			get
			{
				return this.isMixedContent;
			}
			set
			{
				this.isMixedContent = value;
			}
		}

		public DTDContentModel ContentModel
		{
			get
			{
				if (this.contentModel == null)
				{
					this.contentModel = new DTDContentModel(this.root, this.Name);
				}
				return this.contentModel;
			}
		}

		public DTDAttListDeclaration Attributes
		{
			get
			{
				return base.Root.AttListDecls[this.Name];
			}
		}

		internal XmlSchemaElement CreateXsdElement()
		{
			XmlSchemaElement xmlSchemaElement = new XmlSchemaElement();
			base.SetLineInfo(xmlSchemaElement);
			xmlSchemaElement.Name = this.Name;
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			xmlSchemaElement.SchemaType = xmlSchemaComplexType;
			if (this.Attributes != null)
			{
				base.SetLineInfo(xmlSchemaComplexType);
				foreach (object obj in this.Attributes.Definitions)
				{
					DTDAttributeDefinition dtdattributeDefinition = (DTDAttributeDefinition)obj;
					xmlSchemaComplexType.Attributes.Add(dtdattributeDefinition.CreateXsdAttribute());
				}
			}
			if (!this.IsEmpty)
			{
				if (this.IsAny)
				{
					xmlSchemaComplexType.Particle = new XmlSchemaAny
					{
						MinOccurs = 0m,
						MaxOccursString = "unbounded"
					};
				}
				else
				{
					if (this.IsMixedContent)
					{
						xmlSchemaComplexType.IsMixed = true;
					}
					xmlSchemaComplexType.Particle = this.ContentModel.CreateXsdParticle();
				}
			}
			return xmlSchemaElement;
		}

		private DTDObjectModel root;

		private DTDContentModel contentModel;

		private string name;

		private bool isEmpty;

		private bool isAny;

		private bool isMixedContent;
	}
}
