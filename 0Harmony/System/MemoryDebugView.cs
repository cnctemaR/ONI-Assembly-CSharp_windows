using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class MemoryDebugView<[Nullable(2)] T>
	{
		public MemoryDebugView([Nullable(new byte[] { 0, 1 })] global::System.Memory<T> memory)
		{
			this._memory = memory;
		}

		public MemoryDebugView([Nullable(new byte[] { 0, 1 })] global::System.ReadOnlyMemory<T> memory)
		{
			this._memory = memory;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public T[] Items
		{
			get
			{
				return this._memory.ToArray();
			}
		}

		[Nullable(new byte[] { 0, 1 })]
		private readonly global::System.ReadOnlyMemory<T> _memory;
	}
}
