using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal class XPathSelectionIterator : ResetableIterator
	{
		internal XPathSelectionIterator(XPathNavigator nav, Query query)
		{
			this._nav = nav.Clone();
			this._query = query;
		}

		protected XPathSelectionIterator(XPathSelectionIterator it)
		{
			this._nav = it._nav.Clone();
			this._query = (Query)it._query.Clone();
			this._position = it._position;
		}

		public override void Reset()
		{
			this._query.Reset();
		}

		public override bool MoveNext()
		{
			XPathNavigator xpathNavigator = this._query.Advance();
			if (xpathNavigator != null)
			{
				this._position++;
				if (!this._nav.MoveTo(xpathNavigator))
				{
					this._nav = xpathNavigator.Clone();
				}
				return true;
			}
			return false;
		}

		public override int Count
		{
			get
			{
				return this._query.Count;
			}
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

		public override XPathNodeIterator Clone()
		{
			return new XPathSelectionIterator(this);
		}

		private XPathNavigator _nav;

		private Query _query;

		private int _position;
	}
}
