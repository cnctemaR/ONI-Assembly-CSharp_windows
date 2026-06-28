using System;
using System.IO;

namespace System.Xml.Linq
{
	public class XDocument : XContainer
	{
		public XDocument()
		{
		}

		public XDocument(params object[] content)
		{
			base.Add(content);
		}

		public XDocument(XDeclaration xmldecl, params object[] content)
		{
			this.Declaration = xmldecl;
			base.Add(content);
		}

		public XDocument(XDocument other)
		{
			foreach (object obj in other.Nodes())
			{
				base.Add(XUtil.Clone(obj));
			}
		}

		public XDeclaration Declaration
		{
			get
			{
				return this.xmldecl;
			}
			set
			{
				this.xmldecl = value;
			}
		}

		public XDocumentType DocumentType
		{
			get
			{
				foreach (object obj in base.Nodes())
				{
					if (obj is XDocumentType)
					{
						return (XDocumentType)obj;
					}
				}
				return null;
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
				foreach (object obj in base.Nodes())
				{
					if (obj is XElement)
					{
						return (XElement)obj;
					}
				}
				return null;
			}
		}

		public static XDocument Load(string uri)
		{
			return XDocument.Load(uri, LoadOptions.None);
		}

		public static XDocument Load(string uri, LoadOptions options)
		{
			XDocument xdocument;
			using (XmlReader xmlReader = XmlReader.Create(uri, new XmlReaderSettings
			{
				IgnoreWhitespace = ((options & LoadOptions.PreserveWhitespace) == LoadOptions.None)
			}))
			{
				xdocument = XDocument.LoadCore(xmlReader, options);
			}
			return xdocument;
		}

		public static XDocument Load(Stream stream)
		{
			return XDocument.Load(new StreamReader(stream), LoadOptions.None);
		}

		public static XDocument Load(Stream stream, LoadOptions options)
		{
			return XDocument.Load(new StreamReader(stream), options);
		}

		public static XDocument Load(TextReader reader)
		{
			return XDocument.Load(reader, LoadOptions.None);
		}

		public static XDocument Load(TextReader reader, LoadOptions options)
		{
			XDocument xdocument;
			using (XmlReader xmlReader = XmlReader.Create(reader, new XmlReaderSettings
			{
				IgnoreWhitespace = ((options & LoadOptions.PreserveWhitespace) == LoadOptions.None)
			}))
			{
				xdocument = XDocument.LoadCore(xmlReader, options);
			}
			return xdocument;
		}

		public static XDocument Load(XmlReader reader)
		{
			return XDocument.Load(reader, LoadOptions.None);
		}

		public static XDocument Load(XmlReader reader, LoadOptions options)
		{
			XmlReaderSettings xmlReaderSettings = ((reader.Settings == null) ? new XmlReaderSettings() : reader.Settings.Clone());
			xmlReaderSettings.IgnoreWhitespace = (options & LoadOptions.PreserveWhitespace) == LoadOptions.None;
			XDocument xdocument;
			using (XmlReader xmlReader = XmlReader.Create(reader, xmlReaderSettings))
			{
				xdocument = XDocument.LoadCore(xmlReader, options);
			}
			return xdocument;
		}

		private static XDocument LoadCore(XmlReader reader, LoadOptions options)
		{
			XDocument xdocument = new XDocument();
			xdocument.ReadContent(reader, options);
			return xdocument;
		}

		private void ReadContent(XmlReader reader, LoadOptions options)
		{
			if (reader.ReadState == ReadState.Initial)
			{
				reader.Read();
			}
			if (reader.NodeType == XmlNodeType.XmlDeclaration)
			{
				this.Declaration = new XDeclaration(reader.GetAttribute("version"), reader.GetAttribute("encoding"), reader.GetAttribute("standalone"));
				reader.Read();
			}
			base.ReadContentFrom(reader, options);
			if (this.Root == null)
			{
				throw new InvalidOperationException("The document element is missing.");
			}
		}

		private static void ValidateWhitespace(string s)
		{
			foreach (char c in s)
			{
				switch (c)
				{
				case '\t':
				case '\n':
				case '\r':
					break;
				default:
					if (c != ' ')
					{
						throw new ArgumentException("Non-whitespace text appears directly in the document.");
					}
					break;
				}
			}
		}

		public static XDocument Parse(string s)
		{
			return XDocument.Parse(s, LoadOptions.None);
		}

		public static XDocument Parse(string s, LoadOptions options)
		{
			return XDocument.Load(new StringReader(s), options);
		}

		public void Save(string filename)
		{
			this.Save(filename, SaveOptions.None);
		}

		public void Save(string filename, SaveOptions options)
		{
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			if ((options & SaveOptions.DisableFormatting) == SaveOptions.None)
			{
				xmlWriterSettings.Indent = true;
			}
			using (XmlWriter xmlWriter = XmlWriter.Create(filename, xmlWriterSettings))
			{
				this.Save(xmlWriter);
			}
		}

		public void Save(TextWriter tw)
		{
			this.Save(tw, SaveOptions.None);
		}

		public void Save(TextWriter tw, SaveOptions options)
		{
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			if ((options & SaveOptions.DisableFormatting) == SaveOptions.None)
			{
				xmlWriterSettings.Indent = true;
			}
			using (XmlWriter xmlWriter = XmlWriter.Create(tw, xmlWriterSettings))
			{
				this.Save(xmlWriter);
			}
		}

		public void Save(XmlWriter w)
		{
			this.WriteTo(w);
		}

		public override void WriteTo(XmlWriter w)
		{
			if (this.xmldecl != null)
			{
				if (this.xmldecl.Standalone != null)
				{
					w.WriteStartDocument(this.xmldecl.Standalone == "yes");
				}
				else
				{
					w.WriteStartDocument();
				}
			}
			foreach (XNode xnode in base.Nodes())
			{
				xnode.WriteTo(w);
			}
		}

		internal override bool OnAddingObject(object obj, bool rejectAttribute, XNode refNode, bool addFirst)
		{
			this.VerifyAddedNode(obj, addFirst);
			return false;
		}

		private void VerifyAddedNode(object node, bool addFirst)
		{
			if (node == null)
			{
				throw new InvalidOperationException("Only a node is allowed here");
			}
			if (node is string)
			{
				XDocument.ValidateWhitespace((string)node);
			}
			if (node is XText)
			{
				XDocument.ValidateWhitespace(((XText)node).Value);
			}
			else if (node is XDocumentType)
			{
				if (this.DocumentType != null)
				{
					throw new InvalidOperationException("There already is another document type declaration");
				}
				if (this.Root != null && !addFirst)
				{
					throw new InvalidOperationException("A document type cannot be added after the document element");
				}
			}
			else if (node is XElement)
			{
				if (this.Root != null)
				{
					throw new InvalidOperationException("There already is another document element");
				}
				if (this.DocumentType != null && addFirst)
				{
					throw new InvalidOperationException("An element cannot be added before the document type declaration");
				}
			}
		}

		private XDeclaration xmldecl;
	}
}
