using System;
using System.Collections;

namespace System.Xml.XPath
{
	internal abstract class RelationalExpr : ExprBoolean
	{
		public RelationalExpr(Expression left, Expression right)
			: base(left, right)
		{
		}

		public override bool StaticValueAsBoolean
		{
			get
			{
				return this.HasStaticValue && this.Compare(this._left.StaticValueAsNumber, this._right.StaticValueAsNumber);
			}
		}

		public override bool EvaluateBoolean(BaseIterator iter)
		{
			XPathResultType xpathResultType = this._left.GetReturnType(iter);
			XPathResultType xpathResultType2 = this._right.GetReturnType(iter);
			if (xpathResultType == XPathResultType.Any)
			{
				xpathResultType = Expression.GetReturnType(this._left.Evaluate(iter));
			}
			if (xpathResultType2 == XPathResultType.Any)
			{
				xpathResultType2 = Expression.GetReturnType(this._right.Evaluate(iter));
			}
			if (xpathResultType == XPathResultType.Navigator)
			{
				xpathResultType = XPathResultType.String;
			}
			if (xpathResultType2 == XPathResultType.Navigator)
			{
				xpathResultType2 = XPathResultType.String;
			}
			if (xpathResultType != XPathResultType.NodeSet && xpathResultType2 != XPathResultType.NodeSet)
			{
				return this.Compare(this._left.EvaluateNumber(iter), this._right.EvaluateNumber(iter));
			}
			bool flag = false;
			Expression expression;
			Expression expression2;
			if (xpathResultType != XPathResultType.NodeSet)
			{
				flag = true;
				expression = this._right;
				expression2 = this._left;
				XPathResultType xpathResultType3 = xpathResultType;
				xpathResultType2 = xpathResultType3;
			}
			else
			{
				expression = this._left;
				expression2 = this._right;
			}
			if (xpathResultType2 == XPathResultType.Boolean)
			{
				bool flag2 = expression.EvaluateBoolean(iter);
				bool flag3 = expression2.EvaluateBoolean(iter);
				return this.Compare(Convert.ToDouble(flag2), Convert.ToDouble(flag3), flag);
			}
			BaseIterator baseIterator = expression.EvaluateNodeSet(iter);
			if (xpathResultType2 == XPathResultType.Number || xpathResultType2 == XPathResultType.String)
			{
				double num = expression2.EvaluateNumber(iter);
				while (baseIterator.MoveNext())
				{
					if (this.Compare(XPathFunctions.ToNumber(baseIterator.Current.Value), num, flag))
					{
						return true;
					}
				}
			}
			else if (xpathResultType2 == XPathResultType.NodeSet)
			{
				BaseIterator baseIterator2 = expression2.EvaluateNodeSet(iter);
				ArrayList arrayList = new ArrayList();
				while (baseIterator.MoveNext())
				{
					XPathNavigator xpathNavigator = baseIterator.Current;
					arrayList.Add(XPathFunctions.ToNumber(xpathNavigator.Value));
				}
				while (baseIterator2.MoveNext())
				{
					XPathNavigator xpathNavigator2 = baseIterator2.Current;
					double num2 = XPathFunctions.ToNumber(xpathNavigator2.Value);
					for (int i = 0; i < arrayList.Count; i++)
					{
						if (this.Compare((double)arrayList[i], num2))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public abstract bool Compare(double arg1, double arg2);

		public bool Compare(double arg1, double arg2, bool fReverse)
		{
			if (fReverse)
			{
				return this.Compare(arg2, arg1);
			}
			return this.Compare(arg1, arg2);
		}
	}
}
