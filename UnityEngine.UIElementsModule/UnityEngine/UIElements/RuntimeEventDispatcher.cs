using System;

namespace UnityEngine.UIElements
{
	internal static class RuntimeEventDispatcher
	{
		public static EventDispatcher Create()
		{
			return EventDispatcher.CreateDefault();
		}
	}
}
