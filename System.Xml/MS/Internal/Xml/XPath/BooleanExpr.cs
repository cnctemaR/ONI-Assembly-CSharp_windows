using System;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace MS.Internal.Xml.XPath
{
	internal sealed class BooleanExpr : ValueQuery
	{
		public BooleanExpr(Operator.Op op, Query opnd1, Query opnd2)
		{
			if (opnd1.StaticType != XPathResultType.Boolean)
			{
				opnd1 = new BooleanFunctions(Function.FunctionType.FuncBoolean, opnd1);
			}
			if (opnd2.StaticType != XPathResultType.Boolean)
			{
				opnd2 = new BooleanFunctions(Function.FunctionType.FuncBoolean, opnd2);
			}
			this._opnd1 = opnd1;
			this._opnd2 = opnd2;
			this._isOr = op == Operator.Op.OR;
		}

		private BooleanExpr(BooleanExpr other)
			: base(other)
		{
			this._opnd1 = Query.Clone(other._opnd1);
			this._opnd2 = Query.Clone(other._opnd2);
			this._isOr = other._isOr;
		}

		public override void SetXsltContext(XsltContext context)
		{
			this._opnd1.SetXsltContext(context);
			this._opnd2.SetXsltContext(context);
		}

		public override object Evaluate(XPathNodeIterator nodeIterator)
		{
			object obj = this._opnd1.Evaluate(nodeIterator);
			if ((bool)obj == this._isOr)
			{
				return obj;
			}
			return this._opnd2.Evaluate(nodeIterator);
		}

		public override XPathNodeIterator Clone()
		{
			return new BooleanExpr(this);
		}

		public override XPathResultType StaticType
		{
			get
			{
				return XPathResultType.Boolean;
			}
		}

		private Query _opnd1;

		private Query _opnd2;

		private bool _isOr;
	}
}
