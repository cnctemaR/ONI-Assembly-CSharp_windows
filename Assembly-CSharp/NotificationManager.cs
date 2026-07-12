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
		Components.Notifiers.OnAdd += this.OnAddNotifier;
		Components.Notifiers.OnRemove += this.OnRemoveNotifier;
		foreach (Notifier notifier in Components.Notifiers.Items)
		{
			this.OnAddNotifier(notifier);
		}
	}

	protected override void OnForcedCleanUp()
	{
		NotificationManager.Instance = null;
	}

	private void OnAddNotifier(Notifier notifier)
	{
		notifier.OnAdd = (Action<Notification>)Delegate.Combine(notifier.OnAdd, new Action<Notification>(this.OnAddNotification));
		notifier.OnRemove = (Action<Notification>)Delegate.Combine(notifier.OnRemove, new Action<Notification>(this.OnRemoveNotification));
	}

	private void OnRemoveNotifier(Notifier notifier)
	{
		notifier.OnAdd = (Action<Notification>)Delegate.Remove(notifier.OnAdd, new Action<Notification>(this.OnAddNotification));
		notifier.OnRemove = (Action<Notification>)Delegate.Remove(notifier.OnRemove, new Action<Notification>(this.OnRemoveNotification));
	}

	private void OnAddNotification(Notification notification)
	{
		this.pendingNotifications.Add(notification);
	}

	private void OnRemoveNotification(Notification notification)
	{
		this.pendingNotifications.Remove(notification);
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
