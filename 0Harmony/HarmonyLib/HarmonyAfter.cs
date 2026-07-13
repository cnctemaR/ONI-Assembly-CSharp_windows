using System;

namespace HarmonyLib
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method)]
	public class HarmonyAfter : HarmonyAttribute
	{
		public HarmonyAfter(params string[] after)
		{
			this.info.after = after;
		}
	}
}
