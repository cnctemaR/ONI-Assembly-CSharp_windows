using System;
using System.Text;
using System.Xml.XPath;

namespace MS.Internal.Xml.Cache
{
	internal sealed class XPathNodeInfoTable
	{
		public XPathNodeInfoTable()
		{
			this.hashTable = new XPathNodeInfoAtom[32];
			this.sizeTable = 0;
		}

		public XPathNodeInfoAtom Create(string localName, string namespaceUri, string prefix, string baseUri, XPathNode[] pageParent, XPathNode[] pageSibling, XPathNode[] pageSimilar, XPathDocument doc, int lineNumBase, int linePosBase)
		{
			XPathNodeInfoAtom xpathNodeInfoAtom;
			if (this.infoCached == null)
			{
				xpathNodeInfoAtom = new XPathNodeInfoAtom(localName, namespaceUri, prefix, baseUri, pageParent, pageSibling, pageSimilar, doc, lineNumBase, linePosBase);
			}
			else
			{
				xpathNodeInfoAtom = this.infoCached;
				this.infoCached = xpathNodeInfoAtom.Next;
				xpathNodeInfoAtom.Init(localName, namespaceUri, prefix, baseUri, pageParent, pageSibling, pageSimilar, doc, lineNumBase, linePosBase);
			}
			return this.Atomize(xpathNodeInfoAtom);
		}

		private XPathNodeInfoAtom Atomize(XPathNodeInfoAtom info)
		{
			for (XPathNodeInfoAtom xpathNodeInfoAtom = this.hashTable[info.GetHashCode() & (this.hashTable.Length - 1)]; xpathNodeInfoAtom != null; xpathNodeInfoAtom = xpathNodeInfoAtom.Next)
			{
				if (info.Equals(xpathNodeInfoAtom))
				{
					info.Next = this.infoCached;
					this.infoCached = info;
					return xpathNodeInfoAtom;
				}
			}
			if (this.sizeTable >= this.hashTable.Length)
			{
				XPathNodeInfoAtom[] array = this.hashTable;
				this.hashTable = new XPathNodeInfoAtom[array.Length * 2];
				foreach (XPathNodeInfoAtom xpathNodeInfoAtom in array)
				{
					while (xpathNodeInfoAtom != null)
					{
						XPathNodeInfoAtom next = xpathNodeInfoAtom.Next;
						this.AddInfo(xpathNodeInfoAtom);
						xpathNodeInfoAtom = next;
					}
				}
			}
			this.AddInfo(info);
			return info;
		}

		private void AddInfo(XPathNodeInfoAtom info)
		{
			int num = info.GetHashCode() & (this.hashTable.Length - 1);
			info.Next = this.hashTable[num];
			this.hashTable[num] = info;
			this.sizeTable++;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.hashTable.Length; i++)
			{
				stringBuilder.AppendFormat("{0,4}: ", i);
				for (XPathNodeInfoAtom xpathNodeInfoAtom = this.hashTable[i]; xpathNodeInfoAtom != null; xpathNodeInfoAtom = xpathNodeInfoAtom.Next)
				{
					if (xpathNodeInfoAtom != this.hashTable[i])
					{
						stringBuilder.Append("\n      ");
					}
					stringBuilder.Append(xpathNodeInfoAtom);
				}
				stringBuilder.Append('\n');
			}
			return stringBuilder.ToString();
		}

		private XPathNodeInfoAtom[] hashTable;

		private int sizeTable;

		private XPathNodeInfoAtom infoCached;

		private const int DefaultTableSize = 32;
	}
}
