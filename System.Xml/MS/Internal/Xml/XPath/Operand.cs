using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal class Operand : AstNode
	{
		public Operand(string val)
		{
			this._type = XPathResultType.String;
			this._val = val;
		}

		public Operand(double val)
		{
			this._type = XPathResultType.Number;
			this._val = val;
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
				return this._type;
			}
		}

		public object OperandValue
		{
			get
			{
				return this._val;
			}
		}

		private XPathResultType _type;

		private object _val;
	}
}
