using System;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
	public class ContextMenuItemAttribute : PropertyAttribute
	{
		public ContextMenuItemAttribute(string name, string function)
		{
			this.name = name;
			this.function = function;
		}

		public readonly string name;

		public readonly string function;
	}
}
