using System;

namespace System.Net
{
	internal class FileWebRequestCreator : IWebRequestCreate
	{
		internal FileWebRequestCreator()
		{
		}

		public WebRequest Create(global::System.Uri uri)
		{
			return new FileWebRequest(uri);
		}
	}
}
