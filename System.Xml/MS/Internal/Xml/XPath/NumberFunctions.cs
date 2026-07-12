using System;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace MS.Internal.Xml.XPath
{
	internal sealed class NumberFunctions : ValueQuery
	{
		public NumberFunctions(Function.FunctionType ftype, Query arg)
		{
			this._arg = arg;
			this._ftype = ftype;
		}

		private NumberFunctions(NumberFunctions other)
			: base(other)
		{
			this._arg = Query.Clone(other._arg);
			this._ftype = other._ftype;
		}

		public override void SetXsltContext(XsltContext context)
		{
			if (this._arg != null)
			{
				this._arg.SetXsltContext(context);
			}
		}

		internal static double Number(bool arg)
		{
			if (!arg)
			{
				return 0.0;
			}
			return 1.0;
		}

		internal static double Number(string arg)
		{
			return XmlConvert.ToXPathDouble(arg);
		}

		public override object Evaluate(XPathNodeIterator nodeIterator)
		{
			Function.FunctionType ftype = this._ftype;
			if (ftype == Function.FunctionType.FuncNumber)
			{
				return this.Number(nodeIterator);
			}
			switch (ftype)
			{
			case Function.FunctionType.FuncSum:
				return this.Sum(nodeIterator);
			case Function.FunctionType.FuncFloor:
				return this.Floor(nodeIterator);
			case Function.FunctionType.FuncCeiling:
				return this.Ceiling(nodeIterator);
			case Function.FunctionType.FuncRound:
				return this.Round(nodeIterator);
			default:
				return null;
			}
		}

		private double Number(XPathNodeIterator nodeIterator)
		{
			if (this._arg == null)
			{
				return XmlConvert.ToXPathDouble(nodeIterator.Current.Value);
			}
			object obj = this._arg.Evaluate(nodeIterator);
			switch (base.GetXPathType(obj))
			{
			case XPathResultType.Number:
				return (double)obj;
			case XPathResultType.String:
				return NumberFunctions.Number((string)obj);
			case XPathResultType.Boolean:
				return NumberFunctions.Number((bool)obj);
			case XPathResultType.NodeSet:
			{
				XPathNavigator xpathNavigator = this._arg.Advance();
				if (xpathNavigator != null)
				{
					return NumberFunctions.Number(xpathNavigator.Value);
				}
				break;
			}
			case (XPathResultType)4:
				return NumberFunctions.Number(((XPathNavigator)obj).Value);
			}
			return double.NaN;
		}

		private double Sum(XPathNodeIterator nodeIterator)
		{
			double num = 0.0;
			this._arg.Evaluate(nodeIterator);
			XPathNavigator xpathNavigator;
			while ((xpathNavigator = this._arg.Advance()) != null)
			{
				num += NumberFunctions.Number(xpathNavigator.Value);
			}
			return num;
		}

		private double Floor(XPathNodeIterator nodeIterator)
		{
			return Math.Floor((double)this._arg.Evaluate(nodeIterator));
		}

		private double Ceiling(XPathNodeIterator nodeIterator)
		{
			return Math.Ceiling((double)this._arg.Evaluate(nodeIterator));
		}

		private double Round(XPathNodeIterator nodeIterator)
		{
			return XmlConvert.XPathRound(XmlConvert.ToXPathDouble(this._arg.Evaluate(nodeIterator)));
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
			return new NumberFunctions(this);
		}

		private Query _arg;

		private Function.FunctionType _ftype;
	}
}
