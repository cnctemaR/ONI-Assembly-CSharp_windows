using System;

namespace MonoMod.Core.Platforms
{
	[Flags]
	internal enum RuntimeFeature
	{
		None = 0,
		PreciseGC = 1,
		CompileMethodHook = 2,
		ILDetour = 4,
		GenericSharing = 8,
		ListGenericInstantiations = 64,
		DisableInlining = 16,
		Uninlining = 32,
		RequiresMethodPinning = 128,
		RequiresMethodIdentification = 256,
		RequiresBodyThunkWalking = 512,
		HasKnownABI = 1024,
		RequiresCustomMethodCompile = 2048
	}
}
