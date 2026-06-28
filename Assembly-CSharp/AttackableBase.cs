using System;

public class AttackableBase : Workable, IApproachable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.Subscribe(809822742, new EventSystem.EventHandler(this.OnCollectPriorityCommands));
	}

	private void OnCollectPriorityCommands(object data)
	{
		PriorityCommandCollector priorityCommandCollector = (PriorityCommandCollector)data;
		if (base.GetComponent<MinionIdentity>() == null)
		{
			priorityCommandCollector.commands.Add(new PriorityAttackCommand(priorityCommandCollector.consumer, this));
		}
	}
}
