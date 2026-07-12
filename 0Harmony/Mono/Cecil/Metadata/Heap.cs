using System;

namespace Mono.Cecil.Metadata
{
	internal abstract class Heap
	{
		protected Heap(byte[] data)
		{
			this.data = data;
		}

		public int IndexSize;

		internal readonly byte[] data;
	}
}
