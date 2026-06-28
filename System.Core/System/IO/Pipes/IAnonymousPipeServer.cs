using System;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes
{
	internal interface IAnonymousPipeServer : IPipe
	{
		SafePipeHandle ClientHandle { get; }

		void DisposeLocalCopyOfClientHandle();
	}
}
