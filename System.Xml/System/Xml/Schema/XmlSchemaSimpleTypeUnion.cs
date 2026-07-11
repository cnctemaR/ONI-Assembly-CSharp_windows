using System;
using System.Collections;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaSimpleTypeUnion : XmlSchemaSimpleTypeContent
	{
		public XmlSchemaSimpleTypeUnion()
		{
			this.baseTypes = new XmlSchemaObjectCollection();
		}

		[XmlElement("simpleType", typeof(XmlSchemaSimpleType))]
		public XmlSchemaObjectCollection BaseTypes
		{
			get
			{
				return this.baseTypes;
			}
		}

		[XmlAttribute("memberTypes")]
		public XmlQualifiedName[] MemberTypes
		{
			get
			{
				return this.memberTypes;
			}
			set
			{
				this.memberTypes = value;
			}
		}

		[XmlIgnore]
		public XmlSchemaSimpleType[] BaseMemberTypes
		{
			get
			{
				return this.validatedSchemaTypes;
			}
		}

		internal object[] ValidatedTypes
		{
			get
			{
				return this.validatedTypes;
			}
		}

		internal override void SetParent(XmlSchemaObject parent)
		{
			base.SetParent(parent);
			foreach (XmlSchemaObject xmlSchemaObject in this.BaseTypes)
			{
				xmlSchemaObject.SetParent(this);
			}
		}

		internal override int Compile(ValidationEventHandler h, XmlSchema schema)
		{
			if (this.CompilationId == schema.CompilationId)
			{
				return 0;
			}
			this.errorCount = 0;
			int num = this.BaseTypes.Count;
			foreach (XmlSchemaObject xmlSchemaObject in this.baseTypes)
			{
				if (xmlSchemaObject != null && xmlSchemaObject is XmlSchemaSimpleType)
				{
					XmlSchemaSimpleType xmlSchemaSimpleType = (XmlSchemaSimpleType)xmlSchemaObject;
					this.errorCount += xmlSchemaSimpleType.Compile(h, schema);
				}
				else
				{
					base.error(h, "baseTypes can't have objects other than a simpletype");
				}
			}
			if (this.memberTypes != null)
			{
				for (int i = 0; i < this.memberTypes.Length; i++)
				{
					if (this.memberTypes[i] == null || !XmlSchemaUtil.CheckQName(this.MemberTypes[i]))
					{
						base.error(h, "Invalid membertype");
						this.memberTypes[i] = XmlQualifiedName.Empty;
					}
					else
					{
						num += this.MemberTypes.Length;
					}
				}
			}
			if (num == 0)
			{
				base.error(h, "Atleast one simpletype or membertype must be present");
			}
			XmlSchemaUtil.CompileID(base.Id, this, schema.IDCollection, h);
			this.CompilationId = schema.CompilationId;
			return this.errorCount;
		}

		internal override int Validate(ValidationEventHandler h, XmlSchema schema)
		{
			if (base.IsValidated(schema.ValidationId))
			{
				return this.errorCount;
			}
			ArrayList arrayList = new ArrayList();
			if (this.MemberTypes != null)
			{
				foreach (XmlQualifiedName xmlQualifiedName in this.MemberTypes)
				{
					object obj = null;
					XmlSchemaType xmlSchemaType = schema.FindSchemaType(xmlQualifiedName) as XmlSchemaSimpleType;
					if (xmlSchemaType != null)
					{
						this.errorCount += xmlSchemaType.Validate(h, schema);
						obj = xmlSchemaType;
					}
					else if (xmlQualifiedName == XmlSchemaComplexType.AnyTypeName)
					{
						obj = XmlSchemaSimpleType.AnySimpleType;
					}
					else if (xmlQualifiedName.Namespace == "http://www.w3.org/2001/XMLSchema" || xmlQualifiedName.Namespace == "http://www.w3.org/2003/11/xpath-datatypes")
					{
						obj = XmlSchemaDatatype.FromName(xmlQualifiedName);
						if (obj == null)
						{
							base.error(h, "Invalid schema type name was specified: " + xmlQualifiedName);
						}
					}
					else if (!schema.IsNamespaceAbsent(xmlQualifiedName.Namespace))
					{
						base.error(h, "Referenced base schema type " + xmlQualifiedName + " was not found in the corresponding schema.");
					}
					arrayList.Add(obj);
				}
			}
			if (this.BaseTypes != null)
			{
				foreach (XmlSchemaObject xmlSchemaObject in this.BaseTypes)
				{
					XmlSchemaSimpleType xmlSchemaSimpleType = (XmlSchemaSimpleType)xmlSchemaObject;
					xmlSchemaSimpleType.Validate(h, schema);
					arrayList.Add(xmlSchemaSimpleType);
				}
			}
			this.validatedTypes = arrayList.ToArray();
			if (this.validatedTypes != null)
			{
				this.validatedSchemaTypes = new XmlSchemaSimpleType[this.validatedTypes.Length];
				for (int j = 0; j < this.validatedTypes.Length; j++)
				{
					object obj2 = this.validatedTypes[j];
					XmlSchemaSimpleType xmlSchemaSimpleType2 = obj2 as XmlSchemaSimpleType;
					if (xmlSchemaSimpleType2 == null && obj2 != null)
					{
						xmlSchemaSimpleType2 = XmlSchemaType.GetBuiltInSimpleType(((XmlSchemaDatatype)obj2).TypeCode);
					}
					this.validatedSchemaTypes[j] = xmlSchemaSimpleType2;
				}
			}
			this.ValidationId = schema.ValidationId;
			return this.errorCount;
		}

		internal static XmlSchemaSimpleTypeUnion Read(XmlSchemaReader reader, ValidationEventHandler h)
		{
			XmlSchemaSimpleTypeUnion xmlSchemaSimpleTypeUnion = new XmlSchemaSimpleTypeUnion();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "union")
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaSimpleTypeUnion.Read, name=" + reader.Name, null);
				reader.Skip();
				return null;
			}
			xmlSchemaSimpleTypeUnion.LineNumber = reader.LineNumber;
			xmlSchemaSimpleTypeUnion.LinePosition = reader.LinePosition;
			xmlSchemaSimpleTypeUnion.SourceUri = reader.BaseURI;
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "id")
				{
					xmlSchemaSimpleTypeUnion.Id = reader.Value;
				}
				else if (reader.Name == "memberTypes")
				{
					string[] array = XmlSchemaUtil.SplitList(reader.Value);
					xmlSchemaSimpleTypeUnion.memberTypes = new XmlQualifiedName[array.Length];
					for (int i = 0; i < array.Length; i++)
					{
						Exception ex;
						xmlSchemaSimpleTypeUnion.memberTypes[i] = XmlSchemaUtil.ToQName(reader, array[i], out ex);
						if (ex != null)
						{
							XmlSchemaObject.error(h, "'" + array[i] + "' is not a valid memberType", ex);
						}
					}
				}
				else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for union", null);
				}
				else
				{
					XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaSimpleTypeUnion);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				return xmlSchemaSimpleTypeUnion;
			}
			int num = 1;
			while (reader.ReadNextElement())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.LocalName != "union")
					{
						XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaSimpleTypeUnion.Read, name=" + reader.Name, null);
					}
					break;
				}
				if (num <= 1 && reader.LocalName == "annotation")
				{
					num = 2;
					XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
					if (xmlSchemaAnnotation != null)
					{
						xmlSchemaSimpleTypeUnion.Annotation = xmlSchemaAnnotation;
					}
				}
				else if (num <= 2 && reader.LocalName == "simpleType")
				{
					num = 2;
					XmlSchemaSimpleType xmlSchemaSimpleType = XmlSchemaSimpleType.Read(reader, h);
					if (xmlSchemaSimpleType != null)
					{
						xmlSchemaSimpleTypeUnion.baseTypes.Add(xmlSchemaSimpleType);
					}
				}
				else
				{
					reader.RaiseInvalidElementError();
				}
			}
			return xmlSchemaSimpleTypeUnion;
		}

		private const string xmlname = "union";

		private XmlSchemaObjectCollection baseTypes;

		private XmlQualifiedName[] memberTypes;

		private object[] validatedTypes;

		private XmlSchemaSimpleType[] validatedSchemaTypes;
	}
}
