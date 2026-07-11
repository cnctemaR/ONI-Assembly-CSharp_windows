using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class SolidLogicValve : StateMachineComponent<SolidLogicValve.StatesInstance>
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
			this.off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, this.on, (SolidLogicValve.StatesInstance smi) => smi.GetComponent<Operational>().IsOperational).Enter(delegate(SolidLogicValve.StatesInstance smi)
			{
				smi.GetComponent<Operational>().SetActive(false, false);
			});
			this.on.DefaultState(this.on.idle).EventTransition(GameHashes.OperationalChanged, this.off, (SolidLogicValve.StatesInstance smi) => !smi.GetComponent<Operational>().IsOperational).Enter(delegate(SolidLogicValve.StatesInstance smi)
			{
				smi.GetComponent<Operational>().SetActive(true, false);
			});
			this.on.idle.PlayAnim("on").Transition(this.on.working, (SolidLogicValve.StatesInstance smi) => smi.IsDispensing(), UpdateRate.SIM_200ms);
			this.on.working.PlayAnim("on_flow", KAnim.PlayMode.Loop).Transition(this.on.idle, (SolidLogicValve.StatesInstance smi) => !smi.IsDispensing(), UpdateRate.SIM_200ms);
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

		public bool IsDispensing()
		{
			return base.master.bridge.IsDispensing;
		}
	}
}
