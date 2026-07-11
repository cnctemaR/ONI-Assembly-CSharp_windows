using System;
using System.Collections.Generic;

namespace System.Xml.Linq
{
	internal class XNodeReader : XmlReader
	{
		public XNodeReader(XNode node)
		{
			this.node = node;
			this.start = node;
		}

		public override int AttributeCount
		{
			get
			{
				if (this.state != ReadState.Interactive || this.end_element)
				{
					return 0;
				}
				int num = 0;
				XmlNodeType nodeType = this.node.NodeType;
				if (nodeType == XmlNodeType.Document)
				{
					XDeclaration declaration = ((XDocument)this.node).Declaration;
					return ((declaration.Version == null) ? 0 : 1) + ((declaration.Encoding == null) ? 0 : 1) + ((declaration.Standalone == null) ? 0 : 1);
				}
				if (nodeType == XmlNodeType.DocumentType)
				{
					XDocumentType xdocumentType = (XDocumentType)this.node;
					return ((xdocumentType.PublicId == null) ? 0 : 1) + ((xdocumentType.SystemId == null) ? 0 : 1) + ((xdocumentType.InternalSubset == null) ? 0 : 1);
				}
				if (nodeType != XmlNodeType.Element)
				{
					return 0;
				}
				XElement xelement = (XElement)this.node;
				for (XAttribute xattribute = xelement.FirstAttribute; xattribute != null; xattribute = xattribute.NextAttribute)
				{
					num++;
				}
				return num;
			}
		}

		public override string BaseURI
		{
			get
			{
				return this.node.BaseUri ?? string.Empty;
			}
		}

		public override int Depth
		{
			get
			{
				if (this.EOF)
				{
					return 0;
				}
				int num = 0;
				for (XNode xnode = this.node.Parent; xnode != null; xnode = xnode.Parent)
				{
					num++;
				}
				if (this.attr >= 0)
				{
					num++;
				}
				if (this.attr_value)
				{
					num++;
				}
				return num;
			}
		}

		public override bool EOF
		{
			get
			{
				return this.state == ReadState.EndOfFile || this.state == ReadState.Error;
			}
		}

		public override bool HasAttributes
		{
			get
			{
				if (this.EOF || this.end_element || this.node == null)
				{
					return false;
				}
				if (this.node is XElement)
				{
					return ((XElement)this.node).HasAttributes;
				}
				return this.AttributeCount > 0;
			}
		}

		public override bool HasValue
		{
			get
			{
				if (this.EOF)
				{
					return false;
				}
				if (this.attr >= 0)
				{
					return true;
				}
				XmlNodeType nodeType = this.node.NodeType;
				return nodeType != XmlNodeType.Element && nodeType != XmlNodeType.Document && nodeType != XmlNodeType.EndElement;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				return !this.EOF && this.attr < 0 && this.node is XElement && ((XElement)this.node).IsEmpty;
			}
		}

		private XAttribute GetCurrentAttribute()
		{
			return this.GetXAttribute(this.attr);
		}

		private XAttribute GetXAttribute(int idx)
		{
			if (this.EOF)
			{
				return null;
			}
			XElement xelement = this.node as XElement;
			if (xelement == null)
			{
				return null;
			}
			int num = 0;
			foreach (XAttribute xattribute in xelement.Attributes())
			{
				if (num++ == idx)
				{
					return xattribute;
				}
			}
			return null;
		}

		private object GetCurrentName()
		{
			if (this.EOF || this.attr_value)
			{
				return null;
			}
			return this.GetName(this.attr);
		}

		private object GetName(int attr)
		{
			if (attr >= 0)
			{
				XmlNodeType xmlNodeType = this.node.NodeType;
				if (xmlNodeType != XmlNodeType.Document)
				{
					if (xmlNodeType != XmlNodeType.DocumentType)
					{
						if (xmlNodeType == XmlNodeType.Element)
						{
							XAttribute xattribute = this.GetXAttribute(attr);
							return xattribute.Name;
						}
					}
					else
					{
						if (attr == 0)
						{
							return (((XDocumentType)this.node).PublicId == null) ? "SYSTEM" : "PUBLIC";
						}
						return "SYSTEM";
					}
				}
				else
				{
					XDeclaration declaration = ((XDocument)this.node).Declaration;
					if (attr == 0)
					{
						return (declaration.Version == null) ? ((declaration.Encoding == null) ? "standalone" : "encoding") : "version";
					}
					if (attr != 1)
					{
						return "standalone";
					}
					return (declaration.Version == null) ? "standalone" : ((declaration.Encoding == null) ? "standalone" : "encoding");
				}
			}
			else
			{
				XmlNodeType xmlNodeType = this.node.NodeType;
				switch (xmlNodeType)
				{
				case XmlNodeType.ProcessingInstruction:
					return ((XProcessingInstruction)this.node).Target;
				default:
					if (xmlNodeType == XmlNodeType.Element)
					{
						return ((XElement)this.node).Name;
					}
					break;
				case XmlNodeType.Document:
					return "xml";
				case XmlNodeType.DocumentType:
					return ((XDocumentType)this.node).Name;
				}
			}
			return null;
		}

