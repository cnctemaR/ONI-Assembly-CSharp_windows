using System;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace MS.Internal.Xml.XPath
{
	internal sealed class NumericExpr : ValueQuery
	{
		public NumericExpr(Operator.Op op, Query opnd1, Query opnd2)
		{
			if (opnd1.StaticType != XPathResultType.Number)
			{
				opnd1 = new NumberFunctions(Function.FunctionType.FuncNumber, opnd1);
			}
			if (opnd2.StaticType != XPathResultType.Number)
			{
				opnd2 = new NumberFunctions(Function.FunctionType.FuncNumber, opnd2);
			}
			this._op = op;
			this._opnd1 = opnd1;
			this._opnd2 = opnd2;
		}

		private NumericExpr(NumericExpr other)
			: base(other)
		{
			this._op = other._op;
			this._opnd1 = Query.Clone(other._opnd1);
			this._opnd2 = Query.Clone(other._opnd2);
		}

		public override void SetXsltContext(XsltContext context)
		{
			this._opnd1.SetXsltContext(context);
			this._opnd2.SetXsltContext(context);
		}

		public override object Evaluate(XPathNodeIterator nodeIterator)
		{
			return NumericExpr.GetValue(this._op, XmlConvert.ToXPathDouble(this._opnd1.Evaluate(nodeIterator)), XmlConvert.ToXPathDouble(this._opnd2.Evaluate(nodeIterator)));
		}

		private static double GetValue(Operator.Op op, double n1, double n2)
		{
			switch (op)
			{
			case Operator.Op.PLUS:
				return n1 + n2;
			case Operator.Op.MINUS:
				return n1 - n2;
			case Operator.Op.MUL:
				return n1 * n2;
			case Operator.Op.DIV:
				return n1 / n2;
			case Operator.Op.MOD:
				return n1 % n2;
			default:
				return 0.0;
			}
		}

		public override XPathResultType StaticType
		{
			get
			{
				return XPathResultType.Number;
			}
		}

		public override XPathNodeIterator Clone()
		{
			return new NumericExpr(this);
		}

		private Operator.Op _op;

		private Query _opnd1;

		private Query _opnd2;
	}
}
