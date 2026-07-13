using System;

namespace HarmonyLib
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method)]
	public class HarmonyBefore : HarmonyAttribute
	{
		public HarmonyBefore(params string[] before)
		{
			this.info.before = before;
		}
	}
}
