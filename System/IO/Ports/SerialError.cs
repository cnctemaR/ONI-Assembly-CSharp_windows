using System;

namespace System.IO.Ports
{
	public enum SerialError
	{
		RXOver = 1,
		Overrun,
		RXParity = 4,
		Frame = 8,
		TXFull = 256
	}
}
