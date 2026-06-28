using System;

namespace System.Diagnostics
{
	public enum ThreadState
	{
		Initialized,
		Ready,
		Running,
		Standby,
		Terminated,
		Transition = 6,
		Unknown,
		Wait = 5
	}
}
