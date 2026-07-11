using System;
using KSerialization;
using STRINGS;

public class SkillMasteredMessage : Message
{
	public SkillMasteredMessage()
	{
	}

	public SkillMasteredMessage(MinionResume resume)
	{
		this.minionName = resume.GetProperName();
	}

	public override string GetSound()
	{
		return "AI_Notification_ResearchComplete";
	}

	public override string GetMessageBody()
	{
		Debug.Assert(this.minionName != null);
		string text = string.Format(MISC.NOTIFICATIONS.SKILL_POINT_EARNED.LINE, this.minionName);
		return string.Format(MISC.NOTIFICATIONS.SKILL_POINT_EARNED.MESSAGEBODY, text);
	}

	public override string GetTitle()
	{
		return MISC.NOTIFICATIONS.SKILL_POINT_EARNED.NAME;
	}

	public override string GetTooltip()
	{
		return string.Format(MISC.NOTIFICATIONS.SKILL_POINT_EARNED.TOOLTIP, "");
	}

	public override bool IsValid()
	{
		return this.minionName != null;
	}

	[Serialize]
	private string minionName;
}
