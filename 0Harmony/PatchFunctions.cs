using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Harmony.ILCopying;

namespace Harmony
{
	public static class PatchFunctions
	{
		public static void AddPrefix(PatchInfo patchInfo, string owner, HarmonyMethod info)
		{
			bool flag = info == null || info.method == null;
			if (!flag)
			{
				int num = ((info.prioritiy == -1) ? 400 : info.prioritiy);
				string[] array = info.before ?? new string[0];
				string[] array2 = info.after ?? new string[0];
				patchInfo.AddPrefix(info.method, owner, num, array, array2);
			}
		}

		public static void RemovePrefix(PatchInfo patchInfo, string owner)
		{
			patchInfo.RemovePrefix(owner);
		}

		public static void AddPostfix(PatchInfo patchInfo, string owner, HarmonyMethod info)
		{
			bool flag = info == null || info.method == null;
			if (!flag)
			{
				int num = ((info.prioritiy == -1) ? 400 : info.prioritiy);
				string[] array = info.before ?? new string[0];
				string[] array2 = info.after ?? new string[0];
				patchInfo.AddPostfix(info.method, owner, num, array, array2);
			}
		}

		public static void RemovePostfix(PatchInfo patchInfo, string owner)
		{
			patchInfo.RemovePostfix(owner);
		}

		public static void AddTranspiler(PatchInfo patchInfo, string owner, HarmonyMethod info)
		{
			bool flag = info == null || info.method == null;
			if (!flag)
			{
				int num = ((info.prioritiy == -1) ? 400 : info.prioritiy);
				string[] array = info.before ?? new string[0];
				string[] array2 = info.after ?? new string[0];
				patchInfo.AddTranspiler(info.method, owner, num, array, array2);
			}
		}

		public static void RemoveTranspiler(PatchInfo patchInfo, string owner)
		{
			patchInfo.RemoveTranspiler(owner);
		}

		public static void RemovePatch(PatchInfo patchInfo, MethodInfo patch)
		{
			patchInfo.RemovePatch(patch);
		}

		public static List<ILInstruction> GetInstructions(ILGenerator generator, MethodBase method)
		{
			return MethodBodyReader.GetInstructions(generator, method);
		}

		public static List<MethodInfo> GetSortedPatchMethods(MethodBase original, Patch[] patches)
		{
			return (from p in patches
				where p.patch != null
				orderby p
				select p.GetMethod(original)).ToList<MethodInfo>();
		}

		public static DynamicMethod UpdateWrapper(MethodBase original, PatchInfo patchInfo, string instanceID)
		{
			List<MethodInfo> sortedPatchMethods = PatchFunctions.GetSortedPatchMethods(original, patchInfo.prefixes);
			List<MethodInfo> sortedPatchMethods2 = PatchFunctions.GetSortedPatchMethods(original, patchInfo.postfixes);
			List<MethodInfo> sortedPatchMethods3 = PatchFunctions.GetSortedPatchMethods(original, patchInfo.transpilers);
			DynamicMethod dynamicMethod = MethodPatcher.CreatePatchedMethod(original, instanceID, sortedPatchMethods, sortedPatchMethods2, sortedPatchMethods3);
			bool flag = dynamicMethod == null;
			if (flag)
			{
				throw new MissingMethodException("Cannot create dynamic replacement for " + original.FullDescription());
			}
			string text = Memory.DetourMethod(original, dynamicMethod);
			bool flag2 = text != null;
			if (flag2)
			{
				throw new FormatException("Method " + original.FullDescription() + " cannot be patched. Reason: " + text);
			}
			PatchTools.RememberObject(original, dynamicMethod);
			return dynamicMethod;
		}
	}
}
