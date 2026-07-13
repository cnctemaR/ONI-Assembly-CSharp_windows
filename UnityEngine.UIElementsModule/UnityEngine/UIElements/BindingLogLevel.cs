using System;

namespace UnityEngine.UIElements
{
	public enum BindingLogLevel
	{
		[InspectorName("No logs")]
		None,
		[InspectorName("One log per result")]
		Once,
		[InspectorName("All logs")]
		All
	}
}
