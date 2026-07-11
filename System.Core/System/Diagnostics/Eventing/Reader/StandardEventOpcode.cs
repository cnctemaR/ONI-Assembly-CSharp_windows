using System;

namespace System.Diagnostics.Eventing.Reader
{
	public enum StandardEventOpcode
	{
		DataCollectionStart = 3,
		DataCollectionStop,
		Extension,
		Info = 0,
		Receive = 240,
		Reply = 6,
		Resume,
		Send = 9,
		Start = 1,
		Stop,
		Suspend = 8
	}
}
