using System;

namespace System.Xml.Xsl.XsltOld
{
	internal class NamespaceInfo
	{
		internal NamespaceInfo(string prefix, string nameSpace, int stylesheetId)
		{
			this.prefix = prefix;
			this.nameSpace = nameSpace;
			this.stylesheetId = stylesheetId;
		}

		internal string prefix;

		internal string nameSpace;

		internal int stylesheetId;
	}
}
