using System;

namespace HarmonyLib
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method)]
	public class HarmonyDebug : HarmonyAttribute
	{
		public HarmonyDebug()
		{
			this.info.debug = new bool?(true);
		}
	}
}
