using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Buffers
{
	internal sealed class ReadOnlySequenceDebugView<[Nullable(2)] T>
	{
		public ReadOnlySequenceDebugView([Nullable(new byte[] { 0, 1 })] ReadOnlySequence<T> sequence)
		{
			this._array = (in sequence).ToArray<T>();
			int num = 0;
			foreach (global::System.ReadOnlyMemory<T> readOnlyMemory in sequence)
			{
				num++;
			}
			global::System.ReadOnlyMemory<T>[] array = new global::System.ReadOnlyMemory<T>[num];
			int num2 = 0;
			foreach (global::System.ReadOnlyMemory<T> readOnlyMemory2 in sequence)
			{
				array[num2] = readOnlyMemory2;
				num2++;
			}
			this._segments = new ReadOnlySequenceDebugView<T>.ReadOnlySequenceDebugViewSegments
			{
				Segments = array
			};
		}

		public ReadOnlySequenceDebugView<T>.ReadOnlySequenceDebugViewSegments BufferSegments
		{
			get
			{
				return this._segments;
			}
		}

		[Nullable(1)]
		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public T[] Items
		{
			[NullableContext(1)]
			get
			{
				return this._array;
			}
		}

		[Nullable(1)]
		private readonly T[] _array;

		private readonly ReadOnlySequenceDebugView<T>.ReadOnlySequenceDebugViewSegments _segments;

		[DebuggerDisplay("Count: {Segments.Length}", Name = "Segments")]
		public struct ReadOnlySequenceDebugViewSegments
		{
			[Nullable(new byte[] { 1, 0, 1 })]
			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public global::System.ReadOnlyMemory<T>[] Segments
			{
				[return: Nullable(new byte[] { 1, 0, 1 })]
				readonly get;
				[param: Nullable(new byte[] { 1, 0, 1 })]
				set;
			}
		}
	}
}
