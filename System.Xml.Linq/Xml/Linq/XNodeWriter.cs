using System;

namespace System.Xml.Linq
{
	internal class XNodeWriter : XmlWriter
	{
		public XNodeWriter(XContainer fragment)
		{
			this.root = fragment;
			this.state = XmlNodeType.None;
			this.current = fragment;
		}

		public override WriteState WriteState
		{
			get
			{
				if (this.is_closed)
				{
					return WriteState.Closed;
				}
				if (this.attribute != null)
				{
					return WriteState.Attribute;
				}
				XmlNodeType xmlNodeType = this.state;
				if (xmlNodeType == XmlNodeType.None)
				{
					return WriteState.Start;
				}
				if (xmlNodeType == XmlNodeType.DocumentType)
				{
					return WriteState.Element;
				}
				if (xmlNodeType != XmlNodeType.XmlDeclaration)
				{
					return WriteState.Content;
				}
				return WriteState.Prolog;
			}
		}

		private void CheckState()
		{
			if (this.is_closed)
			{
				throw new InvalidOperationException();
			}
		}

		private void WritePossiblyTopLevelNode(XNode n, bool possiblyAttribute)
		{
			this.CheckState();
			if (!possiblyAttribute && this.attribute != null)
			{
				throw new InvalidOperationException(string.Format("Current state is not acceptable for {0}.", n.NodeType));
			}
			if (this.state != XmlNodeType.Element)
			{
				this.root.Add(n);
			}
			else if (this.attribute != null)
			{
				XAttribute xattribute = this.attribute;
				xattribute.Value += XUtil.ToString(n);
			}
			else
			{
				this.current.Add(n);
			}
			if (this.state == XmlNodeType.None)
			{
				this.state = XmlNodeType.XmlDeclaration;
			}
		}

		private void FillXmlns(XElement el, string prefix, XNamespace xns)
		{
			if (xns == XNamespace.Xmlns)
			{
				return;
			}
			if (xns == XNamespace.None)
			{
				if (el.GetPrefixOfNamespace(xns) != prefix)
				{
					el.SetAttributeValue((!(prefix == string.Empty)) ? XNamespace.Xmlns.GetName(prefix) : XNamespace.None.GetName("xmlns"), xns.NamespaceName);
				}
				else if (el.GetDefaultNamespace() != XNamespace.None)
				{
					el.SetAttributeValue(XNamespace.None.GetName("xmlns"), xns.NamespaceName);
				}
			}
		}

		public override void Close()
		{
			this.CheckState();
			this.is_closed = true;
		}

		public override void Flush()
		{
		}

		public override string LookupPrefix(string ns)
		{
			this.CheckState();
			if (this.current == null)
			{
				throw new InvalidOperationException();
			}
			XElement xelement = (this.current as XElement) ?? this.current.Parent;
			return (xelement == null) ? null : xelement.GetPrefixOfNamespace(XNamespace.Get(ns));
		}

		public override void WriteStartDocument()
		{
			this.WriteStartDocument(null);
		}

		public override void WriteStartDocument(bool standalone)
		{
			this.WriteStartDocument((!standalone) ? "no" : "yes");
		}

		private void WriteStartDocument(string sddecl)
		{
			this.CheckState();
			if (this.state != XmlNodeType.None)
			{
				throw new InvalidOperationException("Current state is not acceptable for xmldecl.");
			}
			XDocument xdocument = this.current as XDocument;
			if (xdocument == null)
			{
				throw new InvalidOperationException("Only document node can accept xml declaration");
			}
			xdocument.Declaration = new XDeclaration("1.0", null, sddecl);
			this.state = XmlNodeType.XmlDeclaration;
		}

		public override void WriteEndDocument()
		{
			this.CheckState();
			this.is_closed = true;
		}

		public override void WriteDocType(string name, string publicId, string systemId, string internalSubset)
		{
			this.CheckState();
			XmlNodeType xmlNodeType = this.state;
			if (xmlNodeType != XmlNodeType.None && xmlNodeType != XmlNodeType.XmlDeclaration)
			{
				throw new InvalidOperationException("Current state is not acceptable for doctype.");
			}
			XDocument xdocument = this.current as XDocument;
			if (xdocument == null)
			{
				throw new InvalidOperationException("Only document node can accept doctype declaration");
			}
			xdocument.Add(new XDocumentType(name, publicId, systemId, internalSubset));
			this.state = XmlNodeType.DocumentType;
		}

