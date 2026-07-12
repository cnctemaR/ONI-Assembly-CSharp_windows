using System;
using UnityEngine.Scripting;

namespace UnityEngine.PlayerLoop
{
	[RequiredByNativeCode]
	public struct TimeUpdate
	{
		[RequiredByNativeCode]
		public struct WaitForLastPresentationAndUpdateTime
		{
		}

		[Obsolete("ProfilerStartFrame player loop component has been moved to the Initialization category. (UnityUpgradable) -> UnityEngine.PlayerLoop.Initialization/ProfilerStartFrame", true)]
		public struct ProfilerStartFrame
		{
		}
	}
}
