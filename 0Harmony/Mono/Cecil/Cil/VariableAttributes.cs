using System;

namespace Mono.Cecil.Cil
{
	[Flags]
	public enum VariableAttributes : ushort
	{
		None = 0,
		DebuggerHidden = 1
	}
}
