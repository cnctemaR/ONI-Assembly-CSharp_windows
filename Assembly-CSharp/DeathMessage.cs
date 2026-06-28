using System;
using KSerialization;
using STRINGS;
using UnityEngine;

public class DeathMessage : TargetMessage
{
	public DeathMessage()
	{
	}

	public DeathMessage(GameObject go, Death death)
		: base(go.GetComponent<KPrefabID>())
	{
		this.death.Set(death);
	}

	public override string GetSound()
	{
		return string.Empty;
	}

	public override string GetTitle()
	{
		return MISC.NOTIFICATIONS.DUPLICANTDIED.NAME;
	}

	public override string GetTooltip()
	{
		return this.GetMessageBody();
	}

	public override string GetMessageBody()
	{
		string description = this.death.Get().description;
		return description.Replace("{Target}", base.GetTarget().GetName());
	}

	[Serialize]
	private ResourceRef<Death> death = new ResourceRef<Death>();
}
