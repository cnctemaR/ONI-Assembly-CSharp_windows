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
		this.assignables = consumer.GetComponent<Assignables>();
		this.solidTransferArm = consumer.GetComponent<SolidTransferArm>();
		this.hasSolidTransferArm = this.solidTransferArm != null;
		this.resume = consumer.GetComponent<MinionResume>();
		this.choreDriver = consumer.GetComponent<ChoreDriver>();
		this.schedulable = consumer.GetComponent<Schedulable>();
		this.traits = consumer.GetComponent<Traits>();
		this.choreProvider = consumer.GetComponent<ChoreProvider>();
		this.equipment = consumer.GetComponent<Equipment>();
		this.storage = consumer.GetComponent<Storage>();
		this.consumableConsumer = consumer.GetComponent<ConsumableConsumer>();
		this.worker = consumer.GetComponent<Worker>();
	}

	public void Refresh()
	{
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

	public Worker worker;

	public SolidTransferArm solidTransferArm;

	public bool hasSolidTransferArm;
}
