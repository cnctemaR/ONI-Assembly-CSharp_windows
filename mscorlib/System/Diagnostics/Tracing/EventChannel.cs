using System;
using System.Runtime.CompilerServices;

namespace System.Diagnostics.Tracing
{
	[FriendAccessAllowed]
	public enum EventChannel : byte
	{
		None,
		Admin = 16,
		Operational,
		Analytic,
		Debug
	}
}
