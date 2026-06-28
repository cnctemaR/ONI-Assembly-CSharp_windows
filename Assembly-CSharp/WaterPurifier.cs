using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class WaterPurifier : StateMachineComponent<WaterPurifier.StatesInstance>
{
	public int DescriptionOrder { get; set; }

	public bool HasRequiredElements()
	{
		return this.converter.HasEnoughMass();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.deliveryComponents = base.GetComponents<ManualDeliveryKG>();
		this.OnConduitConnectionChanged(base.GetComponent<ConduitConsumer>().IsConnected);
		this.Subscribe(-2094018600, new EventSystem.EventHandler(this.OnConduitConnectionChanged));
		base.smi.StartSM();
	}

	private void OnConduitConnectionChanged(object data)
	{
		bool flag = (bool)data;
		foreach (ManualDeliveryKG manualDeliveryKG in this.deliveryComponents)
		{
			Element element = ElementLoader.GetElement(manualDeliveryKG.requestedItemTag);
			if (element != null && element.IsLiquid)
			{
				manualDeliveryKG.Pause(flag, "pipe connected");
			}
		}
	}

	[MyCmpGet]
	private Operational operational;

	[MyCmpGet]
	private ElementConverter converter;

	private ManualDeliveryKG[] deliveryComponents;

	public class StatesInstance : GameStateMachine<WaterPurifier.States, WaterPurifier.StatesInstance, WaterPurifier>.GameInstance
	{
		public StatesInstance(WaterPurifier smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<WaterPurifier.States, WaterPurifier.StatesInstance, WaterPurifier>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.off;
			this.off.PlayAnim("off", KAnim.PlayMode.Once, null).EventTransition(GameHashes.OperationalChanged, this.on, (WaterPurifier.StatesInstance smi) => smi.master.operational.IsOperational);
			this.on.PlayAnim("on", KAnim.PlayMode.Once, null).EventTransition(GameHashes.OperationalChanged, this.off, (WaterPurifier.StatesInstance smi) => !smi.master.operational.IsOperational).DefaultState(this.on.waiting);
			this.on.waiting.EventTransition(GameHashes.OnStorageChange, this.on.working_pre, (WaterPurifier.StatesInstance smi) => smi.master.HasRequiredElements());
			this.on.working_pre.QueueAnim("working_pre", false, null).OnAnimQueueComplete(this.on.working);
			this.on.working.Enter(delegate(WaterPurifier.StatesInstance smi)
			{
				smi.master.operational.SetActive(true, false);
			}).QueueAnim("working_loop", true, null).EventTransition(GameHashes.OnStorageChange, this.on.working_pst, (WaterPurifier.StatesInstance smi) => !smi.master.HasRequiredElements())
				.Exit(delegate(WaterPurifier.StatesInstance smi)
				{
					smi.master.operational.SetActive(false, false);
				});
			this.on.working_pst.QueueAnim("working_pst", false, null).OnAnimQueueComplete(this.on.waiting);
		}

		public GameStateMachine<WaterPurifier.States, WaterPurifier.StatesInstance, WaterPurifier>.State off;

		public WaterPurifier.States.OnStates on;

		public class OnStates : GameStateMachine<WaterPurifier.States, WaterPurifier.StatesInstance, WaterPurifier>.State
		{
			public GameStateMachine<WaterPurifier.States, WaterPurifier.StatesInstance, WaterPurifier>.State waiting;

			public GameStateMachine<WaterPurifier.States, WaterPurifier.StatesInstance, WaterPurifier>.State working_pre;

			public GameStateMachine<WaterPurifier.States, WaterPurifier.StatesInstance, WaterPurifier>.State working;

			public GameStateMachine<WaterPurifier.States, WaterPurifier.StatesInstance, WaterPurifier>.State working_pst;
		}
	}
}
