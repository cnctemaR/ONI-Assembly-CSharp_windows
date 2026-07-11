using System;

namespace System.Xml.Xsl.Xslt
{
	internal class NsDecl
	{
		public NsDecl(NsDecl prev, string prefix, string nsUri)
		{
			this.Prev = prev;
			this.Prefix = prefix;
			this.NsUri = nsUri;
		}

		public readonly NsDecl Prev;

		public readonly string Prefix;

		public readonly string NsUri;
	}
}
