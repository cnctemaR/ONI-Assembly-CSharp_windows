using System;

namespace System.Net.Cache
{
	public enum HttpCacheAgeControl
	{
		None,
		MinFresh,
		MaxAge,
		MaxStale = 4,
		MaxAgeAndMinFresh = 3,
		MaxAgeAndMaxStale = 6
	}
}
