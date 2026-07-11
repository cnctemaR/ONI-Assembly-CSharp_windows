using System;

namespace System.Runtime
{
	internal enum TraceEventOpcode
	{
		Info,
		Start,
		Stop,
		Reply = 6,
		Resume,
		Suspend,
		Send,
		Receive = 240
	}
}