		public override void WriteStartElement(string prefix, string name, string ns)
		{
			this.CheckState();
			XNamespace xnamespace = XNamespace.Get(ns ?? string.Empty);
			XElement xelement = new XElement(xnamespace.GetName(name));
			if (this.current == null)
			{
				this.root.Add(xelement);
				this.state = XmlNodeType.Element;
			}
			else
			{
				this.current.Add(xelement);
				this.state = XmlNodeType.Element;
			}
			this.FillXmlns(xelement, prefix ?? string.Empty, xnamespace);
			this.current = xelement;
		}

		public override void WriteEndElement()
		{
			this.WriteEndElementInternal(false);
		}

		public override void WriteFullEndElement()
		{
			this.WriteEndElementInternal(true);
		}

		private void WriteEndElementInternal(bool forceFull)
		{
			this.CheckState();
			if (this.current == null)
			{
				throw new InvalidOperationException("Current state is not acceptable for endElement.");
			}
			XElement xelement = this.current as XElement;
			if (forceFull)
			{
				xelement.IsEmpty = false;
			}
			this.current = this.current.Parent;
		}

		public override void WriteStartAttribute(string prefix, string name, string ns)
		{
			this.CheckState();
			if (this.attribute != null)
			{
				throw new InvalidOperationException("There is an open attribute.");
			}
			XElement xelement = this.current as XElement;
			if (xelement == null)
			{
				throw new InvalidOperationException("Current state is not acceptable for startAttribute.");
			}
			if (prefix == null)
			{
				prefix = string.Empty;
			}
			if (prefix.Length == 0 && name == "xmlns" && ns == XNamespace.Xmlns.NamespaceName)
			{
				ns = string.Empty;
			}
			XNamespace xnamespace = XNamespace.Get(ns);
			xelement.SetAttributeValue(xnamespace.GetName(name), string.Empty);
			this.attribute = xelement.LastAttribute;
			this.FillXmlns(xelement, prefix, xnamespace);
		}

		public override void WriteEndAttribute()
		{
			this.CheckState();
			if (this.attribute == null)
			{
				throw new InvalidOperationException("Current state is not acceptable for startAttribute.");
			}
			this.attribute = null;
		}

		public override void WriteCData(string data)
		{
			this.CheckState();
			if (this.current == null)
			{
				throw new InvalidOperationException("Current state is not acceptable for CDATAsection.");
			}
			this.current.Add(new XCData(data));
		}

		public override void WriteComment(string comment)
		{
			this.WritePossiblyTopLevelNode(new XComment(comment), false);
		}

		public override void WriteProcessingInstruction(string name, string value)
		{
			this.WritePossiblyTopLevelNode(new XProcessingInstruction(name, value), false);
		}

		public override void WriteEntityRef(string name)
		{
			throw new NotSupportedException();
		}

		public override void WriteCharEntity(char c)
		{
			throw new NotSupportedException();
		}

		public override void WriteWhitespace(string ws)
		{
			this.WritePossiblyTopLevelNode(new XText(ws), true);
		}

		public override void WriteString(string data)
		{
			this.CheckState();
			if (this.current == null)
			{
				throw new InvalidOperationException("Current state is not acceptable for Text.");
			}
			if (this.attribute != null)
			{
				XAttribute xattribute = this.attribute;
				xattribute.Value += data;
			}
			else
			{
				this.current.Add(data);
			}
		}

		public override void WriteName(string name)
		{
			this.WriteString(name);
		}

		public override void WriteNmToken(string nmtoken)
		{
			this.WriteString(nmtoken);
		}

		public override void WriteQualifiedName(string name, string ns)
		{
			string text = this.LookupPrefix(ns);
			if (text == null)
			{
				throw new ArgumentException(string.Format("Invalid namespace {0}", ns));
			}
			if (text != string.Empty)
			{
				this.WriteString(name);
			}
			else
			{
				this.WriteString(text + ":" + name);
			}
		}

		public override void WriteChars(char[] chars, int start, int len)
		{
			this.WriteString(new string(chars, start, len));
		}

		public override void WriteRaw(string data)
		{
			this.WriteString(data);
		}

		public override void WriteRaw(char[] chars, int start, int len)
		{
			this.WriteChars(chars, start, len);
		}

		public override void WriteBase64(byte[] data, int start, int len)
		{
			this.WriteString(Convert.ToBase64String(data, start, len));
		}

		public override void WriteBinHex(byte[] data, int start, int len)
		{
			throw new NotImplementedException();
		}

		public override void WriteSurrogateCharEntity(char c1, char c2)
		{
			throw new NotImplementedException();
		}

		private XContainer root;

		private bool is_closed;

		private XContainer current;

		private XAttribute attribute;

		private XmlNodeType state;
	}
}
