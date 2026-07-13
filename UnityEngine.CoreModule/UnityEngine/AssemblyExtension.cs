using System;
using System.Reflection;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Export/Scripting/ScriptingRuntime.h")]
	internal static class AssemblyExtension
	{
		public static string GetLoadedAssemblyPath(this Assembly assembly)
		{
			bool flag = assembly == null;
			if (flag)
			{
				throw new ArgumentNullException("assembly");
			}
			return assembly.Location;
		}
	}
}
