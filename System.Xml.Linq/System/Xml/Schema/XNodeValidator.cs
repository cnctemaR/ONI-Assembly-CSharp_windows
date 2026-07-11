using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

namespace System.Xml.Schema
{
	internal class XNodeValidator
	{
		public XNodeValidator(XmlSchemaSet schemas, ValidationEventHandler validationEventHandler)
		{
			this.schemas = schemas;
			this.validationEventHandler = validationEventHandler;
			XNamespace xnamespace = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
			this.xsiTypeName = xnamespace.GetName("type");
			this.xsiNilName = xnamespace.GetName("nil");
		}

		public void Validate(XObject source, XmlSchemaObject partialValidationType, bool addSchemaInfo)
		{
			this.source = source;
			this.addSchemaInfo = addSchemaInfo;
			XmlSchemaValidationFlags xmlSchemaValidationFlags = XmlSchemaValidationFlags.AllowXmlAttributes;
			XmlNodeType nodeType = source.NodeType;
			if (nodeType != XmlNodeType.Element)
			{
				if (nodeType != XmlNodeType.Attribute)
				{
					if (nodeType == XmlNodeType.Document)
					{
						source = ((XDocument)source).Root;
						if (source == null)
						{
							throw new InvalidOperationException(Res.GetString("InvalidOperation_MissingRoot"));
						}
						xmlSchemaValidationFlags |= XmlSchemaValidationFlags.ProcessIdentityConstraints;
						goto IL_008F;
					}
				}
				else if (!((XAttribute)source).IsNamespaceDeclaration)
				{
					if (source.Parent == null)
					{
						throw new InvalidOperationException(Res.GetString("InvalidOperation_MissingParent"));
					}
					goto IL_008F;
				}
				throw new InvalidOperationException(Res.GetString("InvalidOperation_BadNodeType", new object[] { nodeType }));
			}
			IL_008F:
			this.namespaceManager = new XmlNamespaceManager(this.schemas.NameTable);
			this.PushAncestorsAndSelf(source.Parent);
			this.validator = new XmlSchemaValidator(this.schemas.NameTable, this.schemas, this.namespaceManager, xmlSchemaValidationFlags);
			this.validator.ValidationEventHandler += this.ValidationCallback;
			this.validator.XmlResolver = null;
			if (partialValidationType != null)
			{
				this.validator.Initialize(partialValidationType);
			}
			else
			{
				this.validator.Initialize();
			}
			IXmlLineInfo xmlLineInfo = this.SaveLineInfo(source);
			if (nodeType == XmlNodeType.Attribute)
			{
				this.ValidateAttribute((XAttribute)source);
			}
			else
			{
				this.ValidateElement((XElement)source);
			}
			this.validator.EndValidation();
			this.RestoreLineInfo(xmlLineInfo);
		}

		private XmlSchemaInfo GetDefaultAttributeSchemaInfo(XmlSchemaAttribute sa)
		{
			XmlSchemaInfo xmlSchemaInfo = new XmlSchemaInfo();
			xmlSchemaInfo.IsDefault = true;
			xmlSchemaInfo.IsNil = false;
			xmlSchemaInfo.SchemaAttribute = sa;
			XmlSchemaSimpleType attributeSchemaType = sa.AttributeSchemaType;
			xmlSchemaInfo.SchemaType = attributeSchemaType;
			if (attributeSchemaType.Datatype.Variety == XmlSchemaDatatypeVariety.Union)
			{
				string defaultValue = this.GetDefaultValue(sa);
				foreach (XmlSchemaSimpleType xmlSchemaSimpleType in ((XmlSchemaSimpleTypeUnion)attributeSchemaType.Content).BaseMemberTypes)
				{
					object obj = null;
					try
					{
						obj = xmlSchemaSimpleType.Datatype.ParseValue(defaultValue, this.schemas.NameTable, this.namespaceManager);
					}
					catch (XmlSchemaException)
					{
					}
					if (obj != null)
					{
						xmlSchemaInfo.MemberType = xmlSchemaSimpleType;
						break;
					}
				}
			}
			xmlSchemaInfo.Validity = XmlSchemaValidity.Valid;
			return xmlSchemaInfo;
		}

