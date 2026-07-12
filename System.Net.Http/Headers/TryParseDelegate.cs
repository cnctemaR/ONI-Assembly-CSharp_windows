using System;

namespace System.Net.Http.Headers
{
	internal delegate bool TryParseDelegate<T>(string value, out T result);
}
