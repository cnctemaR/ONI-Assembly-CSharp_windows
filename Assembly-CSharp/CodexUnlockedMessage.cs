using System;
using STRINGS;

public class CodexUnlockedMessage : Message
{
	public CodexUnlockedMessage()
	{
	}

	public CodexUnlockedMessage(string lock_id, string unlock_message)
	{
		this.lockId = lock_id;
		this.unlockMessage = unlock_message;
	}

	public string GetLockId()
	{
		return this.lockId;
	}

	public override string GetSound()
	{
		return "AI_Notification_ResearchComplete";
	}

	public override string GetMessageBody()
	{
		return UI.CODEX.CODEX_DISCOVERED_MESSAGE.BODY.Replace("{codex}", this.unlockMessage);
	}

	public override string GetTitle()
	{
		return UI.CODEX.CODEX_DISCOVERED_MESSAGE.TITLE;
	}

	public override string GetTooltip()
	{
		return this.GetMessageBody();
	}

	public override bool IsValid()
	{
		return true;
	}

	private string unlockMessage;

	private string lockId;
}
