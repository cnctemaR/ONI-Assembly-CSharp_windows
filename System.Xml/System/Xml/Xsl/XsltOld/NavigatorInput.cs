using System;
using System.Diagnostics;
using System.Xml.XPath;
using System.Xml.Xsl.Xslt;

namespace System.Xml.Xsl.XsltOld
{
	internal class NavigatorInput
	{
		internal NavigatorInput Next
		{
			get
			{
				return this._Next;
			}
			set
			{
				this._Next = value;
			}
		}

		internal string Href
		{
			get
			{
				return this._Href;
			}
		}

		internal KeywordsTable Atoms
		{
			get
			{
				return this._Atoms;
			}
		}

		internal XPathNavigator Navigator
		{
			get
			{
				return this._Navigator;
			}
		}

		internal InputScopeManager InputScopeManager
		{
			get
			{
				return this._Manager;
			}
		}

		internal bool Advance()
		{
			return this._Navigator.MoveToNext();
		}

		internal bool Recurse()
		{
			return this._Navigator.MoveToFirstChild();
		}

		internal bool ToParent()
		{
			return this._Navigator.MoveToParent();
		}

		internal void Close()
		{
			this._Navigator = null;
			this._PositionInfo = null;
		}

		internal int LineNumber
		{
			get
			{
				return this._PositionInfo.LineNumber;
			}
		}

		internal int LinePosition
		{
			get
			{
				return this._PositionInfo.LinePosition;
			}
		}

		internal XPathNodeType NodeType
		{
			get
			{
				return this._Navigator.NodeType;
			}
		}

		internal string Name
		{
			get
			{
				return this._Navigator.Name;
			}
		}

		internal string LocalName
		{
			get
			{
				return this._Navigator.LocalName;
			}
		}

		internal string NamespaceURI
		{
			get
			{
				return this._Navigator.NamespaceURI;
			}
		}

		internal string Prefix
		{
			get
			{
				return this._Navigator.Prefix;
			}
		}

		internal string Value
		{
			get
			{
				return this._Navigator.Value;
			}
		}

		internal bool IsEmptyTag
		{
			get
			{
				return this._Navigator.IsEmptyElement;
			}
		}

		internal string BaseURI
		{
			get
			{
				return this._Navigator.BaseURI;
			}
		}

		internal bool MoveToFirstAttribute()
		{
			return this._Navigator.MoveToFirstAttribute();
		}

		internal bool MoveToNextAttribute()
		{
			return this._Navigator.MoveToNextAttribute();
		}

		internal bool MoveToFirstNamespace()
		{
			return this._Navigator.MoveToFirstNamespace(XPathNamespaceScope.ExcludeXml);
		}

		internal bool MoveToNextNamespace()
		{
			return this._Navigator.MoveToNextNamespace(XPathNamespaceScope.ExcludeXml);
		}

		internal NavigatorInput(XPathNavigator navigator, string baseUri, InputScope rootScope)
		{
			if (navigator == null)
			{
				throw new ArgumentNullException("navigator");
			}
			if (baseUri == null)
			{
				throw new ArgumentNullException("baseUri");
			}
			this._Next = null;
			this._Href = baseUri;
			this._Atoms = new KeywordsTable(navigator.NameTable);
			this._Navigator = navigator;
			this._Manager = new InputScopeManager(this._Navigator, rootScope);
			this._PositionInfo = PositionInfo.GetPositionInfo(this._Navigator);
			if (this.NodeType == XPathNodeType.Root)
			{
				this._Navigator.MoveToFirstChild();
			}
		}

		internal NavigatorInput(XPathNavigator navigator)
			: this(navigator, navigator.BaseURI, null)
		{
		}

		[Conditional("DEBUG")]
		internal void AssertInput()
		{
		}

		private XPathNavigator _Navigator;

		private PositionInfo _PositionInfo;

		private InputScopeManager _Manager;

		private NavigatorInput _Next;

		private string _Href;

		private KeywordsTable _Atoms;
	}
}
