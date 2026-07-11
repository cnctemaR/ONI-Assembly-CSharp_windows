using System;
using System.Runtime.CompilerServices;

namespace System.Diagnostics.Tracing
{
	[FriendAccessAllowed]
	public enum EventOpcode
	{
		Info,
		Start,
		Stop,
		DataCollectionStart,
		DataCollectionStop,
		Extension,
		Reply,
		Resume,
		Suspend,
		Send,
		Receive = 240
	}
}
