using System;

namespace System.Net
{
	public interface IWebRequestCreate
	{
		WebRequest Create(global::System.Uri uri);
	}
}
