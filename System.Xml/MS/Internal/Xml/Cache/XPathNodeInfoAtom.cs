using System;
using System.Text;
using System.Xml.XPath;

namespace MS.Internal.Xml.Cache
{
	internal sealed class XPathNodeInfoAtom : IEquatable<XPathNodeInfoAtom>
	{
		public XPathNodeInfoAtom(XPathNodePageInfo pageInfo)
		{
			this._pageInfo = pageInfo;
		}

		public XPathNodeInfoAtom(string localName, string namespaceUri, string prefix, string baseUri, XPathNode[] pageParent, XPathNode[] pageSibling, XPathNode[] pageSimilar, XPathDocument doc, int lineNumBase, int linePosBase)
		{
			this.Init(localName, namespaceUri, prefix, baseUri, pageParent, pageSibling, pageSimilar, doc, lineNumBase, linePosBase);
		}

		public void Init(string localName, string namespaceUri, string prefix, string baseUri, XPathNode[] pageParent, XPathNode[] pageSibling, XPathNode[] pageSimilar, XPathDocument doc, int lineNumBase, int linePosBase)
		{
			this._localName = localName;
			this._namespaceUri = namespaceUri;
			this._prefix = prefix;
			this._baseUri = baseUri;
			this._pageParent = pageParent;
			this._pageSibling = pageSibling;
			this._pageSimilar = pageSimilar;
			this._doc = doc;
			this._lineNumBase = lineNumBase;
			this._linePosBase = linePosBase;
			this._next = null;
			this._pageInfo = null;
			this._hashCode = 0;
			this._localNameHash = 0;
			for (int i = 0; i < this._localName.Length; i++)
			{
				this._localNameHash += (this._localNameHash << 7) ^ (int)this._localName[i];
			}
		}

		public XPathNodePageInfo PageInfo
		{
			get
			{
				return this._pageInfo;
			}
		}

		public string LocalName
		{
			get
			{
				return this._localName;
			}
		}

		public string NamespaceUri
		{
			get
			{
				return this._namespaceUri;
			}
		}

		public string Prefix
		{
			get
			{
				return this._prefix;
			}
		}

		public string BaseUri
		{
			get
			{
				return this._baseUri;
			}
		}

		public XPathNode[] SiblingPage
		{
			get
			{
				return this._pageSibling;
			}
		}

		public XPathNode[] SimilarElementPage
		{
			get
			{
				return this._pageSimilar;
			}
		}

		public XPathNode[] ParentPage
		{
			get
			{
				return this._pageParent;
			}
		}

		public XPathDocument Document
		{
			get
			{
				return this._doc;
			}
		}

		public int LineNumberBase
		{
			get
			{
				return this._lineNumBase;
			}
		}

		public int LinePositionBase
		{
			get
			{
				return this._linePosBase;
			}
		}

		public int LocalNameHashCode
		{
			get
			{
				return this._localNameHash;
			}
		}

		public XPathNodeInfoAtom Next
		{
			get
			{
				return this._next;
			}
			set
			{
				this._next = value;
			}
		}

		public override int GetHashCode()
		{
			if (this._hashCode == 0)
			{
				int num = this._localNameHash;
				if (this._pageSibling != null)
				{
					num += (num << 7) ^ this._pageSibling[0].PageInfo.PageNumber;
				}
				if (this._pageParent != null)
				{
					num += (num << 7) ^ this._pageParent[0].PageInfo.PageNumber;
				}
				if (this._pageSimilar != null)
				{
					num += (num << 7) ^ this._pageSimilar[0].PageInfo.PageNumber;
				}
				this._hashCode = ((num == 0) ? 1 : num);
			}
			return this._hashCode;
		}

		public override bool Equals(object other)
		{
			return this.Equals(other as XPathNodeInfoAtom);
		}

		public bool Equals(XPathNodeInfoAtom other)
		{
			return this.GetHashCode() == other.GetHashCode() && this._localName == other._localName && this._pageSibling == other._pageSibling && this._namespaceUri == other._namespaceUri && this._pageParent == other._pageParent && this._pageSimilar == other._pageSimilar && this._prefix == other._prefix && this._baseUri == other._baseUri && this._lineNumBase == other._lineNumBase && this._linePosBase == other._linePosBase;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("hash=");
			stringBuilder.Append(this.GetHashCode());
			stringBuilder.Append(", ");
			if (this._localName.Length != 0)
			{
				stringBuilder.Append('{');
				stringBuilder.Append(this._namespaceUri);
				stringBuilder.Append('}');
				if (this._prefix.Length != 0)
				{
					stringBuilder.Append(this._prefix);
					stringBuilder.Append(':');
				}
				stringBuilder.Append(this._localName);
				stringBuilder.Append(", ");
			}
			if (this._pageParent != null)
			{
				stringBuilder.Append("parent=");
				stringBuilder.Append(this._pageParent[0].PageInfo.PageNumber);
				stringBuilder.Append(", ");
			}
			if (this._pageSibling != null)
			{
				stringBuilder.Append("sibling=");
				stringBuilder.Append(this._pageSibling[0].PageInfo.PageNumber);
				stringBuilder.Append(", ");
			}
			if (this._pageSimilar != null)
			{
				stringBuilder.Append("similar=");
				stringBuilder.Append(this._pageSimilar[0].PageInfo.PageNumber);
				stringBuilder.Append(", ");
			}
			stringBuilder.Append("lineNum=");
			stringBuilder.Append(this._lineNumBase);
			stringBuilder.Append(", ");
			stringBuilder.Append("linePos=");
			stringBuilder.Append(this._linePosBase);
			return stringBuilder.ToString();
		}

		private string _localName;

		private string _namespaceUri;

		private string _prefix;

		private string _baseUri;

		private XPathNode[] _pageParent;

		private XPathNode[] _pageSibling;

		private XPathNode[] _pageSimilar;

		private XPathDocument _doc;

		private int _lineNumBase;

		private int _linePosBase;

		private int _hashCode;

		private int _localNameHash;

		private XPathNodeInfoAtom _next;

		private XPathNodePageInfo _pageInfo;
	}
}
