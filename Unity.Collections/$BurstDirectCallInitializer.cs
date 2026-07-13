using System;
using Unity.Collections;
using UnityEngine;

internal static class $BurstDirectCallInitializer
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	private static void Initialize()
	{
		AllocatorManager.Initialize$StackAllocator_Try_00000980$BurstDirectCall();
		AllocatorManager.Initialize$SlabAllocator_Try_0000098E$BurstDirectCall();
		RewindableAllocator.Try_000006E8$BurstDirectCall.Initialize();
		xxHash3.Hash64Long_0000071F$BurstDirectCall.Initialize();
		xxHash3.Hash128Long_00000726$BurstDirectCall.Initialize();
	}
}
