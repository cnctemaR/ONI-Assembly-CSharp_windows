using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[Flags]
	public enum ADVF
	{
		ADVF_NODATA = 1,
		ADVF_PRIMEFIRST = 2,
		ADVF_ONLYONCE = 4,
		ADVFCACHE_NOHANDLER = 8,
		ADVFCACHE_FORCEBUILTIN = 16,
		ADVFCACHE_ONSAVE = 32,
		ADVF_DATAONSTOP = 64
	}
}
