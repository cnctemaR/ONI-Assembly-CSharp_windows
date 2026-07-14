using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace VYaml.Internal
{
	internal class ReusableByteSequenceBuilder
	{
		public void Add(ReadOnlyMemory<byte> buffer, bool returnToPool)
		{
			ReusableByteSequenceSegment reusableByteSequenceSegment;
			if (!this.segmentPool.TryPop(out reusableByteSequenceSegment))
			{
				reusableByteSequenceSegment = new ReusableByteSequenceSegment();
			}
			reusableByteSequenceSegment.SetBuffer(buffer, returnToPool);
			this.segments.Add(reusableByteSequenceSegment);
		}

		public bool TryGetSingleMemory(out ReadOnlyMemory<byte> memory)
		{
			if (this.segments.Count == 1)
			{
				memory = this.segments[0].Memory;
				return true;
			}
			memory = default(ReadOnlyMemory<byte>);
			return false;
		}

		public ReadOnlySequence<byte> Build()
		{
			if (this.segments.Count == 0)
			{
				return ReadOnlySequence<byte>.Empty;
			}
			if (this.segments.Count == 1)
			{
				return new ReadOnlySequence<byte>(this.segments[0].Memory);
			}
			long num = 0L;
			for (int i = 0; i < this.segments.Count; i++)
			{
				ReusableByteSequenceSegment reusableByteSequenceSegment = ((i < this.segments.Count - 1) ? this.segments[i + 1] : null);
				this.segments[i].SetRunningIndexAndNext(num, reusableByteSequenceSegment);
				num += (long)this.segments[i].Memory.Length;
			}
			ReadOnlySequenceSegment<byte> readOnlySequenceSegment = this.segments[0];
			List<ReusableByteSequenceSegment> list = this.segments;
			ReusableByteSequenceSegment reusableByteSequenceSegment2 = list[list.Count - 1];
			return new ReadOnlySequence<byte>(readOnlySequenceSegment, 0, reusableByteSequenceSegment2, reusableByteSequenceSegment2.Memory.Length);
		}

		public void Reset()
		{
			foreach (ReusableByteSequenceSegment reusableByteSequenceSegment in this.segments)
			{
				reusableByteSequenceSegment.Reset();
				this.segmentPool.Push(reusableByteSequenceSegment);
			}
			this.segments.Clear();
		}

		[Nullable(1)]
		private readonly Stack<ReusableByteSequenceSegment> segmentPool = new Stack<ReusableByteSequenceSegment>();

		[Nullable(1)]
		private readonly List<ReusableByteSequenceSegment> segments = new List<ReusableByteSequenceSegment>();
	}
}
