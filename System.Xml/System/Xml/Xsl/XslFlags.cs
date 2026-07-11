using System;

namespace System.Xml.Xsl
{
	[Flags]
	internal enum XslFlags
	{
		None = 0,
		String = 1,
		Number = 2,
		Boolean = 4,
		Node = 8,
		Nodeset = 16,
		Rtf = 32,
		TypeFilter = 63,
		AnyType = 63,
		Current = 256,
		Position = 512,
		Last = 1024,
		FocusFilter = 1792,
		FullFocus = 1792,
		HasCalls = 4096,
		MayBeDefault = 8192,
		SideEffects = 16384,
		Stop = 32768
	}
}
