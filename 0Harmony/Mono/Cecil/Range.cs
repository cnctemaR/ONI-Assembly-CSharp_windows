using System;

namespace Mono.Cecil
{
	internal struct Range
	{
		public Range(uint index, uint length)
		{
			this.Start = index;
			this.Length = length;
		}

		public uint Start;

		public uint Length;
	}
}
