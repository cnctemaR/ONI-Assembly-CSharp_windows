using System;
using KSerialization;
using STRINGS;

public class RoleMasteredMessage : Message
{
	public RoleMasteredMessage()
	{
	}

	public RoleMasteredMessage(MinionResume resume)
	{
		this.description = new Tuple<string, string>(resume.GetProperName(), resume.CurrentRole);
	}

	public override string GetSound()
	{
		return "AI_Notification_ResearchComplete";
	}

	public override string GetMessageBody()
	{
		string text = string.Format(MISC.NOTIFICATIONS.ROLEMASTERED.LINE, this.description.first, Game.Instance.roleManager.GetRole(this.description.second).name);
		return string.Format(MISC.NOTIFICATIONS.ROLEMASTERED.MESSAGEBODY, text);
	}

	public override string GetTitle()
	{
		return MISC.NOTIFICATIONS.ROLEMASTERED.NAME;
	}

	public override string GetTooltip()
	{
		return string.Format(MISC.NOTIFICATIONS.ROLEMASTERED.TOOLTIP, string.Empty);
	}

	public override bool IsValid()
	{
		return true;
	}

	[Serialize]
	private Tuple<string, string> description;
}
