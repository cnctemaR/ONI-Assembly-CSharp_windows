using System;
using UnityEngine;

public class StandardMessageDialog : MessageDialog
{
	public override bool CanDisplay(Message message)
	{
		return typeof(Message).IsAssignableFrom(message.GetType());
	}

	public override void SetMessage(Message base_message)
	{
		this.message = base_message;
		this.description.text = this.message.GetMessageBody();
	}

	public override void OnClickAction()
	{
	}

	[SerializeField]
	private LocText description;

	private Message message;
}
