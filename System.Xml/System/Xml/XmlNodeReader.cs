using System;
using System.Collections.Generic;
using System.Xml.Schema;
using Mono.Xml;

namespace System.Xml
{
	public class XmlNodeReader : XmlReader, IHasXmlParserContext, IXmlNamespaceResolver
	{
		public XmlNodeReader(XmlNode node)
		{
			this.source = new XmlNodeReaderImpl(node);
		}

		private XmlNodeReader(XmlNodeReaderImpl entityContainer, bool insideAttribute)
		{
			this.source = new XmlNodeReaderImpl(entityContainer);
			this.entityInsideAttribute = insideAttribute;
		}

		XmlParserContext IHasXmlParserContext.ParserContext
		{
			get
			{
				return ((IHasXmlParserContext)this.Current).ParserContext;
			}
		}

		IDictionary<string, string> IXmlNamespaceResolver.GetNamespacesInScope(XmlNamespaceScope scope)
		{
			return ((IXmlNamespaceResolver)this.Current).GetNamespacesInScope(scope);
		}

		string IXmlNamespaceResolver.LookupPrefix(string ns)
		{
			return ((IXmlNamespaceResolver)this.Current).LookupPrefix(ns);
		}

		private XmlReader Current
		{
			get
			{
				return (this.entity == null || this.entity.ReadState == ReadState.Initial) ? this.source : this.entity;
			}
		}

		public override int AttributeCount
		{
			get
			{
				return this.Current.AttributeCount;
			}
		}

		public override string BaseURI
		{
			get
			{
				return this.Current.BaseURI;
			}
		}

		public override bool CanReadBinaryContent
		{
			get
			{
				return true;
			}
		}

		public override bool CanResolveEntity
		{
			get
			{
				return true;
			}
		}

		public override int Depth
		{
			get
			{
				if (this.entity != null && this.entity.ReadState == ReadState.Interactive)
				{
					return this.source.Depth + this.entity.Depth + 1;
				}
				return this.source.Depth;
			}
		}

		public override bool EOF
		{
			get
			{
				return this.source.EOF;
			}
		}

		public override bool HasAttributes
		{
			get
			{
				return this.Current.HasAttributes;
			}
		}

		public override bool HasValue
		{
			get
			{
				return this.Current.HasValue;
			}
		}

		public override bool IsDefault
		{
			get
			{
				return this.Current.IsDefault;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				return this.Current.IsEmptyElement;
			}
		}

		public override string LocalName
		{
			get
			{
				return this.Current.LocalName;
			}
		}

		public override string Name
		{
			get
			{
				return this.Current.Name;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				return this.Current.NamespaceURI;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return this.Current.NameTable;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				if (this.entity != null)
				{
					return (this.entity.ReadState != ReadState.Initial) ? ((!this.entity.EOF) ? this.entity.NodeType : XmlNodeType.EndEntity) : this.source.NodeType;
				}
				return this.source.NodeType;
			}
		}

		public override string Prefix
		{
			get
			{
				return this.Current.Prefix;
			}
		}

		public override ReadState ReadState
		{
			get
			{
				return (this.entity == null) ? this.source.ReadState : ReadState.Interactive;
			}
		}

		public override IXmlSchemaInfo SchemaInfo
		{
			get
			{
				IXmlSchemaInfo xmlSchemaInfo;
				if (this.entity != null)
				{
					IXmlSchemaInfo schemaInfo = this.entity.SchemaInfo;
					xmlSchemaInfo = schemaInfo;
				}
				else
				{
					xmlSchemaInfo = this.source.SchemaInfo;
				}
				return xmlSchemaInfo;
			}
		}

		public override string Value
		{
			get
			{
				return this.Current.Value;
			}
		}

		public override string XmlLang
		{
			get
			{
				return this.Current.XmlLang;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				return this.Current.XmlSpace;
			}
		}

		public override void Close()
		{
			if (this.entity != null)
			{
				this.entity.Close();
			}
			this.source.Close();
		}

		public override string GetAttribute(int attributeIndex)
		{
			return this.Current.GetAttribute(attributeIndex);
		}

		public override string GetAttribute(string name)
		{
			return this.Current.GetAttribute(name);
		}

		public override string GetAttribute(string name, string namespaceURI)
		{
			return this.Current.GetAttribute(name, namespaceURI);
		}

		public override string LookupNamespace(string prefix)
		{
			return this.Current.LookupNamespace(prefix);
		}

		public override void MoveToAttribute(int i)
		{
			if (this.entity != null && this.entityInsideAttribute)
			{
				this.entity.Close();
				this.entity = null;
			}
			this.Current.MoveToAttribute(i);
			this.insideAttribute = true;
		}

