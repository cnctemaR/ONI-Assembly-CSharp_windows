using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Unity.Scripting.LifecycleManagement;

namespace UnityEngine.Assemblies
{
	internal static class CurrentAssemblies
	{
		private static CurrentAssemblies.AssemblyLoadContextStateHelper GetAssemblyLoadContextStateHelperImpl()
		{
			Type type = Type.GetType("System.Runtime.Loader.AssemblyLoadContext");
			MethodInfo methodInfo = ((type != null) ? type.GetMethod("GetLoadContext", BindingFlags.Static | BindingFlags.Public) : null);
			bool flag = methodInfo == null;
			CurrentAssemblies.AssemblyLoadContextStateHelper assemblyLoadContextStateHelper;
			if (flag)
			{
				assemblyLoadContextStateHelper = default(CurrentAssemblies.AssemblyLoadContextStateHelper);
			}
			else
			{
				Type declaringType = methodInfo.DeclaringType;
				FieldInfo fieldInfo = ((declaringType != null) ? declaringType.GetField("_state", BindingFlags.Instance | BindingFlags.NonPublic) : null);
				CurrentAssemblies.AssemblyLoadContextStateHelper assemblyLoadContextStateHelper2 = new CurrentAssemblies.AssemblyLoadContextStateHelper
				{
					GetAssemblyLoadContextMethod = methodInfo,
					AssemblyLoadContextStateField = fieldInfo
				};
				assemblyLoadContextStateHelper = assemblyLoadContextStateHelper2;
			}
			return assemblyLoadContextStateHelper;
		}

		private static bool IsFromLiveAssemblyLoadContext(Assembly assembly)
		{
			object obj = CurrentAssemblies.k_AssemblyLoadContextStateHelper.GetAssemblyLoadContextMethod.Invoke(null, new object[] { assembly });
			bool flag = obj == null;
			bool flag2;
			if (flag)
			{
				flag2 = true;
			}
			else
			{
				FieldInfo assemblyLoadContextStateField = CurrentAssemblies.k_AssemblyLoadContextStateHelper.AssemblyLoadContextStateField;
				object obj2 = ((assemblyLoadContextStateField != null) ? assemblyLoadContextStateField.GetValue(obj) : null);
				bool flag3 = Convert.ToInt32(obj2) == 0;
				flag2 = flag3;
			}
			return flag2;
		}

		internal static IReadOnlyList<Assembly> GetLoadedAssemblies()
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			bool flag = CurrentAssemblies.k_AssemblyLoadContextStateHelper.GetAssemblyLoadContextMethod == null;
			IReadOnlyList<Assembly> readOnlyList;
			if (flag)
			{
				readOnlyList = assemblies;
			}
			else
			{
				List<Assembly> list = new List<Assembly>();
				foreach (Assembly assembly in assemblies)
				{
					bool flag2 = CurrentAssemblies.IsFromLiveAssemblyLoadContext(assembly);
					if (flag2)
					{
						list.Add(assembly);
					}
				}
				readOnlyList = list;
			}
			return readOnlyList;
		}

		internal static Assembly LoadFromPath(string assemblyPath)
		{
			bool flag = !Path.IsPathFullyQualified(assemblyPath);
			if (flag)
			{
				throw new ArgumentException("Assembly path must be fully qualified", "assemblyPath");
			}
			return Assembly.LoadFrom(assemblyPath);
		}

		internal static Assembly LoadFromBytes(byte[] rawAssembly)
		{
			return CurrentAssemblies.LoadFromBytes(rawAssembly, null);
		}

		internal static Assembly LoadFromBytes(byte[] rawAssembly, byte[] rawSymbolStore)
		{
			bool flag = rawAssembly == null;
			if (flag)
			{
				throw new ArgumentNullException("rawAssembly");
			}
			bool flag2 = rawAssembly.Length == 0;
			if (flag2)
			{
				throw new BadImageFormatException("Empty raw assembly byte array");
			}
			bool flag3 = rawSymbolStore != null && rawSymbolStore.Length == 0;
			if (flag3)
			{
				throw new BadImageFormatException("Empty raw assembly symbols byte array");
			}
			return Assembly.Load(rawAssembly, rawSymbolStore);
		}

		[NoAutoStaticsCleanup]
		private static readonly CurrentAssemblies.AssemblyLoadContextStateHelper k_AssemblyLoadContextStateHelper = CurrentAssemblies.GetAssemblyLoadContextStateHelperImpl();

		private struct AssemblyLoadContextStateHelper
		{
			public MethodInfo GetAssemblyLoadContextMethod;

			public FieldInfo AssemblyLoadContextStateField;
		}
	}
}
