using System;
using System.Globalization;
using System.IO;
using System.Runtime;
using System.Runtime.Serialization;
using System.Text;

namespace System.Xml
{
	internal abstract class XmlBaseWriter : XmlDictionaryWriter, IFragmentCapableXmlDictionaryWriter
	{
		protected XmlBaseWriter()
		{
			this.nsMgr = new XmlBaseWriter.NamespaceManager();
			this.writeState = WriteState.Start;
			this.documentState = XmlBaseWriter.DocumentState.None;
		}

		protected void SetOutput(XmlStreamNodeWriter writer)
		{
			this.inList = false;
			this.writer = writer;
			this.nodeWriter = writer;
			this.writeState = WriteState.Start;
			this.documentState = XmlBaseWriter.DocumentState.None;
			this.nsMgr.Clear();
			if (this.depth != 0)
			{
				this.elements = null;
				this.depth = 0;
			}
			this.attributeLocalName = null;
			this.attributeValue = null;
			this.oldWriter = null;
			this.oldStream = null;
		}

		public override void Flush()
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			this.writer.Flush();
		}

		public override void Close()
		{
			if (this.IsClosed)
			{
				return;
			}
			try
			{
				this.FinishDocument();
				this.AutoComplete(WriteState.Closed);
				this.writer.Flush();
			}
			finally
			{
				this.nsMgr.Close();
				if (this.depth != 0)
				{
					this.elements = null;
					this.depth = 0;
				}
				this.attributeValue = null;
				this.attributeLocalName = null;
				this.nodeWriter.Close();
				if (this.signingWriter != null)
				{
					this.signingWriter.Close();
				}
				if (this.textFragmentWriter != null)
				{
					this.textFragmentWriter.Close();
				}
				this.oldWriter = null;
				this.oldStream = null;
			}
		}

		protected bool IsClosed
		{
			get
			{
				return this.writeState == WriteState.Closed;
			}
		}

