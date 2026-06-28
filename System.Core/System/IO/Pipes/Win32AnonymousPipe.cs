using System;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes
{
	internal abstract class Win32AnonymousPipe : IPipe
	{
		public abstract SafePipeHandle Handle { get; }

		public void WaitForPipeDrain()
		{
			throw new NotImplementedException();
		}
	}
}
