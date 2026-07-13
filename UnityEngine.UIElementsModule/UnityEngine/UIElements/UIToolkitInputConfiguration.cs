using System;

namespace UnityEngine.UIElements
{
	[Obsolete("All the methods in this class are obsolete.")]
	public static class UIToolkitInputConfiguration
	{
		[Obsolete("This method should not be used. Setting a backend other than the Default backend may lead to unexpected results or undefined behavior.")]
		public static void SetRuntimeInputBackend(UIToolkitInputBackendOption backend)
		{
			UIElementsRuntimeUtility.defaultEventSystem.useInputForUI = backend != UIToolkitInputBackendOption.LegacyBackend;
		}
	}
}
