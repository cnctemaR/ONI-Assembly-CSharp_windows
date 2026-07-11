using System;

namespace Mono.Xml.Xsl
{
	internal struct Attribute
	{
		public Attribute(string prefix, string namespaceUri, string localName, string value)
		{
			this.Prefix = prefix;
			this.Namespace = namespaceUri;
			this.LocalName = localName;
			this.Value = value;
		}

		public string Prefix;

		public string Namespace;

		public string LocalName;

		public string Value;
	}
}
