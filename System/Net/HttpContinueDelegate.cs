using System;

namespace System.Net
{
	public delegate void HttpContinueDelegate(int StatusCode, WebHeaderCollection httpHeaders);
}
