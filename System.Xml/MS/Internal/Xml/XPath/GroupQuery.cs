using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal sealed class GroupQuery : BaseAxisQuery
	{
		public GroupQuery(Query qy)
			: base(qy)
		{
		}

		private GroupQuery(GroupQuery other)
			: base(other)
		{
		}

		public override XPathNavigator Advance()
		{
			this.currentNode = this.qyInput.Advance();
			if (this.currentNode != null)
			{
				this.position++;
			}
			return this.currentNode;
		}

		public override object Evaluate(XPathNodeIterator nodeIterator)
		{
			return this.qyInput.Evaluate(nodeIterator);
		}

		public override XPathNodeIterator Clone()
		{
			return new GroupQuery(this);
		}

		public override XPathResultType StaticType
		{
			get
			{
				return this.qyInput.StaticType;
			}
		}

		public override QueryProps Properties
		{
			get
			{
				return QueryProps.Position;
			}
		}
	}
}
