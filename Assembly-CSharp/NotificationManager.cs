using System;
using System.Collections.Generic;

public class NotificationManager : KMonoBehaviour
{
	public static NotificationManager Instance { get; private set; }

	public event Action<Notification> notificationAdded;

	public event Action<Notification> notificationRemoved;

	protected override void OnPrefabInit()
	{
		Debug.Assert(NotificationManager.Instance == null);
		NotificationManager.Instance = this;
	}

	protected override void OnForcedCleanUp()
	{
		NotificationManager.Instance = null;
	}

	public void AddNotification(Notification notification)
	{
		this.pendingNotifications.Add(notification);
		if (NotificationScreen.Instance != null)
		{
			NotificationScreen.Instance.AddPendingNotification(notification);
		}
	}

	public void RemoveNotification(Notification notification)
	{
		this.pendingNotifications.Remove(notification);
		if (NotificationScreen.Instance != null)
		{
			NotificationScreen.Instance.RemovePendingNotification(notification);
		}
		if (this.notifications.Remove(notification))
		{
			this.notificationRemoved(notification);
		}
	}

	private void Update()
	{
		int i = 0;
		while (i < this.pendingNotifications.Count)
		{
			if (this.pendingNotifications[i].IsReady())
			{
				this.DoAddNotification(this.pendingNotifications[i]);
				this.pendingNotifications.RemoveAt(i);
			}
			else
			{
				i++;
			}
		}
	}

	private void DoAddNotification(Notification notification)
	{
		this.notifications.Add(notification);
		if (this.notificationAdded != null)
		{
			this.notificationAdded(notification);
		}
	}

	private List<Notification> pendingNotifications = new List<Notification>();

	private List<Notification> notifications = new List<Notification>();
}
