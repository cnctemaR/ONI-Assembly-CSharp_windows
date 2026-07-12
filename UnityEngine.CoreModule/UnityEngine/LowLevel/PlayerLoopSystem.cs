using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.LowLevel
{
	[MovedFrom("UnityEngine.Experimental.LowLevel")]
	public struct PlayerLoopSystem
	{
		public override string ToString()
		{
			return this.type.Name;
		}

		public Type type;

		public PlayerLoopSystem[] subSystemList;

		public PlayerLoopSystem.UpdateFunction updateDelegate;

		public IntPtr updateFunction;

		public IntPtr loopConditionFunction;

		public delegate void UpdateFunction();
	}
}
