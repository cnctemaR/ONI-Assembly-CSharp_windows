using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class SpanDebugView<[Nullable(2)] T>
	{
		public SpanDebugView([Nullable(new byte[] { 0, 1 })] global::System.Span<T> span)
		{
			this._array = span.ToArray();
		}

		public SpanDebugView([Nullable(new byte[] { 0, 1 })] global::System.ReadOnlySpan<T> span)
		{
			this._array = span.ToArray();
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public T[] Items
		{
			get
			{
				return this._array;
			}
		}

		private readonly T[] _array;
	}
}
