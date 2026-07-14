using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace VYaml.Internal
{
	internal class ReusableByteSequenceSegment : ReadOnlySequenceSegment<byte>
	{
		public ReusableByteSequenceSegment()
		{
			this.returnToPool = false;
		}

		public void SetBuffer(ReadOnlyMemory<byte> buffer, bool returnToPool)
		{
			base.Memory = buffer;
			this.returnToPool = returnToPool;
		}

		public void Reset()
		{
			ArraySegment<byte> arraySegment;
			if (this.returnToPool && MemoryMarshal.TryGetArray<byte>(base.Memory, out arraySegment) && arraySegment.Array != null)
			{
				ArrayPool<byte>.Shared.Return(arraySegment.Array, false);
			}
			base.Memory = default(ReadOnlyMemory<byte>);
			base.RunningIndex = 0L;
			base.Next = null;
		}

		[NullableContext(2)]
		public void SetRunningIndexAndNext(long runningIndex, ReusableByteSequenceSegment nextSegment)
		{
			base.RunningIndex = runningIndex;
			base.Next = nextSegment;
		}

		private bool returnToPool;
	}
}
