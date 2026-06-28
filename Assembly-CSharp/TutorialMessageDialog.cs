using System;
using UnityEngine;

public class TutorialMessageDialog : MessageDialog
{
	public override bool CanDontShowAgain
	{
		get
		{
			return true;
		}
	}

	public override bool CanDisplay(Message message)
	{
		return typeof(TutorialMessage).IsAssignableFrom(message.GetType());
	}

	public override void SetMessage(Message base_message)
	{
		this.message = base_message as TutorialMessage;
		this.description.text = this.message.GetMessageBody();
	}

	public override void OnClickAction()
	{
	}

	public override void OnDontShowAgain()
	{
		Tutorial.Instance.HideTutorialMessage(this.message.messageId);
	}

	[SerializeField]
	private LocText description;

	private TutorialMessage message;
}
