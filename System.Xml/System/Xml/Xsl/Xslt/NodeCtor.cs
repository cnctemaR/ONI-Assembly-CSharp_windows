using System;

namespace System.Xml.Xsl.Xslt
{
	internal class NodeCtor : XslNode
	{
		public NodeCtor(XslNodeType nt, string nameAvt, string nsAvt, XslVersion xslVer)
			: base(nt, null, null, xslVer)
		{
			this.NameAvt = nameAvt;
			this.NsAvt = nsAvt;
		}

		public readonly string NameAvt;

		public readonly string NsAvt;
	}
}
