using System;
using System.Collections.Generic;

public class MessageNotification : Notification
{
	public MessageNotification(Message m)
	{
		string title = m.GetTitle();
		NotificationType notificationType = NotificationType.Messages;
		HashedString invalid = HashedString.Invalid;
		string sound = m.GetSound();
		base..ctor(title, notificationType, invalid, null, null, false, 0f, null, null, sound);
		MessageNotification $this = this;
		this.message = m;
		if (!this.message.PlayNotificationSound())
		{
			this.playSound = false;
		}
		base.ToolTip = (List<Notification> notifications, object data) => $this.OnToolTip(notifications, m.GetTooltip());
		this.hasLocation = false;
	}

	private string OnToolTip(List<Notification> notifications, string tooltipText)
	{
		return tooltipText;
	}

	public Message message;
}
