using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Xml.Schema;

namespace System.Xml
{
	[Obsolete("Use XmlReader created by XmlReader.Create() method using appropriate XmlReaderSettings instead. http://go.microsoft.com/fwlink/?linkid=14202")]
	[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	public class XmlValidatingReader : XmlReader, IXmlLineInfo, IXmlNamespaceResolver
	{
		public XmlValidatingReader(XmlReader reader)
		{
			this.impl = new XmlValidatingReaderImpl(reader);
			this.impl.OuterReader = this;
		}

		public XmlValidatingReader(string xmlFragment, XmlNodeType fragType, XmlParserContext context)
		{
			if (xmlFragment == null)
			{
				throw new ArgumentNullException("xmlFragment");
			}
			this.impl = new XmlValidatingReaderImpl(xmlFragment, fragType, context);
			this.impl.OuterReader = this;
		}

		public XmlValidatingReader(Stream xmlFragment, XmlNodeType fragType, XmlParserContext context)
		{
			if (xmlFragment == null)
			{
				throw new ArgumentNullException("xmlFragment");
			}
			this.impl = new XmlValidatingReaderImpl(xmlFragment, fragType, context);
			this.impl.OuterReader = this;
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return this.impl.NodeType;
			}
		}

		public override string Name
		{
			get
			{
				return this.impl.Name;
			}
		}

		public override string LocalName
		{
			get
			{
				return this.impl.LocalName;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				return this.impl.NamespaceURI;
			}
		}

		public override string Prefix
		{
			get
			{
				return this.impl.Prefix;
			}
		}

		public override bool HasValue
		{
			get
			{
				return this.impl.HasValue;
			}
		}

		public override string Value
		{
			get
			{
				return this.impl.Value;
			}
		}

		public override int Depth
		{
			get
			{
				return this.impl.Depth;
			}
		}

		public override string BaseURI
		{
			get
			{
				return this.impl.BaseURI;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				return this.impl.IsEmptyElement;
			}
		}

		public override bool IsDefault
		{
			get
			{
				return this.impl.IsDefault;
			}
		}

		public override char QuoteChar
		{
			get
			{
				return this.impl.QuoteChar;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				return this.impl.XmlSpace;
			}
		}

		public override string XmlLang
		{
			get
			{
				return this.impl.XmlLang;
			}
		}

		public override int AttributeCount
		{
			get
			{
				return this.impl.AttributeCount;
			}
		}

		public override string GetAttribute(string name)
		{
			return this.impl.GetAttribute(name);
		}

		public override string GetAttribute(string localName, string namespaceURI)
		{
			return this.impl.GetAttribute(localName, namespaceURI);
		}

		public override string GetAttribute(int i)
		{
			return this.impl.GetAttribute(i);
		}

		public override bool MoveToAttribute(string name)
		{
			return this.impl.MoveToAttribute(name);
		}

		public override bool MoveToAttribute(string localName, string namespaceURI)
		{
			return this.impl.MoveToAttribute(localName, namespaceURI);
		}

		public override void MoveToAttribute(int i)
		{
			this.impl.MoveToAttribute(i);
		}

		public override bool MoveToFirstAttribute()
		{
			return this.impl.MoveToFirstAttribute();
		}

		public override bool MoveToNextAttribute()
		{
			return this.impl.MoveToNextAttribute();
		}

		public override bool MoveToElement()
		{
			return this.impl.MoveToElement();
		}

		public override bool ReadAttributeValue()
		{
			return this.impl.ReadAttributeValue();
		}

		public override bool Read()
		{
			return this.impl.Read();
		}

		public override bool EOF
		{
			get
			{
				return this.impl.EOF;
			}
		}

		public override void Close()
		{
			this.impl.Close();
		}

		public override ReadState ReadState
		{
			get
			{
				return this.impl.ReadState;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return this.impl.NameTable;
			}
		}

		public override string LookupNamespace(string prefix)
		{
			string text = this.impl.LookupNamespace(prefix);
			if (text != null && text.Length == 0)
			{
				text = null;
			}
			return text;
		}

		public override bool CanResolveEntity
		{
			get
			{
				return true;
			}
		}

		public override void ResolveEntity()
		{
			this.impl.ResolveEntity();
		}

		public override bool CanReadBinaryContent
		{
			get
			{
				return true;
			}
		}

		public override int ReadContentAsBase64(byte[] buffer, int index, int count)
		{
			return this.impl.ReadContentAsBase64(buffer, index, count);
		}

		public override int ReadElementContentAsBase64(byte[] buffer, int index, int count)
		{
			return this.impl.ReadElementContentAsBase64(buffer, index, count);
		}

		public override int ReadContentAsBinHex(byte[] buffer, int index, int count)
		{
			return this.impl.ReadContentAsBinHex(buffer, index, count);
		}

		public override int ReadElementContentAsBinHex(byte[] buffer, int index, int count)
		{
			return this.impl.ReadElementContentAsBinHex(buffer, index, count);
		}

		public override string ReadString()
		{
			this.impl.MoveOffEntityReference();
			return base.ReadString();
		}

		public bool HasLineInfo()
		{
			return true;
		}

		public int LineNumber
		{
			get
			{
				return this.impl.LineNumber;
			}
		}

		public int LinePosition
		{
			get
			{
				return this.impl.LinePosition;
			}
		}

		IDictionary<string, string> IXmlNamespaceResolver.GetNamespacesInScope(XmlNamespaceScope scope)
		{
			return this.impl.GetNamespacesInScope(scope);
		}

		string IXmlNamespaceResolver.LookupNamespace(string prefix)
		{
			return this.impl.LookupNamespace(prefix);
		}

		string IXmlNamespaceResolver.LookupPrefix(string namespaceName)
		{
			return this.impl.LookupPrefix(namespaceName);
		}

		public event ValidationEventHandler ValidationEventHandler
		{
			add
			{
				this.impl.ValidationEventHandler += value;
			}
			remove
			{
				this.impl.ValidationEventHandler -= value;
			}
		}

		public object SchemaType
		{
			get
			{
				return this.impl.SchemaType;
			}
		}

		public XmlReader Reader
		{
			get
			{
				return this.impl.Reader;
			}
		}

		public ValidationType ValidationType
		{
			get
			{
				return this.impl.ValidationType;
			}
			set
			{
				this.impl.ValidationType = value;
			}
		}

		public XmlSchemaCollection Schemas
		{
			get
			{
				return this.impl.Schemas;
			}
		}

		public EntityHandling EntityHandling
		{
			get
			{
				return this.impl.EntityHandling;
			}
			set
			{
				this.impl.EntityHandling = value;
			}
		}

		public XmlResolver XmlResolver
		{
			set
			{
				this.impl.XmlResolver = value;
			}
		}

		public bool Namespaces
		{
			get
			{
				return this.impl.Namespaces;
			}
			set
			{
				this.impl.Namespaces = value;
			}
		}

		public object ReadTypedValue()
		{
			return this.impl.ReadTypedValue();
		}

		public Encoding Encoding
		{
			get
			{
				return this.impl.Encoding;
			}
		}

		internal XmlValidatingReaderImpl Impl
		{
			get
			{
				return this.impl;
			}
		}

		internal override IDtdInfo DtdInfo
		{
			get
			{
				return this.impl.DtdInfo;
			}
		}

		private XmlValidatingReaderImpl impl;
	}
}
