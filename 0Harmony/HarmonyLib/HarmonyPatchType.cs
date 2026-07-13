using System;

namespace HarmonyLib
{
	public enum HarmonyPatchType
	{
		All,
		Prefix,
		Postfix,
		Transpiler,
		Finalizer,
		ReversePatch,
		InnerPrefix,
		InnerPostfix
	}
}
