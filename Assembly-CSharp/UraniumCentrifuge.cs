using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class UraniumCentrifuge : StateMachineComponent<UraniumCentrifuge.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	[MyCmpAdd]
	private Storage storage;

	[MyCmpReq]
	private Operational operational;

	public class StatesInstance : GameStateMachine<UraniumCentrifuge.States, UraniumCentrifuge.StatesInstance, UraniumCentrifuge, object>.GameInstance
	{
		public StatesInstance(UraniumCentrifuge smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<UraniumCentrifuge.States, UraniumCentrifuge.StatesInstance, UraniumCentrifuge>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.disabled;
			this.root.EventTransition(GameHashes.OperationalChanged, this.disabled, (UraniumCentrifuge.StatesInstance smi) => !smi.master.operational.IsOperational);
			this.disabled.EventTransition(GameHashes.OperationalChanged, this.waiting, (UraniumCentrifuge.StatesInstance smi) => smi.master.operational.IsOperational);
			this.waiting.EventTransition(GameHashes.OnStorageChange, this.converting, (UraniumCentrifuge.StatesInstance smi) => smi.master.GetComponent<ElementConverter>().HasEnoughMassToStartConverting());
			this.converting.Enter(delegate(UraniumCentrifuge.StatesInstance smi)
			{
				smi.master.operational.SetActive(true, false);
			}).Exit(delegate(UraniumCentrifuge.StatesInstance smi)
			{
				smi.master.operational.SetActive(false, false);
			}).Transition(this.waiting, (UraniumCentrifuge.StatesInstance smi) => !smi.master.GetComponent<ElementConverter>().CanConvertAtAll(), UpdateRate.SIM_200ms);
		}

		public GameStateMachine<UraniumCentrifuge.States, UraniumCentrifuge.StatesInstance, UraniumCentrifuge, object>.State disabled;

		public GameStateMachine<UraniumCentrifuge.States, UraniumCentrifuge.StatesInstance, UraniumCentrifuge, object>.State waiting;

		public GameStateMachine<UraniumCentrifuge.States, UraniumCentrifuge.StatesInstance, UraniumCentrifuge, object>.State converting;
	}
}
