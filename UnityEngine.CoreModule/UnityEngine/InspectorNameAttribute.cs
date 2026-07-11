using System;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	internal class InspectorNameAttribute : PropertyAttribute
	{
		public InspectorNameAttribute(string displayName)
		{
			this.displayName = displayName;
		}

		public readonly string displayName;
	}
}
