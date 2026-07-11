using System;
using System.Collections;

namespace System.Xml.XPath
{
	internal class OrderedIterator : BaseIterator
	{
		public OrderedIterator(BaseIterator iter)
			: base(iter.NamespaceManager)
		{
			this.list = new ArrayList();
			while (iter.MoveNext())
			{
				XPathNavigator xpathNavigator = iter.Current;
				this.list.Add(xpathNavigator);
			}
			this.list.Sort(XPathNavigatorComparer.Instance);
		}

		private OrderedIterator(OrderedIterator other, bool dummy)
			: base(other)
		{
			if (other.iter != null)
			{
				this.iter = (BaseIterator)other.iter.Clone();
			}
			this.list = other.list;
			this.index = other.index;
		}

		public override XPathNodeIterator Clone()
		{
			return new OrderedIterator(this);
		}

		public override bool MoveNextCore()
		{
			if (this.iter != null)
			{
				return this.iter.MoveNext();
			}
			if (this.index++ < this.list.Count)
			{
				return true;
			}
			this.index--;
			return false;
		}

		public override XPathNavigator Current
		{
			get
			{
				return (this.iter == null) ? ((this.index >= 0) ? ((XPathNavigator)this.list[this.index]) : null) : this.iter.Current;
			}
		}

		private BaseIterator iter;

		private ArrayList list;

		private int index = -1;
	}
}
