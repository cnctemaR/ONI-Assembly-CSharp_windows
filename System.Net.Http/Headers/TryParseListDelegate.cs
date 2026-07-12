using System;
using System.Collections.Generic;

namespace System.Net.Http.Headers
{
	internal delegate bool TryParseListDelegate<T>(string value, int minimalCount, out List<T> result);
}
