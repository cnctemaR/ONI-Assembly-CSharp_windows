using System;

namespace System.IO.Ports
{
	internal enum SerialSignal
	{
		None,
		Cd,
		Cts,
		Dsr = 4,
		Dtr = 8,
		Rts = 16
	}
}
