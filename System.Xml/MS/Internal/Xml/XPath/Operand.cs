using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal class Operand : AstNode
	{
		public Operand(string val)
		{
			this.type = XPathResultType.String;
			this.val = val;
		}

		public Operand(double val)
		{
			this.type = XPathResultType.Number;
			this.val = val;
		}

		public Operand(bool val)
		{
			this.type = XPathResultType.Boolean;
			this.val = val;
		}

		public override AstNode.AstType Type
		{
			get
			{
				return AstNode.AstType.ConstantOperand;
			}
		}

		public override XPathResultType ReturnType
		{
			get
			{
				return this.type;
			}
		}

		public object OperandValue
		{
			get
			{
				return this.val;
			}
		}

		private XPathResultType type;

		private object val;
	}
}
