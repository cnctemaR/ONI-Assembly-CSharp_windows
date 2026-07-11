using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal class XPathSelectionIterator : ResetableIterator
	{
		internal XPathSelectionIterator(XPathNavigator nav, Query query)
		{
			this.nav = nav.Clone();
			this.query = query;
		}

		protected XPathSelectionIterator(XPathSelectionIterator it)
		{
			this.nav = it.nav.Clone();
			this.query = (Query)it.query.Clone();
			this.position = it.position;
		}

		public override void Reset()
		{
			this.query.Reset();
		}

		public override bool MoveNext()
		{
			XPathNavigator xpathNavigator = this.query.Advance();
			if (xpathNavigator != null)
			{
				this.position++;
				if (!this.nav.MoveTo(xpathNavigator))
				{
					this.nav = xpathNavigator.Clone();
				}
				return true;
			}
			return false;
		}

		public override int Count
		{
			get
			{
				return this.query.Count;
			}
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

		public override XPathNodeIterator Clone()
		{
			return new XPathSelectionIterator(this);
		}

		private XPathNavigator nav;

		private Query query;

		private int position;
	}
}
