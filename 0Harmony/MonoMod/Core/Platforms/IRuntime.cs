using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using MonoMod.Utils;

namespace MonoMod.Core.Platforms
{
	[NullableContext(1)]
	internal interface IRuntime
	{
		RuntimeKind Target { get; }

		RuntimeFeature Features { get; }

		Abi Abi { get; }

		[Nullable(2)]
		event OnMethodCompiledCallback OnMethodCompiled;

		MethodBase GetIdentifiable(MethodBase method);

		RuntimeMethodHandle GetMethodHandle(MethodBase method);

		bool RequiresGenericContext(MethodBase method);

		void DisableInlining(MethodBase method);

		[return: Nullable(2)]
		IDisposable PinMethodIfNeeded(MethodBase method);

		IntPtr GetMethodEntryPoint(MethodBase method);

		void Compile(MethodBase method);
	}
}
