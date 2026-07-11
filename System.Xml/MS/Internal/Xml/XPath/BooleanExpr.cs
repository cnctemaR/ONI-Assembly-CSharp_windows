using System;
using System.Xml;
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
			this.opnd1 = opnd1;
			this.opnd2 = opnd2;
			this.isOr = op == Operator.Op.OR;
		}

		private BooleanExpr(BooleanExpr other)
			: base(other)
		{
			this.opnd1 = Query.Clone(other.opnd1);
			this.opnd2 = Query.Clone(other.opnd2);
			this.isOr = other.isOr;
		}

		public override void SetXsltContext(XsltContext context)
		{
			this.opnd1.SetXsltContext(context);
			this.opnd2.SetXsltContext(context);
		}

		public override object Evaluate(XPathNodeIterator nodeIterator)
		{
			object obj = this.opnd1.Evaluate(nodeIterator);
			if ((bool)obj == this.isOr)
			{
				return obj;
			}
			return this.opnd2.Evaluate(nodeIterator);
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

		public override void PrintQuery(XmlWriter w)
		{
			w.WriteStartElement(base.GetType().Name);
			w.WriteAttributeString("op", (this.isOr ? Operator.Op.OR : Operator.Op.AND).ToString());
			this.opnd1.PrintQuery(w);
			this.opnd2.PrintQuery(w);
			w.WriteEndElement();
		}

		private Query opnd1;

		private Query opnd2;

		private bool isOr;
	}
}
