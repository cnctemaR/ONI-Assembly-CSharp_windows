using System;
using System.Security;

namespace System.IO
{
	internal abstract class SearchResultHandler<TSource>
	{
		[SecurityCritical]
		internal abstract bool IsResultIncluded(SearchResult result);

		[SecurityCritical]
		internal abstract TSource CreateObject(SearchResult result);
	}
}
