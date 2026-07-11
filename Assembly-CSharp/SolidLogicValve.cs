using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class SolidLogicValve : StateMachineComponent<SolidLogicValve.StatesInstance>, ISim200ms
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	public void Sim200ms(float dt)
	{
		if (this.operational.IsOperational && this.bridge.IsDispensing)
		{
			this.operational.SetActive(true, false);
		}
		else
		{
			this.operational.SetActive(false, false);
		}
	}

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private SolidConduitBridge bridge;

	public class States : GameStateMachine<SolidLogicValve.States, SolidLogicValve.StatesInstance, SolidLogicValve>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.off;
			this.root.DoNothing();
			this.off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, this.on, (SolidLogicValve.StatesInstance smi) => smi.GetComponent<Operational>().IsOperational);
			this.on.DefaultState(this.on.idle).EventTransition(GameHashes.OperationalChanged, this.off, (SolidLogicValve.StatesInstance smi) => !smi.GetComponent<Operational>().IsOperational);
			this.on.idle.PlayAnim("on").EventTransition(GameHashes.ActiveChanged, this.on.working, (SolidLogicValve.StatesInstance smi) => smi.GetComponent<Operational>().IsActive);
			this.on.working.PlayAnim("on_flow", KAnim.PlayMode.Loop).EventTransition(GameHashes.ActiveChanged, this.on.idle, (SolidLogicValve.StatesInstance smi) => !smi.GetComponent<Operational>().IsActive);
		}

		public GameStateMachine<SolidLogicValve.States, SolidLogicValve.StatesInstance, SolidLogicValve, object>.State off;

		public SolidLogicValve.States.ReadyStates on;

		public class ReadyStates : GameStateMachine<SolidLogicValve.States, SolidLogicValve.StatesInstance, SolidLogicValve, object>.State
		{
			public GameStateMachine<SolidLogicValve.States, SolidLogicValve.StatesInstance, SolidLogicValve, object>.State idle;

			public GameStateMachine<SolidLogicValve.States, SolidLogicValve.StatesInstance, SolidLogicValve, object>.State working;
		}
	}

	public class StatesInstance : GameStateMachine<SolidLogicValve.States, SolidLogicValve.StatesInstance, SolidLogicValve, object>.GameInstance
	{
		public StatesInstance(SolidLogicValve master)
			: base(master)
		{
		}
	}
}
