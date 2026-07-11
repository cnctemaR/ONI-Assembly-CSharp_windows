using System;

namespace System.Net
{
	internal enum BufferType
	{
		Empty,
		Data,
		Token,
		Parameters,
		Missing,
		Extra,
		Trailer,
		Header,
		Padding = 9,
		Stream,
		ChannelBindings = 14,
		TargetHost = 16,
		ReadOnlyFlag = -2147483648,
		ReadOnlyWithChecksum = 268435456
	}
}