		protected void ThrowClosed()
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("The XmlWriter is closed.")));
		}

		private static BinHexEncoding BinHexEncoding
		{
			get
			{
				if (XmlBaseWriter.binhexEncoding == null)
				{
					XmlBaseWriter.binhexEncoding = new BinHexEncoding();
				}
				return XmlBaseWriter.binhexEncoding;
			}
		}

		public override string XmlLang
		{
			get
			{
				return this.nsMgr.XmlLang;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				return this.nsMgr.XmlSpace;
			}
		}

		public override WriteState WriteState
		{
			get
			{
				return this.writeState;
			}
		}

		public override void WriteXmlnsAttribute(string prefix, string ns)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			if (ns == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("ns");
			}
			if (this.writeState != WriteState.Element)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("'{0}' cannot be called while WriteState is '{1}'.", new object[]
				{
					"WriteXmlnsAttribute",
					this.WriteState.ToString()
				})));
			}
			if (prefix == null)
			{
				prefix = this.nsMgr.LookupPrefix(ns);
				if (prefix == null)
				{
					this.GeneratePrefix(ns, null);
					return;
				}
			}
			else
			{
				this.nsMgr.AddNamespaceIfNotDeclared(prefix, ns, null);
			}
		}

		public override void WriteXmlnsAttribute(string prefix, XmlDictionaryString ns)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			if (ns == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("ns");
			}
			if (this.writeState != WriteState.Element)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("'{0}' cannot be called while WriteState is '{1}'.", new object[]
				{
					"WriteXmlnsAttribute",
					this.WriteState.ToString()
				})));
			}
			if (prefix == null)
			{
				prefix = this.nsMgr.LookupPrefix(ns.Value);
				if (prefix == null)
				{
					this.GeneratePrefix(ns.Value, ns);
					return;
				}
			}
			else
			{
				this.nsMgr.AddNamespaceIfNotDeclared(prefix, ns.Value, ns);
			}
		}

		private void StartAttribute(ref string prefix, string localName, string ns, XmlDictionaryString xNs)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			if (this.writeState == WriteState.Attribute)
			{
				this.WriteEndAttribute();
			}
			if (localName == null || (localName.Length == 0 && prefix != "xmlns"))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("localName"));
			}
			if (this.writeState != WriteState.Element)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("'{0}' cannot be called while WriteState is '{1}'.", new object[]
				{
					"WriteStartAttribute",
					this.WriteState.ToString()
				})));
			}
			if (prefix == null)
			{
				if (ns == "http://www.w3.org/2000/xmlns/" && localName != "xmlns")
				{
					prefix = "xmlns";
				}
				else if (ns == "http://www.w3.org/XML/1998/namespace")
				{
					prefix = "xml";
				}
				else
				{
					prefix = string.Empty;
				}
			}
			if (prefix.Length == 0 && localName == "xmlns")
			{
				prefix = "xmlns";
				localName = string.Empty;
			}
			this.isXmlnsAttribute = false;
			this.isXmlAttribute = false;
			if (prefix == "xml")
			{
				if (ns != null && ns != "http://www.w3.org/XML/1998/namespace")
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("The prefix '{0}' is bound to the namespace '{1}' and cannot be changed to '{2}'.", new object[] { "xml", "http://www.w3.org/XML/1998/namespace", ns }), "ns"));
				}
				this.isXmlAttribute = true;
				this.attributeValue = string.Empty;
				this.attributeLocalName = localName;
			}
			else if (prefix == "xmlns")
			{
				if (ns != null && ns != "http://www.w3.org/2000/xmlns/")
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("The prefix '{0}' is bound to the namespace '{1}' and cannot be changed to '{2}'.", new object[] { "xmlns", "http://www.w3.org/2000/xmlns/", ns }), "ns"));
				}
				this.isXmlnsAttribute = true;
				this.attributeValue = string.Empty;
				this.attributeLocalName = localName;
			}
			else if (ns == null)
			{
				if (prefix.Length == 0)
				{
					ns = string.Empty;
				}
				else
				{
					ns = this.nsMgr.LookupNamespace(prefix);
					if (ns == null)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("The prefix '{0}' is not defined.", new object[] { prefix }), "prefix"));
					}
				}
			}
			else if (ns.Length == 0)
			{
				if (prefix.Length != 0)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("The empty namespace requires a null or empty prefix."), "prefix"));
				}
			}
			else if (prefix.Length == 0)
			{
				prefix = this.nsMgr.LookupAttributePrefix(ns);
				if (prefix == null)
				{
					if (ns.Length == "http://www.w3.org/2000/xmlns/".Length && ns == "http://www.w3.org/2000/xmlns/")
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("The namespace '{1}' can only be bound to the prefix '{0}'.", new object[] { "xmlns", ns })));
					}
					if (ns.Length == "http://www.w3.org/XML/1998/namespace".Length && ns == "http://www.w3.org/XML/1998/namespace")
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("The namespace '{1}' can only be bound to the prefix '{0}'.", new object[] { "xml", ns })));
					}
					prefix = this.GeneratePrefix(ns, xNs);
				}
			}
			else
			{
				this.nsMgr.AddNamespaceIfNotDeclared(prefix, ns, xNs);
			}
			this.writeState = WriteState.Attribute;
		}

		public override void WriteStartAttribute(string prefix, string localName, string namespaceUri)
		{
			this.StartAttribute(ref prefix, localName, namespaceUri, null);
			if (!this.isXmlnsAttribute)
			{
				this.writer.WriteStartAttribute(prefix, localName);
			}
		}

		public override void WriteStartAttribute(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			this.StartAttribute(ref prefix, (localName != null) ? localName.Value : null, (namespaceUri != null) ? namespaceUri.Value : null, namespaceUri);
			if (!this.isXmlnsAttribute)
			{
				this.writer.WriteStartAttribute(prefix, localName);
			}
		}

		public override void WriteEndAttribute()
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			if (this.writeState != WriteState.Attribute)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("'{0}' cannot be called while WriteState is '{1}'.", new object[]
				{
					"WriteEndAttribute",
					this.WriteState.ToString()
				})));
			}
			this.FlushBase64();
			try
			{
				if (this.isXmlAttribute)
				{
					if (this.attributeLocalName == "lang")
					{
						this.nsMgr.AddLangAttribute(this.attributeValue);
					}
					else if (this.attributeLocalName == "space")
					{
						if (this.attributeValue == "preserve")
						{
							this.nsMgr.AddSpaceAttribute(XmlSpace.Preserve);
						}
						else
						{
							if (!(this.attributeValue == "default"))
							{
								throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("'{0}' is not a valid xml:space value. Valid values are 'default' and 'preserve'.", new object[] { this.attributeValue })));
							}
							this.nsMgr.AddSpaceAttribute(XmlSpace.Default);
						}
					}
					this.isXmlAttribute = false;
					this.attributeLocalName = null;
					this.attributeValue = null;
				}
				if (this.isXmlnsAttribute)
				{
					this.nsMgr.AddNamespaceIfNotDeclared(this.attributeLocalName, this.attributeValue, null);
					this.isXmlnsAttribute = false;
					this.attributeLocalName = null;
					this.attributeValue = null;
				}
				else
				{
					this.writer.WriteEndAttribute();
				}
			}
			finally
			{
				this.writeState = WriteState.Element;
			}
		}

		public override void WriteComment(string text)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			if (this.writeState == WriteState.Attribute)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("'{0}' cannot be called while WriteState is '{1}'.", new object[]
				{
					"WriteComment",
					this.WriteState.ToString()
				})));
			}
			if (text == null)
			{
				text = string.Empty;
			}
			else if (text.IndexOf("--", StringComparison.Ordinal) != -1 || (text.Length > 0 && text[text.Length - 1] == '-'))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("XML comments cannot contain '--' or end with '-'."), "text"));
			}
			this.StartComment();
			this.FlushBase64();
			this.writer.WriteComment(text);
			this.EndComment();
		}

		public override void WriteFullEndElement()
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			if (this.writeState == WriteState.Attribute)
			{
				this.WriteEndAttribute();
			}
			if (this.writeState != WriteState.Element && this.writeState != WriteState.Content)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("'{0}' cannot be called while WriteState is '{1}'.", new object[]
				{
					"WriteFullEndElement",
					this.WriteState.ToString()
				})));
			}
			this.AutoComplete(WriteState.Content);
			this.WriteEndElement();
		}

		public override void WriteCData(string text)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			if (this.writeState == WriteState.Attribute)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("'{0}' cannot be called while WriteState is '{1}'.", new object[]
				{
					"WriteCData",
					this.WriteState.ToString()
				})));
			}
			if (text == null)
			{
				text = string.Empty;
			}
			if (text.Length > 0)
			{
				this.StartContent();
				this.FlushBase64();
				this.writer.WriteCData(text);
				this.EndContent();
			}
		}

		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("This XmlWriter implementation does not support the '{0}' method.", new object[] { "WriteDocType" })));
		}

		private void StartElement(ref string prefix, string localName, string ns, XmlDictionaryString xNs)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			if (this.documentState == XmlBaseWriter.DocumentState.Epilog)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("Only one root element is permitted per document.")));
			}
			if (localName == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("localName"));
			}
			if (localName.Length == 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("The empty string is not a valid local name."), "localName"));
			}
			if (this.writeState == WriteState.Attribute)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("'{0}' cannot be called while WriteState is '{1}'.", new object[]
				{
					"WriteStartElement",
					this.WriteState.ToString()
				})));
			}
			this.FlushBase64();
			this.AutoComplete(WriteState.Element);
			XmlBaseWriter.Element element = this.EnterScope();
			if (ns == null)
			{
				if (prefix == null)
				{
					prefix = string.Empty;
				}
				ns = this.nsMgr.LookupNamespace(prefix);
				if (ns == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("The prefix '{0}' is not defined.", new object[] { prefix }), "prefix"));
				}
			}
			else if (prefix == null)
			{
				prefix = this.nsMgr.LookupPrefix(ns);
				if (prefix == null)
				{
					prefix = string.Empty;
					this.nsMgr.AddNamespace(string.Empty, ns, xNs);
				}
			}
			else
			{
				this.nsMgr.AddNamespaceIfNotDeclared(prefix, ns, xNs);
			}
			element.Prefix = prefix;
			element.LocalName = localName;
		}

		public override void WriteStartElement(string prefix, string localName, string namespaceUri)
		{
			this.StartElement(ref prefix, localName, namespaceUri, null);
			this.writer.WriteStartElement(prefix, localName);
		}

		public override void WriteStartElement(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			this.StartElement(ref prefix, (localName != null) ? localName.Value : null, (namespaceUri != null) ? namespaceUri.Value : null, namespaceUri);
			this.writer.WriteStartElement(prefix, localName);
		}

		public override void WriteEndElement()
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			if (this.depth == 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("Cannot call '{0}' while Depth is '{1}'.", new object[]
				{
					"WriteEndElement",
					this.depth.ToString(CultureInfo.InvariantCulture)
				})));
			}
			if (this.writeState == WriteState.Attribute)
			{
				this.WriteEndAttribute();
			}
			this.FlushBase64();
			if (this.writeState == WriteState.Element)
			{
				this.nsMgr.DeclareNamespaces(this.writer);
				this.writer.WriteEndStartElement(true);
			}
			else
			{
				XmlBaseWriter.Element element = this.elements[this.depth];
				this.writer.WriteEndElement(element.Prefix, element.LocalName);
			}
			this.ExitScope();
			this.writeState = WriteState.Content;
		}

		private XmlBaseWriter.Element EnterScope()
		{
			this.nsMgr.EnterScope();
			this.depth++;
			if (this.elements == null)
			{
				this.elements = new XmlBaseWriter.Element[4];
			}
			else if (this.elements.Length == this.depth)
			{
				XmlBaseWriter.Element[] array = new XmlBaseWriter.Element[this.depth * 2];
				Array.Copy(this.elements, array, this.depth);
				this.elements = array;
			}
			XmlBaseWriter.Element element = this.elements[this.depth];
			if (element == null)
			{
				element = new XmlBaseWriter.Element();
				this.elements[this.depth] = element;
			}
			return element;
		}

		private void ExitScope()
		{
			this.elements[this.depth].Clear();
			this.depth--;
			if (this.depth == 0 && this.documentState == XmlBaseWriter.DocumentState.Document)
			{
				this.documentState = XmlBaseWriter.DocumentState.Epilog;
			}
			this.nsMgr.ExitScope();
		}

		protected void FlushElement()
		{
			if (this.writeState == WriteState.Element)
			{
				this.AutoComplete(WriteState.Content);
			}
		}

		protected void StartComment()
		{
			this.FlushElement();
		}

		protected void EndComment()
		{
		}

		protected void StartContent()
		{
			this.FlushElement();
			if (this.depth == 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("Text cannot be written outside the root element.")));
			}
		}

		protected void StartContent(char ch)
		{
			this.FlushElement();
			if (this.depth == 0)
			{
				this.VerifyWhitespace(ch);
			}
		}

		protected void StartContent(string s)
		{
			this.FlushElement();
			if (this.depth == 0)
			{
				this.VerifyWhitespace(s);
			}
		}

		protected void StartContent(char[] chars, int offset, int count)
		{
			this.FlushElement();
			if (this.depth == 0)
			{
				this.VerifyWhitespace(chars, offset, count);
			}
		}

		private void VerifyWhitespace(char ch)
		{
			if (!this.IsWhitespace(ch))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("Text cannot be written outside the root element.")));
			}
		}

		private void VerifyWhitespace(string s)
		{
			for (int i = 0; i < s.Length; i++)
			{
				if (!this.IsWhitespace(s[i]))
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("Text cannot be written outside the root element.")));
				}
			}
		}

		private void VerifyWhitespace(char[] chars, int offset, int count)
		{
			for (int i = 0; i < count; i++)
			{
				if (!this.IsWhitespace(chars[offset + i]))
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("Text cannot be written outside the root element.")));
				}
			}
		}

		private bool IsWhitespace(char ch)
		{
			return ch == ' ' || ch == '\n' || ch == '\r' || ch == 't';
		}

		protected void EndContent()
		{
		}

		private void AutoComplete(WriteState writeState)
		{
			if (this.writeState == WriteState.Element)
			{
				this.EndStartElement();
			}
			this.writeState = writeState;
		}

		private void EndStartElement()
		{
			this.nsMgr.DeclareNamespaces(this.writer);
			this.writer.WriteEndStartElement(false);
		}

		public override string LookupPrefix(string ns)
		{
			if (ns == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("ns"));
			}
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			return this.nsMgr.LookupPrefix(ns);
		}

		internal string LookupNamespace(string prefix)
		{
			if (prefix == null)
			{
				return null;
			}
			return this.nsMgr.LookupNamespace(prefix);
		}

		private string GetQualifiedNamePrefix(string namespaceUri, XmlDictionaryString xNs)
		{
			string text = this.nsMgr.LookupPrefix(namespaceUri);
			if (text == null)
			{
				if (this.writeState != WriteState.Attribute)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("The namespace '{0}' is not defined.", new object[] { namespaceUri }), "namespaceUri"));
				}
				text = this.GeneratePrefix(namespaceUri, xNs);
			}
			return text;
		}

		public override void WriteQualifiedName(string localName, string namespaceUri)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			if (localName == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("localName"));
			}
			if (localName.Length == 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("The empty string is not a valid local name."), "localName"));
			}
			if (namespaceUri == null)
			{
				namespaceUri = string.Empty;
			}
			string qualifiedNamePrefix = this.GetQualifiedNamePrefix(namespaceUri, null);
			if (qualifiedNamePrefix.Length != 0)
			{
				this.WriteString(qualifiedNamePrefix);
				this.WriteString(":");
			}
			this.WriteString(localName);
		}

		public override void WriteQualifiedName(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			if (localName == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("localName"));
			}
			if (localName.Value.Length == 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("The empty string is not a valid local name."), "localName"));
			}
			if (namespaceUri == null)
			{
				namespaceUri = XmlDictionaryString.Empty;
			}
			string qualifiedNamePrefix = this.GetQualifiedNamePrefix(namespaceUri.Value, namespaceUri);
			this.FlushBase64();
			if (this.attributeValue != null)
			{
				this.WriteAttributeText(qualifiedNamePrefix + ":" + namespaceUri.Value);
			}
			if (!this.isXmlnsAttribute)
			{
				this.StartContent();
				this.writer.WriteQualifiedName(qualifiedNamePrefix, localName);
				this.EndContent();
			}
		}

		public override void WriteStartDocument()
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			if (this.writeState != WriteState.Start)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("'{0}' cannot be called while WriteState is '{1}'.", new object[]
				{
					"WriteStartDocument",
					this.WriteState.ToString()
				})));
			}
			this.writeState = WriteState.Prolog;
			this.documentState = XmlBaseWriter.DocumentState.Document;
			this.writer.WriteDeclaration();
		}

		public override void WriteStartDocument(bool standalone)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			this.WriteStartDocument();
		}

		public override void WriteProcessingInstruction(string name, string text)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			if (name != "xml")
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("Processing instructions (other than the XML declaration) and DTDs are not supported."), "name"));
			}
			if (this.writeState != WriteState.Start)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("XML declaration can only be written at the beginning of the document.")));
			}
			this.writer.WriteDeclaration();
		}

		private void FinishDocument()
		{
			if (this.writeState == WriteState.Attribute)
			{
				this.WriteEndAttribute();
			}
			while (this.depth > 0)
			{
				this.WriteEndElement();
			}
		}

		public override void WriteEndDocument()
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			if (this.writeState == WriteState.Start || this.writeState == WriteState.Prolog)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("The document does not have a root element.")));
			}
			this.FinishDocument();
			this.writeState = WriteState.Start;
			this.documentState = XmlBaseWriter.DocumentState.End;
		}

		protected int NamespaceBoundary
		{
			get
			{
				return this.nsMgr.NamespaceBoundary;
			}
			set
			{
				this.nsMgr.NamespaceBoundary = value;
			}
		}

		public override void WriteEntityRef(string name)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("This XmlWriter implementation does not support the '{0}' method.", new object[] { "WriteEntityRef" })));
		}

		public override void WriteName(string name)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			this.WriteString(name);
		}

		public override void WriteNmToken(string name)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("This XmlWriter implementation does not support the '{0}' method.", new object[] { "WriteNmToken" })));
		}

		public override void WriteWhitespace(string whitespace)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			if (whitespace == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("whitespace");
			}
			foreach (char c in whitespace)
			{
				if (c != ' ' && c != '\t' && c != '\n' && c != '\r')
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("Only white space characters can be written with this method."), "whitespace"));
				}
			}
			this.WriteString(whitespace);
		}

		public override void WriteString(string value)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			if (value == null)
			{
				value = string.Empty;
			}
			if (value.Length > 0 || this.inList)
			{
				this.FlushBase64();
				if (this.attributeValue != null)
				{
					this.WriteAttributeText(value);
				}
				if (!this.isXmlnsAttribute)
				{
					this.StartContent(value);
					this.writer.WriteEscapedText(value);
					this.EndContent();
				}
			}
		}

		public override void WriteString(XmlDictionaryString value)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			if (value == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
			}
			if (value.Value.Length > 0)
			{
				this.FlushBase64();
				if (this.attributeValue != null)
				{
					this.WriteAttributeText(value.Value);
				}
				if (!this.isXmlnsAttribute)
				{
					this.StartContent(value.Value);
					this.writer.WriteEscapedText(value);
					this.EndContent();
				}
			}
		}

		public override void WriteChars(char[] chars, int offset, int count)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			if (chars == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("chars"));
			}
			if (offset < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (count < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (count > chars.Length - offset)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The specified size exceeds the remaining buffer space ({0} bytes).", new object[] { chars.Length - offset })));
			}
			if (count > 0)
			{
				this.FlushBase64();
				if (this.attributeValue != null)
				{
					this.WriteAttributeText(new string(chars, offset, count));
				}
				if (!this.isXmlnsAttribute)
				{
					this.StartContent(chars, offset, count);
					this.writer.WriteEscapedText(chars, offset, count);
					this.EndContent();
				}
			}
		}

		public override void WriteRaw(string value)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			if (value == null)
			{
				value = string.Empty;
			}
			if (value.Length > 0)
			{
				this.FlushBase64();
				if (this.attributeValue != null)
				{
					this.WriteAttributeText(value);
				}
				if (!this.isXmlnsAttribute)
				{
					this.StartContent(value);
					this.writer.WriteText(value);
					this.EndContent();
				}
			}
		}

		public override void WriteRaw(char[] chars, int offset, int count)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			if (chars == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("chars"));
			}
			if (offset < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (count < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (count > chars.Length - offset)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The specified size exceeds the remaining buffer space ({0} bytes).", new object[] { chars.Length - offset })));
			}
			if (count > 0)
			{
				this.FlushBase64();
				if (this.attributeValue != null)
				{
					this.WriteAttributeText(new string(chars, offset, count));
				}
				if (!this.isXmlnsAttribute)
				{
					this.StartContent(chars, offset, count);
					this.writer.WriteText(chars, offset, count);
					this.EndContent();
				}
			}
		}

		public override void WriteCharEntity(char ch)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			if (ch >= '\ud800' && ch <= '\udfff')
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("The surrogate pair is invalid. Missing a low surrogate character."), "ch"));
			}
			if (this.attributeValue != null)
			{
				this.WriteAttributeText(ch.ToString());
			}
			if (!this.isXmlnsAttribute)
			{
				this.StartContent(ch);
				this.FlushBase64();
				this.writer.WriteCharEntity((int)ch);
				this.EndContent();
			}
		}

		public override void WriteSurrogateCharEntity(char lowChar, char highChar)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			SurrogateChar surrogateChar = new SurrogateChar(lowChar, highChar);
			if (this.attributeValue != null)
			{
				char[] array = new char[] { highChar, lowChar };
				this.WriteAttributeText(new string(array));
			}
			if (!this.isXmlnsAttribute)
			{
				this.StartContent();
				this.FlushBase64();
				this.writer.WriteCharEntity(surrogateChar.Char);
				this.EndContent();
			}
		}

		public override void WriteValue(object value)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			if (value == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("value"));
			}
			if (value is object[])
			{
				this.WriteValue((object[])value);
				return;
			}
			if (value is Array)
			{
				this.WriteValue((Array)value);
				return;
			}
			if (value is IStreamProvider)
			{
				this.WriteValue((IStreamProvider)value);
				return;
			}
			this.WritePrimitiveValue(value);
		}

		protected void WritePrimitiveValue(object value)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			if (value == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("value"));
			}
			if (value is ulong)
			{
				this.WriteValue((ulong)value);
				return;
			}
			if (value is string)
			{
				this.WriteValue((string)value);
				return;
			}
			if (value is int)
			{
				this.WriteValue((int)value);
				return;
			}
			if (value is long)
			{
				this.WriteValue((long)value);
				return;
			}
			if (value is bool)
			{
				this.WriteValue((bool)value);
				return;
			}
			if (value is double)
			{
				this.WriteValue((double)value);
				return;
			}
			if (value is DateTime)
			{
				this.WriteValue((DateTime)value);
				return;
			}
			if (value is float)
			{
				this.WriteValue((float)value);
				return;
			}
			if (value is decimal)
			{
				this.WriteValue((decimal)value);
				return;
			}
			if (value is XmlDictionaryString)
			{
				this.WriteValue((XmlDictionaryString)value);
				return;
			}
			if (value is UniqueId)
			{
				this.WriteValue((UniqueId)value);
				return;
			}
			if (value is Guid)
			{
				this.WriteValue((Guid)value);
				return;
			}
			if (value is TimeSpan)
			{
				this.WriteValue((TimeSpan)value);
				return;
			}
			if (value.GetType().IsArray)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("Nested arrays are not supported."), "value"));
			}
			base.WriteValue(value);
		}

		public override void WriteValue(string value)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			this.WriteString(value);
		}

		public override void WriteValue(int value)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			this.FlushBase64();
			if (this.attributeValue != null)
			{
				this.WriteAttributeText(XmlConverter.ToString(value));
			}
			if (!this.isXmlnsAttribute)
			{
				this.StartContent();
				this.writer.WriteInt32Text(value);
				this.EndContent();
			}
		}

		public override void WriteValue(long value)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			this.FlushBase64();
			if (this.attributeValue != null)
			{
				this.WriteAttributeText(XmlConverter.ToString(value));
			}
			if (!this.isXmlnsAttribute)
			{
				this.StartContent();
				this.writer.WriteInt64Text(value);
				this.EndContent();
			}
		}

		private void WriteValue(ulong value)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			this.FlushBase64();
			if (this.attributeValue != null)
			{
				this.WriteAttributeText(XmlConverter.ToString(value));
			}
			if (!this.isXmlnsAttribute)
			{
				this.StartContent();
				this.writer.WriteUInt64Text(value);
				this.EndContent();
			}
		}

		public override void WriteValue(bool value)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			this.FlushBase64();
			if (this.attributeValue != null)
			{
				this.WriteAttributeText(XmlConverter.ToString(value));
			}
			if (!this.isXmlnsAttribute)
			{
				this.StartContent();
				this.writer.WriteBoolText(value);
				this.EndContent();
			}
		}

		public override void WriteValue(decimal value)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			this.FlushBase64();
			if (this.attributeValue != null)
			{
				this.WriteAttributeText(XmlConverter.ToString(value));
			}
			if (!this.isXmlnsAttribute)
			{
				this.StartContent();
				this.writer.WriteDecimalText(value);
				this.EndContent();
			}
		}

		public override void WriteValue(float value)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			this.FlushBase64();
			if (this.attributeValue != null)
			{
				this.WriteAttributeText(XmlConverter.ToString(value));
			}
			if (!this.isXmlnsAttribute)
			{
				this.StartContent();
				this.writer.WriteFloatText(value);
				this.EndContent();
			}
		}

		public override void WriteValue(double value)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			this.FlushBase64();
			if (this.attributeValue != null)
			{
				this.WriteAttributeText(XmlConverter.ToString(value));
			}
			if (!this.isXmlnsAttribute)
			{
				this.StartContent();
				this.writer.WriteDoubleText(value);
				this.EndContent();
			}
		}

		public override void WriteValue(XmlDictionaryString value)
		{
			this.WriteString(value);
		}

		public override void WriteValue(DateTime value)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			this.FlushBase64();
			if (this.attributeValue != null)
			{
				this.WriteAttributeText(XmlConverter.ToString(value));
			}
			if (!this.isXmlnsAttribute)
			{
				this.StartContent();
				this.writer.WriteDateTimeText(value);
				this.EndContent();
			}
		}

		public override void WriteValue(UniqueId value)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			if (value == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
			}
			this.FlushBase64();
			if (this.attributeValue != null)
			{
				this.WriteAttributeText(XmlConverter.ToString(value));
			}
			if (!this.isXmlnsAttribute)
			{
				this.StartContent();
				this.writer.WriteUniqueIdText(value);
				this.EndContent();
			}
		}

		public override void WriteValue(Guid value)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			this.FlushBase64();
			if (this.attributeValue != null)
			{
				this.WriteAttributeText(XmlConverter.ToString(value));
			}
			if (!this.isXmlnsAttribute)
			{
				this.StartContent();
				this.writer.WriteGuidText(value);
				this.EndContent();
			}
		}

		public override void WriteValue(TimeSpan value)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			this.FlushBase64();
			if (this.attributeValue != null)
			{
				this.WriteAttributeText(XmlConverter.ToString(value));
			}
			if (!this.isXmlnsAttribute)
			{
				this.StartContent();
				this.writer.WriteTimeSpanText(value);
				this.EndContent();
			}
		}

		public override void WriteBase64(byte[] buffer, int offset, int count)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			this.EnsureBufferBounds(buffer, offset, count);
			if (count > 0)
			{
				if (this.trailByteCount > 0)
				{
					while (this.trailByteCount < 3 && count > 0)
					{
						byte[] array = this.trailBytes;
						int num = this.trailByteCount;
						this.trailByteCount = num + 1;
						array[num] = buffer[offset++];
						count--;
					}
				}
				int num2 = this.trailByteCount + count;
				int num3 = num2 - num2 % 3;
				if (this.trailBytes == null)
				{
					this.trailBytes = new byte[3];
				}
				if (num3 >= 3)
				{
					if (this.attributeValue != null)
					{
						this.WriteAttributeText(XmlConverter.Base64Encoding.GetString(this.trailBytes, 0, this.trailByteCount));
						this.WriteAttributeText(XmlConverter.Base64Encoding.GetString(buffer, offset, num3 - this.trailByteCount));
					}
					if (!this.isXmlnsAttribute)
					{
						this.StartContent();
						this.writer.WriteBase64Text(this.trailBytes, this.trailByteCount, buffer, offset, num3 - this.trailByteCount);
						this.EndContent();
					}
					this.trailByteCount = num2 - num3;
					if (this.trailByteCount > 0)
					{
						int num4 = offset + count - this.trailByteCount;
						for (int i = 0; i < this.trailByteCount; i++)
						{
							this.trailBytes[i] = buffer[num4++];
						}
						return;
					}
				}
				else
				{
					Buffer.BlockCopy(buffer, offset, this.trailBytes, this.trailByteCount, count);
					this.trailByteCount += count;
				}
			}
		}

		internal override IAsyncResult BeginWriteBase64(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			this.EnsureBufferBounds(buffer, offset, count);
			return new XmlBaseWriter.WriteBase64AsyncResult(buffer, offset, count, this, callback, state);
		}

		internal override void EndWriteBase64(IAsyncResult result)
		{
			XmlBaseWriter.WriteBase64AsyncResult.End(result);
		}

		internal override AsyncCompletionResult WriteBase64Async(AsyncEventArgs<XmlWriteBase64AsyncArguments> state)
		{
			if (this.nodeWriterAsyncHelper == null)
			{
				this.nodeWriterAsyncHelper = new XmlBaseWriter.XmlBaseWriterNodeWriterAsyncHelper(this);
			}
			this.nodeWriterAsyncHelper.SetArguments(state);
			if (this.nodeWriterAsyncHelper.StartAsync() == AsyncCompletionResult.Completed)
			{
				return AsyncCompletionResult.Completed;
			}
			return AsyncCompletionResult.Queued;
		}

		public override void WriteBinHex(byte[] buffer, int offset, int count)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			this.EnsureBufferBounds(buffer, offset, count);
			this.WriteRaw(XmlBaseWriter.BinHexEncoding.GetString(buffer, offset, count));
		}

		public override bool CanCanonicalize
		{
			get
			{
				return true;
			}
		}

		protected bool Signing
		{
			get
			{
				return this.writer == this.signingWriter;
			}
		}

		public override void StartCanonicalization(Stream stream, bool includeComments, string[] inclusivePrefixes)
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			if (this.Signing)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("XML canonicalization started")));
			}
			this.FlushElement();
			if (this.signingWriter == null)
			{
				this.signingWriter = this.CreateSigningNodeWriter();
			}
			this.signingWriter.SetOutput(this.writer, stream, includeComments, inclusivePrefixes);
			this.writer = this.signingWriter;
			this.SignScope(this.signingWriter.CanonicalWriter);
		}

		public override void EndCanonicalization()
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			if (!this.Signing)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("XML canonicalization was not started.")));
			}
			this.signingWriter.Flush();
			this.writer = this.signingWriter.NodeWriter;
		}

		protected abstract XmlSigningNodeWriter CreateSigningNodeWriter();

		public virtual bool CanFragment
		{
			get
			{
				return true;
			}
		}

		public void StartFragment(Stream stream, bool generateSelfContainedTextFragment)
		{
			if (!this.CanFragment)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
			}
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			if (stream == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("stream"));
			}
			if (this.oldStream != null || this.oldWriter != null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException());
			}
			if (this.WriteState == WriteState.Attribute)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("'{0}' cannot be called while WriteState is '{1}'.", new object[]
				{
					"StartFragment",
					this.WriteState.ToString()
				})));
			}
			this.FlushElement();
			this.writer.Flush();
			this.oldNamespaceBoundary = this.NamespaceBoundary;
			XmlStreamNodeWriter xmlStreamNodeWriter = null;
			if (generateSelfContainedTextFragment)
			{
				this.NamespaceBoundary = this.depth + 1;
				if (this.textFragmentWriter == null)
				{
					this.textFragmentWriter = new XmlUTF8NodeWriter();
				}
				this.textFragmentWriter.SetOutput(stream, false, Encoding.UTF8);
				xmlStreamNodeWriter = this.textFragmentWriter;
			}
			if (this.Signing)
			{
				if (xmlStreamNodeWriter != null)
				{
					this.oldWriter = this.signingWriter.NodeWriter;
					this.signingWriter.NodeWriter = xmlStreamNodeWriter;
					return;
				}
				this.oldStream = ((XmlStreamNodeWriter)this.signingWriter.NodeWriter).Stream;
				((XmlStreamNodeWriter)this.signingWriter.NodeWriter).Stream = stream;
				return;
			}
			else
			{
				if (xmlStreamNodeWriter != null)
				{
					this.oldWriter = this.writer;
					this.writer = xmlStreamNodeWriter;
					return;
				}
				this.oldStream = this.nodeWriter.Stream;
				this.nodeWriter.Stream = stream;
				return;
			}
		}

		public void EndFragment()
		{
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			if (this.oldStream == null && this.oldWriter == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException());
			}
			if (this.WriteState == WriteState.Attribute)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("'{0}' cannot be called while WriteState is '{1}'.", new object[]
				{
					"EndFragment",
					this.WriteState.ToString()
				})));
			}
			this.FlushElement();
			this.writer.Flush();
			if (this.Signing)
			{
				if (this.oldWriter != null)
				{
					this.signingWriter.NodeWriter = this.oldWriter;
				}
				else
				{
					((XmlStreamNodeWriter)this.signingWriter.NodeWriter).Stream = this.oldStream;
				}
			}
			else if (this.oldWriter != null)
			{
				this.writer = this.oldWriter;
			}
			else
			{
				this.nodeWriter.Stream = this.oldStream;
			}
			this.NamespaceBoundary = this.oldNamespaceBoundary;
			this.oldWriter = null;
			this.oldStream = null;
		}

		public void WriteFragment(byte[] buffer, int offset, int count)
		{
			if (!this.CanFragment)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
			}
			if (this.IsClosed)
			{
				this.ThrowClosed();
			}
			if (buffer == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("buffer"));
			}
			if (offset < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (count < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (count > buffer.Length - offset)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The specified size exceeds the remaining buffer space ({0} bytes).", new object[] { buffer.Length - offset })));
			}
			if (this.WriteState == WriteState.Attribute)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("'{0}' cannot be called while WriteState is '{1}'.", new object[]
				{
					"WriteFragment",
					this.WriteState.ToString()
				})));
			}
			if (this.writer != this.nodeWriter)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException());
			}
			this.FlushElement();
			this.FlushBase64();
			this.nodeWriter.Flush();
			this.nodeWriter.Stream.Write(buffer, offset, count);
		}

		private void FlushBase64()
		{
			if (this.trailByteCount > 0)
			{
				this.FlushTrailBytes();
			}
		}

		private void FlushTrailBytes()
		{
			if (this.attributeValue != null)
			{
				this.WriteAttributeText(XmlConverter.Base64Encoding.GetString(this.trailBytes, 0, this.trailByteCount));
			}
			if (!this.isXmlnsAttribute)
			{
				this.StartContent();
				this.writer.WriteBase64Text(this.trailBytes, this.trailByteCount, this.trailBytes, 0, 0);
				this.EndContent();
			}
			this.trailByteCount = 0;
		}

		private void WriteValue(object[] array)
		{
			this.FlushBase64();
			this.StartContent();
			this.writer.WriteStartListText();
			this.inList = true;
			for (int i = 0; i < array.Length; i++)
			{
				if (i != 0)
				{
					this.writer.WriteListSeparator();
				}
				this.WritePrimitiveValue(array[i]);
			}
			this.inList = false;
			this.writer.WriteEndListText();
			this.EndContent();
		}

		private void WriteValue(Array array)
		{
			this.FlushBase64();
			this.StartContent();
			this.writer.WriteStartListText();
			this.inList = true;
			for (int i = 0; i < array.Length; i++)
			{
				if (i != 0)
				{
					this.writer.WriteListSeparator();
				}
				this.WritePrimitiveValue(array.GetValue(i));
			}
			this.inList = false;
			this.writer.WriteEndListText();
			this.EndContent();
		}

		protected void StartArray(int count)
		{
			this.FlushBase64();
			if (this.documentState == XmlBaseWriter.DocumentState.Epilog)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("Only one root element is permitted per document.")));
			}
			if (this.documentState == XmlBaseWriter.DocumentState.Document && count > 1 && this.depth == 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("Only one root element is permitted per document.")));
			}
			if (this.writeState == WriteState.Attribute)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("'{0}' cannot be called while WriteState is '{1}'.", new object[]
				{
					"WriteStartElement",
					this.WriteState.ToString()
				})));
			}
			this.AutoComplete(WriteState.Content);
		}

		protected void EndArray()
		{
		}

		private void EnsureBufferBounds(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("buffer");
			}
			if (offset < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (count < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (count > buffer.Length - offset)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The specified size exceeds the remaining buffer space ({0} bytes).", new object[] { buffer.Length - offset })));
			}
		}

		private string GeneratePrefix(string ns, XmlDictionaryString xNs)
		{
			if (this.writeState != WriteState.Element && this.writeState != WriteState.Attribute)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("A prefix cannot be defined while WriteState is '{0}'.", new object[] { this.WriteState.ToString() })));
			}
			string text = this.nsMgr.AddNamespace(ns, xNs);
			if (text != null)
			{
				return text;
			}
			do
			{
				XmlBaseWriter.Element element = this.elements[this.depth];
				int prefixId = element.PrefixId;
				element.PrefixId = prefixId + 1;
				int num = prefixId;
				text = "d" + this.depth.ToString(CultureInfo.InvariantCulture) + "p" + num.ToString(CultureInfo.InvariantCulture);
			}
			while (this.nsMgr.LookupNamespace(text) != null);
			this.nsMgr.AddNamespace(text, ns, xNs);
			return text;
		}

		protected void SignScope(XmlCanonicalWriter signingWriter)
		{
			this.nsMgr.Sign(signingWriter);
		}

		private void WriteAttributeText(string value)
		{
			if (this.attributeValue.Length == 0)
			{
				this.attributeValue = value;
				return;
			}
			this.attributeValue += value;
		}

		private XmlNodeWriter writer;

		private XmlBaseWriter.NamespaceManager nsMgr;

		private XmlBaseWriter.Element[] elements;

		private int depth;

		private string attributeLocalName;

		private string attributeValue;

		private bool isXmlAttribute;

		private bool isXmlnsAttribute;

		private WriteState writeState;

		private XmlBaseWriter.DocumentState documentState;

		private byte[] trailBytes;

		private int trailByteCount;

		private XmlStreamNodeWriter nodeWriter;

		private XmlSigningNodeWriter signingWriter;

		private XmlUTF8NodeWriter textFragmentWriter;

		private XmlNodeWriter oldWriter;

		private Stream oldStream;

		private int oldNamespaceBoundary;

		private bool inList;

		private const string xmlnsNamespace = "http://www.w3.org/2000/xmlns/";

		private const string xmlNamespace = "http://www.w3.org/XML/1998/namespace";

		private static BinHexEncoding binhexEncoding;

		private static string[] prefixes = new string[]
		{
			"a", "b", "c", "d", "e", "f", "g", "h", "i", "j",
			"k", "l", "m", "n", "o", "p", "q", "r", "s", "t",
			"u", "v", "w", "x", "y", "z"
		};

		private XmlBaseWriter.XmlBaseWriterNodeWriterAsyncHelper nodeWriterAsyncHelper;

		private class WriteBase64AsyncResult : AsyncResult
		{
			public WriteBase64AsyncResult(byte[] buffer, int offset, int count, XmlBaseWriter writer, AsyncCallback callback, object state)
				: base(callback, state)
			{
				this.writer = writer;
				this.buffer = buffer;
				this.offset = offset;
				this.count = count;
				bool flag = true;
				if (this.count > 0)
				{
					if (writer.trailByteCount > 0)
					{
						while (writer.trailByteCount < 3 && this.count > 0)
						{
							byte[] trailBytes = writer.trailBytes;
							int trailByteCount = writer.trailByteCount;
							writer.trailByteCount = trailByteCount + 1;
							int num = trailByteCount;
							trailByteCount = this.offset;
							this.offset = trailByteCount + 1;
							trailBytes[num] = buffer[trailByteCount];
							this.count--;
						}
					}
					this.totalByteCount = writer.trailByteCount + this.count;
					this.actualByteCount = this.totalByteCount - this.totalByteCount % 3;
					if (writer.trailBytes == null)
					{
						writer.trailBytes = new byte[3];
					}
					if (this.actualByteCount >= 3)
					{
						if (writer.attributeValue != null)
						{
							writer.WriteAttributeText(XmlConverter.Base64Encoding.GetString(writer.trailBytes, 0, writer.trailByteCount));
							writer.WriteAttributeText(XmlConverter.Base64Encoding.GetString(buffer, this.offset, this.actualByteCount - writer.trailByteCount));
						}
						flag = this.HandleWriteBase64Text(null);
					}
					else
					{
						Buffer.BlockCopy(buffer, this.offset, writer.trailBytes, writer.trailByteCount, this.count);
						writer.trailByteCount += this.count;
					}
				}
				if (flag)
				{
					base.Complete(true);
				}
			}

			private static bool OnComplete(IAsyncResult result)
			{
				return ((XmlBaseWriter.WriteBase64AsyncResult)result.AsyncState).HandleWriteBase64Text(result);
			}

			private bool HandleWriteBase64Text(IAsyncResult result)
			{
				if (!this.writer.isXmlnsAttribute)
				{
					if (result == null)
					{
						this.writer.StartContent();
						result = this.writer.writer.BeginWriteBase64Text(this.writer.trailBytes, this.writer.trailByteCount, this.buffer, this.offset, this.actualByteCount - this.writer.trailByteCount, base.PrepareAsyncCompletion(XmlBaseWriter.WriteBase64AsyncResult.onComplete), this);
						if (!result.CompletedSynchronously)
						{
							return false;
						}
					}
					this.writer.writer.EndWriteBase64Text(result);
					this.writer.EndContent();
				}
				this.writer.trailByteCount = this.totalByteCount - this.actualByteCount;
				if (this.writer.trailByteCount > 0)
				{
					int num = this.offset + this.count - this.writer.trailByteCount;
					for (int i = 0; i < this.writer.trailByteCount; i++)
					{
						this.writer.trailBytes[i] = this.buffer[num++];
					}
				}
				return true;
			}

			public static void End(IAsyncResult result)
			{
				AsyncResult.End<XmlBaseWriter.WriteBase64AsyncResult>(result);
			}

			private static AsyncResult.AsyncCompletion onComplete = new AsyncResult.AsyncCompletion(XmlBaseWriter.WriteBase64AsyncResult.OnComplete);

			private XmlBaseWriter writer;

			private byte[] buffer;

			private int offset;

			private int count;

			private int actualByteCount;

			private int totalByteCount;
		}

		private class Element
		{
			public string Prefix
			{
				get
				{
					return this.prefix;
				}
				set
				{
					this.prefix = value;
				}
			}

			public string LocalName
			{
				get
				{
					return this.localName;
				}
				set
				{
					this.localName = value;
				}
			}

			public int PrefixId
			{
				get
				{
					return this.prefixId;
				}
				set
				{
					this.prefixId = value;
				}
			}

			public void Clear()
			{
				this.prefix = null;
				this.localName = null;
				this.prefixId = 0;
			}

			private string prefix;

			private string localName;

			private int prefixId;
		}

		private enum DocumentState : byte
		{
			None,
			Document,
			Epilog,
			End
		}

		private class NamespaceManager
		{
			public NamespaceManager()
			{
				this.defaultNamespace = new XmlBaseWriter.NamespaceManager.Namespace();
				this.defaultNamespace.Depth = 0;
				this.defaultNamespace.Prefix = string.Empty;
				this.defaultNamespace.Uri = string.Empty;
				this.defaultNamespace.UriDictionaryString = null;
			}

			public string XmlLang
			{
				get
				{
					return this.lang;
				}
			}

			public XmlSpace XmlSpace
			{
				get
				{
					return this.space;
				}
			}

			public void Clear()
			{
				if (this.namespaces == null)
				{
					this.namespaces = new XmlBaseWriter.NamespaceManager.Namespace[4];
					this.namespaces[0] = this.defaultNamespace;
				}
				this.nsCount = 1;
				this.nsTop = 0;
				this.depth = 0;
				this.attributeCount = 0;
				this.space = XmlSpace.None;
				this.lang = null;
				this.lastNameSpace = null;
				this.namespaceBoundary = 0;
			}

			public int NamespaceBoundary
			{
				get
				{
					return this.namespaceBoundary;
				}
				set
				{
					int num = 0;
					while (num < this.nsCount && this.namespaces[num].Depth < value)
					{
						num++;
					}
					this.nsTop = num;
					this.namespaceBoundary = value;
					this.lastNameSpace = null;
				}
			}

			public void Close()
			{
				if (this.depth == 0)
				{
					if (this.namespaces != null && this.namespaces.Length > 32)
					{
						this.namespaces = null;
					}
					if (this.attributes != null && this.attributes.Length > 4)
					{
						this.attributes = null;
					}
				}
				else
				{
					this.namespaces = null;
					this.attributes = null;
				}
				this.lang = null;
			}

			public void DeclareNamespaces(XmlNodeWriter writer)
			{
				for (int i = this.nsCount; i > 0; i--)
				{
					if (this.namespaces[i - 1].Depth != this.depth)
					{
						IL_0065:
						while (i < this.nsCount)
						{
							XmlBaseWriter.NamespaceManager.Namespace @namespace = this.namespaces[i];
							if (@namespace.UriDictionaryString != null)
							{
								writer.WriteXmlnsAttribute(@namespace.Prefix, @namespace.UriDictionaryString);
							}
							else
							{
								writer.WriteXmlnsAttribute(@namespace.Prefix, @namespace.Uri);
							}
							i++;
						}
						return;
					}
				}
				goto IL_0065;
			}

			public void EnterScope()
			{
				this.depth++;
			}

			public void ExitScope()
			{
				while (this.nsCount > 0)
				{
					XmlBaseWriter.NamespaceManager.Namespace @namespace = this.namespaces[this.nsCount - 1];
					if (@namespace.Depth != this.depth)
					{
						IL_0099:
						while (this.attributeCount > 0)
						{
							XmlBaseWriter.NamespaceManager.XmlAttribute xmlAttribute = this.attributes[this.attributeCount - 1];
							if (xmlAttribute.Depth != this.depth)
							{
								break;
							}
							this.space = xmlAttribute.XmlSpace;
							this.lang = xmlAttribute.XmlLang;
							xmlAttribute.Clear();
							this.attributeCount--;
						}
						this.depth--;
						return;
					}
					if (this.lastNameSpace == @namespace)
					{
						this.lastNameSpace = null;
					}
					@namespace.Clear();
					this.nsCount--;
				}
				goto IL_0099;
			}

			public void AddLangAttribute(string lang)
			{
				this.AddAttribute();
				this.lang = lang;
			}

			public void AddSpaceAttribute(XmlSpace space)
			{
				this.AddAttribute();
				this.space = space;
			}

			private void AddAttribute()
			{
				if (this.attributes == null)
				{
					this.attributes = new XmlBaseWriter.NamespaceManager.XmlAttribute[1];
				}
				else if (this.attributes.Length == this.attributeCount)
				{
					XmlBaseWriter.NamespaceManager.XmlAttribute[] array = new XmlBaseWriter.NamespaceManager.XmlAttribute[this.attributeCount * 2];
					Array.Copy(this.attributes, array, this.attributeCount);
					this.attributes = array;
				}
				XmlBaseWriter.NamespaceManager.XmlAttribute xmlAttribute = this.attributes[this.attributeCount];
				if (xmlAttribute == null)
				{
					xmlAttribute = new XmlBaseWriter.NamespaceManager.XmlAttribute();
					this.attributes[this.attributeCount] = xmlAttribute;
				}
				xmlAttribute.XmlLang = this.lang;
				xmlAttribute.XmlSpace = this.space;
				xmlAttribute.Depth = this.depth;
				this.attributeCount++;
			}

			public string AddNamespace(string uri, XmlDictionaryString uriDictionaryString)
			{
				if (uri.Length == 0)
				{
					this.AddNamespaceIfNotDeclared(string.Empty, uri, uriDictionaryString);
					return string.Empty;
				}
				for (int i = 0; i < XmlBaseWriter.prefixes.Length; i++)
				{
					string text = XmlBaseWriter.prefixes[i];
					bool flag = false;
					for (int j = this.nsCount - 1; j >= this.nsTop; j--)
					{
						if (this.namespaces[j].Prefix == text)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						this.AddNamespace(text, uri, uriDictionaryString);
						return text;
					}
				}
				return null;
			}

			public void AddNamespaceIfNotDeclared(string prefix, string uri, XmlDictionaryString uriDictionaryString)
			{
				if (this.LookupNamespace(prefix) != uri)
				{
					this.AddNamespace(prefix, uri, uriDictionaryString);
				}
			}

			public void AddNamespace(string prefix, string uri, XmlDictionaryString uriDictionaryString)
			{
				if (prefix.Length >= 3 && ((int)prefix[0] & -33) == 88 && ((int)prefix[1] & -33) == 77 && ((int)prefix[2] & -33) == 76)
				{
					if (prefix == "xml" && uri == "http://www.w3.org/XML/1998/namespace")
					{
						return;
					}
					if (prefix == "xmlns" && uri == "http://www.w3.org/2000/xmlns/")
					{
						return;
					}
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("Prefixes beginning with \"xml\" (regardless of casing) are reserved for use by XML."), "prefix"));
				}
				else
				{
					int i = this.nsCount - 1;
					XmlBaseWriter.NamespaceManager.Namespace @namespace;
					while (i >= 0)
					{
						@namespace = this.namespaces[i];
						if (@namespace.Depth != this.depth)
						{
							break;
						}
						if (@namespace.Prefix == prefix)
						{
							if (@namespace.Uri == uri)
							{
								return;
							}
							throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("The prefix '{0}' is bound to the namespace '{1}' and cannot be changed to '{2}'.", new object[] { prefix, @namespace.Uri, uri }), "prefix"));
						}
						else
						{
							i--;
						}
					}
					if (prefix.Length != 0 && uri.Length == 0)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("The empty namespace requires a null or empty prefix."), "prefix"));
					}
					if (uri.Length == "http://www.w3.org/2000/xmlns/".Length && uri == "http://www.w3.org/2000/xmlns/")
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("The namespace '{1}' can only be bound to the prefix '{0}'.", new object[] { "xmlns", uri })));
					}
					if (uri.Length == "http://www.w3.org/XML/1998/namespace".Length && uri[18] == 'X' && uri == "http://www.w3.org/XML/1998/namespace")
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("The namespace '{1}' can only be bound to the prefix '{0}'.", new object[] { "xml", uri })));
					}
					if (this.namespaces.Length == this.nsCount)
					{
						XmlBaseWriter.NamespaceManager.Namespace[] array = new XmlBaseWriter.NamespaceManager.Namespace[this.nsCount * 2];
						Array.Copy(this.namespaces, array, this.nsCount);
						this.namespaces = array;
					}
					@namespace = this.namespaces[this.nsCount];
					if (@namespace == null)
					{
						@namespace = new XmlBaseWriter.NamespaceManager.Namespace();
						this.namespaces[this.nsCount] = @namespace;
					}
					@namespace.Depth = this.depth;
					@namespace.Prefix = prefix;
					@namespace.Uri = uri;
					@namespace.UriDictionaryString = uriDictionaryString;
					this.nsCount++;
					this.lastNameSpace = null;
					return;
				}
			}

			public string LookupPrefix(string ns)
			{
				if (this.lastNameSpace != null && this.lastNameSpace.Uri == ns)
				{
					return this.lastNameSpace.Prefix;
				}
				int num = this.nsCount;
				for (int i = num - 1; i >= this.nsTop; i--)
				{
					XmlBaseWriter.NamespaceManager.Namespace @namespace = this.namespaces[i];
					if (@namespace.Uri == ns)
					{
						string prefix = @namespace.Prefix;
						bool flag = false;
						for (int j = i + 1; j < num; j++)
						{
							if (this.namespaces[j].Prefix == prefix)
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							this.lastNameSpace = @namespace;
							return prefix;
						}
					}
				}
				for (int k = num - 1; k >= this.nsTop; k--)
				{
					XmlBaseWriter.NamespaceManager.Namespace namespace2 = this.namespaces[k];
					if (namespace2.Uri == ns)
					{
						string prefix2 = namespace2.Prefix;
						bool flag2 = false;
						for (int l = k + 1; l < num; l++)
						{
							if (this.namespaces[l].Prefix == prefix2)
							{
								flag2 = true;
								break;
							}
						}
						if (!flag2)
						{
							this.lastNameSpace = namespace2;
							return prefix2;
						}
					}
				}
				if (ns.Length == 0)
				{
					bool flag3 = true;
					for (int m = num - 1; m >= this.nsTop; m--)
					{
						if (this.namespaces[m].Prefix.Length == 0)
						{
							flag3 = false;
							break;
						}
					}
					if (flag3)
					{
						return string.Empty;
					}
				}
				if (ns == "http://www.w3.org/2000/xmlns/")
				{
					return "xmlns";
				}
				if (ns == "http://www.w3.org/XML/1998/namespace")
				{
					return "xml";
				}
				return null;
			}

			public string LookupAttributePrefix(string ns)
			{
				if (this.lastNameSpace != null && this.lastNameSpace.Uri == ns && this.lastNameSpace.Prefix.Length != 0)
				{
					return this.lastNameSpace.Prefix;
				}
				int num = this.nsCount;
				for (int i = num - 1; i >= this.nsTop; i--)
				{
					XmlBaseWriter.NamespaceManager.Namespace @namespace = this.namespaces[i];
					if (@namespace.Uri == ns)
					{
						string prefix = @namespace.Prefix;
						if (prefix.Length != 0)
						{
							bool flag = false;
							for (int j = i + 1; j < num; j++)
							{
								if (this.namespaces[j].Prefix == prefix)
								{
									flag = true;
									break;
								}
							}
							if (!flag)
							{
								this.lastNameSpace = @namespace;
								return prefix;
							}
						}
					}
				}
				for (int k = num - 1; k >= this.nsTop; k--)
				{
					XmlBaseWriter.NamespaceManager.Namespace namespace2 = this.namespaces[k];
					if (namespace2.Uri == ns)
					{
						string prefix2 = namespace2.Prefix;
						if (prefix2.Length != 0)
						{
							bool flag2 = false;
							for (int l = k + 1; l < num; l++)
							{
								if (this.namespaces[l].Prefix == prefix2)
								{
									flag2 = true;
									break;
								}
							}
							if (!flag2)
							{
								this.lastNameSpace = namespace2;
								return prefix2;
							}
						}
					}
				}
				if (ns.Length == 0)
				{
					return string.Empty;
				}
				return null;
			}

			public string LookupNamespace(string prefix)
			{
				int num = this.nsCount;
				if (prefix.Length == 0)
				{
					for (int i = num - 1; i >= this.nsTop; i--)
					{
						XmlBaseWriter.NamespaceManager.Namespace @namespace = this.namespaces[i];
						if (@namespace.Prefix.Length == 0)
						{
							return @namespace.Uri;
						}
					}
					return string.Empty;
				}
				if (prefix.Length == 1)
				{
					char c = prefix[0];
					for (int j = num - 1; j >= this.nsTop; j--)
					{
						XmlBaseWriter.NamespaceManager.Namespace namespace2 = this.namespaces[j];
						if (namespace2.PrefixChar == c)
						{
							return namespace2.Uri;
						}
					}
					return null;
				}
				for (int k = num - 1; k >= this.nsTop; k--)
				{
					XmlBaseWriter.NamespaceManager.Namespace namespace3 = this.namespaces[k];
					if (namespace3.Prefix == prefix)
					{
						return namespace3.Uri;
					}
				}
				if (prefix == "xmlns")
				{
					return "http://www.w3.org/2000/xmlns/";
				}
				if (prefix == "xml")
				{
					return "http://www.w3.org/XML/1998/namespace";
				}
				return null;
			}

			public void Sign(XmlCanonicalWriter signingWriter)
			{
				int num = this.nsCount;
				for (int i = 1; i < num; i++)
				{
					XmlBaseWriter.NamespaceManager.Namespace @namespace = this.namespaces[i];
					bool flag = false;
					int num2 = i + 1;
					while (num2 < num && !flag)
					{
						flag = @namespace.Prefix == this.namespaces[num2].Prefix;
						num2++;
					}
					if (!flag)
					{
						signingWriter.WriteXmlnsAttribute(@namespace.Prefix, @namespace.Uri);
					}
				}
			}

			private XmlBaseWriter.NamespaceManager.Namespace[] namespaces;

			private XmlBaseWriter.NamespaceManager.Namespace lastNameSpace;

			private int nsCount;

			private int depth;

			private XmlBaseWriter.NamespaceManager.XmlAttribute[] attributes;

			private int attributeCount;

			private XmlSpace space;

			private string lang;

			private int namespaceBoundary;

			private int nsTop;

			private XmlBaseWriter.NamespaceManager.Namespace defaultNamespace;

			private class XmlAttribute
			{
				public int Depth
				{
					get
					{
						return this.depth;
					}
					set
					{
						this.depth = value;
					}
				}

				public string XmlLang
				{
					get
					{
						return this.lang;
					}
					set
					{
						this.lang = value;
					}
				}

				public XmlSpace XmlSpace
				{
					get
					{
						return this.space;
					}
					set
					{
						this.space = value;
					}
				}

				public void Clear()
				{
					this.lang = null;
				}

				private XmlSpace space;

				private string lang;

				private int depth;
			}

			private class Namespace
			{
				public void Clear()
				{
					this.prefix = null;
					this.prefixChar = '\0';
					this.ns = null;
					this.xNs = null;
					this.depth = 0;
				}

				public int Depth
				{
					get
					{
						return this.depth;
					}
					set
					{
						this.depth = value;
					}
				}

				public char PrefixChar
				{
					get
					{
						return this.prefixChar;
					}
				}

				public string Prefix
				{
					get
					{
						return this.prefix;
					}
					set
					{
						if (value.Length == 1)
						{
							this.prefixChar = value[0];
						}
						else
						{
							this.prefixChar = '\0';
						}
						this.prefix = value;
					}
				}

				public string Uri
				{
					get
					{
						return this.ns;
					}
					set
					{
						this.ns = value;
					}
				}

				public XmlDictionaryString UriDictionaryString
				{
					get
					{
						return this.xNs;
					}
					set
					{
						this.xNs = value;
					}
				}

				private string prefix;

				private string ns;

				private XmlDictionaryString xNs;

				private int depth;

				private char prefixChar;
			}
		}

		private class XmlBaseWriterNodeWriterAsyncHelper
		{
			public XmlBaseWriterNodeWriterAsyncHelper(XmlBaseWriter writer)
			{
				this.writer = writer;
			}

			public void SetArguments(AsyncEventArgs<XmlWriteBase64AsyncArguments> inputState)
			{
				this.inputState = inputState;
				this.buffer = inputState.Arguments.Buffer;
				this.offset = inputState.Arguments.Offset;
				this.count = inputState.Arguments.Count;
			}

			public AsyncCompletionResult StartAsync()
			{
				bool flag = true;
				if (this.count > 0)
				{
					if (this.writer.trailByteCount > 0)
					{
						while (this.writer.trailByteCount < 3 && this.count > 0)
						{
							byte[] trailBytes = this.writer.trailBytes;
							XmlBaseWriter xmlBaseWriter = this.writer;
							int trailByteCount = xmlBaseWriter.trailByteCount;
							xmlBaseWriter.trailByteCount = trailByteCount + 1;
							int num = trailByteCount;
							byte[] array = this.buffer;
							trailByteCount = this.offset;
							this.offset = trailByteCount + 1;
							trailBytes[num] = array[trailByteCount];
							this.count--;
						}
					}
					this.totalByteCount = this.writer.trailByteCount + this.count;
					this.actualByteCount = this.totalByteCount - this.totalByteCount % 3;
					if (this.writer.trailBytes == null)
					{
						this.writer.trailBytes = new byte[3];
					}
					if (this.actualByteCount >= 3)
					{
						if (this.writer.attributeValue != null)
						{
							this.writer.WriteAttributeText(XmlConverter.Base64Encoding.GetString(this.writer.trailBytes, 0, this.writer.trailByteCount));
							this.writer.WriteAttributeText(XmlConverter.Base64Encoding.GetString(this.buffer, this.offset, this.actualByteCount - this.writer.trailByteCount));
						}
						flag = this.HandleWriteBase64Text(false);
					}
					else
					{
						Buffer.BlockCopy(this.buffer, this.offset, this.writer.trailBytes, this.writer.trailByteCount, this.count);
						this.writer.trailByteCount += this.count;
					}
				}
				if (flag)
				{
					this.Clear();
					return AsyncCompletionResult.Completed;
				}
				return AsyncCompletionResult.Queued;
			}

			private static void OnWriteComplete(IAsyncEventArgs asyncEventArgs)
			{
				bool flag = false;
				Exception ex = null;
				XmlBaseWriter.XmlBaseWriterNodeWriterAsyncHelper xmlBaseWriterNodeWriterAsyncHelper = (XmlBaseWriter.XmlBaseWriterNodeWriterAsyncHelper)asyncEventArgs.AsyncState;
				AsyncEventArgs<XmlWriteBase64AsyncArguments> asyncEventArgs2 = xmlBaseWriterNodeWriterAsyncHelper.inputState;
				try
				{
					if (asyncEventArgs.Exception != null)
					{
						ex = asyncEventArgs.Exception;
						flag = true;
					}
					else
					{
						flag = xmlBaseWriterNodeWriterAsyncHelper.HandleWriteBase64Text(true);
					}
				}
				catch (Exception ex2)
				{
					if (Fx.IsFatal(ex2))
					{
						throw;
					}
					ex = ex2;
					flag = true;
				}
				if (flag)
				{
					xmlBaseWriterNodeWriterAsyncHelper.Clear();
					asyncEventArgs2.Complete(false, ex);
				}
			}

			private bool HandleWriteBase64Text(bool isAsyncCallback)
			{
				if (!this.writer.isXmlnsAttribute)
				{
					if (!isAsyncCallback)
					{
						if (this.nodeWriterAsyncState == null)
						{
							this.nodeWriterAsyncState = new AsyncEventArgs<XmlNodeWriterWriteBase64TextArgs>();
							this.nodeWriterArgs = new XmlNodeWriterWriteBase64TextArgs();
						}
						if (XmlBaseWriter.XmlBaseWriterNodeWriterAsyncHelper.onWriteComplete == null)
						{
							XmlBaseWriter.XmlBaseWriterNodeWriterAsyncHelper.onWriteComplete = new AsyncEventArgsCallback(XmlBaseWriter.XmlBaseWriterNodeWriterAsyncHelper.OnWriteComplete);
						}
						this.writer.StartContent();
						this.nodeWriterArgs.TrailBuffer = this.writer.trailBytes;
						this.nodeWriterArgs.TrailCount = this.writer.trailByteCount;
						this.nodeWriterArgs.Buffer = this.buffer;
						this.nodeWriterArgs.Offset = this.offset;
						this.nodeWriterArgs.Count = this.actualByteCount - this.writer.trailByteCount;
						this.nodeWriterAsyncState.Set(XmlBaseWriter.XmlBaseWriterNodeWriterAsyncHelper.onWriteComplete, this.nodeWriterArgs, this);
						if (this.writer.writer.WriteBase64TextAsync(this.nodeWriterAsyncState) != AsyncCompletionResult.Completed)
						{
							return false;
						}
						this.nodeWriterAsyncState.Complete(true);
					}
					this.writer.EndContent();
				}
				this.writer.trailByteCount = this.totalByteCount - this.actualByteCount;
				if (this.writer.trailByteCount > 0)
				{
					int num = this.offset + this.count - this.writer.trailByteCount;
					for (int i = 0; i < this.writer.trailByteCount; i++)
					{
						this.writer.trailBytes[i] = this.buffer[num++];
					}
				}
				return true;
			}

			private void Clear()
			{
				this.inputState = null;
				this.buffer = null;
				this.offset = 0;
				this.count = 0;
				this.actualByteCount = 0;
				this.totalByteCount = 0;
			}

			private static AsyncEventArgsCallback onWriteComplete;

			private XmlBaseWriter writer;

			private byte[] buffer;

			private int offset;

			private int count;

			private int actualByteCount;

			private int totalByteCount;

			private AsyncEventArgs<XmlNodeWriterWriteBase64TextArgs> nodeWriterAsyncState;

			private XmlNodeWriterWriteBase64TextArgs nodeWriterArgs;

			private AsyncEventArgs<XmlWriteBase64AsyncArguments> inputState;
		}
	}
}
