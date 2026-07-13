using System;
using System.Runtime.CompilerServices;
using Mono.Cecil;

namespace MonoMod.Utils
{
	[NullableContext(1)]
	internal interface ICallSiteGenerator
	{
		Mono.Cecil.CallSite ToCallSite(ModuleDefinition module);
	}
}
