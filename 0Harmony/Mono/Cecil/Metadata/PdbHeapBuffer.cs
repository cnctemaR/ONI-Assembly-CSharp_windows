using System;

namespace Mono.Cecil.Metadata
{
	internal sealed class PdbHeapBuffer : HeapBuffer
	{
		public override bool IsEmpty
		{
			get
			{
				return false;
			}
		}

		public PdbHeapBuffer()
			: base(0)
		{
		}
	}
}
