using System;
using Klei.AI;
using STRINGS;

public class OvercrowdingMonitor : GameStateMachine<OvercrowdingMonitor, OvercrowdingMonitor.Instance, IStateMachineTarget, OvercrowdingMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.comfortable;
		this.root.Update(new Action<OvercrowdingMonitor.Instance, float>(OvercrowdingMonitor.UpdateCavity), UpdateRate.SIM_1000ms, false);
		this.stuck.Transition(this.comfortable, new StateMachine<OvercrowdingMonitor, OvercrowdingMonitor.Instance, IStateMachineTarget, OvercrowdingMonitor.Def>.Transition.ConditionCallback(OvercrowdingMonitor.IsNotConfined), UpdateRate.SIM_1000ms).ToggleEffect((OvercrowdingMonitor.Instance smi) => OvercrowdingMonitor.stuckEffect);
		this.comfortable.Transition(this.stuck, new StateMachine<OvercrowdingMonitor, OvercrowdingMonitor.Instance, IStateMachineTarget, OvercrowdingMonitor.Def>.Transition.ConditionCallback(OvercrowdingMonitor.IsConfined), UpdateRate.SIM_1000ms).Transition(this.future_overcrowded, new StateMachine<OvercrowdingMonitor, OvercrowdingMonitor.Instance, IStateMachineTarget, OvercrowdingMonitor.Def>.Transition.ConditionCallback(OvercrowdingMonitor.IsFutureOvercrowded), UpdateRate.SIM_1000ms);
		this.future_overcrowded.Transition(this.comfortable, new StateMachine<OvercrowdingMonitor, OvercrowdingMonitor.Instance, IStateMachineTarget, OvercrowdingMonitor.Def>.Transition.ConditionCallback(OvercrowdingMonitor.IsNotFutureOvercrowded), UpdateRate.SIM_1000ms).Transition(this.overcrowded, new StateMachine<OvercrowdingMonitor, OvercrowdingMonitor.Instance, IStateMachineTarget, OvercrowdingMonitor.Def>.Transition.ConditionCallback(OvercrowdingMonitor.IsOvercrowded), UpdateRate.SIM_1000ms).ToggleEffect((OvercrowdingMonitor.Instance smi) => OvercrowdingMonitor.futureOvercrowdedEffect);
		this.overcrowded.Transition(this.future_overcrowded, new StateMachine<OvercrowdingMonitor, OvercrowdingMonitor.Instance, IStateMachineTarget, OvercrowdingMonitor.Def>.Transition.ConditionCallback(OvercrowdingMonitor.IsNotOvercrowded), UpdateRate.SIM_1000ms).ToggleEffect((OvercrowdingMonitor.Instance smi) => OvercrowdingMonitor.overcrowdedEffect);
		OvercrowdingMonitor.futureOvercrowdedEffect = new Effect("FutureOvercrowded", CREATURES.MODIFIERS.FUTURE_OVERCROWDED.NAME, CREATURES.MODIFIERS.FUTURE_OVERCROWDED.TOOLTIP, 0f, true, false, true);
		OvercrowdingMonitor.futureOvercrowdedEffect.Add(new AttributeModifier(Db.Get().Amounts.Fertility.deltaAttribute.Id, -1f, CREATURES.MODIFIERS.FUTURE_OVERCROWDED.NAME, true, false, true));
		OvercrowdingMonitor.overcrowdedEffect = new Effect("Overcrowded", CREATURES.MODIFIERS.OVERCROWDED.NAME, CREATURES.MODIFIERS.OVERCROWDED.TOOLTIP, 0f, true, false, true);
		OvercrowdingMonitor.overcrowdedEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, -5f, CREATURES.MODIFIERS.OVERCROWDED.NAME, false, false, true));
		OvercrowdingMonitor.stuckEffect = new Effect("Confined", CREATURES.MODIFIERS.CONFINED.NAME, CREATURES.MODIFIERS.CONFINED.TOOLTIP, 0f, true, false, true);
		OvercrowdingMonitor.stuckEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, -10f, CREATURES.MODIFIERS.CONFINED.NAME, false, false, true));
	}

	private static bool IsNotConfined(OvercrowdingMonitor.Instance smi)
	{
		return !OvercrowdingMonitor.IsConfined(smi);
	}

	private static bool IsConfined(OvercrowdingMonitor.Instance smi)
	{
		return !smi.HasTag(GameTags.Creatures.Burrowed) && (smi.cavity == null || smi.cavity.numCells < smi.def.spaceRequiredPerCreature);
	}

	private static bool IsNotFutureOvercrowded(OvercrowdingMonitor.Instance smi)
	{
		return !OvercrowdingMonitor.IsFutureOvercrowded(smi);
	}

	private static bool IsFutureOvercrowded(OvercrowdingMonitor.Instance smi)
	{
		if (smi.def.spaceRequiredPerCreature == 0)
		{
			return false;
		}
		if (smi.cavity != null && smi.cavity.creatures.Count > 1)
		{
			int count = smi.cavity.creatures.Count;
			int num = smi.cavity.numCells / count;
			return num < smi.def.spaceRequiredPerCreature;
		}
		return false;
	}

	private static bool IsNotOvercrowded(OvercrowdingMonitor.Instance smi)
	{
		return !OvercrowdingMonitor.IsOvercrowded(smi);
	}

	private static bool IsOvercrowded(OvercrowdingMonitor.Instance smi)
	{
		if (smi.def.spaceRequiredPerCreature == 0)
		{
			return false;
		}
		FishOvercrowdingMonitor.Instance smi2 = smi.GetSMI<FishOvercrowdingMonitor.Instance>();
		if (smi2 != null)
		{
			int fishCount = smi2.fishCount;
			if (fishCount > 0)
			{
				int cellCount = smi2.cellCount;
				int num = cellCount / fishCount;
				return num < smi.def.spaceRequiredPerCreature;
			}
			return false;
		}
		else
		{
			if (smi.cavity != null && smi.cavity.creatures.Count > 1)
			{
				int num2 = 0;
				foreach (KPrefabID kprefabID in smi.cavity.creatures)
				{
					if (!kprefabID.HasTag(GameTags.Egg))
					{
						num2++;
					}
				}
				int num3 = smi.cavity.numCells / num2;
				return num3 < smi.def.spaceRequiredPerCreature;
			}
			return false;
		}
	}

	private static void UpdateCavity(OvercrowdingMonitor.Instance smi, float dt)
	{
		CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(Grid.PosToCell(smi));
		if (cavityForCell != smi.cavity)
		{
			KPrefabID component = smi.GetComponent<KPrefabID>();
			if (smi.cavity != null)
			{
				smi.cavity.creatures.Remove(component);
			}
			smi.cavity = cavityForCell;
			if (smi.cavity != null)
			{
				smi.cavity.creatures.Add(component);
			}
		}
	}

	private GameStateMachine<OvercrowdingMonitor, OvercrowdingMonitor.Instance, IStateMachineTarget, OvercrowdingMonitor.Def>.State future_overcrowded;

	private GameStateMachine<OvercrowdingMonitor, OvercrowdingMonitor.Instance, IStateMachineTarget, OvercrowdingMonitor.Def>.State overcrowded;

	private GameStateMachine<OvercrowdingMonitor, OvercrowdingMonitor.Instance, IStateMachineTarget, OvercrowdingMonitor.Def>.State stuck;

	private GameStateMachine<OvercrowdingMonitor, OvercrowdingMonitor.Instance, IStateMachineTarget, OvercrowdingMonitor.Def>.State comfortable;

	public static Effect futureOvercrowdedEffect;

	public static Effect overcrowdedEffect;

	public static Effect stuckEffect;

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

		protected override void OnCleanUp()
		{
			KPrefabID component = base.master.GetComponent<KPrefabID>();
			if (this.cavity != null)
			{
				this.cavity.creatures.Remove(component);
			}
		}

		public CavityInfo cavity;
	}
}
