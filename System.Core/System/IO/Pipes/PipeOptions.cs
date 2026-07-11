using System;

namespace System.IO.Pipes
{
	[Flags]
	public enum PipeOptions
	{
		None = 0,
		WriteThrough = -2147483648,
		Asynchronous = 1073741824
	}
}
