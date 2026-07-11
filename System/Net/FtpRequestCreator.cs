using System;

namespace System.Net
{
	internal class FtpRequestCreator : IWebRequestCreate
	{
		public WebRequest Create(global::System.Uri uri)
		{
			return new FtpWebRequest(uri);
		}
	}
}
