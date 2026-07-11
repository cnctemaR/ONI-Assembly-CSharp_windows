using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Xml.Schema;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class XsltFunctions
	{
		public static bool StartsWith(string s1, string s2)
		{
			return s1.Length >= s2.Length && string.CompareOrdinal(s1, 0, s2, 0, s2.Length) == 0;
		}

		public static bool Contains(string s1, string s2)
		{
			return XsltFunctions.compareInfo.IndexOf(s1, s2, CompareOptions.Ordinal) >= 0;
		}

		public static string SubstringBefore(string s1, string s2)
		{
			if (s2.Length == 0)
			{
				return s2;
			}
			int num = XsltFunctions.compareInfo.IndexOf(s1, s2, CompareOptions.Ordinal);
			if (num >= 1)
			{
				return s1.Substring(0, num);
			}
			return string.Empty;
		}

		public static string SubstringAfter(string s1, string s2)
		{
			if (s2.Length == 0)
			{
				return s1;
			}
			int num = XsltFunctions.compareInfo.IndexOf(s1, s2, CompareOptions.Ordinal);
			if (num >= 0)
			{
				return s1.Substring(num + s2.Length);
			}
			return string.Empty;
		}

		public static string Substring(string value, double startIndex)
		{
			startIndex = XsltFunctions.Round(startIndex);
			if (startIndex <= 0.0)
			{
				return value;
			}
			if (startIndex <= (double)value.Length)
			{
				return value.Substring((int)startIndex - 1);
			}
			return string.Empty;
		}

		public static string Substring(string value, double startIndex, double length)
		{
			startIndex = XsltFunctions.Round(startIndex) - 1.0;
			if (startIndex >= (double)value.Length)
			{
				return string.Empty;
			}
			double num = startIndex + XsltFunctions.Round(length);
			startIndex = ((startIndex <= 0.0) ? 0.0 : startIndex);
			if (startIndex < num)
			{
				if (num > (double)value.Length)
				{
					num = (double)value.Length;
				}
				return value.Substring((int)startIndex, (int)(num - startIndex));
			}
			return string.Empty;
		}

		public static string NormalizeSpace(string value)
		{
			XmlCharType instance = XmlCharType.Instance;
			StringBuilder stringBuilder = null;
			int num = 0;
			int num2 = 0;
			int i;
			for (i = 0; i < value.Length; i++)
			{
				if (instance.IsWhiteSpace(value[i]))
				{
					if (i == num)
					{
						num++;
					}
					else if (value[i] != ' ' || num2 == i)
					{
						if (stringBuilder == null)
						{
							stringBuilder = new StringBuilder(value.Length);
						}
						else
						{
							stringBuilder.Append(' ');
						}
						if (num2 == i)
						{
							stringBuilder.Append(value, num, i - num - 1);
						}
						else
						{
							stringBuilder.Append(value, num, i - num);
						}
						num = i + 1;
					}
					else
					{
						num2 = i + 1;
					}
				}
			}
			if (stringBuilder == null)
			{
				if (num == i)
				{
					return string.Empty;
				}
				if (num == 0 && num2 != i)
				{
					return value;
				}
				stringBuilder = new StringBuilder(value.Length);
			}
			else if (i != num)
			{
				stringBuilder.Append(' ');
			}
			if (num2 == i)
			{
				stringBuilder.Append(value, num, i - num - 1);
			}
			else
			{
				stringBuilder.Append(value, num, i - num);
			}
			return stringBuilder.ToString();
		}

		public static string Translate(string arg, string mapString, string transString)
		{
			if (mapString.Length == 0)
			{
				return arg;
			}
			StringBuilder stringBuilder = new StringBuilder(arg.Length);
			for (int i = 0; i < arg.Length; i++)
			{
				int num = mapString.IndexOf(arg[i]);
				if (num < 0)
				{
					stringBuilder.Append(arg[i]);
				}
				else if (num < transString.Length)
				{
					stringBuilder.Append(transString[num]);
				}
			}
			return stringBuilder.ToString();
		}

		public static bool Lang(string value, XPathNavigator context)
		{
			string xmlLang = context.XmlLang;
			return xmlLang.StartsWith(value, StringComparison.OrdinalIgnoreCase) && (xmlLang.Length == value.Length || xmlLang[value.Length] == '-');
		}

		public static double Round(double value)
		{
			double num = Math.Round(value);
			if (value - num != 0.5)
			{
				return num;
			}
			return num + 1.0;
		}

		public static XPathItem SystemProperty(XmlQualifiedName name)
		{
			if (name.Namespace == "http://www.w3.org/1999/XSL/Transform")
			{
				string name2 = name.Name;
				if (name2 == "version")
				{
					return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Double), 1.0);
				}
				if (name2 == "vendor")
				{
					return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String), "Microsoft");
				}
				if (name2 == "vendor-url")
				{
					return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String), "http://www.microsoft.com");
				}
			}
			else if (name.Namespace == "urn:schemas-microsoft-com:xslt" && name.Name == "version")
			{
				return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String), typeof(XsltLibrary).Assembly.ImageRuntimeVersion);
			}
			return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String), string.Empty);
		}

		public static string BaseUri(XPathNavigator navigator)
		{
			return navigator.BaseURI;
		}

		public static string OuterXml(XPathNavigator navigator)
		{
			RtfNavigator rtfNavigator = navigator as RtfNavigator;
			if (rtfNavigator == null)
			{
				return navigator.OuterXml;
			}
			StringBuilder stringBuilder = new StringBuilder();
			XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings
			{
				OmitXmlDeclaration = true,
				ConformanceLevel = ConformanceLevel.Fragment,
				CheckCharacters = false
			});
			rtfNavigator.CopyToWriter(xmlWriter);
			xmlWriter.Close();
			return stringBuilder.ToString();
		}

		public static string EXslObjectType(IList<XPathItem> value)
		{
			if (value.Count != 1)
			{
				return "node-set";
			}
			XPathItem xpathItem = value[0];
			if (xpathItem is RtfNavigator)
			{
				return "RTF";
			}
			if (xpathItem.IsNode)
			{
				return "node-set";
			}
			object typedValue = xpathItem.TypedValue;
			if (typedValue is string)
			{
				return "string";
			}
			if (typedValue is double)
			{
				return "number";
			}
			if (typedValue is bool)
			{
				return "boolean";
			}
			return "external";
		}

		public static double MSNumber(IList<XPathItem> value)
		{
			if (value.Count == 0)
			{
				return double.NaN;
			}
			XPathItem xpathItem = value[0];
			string text;
			if (xpathItem.IsNode)
			{
				text = xpathItem.Value;
			}
			else
			{
				Type valueType = xpathItem.ValueType;
				if (valueType == XsltConvert.StringType)
				{
					text = xpathItem.Value;
				}
				else
				{
					if (valueType == XsltConvert.DoubleType)
					{
						return xpathItem.ValueAsDouble;
					}
					if (!xpathItem.ValueAsBoolean)
					{
						return 0.0;
					}
					return 1.0;
				}
			}
			double naN;
			if (XmlConvert.TryToDouble(text, out naN) != null)
			{
				naN = double.NaN;
			}
			return naN;
		}

		public static string MSFormatDateTime(string dateTime, string format, string lang, bool isDate)
		{
			string text;
			try
			{
				XsdDateTime xsdDateTime;
				if (!XsdDateTime.TryParse(dateTime, XsdDateTimeFlags.DateTime | XsdDateTimeFlags.Time | XsdDateTimeFlags.Date | XsdDateTimeFlags.GYearMonth | XsdDateTimeFlags.GYear | XsdDateTimeFlags.GMonthDay | XsdDateTimeFlags.GDay | XsdDateTimeFlags.GMonth | XsdDateTimeFlags.XdrDateTime | XsdDateTimeFlags.XdrTimeNoTz, out xsdDateTime))
				{
					text = string.Empty;
				}
				else
				{
					string name = XsltFunctions.GetCultureInfo(lang).Name;
					DateTime dateTime2 = xsdDateTime.ToZulu();
					if (format.Length == 0)
					{
						format = null;
					}
					text = dateTime2.ToString(format, new CultureInfo(name));
				}
			}
			catch (ArgumentException)
			{
				text = string.Empty;
			}
			return text;
		}

		public static double MSStringCompare(string s1, string s2, string lang, string options)
		{
			CultureInfo cultureInfo = XsltFunctions.GetCultureInfo(lang);
			CompareOptions compareOptions = CompareOptions.None;
			bool flag = false;
			foreach (char c in options)
			{
				if (c != 'i')
				{
					if (c != 'u')
					{
						flag = true;
						compareOptions = CompareOptions.IgnoreCase;
					}
					else
					{
						flag = true;
					}
				}
				else
				{
					compareOptions = CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth;
				}
			}
			if (flag)
			{
				if (compareOptions != CompareOptions.None)
				{
					throw new XslTransformException("String comparison option(s) '{0}' are either invalid or cannot be used together.", new string[] { options });
				}
				compareOptions = CompareOptions.IgnoreCase;
			}
			int num = cultureInfo.CompareInfo.Compare(s1, s2, compareOptions);
			if (flag && num == 0)
			{
				num = -cultureInfo.CompareInfo.Compare(s1, s2, CompareOptions.None);
			}
			return (double)num;
		}

		public static string MSUtc(string dateTime)
		{
			XsdDateTime xsdDateTime;
			DateTime dateTime2;
			try
			{
				if (!XsdDateTime.TryParse(dateTime, XsdDateTimeFlags.DateTime | XsdDateTimeFlags.Time | XsdDateTimeFlags.Date | XsdDateTimeFlags.GYearMonth | XsdDateTimeFlags.GYear | XsdDateTimeFlags.GMonthDay | XsdDateTimeFlags.GDay | XsdDateTimeFlags.GMonth | XsdDateTimeFlags.XdrDateTime | XsdDateTimeFlags.XdrTimeNoTz, out xsdDateTime))
				{
					return string.Empty;
				}
				dateTime2 = xsdDateTime.ToZulu();
			}
			catch (ArgumentException)
			{
				return string.Empty;
			}
			char[] array = "----------T00:00:00.000".ToCharArray();
			switch (xsdDateTime.TypeCode)
			{
			case XmlTypeCode.DateTime:
				XsltFunctions.PrintDate(array, dateTime2);
				XsltFunctions.PrintTime(array, dateTime2);
				break;
			case XmlTypeCode.Time:
				XsltFunctions.PrintTime(array, dateTime2);
				break;
			case XmlTypeCode.Date:
				XsltFunctions.PrintDate(array, dateTime2);
				break;
			case XmlTypeCode.GYearMonth:
				XsltFunctions.PrintYear(array, dateTime2.Year);
				XsltFunctions.ShortToCharArray(array, 5, dateTime2.Month);
				break;
			case XmlTypeCode.GYear:
				XsltFunctions.PrintYear(array, dateTime2.Year);
				break;
			case XmlTypeCode.GMonthDay:
				XsltFunctions.ShortToCharArray(array, 5, dateTime2.Month);
				XsltFunctions.ShortToCharArray(array, 8, dateTime2.Day);
				break;
			case XmlTypeCode.GDay:
				XsltFunctions.ShortToCharArray(array, 8, dateTime2.Day);
				break;
			case XmlTypeCode.GMonth:
				XsltFunctions.ShortToCharArray(array, 5, dateTime2.Month);
				break;
			}
			return new string(array);
		}

		public static string MSLocalName(string name)
		{
			int num;
			if (ValidateNames.ParseQName(name, 0, out num) != name.Length)
			{
				return string.Empty;
			}
			if (num == 0)
			{
				return name;
			}
			return name.Substring(num + 1);
		}

		public static string MSNamespaceUri(string name, XPathNavigator currentNode)
		{
			int num;
			if (ValidateNames.ParseQName(name, 0, out num) != name.Length)
			{
				return string.Empty;
			}
			string text = name.Substring(0, num);
			if (text == "xmlns")
			{
				return string.Empty;
			}
			string text2 = currentNode.LookupNamespace(text);
			if (text2 != null)
			{
				return text2;
			}
			if (text == "xml")
			{
				return "http://www.w3.org/XML/1998/namespace";
			}
			return string.Empty;
		}

		private static CultureInfo GetCultureInfo(string lang)
		{
			if (lang.Length == 0)
			{
				return CultureInfo.CurrentCulture;
			}
			CultureInfo cultureInfo;
			try
			{
				cultureInfo = new CultureInfo(lang);
			}
			catch (ArgumentException)
			{
				throw new XslTransformException("'{0}' is not a supported language identifier.", new string[] { lang });
			}
			return cultureInfo;
		}

		private static void PrintDate(char[] text, DateTime dt)
		{
			XsltFunctions.PrintYear(text, dt.Year);
			XsltFunctions.ShortToCharArray(text, 5, dt.Month);
			XsltFunctions.ShortToCharArray(text, 8, dt.Day);
		}

		private static void PrintTime(char[] text, DateTime dt)
		{
			XsltFunctions.ShortToCharArray(text, 11, dt.Hour);
			XsltFunctions.ShortToCharArray(text, 14, dt.Minute);
			XsltFunctions.ShortToCharArray(text, 17, dt.Second);
			XsltFunctions.PrintMsec(text, dt.Millisecond);
		}

		private static void PrintYear(char[] text, int value)
		{
			text[0] = (char)(value / 1000 % 10 + 48);
			text[1] = (char)(value / 100 % 10 + 48);
			text[2] = (char)(value / 10 % 10 + 48);
			text[3] = (char)(value / 1 % 10 + 48);
		}

		private static void PrintMsec(char[] text, int value)
		{
			if (value == 0)
			{
				return;
			}
			text[20] = (char)(value / 100 % 10 + 48);
			text[21] = (char)(value / 10 % 10 + 48);
			text[22] = (char)(value / 1 % 10 + 48);
		}

		private static void ShortToCharArray(char[] text, int start, int value)
		{
			text[start] = (char)(value / 10 + 48);
			text[start + 1] = (char)(value % 10 + 48);
		}

		private static readonly CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;
	}
}
