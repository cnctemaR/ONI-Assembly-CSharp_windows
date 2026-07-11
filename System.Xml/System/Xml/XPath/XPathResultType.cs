using System;

namespace System.Xml.XPath
{
	public enum XPathResultType
	{
		Number,
		String,
		Boolean,
		NodeSet,
		Navigator = 1,
		Any = 5,
		Error
	}
}
