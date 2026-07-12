using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.LowLevel
{
	[RequiredByNativeCode]
	[MovedFrom("UnityEngine.Experimental.LowLevel")]
	[NativeType(Header = "Runtime/Misc/PlayerLoop.h")]
	internal struct PlayerLoopSystemInternal
	{
		public Type type;

		public PlayerLoopSystem.UpdateFunction updateDelegate;

		public IntPtr updateFunction;

		public IntPtr loopConditionFunction;

		public int numSubSystems;
	}
}
