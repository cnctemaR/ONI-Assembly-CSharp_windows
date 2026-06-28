using System;
using System.Reflection;

namespace UnityEngine
{
	public static class Types
	{
		public static Type GetType(string typeName, string assemblyName)
		{
			Type type;
			try
			{
				type = Assembly.Load(assemblyName).GetType(typeName);
			}
			catch (Exception)
			{
				type = null;
			}
			return type;
		}
	}
}
