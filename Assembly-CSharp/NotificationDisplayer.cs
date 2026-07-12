using System;
using System.Collections.Generic;

public abstract class NotificationDisplayer : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		this.displayedNotifications = new List<Notification>();
		NotificationManager.Instance.notificationAdded += this.NotificationAdded;
		NotificationManager.Instance.notificationRemoved += this.NotificationRemoved;
	}

	public void NotificationAdded(Notification notification)
	{
		if (this.ShouldDisplayNotification(notification))
		{
			this.displayedNotifications.Add(notification);
			this.OnNotificationAdded(notification);
		}
	}

	protected abstract void OnNotificationAdded(Notification notification);

	public void NotificationRemoved(Notification notification)
	{
		if (this.displayedNotifications.Contains(notification))
		{
			this.displayedNotifications.Remove(notification);
			this.OnNotificationRemoved(notification);
		}
	}

	protected abstract void OnNotificationRemoved(Notification notification);

	protected abstract bool ShouldDisplayNotification(Notification notification);

	protected List<Notification> displayedNotifications;
}
