using System;
using Klei.AI;

public class OvercrowdingMonitor : GameStateMachine<OvercrowdingMonitor, OvercrowdingMonitor.Instance, IStateMachineTarget, OvercrowdingMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.comfortable;
		this.overcrowded.Transition(this.comfortable, (OvercrowdingMonitor.Instance smi) => !smi.IsOvercrowded(), UpdateRate.SIM_1000ms).ToggleEffect((OvercrowdingMonitor.Instance smi) => smi.def.overcrowdedEffect);
		this.comfortable.Transition(this.overcrowded, (OvercrowdingMonitor.Instance smi) => smi.IsOvercrowded(), UpdateRate.SIM_1000ms);
	}

	private GameStateMachine<OvercrowdingMonitor, OvercrowdingMonitor.Instance, IStateMachineTarget, OvercrowdingMonitor.Def>.State overcrowded;

	private GameStateMachine<OvercrowdingMonitor, OvercrowdingMonitor.Instance, IStateMachineTarget, OvercrowdingMonitor.Def>.State comfortable;

	public class Def : StateMachine.BaseDef
	{
		public Effect overcrowdedEffect;

		public int spaceRequiredPerCreature;
	}

	public new class Instance : GameStateMachine<OvercrowdingMonitor, OvercrowdingMonitor.Instance, IStateMachineTarget, OvercrowdingMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, OvercrowdingMonitor.Def def)
			: base(master, def)
		{
		}

		public bool IsOvercrowded()
		{
			CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(Grid.PosToCell(base.gameObject));
			if (cavityForCell != this.cavity)
			{
				KPrefabID component = base.master.GetComponent<KPrefabID>();
				if (this.cavity != null)
				{
					this.cavity.creatures.Remove(component);
				}
				this.cavity = cavityForCell;
				if (this.cavity != null)
				{
					this.cavity.creatures.Add(component);
				}
			}
			if (this.cavity != null)
			{
				int num = this.cavity.numCells / this.cavity.creatures.Count;
				return num < base.def.spaceRequiredPerCreature;
			}
			return false;
		}

		protected override void OnCleanUp()
		{
			KPrefabID component = base.master.GetComponent<KPrefabID>();
			if (this.cavity != null)
			{
				this.cavity.creatures.Remove(component);
			}
		}

		private CavityInfo cavity;
	}
}
