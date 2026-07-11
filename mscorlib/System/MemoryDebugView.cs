using System;
using System.Diagnostics;

namespace System
{
	internal sealed class MemoryDebugView<T>
	{
		public MemoryDebugView(Memory<T> memory)
		{
			this._memory = memory;
		}

		public MemoryDebugView(ReadOnlyMemory<T> memory)
		{
			this._memory = memory;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public T[] Items
		{
			get
			{
				ArraySegment<T> arraySegment;
				if (this._memory.DangerousTryGetArray(out arraySegment))
				{
					T[] array = new T[this._memory.Length];
					Array.Copy(arraySegment.Array, arraySegment.Offset, array, 0, array.Length);
					return array;
				}
				return SpanHelpers.PerTypeValues<T>.EmptyArray;
			}
		}

		private readonly ReadOnlyMemory<T> _memory;
	}
}
