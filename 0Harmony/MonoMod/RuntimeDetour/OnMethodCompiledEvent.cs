using System;
using System.Reflection;

namespace MonoMod.RuntimeDetour
{
	public delegate void OnMethodCompiledEvent(MethodBase method, IntPtr codeStart, ulong codeSize);
}
