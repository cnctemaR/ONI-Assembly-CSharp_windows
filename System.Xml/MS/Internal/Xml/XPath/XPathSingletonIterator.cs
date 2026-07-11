using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal class XPathSingletonIterator : ResetableIterator
	{
		public XPathSingletonIterator(XPathNavigator nav)
		{
			this.nav = nav;
		}

		public XPathSingletonIterator(XPathNavigator nav, bool moved)
			: this(nav)
		{
			if (moved)
			{
				this.position = 1;
			}
		}

		public XPathSingletonIterator(XPathSingletonIterator it)
		{
			this.nav = it.nav.Clone();
			this.position = it.position;
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathSingletonIterator(this);
		}

		public override XPathNavigator Current
		{
			get
			{
				return this.nav;
			}
		}

		public override int CurrentPosition
		{
			get
			{
				return this.position;
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
			if (this.position == 0)
			{
				this.position = 1;
				return true;
			}
			return false;
		}

		public override void Reset()
		{
			this.position = 0;
		}

		private XPathNavigator nav;

		private int position;
	}
}
