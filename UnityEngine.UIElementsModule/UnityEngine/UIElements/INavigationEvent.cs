using System;

namespace UnityEngine.UIElements
{
	public interface INavigationEvent
	{
		EventModifiers modifiers { get; }

		NavigationDeviceType deviceType { get; }

		bool shiftKey { get; }

		bool ctrlKey { get; }

		bool commandKey { get; }

		bool altKey { get; }

		bool actionKey { get; }
	}
}
