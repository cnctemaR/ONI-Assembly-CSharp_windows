using System;
using KSerialization;
using STRINGS;

public class WorldDetectedMessage : Message
{
	public WorldDetectedMessage()
	{
	}

	public WorldDetectedMessage(WorldContainer world)
	{
		this.worldID = world.id;
	}

	public override string GetSound()
	{
		return "AI_Notification_ResearchComplete";
	}

	public override string GetMessageBody()
	{
		WorldContainer world = ClusterManager.Instance.GetWorld(this.worldID);
		return string.Format(MISC.NOTIFICATIONS.WORLDDETECTED.MESSAGEBODY, world.GetProperName());
	}

	public override string GetTitle()
	{
		return MISC.NOTIFICATIONS.WORLDDETECTED.NAME;
	}

	public override string GetTooltip()
	{
		WorldContainer world = ClusterManager.Instance.GetWorld(this.worldID);
		return string.Format(MISC.NOTIFICATIONS.WORLDDETECTED.TOOLTIP, world.GetProperName());
	}

	public override bool IsValid()
	{
		return this.worldID != 255;
	}

	[Serialize]
	private int worldID;
}
