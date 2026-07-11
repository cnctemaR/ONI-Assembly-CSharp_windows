using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal class ContextQuery : Query
	{
		public ContextQuery()
		{
			this.count = 0;
		}

		protected ContextQuery(ContextQuery other)
			: base(other)
		{
			this.contextNode = other.contextNode;
		}

		public override void Reset()
		{
			this.count = 0;
		}

		public override XPathNavigator Current
		{
			get
			{
				return this.contextNode;
			}
		}

		public override object Evaluate(XPathNodeIterator context)
		{
			this.contextNode = context.Current;
			this.count = 0;
			return this;
		}

		public override XPathNavigator Advance()
		{
			if (this.count == 0)
			{
				this.count = 1;
				return this.contextNode;
			}
			return null;
		}

		public override XPathNavigator MatchNode(XPathNavigator current)
		{
			return current;
		}

		public override XPathNodeIterator Clone()
		{
			return new ContextQuery(this);
		}

		public override XPathResultType StaticType
		{
			get
			{
				return XPathResultType.NodeSet;
			}
		}

		public override int CurrentPosition
		{
			get
			{
				return this.count;
			}
		}

		public override int Count
		{
			get
			{
				return 1;
			}
		}

		public override QueryProps Properties
		{
			get
			{
				return (QueryProps)23;
			}
		}

		protected XPathNavigator contextNode;
	}
}
