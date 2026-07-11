using System;

namespace System.Xml.Xsl.Xslt
{
	internal class NsAlias
	{
		public NsAlias(string resultNsUri, string resultPrefix, int importPrecedence)
		{
			this.ResultNsUri = resultNsUri;
			this.ResultPrefix = resultPrefix;
			this.ImportPrecedence = importPrecedence;
		}

		public readonly string ResultNsUri;

		public readonly string ResultPrefix;

		public readonly int ImportPrecedence;
	}
}
