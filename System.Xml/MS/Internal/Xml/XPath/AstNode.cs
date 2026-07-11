using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal abstract class AstNode
	{
		public abstract AstNode.AstType Type { get; }

		public abstract XPathResultType ReturnType { get; }

		public enum AstType
		{
			Axis,
			Operator,
			Filter,
			ConstantOperand,
			Function,
			Group,
			Root,
			Variable,
			Error
		}
	}
}
