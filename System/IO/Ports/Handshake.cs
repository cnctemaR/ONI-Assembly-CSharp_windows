using System;

namespace System.IO.Ports
{
	public enum Handshake
	{
		None,
		XOnXOff,
		RequestToSend,
		RequestToSendXOnXOff
	}
}
