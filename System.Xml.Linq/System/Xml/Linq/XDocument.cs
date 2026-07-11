using System;
using System.IO;
using System.Text;

namespace System.Xml.Linq
{
	public class XDocument : XContainer
	{
		public XDocument()
		{
		}

		public XDocument(params object[] content)
			: this()
		{
			base.AddContentSkipNotify(content);
		}

		public XDocument(XDeclaration declaration, params object[] content)
			: this(content)
		{
			this.declaration = declaration;
		}

		public XDocument(XDocument other)
			: base(other)
		{
			if (other.declaration != null)
			{
				this.declaration = new XDeclaration(other.declaration);
			}
		}

		public XDeclaration Declaration
		{
			get
			{
				return this.declaration;
			}
			set
			{
				this.declaration = value;
			}
		}

		public XDocumentType DocumentType
		{
			get
			{
				return this.GetFirstNode<XDocumentType>();
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.Document;
			}
		}

		public XElement Root
		{
			get
			{
				return this.GetFirstNode<XElement>();
			}
		}

		public static XDocument Load(string uri)
		{
			return XDocument.Load(uri, LoadOptions.None);
		}

		public static XDocument Load(string uri, LoadOptions options)
		{
			XmlReaderSettings xmlReaderSettings = XNode.GetXmlReaderSettings(options);
			XDocument xdocument;
			using (XmlReader xmlReader = XmlReader.Create(uri, xmlReaderSettings))
			{
				xdocument = XDocument.Load(xmlReader, options);
			}
			return xdocument;
		}

		public static XDocument Load(Stream stream)
		{
			return XDocument.Load(stream, LoadOptions.None);
		}

		public static XDocument Load(Stream stream, LoadOptions options)
		{
			XmlReaderSettings xmlReaderSettings = XNode.GetXmlReaderSettings(options);
			XDocument xdocument;
			using (XmlReader xmlReader = XmlReader.Create(stream, xmlReaderSettings))
			{
				xdocument = XDocument.Load(xmlReader, options);
			}
			return xdocument;
		}

		public static XDocument Load(TextReader textReader)
		{
			return XDocument.Load(textReader, LoadOptions.None);
		}

		public static XDocument Load(TextReader textReader, LoadOptions options)
		{
			XmlReaderSettings xmlReaderSettings = XNode.GetXmlReaderSettings(options);
			XDocument xdocument;
			using (XmlReader xmlReader = XmlReader.Create(textReader, xmlReaderSettings))
			{
				xdocument = XDocument.Load(xmlReader, options);
			}
			return xdocument;
		}

		public static XDocument Load(XmlReader reader)
		{
			return XDocument.Load(reader, LoadOptions.None);
		}

		public static XDocument Load(XmlReader reader, LoadOptions options)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (reader.ReadState == ReadState.Initial)
			{
				reader.Read();
			}
			XDocument xdocument = new XDocument();
			if ((options & LoadOptions.SetBaseUri) != LoadOptions.None)
			{
				string baseURI = reader.BaseURI;
				if (baseURI != null && baseURI.Length != 0)
				{
					xdocument.SetBaseUri(baseURI);
				}
			}
			if ((options & LoadOptions.SetLineInfo) != LoadOptions.None)
			{
				IXmlLineInfo xmlLineInfo = reader as IXmlLineInfo;
				if (xmlLineInfo != null && xmlLineInfo.HasLineInfo())
				{
					xdocument.SetLineInfo(xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
				}
			}
			if (reader.NodeType == XmlNodeType.XmlDeclaration)
			{
				xdocument.Declaration = new XDeclaration(reader);
			}
			xdocument.ReadContentFrom(reader, options);
			if (!reader.EOF)
			{
				throw new InvalidOperationException(Res.GetString("InvalidOperation_ExpectedEndOfFile"));
			}
			if (xdocument.Root == null)
			{
				throw new InvalidOperationException(Res.GetString("InvalidOperation_MissingRoot"));
			}
			return xdocument;
		}

		public static XDocument Parse(string text)
		{
			return XDocument.Parse(text, LoadOptions.None);
		}

		public static XDocument Parse(string text, LoadOptions options)
		{
			XDocument xdocument;
			using (StringReader stringReader = new StringReader(text))
			{
				XmlReaderSettings xmlReaderSettings = XNode.GetXmlReaderSettings(options);
				using (XmlReader xmlReader = XmlReader.Create(stringReader, xmlReaderSettings))
				{
					xdocument = XDocument.Load(xmlReader, options);
				}
			}
			return xdocument;
		}

		public void Save(string fileName)
		{
			this.Save(fileName, base.GetSaveOptionsFromAnnotations());
		}

		public void Save(string fileName, SaveOptions options)
		{
			XmlWriterSettings xmlWriterSettings = XNode.GetXmlWriterSettings(options);
			if (this.declaration != null && !string.IsNullOrEmpty(this.declaration.Encoding))
			{
				try
				{
					xmlWriterSettings.Encoding = Encoding.GetEncoding(this.declaration.Encoding);
				}
				catch (ArgumentException)
				{
				}
			}
			using (XmlWriter xmlWriter = XmlWriter.Create(fileName, xmlWriterSettings))
			{
				this.Save(xmlWriter);
			}
		}

