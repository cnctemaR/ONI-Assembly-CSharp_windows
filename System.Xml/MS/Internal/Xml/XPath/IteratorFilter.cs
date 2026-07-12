using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal class IteratorFilter : XPathNodeIterator
	{
		internal IteratorFilter(XPathNodeIterator innerIterator, string name)
		{
			this._innerIterator = innerIterator;
			this._name = name;
		}

		private IteratorFilter(IteratorFilter it)
		{
			this._innerIterator = it._innerIterator.Clone();
			this._name = it._name;
			this._position = it._position;
		}

		public override XPathNodeIterator Clone()
		{
			return new IteratorFilter(this);
		}

		public override XPathNavigator Current
		{
			get
			{
				return this._innerIterator.Current;
			}
		}

		public override int CurrentPosition
		{
			get
			{
				return this._position;
			}
		}

		public override bool MoveNext()
		{
			while (this._innerIterator.MoveNext())
			{
				if (this._innerIterator.Current.LocalName == this._name)
				{
					this._position++;
					return true;
				}
			}
			return false;
		}

		private XPathNodeIterator _innerIterator;

		private string _name;

		private int _position;
	}
}
