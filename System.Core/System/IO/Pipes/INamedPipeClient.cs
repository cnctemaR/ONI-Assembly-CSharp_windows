using System;

namespace System.IO.Pipes
{
	internal interface INamedPipeClient : IPipe
	{
		void Connect();

		void Connect(int timeout);

		int NumberOfServerInstances { get; }

		bool IsAsync { get; }
	}
}
