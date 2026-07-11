using System;

namespace MS.Internal.Xml.XPath
{
	internal enum QueryProps
	{
		None,
		Position,
		Count,
		Cached = 4,
		Reverse = 8,
		Merge = 16
	}
}
