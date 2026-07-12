using System;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace MS.Internal.Xml.XPath
{
	internal sealed class BooleanFunctions : ValueQuery
	{
		public BooleanFunctions(Function.FunctionType funcType, Query arg)
		{
			this._arg = arg;
			this._funcType = funcType;
		}

		private BooleanFunctions(BooleanFunctions other)
			: base(other)
		{
			this._arg = Query.Clone(other._arg);
			this._funcType = other._funcType;
		}

		public override void SetXsltContext(XsltContext context)
		{
			if (this._arg != null)
			{
				this._arg.SetXsltContext(context);
			}
		}

		public override object Evaluate(XPathNodeIterator nodeIterator)
		{
			Function.FunctionType funcType = this._funcType;
			switch (funcType)
			{
			case Function.FunctionType.FuncBoolean:
				return this.toBoolean(nodeIterator);
			case Function.FunctionType.FuncNumber:
				break;
			case Function.FunctionType.FuncTrue:
				return true;
			case Function.FunctionType.FuncFalse:
				return false;
			case Function.FunctionType.FuncNot:
				return this.Not(nodeIterator);
			default:
				if (funcType == Function.FunctionType.FuncLang)
				{
					return this.Lang(nodeIterator);
				}
				break;
			}
			return false;
		}

		internal static bool toBoolean(double number)
		{
			return number != 0.0 && !double.IsNaN(number);
		}

		internal static bool toBoolean(string str)
		{
			return str.Length > 0;
		}

		internal bool toBoolean(XPathNodeIterator nodeIterator)
		{
			object obj = this._arg.Evaluate(nodeIterator);
			if (obj is XPathNodeIterator)
			{
				return this._arg.Advance() != null;
			}
			string text = obj as string;
			if (text != null)
			{
				return BooleanFunctions.toBoolean(text);
			}
			if (obj is double)
			{
				return BooleanFunctions.toBoolean((double)obj);
			}
			return !(obj is bool) || (bool)obj;
		}

		public override XPathResultType StaticType
		{
			get
			{
				return XPathResultType.Boolean;
			}
		}

		private bool Not(XPathNodeIterator nodeIterator)
		{
			return !(bool)this._arg.Evaluate(nodeIterator);
		}

		private bool Lang(XPathNodeIterator nodeIterator)
		{
			string text = this._arg.Evaluate(nodeIterator).ToString();
			string xmlLang = nodeIterator.Current.XmlLang;
			return xmlLang.StartsWith(text, StringComparison.OrdinalIgnoreCase) && (xmlLang.Length == text.Length || xmlLang[text.Length] == '-');
		}

		public override XPathNodeIterator Clone()
		{
			return new BooleanFunctions(this);
		}

		private Query _arg;

		private Function.FunctionType _funcType;
	}
}
