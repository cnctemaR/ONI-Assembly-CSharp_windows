using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Harmony
{
	public static class PatchTools
	{
		public static void RememberObject(object key, object value)
		{
			PatchTools.objectReferences[key] = value;
		}

		public static MethodInfo GetPatchMethod<T>(Type patchType, string name, Type[] parameters = null)
		{
			MethodInfo methodInfo = patchType.GetMethods(AccessTools.all).FirstOrDefault<MethodInfo>((MethodInfo m) => m.GetCustomAttributes(typeof(T), true).Any<object>());
			bool flag = methodInfo == null;
			if (flag)
			{
				methodInfo = AccessTools.Method(patchType, name, parameters, null);
			}
			return methodInfo;
		}

		public static void GetPatches(Type patchType, out MethodInfo prefix, out MethodInfo postfix, out MethodInfo transpiler)
		{
			prefix = PatchTools.GetPatchMethod<HarmonyPrefix>(patchType, "Prefix", null);
			postfix = PatchTools.GetPatchMethod<HarmonyPostfix>(patchType, "Postfix", null);
			transpiler = PatchTools.GetPatchMethod<HarmonyTranspiler>(patchType, "Transpiler", null);
		}

		private static Dictionary<object, object> objectReferences = new Dictionary<object, object>();
	}
}
