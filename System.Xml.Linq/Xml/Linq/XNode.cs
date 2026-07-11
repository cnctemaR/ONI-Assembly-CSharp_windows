using System;
using System.Collections.Generic;
using System.IO;

namespace System.Xml.Linq
{
	public abstract class XNode : XObject
	{
		internal XNode()
		{
		}

		public static int CompareDocumentOrder(XNode n1, XNode n2)
		{
			return XNode.order_comparer.Compare(n1, n2);
		}

		public static bool DeepEquals(XNode n1, XNode n2)
		{
			return XNode.eq_comparer.Equals(n1, n2);
		}

		public static XNodeDocumentOrderComparer DocumentOrderComparer
		{
			get
			{
				return XNode.order_comparer;
			}
		}

		public static XNodeEqualityComparer EqualityComparer
		{
			get
			{
				return XNode.eq_comparer;
			}
		}

		public XNode PreviousNode
		{
			get
			{
				return this.previous;
			}
			internal set
			{
				this.previous = value;
			}
		}

		public XNode NextNode
		{
			get
			{
				return this.next;
			}
			internal set
			{
				this.next = value;
			}
		}

		public string ToString(SaveOptions options)
		{
			StringWriter stringWriter = new StringWriter();
			XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
			{
				ConformanceLevel = ConformanceLevel.Auto,
				Indent = (options != SaveOptions.DisableFormatting)
			});
			this.WriteTo(xmlWriter);
			xmlWriter.Close();
			return stringWriter.ToString();
		}

		public void AddAfterSelf(object content)
		{
			if (base.Parent == null)
			{
				throw new InvalidOperationException();
			}
			XNode xnode = this;
			XNode xnode2 = this.next;
			foreach (object obj in XUtil.ExpandArray(content))
			{
				if (!base.Owner.OnAddingObject(obj, true, xnode, false))
				{
					XNode xnode3 = XUtil.ToNode(obj);
					xnode3 = (XNode)XUtil.GetDetachedObject(xnode3);
					xnode3.SetOwner(base.Parent);
					xnode3.previous = xnode;
					xnode.next = xnode3;
					xnode3.next = xnode2;
					if (xnode2 != null)
					{
						xnode2.previous = xnode3;
					}
					else
					{
						base.Parent.LastNode = xnode3;
					}
					xnode = xnode3;
				}
			}
		}

		public void AddAfterSelf(params object[] content)
		{
			if (base.Parent == null)
			{
				throw new InvalidOperationException();
			}
			this.AddAfterSelf(content);
		}

		public void AddBeforeSelf(object content)
		{
			if (base.Parent == null)
			{
				throw new InvalidOperationException();
			}
			foreach (object obj in XUtil.ExpandArray(content))
			{
				if (!base.Owner.OnAddingObject(obj, true, this.previous, true))
				{
					XNode xnode = XUtil.ToNode(obj);
					xnode = (XNode)XUtil.GetDetachedObject(xnode);
					xnode.SetOwner(base.Parent);
					xnode.previous = this.previous;
					xnode.next = this;
					if (this.previous != null)
					{
						this.previous.next = xnode;
					}
					this.previous = xnode;
					if (base.Parent.FirstNode == this)
					{
						base.Parent.FirstNode = xnode;
					}
				}
			}
		}

		public void AddBeforeSelf(params object[] content)
		{
			if (base.Parent == null)
			{
				throw new InvalidOperationException();
			}
			this.AddBeforeSelf(content);
		}

		public static XNode ReadFrom(XmlReader r)
		{
			return XNode.ReadFrom(r, LoadOptions.None);
		}

