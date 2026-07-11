using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal class Group : AstNode
	{
		public Group(AstNode groupNode)
		{
			this.groupNode = groupNode;
		}

		public override AstNode.AstType Type
		{
			get
			{
				return AstNode.AstType.Group;
			}
		}

		public override XPathResultType ReturnType
		{
			get
			{
				return XPathResultType.NodeSet;
			}
		}

		public AstNode GroupNode
		{
			get
			{
				return this.groupNode;
			}
		}

		private AstNode groupNode;
	}
}
