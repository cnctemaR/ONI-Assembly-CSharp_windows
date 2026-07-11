using System;
using System.Diagnostics;

namespace System.Collections.Generic
{
	internal sealed class StackDebugView<T>
	{
		public StackDebugView(Stack<T> stack)
		{
			if (stack == null)
			{
				throw new ArgumentNullException("stack");
			}
			this._stack = stack;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public T[] Items
		{
			get
			{
				return this._stack.ToArray();
			}
		}

		private readonly Stack<T> _stack;
	}
}
