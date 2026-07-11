using System;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	internal class XmlCachedSequenceWriter : XmlSequenceWriter
	{
		public XmlCachedSequenceWriter()
		{
			this.seqTyped = new XmlQueryItemSequence();
		}

		public XmlQueryItemSequence ResultSequence
		{
			get
			{
				return this.seqTyped;
			}
		}

		public override XmlRawWriter StartTree(XPathNodeType rootType, IXmlNamespaceResolver nsResolver, XmlNameTable nameTable)
		{
			this.doc = new XPathDocument(nameTable);
			this.writer = this.doc.LoadFromWriter(XPathDocument.LoadFlags.AtomizeNames | ((rootType == XPathNodeType.Root) ? XPathDocument.LoadFlags.None : XPathDocument.LoadFlags.Fragment), string.Empty);
			this.writer.NamespaceResolver = nsResolver;
			return this.writer;
		}

		public override void EndTree()
		{
			this.writer.Close();
			this.seqTyped.Add(this.doc.CreateNavigator());
		}

		public override void WriteItem(XPathItem item)
		{
			this.seqTyped.AddClone(item);
		}

		private XmlQueryItemSequence seqTyped;

		private XPathDocument doc;

		private XmlRawWriter writer;
	}
}
