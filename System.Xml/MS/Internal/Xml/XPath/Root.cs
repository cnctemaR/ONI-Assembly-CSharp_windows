using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal class Root : AstNode
	{
		public override AstNode.AstType Type
		{
			get
			{
				return AstNode.AstType.Root;
			}
		}

		public override XPathResultType ReturnType
		{
			get
			{
				return XPathResultType.NodeSet;
			}
		}
	}
}
