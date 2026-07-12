using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal class Filter : AstNode
	{
		public Filter(AstNode input, AstNode condition)
		{
			this._input = input;
			this._condition = condition;
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
				return this._input;
			}
		}

		public AstNode Condition
		{
			get
			{
				return this._condition;
			}
		}

		private AstNode _input;

		private AstNode _condition;
	}
}
