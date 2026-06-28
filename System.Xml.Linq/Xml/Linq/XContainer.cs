using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Xml.Linq
{
	public abstract class XContainer : XNode
	{
		internal XContainer()
		{
		}

		public XNode FirstNode
		{
			get
			{
				return this.first;
			}
			internal set
			{
				this.first = value;
			}
		}

		public XNode LastNode
		{
			get
			{
				return this.last;
			}
			internal set
			{
				this.last = value;
			}
		}

		private void CheckChildType(object o, bool addFirst)
		{
			if (o == null || o is string || o is XNode)
			{
				return;
			}
			if (o is IEnumerable)
			{
				foreach (object obj in ((IEnumerable)o))
				{
					this.CheckChildType(obj, addFirst);
				}
				return;
			}
			throw new ArgumentException("Invalid child type: " + o.GetType());
		}

		public void Add(object content)
		{
			if (content == null)
			{
				return;
			}
			foreach (object obj in XUtil.ExpandArray(content))
			{
				if (!this.OnAddingObject(obj, false, this.last, false))
				{
					this.AddNode(XUtil.ToNode(obj));
				}
			}
		}

		private void AddNode(XNode n)
		{
			this.CheckChildType(n, false);
			n = (XNode)XUtil.GetDetachedObject(n);
			n.SetOwner(this);
			if (this.first == null)
			{
				this.last = (this.first = n);
			}
			else
			{
				this.last.NextNode = n;
				n.PreviousNode = this.last;
				this.last = n;
			}
		}

		public void Add(params object[] content)
		{
			if (content == null)
			{
				return;
			}
			foreach (object obj in XUtil.ExpandArray(content))
			{
				this.Add(obj);
			}
		}

		public void AddFirst(object content)
		{
			if (this.first == null)
			{
				this.Add(content);
			}
			else
			{
				this.first.AddBeforeSelf(XUtil.ExpandArray(content));
			}
		}

		public void AddFirst(params object[] content)
		{
			if (content == null)
			{
				return;
			}
			if (this.first == null)
			{
				this.Add(content);
			}
			else
			{
				foreach (object obj in XUtil.ExpandArray(content))
				{
					if (!this.OnAddingObject(obj, false, this.first.PreviousNode, true))
					{
						this.first.AddBeforeSelf(obj);
					}
				}
			}
		}

		internal virtual bool OnAddingObject(object o, bool rejectAttribute, XNode refNode, bool addFirst)
		{
			return false;
		}

		public XmlWriter CreateWriter()
		{
			return new XNodeWriter(this);
		}

		public IEnumerable<XNode> Nodes()
		{
			XNode next;
			for (XNode i = this.FirstNode; i != null; i = next)
			{
				next = i.NextNode;
				yield return i;
			}
			yield break;
		}

		public IEnumerable<XNode> DescendantNodes()
		{
			foreach (XNode i in this.Nodes())
			{
				yield return i;
				XContainer c = i as XContainer;
				if (c != null)
				{
					foreach (XNode d in c.DescendantNodes())
					{
						yield return d;
					}
				}
			}
			yield break;
		}

		public IEnumerable<XElement> Descendants()
		{
			foreach (XNode i in this.DescendantNodes())
			{
				XElement el = i as XElement;
				if (el != null)
				{
					yield return el;
				}
			}
			yield break;
		}

		public IEnumerable<XElement> Descendants(XName name)
		{
			foreach (XElement el in this.Descendants())
			{
				if (el.Name == name)
				{
					yield return el;
				}
			}
			yield break;
		}

		public IEnumerable<XElement> Elements()
		{
			foreach (XNode i in this.Nodes())
			{
				XElement el = i as XElement;
				if (el != null)
				{
					yield return el;
				}
			}
			yield break;
		}

		public IEnumerable<XElement> Elements(XName name)
		{
			foreach (XElement el in this.Elements())
			{
				if (el.Name == name)
				{
					yield return el;
				}
			}
			yield break;
		}

		public XElement Element(XName name)
		{
			foreach (XElement xelement in this.Elements())
			{
				if (xelement.Name == name)
				{
					return xelement;
				}
			}
			return null;
		}

		internal void ReadContentFrom(XmlReader reader, LoadOptions options)
		{
			while (!reader.EOF)
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					break;
				}
				this.Add(XNode.ReadFrom(reader, options));
			}
		}

		public void RemoveNodes()
		{
			foreach (XNode xnode in this.Nodes())
			{
				xnode.Remove();
			}
		}

		public void ReplaceNodes(object content)
		{
			XNode firstNode = this.FirstNode;
			XNode lastNode = this.LastNode;
			this.Add(content);
			if (firstNode == null)
			{
				return;
			}
			XNode nextNode;
			for (XNode xnode = firstNode; xnode != lastNode; xnode = nextNode)
			{
				nextNode = xnode.NextNode;
				xnode.Remove();
			}
			lastNode.Remove();
		}

		public void ReplaceNodes(params object[] content)
		{
			this.ReplaceNodes(content);
		}

		private XNode first;

		private XNode last;
	}
}
