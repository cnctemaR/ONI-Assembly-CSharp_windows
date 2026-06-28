using System;
using System.Runtime.InteropServices;

namespace System.Diagnostics
{
	[ComVisible(true)]
	public enum DebuggerBrowsableState
	{
		Never,
		Collapsed = 2,
		RootHidden
	}
}
