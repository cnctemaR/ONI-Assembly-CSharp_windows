using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public class InspectorNameAttribute : PropertyAttribute
	{
		public InspectorNameAttribute(string displayName)
		{
			this.displayName = displayName;
		}

		public readonly string displayName;
	}
}
