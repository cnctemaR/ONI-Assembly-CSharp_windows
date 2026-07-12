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
			this._nodeIterator = Query.Clone(other._nodeIterator);
		}

		public override void Reset()
		{
			this._nodeIterator = null;
			base.Reset();
		}

		public override XPathNavigator Advance()
		{
			for (;;)
			{
				if (this._nodeIterator == null)
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
							this._nodeIterator = new IteratorFilter(xpathNavigator.SelectDescendants(base.TypeTest, this.matchSelf), base.Name);
						}
						else
						{
							this._nodeIterator = xpathNavigator.SelectDescendants(base.Name, base.Namespace, this.matchSelf);
						}
					}
					else
					{
						this._nodeIterator = xpathNavigator.SelectDescendants(base.TypeTest, this.matchSelf);
					}
				}
				if (this._nodeIterator.MoveNext())
				{
					goto Block_4;
				}
				this._nodeIterator = null;
			}
			return null;
			Block_4:
			this.position++;
			this.currentNode = this._nodeIterator.Current;
			return this.currentNode;
		}

		public override XPathNodeIterator Clone()
		{
			return new DescendantQuery(this);
		}

		private XPathNodeIterator _nodeIterator;
	}
}
