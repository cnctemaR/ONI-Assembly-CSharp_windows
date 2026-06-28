using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	[Serializable]
	public enum FlowControl
	{
		Branch,
		Break,
		Call,
		Cond_Branch,
		Meta,
		Next,
		[Obsolete("This API has been deprecated.")]
		Phi,
		Return,
		Throw
	}
}
