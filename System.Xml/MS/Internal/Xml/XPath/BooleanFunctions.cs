using System;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace MS.Internal.Xml.XPath
{
	internal sealed class BooleanFunctions : ValueQuery
	{
		public BooleanFunctions(Function.FunctionType funcType, Query arg)
		{
			this.arg = arg;
			this.funcType = funcType;
		}

		private BooleanFunctions(BooleanFunctions other)
			: base(other)
		{
			this.arg = Query.Clone(other.arg);
			this.funcType = other.funcType;
		}

		public override void SetXsltContext(XsltContext context)
		{
			if (this.arg != null)
			{
				this.arg.SetXsltContext(context);
			}
		}

		public override object Evaluate(XPathNodeIterator nodeIterator)
		{
			Function.FunctionType functionType = this.funcType;
			switch (functionType)
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
				if (functionType == Function.FunctionType.FuncLang)
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
			object obj = this.arg.Evaluate(nodeIterator);
			if (obj is XPathNodeIterator)
			{
				return this.arg.Advance() != null;
			}
			if (obj is string)
			{
				return BooleanFunctions.toBoolean((string)obj);
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
			return !(bool)this.arg.Evaluate(nodeIterator);
		}

		private bool Lang(XPathNodeIterator nodeIterator)
		{
			string text = this.arg.Evaluate(nodeIterator).ToString();
			string xmlLang = nodeIterator.Current.XmlLang;
			return xmlLang.StartsWith(text, StringComparison.OrdinalIgnoreCase) && (xmlLang.Length == text.Length || xmlLang[text.Length] == '-');
		}

		public override XPathNodeIterator Clone()
		{
			return new BooleanFunctions(this);
		}

		public override void PrintQuery(XmlWriter w)
		{
			w.WriteStartElement(base.GetType().Name);
			w.WriteAttributeString("name", this.funcType.ToString());
			if (this.arg != null)
			{
				this.arg.PrintQuery(w);
			}
			w.WriteEndElement();
		}

		private Query arg;

		private Function.FunctionType funcType;
	}
}
