using System;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
public class Notifier : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		Components.Notifiers.Add(this);
	}

	protected override void OnCleanUp()
	{
		this.ClearNotifications();
		Components.Notifiers.Remove(this);
	}

	public void Add(Notification notification, string suffix = "")
	{
		if (KScreenManager.Instance == null)
		{
			return;
		}
		if (this.DisableNotifications)
		{
			return;
		}
		if (notification.Notifier == null)
		{
			if (this.Selectable != null)
			{
				notification.NotifierName = "• " + this.Selectable.GetName() + suffix;
			}
			else
			{
				notification.NotifierName = "• " + base.name + suffix;
			}
			notification.Notifier = this;
			if (this.AutoClickFocus && notification.clickFocus == null)
			{
				notification.clickFocus = base.transform;
			}
			if (notification.Group.IsValid && notification.Group != "")
			{
				if (this.NotificationGroups == null)
				{
					this.NotificationGroups = new Dictionary<HashedString, Notification>();
				}
				Notification notification2;
				this.NotificationGroups.TryGetValue(notification.Group, out notification2);
				if (notification2 != null)
				{
					this.Remove(notification2);
				}
				this.NotificationGroups[notification.Group] = notification;
			}
			if (this.OnAdd != null)
			{
				this.OnAdd(notification);
			}
			notification.GameTime = Time.time;
		}
		else
		{
			DebugUtil.Assert(notification.Notifier == this);
		}
		notification.Time = KTime.Instance.UnscaledGameTime;
	}

	public void Remove(Notification notification)
	{
		if (notification.Notifier != null)
		{
			notification.Notifier = null;
			if (this.NotificationGroups != null && notification.Group.IsValid && notification.Group != "")
			{
				this.NotificationGroups.Remove(notification.Group);
			}
			if (this.OnRemove != null)
			{
				this.OnRemove(notification);
			}
		}
	}

	public void ClearNotifications()
	{
		if (this.NotificationGroups != null)
		{
			foreach (HashedString hashedString in new List<HashedString>(this.NotificationGroups.Keys))
			{
				this.Remove(this.NotificationGroups[hashedString]);
			}
		}
	}

	[MyCmpGet]
	private KSelectable Selectable;

	public Action<Notification> OnAdd;

	public Action<Notification> OnRemove;

	public bool DisableNotifications;

	public bool AutoClickFocus = true;

	private Dictionary<HashedString, Notification> NotificationGroups;
}
