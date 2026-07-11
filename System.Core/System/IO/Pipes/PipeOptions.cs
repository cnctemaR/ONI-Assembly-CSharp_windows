using System;

namespace System.IO.Pipes
{
	[Flags]
	[Serializable]
	public enum PipeOptions
	{
		None = 0,
		WriteThrough = 1,
		Asynchronous = 2
	}
}
