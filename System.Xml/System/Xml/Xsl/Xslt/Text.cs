using System;
using System.Xml.Xsl.Qil;

namespace System.Xml.Xsl.Xslt
{
	internal class Text : XslNode
	{
		public Text(string data, SerializationHints hints, XslVersion xslVer)
			: base(XslNodeType.Text, null, data, xslVer)
		{
			this.Hints = hints;
		}

		public readonly SerializationHints Hints;
	}
}
