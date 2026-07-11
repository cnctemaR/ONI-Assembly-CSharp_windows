using System;
using System.Reflection;
using NodeEditorFramework.Utilities;

namespace NodeEditorFramework
{
	[AttributeUsage(AttributeTargets.Method)]
	public class ContextFillerAttribute : Attribute
	{
		public ContextFillerAttribute(ContextType type)
		{
			this.contextType = type;
		}

		public ContextType contextType { get; private set; }

		internal static bool AssureValidity(MethodInfo method, ContextFillerAttribute attr)
		{
			if (!method.IsGenericMethod && !method.IsGenericMethodDefinition && (method.ReturnType == null || method.ReturnType == typeof(void)))
			{
				ParameterInfo[] parameters = method.GetParameters();
				if (parameters.Length == 2 && parameters[0].ParameterType == typeof(NodeEditorInputInfo) && parameters[1].ParameterType == typeof(GenericMenu))
				{
					return true;
				}
				Debug.LogWarning("Method " + method.Name + " has incorrect signature for ContextAttribute!");
			}
			return false;
		}
	}
}
