using System;
using System.ComponentModel;

namespace System.Data
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Flags]
	[Obsolete("PropertyAttributes has been deprecated.  http://go.microsoft.com/fwlink/?linkid=14202")]
	public enum PropertyAttributes
	{
		NotSupported = 0,
		Required = 1,
		Optional = 2,
		Read = 512,
		Write = 1024
	}
}
