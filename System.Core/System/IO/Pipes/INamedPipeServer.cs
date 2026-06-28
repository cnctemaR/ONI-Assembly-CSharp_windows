using System;

namespace System.IO.Pipes
{
	internal interface INamedPipeServer : IPipe
	{
		void Disconnect();

		void WaitForConnection();
	}
}
