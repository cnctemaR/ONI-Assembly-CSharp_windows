using System;
using Klei.AI;
using STRINGS;

public class OvercrowdingMonitor : GameStateMachine<OvercrowdingMonitor, OvercrowdingMonitor.Instance, IStateMachineTarget, OvercrowdingMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.comfortable;
		this.root.Update("UpdateCavity", delegate(OvercrowdingMonitor.Instance smi, float dt)
		{
			smi.UpdateCavity();
		}, UpdateRate.SIM_1000ms, false);
		this.overcrowded.Transition(this.comfortable, (OvercrowdingMonitor.Instance smi) => !smi.IsOvercrowded(), UpdateRate.SIM_200ms).ToggleEffect((OvercrowdingMonitor.Instance smi) => this.overcrowdedEffect);
		this.stuck.Transition(this.comfortable, (OvercrowdingMonitor.Instance smi) => !smi.IsConfined(), UpdateRate.SIM_200ms).ToggleEffect((OvercrowdingMonitor.Instance smi) => this.stuckEffect);
		this.comfortable.Transition(this.stuck, (OvercrowdingMonitor.Instance smi) => smi.IsConfined(), UpdateRate.SIM_200ms).Transition(this.overcrowded, (OvercrowdingMonitor.Instance smi) => smi.IsOvercrowded(), UpdateRate.SIM_200ms);
		this.overcrowdedEffect = new Effect("Overcrowded", CREATURES.MODIFIERS.OVERCROWDED.NAME, CREATURES.MODIFIERS.OVERCROWDED.TOOLTIP, 0f, true, true, true);
		this.overcrowdedEffect.Add(new AttributeModifier(Db.Get().Amounts.Happiness.deltaAttribute.Id, -0.083333336f, CREATURES.MODIFIERS.OVERCROWDED.NAME, false, false, true));
		this.stuckEffect = new Effect("Confined", CREATURES.MODIFIERS.CONFINED.NAME, CREATURES.MODIFIERS.CONFINED.TOOLTIP, 0f, true, true, true);
		this.stuckEffect.Add(new AttributeModifier(Db.Get().Amounts.Happiness.deltaAttribute.Id, -0.083333336f, CREATURES.MODIFIERS.CONFINED.NAME, false, false, true));
	}

	private GameStateMachine<OvercrowdingMonitor, OvercrowdingMonitor.Instance, IStateMachineTarget, OvercrowdingMonitor.Def>.State overcrowded;

	private GameStateMachine<OvercrowdingMonitor, OvercrowdingMonitor.Instance, IStateMachineTarget, OvercrowdingMonitor.Def>.State stuck;

	private GameStateMachine<OvercrowdingMonitor, OvercrowdingMonitor.Instance, IStateMachineTarget, OvercrowdingMonitor.Def>.State comfortable;

	public Effect overcrowdedEffect;

	public Effect stuckEffect;

	public class Def : StateMachine.BaseDef
	{
		public int spaceRequiredPerCreature;
	}

	public new class Instance : GameStateMachine<OvercrowdingMonitor, OvercrowdingMonitor.Instance, IStateMachineTarget, OvercrowdingMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, OvercrowdingMonitor.Def def)
			: base(master, def)
		{
		}

		public void UpdateCavity()
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
		}

		public bool IsOvercrowded()
		{
			if (this.cavity != null && this.cavity.creatures.Count > 1)
			{
				int num = this.cavity.numCells / this.cavity.creatures.Count;
				return num < base.def.spaceRequiredPerCreature;
			}
			return false;
		}

		public bool IsConfined()
		{
			return !base.HasTag(GameTags.Creatures.Burrowed) && (this.cavity == null || this.cavity.numCells < base.def.spaceRequiredPerCreature);
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
