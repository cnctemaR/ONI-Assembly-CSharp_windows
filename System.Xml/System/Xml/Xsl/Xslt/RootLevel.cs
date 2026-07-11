using System;

namespace System.Xml.Xsl.Xslt
{
	internal class RootLevel : StylesheetLevel
	{
		public RootLevel(Stylesheet principal)
		{
			this.Imports = new Stylesheet[] { principal };
		}
	}
}
