using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using MonoMod.Utils;

namespace MonoMod.ModInterop
{
	[NullableContext(1)]
	[Nullable(0)]
	internal static class ModInteropManager
	{
		public static void ModInterop(this Type type)
		{
			Helpers.ThrowIfArgumentNull<Type>(type, "type");
			if (ModInteropManager.Registered.Contains(type))
			{
				return;
			}
			ModInteropManager.Registered.Add(type);
			string text = type.Assembly.GetName().Name;
			object[] customAttributes = type.GetCustomAttributes(typeof(ModExportNameAttribute), false);
			for (int i = 0; i < customAttributes.Length; i++)
			{
				text = ((ModExportNameAttribute)customAttributes[i]).Name;
			}
			foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Static | BindingFlags.Public))
			{
				if (typeof(Delegate).IsAssignableFrom(fieldInfo.FieldType))
				{
					ModInteropManager.Fields.Add(fieldInfo);
				}
			}
			foreach (MethodInfo methodInfo in type.GetMethods(BindingFlags.Static | BindingFlags.Public))
			{
				methodInfo.RegisterModExport(null);
				methodInfo.RegisterModExport(text);
			}
			foreach (FieldInfo fieldInfo2 in ModInteropManager.Fields)
			{
				List<MethodInfo> list;
				if (!ModInteropManager.Methods.TryGetValue(fieldInfo2.GetModImportName(), out list))
				{
					fieldInfo2.SetValue(null, null);
				}
				else
				{
					bool flag = false;
					foreach (MethodInfo methodInfo2 in list)
					{
						try
						{
							fieldInfo2.SetValue(null, Delegate.CreateDelegate(fieldInfo2.FieldType, null, methodInfo2));
							flag = true;
							break;
						}
						catch
						{
						}
					}
					if (!flag)
					{
						fieldInfo2.SetValue(null, null);
					}
				}
			}
		}

		public static void RegisterModExport(this MethodInfo method, [Nullable(2)] string prefix = null)
		{
			Helpers.ThrowIfArgumentNull<MethodInfo>(method, "method");
			if (!method.IsPublic || !method.IsStatic)
			{
				throw new MemberAccessException("Utility must be public static");
			}
			string text = method.Name;
			if (!string.IsNullOrEmpty(prefix))
			{
				text = prefix + "." + text;
			}
			List<MethodInfo> list;
			if (!ModInteropManager.Methods.TryGetValue(text, out list))
			{
				list = (ModInteropManager.Methods[text] = new List<MethodInfo>());
			}
			if (!list.Contains(method))
			{
				list.Add(method);
			}
		}

		private static string GetModImportName(this FieldInfo field)
		{
			object[] array = field.GetCustomAttributes(typeof(ModImportNameAttribute), false);
			int num = 0;
			if (num >= array.Length)
			{
				if (field.DeclaringType != null)
				{
					array = field.DeclaringType.GetCustomAttributes(typeof(ModImportNameAttribute), false);
					num = 0;
					if (num < array.Length)
					{
						return ((ModImportNameAttribute)array[num]).Name + "." + field.Name;
					}
				}
				return field.Name;
			}
			return ((ModImportNameAttribute)array[num]).Name;
		}

		private static HashSet<Type> Registered = new HashSet<Type>();

		private static Dictionary<string, List<MethodInfo>> Methods = new Dictionary<string, List<MethodInfo>>();

		private static List<FieldInfo> Fields = new List<FieldInfo>();
	}
}
