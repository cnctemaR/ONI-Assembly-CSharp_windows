using System;
using System.Collections.Generic;

public class MessageNotification : Notification
{
	public MessageNotification(Message m)
		: base(m.GetTitle(), NotificationType.Messages, HashedString.Invalid, null, null, false, 0f, null, null, null)
	{
		MessageNotification $this = this;
		this.message = m;
		if (!this.message.PlayNotificationSound())
		{
			this.playSound = false;
		}
		base.ToolTip = (List<Notification> notifications, object data) => $this.OnToolTip(notifications, m.GetTooltip());
		base.clickFocus = null;
	}

	private string OnToolTip(List<Notification> notifications, string tooltipText)
	{
		return tooltipText;
	}

	public Message message;
}