		public override string LocalName
		{
			get
			{
				object currentName = this.GetCurrentName();
				if (currentName == null)
				{
					return string.Empty;
				}
				if (currentName is string)
				{
					return (string)currentName;
				}
				return ((XName)currentName).LocalName;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				XName xname = this.GetCurrentName() as XName;
				if (xname != null)
				{
					return (!(xname.LocalName == "xmlns") || !(xname.Namespace == XNamespace.None)) ? xname.NamespaceName : XNamespace.Xmlns.NamespaceName;
				}
				return string.Empty;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return this.name_table;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return (this.state == ReadState.Interactive) ? ((!this.end_element) ? ((!this.attr_value) ? ((this.attr < 0) ? ((this.node.NodeType != XmlNodeType.Document) ? this.node.NodeType : XmlNodeType.XmlDeclaration) : XmlNodeType.Attribute) : XmlNodeType.Text) : XmlNodeType.EndElement) : XmlNodeType.None;
			}
		}

		public override string Prefix
		{
			get
			{
				XName xname = this.GetCurrentName() as XName;
				if (xname == null || xname.Namespace == XNamespace.None)
				{
					return string.Empty;
				}
				XElement xelement = (this.node as XElement) ?? this.node.Parent;
				if (xelement == null)
				{
					return string.Empty;
				}
				return xelement.GetPrefixOfNamespace(xname.Namespace) ?? string.Empty;
			}
		}

		public override ReadState ReadState
		{
			get
			{
				return this.state;
			}
		}

		public override string Value
		{
			get
			{
				if (this.ReadState != ReadState.Interactive)
				{
					return string.Empty;
				}
				XAttribute currentAttribute = this.GetCurrentAttribute();
				if (currentAttribute != null)
				{
					return currentAttribute.Value;
				}
				switch (this.node.NodeType)
				{
				case XmlNodeType.Text:
				case XmlNodeType.CDATA:
					return ((XText)this.node).Value;
				case XmlNodeType.ProcessingInstruction:
					return ((XProcessingInstruction)this.node).Data;
				case XmlNodeType.Comment:
					return ((XComment)this.node).Value;
				case XmlNodeType.Document:
				{
					XDeclaration declaration = ((XDocument)this.node).Declaration;
					if (this.attr >= 0)
					{
						string text = this.LocalName;
						if (text != null)
						{
							if (XNodeReader.<>f__switch$map0 == null)
							{
								XNodeReader.<>f__switch$map0 = new Dictionary<string, int>(2)
								{
									{ "version", 0 },
									{ "encoding", 1 }
								};
							}
							int num;
							if (XNodeReader.<>f__switch$map0.TryGetValue(text, out num))
							{
								if (num == 0)
								{
									return declaration.Version;
								}
								if (num == 1)
								{
									return declaration.Encoding;
								}
							}
						}
						return declaration.Standalone;
					}
					string text2 = declaration.ToString();
					return text2.Substring(6, text2.Length - 6 - 2);
				}
				case XmlNodeType.DocumentType:
				{
					XDocumentType xdocumentType = (XDocumentType)this.node;
					string text = this.LocalName;
					if (text != null)
					{
						if (XNodeReader.<>f__switch$map1 == null)
						{
							XNodeReader.<>f__switch$map1 = new Dictionary<string, int>(2)
							{
								{ "PUBLIC", 0 },
								{ "SYSTEM", 1 }
							};
						}
						int num;
						if (XNodeReader.<>f__switch$map1.TryGetValue(text, out num))
						{
							if (num == 0)
							{
								return xdocumentType.PublicId;
							}
							if (num == 1)
							{
								return xdocumentType.SystemId;
							}
						}
					}
					return xdocumentType.InternalSubset;
				}
				}
				return string.Empty;
			}
		}

		public override void Close()
		{
			this.state = ReadState.Closed;
		}

		public override string LookupNamespace(string prefix)
		{
			if (this.EOF)
			{
				return null;
			}
			XElement xelement = (this.node as XElement) ?? this.node.Parent;
			if (xelement == null)
			{
				return null;
			}
			XNamespace namespaceOfPrefix = xelement.GetNamespaceOfPrefix(prefix);
			return (!(namespaceOfPrefix != XNamespace.None)) ? null : namespaceOfPrefix.NamespaceName;
		}

		public override bool MoveToElement()
		{
			if (this.attr >= 0)
			{
				this.attr_value = false;
				this.attr = -1;
				return true;
			}
			return false;
		}

		public override bool MoveToFirstAttribute()
		{
			if (this.AttributeCount > 0)
			{
				this.attr = 0;
				this.attr_value = false;
				return true;
			}
			return false;
		}

