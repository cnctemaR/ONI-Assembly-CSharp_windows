using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Security;
using System.Xml.XPath;
using System.Xml.Xsl.Runtime;
using MS.Internal.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal class XsltCompileContext : XsltContext
	{
		internal XsltCompileContext(InputScopeManager manager, Processor processor)
			: base(false)
		{
			this.manager = manager;
			this.processor = processor;
		}

		internal XsltCompileContext()
			: base(false)
		{
		}

		internal void Recycle()
		{
			this.manager = null;
			this.processor = null;
		}

		internal void Reinitialize(InputScopeManager manager, Processor processor)
		{
			this.manager = manager;
			this.processor = processor;
		}

		public override int CompareDocument(string baseUri, string nextbaseUri)
		{
			return string.Compare(baseUri, nextbaseUri, StringComparison.Ordinal);
		}

		public override string DefaultNamespace
		{
			get
			{
				return string.Empty;
			}
		}

		public override string LookupNamespace(string prefix)
		{
			return this.manager.ResolveXPathNamespace(prefix);
		}

		public override IXsltContextVariable ResolveVariable(string prefix, string name)
		{
			string text = this.LookupNamespace(prefix);
			XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(name, text);
			IXsltContextVariable xsltContextVariable = this.manager.VariableScope.ResolveVariable(xmlQualifiedName);
			if (xsltContextVariable == null)
			{
				throw XsltException.Create("The variable or parameter '{0}' is either not defined or it is out of scope.", new string[] { xmlQualifiedName.ToString() });
			}
			return xsltContextVariable;
		}

		internal object EvaluateVariable(VariableAction variable)
		{
			object obj = this.processor.GetVariableValue(variable);
			if (obj == null && !variable.IsGlobal)
			{
				VariableAction variableAction = this.manager.VariableScope.ResolveGlobalVariable(variable.Name);
				if (variableAction != null)
				{
					obj = this.processor.GetVariableValue(variableAction);
				}
			}
			if (obj == null)
			{
				throw XsltException.Create("The variable or parameter '{0}' is either not defined or it is out of scope.", new string[] { variable.Name.ToString() });
			}
			return obj;
		}

		public override bool Whitespace
		{
			get
			{
				return this.processor.Stylesheet.Whitespace;
			}
		}

		public override bool PreserveWhitespace(XPathNavigator node)
		{
			node = node.Clone();
			node.MoveToParent();
			return this.processor.Stylesheet.PreserveWhiteSpace(this.processor, node);
		}

		private MethodInfo FindBestMethod(MethodInfo[] methods, bool ignoreCase, bool publicOnly, string name, XPathResultType[] argTypes)
		{
			int num = methods.Length;
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				if (string.Compare(name, methods[i].Name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) == 0 && (!publicOnly || methods[i].GetBaseDefinition().IsPublic))
				{
					methods[num2++] = methods[i];
				}
			}
			num = num2;
			if (num == 0)
			{
				return null;
			}
			if (argTypes == null)
			{
				return methods[0];
			}
			num2 = 0;
			for (int j = 0; j < num; j++)
			{
				if (methods[j].GetParameters().Length == argTypes.Length)
				{
					methods[num2++] = methods[j];
				}
			}
			num = num2;
			if (num <= 1)
			{
				return methods[0];
			}
			num2 = 0;
			for (int k = 0; k < num; k++)
			{
				bool flag = true;
				ParameterInfo[] parameters = methods[k].GetParameters();
				for (int l = 0; l < parameters.Length; l++)
				{
					XPathResultType xpathResultType = argTypes[l];
					if (xpathResultType != XPathResultType.Any)
					{
						XPathResultType xpathType = XsltCompileContext.GetXPathType(parameters[l].ParameterType);
						if (xpathType != xpathResultType && xpathType != XPathResultType.Any)
						{
							flag = false;
							break;
						}
					}
				}
				if (flag)
				{
					methods[num2++] = methods[k];
				}
			}
			return methods[0];
		}

		private IXsltContextFunction GetExtentionMethod(string ns, string name, XPathResultType[] argTypes, out object extension)
		{
			XsltCompileContext.FuncExtension funcExtension = null;
			extension = this.processor.GetScriptObject(ns);
			if (extension != null)
			{
				MethodInfo methodInfo = this.FindBestMethod(extension.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic), true, false, name, argTypes);
				if (methodInfo != null)
				{
					funcExtension = new XsltCompileContext.FuncExtension(extension, methodInfo, null);
				}
				return funcExtension;
			}
			extension = this.processor.GetExtensionObject(ns);
			if (extension != null)
			{
				MethodInfo methodInfo2 = this.FindBestMethod(extension.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic), false, true, name, argTypes);
				if (methodInfo2 != null)
				{
					funcExtension = new XsltCompileContext.FuncExtension(extension, methodInfo2, this.processor.permissions);
				}
				return funcExtension;
			}
			return null;
		}

		public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] argTypes)
		{
			IXsltContextFunction xsltContextFunction;
			if (prefix.Length == 0)
			{
				xsltContextFunction = XsltCompileContext.s_FunctionTable[name] as IXsltContextFunction;
			}
			else
			{
				string text = this.LookupNamespace(prefix);
				if (text == "urn:schemas-microsoft-com:xslt" && name == "node-set")
				{
					xsltContextFunction = XsltCompileContext.s_FuncNodeSet;
				}
				else
				{
					object obj;
					xsltContextFunction = this.GetExtentionMethod(text, name, argTypes, out obj);
					if (obj == null)
					{
						throw XsltException.Create("Cannot find the script or external object that implements prefix '{0}'.", new string[] { prefix });
					}
				}
			}
			if (xsltContextFunction == null)
			{
				throw XsltException.Create("'{0}()' is an unknown XSLT function.", new string[] { name });
			}
			if (argTypes.Length < xsltContextFunction.Minargs || xsltContextFunction.Maxargs < argTypes.Length)
			{
				throw XsltException.Create("XSLT function '{0}()' has the wrong number of arguments.", new string[]
				{
					name,
					argTypes.Length.ToString(CultureInfo.InvariantCulture)
				});
			}
			return xsltContextFunction;
		}

		private Uri ComposeUri(string thisUri, string baseUri)
		{
			XmlResolver resolver = this.processor.Resolver;
			Uri uri = null;
			if (baseUri.Length != 0)
			{
				uri = resolver.ResolveUri(null, baseUri);
			}
			return resolver.ResolveUri(uri, thisUri);
		}

		private XPathNodeIterator Document(object arg0, string baseUri)
		{
			if (this.processor.permissions != null)
			{
				this.processor.permissions.PermitOnly();
			}
			XPathNodeIterator xpathNodeIterator = arg0 as XPathNodeIterator;
			if (xpathNodeIterator != null)
			{
				ArrayList arrayList = new ArrayList();
				Hashtable hashtable = new Hashtable();
				while (xpathNodeIterator.MoveNext())
				{
					Uri uri = this.ComposeUri(xpathNodeIterator.Current.Value, baseUri ?? xpathNodeIterator.Current.BaseURI);
					if (!hashtable.ContainsKey(uri))
					{
						hashtable.Add(uri, null);
						arrayList.Add(this.processor.GetNavigator(uri));
					}
				}
				return new XPathArrayIterator(arrayList);
			}
			return new XPathSingletonIterator(this.processor.GetNavigator(this.ComposeUri(XmlConvert.ToXPathString(arg0), baseUri ?? this.manager.Navigator.BaseURI)));
		}

		private Hashtable BuildKeyTable(Key key, XPathNavigator root)
		{
			Hashtable hashtable = new Hashtable();
			string queryExpression = this.processor.GetQueryExpression(key.MatchKey);
			Query compiledQuery = this.processor.GetCompiledQuery(key.MatchKey);
			Query compiledQuery2 = this.processor.GetCompiledQuery(key.UseKey);
			XPathNodeIterator xpathNodeIterator = root.SelectDescendants(XPathNodeType.All, false);
			while (xpathNodeIterator.MoveNext())
			{
				XPathNavigator xpathNavigator = xpathNodeIterator.Current;
				XsltCompileContext.EvaluateKey(xpathNavigator, compiledQuery, queryExpression, compiledQuery2, hashtable);
				if (xpathNavigator.MoveToFirstAttribute())
				{
					do
					{
						XsltCompileContext.EvaluateKey(xpathNavigator, compiledQuery, queryExpression, compiledQuery2, hashtable);
					}
					while (xpathNavigator.MoveToNextAttribute());
					xpathNavigator.MoveToParent();
				}
			}
			return hashtable;
		}

		private static void AddKeyValue(Hashtable keyTable, string key, XPathNavigator value, bool checkDuplicates)
		{
			ArrayList arrayList = (ArrayList)keyTable[key];
			if (arrayList == null)
			{
				arrayList = new ArrayList();
				keyTable.Add(key, arrayList);
			}
			else if (checkDuplicates && value.ComparePosition((XPathNavigator)arrayList[arrayList.Count - 1]) == XmlNodeOrder.Same)
			{
				return;
			}
			arrayList.Add(value.Clone());
		}

		private static void EvaluateKey(XPathNavigator node, Query matchExpr, string matchStr, Query useExpr, Hashtable keyTable)
		{
			try
			{
				if (matchExpr.MatchNode(node) == null)
				{
					return;
				}
			}
			catch (XPathException)
			{
				throw XsltException.Create("'{0}' is an invalid XSLT pattern.", new string[] { matchStr });
			}
			object obj = useExpr.Evaluate(new XPathSingletonIterator(node, true));
			XPathNodeIterator xpathNodeIterator = obj as XPathNodeIterator;
			if (xpathNodeIterator != null)
			{
				bool flag = false;
				while (xpathNodeIterator.MoveNext())
				{
					XPathNavigator xpathNavigator = xpathNodeIterator.Current;
					XsltCompileContext.AddKeyValue(keyTable, xpathNavigator.Value, node, flag);
					flag = true;
				}
				return;
			}
			string text = XmlConvert.ToXPathString(obj);
			XsltCompileContext.AddKeyValue(keyTable, text, node, false);
		}

		private DecimalFormat ResolveFormatName(string formatName)
		{
			string text = string.Empty;
			string empty = string.Empty;
			if (formatName != null)
			{
				string text2;
				PrefixQName.ParseQualifiedName(formatName, out text2, out empty);
				text = this.LookupNamespace(text2);
			}
			DecimalFormat decimalFormat = this.processor.RootAction.GetDecimalFormat(new XmlQualifiedName(empty, text));
			if (decimalFormat == null)
			{
				if (formatName != null)
				{
					throw XsltException.Create("Decimal format '{0}' is not defined.", new string[] { formatName });
				}
				decimalFormat = new DecimalFormat(new NumberFormatInfo(), '#', '0', ';');
			}
			return decimalFormat;
		}

		private bool ElementAvailable(string qname)
		{
			string text;
			string text2;
			PrefixQName.ParseQualifiedName(qname, out text, out text2);
			return this.manager.ResolveXmlNamespace(text) == "http://www.w3.org/1999/XSL/Transform" && (text2 == "apply-imports" || text2 == "apply-templates" || text2 == "attribute" || text2 == "call-template" || text2 == "choose" || text2 == "comment" || text2 == "copy" || text2 == "copy-of" || text2 == "element" || text2 == "fallback" || text2 == "for-each" || text2 == "if" || text2 == "message" || text2 == "number" || text2 == "processing-instruction" || text2 == "text" || text2 == "value-of" || text2 == "variable");
		}

		private bool FunctionAvailable(string qname)
		{
			string text;
			string text2;
			PrefixQName.ParseQualifiedName(qname, out text, out text2);
			string text3 = this.LookupNamespace(text);
			if (text3 == "urn:schemas-microsoft-com:xslt")
			{
				return text2 == "node-set";
			}
			if (text3.Length == 0)
			{
				return text2 == "last" || text2 == "position" || text2 == "name" || text2 == "namespace-uri" || text2 == "local-name" || text2 == "count" || text2 == "id" || text2 == "string" || text2 == "concat" || text2 == "starts-with" || text2 == "contains" || text2 == "substring-before" || text2 == "substring-after" || text2 == "substring" || text2 == "string-length" || text2 == "normalize-space" || text2 == "translate" || text2 == "boolean" || text2 == "not" || text2 == "true" || text2 == "false" || text2 == "lang" || text2 == "number" || text2 == "sum" || text2 == "floor" || text2 == "ceiling" || text2 == "round" || (XsltCompileContext.s_FunctionTable[text2] != null && text2 != "unparsed-entity-uri");
			}
			object obj;
			return this.GetExtentionMethod(text3, text2, null, out obj) != null;
		}

		private XPathNodeIterator Current()
		{
			XPathNavigator xpathNavigator = this.processor.Current;
			if (xpathNavigator != null)
			{
				return new XPathSingletonIterator(xpathNavigator.Clone());
			}
			return XPathEmptyIterator.Instance;
		}

		private string SystemProperty(string qname)
		{
			string text = string.Empty;
			string text2;
			string text3;
			PrefixQName.ParseQualifiedName(qname, out text2, out text3);
			string text4 = this.LookupNamespace(text2);
			if (text4 == "http://www.w3.org/1999/XSL/Transform")
			{
				if (text3 == "version")
				{
					text = "1";
				}
				else if (text3 == "vendor")
				{
					text = "Microsoft";
				}
				else if (text3 == "vendor-url")
				{
					text = "http://www.microsoft.com";
				}
				return text;
			}
			if (text4 == null && text2 != null)
			{
				throw XsltException.Create("Prefix '{0}' is not defined.", new string[] { text2 });
			}
			return string.Empty;
		}

		public static XPathResultType GetXPathType(Type type)
		{
			TypeCode typeCode = Type.GetTypeCode(type);
			if (typeCode <= TypeCode.Boolean)
			{
				if (typeCode != TypeCode.Object)
				{
					if (typeCode == TypeCode.Boolean)
					{
						return XPathResultType.Boolean;
					}
				}
				else
				{
					if (typeof(XPathNavigator).IsAssignableFrom(type) || typeof(IXPathNavigable).IsAssignableFrom(type))
					{
						return XPathResultType.String;
					}
					if (typeof(XPathNodeIterator).IsAssignableFrom(type))
					{
						return XPathResultType.NodeSet;
					}
					return XPathResultType.Any;
				}
			}
			else
			{
				if (typeCode == TypeCode.DateTime)
				{
					return XPathResultType.Error;
				}
				if (typeCode == TypeCode.String)
				{
					return XPathResultType.String;
				}
			}
			return XPathResultType.Number;
		}

		private static Hashtable CreateFunctionTable()
		{
			Hashtable hashtable = new Hashtable(10);
			hashtable["current"] = new XsltCompileContext.FuncCurrent();
			hashtable["unparsed-entity-uri"] = new XsltCompileContext.FuncUnEntityUri();
			hashtable["generate-id"] = new XsltCompileContext.FuncGenerateId();
			hashtable["system-property"] = new XsltCompileContext.FuncSystemProp();
			hashtable["element-available"] = new XsltCompileContext.FuncElementAvailable();
			hashtable["function-available"] = new XsltCompileContext.FuncFunctionAvailable();
			hashtable["document"] = new XsltCompileContext.FuncDocument();
			hashtable["key"] = new XsltCompileContext.FuncKey();
			hashtable["format-number"] = new XsltCompileContext.FuncFormatNumber();
			return hashtable;
		}

		private InputScopeManager manager;

		private Processor processor;

		private static Hashtable s_FunctionTable = XsltCompileContext.CreateFunctionTable();

		private static IXsltContextFunction s_FuncNodeSet = new XsltCompileContext.FuncNodeSet();

		private const string f_NodeSet = "node-set";

		private const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		private abstract class XsltFunctionImpl : IXsltContextFunction
		{
			public XsltFunctionImpl()
			{
			}

			public XsltFunctionImpl(int minArgs, int maxArgs, XPathResultType returnType, XPathResultType[] argTypes)
			{
				this.Init(minArgs, maxArgs, returnType, argTypes);
			}

			protected void Init(int minArgs, int maxArgs, XPathResultType returnType, XPathResultType[] argTypes)
			{
				this.minargs = minArgs;
				this.maxargs = maxArgs;
				this.returnType = returnType;
				this.argTypes = argTypes;
			}

			public int Minargs
			{
				get
				{
					return this.minargs;
				}
			}

			public int Maxargs
			{
				get
				{
					return this.maxargs;
				}
			}

			public XPathResultType ReturnType
			{
				get
				{
					return this.returnType;
				}
			}

			public XPathResultType[] ArgTypes
			{
				get
				{
					return this.argTypes;
				}
			}

			public abstract object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext);

			public static XPathNodeIterator ToIterator(object argument)
			{
				XPathNodeIterator xpathNodeIterator = argument as XPathNodeIterator;
				if (xpathNodeIterator == null)
				{
					throw XsltException.Create("Cannot convert the operand to a node-set.", Array.Empty<string>());
				}
				return xpathNodeIterator;
			}

			public static XPathNavigator ToNavigator(object argument)
			{
				XPathNavigator xpathNavigator = argument as XPathNavigator;
				if (xpathNavigator == null)
				{
					throw XsltException.Create("Cannot convert the operand to 'Result tree fragment'.", Array.Empty<string>());
				}
				return xpathNavigator;
			}

			private static string IteratorToString(XPathNodeIterator it)
			{
				if (it.MoveNext())
				{
					return it.Current.Value;
				}
				return string.Empty;
			}

			public static string ToString(object argument)
			{
				XPathNodeIterator xpathNodeIterator = argument as XPathNodeIterator;
				if (xpathNodeIterator != null)
				{
					return XsltCompileContext.XsltFunctionImpl.IteratorToString(xpathNodeIterator);
				}
				return XmlConvert.ToXPathString(argument);
			}

			public static bool ToBoolean(object argument)
			{
				XPathNodeIterator xpathNodeIterator = argument as XPathNodeIterator;
				if (xpathNodeIterator != null)
				{
					return Convert.ToBoolean(XsltCompileContext.XsltFunctionImpl.IteratorToString(xpathNodeIterator), CultureInfo.InvariantCulture);
				}
				XPathNavigator xpathNavigator = argument as XPathNavigator;
				if (xpathNavigator != null)
				{
					return Convert.ToBoolean(xpathNavigator.ToString(), CultureInfo.InvariantCulture);
				}
				return Convert.ToBoolean(argument, CultureInfo.InvariantCulture);
			}

			public static double ToNumber(object argument)
			{
				XPathNodeIterator xpathNodeIterator = argument as XPathNodeIterator;
				if (xpathNodeIterator != null)
				{
					return XmlConvert.ToXPathDouble(XsltCompileContext.XsltFunctionImpl.IteratorToString(xpathNodeIterator));
				}
				XPathNavigator xpathNavigator = argument as XPathNavigator;
				if (xpathNavigator != null)
				{
					return XmlConvert.ToXPathDouble(xpathNavigator.ToString());
				}
				return XmlConvert.ToXPathDouble(argument);
			}

			private static object ToNumeric(object argument, TypeCode typeCode)
			{
				return Convert.ChangeType(XsltCompileContext.XsltFunctionImpl.ToNumber(argument), typeCode, CultureInfo.InvariantCulture);
			}

			public static object ConvertToXPathType(object val, XPathResultType xt, TypeCode typeCode)
			{
				switch (xt)
				{
				case XPathResultType.Number:
					return XsltCompileContext.XsltFunctionImpl.ToNumeric(val, typeCode);
				case XPathResultType.String:
					if (typeCode == TypeCode.String)
					{
						return XsltCompileContext.XsltFunctionImpl.ToString(val);
					}
					return XsltCompileContext.XsltFunctionImpl.ToNavigator(val);
				case XPathResultType.Boolean:
					return XsltCompileContext.XsltFunctionImpl.ToBoolean(val);
				case XPathResultType.NodeSet:
					return XsltCompileContext.XsltFunctionImpl.ToIterator(val);
				case XPathResultType.Any:
				case XPathResultType.Error:
					return val;
				}
				return val;
			}

			private int minargs;

			private int maxargs;

			private XPathResultType returnType;

			private XPathResultType[] argTypes;
		}

		private class FuncCurrent : XsltCompileContext.XsltFunctionImpl
		{
			public FuncCurrent()
				: base(0, 0, XPathResultType.NodeSet, new XPathResultType[0])
			{
			}

			public override object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
			{
				return ((XsltCompileContext)xsltContext).Current();
			}
		}

		private class FuncUnEntityUri : XsltCompileContext.XsltFunctionImpl
		{
			public FuncUnEntityUri()
				: base(1, 1, XPathResultType.String, new XPathResultType[] { XPathResultType.String })
			{
			}

			public override object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
			{
				throw XsltException.Create("'{0}()' is an unsupported XSLT function.", new string[] { "unparsed-entity-uri" });
			}
		}

		private class FuncGenerateId : XsltCompileContext.XsltFunctionImpl
		{
			public FuncGenerateId()
				: base(0, 1, XPathResultType.String, new XPathResultType[] { XPathResultType.NodeSet })
			{
			}

			public override object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
			{
				if (args.Length == 0)
				{
					return docContext.UniqueId;
				}
				XPathNodeIterator xpathNodeIterator = XsltCompileContext.XsltFunctionImpl.ToIterator(args[0]);
				if (xpathNodeIterator.MoveNext())
				{
					return xpathNodeIterator.Current.UniqueId;
				}
				return string.Empty;
			}
		}

		private class FuncSystemProp : XsltCompileContext.XsltFunctionImpl
		{
			public FuncSystemProp()
				: base(1, 1, XPathResultType.String, new XPathResultType[] { XPathResultType.String })
			{
			}

			public override object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
			{
				return ((XsltCompileContext)xsltContext).SystemProperty(XsltCompileContext.XsltFunctionImpl.ToString(args[0]));
			}
		}

		private class FuncElementAvailable : XsltCompileContext.XsltFunctionImpl
		{
			public FuncElementAvailable()
				: base(1, 1, XPathResultType.Boolean, new XPathResultType[] { XPathResultType.String })
			{
			}

			public override object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
			{
				return ((XsltCompileContext)xsltContext).ElementAvailable(XsltCompileContext.XsltFunctionImpl.ToString(args[0]));
			}
		}

		private class FuncFunctionAvailable : XsltCompileContext.XsltFunctionImpl
		{
			public FuncFunctionAvailable()
				: base(1, 1, XPathResultType.Boolean, new XPathResultType[] { XPathResultType.String })
			{
			}

			public override object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
			{
				return ((XsltCompileContext)xsltContext).FunctionAvailable(XsltCompileContext.XsltFunctionImpl.ToString(args[0]));
			}
		}

		private class FuncDocument : XsltCompileContext.XsltFunctionImpl
		{
			public FuncDocument()
				: base(1, 2, XPathResultType.NodeSet, new XPathResultType[]
				{
					XPathResultType.Any,
					XPathResultType.NodeSet
				})
			{
			}

			public override object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
			{
				string text = null;
				if (args.Length == 2)
				{
					XPathNodeIterator xpathNodeIterator = XsltCompileContext.XsltFunctionImpl.ToIterator(args[1]);
					if (xpathNodeIterator.MoveNext())
					{
						text = xpathNodeIterator.Current.BaseURI;
					}
					else
					{
						text = string.Empty;
					}
				}
				object obj;
				try
				{
					obj = ((XsltCompileContext)xsltContext).Document(args[0], text);
				}
				catch (Exception ex)
				{
					if (!XmlException.IsCatchableException(ex))
					{
						throw;
					}
					obj = XPathEmptyIterator.Instance;
				}
				return obj;
			}
		}

		private class FuncKey : XsltCompileContext.XsltFunctionImpl
		{
			public FuncKey()
				: base(2, 2, XPathResultType.NodeSet, new XPathResultType[]
				{
					XPathResultType.String,
					XPathResultType.Any
				})
			{
			}

			public override object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
			{
				XsltCompileContext xsltCompileContext = (XsltCompileContext)xsltContext;
				string text;
				string text2;
				PrefixQName.ParseQualifiedName(XsltCompileContext.XsltFunctionImpl.ToString(args[0]), out text, out text2);
				string text3 = xsltContext.LookupNamespace(text);
				XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(text2, text3);
				XPathNavigator xpathNavigator = docContext.Clone();
				xpathNavigator.MoveToRoot();
				ArrayList arrayList = null;
				foreach (Key key in xsltCompileContext.processor.KeyList)
				{
					if (key.Name == xmlQualifiedName)
					{
						Hashtable hashtable = key.GetKeys(xpathNavigator);
						if (hashtable == null)
						{
							hashtable = xsltCompileContext.BuildKeyTable(key, xpathNavigator);
							key.AddKey(xpathNavigator, hashtable);
						}
						XPathNodeIterator xpathNodeIterator = args[1] as XPathNodeIterator;
						if (xpathNodeIterator != null)
						{
							xpathNodeIterator = xpathNodeIterator.Clone();
							while (xpathNodeIterator.MoveNext())
							{
								XPathNavigator xpathNavigator2 = xpathNodeIterator.Current;
								arrayList = XsltCompileContext.FuncKey.AddToList(arrayList, (ArrayList)hashtable[xpathNavigator2.Value]);
							}
						}
						else
						{
							arrayList = XsltCompileContext.FuncKey.AddToList(arrayList, (ArrayList)hashtable[XsltCompileContext.XsltFunctionImpl.ToString(args[1])]);
						}
					}
				}
				if (arrayList == null)
				{
					return XPathEmptyIterator.Instance;
				}
				if (arrayList[0] is XPathNavigator)
				{
					return new XPathArrayIterator(arrayList);
				}
				return new XPathMultyIterator(arrayList);
			}

			private static ArrayList AddToList(ArrayList resultCollection, ArrayList newList)
			{
				if (newList == null)
				{
					return resultCollection;
				}
				if (resultCollection == null)
				{
					return newList;
				}
				if (!(resultCollection[0] is ArrayList))
				{
					ArrayList arrayList = resultCollection;
					resultCollection = new ArrayList();
					resultCollection.Add(arrayList);
				}
				resultCollection.Add(newList);
				return resultCollection;
			}
		}

		private class FuncFormatNumber : XsltCompileContext.XsltFunctionImpl
		{
			public FuncFormatNumber()
				: base(2, 3, XPathResultType.String, new XPathResultType[]
				{
					XPathResultType.Number,
					XPathResultType.String,
					XPathResultType.String
				})
			{
			}

			public override object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
			{
				DecimalFormat decimalFormat = ((XsltCompileContext)xsltContext).ResolveFormatName((args.Length == 3) ? XsltCompileContext.XsltFunctionImpl.ToString(args[2]) : null);
				return DecimalFormatter.Format(XsltCompileContext.XsltFunctionImpl.ToNumber(args[0]), XsltCompileContext.XsltFunctionImpl.ToString(args[1]), decimalFormat);
			}
		}

		private class FuncNodeSet : XsltCompileContext.XsltFunctionImpl
		{
			public FuncNodeSet()
				: base(1, 1, XPathResultType.NodeSet, new XPathResultType[] { XPathResultType.String })
			{
			}

			public override object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
			{
				return new XPathSingletonIterator(XsltCompileContext.XsltFunctionImpl.ToNavigator(args[0]));
			}
		}

		private class FuncExtension : XsltCompileContext.XsltFunctionImpl
		{
			public FuncExtension(object extension, MethodInfo method, PermissionSet permissions)
			{
				this.extension = extension;
				this.method = method;
				this.permissions = permissions;
				XPathResultType xpathType = XsltCompileContext.GetXPathType(method.ReturnType);
				ParameterInfo[] parameters = method.GetParameters();
				int num = parameters.Length;
				int num2 = parameters.Length;
				this.typeCodes = new TypeCode[parameters.Length];
				XPathResultType[] array = new XPathResultType[parameters.Length];
				bool flag = true;
				int num3 = parameters.Length - 1;
				while (0 <= num3)
				{
					this.typeCodes[num3] = Type.GetTypeCode(parameters[num3].ParameterType);
					array[num3] = XsltCompileContext.GetXPathType(parameters[num3].ParameterType);
					if (flag)
					{
						if (parameters[num3].IsOptional)
						{
							num--;
						}
						else
						{
							flag = false;
						}
					}
					num3--;
				}
				base.Init(num, num2, xpathType, array);
			}

			public override object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
			{
				int num = args.Length - 1;
				while (0 <= num)
				{
					args[num] = XsltCompileContext.XsltFunctionImpl.ConvertToXPathType(args[num], base.ArgTypes[num], this.typeCodes[num]);
					num--;
				}
				if (this.permissions != null)
				{
					this.permissions.PermitOnly();
				}
				return this.method.Invoke(this.extension, args);
			}

			private object extension;

			private MethodInfo method;

			private TypeCode[] typeCodes;

			private PermissionSet permissions;
		}
	}
}
