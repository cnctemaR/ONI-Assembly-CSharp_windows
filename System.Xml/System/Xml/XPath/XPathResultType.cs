using System;

namespace System.Xml.XPath
{
	public enum XPathResultType
	{
		Number,
		String,
		Boolean,
		NodeSet,
		[MonoFIX("MS.NET: 1")]
		Navigator,
		Any,
		Error
	}
}
