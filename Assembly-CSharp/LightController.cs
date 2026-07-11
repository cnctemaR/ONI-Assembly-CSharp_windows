using System;

public class LightController : GameStateMachine<LightController, LightController.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.off;
		this.off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, this.on, (LightController.Instance smi) => smi.GetComponent<Operational>().IsOperational);
		this.on.PlayAnim("on").EventTransition(GameHashes.OperationalChanged, this.off, (LightController.Instance smi) => !smi.GetComponent<Operational>().IsOperational).ToggleStatusItem(Db.Get().BuildingStatusItems.EmittingLight, null)
			.Enter("SetActive", delegate(LightController.Instance smi)
			{
				smi.GetComponent<Operational>().SetActive(true, false);
			});
	}

	public GameStateMachine<LightController, LightController.Instance, IStateMachineTarget, object>.State off;

	public GameStateMachine<LightController, LightController.Instance, IStateMachineTarget, object>.State on;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<LightController, LightController.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master, LightController.Def def)
			: base(master, def)
		{
		}
	}
}
