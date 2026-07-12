using System;

namespace System.Diagnostics.Tracing
{
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
