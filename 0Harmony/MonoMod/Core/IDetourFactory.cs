using System;
using System.Runtime.CompilerServices;

namespace MonoMod.Core
{
	[NullableContext(1)]
	[CLSCompliant(true)]
	internal interface IDetourFactory
	{
		ICoreDetour CreateDetour(CreateDetourRequest request);

		ICoreNativeDetour CreateNativeDetour(CreateNativeDetourRequest request);

		bool SupportsNativeDetourOrigEntrypoint { get; }
	}
}
