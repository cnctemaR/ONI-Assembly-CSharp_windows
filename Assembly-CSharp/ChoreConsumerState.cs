using System;
using Klei.AI;
using UnityEngine;

public class ChoreConsumerState
{
	public ChoreConsumerState(ChoreConsumer consumer)
	{
		this.consumer = consumer;
		this.navigator = consumer.GetComponent<Navigator>();
		this.prefabid = consumer.GetComponent<KPrefabID>();
		this.ownable = consumer.GetComponent<Ownable>();
		this.gameObject = consumer.gameObject;
		this.solidTransferArm = consumer.GetComponent<SolidTransferArm>();
		this.hasSolidTransferArm = this.solidTransferArm != null;
		this.resume = consumer.GetComponent<MinionResume>();
		this.choreDriver = consumer.GetComponent<ChoreDriver>();
		this.schedulable = consumer.GetComponent<Schedulable>();
		this.traits = consumer.GetComponent<Traits>();
		this.choreProvider = consumer.GetComponent<ChoreProvider>();
		MinionIdentity component = consumer.GetComponent<MinionIdentity>();
		if (component != null)
		{
			if (component.assignableProxy == null)
			{
				component.assignableProxy = MinionAssignablesProxy.InitAssignableProxy(component.assignableProxy, component);
			}
			this.assignables = component.GetSoleOwner();
			this.equipment = component.GetEquipment();
		}
		else
		{
			this.assignables = consumer.GetComponent<Assignables>();
			this.equipment = consumer.GetComponent<Equipment>();
		}
		this.storage = consumer.GetComponent<Storage>();
		this.consumableConsumer = consumer.GetComponent<ConsumableConsumer>();
		this.worker = consumer.GetComponent<Worker>();
		this.selectable = consumer.GetComponent<KSelectable>();
		if (this.schedulable != null)
		{
			int blockIdx = Schedule.GetBlockIdx();
			this.scheduleBlock = this.schedulable.GetSchedule().GetBlock(blockIdx);
		}
	}

	public void Refresh()
	{
		if (this.schedulable != null)
		{
			int blockIdx = Schedule.GetBlockIdx();
			Schedule schedule = this.schedulable.GetSchedule();
			if (schedule != null)
			{
				this.scheduleBlock = schedule.GetBlock(blockIdx);
			}
		}
	}

	public KPrefabID prefabid;

	public GameObject gameObject;

	public ChoreConsumer consumer;

	public ChoreProvider choreProvider;

	public Navigator navigator;

	public Ownable ownable;

	public Assignables assignables;

	public MinionResume resume;

	public ChoreDriver choreDriver;

	public Schedulable schedulable;

	public Traits traits;

	public Equipment equipment;

	public Storage storage;

	public ConsumableConsumer consumableConsumer;

	public KSelectable selectable;

	public Worker worker;

	public SolidTransferArm solidTransferArm;

	public bool hasSolidTransferArm;

	public ScheduleBlock scheduleBlock;
}
