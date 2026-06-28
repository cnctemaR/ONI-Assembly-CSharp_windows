using System;

public class NotificationTracker : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	public void OnRecieveNotification(Notification notification)
	{
	}

	private void SetNotification(StringKey key)
	{
	}

	public static void ClearNotificationTracker()
	{
	}

	public ClippyPanel clippyPanelPrefab;
}
