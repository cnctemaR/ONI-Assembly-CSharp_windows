using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal class XPathSingletonIterator : ResetableIterator
	{
		public XPathSingletonIterator(XPathNavigator nav)
		{
			this._nav = nav;
		}

		public XPathSingletonIterator(XPathNavigator nav, bool moved)
			: this(nav)
		{
			if (moved)
			{
				this._position = 1;
			}
		}

		public XPathSingletonIterator(XPathSingletonIterator it)
		{
			this._nav = it._nav.Clone();
			this._position = it._position;
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathSingletonIterator(this);
		}

		public override XPathNavigator Current
		{
			get
			{
				return this._nav;
			}
		}

		public override int CurrentPosition
		{
			get
			{
				return this._position;
			}
		}

		public override int Count
		{
			get
			{
				return 1;
			}
		}

		public override bool MoveNext()
		{
			if (this._position == 0)
			{
				this._position = 1;
				return true;
			}
			return false;
		}

		public override void Reset()
		{
			this._position = 0;
		}

		private XPathNavigator _nav;

		private int _position;
	}
}
