using System;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes
{
	internal interface IPipe
	{
		SafePipeHandle Handle { get; }

		void WaitForPipeDrain();
	}
}
