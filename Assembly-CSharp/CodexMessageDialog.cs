using System;
using UnityEngine;

public class CodexMessageDialog : MessageDialog
{
	public override bool CanDisplay(Message message)
	{
		return typeof(CodexUnlockedMessage).IsAssignableFrom(message.GetType());
	}

	public override void SetMessage(Message base_message)
	{
		this.message = (CodexUnlockedMessage)base_message;
		this.description.text = this.message.GetMessageBody();
	}

	public override void OnClickAction()
	{
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		this.message.OnCleanUp();
	}

	[SerializeField]
	private LocText description;

	private CodexUnlockedMessage message;
}
