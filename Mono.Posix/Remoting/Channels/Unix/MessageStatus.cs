using System;

namespace Mono.Remoting.Channels.Unix
{
	internal enum MessageStatus
	{
		MethodMessage,
		CancelSignal,
		Unknown = 10
	}
}
