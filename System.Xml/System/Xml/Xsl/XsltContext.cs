using System;
using System.Xml.XPath;

namespace System.Xml.Xsl
{
	public abstract class XsltContext : XmlNamespaceManager
	{
		protected XsltContext(NameTable table)
			: base(table)
		{
		}

		protected XsltContext()
			: base(new NameTable())
		{
		}

		internal XsltContext(bool dummy)
		{
		}

		public abstract IXsltContextVariable ResolveVariable(string prefix, string name);

		public abstract IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] ArgTypes);

		public abstract bool Whitespace { get; }

		public abstract bool PreserveWhitespace(XPathNavigator node);

		public abstract int CompareDocument(string baseUri, string nextbaseUri);
	}
}
