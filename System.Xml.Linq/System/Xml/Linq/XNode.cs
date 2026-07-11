using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace System.Xml.Linq
{
	public abstract class XNode : XObject
	{
		internal XNode()
		{
		}

		public XNode NextNode
		{
			get
			{
				if (this.parent != null && this != this.parent.content)
				{
					return this.next;
				}
				return null;
			}
		}

		public XNode PreviousNode
		{
			get
			{
				if (this.parent == null)
				{
					return null;
				}
				XNode xnode = ((XNode)this.parent.content).next;
				XNode xnode2 = null;
				while (xnode != this)
				{
					xnode2 = xnode;
					xnode = xnode.next;
				}
				return xnode2;
			}
		}

		public static XNodeDocumentOrderComparer DocumentOrderComparer
		{
			get
			{
				if (XNode.documentOrderComparer == null)
				{
					XNode.documentOrderComparer = new XNodeDocumentOrderComparer();
				}
				return XNode.documentOrderComparer;
			}
		}

		public static XNodeEqualityComparer EqualityComparer
		{
			get
			{
				if (XNode.equalityComparer == null)
				{
					XNode.equalityComparer = new XNodeEqualityComparer();
				}
				return XNode.equalityComparer;
			}
		}

		public void AddAfterSelf(object content)
		{
			if (this.parent == null)
			{
				throw new InvalidOperationException(Res.GetString("InvalidOperation_MissingParent"));
			}
			new Inserter(this.parent, this).Add(content);
		}

		public void AddAfterSelf(params object[] content)
		{
			this.AddAfterSelf(content);
		}

		public void AddBeforeSelf(object content)
		{
			if (this.parent == null)
			{
				throw new InvalidOperationException(Res.GetString("InvalidOperation_MissingParent"));
			}
			XNode xnode = (XNode)this.parent.content;
			while (xnode.next != this)
			{
				xnode = xnode.next;
			}
			if (xnode == this.parent.content)
			{
				xnode = null;
			}
			new Inserter(this.parent, xnode).Add(content);
		}

		public void AddBeforeSelf(params object[] content)
		{
			this.AddBeforeSelf(content);
		}

		public IEnumerable<XElement> Ancestors()
		{
			return this.GetAncestors(null, false);
		}

		public IEnumerable<XElement> Ancestors(XName name)
		{
			if (!(name != null))
			{
				return XElement.EmptySequence;
			}
			return this.GetAncestors(name, false);
		}

		public static int CompareDocumentOrder(XNode n1, XNode n2)
		{
			if (n1 == n2)
			{
				return 0;
			}
			if (n1 == null)
			{
				return -1;
			}
			if (n2 == null)
			{
				return 1;
			}
			if (n1.parent != n2.parent)
			{
				int num = 0;
				XNode xnode = n1;
				while (xnode.parent != null)
				{
					xnode = xnode.parent;
					num++;
				}
				XNode xnode2 = n2;
				while (xnode2.parent != null)
				{
					xnode2 = xnode2.parent;
					num--;
				}
				if (xnode != xnode2)
				{
					throw new InvalidOperationException(Res.GetString("InvalidOperation_MissingAncestor"));
				}
				if (num < 0)
				{
					do
					{
						n2 = n2.parent;
						num++;
					}
					while (num != 0);
					if (n1 == n2)
					{
						return -1;
					}
				}
				else if (num > 0)
				{
					do
					{
						n1 = n1.parent;
						num--;
					}
					while (num != 0);
					if (n1 == n2)
					{
						return 1;
					}
				}
				while (n1.parent != n2.parent)
				{
					n1 = n1.parent;
					n2 = n2.parent;
				}
			}
			else if (n1.parent == null)
			{
				throw new InvalidOperationException(Res.GetString("InvalidOperation_MissingAncestor"));
			}
			XNode xnode3 = (XNode)n1.parent.content;
			for (;;)
			{
				xnode3 = xnode3.next;
				if (xnode3 == n1)
				{
					break;
				}
				if (xnode3 == n2)
				{
					return 1;
				}
			}
			return -1;
		}

		public XmlReader CreateReader()
		{
			return new XNodeReader(this, null);
		}

		public XmlReader CreateReader(ReaderOptions readerOptions)
		{
			return new XNodeReader(this, null, readerOptions);
		}

		public IEnumerable<XNode> NodesAfterSelf()
		{
			XNode i = this;
			while (i.parent != null && i != i.parent.content)
			{
				i = i.next;
				yield return i;
			}
			yield break;
		}

		public IEnumerable<XNode> NodesBeforeSelf()
		{
			if (this.parent != null)
			{
				XNode i = (XNode)this.parent.content;
				do
				{
					i = i.next;
					if (i == this)
					{
						break;
					}
					yield return i;
				}
				while (this.parent != null && this.parent == i.parent);
				i = null;
			}
			yield break;
		}

		public IEnumerable<XElement> ElementsAfterSelf()
		{
			return this.GetElementsAfterSelf(null);
		}

		public IEnumerable<XElement> ElementsAfterSelf(XName name)
		{
			if (!(name != null))
			{
				return XElement.EmptySequence;
			}
			return this.GetElementsAfterSelf(name);
		}

		public IEnumerable<XElement> ElementsBeforeSelf()
		{
			return this.GetElementsBeforeSelf(null);
		}

		public IEnumerable<XElement> ElementsBeforeSelf(XName name)
		{
			if (!(name != null))
			{
				return XElement.EmptySequence;
			}
			return this.GetElementsBeforeSelf(name);
		}

		public bool IsAfter(XNode node)
		{
			return XNode.CompareDocumentOrder(this, node) > 0;
		}

		public bool IsBefore(XNode node)
		{
			return XNode.CompareDocumentOrder(this, node) < 0;
		}

		public static XNode ReadFrom(XmlReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (reader.ReadState != ReadState.Interactive)
			{
				throw new InvalidOperationException(Res.GetString("InvalidOperation_ExpectedInteractive"));
			}
			switch (reader.NodeType)
			{
			case XmlNodeType.Element:
				return new XElement(reader);
			case XmlNodeType.Text:
			case XmlNodeType.Whitespace:
			case XmlNodeType.SignificantWhitespace:
				return new XText(reader);
			case XmlNodeType.CDATA:
				return new XCData(reader);
			case XmlNodeType.ProcessingInstruction:
				return new XProcessingInstruction(reader);
			case XmlNodeType.Comment:
				return new XComment(reader);
			case XmlNodeType.DocumentType:
				return new XDocumentType(reader);
			}
			throw new InvalidOperationException(Res.GetString("InvalidOperation_UnexpectedNodeType", new object[] { reader.NodeType }));
		}

		public void Remove()
		{
			if (this.parent == null)
			{
				throw new InvalidOperationException(Res.GetString("InvalidOperation_MissingParent"));
			}
			this.parent.RemoveNode(this);
		}

		public void ReplaceWith(object content)
		{
			if (this.parent == null)
			{
				throw new InvalidOperationException(Res.GetString("InvalidOperation_MissingParent"));
			}
			XContainer parent = this.parent;
			XNode xnode = (XNode)this.parent.content;
			while (xnode.next != this)
			{
				xnode = xnode.next;
			}
			if (xnode == this.parent.content)
			{
				xnode = null;
			}
			this.parent.RemoveNode(this);
			if (xnode != null && xnode.parent != parent)
			{
				throw new InvalidOperationException(Res.GetString("InvalidOperation_ExternalCode"));
			}
			new Inserter(parent, xnode).Add(content);
		}

		public void ReplaceWith(params object[] content)
		{
			this.ReplaceWith(content);
		}

		public override string ToString()
		{
			return this.GetXmlString(base.GetSaveOptionsFromAnnotations());
		}

		public string ToString(SaveOptions options)
		{
			return this.GetXmlString(options);
		}

		public static bool DeepEquals(XNode n1, XNode n2)
		{
			return n1 == n2 || (n1 != null && n2 != null && n1.DeepEquals(n2));
		}

		public abstract void WriteTo(XmlWriter writer);

		internal virtual void AppendText(StringBuilder sb)
		{
		}

		internal abstract XNode CloneNode();

		internal abstract bool DeepEquals(XNode node);

		internal IEnumerable<XElement> GetAncestors(XName name, bool self)
		{
			for (XElement e = (self ? this : this.parent) as XElement; e != null; e = e.parent as XElement)
			{
				if (name == null || e.name == name)
				{
					yield return e;
				}
			}
			yield break;
		}

		private IEnumerable<XElement> GetElementsAfterSelf(XName name)
		{
			XNode i = this;
			while (i.parent != null && i != i.parent.content)
			{
				i = i.next;
				XElement xelement = i as XElement;
				if (xelement != null && (name == null || xelement.name == name))
				{
					yield return xelement;
				}
			}
			yield break;
		}

		private IEnumerable<XElement> GetElementsBeforeSelf(XName name)
		{
			if (this.parent != null)
			{
				XNode i = (XNode)this.parent.content;
				do
				{
					i = i.next;
					if (i == this)
					{
						break;
					}
					XElement xelement = i as XElement;
					if (xelement != null && (name == null || xelement.name == name))
					{
						yield return xelement;
					}
				}
				while (this.parent != null && this.parent == i.parent);
				i = null;
			}
			yield break;
		}

		internal abstract int GetDeepHashCode();

		internal static XmlReaderSettings GetXmlReaderSettings(LoadOptions o)
		{
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			if ((o & LoadOptions.PreserveWhitespace) == LoadOptions.None)
			{
				xmlReaderSettings.IgnoreWhitespace = true;
			}
			xmlReaderSettings.DtdProcessing = DtdProcessing.Parse;
			xmlReaderSettings.MaxCharactersFromEntities = 10000000L;
			xmlReaderSettings.XmlResolver = null;
			return xmlReaderSettings;
		}

		internal static XmlWriterSettings GetXmlWriterSettings(SaveOptions o)
		{
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			if ((o & SaveOptions.DisableFormatting) == SaveOptions.None)
			{
				xmlWriterSettings.Indent = true;
			}
			if ((o & SaveOptions.OmitDuplicateNamespaces) != SaveOptions.None)
			{
				xmlWriterSettings.NamespaceHandling |= NamespaceHandling.OmitDuplicates;
			}
			return xmlWriterSettings;
		}

		private string GetXmlString(SaveOptions o)
		{
			string text;
			using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
			{
				XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
				xmlWriterSettings.OmitXmlDeclaration = true;
				if ((o & SaveOptions.DisableFormatting) == SaveOptions.None)
				{
					xmlWriterSettings.Indent = true;
				}
				if ((o & SaveOptions.OmitDuplicateNamespaces) != SaveOptions.None)
				{
					xmlWriterSettings.NamespaceHandling |= NamespaceHandling.OmitDuplicates;
				}
				if (this is XText)
				{
					xmlWriterSettings.ConformanceLevel = ConformanceLevel.Fragment;
				}
				using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, xmlWriterSettings))
				{
					XDocument xdocument = this as XDocument;
					if (xdocument != null)
					{
						xdocument.WriteContentTo(xmlWriter);
					}
					else
					{
						this.WriteTo(xmlWriter);
					}
				}
				text = stringWriter.ToString();
			}
			return text;
		}

		private static XNodeDocumentOrderComparer documentOrderComparer;

		private static XNodeEqualityComparer equalityComparer;

		internal XNode next;
	}
}
