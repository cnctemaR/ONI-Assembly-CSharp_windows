using System;

namespace Harmony
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class HarmonyPriority : HarmonyAttribute
	{
		public HarmonyPriority(int prioritiy)
		{
			this.info.prioritiy = prioritiy;
		}
	}
}