		public override bool MoveToNextAttribute()
		{
			int attributeCount = this.AttributeCount;
			if (this.attr + 1 < attributeCount)
			{
				this.attr++;
				this.attr_value = false;
				return true;
			}
			return false;
		}

		public override bool MoveToAttribute(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			int attributeCount = this.AttributeCount;
			bool flag = false;
			for (int i = 0; i < attributeCount; i++)
			{
				object name2 = this.GetName(i);
				if (name2 != null)
				{
					if (name2 as string == name)
					{
						flag = true;
					}
					XName xname = (XName)name2;
					if (name.EndsWith(xname.LocalName, StringComparison.Ordinal) && name == this.GetPrefixedName((XName)name2))
					{
						flag = true;
					}
					if (flag)
					{
						this.attr = i;
						this.attr_value = false;
						return true;
					}
				}
			}
			return false;
		}

		private string GetPrefixedName(XName name)
		{
			XElement xelement = (this.node as XElement) ?? this.node.Parent;
			if (xelement == null || name.Namespace == XNamespace.None || xelement.GetPrefixOfNamespace(name.Namespace) == string.Empty)
			{
				return name.LocalName;
			}
			return xelement.GetPrefixOfNamespace(name.Namespace) + ":" + name.LocalName;
		}

		public override bool MoveToAttribute(string local, string ns)
		{
			if (local == null)
			{
				throw new ArgumentNullException("local");
			}
			if (ns == null)
			{
				throw new ArgumentNullException("ns");
			}
			int attributeCount = this.AttributeCount;
			bool flag = false;
			for (int i = 0; i < attributeCount; i++)
			{
				object name = this.GetName(i);
				if (name != null)
				{
					if (name as string == local && ns.Length == 0)
					{
						flag = true;
					}
					XName xname = (XName)name;
					if (local == xname.LocalName && ns == xname.NamespaceName)
					{
						flag = true;
					}
					if (flag)
					{
						this.attr = i;
						this.attr_value = false;
						return true;
					}
				}
			}
			return false;
		}

		public override string GetAttribute(int i)
		{
			int num = this.attr;
			bool flag = this.attr_value;
			string value;
			try
			{
				this.MoveToElement();
				this.MoveToAttribute(i);
				value = this.Value;
			}
			finally
			{
				this.attr = num;
				this.attr_value = flag;
			}
			return value;
		}

		public override string GetAttribute(string name)
		{
			int num = this.attr;
			bool flag = this.attr_value;
			string text;
			try
			{
				this.MoveToElement();
				text = ((!this.MoveToAttribute(name)) ? null : this.Value);
			}
			finally
			{
				this.attr = num;
				this.attr_value = flag;
			}
			return text;
		}

		public override string GetAttribute(string local, string ns)
		{
			int num = this.attr;
			bool flag = this.attr_value;
			string text;
			try
			{
				this.MoveToElement();
				text = ((!this.MoveToAttribute(local, ns)) ? null : this.Value);
			}
			finally
			{
				this.attr = num;
				this.attr_value = flag;
			}
			return text;
		}

		public override bool Read()
		{
			this.attr = -1;
			this.attr_value = false;
			ReadState readState = this.state;
			if (readState != ReadState.Initial)
			{
				if (readState != ReadState.Interactive)
				{
					return false;
				}
			}
			else
			{
				this.state = ReadState.Interactive;
				XDocument xdocument = this.node as XDocument;
				if (xdocument == null)
				{
					return true;
				}
				if (xdocument.Declaration != null)
				{
					return true;
				}
			}
			if (this.node is XDocument)
			{
				XDocument xdocument2 = this.node as XDocument;
				this.node = xdocument2.FirstNode;
				if (this.node == null)
				{
					this.state = ReadState.EndOfFile;
					return false;
				}
				this.node = xdocument2.FirstNode;
				return true;
			}
			else
			{
				XElement xelement = this.node as XElement;
				if (xelement != null && !this.end_element)
				{
					if (xelement.FirstNode != null)
					{
						this.node = xelement.FirstNode;
						return true;
					}
					if (!xelement.IsEmpty)
					{
						this.end_element = true;
						return true;
					}
				}
				this.end_element = false;
				if (this.node.NextNode != null && this.node != this.start)
				{
					this.node = this.node.NextNode;
					return true;
				}
				if (this.node.Parent == null || this.node == this.start)
				{
					this.state = ReadState.EndOfFile;
					return false;
				}
				this.node = this.node.Parent;
				this.end_element = true;
				return true;
			}
		}

		public override bool ReadAttributeValue()
		{
			if (this.attr < 0 || this.attr_value)
			{
				return false;
			}
			this.attr_value = true;
			return true;
		}

		public override void ResolveEntity()
		{
			throw new NotSupportedException();
		}

		private ReadState state;

		private XNode node;

		private XNode start;

		private int attr = -1;

		private bool attr_value;

		private bool end_element;

		private NameTable name_table = new NameTable();
	}
}
