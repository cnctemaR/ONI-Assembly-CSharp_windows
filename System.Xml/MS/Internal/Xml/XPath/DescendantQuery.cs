using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal class DescendantQuery : DescendantBaseQuery
	{
		internal DescendantQuery(Query qyParent, string Name, string Prefix, XPathNodeType Type, bool matchSelf, bool abbrAxis)
			: base(qyParent, Name, Prefix, Type, matchSelf, abbrAxis)
		{
		}

		public DescendantQuery(DescendantQuery other)
			: base(other)
		{
			this.nodeIterator = Query.Clone(other.nodeIterator);
		}

		public override void Reset()
		{
			this.nodeIterator = null;
			base.Reset();
		}

		public override XPathNavigator Advance()
		{
			for (;;)
			{
				if (this.nodeIterator == null)
				{
					this.position = 0;
					XPathNavigator xpathNavigator = this.qyInput.Advance();
					if (xpathNavigator == null)
					{
						break;
					}
					if (base.NameTest)
					{
						if (base.TypeTest == XPathNodeType.ProcessingInstruction)
						{
							this.nodeIterator = new IteratorFilter(xpathNavigator.SelectDescendants(base.TypeTest, this.matchSelf), base.Name);
						}
						else
						{
							this.nodeIterator = xpathNavigator.SelectDescendants(base.Name, base.Namespace, this.matchSelf);
						}
					}
					else
					{
						this.nodeIterator = xpathNavigator.SelectDescendants(base.TypeTest, this.matchSelf);
					}
				}
				if (this.nodeIterator.MoveNext())
				{
					goto Block_4;
				}
				this.nodeIterator = null;
			}
			return null;
			Block_4:
			this.position++;
			this.currentNode = this.nodeIterator.Current;
			return this.currentNode;
		}

		public override XPathNodeIterator Clone()
		{
			return new DescendantQuery(this);
		}

		private XPathNodeIterator nodeIterator;
	}
}
