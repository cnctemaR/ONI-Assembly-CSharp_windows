using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace MS.Internal.Xml.XPath
{
	internal sealed class StringFunctions : ValueQuery
	{
		public StringFunctions(Function.FunctionType funcType, IList<Query> argList)
		{
			this.funcType = funcType;
			this.argList = argList;
		}

		private StringFunctions(StringFunctions other)
			: base(other)
		{
			this.funcType = other.funcType;
			Query[] array = new Query[other.argList.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = Query.Clone(other.argList[i]);
			}
			this.argList = array;
		}

		public override void SetXsltContext(XsltContext context)
		{
			for (int i = 0; i < this.argList.Count; i++)
			{
				this.argList[i].SetXsltContext(context);
			}
		}

		public override object Evaluate(XPathNodeIterator nodeIterator)
		{
			switch (this.funcType)
			{
			case Function.FunctionType.FuncString:
				return this.toString(nodeIterator);
			case Function.FunctionType.FuncConcat:
				return this.Concat(nodeIterator);
			case Function.FunctionType.FuncStartsWith:
				return this.StartsWith(nodeIterator);
			case Function.FunctionType.FuncContains:
				return this.Contains(nodeIterator);
			case Function.FunctionType.FuncSubstringBefore:
				return this.SubstringBefore(nodeIterator);
			case Function.FunctionType.FuncSubstringAfter:
				return this.SubstringAfter(nodeIterator);
			case Function.FunctionType.FuncSubstring:
				return this.Substring(nodeIterator);
			case Function.FunctionType.FuncStringLength:
				return this.StringLength(nodeIterator);
			case Function.FunctionType.FuncNormalize:
				return this.Normalize(nodeIterator);
			case Function.FunctionType.FuncTranslate:
				return this.Translate(nodeIterator);
			}
			return string.Empty;
		}

		internal static string toString(double num)
		{
			return num.ToString("R", NumberFormatInfo.InvariantInfo);
		}

		internal static string toString(bool b)
		{
			if (!b)
			{
				return "false";
			}
			return "true";
		}

		private string toString(XPathNodeIterator nodeIterator)
		{
			if (this.argList.Count <= 0)
			{
				return nodeIterator.Current.Value;
			}
			object obj = this.argList[0].Evaluate(nodeIterator);
			switch (base.GetXPathType(obj))
			{
			case XPathResultType.String:
				return (string)obj;
			case XPathResultType.Boolean:
				if (!(bool)obj)
				{
					return "false";
				}
				return "true";
			case XPathResultType.NodeSet:
			{
				XPathNavigator xpathNavigator = this.argList[0].Advance();
				if (xpathNavigator == null)
				{
					return string.Empty;
				}
				return xpathNavigator.Value;
			}
			case (XPathResultType)4:
				return ((XPathNavigator)obj).Value;
			default:
				return StringFunctions.toString((double)obj);
			}
		}

		public override XPathResultType StaticType
		{
			get
			{
				if (this.funcType == Function.FunctionType.FuncStringLength)
				{
					return XPathResultType.Number;
				}
				if (this.funcType == Function.FunctionType.FuncStartsWith || this.funcType == Function.FunctionType.FuncContains)
				{
					return XPathResultType.Boolean;
				}
				return XPathResultType.String;
			}
		}

		private string Concat(XPathNodeIterator nodeIterator)
		{
			int i = 0;
			StringBuilder stringBuilder = new StringBuilder();
			while (i < this.argList.Count)
			{
				stringBuilder.Append(this.argList[i++].Evaluate(nodeIterator).ToString());
			}
			return stringBuilder.ToString();
		}

		private bool StartsWith(XPathNodeIterator nodeIterator)
		{
			string text = this.argList[0].Evaluate(nodeIterator).ToString();
			string text2 = this.argList[1].Evaluate(nodeIterator).ToString();
			return text.Length >= text2.Length && string.CompareOrdinal(text, 0, text2, 0, text2.Length) == 0;
		}

		private bool Contains(XPathNodeIterator nodeIterator)
		{
			string text = this.argList[0].Evaluate(nodeIterator).ToString();
			string text2 = this.argList[1].Evaluate(nodeIterator).ToString();
			return StringFunctions.compareInfo.IndexOf(text, text2, CompareOptions.Ordinal) >= 0;
		}

		private string SubstringBefore(XPathNodeIterator nodeIterator)
		{
			string text = this.argList[0].Evaluate(nodeIterator).ToString();
			string text2 = this.argList[1].Evaluate(nodeIterator).ToString();
			if (text2.Length == 0)
			{
				return text2;
			}
			int num = StringFunctions.compareInfo.IndexOf(text, text2, CompareOptions.Ordinal);
			if (num >= 1)
			{
				return text.Substring(0, num);
			}
			return string.Empty;
		}

		private string SubstringAfter(XPathNodeIterator nodeIterator)
		{
			string text = this.argList[0].Evaluate(nodeIterator).ToString();
			string text2 = this.argList[1].Evaluate(nodeIterator).ToString();
			if (text2.Length == 0)
			{
				return text;
			}
			int num = StringFunctions.compareInfo.IndexOf(text, text2, CompareOptions.Ordinal);
			if (num >= 0)
			{
				return text.Substring(num + text2.Length);
			}
			return string.Empty;
		}

		private string Substring(XPathNodeIterator nodeIterator)
		{
			string text = this.argList[0].Evaluate(nodeIterator).ToString();
			double num = XmlConvert.XPathRound(XmlConvert.ToXPathDouble(this.argList[1].Evaluate(nodeIterator))) - 1.0;
			if (double.IsNaN(num) || (double)text.Length <= num)
			{
				return string.Empty;
			}
			if (this.argList.Count != 3)
			{
				if (num < 0.0)
				{
					num = 0.0;
				}
				return text.Substring((int)num);
			}
			double num2 = XmlConvert.XPathRound(XmlConvert.ToXPathDouble(this.argList[2].Evaluate(nodeIterator)));
			if (double.IsNaN(num2))
			{
				return string.Empty;
			}
			if (num < 0.0 || num2 < 0.0)
			{
				num2 = num + num2;
				if (num2 <= 0.0)
				{
					return string.Empty;
				}
				num = 0.0;
			}
			double num3 = (double)text.Length - num;
			if (num2 > num3)
			{
				num2 = num3;
			}
			return text.Substring((int)num, (int)num2);
		}

		private double StringLength(XPathNodeIterator nodeIterator)
		{
			if (this.argList.Count > 0)
			{
				return (double)this.argList[0].Evaluate(nodeIterator).ToString().Length;
			}
			return (double)nodeIterator.Current.Value.Length;
		}

		private string Normalize(XPathNodeIterator nodeIterator)
		{
			string text;
			if (this.argList.Count > 0)
			{
				text = this.argList[0].Evaluate(nodeIterator).ToString();
			}
			else
			{
				text = nodeIterator.Current.Value;
			}
			text = XmlConvert.TrimString(text);
			int i = 0;
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			XmlCharType instance = XmlCharType.Instance;
			while (i < text.Length)
			{
				if (!instance.IsWhiteSpace(text[i]))
				{
					flag = true;
					stringBuilder.Append(text[i]);
				}
				else if (flag)
				{
					flag = false;
					stringBuilder.Append(' ');
				}
				i++;
			}
			return stringBuilder.ToString();
		}

		private string Translate(XPathNodeIterator nodeIterator)
		{
			string text = this.argList[0].Evaluate(nodeIterator).ToString();
			string text2 = this.argList[1].Evaluate(nodeIterator).ToString();
			string text3 = this.argList[2].Evaluate(nodeIterator).ToString();
			int i = 0;
			StringBuilder stringBuilder = new StringBuilder();
			while (i < text.Length)
			{
				int num = text2.IndexOf(text[i]);
				if (num != -1)
				{
					if (num < text3.Length)
					{
						stringBuilder.Append(text3[num]);
					}
				}
				else
				{
					stringBuilder.Append(text[i]);
				}
				i++;
			}
			return stringBuilder.ToString();
		}

		public override XPathNodeIterator Clone()
		{
			return new StringFunctions(this);
		}

		public override void PrintQuery(XmlWriter w)
		{
			w.WriteStartElement(base.GetType().Name);
			w.WriteAttributeString("name", this.funcType.ToString());
			foreach (Query query in this.argList)
			{
				query.PrintQuery(w);
			}
			w.WriteEndElement();
		}

		private Function.FunctionType funcType;

		private IList<Query> argList;

		private static readonly CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;
	}
}
