using System;
using System.Runtime.InteropServices;

namespace System.Diagnostics
{
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property, Inherited = false)]
	[ComVisible(true)]
	[Serializable]
	public sealed class DebuggerHiddenAttribute : Attribute
	{
	}
}
