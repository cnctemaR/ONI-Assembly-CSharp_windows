using System;

namespace System
{
	internal class DefaultUriParser : global::System.UriParser
	{
		public DefaultUriParser()
		{
		}

		public DefaultUriParser(string scheme)
		{
			this.scheme_name = scheme;
		}
	}
}
