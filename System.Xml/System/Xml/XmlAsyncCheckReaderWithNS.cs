using System;
using System.Collections.Generic;

namespace System.Xml
{
	internal class XmlAsyncCheckReaderWithNS : XmlAsyncCheckReader, IXmlNamespaceResolver
	{
		public XmlAsyncCheckReaderWithNS(XmlReader reader)
			: base(reader)
		{
			this.readerAsIXmlNamespaceResolver = (IXmlNamespaceResolver)reader;
		}

		IDictionary<string, string> IXmlNamespaceResolver.GetNamespacesInScope(XmlNamespaceScope scope)
		{
			return this.readerAsIXmlNamespaceResolver.GetNamespacesInScope(scope);
		}

		string IXmlNamespaceResolver.LookupNamespace(string prefix)
		{
			return this.readerAsIXmlNamespaceResolver.LookupNamespace(prefix);
		}

		string IXmlNamespaceResolver.LookupPrefix(string namespaceName)
		{
			return this.readerAsIXmlNamespaceResolver.LookupPrefix(namespaceName);
		}

		private readonly IXmlNamespaceResolver readerAsIXmlNamespaceResolver;
	}
}
