using System;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/Notifier")]
public class Notifier : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		Components.Notifiers.Add(this);
	}

	protected override void OnCleanUp()
	{
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
		if (DebugHandler.NotificationsDisabled)
		{
			return;
		}
		DebugUtil.DevAssert(notification != null, "Trying to add null notification. It's safe to continue playing, the notification won't be displayed.", null);
		if (notification == null)
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
		if (notification == null)
		{
			return;
		}
		if (notification.Notifier != null)
		{
			notification.Notifier = null;
			if (this.OnRemove != null)
			{
				this.OnRemove(notification);
			}
		}
	}

	[MyCmpGet]
	private KSelectable Selectable;

	public Action<Notification> OnAdd;

	public Action<Notification> OnRemove;

	public bool DisableNotifications;

	public bool AutoClickFocus = true;
}
