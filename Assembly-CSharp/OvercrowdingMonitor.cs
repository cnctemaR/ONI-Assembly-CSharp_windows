using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;

public class OvercrowdingMonitor : GameStateMachine<OvercrowdingMonitor, OvercrowdingMonitor.Instance, IStateMachineTarget, OvercrowdingMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		this.root.Update(new Action<OvercrowdingMonitor.Instance, float>(OvercrowdingMonitor.UpdateState), UpdateRate.SIM_1000ms, false);
		OvercrowdingMonitor.futureOvercrowdedEffect = new Effect("FutureOvercrowded", CREATURES.MODIFIERS.FUTURE_OVERCROWDED.NAME, CREATURES.MODIFIERS.FUTURE_OVERCROWDED.TOOLTIP, 0f, true, false, true, null, 0f, null);
		OvercrowdingMonitor.futureOvercrowdedEffect.Add(new AttributeModifier(Db.Get().Amounts.Fertility.deltaAttribute.Id, -1f, CREATURES.MODIFIERS.FUTURE_OVERCROWDED.NAME, true, false, true));
		OvercrowdingMonitor.overcrowdedEffect = new Effect("Overcrowded", CREATURES.MODIFIERS.OVERCROWDED.NAME, CREATURES.MODIFIERS.OVERCROWDED.TOOLTIP, 0f, true, false, true, null, 0f, null);
		OvercrowdingMonitor.overcrowdedEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, -5f, CREATURES.MODIFIERS.OVERCROWDED.NAME, false, false, true));
		OvercrowdingMonitor.stuckEffect = new Effect("Confined", CREATURES.MODIFIERS.CONFINED.NAME, CREATURES.MODIFIERS.CONFINED.TOOLTIP, 0f, true, false, true, null, 0f, null);
		OvercrowdingMonitor.stuckEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, -10f, CREATURES.MODIFIERS.CONFINED.NAME, false, false, true));
	}

	private static bool IsConfined(OvercrowdingMonitor.Instance smi)
	{
		return !smi.HasTag(GameTags.Creatures.Burrowed) && (smi.cavity == null || smi.cavity.numCells < smi.def.spaceRequiredPerCreature);
	}

	private static bool IsFutureOvercrowded(OvercrowdingMonitor.Instance smi)
	{
		if (smi.def.spaceRequiredPerCreature == 0)
		{
			return false;
		}
		if (smi.cavity == null)
		{
			return false;
		}
		int num = smi.cavity.creatures.Count + smi.cavity.eggs.Count;
		if (num == 0 || smi.cavity.eggs.Count == 0)
		{
			return false;
		}
		int num2 = smi.cavity.numCells / num;
		return num2 < smi.def.spaceRequiredPerCreature;
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
				int num2 = smi.cavity.numCells / smi.cavity.creatures.Count;
				return num2 < smi.def.spaceRequiredPerCreature;
			}
			return false;
		}
	}

	private static void UpdateState(OvercrowdingMonitor.Instance smi, float dt)
	{
		OvercrowdingMonitor.UpdateCavity(smi, dt);
		bool flag = OvercrowdingMonitor.IsConfined(smi);
		bool flag2 = OvercrowdingMonitor.IsOvercrowded(smi);
		bool flag3 = !smi.isBaby && OvercrowdingMonitor.IsFutureOvercrowded(smi);
		KPrefabID component = smi.gameObject.GetComponent<KPrefabID>();
		component.SetTag(GameTags.Creatures.Confined, flag);
		component.SetTag(GameTags.Creatures.Overcrowded, flag2);
		component.SetTag(GameTags.Creatures.Expecting, flag3);
		OvercrowdingMonitor.SetEffect(smi, OvercrowdingMonitor.stuckEffect, flag);
		OvercrowdingMonitor.SetEffect(smi, OvercrowdingMonitor.overcrowdedEffect, !flag && flag2);
		OvercrowdingMonitor.SetEffect(smi, OvercrowdingMonitor.futureOvercrowdedEffect, !flag && flag3);
	}

	private static void SetEffect(OvercrowdingMonitor.Instance smi, Effect effect, bool set)
	{
		Effects component = smi.GetComponent<Effects>();
		if (set)
		{
			component.Add(effect, false);
		}
		else
		{
			component.Remove(effect);
		}
	}

	private static List<KPrefabID> GetCreatureCollection(OvercrowdingMonitor.Instance smi, CavityInfo cavity_info)
	{
		if (smi.HasTag(GameTags.Egg))
		{
			return cavity_info.eggs;
		}
		return cavity_info.creatures;
	}

	private static void UpdateCavity(OvercrowdingMonitor.Instance smi, float dt)
	{
		CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(Grid.PosToCell(smi));
		if (cavityForCell != smi.cavity)
		{
			KPrefabID component = smi.GetComponent<KPrefabID>();
			if (smi.cavity != null)
			{
				OvercrowdingMonitor.GetCreatureCollection(smi, smi.cavity).Remove(component);
			}
			smi.cavity = cavityForCell;
			if (smi.cavity != null)
			{
				OvercrowdingMonitor.GetCreatureCollection(smi, smi.cavity).Add(component);
			}
		}
	}

	public const float OVERCROWDED_FERTILITY_DEBUFF = -1f;

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
			BabyMonitor.Def def2 = master.gameObject.GetDef<BabyMonitor.Def>();
			this.isBaby = def2 != null;
		}

		protected override void OnCleanUp()
		{
			KPrefabID component = base.master.GetComponent<KPrefabID>();
			if (this.cavity != null)
			{
				OvercrowdingMonitor.GetCreatureCollection(this, this.cavity).Remove(component);
			}
		}

		public CavityInfo cavity;

		public bool isBaby;
	}
}
