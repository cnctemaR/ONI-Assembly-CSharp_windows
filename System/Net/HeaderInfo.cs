using System;

namespace System.Net
{
	internal class HeaderInfo
	{
		internal HeaderInfo(string name, bool requestRestricted, bool responseRestricted, bool multi, HeaderParser p)
		{
			this.HeaderName = name;
			this.IsRequestRestricted = requestRestricted;
			this.IsResponseRestricted = responseRestricted;
			this.Parser = p;
			this.AllowMultiValues = multi;
		}

		internal readonly bool IsRequestRestricted;

		internal readonly bool IsResponseRestricted;

		internal readonly HeaderParser Parser;

		internal readonly string HeaderName;

		internal readonly bool AllowMultiValues;
	}
}
