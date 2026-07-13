using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MonoMod.Core
{
	[NullableContext(1)]
	[CLSCompliant(true)]
	internal interface ICoreDetour : ICoreDetourBase, IDisposable
	{
		MethodBase Source { get; }

		MethodBase Target { get; }
	}
}
