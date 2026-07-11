using System;
using System.Text;
using System.Xml.XPath;

namespace MS.Internal.Xml.Cache
{
	internal sealed class XPathNodeInfoAtom
	{
		public XPathNodeInfoAtom(XPathNodePageInfo pageInfo)
		{
			this.pageInfo = pageInfo;
		}

		public XPathNodeInfoAtom(string localName, string namespaceUri, string prefix, string baseUri, XPathNode[] pageParent, XPathNode[] pageSibling, XPathNode[] pageSimilar, XPathDocument doc, int lineNumBase, int linePosBase)
		{
			this.Init(localName, namespaceUri, prefix, baseUri, pageParent, pageSibling, pageSimilar, doc, lineNumBase, linePosBase);
		}

		public void Init(string localName, string namespaceUri, string prefix, string baseUri, XPathNode[] pageParent, XPathNode[] pageSibling, XPathNode[] pageSimilar, XPathDocument doc, int lineNumBase, int linePosBase)
		{
			this.localName = localName;
			this.namespaceUri = namespaceUri;
			this.prefix = prefix;
			this.baseUri = baseUri;
			this.pageParent = pageParent;
			this.pageSibling = pageSibling;
			this.pageSimilar = pageSimilar;
			this.doc = doc;
			this.lineNumBase = lineNumBase;
			this.linePosBase = linePosBase;
			this.next = null;
			this.pageInfo = null;
			this.hashCode = 0;
			this.localNameHash = 0;
			for (int i = 0; i < this.localName.Length; i++)
			{
				this.localNameHash += (this.localNameHash << 7) ^ (int)this.localName[i];
			}
		}

		public XPathNodePageInfo PageInfo
		{
			get
			{
				return this.pageInfo;
			}
		}

		public string LocalName
		{
			get
			{
				return this.localName;
			}
		}

		public string NamespaceUri
		{
			get
			{
				return this.namespaceUri;
			}
		}

		public string Prefix
		{
			get
			{
				return this.prefix;
			}
		}

		public string BaseUri
		{
			get
			{
				return this.baseUri;
			}
		}

		public XPathNode[] SiblingPage
		{
			get
			{
				return this.pageSibling;
			}
		}

		public XPathNode[] SimilarElementPage
		{
			get
			{
				return this.pageSimilar;
			}
		}

		public XPathNode[] ParentPage
		{
			get
			{
				return this.pageParent;
			}
		}

		public XPathDocument Document
		{
			get
			{
				return this.doc;
			}
		}

		public int LineNumberBase
		{
			get
			{
				return this.lineNumBase;
			}
		}

		public int LinePositionBase
		{
			get
			{
				return this.linePosBase;
			}
		}

		public int LocalNameHashCode
		{
			get
			{
				return this.localNameHash;
			}
		}

		public XPathNodeInfoAtom Next
		{
			get
			{
				return this.next;
			}
			set
			{
				this.next = value;
			}
		}

		public override int GetHashCode()
		{
			if (this.hashCode == 0)
			{
				int num = this.localNameHash;
				if (this.pageSibling != null)
				{
					num += (num << 7) ^ this.pageSibling[0].PageInfo.PageNumber;
				}
				if (this.pageParent != null)
				{
					num += (num << 7) ^ this.pageParent[0].PageInfo.PageNumber;
				}
				if (this.pageSimilar != null)
				{
					num += (num << 7) ^ this.pageSimilar[0].PageInfo.PageNumber;
				}
				this.hashCode = ((num == 0) ? 1 : num);
			}
			return this.hashCode;
		}

		public override bool Equals(object other)
		{
			XPathNodeInfoAtom xpathNodeInfoAtom = other as XPathNodeInfoAtom;
			return this.GetHashCode() == xpathNodeInfoAtom.GetHashCode() && this.localName == xpathNodeInfoAtom.localName && this.pageSibling == xpathNodeInfoAtom.pageSibling && this.namespaceUri == xpathNodeInfoAtom.namespaceUri && this.pageParent == xpathNodeInfoAtom.pageParent && this.pageSimilar == xpathNodeInfoAtom.pageSimilar && this.prefix == xpathNodeInfoAtom.prefix && this.baseUri == xpathNodeInfoAtom.baseUri && this.lineNumBase == xpathNodeInfoAtom.lineNumBase && this.linePosBase == xpathNodeInfoAtom.linePosBase;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("hash=");
			stringBuilder.Append(this.GetHashCode());
			stringBuilder.Append(", ");
			if (this.localName.Length != 0)
			{
				stringBuilder.Append('{');
				stringBuilder.Append(this.namespaceUri);
				stringBuilder.Append('}');
				if (this.prefix.Length != 0)
				{
					stringBuilder.Append(this.prefix);
					stringBuilder.Append(':');
				}
				stringBuilder.Append(this.localName);
				stringBuilder.Append(", ");
			}
			if (this.pageParent != null)
			{
				stringBuilder.Append("parent=");
				stringBuilder.Append(this.pageParent[0].PageInfo.PageNumber);
				stringBuilder.Append(", ");
			}
			if (this.pageSibling != null)
			{
				stringBuilder.Append("sibling=");
				stringBuilder.Append(this.pageSibling[0].PageInfo.PageNumber);
				stringBuilder.Append(", ");
			}
			if (this.pageSimilar != null)
			{
				stringBuilder.Append("similar=");
				stringBuilder.Append(this.pageSimilar[0].PageInfo.PageNumber);
				stringBuilder.Append(", ");
			}
			stringBuilder.Append("lineNum=");
			stringBuilder.Append(this.lineNumBase);
			stringBuilder.Append(", ");
			stringBuilder.Append("linePos=");
			stringBuilder.Append(this.linePosBase);
			return stringBuilder.ToString();
		}

		private string localName;

		private string namespaceUri;

		private string prefix;

		private string baseUri;

		private XPathNode[] pageParent;

		private XPathNode[] pageSibling;

		private XPathNode[] pageSimilar;

		private XPathDocument doc;

		private int lineNumBase;

		private int linePosBase;

		private int hashCode;

		private int localNameHash;

		private XPathNodeInfoAtom next;

		private XPathNodePageInfo pageInfo;
	}
}
