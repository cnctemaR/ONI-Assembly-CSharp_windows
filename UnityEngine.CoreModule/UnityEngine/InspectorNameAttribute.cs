using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	[UsedByNativeCode]
	public class InspectorNameAttribute : PropertyAttribute
	{
		public InspectorNameAttribute(string displayName)
			: base(true)
		{
			this.displayName = displayName;
		}

		public readonly string displayName;
	}
}
