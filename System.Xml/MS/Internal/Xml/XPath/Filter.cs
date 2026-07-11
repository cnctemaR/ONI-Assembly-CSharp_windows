using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal class Filter : AstNode
	{
		public Filter(AstNode input, AstNode condition)
		{
			this.input = input;
			this.condition = condition;
		}

		public override AstNode.AstType Type
		{
			get
			{
				return AstNode.AstType.Filter;
			}
		}

		public override XPathResultType ReturnType
		{
			get
			{
				return XPathResultType.NodeSet;
			}
		}

		public AstNode Input
		{
			get
			{
				return this.input;
			}
		}

		public AstNode Condition
		{
			get
			{
				return this.condition;
			}
		}

		private AstNode input;

		private AstNode condition;
	}
}
