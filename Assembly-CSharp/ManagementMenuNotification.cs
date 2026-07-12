using System;
using System.Collections.Generic;
using UnityEngine;

public class ManagementMenuNotification : Notification
{
	public bool hasBeenViewed { get; private set; }

	public string highlightTarget { get; set; }

	public ManagementMenuNotification(global::Action targetMenu, NotificationValence valence, string highlightTarget, string title, NotificationType type, Func<List<Notification>, object, string> tooltip = null, object tooltip_data = null, bool expires = true, float delay = 0f, Notification.ClickCallback custom_click_callback = null, object custom_click_data = null, Transform click_focus = null, bool volume_attenuation = true)
		: base(title, type, tooltip, tooltip_data, expires, delay, custom_click_callback, custom_click_data, click_focus, volume_attenuation, false)
	{
		this.targetMenu = targetMenu;
		this.valence = valence;
		this.highlightTarget = highlightTarget;
	}

	public void View()
	{
		this.hasBeenViewed = true;
		ManagementMenu.Instance.notificationDisplayer.NotificationWasViewed(this);
	}

	public global::Action targetMenu;

	public NotificationValence valence;
}