		internal static XNode ReadFrom(XmlReader r, LoadOptions options)
		{
			switch (r.NodeType)
			{
			case XmlNodeType.Element:
				return XElement.LoadCore(r, options);
			case XmlNodeType.Text:
			case XmlNodeType.Whitespace:
			case XmlNodeType.SignificantWhitespace:
			{
				XText xtext = new XText(r.Value);
				xtext.FillLineInfoAndBaseUri(r, options);
				r.Read();
				return xtext;
			}
			case XmlNodeType.CDATA:
			{
				XCData xcdata = new XCData(r.Value);
				xcdata.FillLineInfoAndBaseUri(r, options);
				r.Read();
				return xcdata;
			}
			case XmlNodeType.ProcessingInstruction:
			{
				XProcessingInstruction xprocessingInstruction = new XProcessingInstruction(r.Name, r.Value);
				xprocessingInstruction.FillLineInfoAndBaseUri(r, options);
				r.Read();
				return xprocessingInstruction;
			}
			case XmlNodeType.Comment:
			{
				XComment xcomment = new XComment(r.Value);
				xcomment.FillLineInfoAndBaseUri(r, options);
				r.Read();
				return xcomment;
			}
			case XmlNodeType.DocumentType:
			{
				XDocumentType xdocumentType = new XDocumentType(r.Name, r.GetAttribute("PUBLIC"), r.GetAttribute("SYSTEM"), r.Value);
				xdocumentType.FillLineInfoAndBaseUri(r, options);
				r.Read();
				return xdocumentType;
			}
			}
			throw new InvalidOperationException(string.Format("Node type {0} is not supported", r.NodeType));
		}

		public void Remove()
		{
			if (base.Parent == null)
			{
				throw new InvalidOperationException("Parent is missing");
			}
			if (base.Parent.FirstNode == this)
			{
				base.Parent.FirstNode = this.next;
			}
			if (base.Parent.LastNode == this)
			{
				base.Parent.LastNode = this.previous;
			}
			if (this.previous != null)
			{
				this.previous.next = this.next;
			}
			if (this.next != null)
			{
				this.next.previous = this.previous;
			}
			this.previous = null;
			this.next = null;
			base.SetOwner(null);
		}

		public override string ToString()
		{
			return this.ToString(SaveOptions.None);
		}

		public abstract void WriteTo(XmlWriter w);

		public IEnumerable<XElement> Ancestors()
		{
			for (XElement el = base.Parent; el != null; el = el.Parent)
			{
				yield return el;
			}
			yield break;
		}

		public IEnumerable<XElement> Ancestors(XName name)
		{
			foreach (XElement el in this.Ancestors())
			{
				if (el.Name == name)
				{
					yield return el;
				}
			}
			yield break;
		}

		public XmlReader CreateReader()
		{
			return new XNodeReader(this);
		}

		public IEnumerable<XElement> ElementsAfterSelf()
		{
			foreach (XNode i in this.NodesAfterSelf())
			{
				if (i is XElement)
				{
					yield return (XElement)i;
				}
			}
			yield break;
		}

		public IEnumerable<XElement> ElementsAfterSelf(XName name)
		{
			foreach (XElement el in this.ElementsAfterSelf())
			{
				if (el.Name == name)
				{
					yield return el;
				}
			}
			yield break;
		}

		public IEnumerable<XElement> ElementsBeforeSelf()
		{
			foreach (XNode i in this.NodesBeforeSelf())
			{
				if (i is XElement)
				{
					yield return (XElement)i;
				}
			}
			yield break;
		}

		public IEnumerable<XElement> ElementsBeforeSelf(XName name)
		{
			foreach (XElement el in this.ElementsBeforeSelf())
			{
				if (el.Name == name)
				{
					yield return el;
				}
			}
			yield break;
		}

		public bool IsAfter(XNode other)
		{
			return XNode.DocumentOrderComparer.Compare(this, other) > 0;
		}

		public bool IsBefore(XNode other)
		{
			return XNode.DocumentOrderComparer.Compare(this, other) < 0;
		}

		public IEnumerable<XNode> NodesAfterSelf()
		{
			if (base.Parent == null)
			{
				yield break;
			}
			for (XNode i = this.NextNode; i != null; i = i.NextNode)
			{
				yield return i;
			}
			yield break;
		}

		public IEnumerable<XNode> NodesBeforeSelf()
		{
			for (XNode i = base.Parent.FirstNode; i != this; i = i.NextNode)
			{
				yield return i;
			}
			yield break;
		}

		public void ReplaceWith(object item)
		{
			this.AddAfterSelf(item);
			this.Remove();
		}

		public void ReplaceWith(params object[] items)
		{
			this.AddAfterSelf(items);
			this.Remove();
		}

		private static XNodeEqualityComparer eq_comparer = new XNodeEqualityComparer();

		private static XNodeDocumentOrderComparer order_comparer = new XNodeDocumentOrderComparer();

		private XNode previous;

		private XNode next;
	}
}
