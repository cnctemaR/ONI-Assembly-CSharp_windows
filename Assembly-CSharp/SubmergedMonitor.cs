using System;

public class SubmergedMonitor : GameStateMachine<SubmergedMonitor, SubmergedMonitor.Instance, IStateMachineTarget, SubmergedMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		this.satisfied.Enter("SetNavType", delegate(SubmergedMonitor.Instance smi)
		{
			smi.GetComponent<Navigator>().SetCurrentNavType(NavType.Hover);
		}).Update("SetNavType", delegate(SubmergedMonitor.Instance smi, float dt)
		{
			smi.GetComponent<Navigator>().SetCurrentNavType(NavType.Hover);
		}, UpdateRate.SIM_1000ms, false).Transition(this.submerged, (SubmergedMonitor.Instance smi) => smi.IsSubmerged(), UpdateRate.SIM_1000ms);
		this.submerged.Enter("SetNavType", delegate(SubmergedMonitor.Instance smi)
		{
			smi.GetComponent<Navigator>().SetCurrentNavType(NavType.Swim);
		}).Update("SetNavType", delegate(SubmergedMonitor.Instance smi, float dt)
		{
			smi.GetComponent<Navigator>().SetCurrentNavType(NavType.Swim);
		}, UpdateRate.SIM_1000ms, false).Transition(this.satisfied, (SubmergedMonitor.Instance smi) => !smi.IsSubmerged(), UpdateRate.SIM_1000ms)
			.ToggleTag(GameTags.Creatures.Submerged);
	}

	public GameStateMachine<SubmergedMonitor, SubmergedMonitor.Instance, IStateMachineTarget, SubmergedMonitor.Def>.State satisfied;

	public GameStateMachine<SubmergedMonitor, SubmergedMonitor.Instance, IStateMachineTarget, SubmergedMonitor.Def>.State submerged;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<SubmergedMonitor, SubmergedMonitor.Instance, IStateMachineTarget, SubmergedMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, SubmergedMonitor.Def def)
			: base(master, def)
		{
		}

		public bool IsSubmerged()
		{
			return Grid.IsSubstantialLiquid(Grid.PosToCell(base.transform.GetPosition()), 0.35f);
		}
	}
}