		public override bool MoveToAttribute(string name)
		{
			if (this.entity != null && !this.entityInsideAttribute)
			{
				return this.entity.MoveToAttribute(name);
			}
			if (!this.source.MoveToAttribute(name))
			{
				return false;
			}
			if (this.entity != null && this.entityInsideAttribute)
			{
				this.entity.Close();
				this.entity = null;
			}
			this.insideAttribute = true;
			return true;
		}

		public override bool MoveToAttribute(string localName, string namespaceURI)
		{
			if (this.entity != null && !this.entityInsideAttribute)
			{
				return this.entity.MoveToAttribute(localName, namespaceURI);
			}
			if (!this.source.MoveToAttribute(localName, namespaceURI))
			{
				return false;
			}
			if (this.entity != null && this.entityInsideAttribute)
			{
				this.entity.Close();
				this.entity = null;
			}
			this.insideAttribute = true;
			return true;
		}

		public override bool MoveToElement()
		{
			if (this.entity != null && this.entityInsideAttribute)
			{
				this.entity = null;
			}
			if (!this.Current.MoveToElement())
			{
				return false;
			}
			this.insideAttribute = false;
			return true;
		}

		public override bool MoveToFirstAttribute()
		{
			if (this.entity != null && !this.entityInsideAttribute)
			{
				return this.entity.MoveToFirstAttribute();
			}
			if (!this.source.MoveToFirstAttribute())
			{
				return false;
			}
			if (this.entity != null && this.entityInsideAttribute)
			{
				this.entity.Close();
				this.entity = null;
			}
			this.insideAttribute = true;
			return true;
		}

		public override bool MoveToNextAttribute()
		{
			if (this.entity != null && !this.entityInsideAttribute)
			{
				return this.entity.MoveToNextAttribute();
			}
			if (!this.source.MoveToNextAttribute())
			{
				return false;
			}
			if (this.entity != null && this.entityInsideAttribute)
			{
				this.entity.Close();
				this.entity = null;
			}
			this.insideAttribute = true;
			return true;
		}

		public override bool Read()
		{
			this.insideAttribute = false;
			if (this.entity != null && (this.entityInsideAttribute || this.entity.EOF))
			{
				this.entity = null;
			}
			if (this.entity != null)
			{
				this.entity.Read();
				return true;
			}
			return this.source.Read();
		}

		public override bool ReadAttributeValue()
		{
			if (this.entity != null && this.entityInsideAttribute)
			{
				if (!this.entity.EOF)
				{
					this.entity.Read();
					return true;
				}
				this.entity = null;
			}
			return this.Current.ReadAttributeValue();
		}

		public override int ReadContentAsBase64(byte[] buffer, int offset, int length)
		{
			if (this.entity != null)
			{
				return this.entity.ReadContentAsBase64(buffer, offset, length);
			}
			return this.source.ReadContentAsBase64(buffer, offset, length);
		}

		public override int ReadContentAsBinHex(byte[] buffer, int offset, int length)
		{
			if (this.entity != null)
			{
				return this.entity.ReadContentAsBinHex(buffer, offset, length);
			}
			return this.source.ReadContentAsBinHex(buffer, offset, length);
		}

		public override int ReadElementContentAsBase64(byte[] buffer, int offset, int length)
		{
			if (this.entity != null)
			{
				return this.entity.ReadElementContentAsBase64(buffer, offset, length);
			}
			return this.source.ReadElementContentAsBase64(buffer, offset, length);
		}

		public override int ReadElementContentAsBinHex(byte[] buffer, int offset, int length)
		{
			if (this.entity != null)
			{
				return this.entity.ReadElementContentAsBinHex(buffer, offset, length);
			}
			return this.source.ReadElementContentAsBinHex(buffer, offset, length);
		}

		public override string ReadString()
		{
			return base.ReadString();
		}

		public override void ResolveEntity()
		{
			if (this.entity != null)
			{
				this.entity.ResolveEntity();
			}
			else
			{
				if (this.source.NodeType != XmlNodeType.EntityReference)
				{
					throw new InvalidOperationException("The current node is not an Entity Reference");
				}
				this.entity = new XmlNodeReader(this.source, this.insideAttribute);
			}
		}

		public override void Skip()
		{
			if (this.entity != null && this.entityInsideAttribute)
			{
				this.entity = null;
			}
			this.Current.Skip();
		}

		private XmlReader entity;

		private XmlNodeReaderImpl source;

		private bool entityInsideAttribute;

		private bool insideAttribute;
	}
}
