using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal class Operator : AstNode
	{
		public static Operator.Op InvertOperator(Operator.Op op)
		{
			return Operator.s_invertOp[(int)op];
		}

		public Operator(Operator.Op op, AstNode opnd1, AstNode opnd2)
		{
			this._opType = op;
			this._opnd1 = opnd1;
			this._opnd2 = opnd2;
		}

		public override AstNode.AstType Type
		{
			get
			{
				return AstNode.AstType.Operator;
			}
		}

		public override XPathResultType ReturnType
		{
			get
			{
				if (this._opType <= Operator.Op.GE)
				{
					return XPathResultType.Boolean;
				}
				if (this._opType <= Operator.Op.MOD)
				{
					return XPathResultType.Number;
				}
				return XPathResultType.NodeSet;
			}
		}

		public Operator.Op OperatorType
		{
			get
			{
				return this._opType;
			}
		}

		public AstNode Operand1
		{
			get
			{
				return this._opnd1;
			}
		}

		public AstNode Operand2
		{
			get
			{
				return this._opnd2;
			}
		}

		private static Operator.Op[] s_invertOp = new Operator.Op[]
		{
			Operator.Op.INVALID,
			Operator.Op.INVALID,
			Operator.Op.INVALID,
			Operator.Op.EQ,
			Operator.Op.NE,
			Operator.Op.GT,
			Operator.Op.GE,
			Operator.Op.LT,
			Operator.Op.LE
		};

		private Operator.Op _opType;

		private AstNode _opnd1;

		private AstNode _opnd2;

		public enum Op
		{
			INVALID,
			OR,
			AND,
			EQ,
			NE,
			LT,
			LE,
			GT,
			GE,
			PLUS,
			MINUS,
			MUL,
			DIV,
			MOD,
			UNION
		}
	}
}
