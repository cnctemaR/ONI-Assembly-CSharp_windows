using System;

namespace Unity.IO.LowLevel.Unsafe
{
	public struct ReadCommandArray
	{
		public unsafe ReadCommand* ReadCommands;

		public int CommandCount;
	}
}