		public void Save(Stream stream)
		{
			this.Save(stream, base.GetSaveOptionsFromAnnotations());
		}

		public void Save(Stream stream, SaveOptions options)
		{
			XmlWriterSettings xmlWriterSettings = XNode.GetXmlWriterSettings(options);
			if (this.declaration != null && !string.IsNullOrEmpty(this.declaration.Encoding))
			{
				try
				{
					xmlWriterSettings.Encoding = Encoding.GetEncoding(this.declaration.Encoding);
				}
				catch (ArgumentException)
				{
				}
			}
			using (XmlWriter xmlWriter = XmlWriter.Create(stream, xmlWriterSettings))
			{
				this.Save(xmlWriter);
			}
		}

		public void Save(TextWriter textWriter)
		{
			this.Save(textWriter, base.GetSaveOptionsFromAnnotations());
		}

		public void Save(TextWriter textWriter, SaveOptions options)
		{
			XmlWriterSettings xmlWriterSettings = XNode.GetXmlWriterSettings(options);
			using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, xmlWriterSettings))
			{
				this.Save(xmlWriter);
			}
		}

		public void Save(XmlWriter writer)
		{
			this.WriteTo(writer);
		}

		public override void WriteTo(XmlWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (this.declaration != null && this.declaration.Standalone == "yes")
			{
				writer.WriteStartDocument(true);
			}
			else if (this.declaration != null && this.declaration.Standalone == "no")
			{
				writer.WriteStartDocument(false);
			}
			else
			{
				writer.WriteStartDocument();
			}
			base.WriteContentTo(writer);
			writer.WriteEndDocument();
		}

		internal override void AddAttribute(XAttribute a)
		{
			throw new ArgumentException(Res.GetString("Argument_AddAttribute"));
		}

		internal override void AddAttributeSkipNotify(XAttribute a)
		{
			throw new ArgumentException(Res.GetString("Argument_AddAttribute"));
		}

		internal override XNode CloneNode()
		{
			return new XDocument(this);
		}

		internal override bool DeepEquals(XNode node)
		{
			XDocument xdocument = node as XDocument;
			return xdocument != null && base.ContentsEqual(xdocument);
		}

		internal override int GetDeepHashCode()
		{
			return base.ContentsHashCode();
		}

		private T GetFirstNode<T>() where T : XNode
		{
			XNode xnode = this.content as XNode;
			if (xnode != null)
			{
				T t;
				for (;;)
				{
					xnode = xnode.next;
					t = xnode as T;
					if (t != null)
					{
						break;
					}
					if (xnode == this.content)
					{
						goto IL_0035;
					}
				}
				return t;
			}
			IL_0035:
			return default(T);
		}

		internal static bool IsWhitespace(string s)
		{
			foreach (char c in s)
			{
				if (c != ' ' && c != '\t' && c != '\r' && c != '\n')
				{
					return false;
				}
			}
			return true;
		}

		internal override void ValidateNode(XNode node, XNode previous)
		{
			XmlNodeType nodeType = node.NodeType;
			switch (nodeType)
			{
			case XmlNodeType.Element:
				this.ValidateDocument(previous, XmlNodeType.DocumentType, XmlNodeType.None);
				return;
			case XmlNodeType.Attribute:
				return;
			case XmlNodeType.Text:
				this.ValidateString(((XText)node).Value);
				return;
			case XmlNodeType.CDATA:
				throw new ArgumentException(Res.GetString("Argument_AddNode", new object[] { XmlNodeType.CDATA }));
			default:
				if (nodeType == XmlNodeType.Document)
				{
					throw new ArgumentException(Res.GetString("Argument_AddNode", new object[] { XmlNodeType.Document }));
				}
				if (nodeType != XmlNodeType.DocumentType)
				{
					return;
				}
				this.ValidateDocument(previous, XmlNodeType.None, XmlNodeType.Element);
				return;
			}
		}

		private void ValidateDocument(XNode previous, XmlNodeType allowBefore, XmlNodeType allowAfter)
		{
			XNode xnode = this.content as XNode;
			if (xnode != null)
			{
				if (previous == null)
				{
					allowBefore = allowAfter;
				}
				for (;;)
				{
					xnode = xnode.next;
					XmlNodeType nodeType = xnode.NodeType;
					if (nodeType == XmlNodeType.Element || nodeType == XmlNodeType.DocumentType)
					{
						if (nodeType != allowBefore)
						{
							break;
						}
						allowBefore = XmlNodeType.None;
					}
					if (xnode == previous)
					{
						allowBefore = allowAfter;
					}
					if (xnode == this.content)
					{
						return;
					}
				}
				throw new InvalidOperationException(Res.GetString("InvalidOperation_DocumentStructure"));
			}
		}

		internal override void ValidateString(string s)
		{
			if (!XDocument.IsWhitespace(s))
			{
				throw new ArgumentException(Res.GetString("Argument_AddNonWhitespace"));
			}
		}

		private XDeclaration declaration;
	}
}
