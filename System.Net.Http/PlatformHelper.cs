using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading;

namespace System.Net.Http
{
	internal static class PlatformHelper
	{
		internal static bool IsContentHeader(string name)
		{
			return HttpHeaders.GetKnownHeaderKind(name) == HttpHeaderKind.Content;
		}

		internal static string GetSingleHeaderString(string name, IEnumerable<string> values)
		{
			return HttpHeaders.GetSingleHeaderString(name, values);
		}

		internal static StreamContent CreateStreamContent(Stream stream, CancellationToken cancellationToken)
		{
			return new StreamContent(stream, cancellationToken);
		}
	}
}
