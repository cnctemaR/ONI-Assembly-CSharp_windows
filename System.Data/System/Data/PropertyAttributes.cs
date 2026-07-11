using System;
using System.ComponentModel;

namespace System.Data
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Flags]
	[Obsolete]
	public enum PropertyAttributes
	{
		NotSupported = 0,
		Optional = 2,
		Read = 512,
		Required = 1,
		Write = 1024
	}
}