		private string GetDefaultValue(XmlSchemaAttribute sa)
		{
			XmlQualifiedName refName = sa.RefName;
			if (!refName.IsEmpty)
			{
				sa = this.schemas.GlobalAttributes[refName] as XmlSchemaAttribute;
				if (sa == null)
				{
					return null;
				}
			}
			string fixedValue = sa.FixedValue;
			if (fixedValue != null)
			{
				return fixedValue;
			}
			return sa.DefaultValue;
		}

		private string GetDefaultValue(XmlSchemaElement se)
		{
			XmlQualifiedName refName = se.RefName;
			if (!refName.IsEmpty)
			{
				se = this.schemas.GlobalElements[refName] as XmlSchemaElement;
				if (se == null)
				{
					return null;
				}
			}
			string fixedValue = se.FixedValue;
			if (fixedValue != null)
			{
				return fixedValue;
			}
			return se.DefaultValue;
		}

		private void ReplaceSchemaInfo(XObject o, XmlSchemaInfo schemaInfo)
		{
			if (this.schemaInfos == null)
			{
				this.schemaInfos = new Dictionary<XmlSchemaInfo, XmlSchemaInfo>(new XmlSchemaInfoEqualityComparer());
			}
			XmlSchemaInfo xmlSchemaInfo = o.Annotation<XmlSchemaInfo>();
			if (xmlSchemaInfo != null)
			{
				if (!this.schemaInfos.ContainsKey(xmlSchemaInfo))
				{
					this.schemaInfos.Add(xmlSchemaInfo, xmlSchemaInfo);
				}
				o.RemoveAnnotations<XmlSchemaInfo>();
			}
			if (!this.schemaInfos.TryGetValue(schemaInfo, out xmlSchemaInfo))
			{
				xmlSchemaInfo = schemaInfo;
				this.schemaInfos.Add(xmlSchemaInfo, xmlSchemaInfo);
			}
			o.AddAnnotation(xmlSchemaInfo);
		}

		private void PushAncestorsAndSelf(XElement e)
		{
			while (e != null)
			{
				XAttribute xattribute = e.lastAttr;
				if (xattribute != null)
				{
					do
					{
						xattribute = xattribute.next;
						if (xattribute.IsNamespaceDeclaration)
						{
							string text = xattribute.Name.LocalName;
							if (text == "xmlns")
							{
								text = string.Empty;
							}
							if (!this.namespaceManager.HasNamespace(text))
							{
								this.namespaceManager.AddNamespace(text, xattribute.Value);
							}
						}
					}
					while (xattribute != e.lastAttr);
				}
				e = e.parent as XElement;
			}
		}

		private void PushElement(XElement e, ref string xsiType, ref string xsiNil)
		{
			this.namespaceManager.PushScope();
			XAttribute xattribute = e.lastAttr;
			if (xattribute != null)
			{
				do
				{
					xattribute = xattribute.next;
					if (xattribute.IsNamespaceDeclaration)
					{
						string text = xattribute.Name.LocalName;
						if (text == "xmlns")
						{
							text = string.Empty;
						}
						this.namespaceManager.AddNamespace(text, xattribute.Value);
					}
					else
					{
						XName name = xattribute.Name;
						if (name == this.xsiTypeName)
						{
							xsiType = xattribute.Value;
						}
						else if (name == this.xsiNilName)
						{
							xsiNil = xattribute.Value;
						}
					}
				}
				while (xattribute != e.lastAttr);
			}
		}

		private IXmlLineInfo SaveLineInfo(XObject source)
		{
			IXmlLineInfo lineInfoProvider = this.validator.LineInfoProvider;
			this.validator.LineInfoProvider = source;
			return lineInfoProvider;
		}

		private void RestoreLineInfo(IXmlLineInfo originalLineInfo)
		{
			this.validator.LineInfoProvider = originalLineInfo;
		}

		private void ValidateAttribute(XAttribute a)
		{
			IXmlLineInfo xmlLineInfo = this.SaveLineInfo(a);
			XmlSchemaInfo xmlSchemaInfo = (this.addSchemaInfo ? new XmlSchemaInfo() : null);
			this.source = a;
			this.validator.ValidateAttribute(a.Name.LocalName, a.Name.NamespaceName, a.Value, xmlSchemaInfo);
			if (this.addSchemaInfo)
			{
				this.ReplaceSchemaInfo(a, xmlSchemaInfo);
			}
			this.RestoreLineInfo(xmlLineInfo);
		}

