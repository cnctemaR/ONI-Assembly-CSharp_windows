using System;

namespace UnityEngine.Timeline
{
	public interface INotificationOptionProvider
	{
		NotificationFlags flags { get; }
	}
}
