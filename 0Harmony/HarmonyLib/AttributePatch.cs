using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace HarmonyLib
{
	internal class AttributePatch
	{
		internal static AttributePatch Create(MethodInfo patch)
		{
			if (patch == null)
			{
				throw new NullReferenceException("Patch method cannot be null");
			}
			object[] customAttributes = patch.GetCustomAttributes(true);
			string name = patch.Name;
			HarmonyPatchType? patchType = AttributePatch.GetPatchType(name, customAttributes);
			if (patchType == null)
			{
				return null;
			}
			if (patchType.GetValueOrDefault() != HarmonyPatchType.ReversePatch && !patch.IsStatic)
			{
				throw new ArgumentException("Patch method " + patch.FullDescription() + " must be static");
			}
			IEnumerable<object> enumerable = customAttributes.Where<object>((object attr) => attr.GetType().BaseType.FullName == PatchTools.harmonyAttributeFullName).Select<object, object>(delegate(object attr)
			{
				FieldInfo fieldInfo = AccessTools.Field(attr.GetType(), "info");
				return fieldInfo.GetValue(attr);
			});
			Func<object, HarmonyMethod> func;
			if ((func = AttributePatch.<>O.<0>__MakeDeepCopy) == null)
			{
				func = (AttributePatch.<>O.<0>__MakeDeepCopy = new Func<object, HarmonyMethod>(AccessTools.MakeDeepCopy<HarmonyMethod>));
			}
			List<HarmonyMethod> list = enumerable.Select<object, HarmonyMethod>(func).ToList<HarmonyMethod>();
			HarmonyMethod harmonyMethod = HarmonyMethod.Merge(list);
			harmonyMethod.method = patch;
			return new AttributePatch
			{
				info = harmonyMethod,
				type = patchType
			};
		}

		private static HarmonyPatchType? GetPatchType(string methodName, object[] allAttributes)
		{
			HashSet<string> hashSet = new HashSet<string>(from attr in allAttributes
				select attr.GetType().FullName into name
				where name.StartsWith("Harmony")
				select name);
			HarmonyPatchType? harmonyPatchType = null;
			foreach (HarmonyPatchType harmonyPatchType2 in AttributePatch.allPatchTypes)
			{
				string text = harmonyPatchType2.ToString();
				if (text == methodName || hashSet.Contains("HarmonyLib.Harmony" + text))
				{
					harmonyPatchType = new HarmonyPatchType?(harmonyPatchType2);
					break;
				}
			}
			return harmonyPatchType;
		}

		private static readonly HarmonyPatchType[] allPatchTypes = new HarmonyPatchType[]
		{
			HarmonyPatchType.Prefix,
			HarmonyPatchType.Postfix,
			HarmonyPatchType.Transpiler,
			HarmonyPatchType.Finalizer,
			HarmonyPatchType.ReversePatch,
			HarmonyPatchType.InnerPrefix,
			HarmonyPatchType.InnerPostfix
		};

		internal HarmonyMethod info;

		internal HarmonyPatchType? type;

		[CompilerGenerated]
		private static class <>O
		{
			public static Func<object, HarmonyMethod> <0>__MakeDeepCopy;
		}
	}
}