		private void ValidateAttributes(XElement e)
		{
			XAttribute xattribute = e.lastAttr;
			IXmlLineInfo xmlLineInfo = this.SaveLineInfo(xattribute);
			if (xattribute != null)
			{
				do
				{
					xattribute = xattribute.next;
					if (!xattribute.IsNamespaceDeclaration)
					{
						this.ValidateAttribute(xattribute);
					}
				}
				while (xattribute != e.lastAttr);
				this.source = e;
			}
			if (this.addSchemaInfo)
			{
				if (this.defaultAttributes == null)
				{
					this.defaultAttributes = new ArrayList();
				}
				else
				{
					this.defaultAttributes.Clear();
				}
				this.validator.GetUnspecifiedDefaultAttributes(this.defaultAttributes);
				foreach (object obj in this.defaultAttributes)
				{
					XmlSchemaAttribute xmlSchemaAttribute = (XmlSchemaAttribute)obj;
					xattribute = new XAttribute(XNamespace.Get(xmlSchemaAttribute.QualifiedName.Namespace).GetName(xmlSchemaAttribute.QualifiedName.Name), this.GetDefaultValue(xmlSchemaAttribute));
					this.ReplaceSchemaInfo(xattribute, this.GetDefaultAttributeSchemaInfo(xmlSchemaAttribute));
					e.Add(xattribute);
				}
			}
			this.RestoreLineInfo(xmlLineInfo);
		}

		private void ValidateElement(XElement e)
		{
			XmlSchemaInfo xmlSchemaInfo = (this.addSchemaInfo ? new XmlSchemaInfo() : null);
			string text = null;
			string text2 = null;
			this.PushElement(e, ref text, ref text2);
			IXmlLineInfo xmlLineInfo = this.SaveLineInfo(e);
			this.source = e;
			this.validator.ValidateElement(e.Name.LocalName, e.Name.NamespaceName, xmlSchemaInfo, text, text2, null, null);
			this.ValidateAttributes(e);
			this.validator.ValidateEndOfAttributes(xmlSchemaInfo);
			this.ValidateNodes(e);
			this.validator.ValidateEndElement(xmlSchemaInfo);
			if (this.addSchemaInfo)
			{
				if (xmlSchemaInfo.Validity == XmlSchemaValidity.Valid && xmlSchemaInfo.IsDefault)
				{
					e.Value = this.GetDefaultValue(xmlSchemaInfo.SchemaElement);
				}
				this.ReplaceSchemaInfo(e, xmlSchemaInfo);
			}
			this.RestoreLineInfo(xmlLineInfo);
			this.namespaceManager.PopScope();
		}

		private void ValidateNodes(XElement e)
		{
			XNode xnode = e.content as XNode;
			IXmlLineInfo xmlLineInfo = this.SaveLineInfo(xnode);
			if (xnode != null)
			{
				do
				{
					xnode = xnode.next;
					XElement xelement = xnode as XElement;
					if (xelement != null)
					{
						this.ValidateElement(xelement);
					}
					else
					{
						XText xtext = xnode as XText;
						if (xtext != null)
						{
							string value = xtext.Value;
							if (value.Length > 0)
							{
								this.validator.LineInfoProvider = xtext;
								this.validator.ValidateText(value);
							}
						}
					}
				}
				while (xnode != e.content);
				this.source = e;
			}
			else
			{
				string text = e.content as string;
				if (text != null && text.Length > 0)
				{
					this.validator.ValidateText(text);
				}
			}
			this.RestoreLineInfo(xmlLineInfo);
		}

		private void ValidationCallback(object sender, ValidationEventArgs e)
		{
			if (this.validationEventHandler != null)
			{
				this.validationEventHandler(this.source, e);
				return;
			}
			if (e.Severity == XmlSeverityType.Error)
			{
				throw e.Exception;
			}
		}

		private XmlSchemaSet schemas;

		private ValidationEventHandler validationEventHandler;

		private XObject source;

		private bool addSchemaInfo;

		private XmlNamespaceManager namespaceManager;

		private XmlSchemaValidator validator;

		private Dictionary<XmlSchemaInfo, XmlSchemaInfo> schemaInfos;

		private ArrayList defaultAttributes;

		private XName xsiTypeName;

		private XName xsiNilName;
	}
}
