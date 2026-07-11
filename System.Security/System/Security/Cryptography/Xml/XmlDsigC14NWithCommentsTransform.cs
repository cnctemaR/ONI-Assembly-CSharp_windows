using System;

namespace System.Security.Cryptography.Xml
{
	public class XmlDsigC14NWithCommentsTransform : XmlDsigC14NTransform
	{
		public XmlDsigC14NWithCommentsTransform()
			: base(true)
		{
		}
	}
}
