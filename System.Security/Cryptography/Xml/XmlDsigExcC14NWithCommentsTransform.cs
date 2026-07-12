using System;

namespace System.Security.Cryptography.Xml
{
	public class XmlDsigExcC14NWithCommentsTransform : XmlDsigExcC14NTransform
	{
		public XmlDsigExcC14NWithCommentsTransform()
			: base(true)
		{
			base.Algorithm = "http://www.w3.org/2001/10/xml-exc-c14n#WithComments";
		}

		public XmlDsigExcC14NWithCommentsTransform(string inclusiveNamespacesPrefixList)
			: base(true, inclusiveNamespacesPrefixList)
		{
			base.Algorithm = "http://www.w3.org/2001/10/xml-exc-c14n#WithComments";
		}
	}
}
