using System;
using KSerialization;

public abstract class TargetMessage : Message
{
	protected TargetMessage()
	{
	}

	public TargetMessage(KPrefabID prefab_id)
	{
		this.target = new MessageTarget(prefab_id);
	}

	public MessageTarget GetTarget()
	{
		return this.target;
	}

	public override void OnCleanUp()
	{
		this.target.OnCleanUp();
	}

	[Serialize]
	private MessageTarget target;
}
