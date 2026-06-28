using System;

namespace System.Xml
{
	public enum ValidationType
	{
		None,
		[Obsolete]
		Auto,
		DTD,
		[Obsolete]
		XDR,
		Schema
	}
}
