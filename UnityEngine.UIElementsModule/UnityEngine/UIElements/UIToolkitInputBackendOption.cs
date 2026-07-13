using System;

namespace UnityEngine.UIElements
{
	[Obsolete("This enum only has one supported value. The Default backend should always be used, as it is more stable and covers all use cases.")]
	public enum UIToolkitInputBackendOption
	{
		Default,
		InputSystemCompatibleBackend = 0,
		[Obsolete("The Legacy backend is not supported anymore. Use the default backend, which is more stable and covers all use cases already.")]
		LegacyBackend
	}
}
