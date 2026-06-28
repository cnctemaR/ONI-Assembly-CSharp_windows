using System;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes
{
	internal abstract class UnixAnonymousPipe : IPipe
	{
		public abstract SafePipeHandle Handle { get; }

		public void WaitForPipeDrain()
		{
			throw new NotImplementedException();
		}
	}
}
