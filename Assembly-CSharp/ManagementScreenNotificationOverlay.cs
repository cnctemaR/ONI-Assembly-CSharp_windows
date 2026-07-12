using System;
using System.Collections.Generic;
using UnityEngine;

public class ManagementScreenNotificationOverlay : KMonoBehaviour
{
	protected void OnEnable()
	{
	}

	protected override void OnDisable()
	{
	}

	private NotificationAlertBar CreateAlertBar(ManagementMenuNotification notification)
	{
		NotificationAlertBar notificationAlertBar = Util.KInstantiateUI<NotificationAlertBar>(this.alertBarPrefab.gameObject, this.alertContainer.gameObject, false);
		notificationAlertBar.Init(notification);
		notificationAlertBar.gameObject.SetActive(true);
		return notificationAlertBar;
	}

	private void NotificationsChanged()
	{
	}

	public global::Action currentMenu;

	public NotificationAlertBar alertBarPrefab;

	public RectTransform alertContainer;

	private List<NotificationAlertBar> alertBars = new List<NotificationAlertBar>();
}
