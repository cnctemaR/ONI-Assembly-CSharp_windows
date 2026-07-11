using System;
using System.ComponentModel;
using System.Diagnostics;

namespace System.Runtime.CompilerServices
{
	[DebuggerStepThrough]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class Closure
	{
		public Closure(object[] constants, object[] locals)
		{
			this.Constants = constants;
			this.Locals = locals;
		}

		public readonly object[] Constants;

		public readonly object[] Locals;
	}
}
