using System;

namespace System.Net
{
	internal class HttpRequestCreator : IWebRequestCreate
	{
		internal HttpRequestCreator()
		{
		}

		public WebRequest Create(global::System.Uri uri)
		{
			return new HttpWebRequest(uri);
		}
	}
}
