using System;
using System.Runtime.InteropServices;

namespace System.Diagnostics
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public sealed class DebuggerBrowsableAttribute : Attribute
	{
		public DebuggerBrowsableAttribute(DebuggerBrowsableState state)
		{
			this.state = state;
		}

		public DebuggerBrowsableState State
		{
			get
			{
				return this.state;
			}
		}

		private DebuggerBrowsableState state;
	}
}
