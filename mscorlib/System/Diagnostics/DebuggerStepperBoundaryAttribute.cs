using System;
using System.Runtime.InteropServices;

namespace System.Diagnostics
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, Inherited = false)]
	[Serializable]
	public sealed class DebuggerStepperBoundaryAttribute : Attribute
	{
	}
}
