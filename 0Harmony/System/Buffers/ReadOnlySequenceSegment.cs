using System;
using System.Runtime.CompilerServices;

namespace System.Buffers
{
	internal abstract class ReadOnlySequenceSegment<[Nullable(2)] T>
	{
		[Nullable(new byte[] { 0, 1 })]
		public global::System.ReadOnlyMemory<T> Memory
		{
			[return: Nullable(new byte[] { 0, 1 })]
			get;
			[param: Nullable(new byte[] { 0, 1 })]
			protected set;
		}

		[Nullable(new byte[] { 2, 1 })]
		public ReadOnlySequenceSegment<T> Next
		{
			[return: Nullable(new byte[] { 2, 1 })]
			get;
			[param: Nullable(new byte[] { 2, 1 })]
			protected set;
		}

		public long RunningIndex { get; protected set; }
	}
}
