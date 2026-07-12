using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace HarmonyLib
{
	internal static class PatchArgumentExtensions
	{
		private static HarmonyArgument[] AllHarmonyArguments(object[] attributes)
		{
			return (from harg in attributes.Select<object, HarmonyArgument>(delegate(object attr)
				{
					if (attr.GetType().Name != "HarmonyArgument")
					{
						return null;
					}
					return AccessTools.MakeDeepCopy<HarmonyArgument>(attr);
				})
				where harg != null
				select harg).ToArray<HarmonyArgument>();
		}

		private static HarmonyArgument GetArgumentAttribute(this ParameterInfo parameter)
		{
			return PatchArgumentExtensions.AllHarmonyArguments(parameter.GetCustomAttributes(false)).FirstOrDefault<HarmonyArgument>();
		}

		private static HarmonyArgument[] GetArgumentAttributes(this MethodInfo method)
		{
			if (method == null || method is DynamicMethod)
			{
				return null;
			}
			return PatchArgumentExtensions.AllHarmonyArguments(method.GetCustomAttributes(false));
		}

		private static HarmonyArgument[] GetArgumentAttributes(this Type type)
		{
			return PatchArgumentExtensions.AllHarmonyArguments(type.GetCustomAttributes(false));
		}

		private static string GetOriginalArgumentName(this ParameterInfo parameter, string[] originalParameterNames)
		{
			HarmonyArgument argumentAttribute = parameter.GetArgumentAttribute();
			if (argumentAttribute == null)
			{
				return null;
			}
			if (!string.IsNullOrEmpty(argumentAttribute.OriginalName))
			{
				return argumentAttribute.OriginalName;
			}
			if (argumentAttribute.Index >= 0 && argumentAttribute.Index < originalParameterNames.Length)
			{
				return originalParameterNames[argumentAttribute.Index];
			}
			return null;
		}

		private static string GetOriginalArgumentName(HarmonyArgument[] attributes, string name, string[] originalParameterNames)
		{
			if (((attributes != null) ? attributes.Length : 0) <= 0)
			{
				return null;
			}
			HarmonyArgument harmonyArgument = attributes.SingleOrDefault<HarmonyArgument>((HarmonyArgument p) => p.NewName == name);
			if (harmonyArgument == null)
			{
				return null;
			}
			if (!string.IsNullOrEmpty(harmonyArgument.OriginalName))
			{
				return harmonyArgument.OriginalName;
			}
			if (originalParameterNames != null && harmonyArgument.Index >= 0 && harmonyArgument.Index < originalParameterNames.Length)
			{
				return originalParameterNames[harmonyArgument.Index];
			}
			return null;
		}

		private static string GetOriginalArgumentName(this MethodInfo method, string[] originalParameterNames, string name)
		{
			string text = PatchArgumentExtensions.GetOriginalArgumentName((method != null) ? method.GetArgumentAttributes() : null, name, originalParameterNames);
			if (text != null)
			{
				return text;
			}
			HarmonyArgument[] array;
			if (method == null)
			{
				array = null;
			}
			else
			{
				Type declaringType = method.DeclaringType;
				array = ((declaringType != null) ? declaringType.GetArgumentAttributes() : null);
			}
			text = PatchArgumentExtensions.GetOriginalArgumentName(array, name, originalParameterNames);
			if (text != null)
			{
				return text;
			}
			return name;
		}

		internal static int GetArgumentIndex(this MethodInfo patch, string[] originalParameterNames, ParameterInfo patchParam)
		{
			if (patch is DynamicMethod)
			{
				return Array.IndexOf<string>(originalParameterNames, patchParam.Name);
			}
			string text = patchParam.GetOriginalArgumentName(originalParameterNames);
			if (text != null)
			{
				return Array.IndexOf<string>(originalParameterNames, text);
			}
			text = patch.GetOriginalArgumentName(originalParameterNames, patchParam.Name);
			if (text != null)
			{
				return Array.IndexOf<string>(originalParameterNames, text);
			}
			return -1;
		}
	}
}
