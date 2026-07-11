using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal sealed class DescendantOverDescendantQuery : DescendantBaseQuery
	{
		public DescendantOverDescendantQuery(Query qyParent, bool matchSelf, string name, string prefix, XPathNodeType typeTest, bool abbrAxis)
			: base(qyParent, name, prefix, typeTest, matchSelf, abbrAxis)
		{
		}

		private DescendantOverDescendantQuery(DescendantOverDescendantQuery other)
			: base(other)
		{
			this.level = other.level;
		}

		public override void Reset()
		{
			this.level = 0;
			base.Reset();
		}

		public override XPathNavigator Advance()
		{
			for (;;)
			{
				IL_0000:
				if (this.level == 0)
				{
					this.currentNode = this.qyInput.Advance();
					this.position = 0;
					if (this.currentNode == null)
					{
						break;
					}
					if (this.matchSelf && this.matches(this.currentNode))
					{
						goto Block_3;
					}
					this.currentNode = this.currentNode.Clone();
					if (!this.MoveToFirstChild())
					{
						continue;
					}
				}
				else if (!this.MoveUpUntillNext())
				{
					continue;
				}
				while (!this.matches(this.currentNode))
				{
					if (!this.MoveToFirstChild())
					{
						goto IL_0000;
					}
				}
				goto Block_5;
			}
			return null;
			Block_3:
			this.position = 1;
			return this.currentNode;
			Block_5:
			this.position++;
			return this.currentNode;
		}

		private bool MoveToFirstChild()
		{
			if (this.currentNode.MoveToFirstChild())
			{
				this.level++;
				return true;
			}
			return false;
		}

		private bool MoveUpUntillNext()
		{
			while (!this.currentNode.MoveToNext())
			{
				this.level--;
				if (this.level == 0)
				{
					return false;
				}
				this.currentNode.MoveToParent();
			}
			return true;
		}

		public override XPathNodeIterator Clone()
		{
			return new DescendantOverDescendantQuery(this);
		}

		private int level;
	}
}
