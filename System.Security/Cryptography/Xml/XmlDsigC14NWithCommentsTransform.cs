using System;

namespace System.Security.Cryptography.Xml
{
	public class XmlDsigC14NWithCommentsTransform : XmlDsigC14NTransform
	{
		public XmlDsigC14NWithCommentsTransform()
			: base(true)
		{
			base.Algorithm = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments";
		}
	}
}
