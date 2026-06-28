using System;

public class LightController : GameStateMachine<LightController, LightController.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.off;
		this.off.PlayAnim("off", KAnim.PlayMode.Once, null).EventTransition(GameHashes.OperationalChanged, this.on, (LightController.Instance smi) => smi.GetComponent<Operational>().IsOperational);
		this.on.PlayAnim("on", KAnim.PlayMode.Once, null).EventTransition(GameHashes.OperationalChanged, this.off, (LightController.Instance smi) => !smi.GetComponent<Operational>().IsOperational).ToggleStatusItem(Db.Get().BuildingStatusItems.EmittingLight, null)
			.Enter("SetActive", delegate(LightController.Instance smi)
			{
				smi.GetComponent<Operational>().SetActive(true, false);
			});
	}

	public GameStateMachine<LightController, LightController.Instance, IStateMachineTarget>.State off;

	public GameStateMachine<LightController, LightController.Instance, IStateMachineTarget>.State on;

	public new class Instance : GameStateMachine<LightController, LightController.Instance, IStateMachineTarget>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}
	}
}
