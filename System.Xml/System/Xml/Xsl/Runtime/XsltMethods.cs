using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	internal static class XsltMethods
	{
		public static MethodInfo GetMethod(Type className, string methName)
		{
			return className.GetMethod(methName);
		}

		public static MethodInfo GetMethod(Type className, string methName, params Type[] args)
		{
			return className.GetMethod(methName, args);
		}

		public static readonly MethodInfo FormatMessage = XsltMethods.GetMethod(typeof(XsltLibrary), "FormatMessage");

		public static readonly MethodInfo EnsureNodeSet = XsltMethods.GetMethod(typeof(XsltConvert), "EnsureNodeSet", new Type[] { typeof(IList<XPathItem>) });

		public static readonly MethodInfo EqualityOperator = XsltMethods.GetMethod(typeof(XsltLibrary), "EqualityOperator");

		public static readonly MethodInfo RelationalOperator = XsltMethods.GetMethod(typeof(XsltLibrary), "RelationalOperator");

		public static readonly MethodInfo StartsWith = XsltMethods.GetMethod(typeof(XsltFunctions), "StartsWith");

		public static readonly MethodInfo Contains = XsltMethods.GetMethod(typeof(XsltFunctions), "Contains");

		public static readonly MethodInfo SubstringBefore = XsltMethods.GetMethod(typeof(XsltFunctions), "SubstringBefore");

		public static readonly MethodInfo SubstringAfter = XsltMethods.GetMethod(typeof(XsltFunctions), "SubstringAfter");

		public static readonly MethodInfo Substring2 = XsltMethods.GetMethod(typeof(XsltFunctions), "Substring", new Type[]
		{
			typeof(string),
			typeof(double)
		});

		public static readonly MethodInfo Substring3 = XsltMethods.GetMethod(typeof(XsltFunctions), "Substring", new Type[]
		{
			typeof(string),
			typeof(double),
			typeof(double)
		});

		public static readonly MethodInfo NormalizeSpace = XsltMethods.GetMethod(typeof(XsltFunctions), "NormalizeSpace");

		public static readonly MethodInfo Translate = XsltMethods.GetMethod(typeof(XsltFunctions), "Translate");

		public static readonly MethodInfo Lang = XsltMethods.GetMethod(typeof(XsltFunctions), "Lang");

		public static readonly MethodInfo Floor = XsltMethods.GetMethod(typeof(Math), "Floor", new Type[] { typeof(double) });

		public static readonly MethodInfo Ceiling = XsltMethods.GetMethod(typeof(Math), "Ceiling", new Type[] { typeof(double) });

		public static readonly MethodInfo Round = XsltMethods.GetMethod(typeof(XsltFunctions), "Round");

		public static readonly MethodInfo SystemProperty = XsltMethods.GetMethod(typeof(XsltFunctions), "SystemProperty");

		public static readonly MethodInfo BaseUri = XsltMethods.GetMethod(typeof(XsltFunctions), "BaseUri");

		public static readonly MethodInfo OuterXml = XsltMethods.GetMethod(typeof(XsltFunctions), "OuterXml");

		public static readonly MethodInfo OnCurrentNodeChanged = XsltMethods.GetMethod(typeof(XmlQueryRuntime), "OnCurrentNodeChanged");

		public static readonly MethodInfo MSFormatDateTime = XsltMethods.GetMethod(typeof(XsltFunctions), "MSFormatDateTime");

		public static readonly MethodInfo MSStringCompare = XsltMethods.GetMethod(typeof(XsltFunctions), "MSStringCompare");

		public static readonly MethodInfo MSUtc = XsltMethods.GetMethod(typeof(XsltFunctions), "MSUtc");

		public static readonly MethodInfo MSNumber = XsltMethods.GetMethod(typeof(XsltFunctions), "MSNumber");

		public static readonly MethodInfo MSLocalName = XsltMethods.GetMethod(typeof(XsltFunctions), "MSLocalName");

		public static readonly MethodInfo MSNamespaceUri = XsltMethods.GetMethod(typeof(XsltFunctions), "MSNamespaceUri");

		public static readonly MethodInfo EXslObjectType = XsltMethods.GetMethod(typeof(XsltFunctions), "EXslObjectType");

		public static readonly MethodInfo CheckScriptNamespace = XsltMethods.GetMethod(typeof(XsltLibrary), "CheckScriptNamespace");

		public static readonly MethodInfo FunctionAvailable = XsltMethods.GetMethod(typeof(XsltLibrary), "FunctionAvailable");

		public static readonly MethodInfo ElementAvailable = XsltMethods.GetMethod(typeof(XsltLibrary), "ElementAvailable");

		public static readonly MethodInfo RegisterDecimalFormat = XsltMethods.GetMethod(typeof(XsltLibrary), "RegisterDecimalFormat");

		public static readonly MethodInfo RegisterDecimalFormatter = XsltMethods.GetMethod(typeof(XsltLibrary), "RegisterDecimalFormatter");

		public static readonly MethodInfo FormatNumberStatic = XsltMethods.GetMethod(typeof(XsltLibrary), "FormatNumberStatic");

		public static readonly MethodInfo FormatNumberDynamic = XsltMethods.GetMethod(typeof(XsltLibrary), "FormatNumberDynamic");

		public static readonly MethodInfo IsSameNodeSort = XsltMethods.GetMethod(typeof(XsltLibrary), "IsSameNodeSort");

		public static readonly MethodInfo LangToLcid = XsltMethods.GetMethod(typeof(XsltLibrary), "LangToLcid");

		public static readonly MethodInfo NumberFormat = XsltMethods.GetMethod(typeof(XsltLibrary), "NumberFormat");
	}
}
