using System;

namespace System.Xml.Linq
{
	internal class BaseUriAnnotation
	{
		public BaseUriAnnotation(string baseUri)
		{
			this.baseUri = baseUri;
		}

		internal string baseUri;
	}
}
