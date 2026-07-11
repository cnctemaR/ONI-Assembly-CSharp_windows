using System;
using System.Collections.Generic;
using System.Reflection;

namespace System.Xml.Xsl.Runtime
{
	internal class XmlExtensionFunctionTable
	{
		public XmlExtensionFunctionTable()
		{
			this.table = new Dictionary<XmlExtensionFunction, XmlExtensionFunction>();
		}

		public XmlExtensionFunction Bind(string name, string namespaceUri, int numArgs, Type objectType, BindingFlags flags)
		{
			if (this.funcCached == null)
			{
				this.funcCached = new XmlExtensionFunction();
			}
			this.funcCached.Init(name, namespaceUri, numArgs, objectType, flags);
			XmlExtensionFunction xmlExtensionFunction;
			if (!this.table.TryGetValue(this.funcCached, out xmlExtensionFunction))
			{
				xmlExtensionFunction = this.funcCached;
				this.funcCached = null;
				xmlExtensionFunction.Bind();
				this.table.Add(xmlExtensionFunction, xmlExtensionFunction);
			}
			return xmlExtensionFunction;
		}

		private Dictionary<XmlExtensionFunction, XmlExtensionFunction> table;

		private XmlExtensionFunction funcCached;
	}
}
