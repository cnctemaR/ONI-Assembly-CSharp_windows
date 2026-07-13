using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using MonoMod.Utils;

namespace MonoMod.Core
{
	[NullableContext(2)]
	internal interface ICoreDetourWithClone : ICoreDetour, ICoreDetourBase, IDisposable
	{
		MethodInfo SourceMethodClone { get; }

		DynamicMethodDefinition SourceMethodCloneIL { get; }
	}
}
