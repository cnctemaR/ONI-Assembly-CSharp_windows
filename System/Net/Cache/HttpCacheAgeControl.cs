using System;

namespace System.Net.Cache
{
	public enum HttpCacheAgeControl
	{
		None,
		MinFresh,
		MaxAge,
		MaxAgeAndMinFresh,
		MaxStale,
		MaxAgeAndMaxStale = 6
	}
}
