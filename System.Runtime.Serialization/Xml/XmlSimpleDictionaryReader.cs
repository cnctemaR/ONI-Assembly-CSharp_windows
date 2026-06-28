using System;
using System.Collections.Generic;

namespace System.Xml
{
	internal class XmlSimpleDictionaryReader : XmlDictionaryReader, IXmlLineInfo, IXmlNamespaceResolver
	{
		public XmlSimpleDictionaryReader(XmlReader reader)
			: this(reader, null)
		{
		}

		public XmlSimpleDictionaryReader(XmlReader reader, XmlDictionary dictionary)
			: this(reader, dictionary, null)
		{
		}

		public XmlSimpleDictionaryReader(XmlReader reader, XmlDictionary dictionary, OnXmlDictionaryReaderClose onClose)
		{
			this.reader = reader;
			this.onClose = onClose;
			this.as_line_info = reader as IXmlLineInfo;
			this.as_dict_reader = reader as XmlDictionaryReader;
			if (dictionary == null)
			{
				dictionary = new XmlDictionary();
			}
			this.dict = dictionary;
		}

		public int LineNumber
		{
			get
			{
				return (this.as_line_info == null) ? 0 : this.as_line_info.LineNumber;
			}
		}

		public int LinePosition
		{
			get
			{
				return (this.as_line_info == null) ? 0 : this.as_line_info.LinePosition;
			}
		}

		public bool HasLineInfo()
		{
			return this.as_line_info != null && this.as_line_info.HasLineInfo();
		}

		public override bool CanCanonicalize
		{
			get
			{
				return this.as_dict_reader != null && this.as_dict_reader.CanCanonicalize;
			}
		}

		public override void EndCanonicalization()
		{
			if (this.as_dict_reader != null)
			{
				this.as_dict_reader.EndCanonicalization();
				return;
			}
			throw new NotSupportedException();
		}

		public override bool TryGetLocalNameAsDictionaryString(out XmlDictionaryString localName)
		{
			localName = null;
			return false;
		}

		public override bool TryGetNamespaceUriAsDictionaryString(out XmlDictionaryString namespaceUri)
		{
			namespaceUri = null;
			return false;
		}

		public IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope)
		{
			IXmlNamespaceResolver xmlNamespaceResolver = this.reader as IXmlNamespaceResolver;
			return xmlNamespaceResolver.GetNamespacesInScope(scope);
		}

		public string LookupPrefix(string ns)
		{
			IXmlNamespaceResolver xmlNamespaceResolver = this.reader as IXmlNamespaceResolver;
			return xmlNamespaceResolver.LookupPrefix(this.NameTable.Get(ns));
		}

		public override int AttributeCount
		{
			get
			{
				return this.reader.AttributeCount;
			}
		}

		public override string BaseURI
		{
			get
			{
				return this.reader.BaseURI;
			}
		}

		public override int Depth
		{
			get
			{
				return this.reader.Depth;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return this.reader.NodeType;
			}
		}

		public override string Name
		{
			get
			{
				return this.reader.Name;
			}
		}

		public override string LocalName
		{
			get
			{
				return this.reader.LocalName;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				return this.reader.NamespaceURI;
			}
		}

		public override string Prefix
		{
			get
			{
				return this.reader.Prefix;
			}
		}

		public override bool HasValue
		{
			get
			{
				return this.reader.HasValue;
			}
		}

		public override string Value
		{
			get
			{
				return this.reader.Value;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				return this.reader.IsEmptyElement;
			}
		}

		public override bool IsDefault
		{
			get
			{
				return this.reader.IsDefault;
			}
		}

		public override char QuoteChar
		{
			get
			{
				return this.reader.QuoteChar;
			}
		}

		public override string XmlLang
		{
			get
			{
				return this.reader.XmlLang;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				return this.reader.XmlSpace;
			}
		}

		public override string this[int i]
		{
			get
			{
				return this.reader[i];
			}
		}

		public override string this[string name]
		{
			get
			{
				return this.reader[name];
			}
		}

		public override string this[string localName, string namespaceURI]
		{
			get
			{
				return this.reader[localName, namespaceURI];
			}
		}

		public override bool EOF
		{
			get
			{
				return this.reader.EOF;
			}
		}

		public override ReadState ReadState
		{
			get
			{
				return this.reader.ReadState;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return this.reader.NameTable;
			}
		}

		public override string GetAttribute(string name)
		{
			return this.reader.GetAttribute(name);
		}

		public override string GetAttribute(string localName, string namespaceURI)
		{
			return this.reader.GetAttribute(localName, namespaceURI);
		}

		public override string GetAttribute(int i)
		{
			return this.reader.GetAttribute(i);
		}

		public override bool MoveToAttribute(string name)
		{
			return this.reader.MoveToAttribute(name);
		}

		public override bool MoveToAttribute(string localName, string namespaceURI)
		{
			return this.reader.MoveToAttribute(localName, namespaceURI);
		}

		public override void MoveToAttribute(int i)
		{
			this.reader.MoveToAttribute(i);
		}

		public override bool MoveToFirstAttribute()
		{
			return this.reader.MoveToFirstAttribute();
		}

		public override bool MoveToNextAttribute()
		{
			return this.reader.MoveToNextAttribute();
		}

		public override bool MoveToElement()
		{
			return this.reader.MoveToElement();
		}

		public override void Close()
		{
			this.reader.Close();
			if (this.onClose != null)
			{
				this.onClose(this);
			}
		}

		public override bool Read()
		{
			if (!this.reader.Read())
			{
				return false;
			}
			this.dict.Add(this.reader.Prefix);
			this.dict.Add(this.reader.LocalName);
			this.dict.Add(this.reader.NamespaceURI);
			if (this.reader.MoveToFirstAttribute())
			{
				do
				{
					this.dict.Add(this.reader.Prefix);
					this.dict.Add(this.reader.LocalName);
					this.dict.Add(this.reader.NamespaceURI);
					this.dict.Add(this.reader.Value);
				}
				while (this.reader.MoveToNextAttribute());
				this.reader.MoveToElement();
			}
			return true;
		}

		public override string ReadString()
		{
			return this.reader.ReadString();
		}

		public override string ReadInnerXml()
		{
			return this.reader.ReadInnerXml();
		}

		public override string ReadOuterXml()
		{
			return this.reader.ReadOuterXml();
		}

		public override string LookupNamespace(string prefix)
		{
			return this.reader.LookupNamespace(prefix);
		}

		public override void ResolveEntity()
		{
			this.reader.ResolveEntity();
		}

		public override bool ReadAttributeValue()
		{
			return this.reader.ReadAttributeValue();
		}

		private XmlDictionary dict;

		private XmlReader reader;

		private XmlDictionaryReader as_dict_reader;

		private IXmlLineInfo as_line_info;

		private OnXmlDictionaryReaderClose onClose;
	}
}
