using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace HarmonyLib
{
	internal static class PatchArgumentExtensions
	{
		private static IEnumerable<HarmonyArgument> AllHarmonyArguments(object[] attributes)
		{
			return attributes.Select<object, HarmonyArgument>(delegate(object attr)
			{
				if (attr.GetType().Name != "HarmonyArgument")
				{
					return null;
				}
				return AccessTools.MakeDeepCopy<HarmonyArgument>(attr);
			}).OfType<HarmonyArgument>();
		}

		internal static HarmonyArgument GetArgumentAttribute(this ParameterInfo parameter)
		{
			HarmonyArgument harmonyArgument;
			try
			{
				object[] customAttributes = parameter.GetCustomAttributes(true);
				harmonyArgument = PatchArgumentExtensions.AllHarmonyArguments(customAttributes).FirstOrDefault<HarmonyArgument>();
			}
			catch (NotSupportedException)
			{
				harmonyArgument = null;
			}
			return harmonyArgument;
		}

		internal static IEnumerable<HarmonyArgument> GetArgumentAttributes(this MethodInfo method)
		{
			IEnumerable<HarmonyArgument> enumerable;
			try
			{
				object[] customAttributes = method.GetCustomAttributes(true);
				enumerable = PatchArgumentExtensions.AllHarmonyArguments(customAttributes);
			}
			catch (NotSupportedException)
			{
				enumerable = Array.Empty<HarmonyArgument>();
			}
			return enumerable;
		}

		internal static IEnumerable<HarmonyArgument> GetArgumentAttributes(this Type type)
		{
			IEnumerable<HarmonyArgument> enumerable;
			try
			{
				object[] customAttributes = type.GetCustomAttributes(true);
				enumerable = PatchArgumentExtensions.AllHarmonyArguments(customAttributes);
			}
			catch (NotSupportedException)
			{
				enumerable = Array.Empty<HarmonyArgument>();
			}
			return enumerable;
		}

		internal static string GetRealName(this IEnumerable<HarmonyArgument> attributes, string name, string[] originalParameterNames)
		{
			HarmonyArgument harmonyArgument = attributes.FirstOrDefault<HarmonyArgument>((HarmonyArgument p) => p.OriginalName == name);
			if (harmonyArgument == null)
			{
				return null;
			}
			if (!string.IsNullOrEmpty(harmonyArgument.NewName))
			{
				return harmonyArgument.NewName;
			}
			if (originalParameterNames != null && harmonyArgument.Index >= 0 && harmonyArgument.Index < originalParameterNames.Length)
			{
				return originalParameterNames[harmonyArgument.Index];
			}
			return null;
		}

		private static string GetRealParameterName(this MethodInfo method, string[] originalParameterNames, string name)
		{
			if (method == null || method is DynamicMethod)
			{
				return name;
			}
			string text = method.GetArgumentAttributes().GetRealName(name, originalParameterNames);
			if (text != null)
			{
				return text;
			}
			Type declaringType = method.DeclaringType;
			if (declaringType != null)
			{
				text = declaringType.GetArgumentAttributes().GetRealName(name, originalParameterNames);
				if (text != null)
				{
					return text;
				}
			}
			return name;
		}

		private static string GetRealParameterName(this ParameterInfo parameter, string[] originalParameterNames)
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

		internal static int GetArgumentIndex(this MethodInfo patch, string[] originalParameterNames, ParameterInfo patchParam)
		{
			if (patch is DynamicMethod)
			{
				return Array.IndexOf<string>(originalParameterNames, patchParam.Name);
			}
			string text = patchParam.GetRealParameterName(originalParameterNames);
			if (text != null)
			{
				return Array.IndexOf<string>(originalParameterNames, text);
			}
			text = patch.GetRealParameterName(originalParameterNames, patchParam.Name);
			if (text != null)
			{
				return Array.IndexOf<string>(originalParameterNames, text);
			}
			return -1;
		}
	}
}
