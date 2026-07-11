using System;

namespace UnityEngine.Timeline
{
	[Flags]
	[Serializable]
	public enum NotificationFlags : short
	{
		TriggerInEditMode = 1,
		Retroactive = 2,
		TriggerOnce = 4
	}
}
