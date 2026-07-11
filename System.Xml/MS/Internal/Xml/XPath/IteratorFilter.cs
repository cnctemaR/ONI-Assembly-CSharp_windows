using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal class IteratorFilter : XPathNodeIterator
	{
		internal IteratorFilter(XPathNodeIterator innerIterator, string name)
		{
			this.innerIterator = innerIterator;
			this.name = name;
		}

		private IteratorFilter(IteratorFilter it)
		{
			this.innerIterator = it.innerIterator.Clone();
			this.name = it.name;
			this.position = it.position;
		}

		public override XPathNodeIterator Clone()
		{
			return new IteratorFilter(this);
		}

		public override XPathNavigator Current
		{
			get
			{
				return this.innerIterator.Current;
			}
		}

		public override int CurrentPosition
		{
			get
			{
				return this.position;
			}
		}

		public override bool MoveNext()
		{
			while (this.innerIterator.MoveNext())
			{
				if (this.innerIterator.Current.LocalName == this.name)
				{
					this.position++;
					return true;
				}
			}
			return false;
		}

		private XPathNodeIterator innerIterator;

		private string name;

		private int position;
	}
}
