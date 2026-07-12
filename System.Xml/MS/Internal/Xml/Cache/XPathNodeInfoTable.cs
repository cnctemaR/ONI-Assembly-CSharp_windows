using System;
using System.Text;
using System.Xml.XPath;

namespace MS.Internal.Xml.Cache
{
	internal sealed class XPathNodeInfoTable
	{
		public XPathNodeInfoTable()
		{
			this._hashTable = new XPathNodeInfoAtom[32];
			this._sizeTable = 0;
		}

		public XPathNodeInfoAtom Create(string localName, string namespaceUri, string prefix, string baseUri, XPathNode[] pageParent, XPathNode[] pageSibling, XPathNode[] pageSimilar, XPathDocument doc, int lineNumBase, int linePosBase)
		{
			XPathNodeInfoAtom xpathNodeInfoAtom;
			if (this._infoCached == null)
			{
				xpathNodeInfoAtom = new XPathNodeInfoAtom(localName, namespaceUri, prefix, baseUri, pageParent, pageSibling, pageSimilar, doc, lineNumBase, linePosBase);
			}
			else
			{
				xpathNodeInfoAtom = this._infoCached;
				this._infoCached = xpathNodeInfoAtom.Next;
				xpathNodeInfoAtom.Init(localName, namespaceUri, prefix, baseUri, pageParent, pageSibling, pageSimilar, doc, lineNumBase, linePosBase);
			}
			return this.Atomize(xpathNodeInfoAtom);
		}

		private XPathNodeInfoAtom Atomize(XPathNodeInfoAtom info)
		{
			for (XPathNodeInfoAtom xpathNodeInfoAtom = this._hashTable[info.GetHashCode() & (this._hashTable.Length - 1)]; xpathNodeInfoAtom != null; xpathNodeInfoAtom = xpathNodeInfoAtom.Next)
			{
				if (info.Equals(xpathNodeInfoAtom))
				{
					info.Next = this._infoCached;
					this._infoCached = info;
					return xpathNodeInfoAtom;
				}
			}
			if (this._sizeTable >= this._hashTable.Length)
			{
				XPathNodeInfoAtom[] hashTable = this._hashTable;
				this._hashTable = new XPathNodeInfoAtom[hashTable.Length * 2];
				foreach (XPathNodeInfoAtom xpathNodeInfoAtom in hashTable)
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
			int num = info.GetHashCode() & (this._hashTable.Length - 1);
			info.Next = this._hashTable[num];
			this._hashTable[num] = info;
			this._sizeTable++;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this._hashTable.Length; i++)
			{
				stringBuilder.AppendFormat("{0,4}: ", i);
				for (XPathNodeInfoAtom xpathNodeInfoAtom = this._hashTable[i]; xpathNodeInfoAtom != null; xpathNodeInfoAtom = xpathNodeInfoAtom.Next)
				{
					if (xpathNodeInfoAtom != this._hashTable[i])
					{
						stringBuilder.Append("\n      ");
					}
					stringBuilder.Append(xpathNodeInfoAtom);
				}
				stringBuilder.Append('\n');
			}
			return stringBuilder.ToString();
		}

		private XPathNodeInfoAtom[] _hashTable;

		private int _sizeTable;

		private XPathNodeInfoAtom _infoCached;

		private const int DefaultTableSize = 32;
	}
}
