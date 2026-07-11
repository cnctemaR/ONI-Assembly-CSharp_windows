using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Xml.XPath;
using System.Xml.Xsl.Xslt;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class XsltLibrary
	{
		internal XsltLibrary(XmlQueryRuntime runtime)
		{
			this.runtime = runtime;
		}

		public string FormatMessage(string res, IList<string> args)
		{
			string[] array = new string[args.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = args[i];
			}
			return XslTransformException.CreateMessage(res, array);
		}

		public int CheckScriptNamespace(string nsUri)
		{
			if (this.runtime.ExternalContext.GetLateBoundObject(nsUri) != null)
			{
				throw new XslTransformException("Cannot have both an extension object and a script implementing the same namespace '{0}'.", new string[] { nsUri });
			}
			return 0;
		}

		public bool ElementAvailable(XmlQualifiedName name)
		{
			return QilGenerator.IsElementAvailable(name);
		}

		public bool FunctionAvailable(XmlQualifiedName name)
		{
			if (this.functionsAvail == null)
			{
				this.functionsAvail = new HybridDictionary();
			}
			else
			{
				object obj = this.functionsAvail[name];
				if (obj != null)
				{
					return (bool)obj;
				}
			}
			bool flag = this.FunctionAvailableHelper(name);
			this.functionsAvail[name] = flag;
			return flag;
		}

		private bool FunctionAvailableHelper(XmlQualifiedName name)
		{
			return QilGenerator.IsFunctionAvailable(name.Name, name.Namespace) || (name.Namespace.Length != 0 && !(name.Namespace == "http://www.w3.org/1999/XSL/Transform") && (this.runtime.ExternalContext.LateBoundFunctionExists(name.Name, name.Namespace) || this.runtime.EarlyBoundFunctionExists(name.Name, name.Namespace)));
		}

		public int RegisterDecimalFormat(XmlQualifiedName name, string infinitySymbol, string nanSymbol, string characters)
		{
			if (this.decimalFormats == null)
			{
				this.decimalFormats = new Dictionary<XmlQualifiedName, DecimalFormat>();
			}
			this.decimalFormats.Add(name, this.CreateDecimalFormat(infinitySymbol, nanSymbol, characters));
			return 0;
		}

		private DecimalFormat CreateDecimalFormat(string infinitySymbol, string nanSymbol, string characters)
		{
			NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
			numberFormatInfo.NumberDecimalSeparator = char.ToString(characters[0]);
			numberFormatInfo.NumberGroupSeparator = char.ToString(characters[1]);
			numberFormatInfo.PositiveInfinitySymbol = infinitySymbol;
			numberFormatInfo.NegativeSign = char.ToString(characters[7]);
			numberFormatInfo.NaNSymbol = nanSymbol;
			numberFormatInfo.PercentSymbol = char.ToString(characters[2]);
			numberFormatInfo.PerMilleSymbol = char.ToString(characters[3]);
			numberFormatInfo.NegativeInfinitySymbol = numberFormatInfo.NegativeSign + numberFormatInfo.PositiveInfinitySymbol;
			return new DecimalFormat(numberFormatInfo, characters[5], characters[4], characters[6]);
		}

		public double RegisterDecimalFormatter(string formatPicture, string infinitySymbol, string nanSymbol, string characters)
		{
			if (this.decimalFormatters == null)
			{
				this.decimalFormatters = new List<DecimalFormatter>();
			}
			this.decimalFormatters.Add(new DecimalFormatter(formatPicture, this.CreateDecimalFormat(infinitySymbol, nanSymbol, characters)));
			return (double)(this.decimalFormatters.Count - 1);
		}

		public string FormatNumberStatic(double value, double decimalFormatterIndex)
		{
			int num = (int)decimalFormatterIndex;
			return this.decimalFormatters[num].Format(value);
		}

		public string FormatNumberDynamic(double value, string formatPicture, XmlQualifiedName decimalFormatName, string errorMessageName)
		{
			DecimalFormat decimalFormat;
			if (this.decimalFormats == null || !this.decimalFormats.TryGetValue(decimalFormatName, out decimalFormat))
			{
				throw new XslTransformException("Decimal format '{0}' is not defined.", new string[] { errorMessageName });
			}
			return new DecimalFormatter(formatPicture, decimalFormat).Format(value);
		}

		public string NumberFormat(IList<XPathItem> value, string formatString, double lang, string letterValue, string groupingSeparator, double groupingSize)
		{
			return new NumberFormatter(formatString, (int)lang, letterValue, groupingSeparator, (int)groupingSize).FormatSequence(value);
		}

		public int LangToLcid(string lang, bool forwardCompatibility)
		{
			return XsltLibrary.LangToLcidInternal(lang, forwardCompatibility, null);
		}

		internal static int LangToLcidInternal(string lang, bool forwardCompatibility, IErrorHelper errorHelper)
		{
			int num = 127;
			if (lang != null)
			{
				if (lang.Length == 0)
				{
					if (!forwardCompatibility)
					{
						if (errorHelper == null)
						{
							throw new XslTransformException("'{1}' is an invalid value for the '{0}' attribute.", new string[] { "lang", lang });
						}
						errorHelper.ReportError("'{1}' is an invalid value for the '{0}' attribute.", new string[] { "lang", lang });
					}
				}
				else
				{
					try
					{
						num = new CultureInfo(lang).LCID;
					}
					catch (ArgumentException)
					{
						if (!forwardCompatibility)
						{
							if (errorHelper == null)
							{
								throw new XslTransformException("'{0}' is not a supported language identifier.", new string[] { lang });
							}
							errorHelper.ReportError("'{0}' is not a supported language identifier.", new string[] { lang });
						}
					}
				}
			}
			return num;
		}

		private static TypeCode GetTypeCode(XPathItem item)
		{
			Type valueType = item.ValueType;
			if (valueType == XsltConvert.StringType)
			{
				return TypeCode.String;
			}
			if (valueType == XsltConvert.DoubleType)
			{
				return TypeCode.Double;
			}
			return TypeCode.Boolean;
		}

		private static TypeCode WeakestTypeCode(TypeCode typeCode1, TypeCode typeCode2)
		{
			if (typeCode1 >= typeCode2)
			{
				return typeCode2;
			}
			return typeCode1;
		}

		private static bool CompareNumbers(XsltLibrary.ComparisonOperator op, double left, double right)
		{
			switch (op)
			{
			case XsltLibrary.ComparisonOperator.Eq:
				return left == right;
			case XsltLibrary.ComparisonOperator.Ne:
				return left != right;
			case XsltLibrary.ComparisonOperator.Lt:
				return left < right;
			case XsltLibrary.ComparisonOperator.Le:
				return left <= right;
			case XsltLibrary.ComparisonOperator.Gt:
				return left > right;
			default:
				return left >= right;
			}
		}

		private static bool CompareValues(XsltLibrary.ComparisonOperator op, XPathItem left, XPathItem right, TypeCode compType)
		{
			if (compType == TypeCode.Double)
			{
				return XsltLibrary.CompareNumbers(op, XsltConvert.ToDouble(left), XsltConvert.ToDouble(right));
			}
			if (compType == TypeCode.String)
			{
				return XsltConvert.ToString(left) == XsltConvert.ToString(right) == (op == XsltLibrary.ComparisonOperator.Eq);
			}
			return XsltConvert.ToBoolean(left) == XsltConvert.ToBoolean(right) == (op == XsltLibrary.ComparisonOperator.Eq);
		}

		private static bool CompareNodeSetAndValue(XsltLibrary.ComparisonOperator op, IList<XPathNavigator> nodeset, XPathItem val, TypeCode compType)
		{
			if (compType == TypeCode.Boolean)
			{
				return XsltLibrary.CompareNumbers(op, (double)((nodeset.Count != 0) ? 1 : 0), (double)(XsltConvert.ToBoolean(val) ? 1 : 0));
			}
			int count = nodeset.Count;
			for (int i = 0; i < count; i++)
			{
				if (XsltLibrary.CompareValues(op, nodeset[i], val, compType))
				{
					return true;
				}
			}
			return false;
		}

		private static bool CompareNodeSetAndNodeSet(XsltLibrary.ComparisonOperator op, IList<XPathNavigator> left, IList<XPathNavigator> right, TypeCode compType)
		{
			int count = left.Count;
			int count2 = right.Count;
			for (int i = 0; i < count; i++)
			{
				for (int j = 0; j < count2; j++)
				{
					if (XsltLibrary.CompareValues(op, left[i], right[j], compType))
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool EqualityOperator(double opCode, IList<XPathItem> left, IList<XPathItem> right)
		{
			XsltLibrary.ComparisonOperator comparisonOperator = (XsltLibrary.ComparisonOperator)opCode;
			if (XsltLibrary.IsNodeSetOrRtf(left))
			{
				if (XsltLibrary.IsNodeSetOrRtf(right))
				{
					return XsltLibrary.CompareNodeSetAndNodeSet(comparisonOperator, XsltLibrary.ToNodeSetOrRtf(left), XsltLibrary.ToNodeSetOrRtf(right), TypeCode.String);
				}
				XPathItem xpathItem = right[0];
				return XsltLibrary.CompareNodeSetAndValue(comparisonOperator, XsltLibrary.ToNodeSetOrRtf(left), xpathItem, XsltLibrary.GetTypeCode(xpathItem));
			}
			else
			{
				if (XsltLibrary.IsNodeSetOrRtf(right))
				{
					XPathItem xpathItem2 = left[0];
					return XsltLibrary.CompareNodeSetAndValue(comparisonOperator, XsltLibrary.ToNodeSetOrRtf(right), xpathItem2, XsltLibrary.GetTypeCode(xpathItem2));
				}
				XPathItem xpathItem3 = left[0];
				XPathItem xpathItem4 = right[0];
				return XsltLibrary.CompareValues(comparisonOperator, xpathItem3, xpathItem4, XsltLibrary.WeakestTypeCode(XsltLibrary.GetTypeCode(xpathItem3), XsltLibrary.GetTypeCode(xpathItem4)));
			}
		}

		private static XsltLibrary.ComparisonOperator InvertOperator(XsltLibrary.ComparisonOperator op)
		{
			switch (op)
			{
			case XsltLibrary.ComparisonOperator.Lt:
				return XsltLibrary.ComparisonOperator.Gt;
			case XsltLibrary.ComparisonOperator.Le:
				return XsltLibrary.ComparisonOperator.Ge;
			case XsltLibrary.ComparisonOperator.Gt:
				return XsltLibrary.ComparisonOperator.Lt;
			case XsltLibrary.ComparisonOperator.Ge:
				return XsltLibrary.ComparisonOperator.Le;
			default:
				return op;
			}
		}

		public bool RelationalOperator(double opCode, IList<XPathItem> left, IList<XPathItem> right)
		{
			XsltLibrary.ComparisonOperator comparisonOperator = (XsltLibrary.ComparisonOperator)opCode;
			if (XsltLibrary.IsNodeSetOrRtf(left))
			{
				if (XsltLibrary.IsNodeSetOrRtf(right))
				{
					return XsltLibrary.CompareNodeSetAndNodeSet(comparisonOperator, XsltLibrary.ToNodeSetOrRtf(left), XsltLibrary.ToNodeSetOrRtf(right), TypeCode.Double);
				}
				XPathItem xpathItem = right[0];
				return XsltLibrary.CompareNodeSetAndValue(comparisonOperator, XsltLibrary.ToNodeSetOrRtf(left), xpathItem, XsltLibrary.WeakestTypeCode(XsltLibrary.GetTypeCode(xpathItem), TypeCode.Double));
			}
			else
			{
				if (XsltLibrary.IsNodeSetOrRtf(right))
				{
					XPathItem xpathItem2 = left[0];
					comparisonOperator = XsltLibrary.InvertOperator(comparisonOperator);
					return XsltLibrary.CompareNodeSetAndValue(comparisonOperator, XsltLibrary.ToNodeSetOrRtf(right), xpathItem2, XsltLibrary.WeakestTypeCode(XsltLibrary.GetTypeCode(xpathItem2), TypeCode.Double));
				}
				XPathItem xpathItem3 = left[0];
				XPathItem xpathItem4 = right[0];
				return XsltLibrary.CompareValues(comparisonOperator, xpathItem3, xpathItem4, TypeCode.Double);
			}
		}

		public bool IsSameNodeSort(XPathNavigator nav1, XPathNavigator nav2)
		{
			XPathNodeType nodeType = nav1.NodeType;
			XPathNodeType nodeType2 = nav2.NodeType;
			if (XPathNodeType.Text <= nodeType && nodeType <= XPathNodeType.Whitespace)
			{
				return XPathNodeType.Text <= nodeType2 && nodeType2 <= XPathNodeType.Whitespace;
			}
			return nodeType == nodeType2 && Ref.Equal(nav1.LocalName, nav2.LocalName) && Ref.Equal(nav1.NamespaceURI, nav2.NamespaceURI);
		}

		[Conditional("DEBUG")]
		internal static void CheckXsltValue(XPathItem item)
		{
		}

		[Conditional("DEBUG")]
		internal static void CheckXsltValue(IList<XPathItem> val)
		{
			if (val.Count == 1)
			{
				XsltFunctions.EXslObjectType(val);
				return;
			}
			int count = val.Count;
			int num = 0;
			while (num < count && val[num].IsNode)
			{
				if (num == 1)
				{
					num += Math.Max(count - 4, 0);
				}
				num++;
			}
		}

		private static bool IsNodeSetOrRtf(IList<XPathItem> val)
		{
			return val.Count != 1 || val[0].IsNode;
		}

		private static IList<XPathNavigator> ToNodeSetOrRtf(IList<XPathItem> val)
		{
			return XmlILStorageConverter.ItemsToNavigators(val);
		}

		private XmlQueryRuntime runtime;

		private HybridDictionary functionsAvail;

		private Dictionary<XmlQualifiedName, DecimalFormat> decimalFormats;

		private List<DecimalFormatter> decimalFormatters;

		internal const int InvariantCultureLcid = 127;

		internal enum ComparisonOperator
		{
			Eq,
			Ne,
			Lt,
			Le,
			Gt,
			Ge
		}
	}
}
