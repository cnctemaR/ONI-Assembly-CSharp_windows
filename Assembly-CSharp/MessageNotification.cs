using System;
using System.Collections.Generic;

public class MessageNotification : Notification
{
	public MessageNotification(Message m)
	{
		string sound = m.GetSound();
		base..ctor(m.GetTitle(), NotificationType.Messages, null, null, null, false, 0f, null, null, sound);
		MessageNotification <>f__this = this;
		this.message = m;
		base.ToolTip = (List<Notification> notifications, object data) => <>f__this.OnToolTip(notifications, m.GetTooltip());
		this.hasLocation = false;
	}

	private string OnToolTip(List<Notification> notifications, string tooltipText)
	{
		return tooltipText;
	}

	public Message message;
}
