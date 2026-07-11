using System;
using System.Collections;

namespace System.Xml
{
	internal class XmlElementList : XmlNodeList
	{
		private XmlElementList(XmlNode parent)
		{
			this.rootNode = parent;
			this.curInd = -1;
			this.curElem = this.rootNode;
			this.changeCount = 0;
			this.empty = false;
			this.atomized = true;
			this.matchCount = -1;
			this.listener = new WeakReference(new XmlElementListListener(parent.Document, this));
		}

		~XmlElementList()
		{
			this.Dispose(false);
		}

		internal void ConcurrencyCheck(XmlNodeChangedEventArgs args)
		{
			if (!this.atomized)
			{
				XmlNameTable nameTable = this.rootNode.Document.NameTable;
				this.localName = nameTable.Add(this.localName);
				this.namespaceURI = nameTable.Add(this.namespaceURI);
				this.atomized = true;
			}
			if (this.IsMatch(args.Node))
			{
				this.changeCount++;
				this.curInd = -1;
				this.curElem = this.rootNode;
				if (args.Action == XmlNodeChangedAction.Insert)
				{
					this.empty = false;
				}
			}
			this.matchCount = -1;
		}

		internal XmlElementList(XmlNode parent, string name)
			: this(parent)
		{
			XmlNameTable nameTable = parent.Document.NameTable;
			this.asterisk = nameTable.Add("*");
			this.name = nameTable.Add(name);
			this.localName = null;
			this.namespaceURI = null;
		}

		internal XmlElementList(XmlNode parent, string localName, string namespaceURI)
			: this(parent)
		{
			XmlNameTable nameTable = parent.Document.NameTable;
			this.asterisk = nameTable.Add("*");
			this.localName = nameTable.Get(localName);
			this.namespaceURI = nameTable.Get(namespaceURI);
			if (this.localName == null || this.namespaceURI == null)
			{
				this.empty = true;
				this.atomized = false;
				this.localName = localName;
				this.namespaceURI = namespaceURI;
			}
			this.name = null;
		}

		internal int ChangeCount
		{
			get
			{
				return this.changeCount;
			}
		}

		private XmlNode NextElemInPreOrder(XmlNode curNode)
		{
			XmlNode xmlNode = curNode.FirstChild;
			if (xmlNode == null)
			{
				xmlNode = curNode;
				while (xmlNode != null && xmlNode != this.rootNode && xmlNode.NextSibling == null)
				{
					xmlNode = xmlNode.ParentNode;
				}
				if (xmlNode != null && xmlNode != this.rootNode)
				{
					xmlNode = xmlNode.NextSibling;
				}
			}
			if (xmlNode == this.rootNode)
			{
				xmlNode = null;
			}
			return xmlNode;
		}

		private XmlNode PrevElemInPreOrder(XmlNode curNode)
		{
			XmlNode xmlNode = curNode.PreviousSibling;
			while (xmlNode != null && xmlNode.LastChild != null)
			{
				xmlNode = xmlNode.LastChild;
			}
			if (xmlNode == null)
			{
				xmlNode = curNode.ParentNode;
			}
			if (xmlNode == this.rootNode)
			{
				xmlNode = null;
			}
			return xmlNode;
		}

		private bool IsMatch(XmlNode curNode)
		{
			if (curNode.NodeType == XmlNodeType.Element)
			{
				if (this.name != null)
				{
					if (Ref.Equal(this.name, this.asterisk) || Ref.Equal(curNode.Name, this.name))
					{
						return true;
					}
				}
				else if ((Ref.Equal(this.localName, this.asterisk) || Ref.Equal(curNode.LocalName, this.localName)) && (Ref.Equal(this.namespaceURI, this.asterisk) || curNode.NamespaceURI == this.namespaceURI))
				{
					return true;
				}
			}
			return false;
		}

		private XmlNode GetMatchingNode(XmlNode n, bool bNext)
		{
			XmlNode xmlNode = n;
			do
			{
				if (bNext)
				{
					xmlNode = this.NextElemInPreOrder(xmlNode);
				}
				else
				{
					xmlNode = this.PrevElemInPreOrder(xmlNode);
				}
			}
			while (xmlNode != null && !this.IsMatch(xmlNode));
			return xmlNode;
		}

		private XmlNode GetNthMatchingNode(XmlNode n, bool bNext, int nCount)
		{
			XmlNode xmlNode = n;
			for (int i = 0; i < nCount; i++)
			{
				xmlNode = this.GetMatchingNode(xmlNode, bNext);
				if (xmlNode == null)
				{
					return null;
				}
			}
			return xmlNode;
		}

		public XmlNode GetNextNode(XmlNode n)
		{
			if (this.empty)
			{
				return null;
			}
			XmlNode xmlNode = ((n == null) ? this.rootNode : n);
			return this.GetMatchingNode(xmlNode, true);
		}

		public override XmlNode Item(int index)
		{
			if (this.rootNode == null || index < 0)
			{
				return null;
			}
			if (this.empty)
			{
				return null;
			}
			if (this.curInd == index)
			{
				return this.curElem;
			}
			int num = index - this.curInd;
			bool flag = num > 0;
			if (num < 0)
			{
				num = -num;
			}
			XmlNode nthMatchingNode;
			if ((nthMatchingNode = this.GetNthMatchingNode(this.curElem, flag, num)) != null)
			{
				this.curInd = index;
				this.curElem = nthMatchingNode;
				return this.curElem;
			}
			return null;
		}

		public override int Count
		{
			get
			{
				if (this.empty)
				{
					return 0;
				}
				if (this.matchCount < 0)
				{
					int num = 0;
					int num2 = this.changeCount;
					XmlNode matchingNode = this.rootNode;
					while ((matchingNode = this.GetMatchingNode(matchingNode, true)) != null)
					{
						num++;
					}
					if (num2 != this.changeCount)
					{
						return num;
					}
					this.matchCount = num;
				}
				return this.matchCount;
			}
		}

		public override IEnumerator GetEnumerator()
		{
			if (this.empty)
			{
				return new XmlEmptyElementListEnumerator(this);
			}
			return new XmlElementListEnumerator(this);
		}

		protected override void PrivateDisposeNodeList()
		{
			GC.SuppressFinalize(this);
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.listener != null)
			{
				XmlElementListListener xmlElementListListener = (XmlElementListListener)this.listener.Target;
				if (xmlElementListListener != null)
				{
					xmlElementListListener.Unregister();
				}
				this.listener = null;
			}
		}

		private string asterisk;

		private int changeCount;

		private string name;

		private string localName;

		private string namespaceURI;

		private XmlNode rootNode;

		private int curInd;

		private XmlNode curElem;

		private bool empty;

		private bool atomized;

		private int matchCount;

		private WeakReference listener;
	}
}
