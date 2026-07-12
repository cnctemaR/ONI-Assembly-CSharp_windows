using System;
using Klei.AI;
using STRINGS;

public class OvercrowdingMonitor : GameStateMachine<OvercrowdingMonitor, OvercrowdingMonitor.Instance, IStateMachineTarget, OvercrowdingMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		this.root.Update(new Action<OvercrowdingMonitor.Instance, float>(OvercrowdingMonitor.UpdateState), UpdateRate.SIM_1000ms, true);
	}

	private static bool IsConfined(OvercrowdingMonitor.Instance smi)
	{
		if (smi.HasTag(GameTags.Creatures.Burrowed))
		{
			return false;
		}
		if (smi.HasTag(GameTags.Creatures.Digger))
		{
			return false;
		}
		if (smi.isFish)
		{
			int num = Grid.PosToCell(smi);
			if (Grid.IsValidCell(num) && !Grid.IsLiquid(num))
			{
				return true;
			}
			FishOvercrowdingMonitor.Instance smi2 = smi.GetSMI<FishOvercrowdingMonitor.Instance>();
			if (smi2 != null && smi2.cellCount < smi.def.spaceRequiredPerCreature)
			{
				return true;
			}
		}
		else
		{
			if (smi.cavity == null)
			{
				return true;
			}
			if (smi.cavity.numCells < smi.def.spaceRequiredPerCreature)
			{
				return true;
			}
		}
		return false;
	}

	private static bool IsFutureOvercrowded(OvercrowdingMonitor.Instance smi)
	{
		if (smi.def.spaceRequiredPerCreature == 0)
		{
			return false;
		}
		if (smi.cavity != null)
		{
			int num = smi.cavity.creatures.Count + smi.cavity.eggs.Count;
			return num != 0 && smi.cavity.eggs.Count != 0 && smi.cavity.numCells / num < smi.def.spaceRequiredPerCreature;
		}
		return false;
	}

	private static int CalculateOvercrowdedModifer(OvercrowdingMonitor.Instance smi)
	{
		if (smi.def.spaceRequiredPerCreature == 0)
		{
			return 0;
		}
		if (smi.cavity == null)
		{
			return 0;
		}
		FishOvercrowdingMonitor.Instance smi2 = smi.GetSMI<FishOvercrowdingMonitor.Instance>();
		if (smi2 != null)
		{
			int fishCount = smi2.fishCount;
			if (fishCount <= 0)
			{
				return 0;
			}
			int num = smi2.cellCount / smi.def.spaceRequiredPerCreature;
			if (num < smi2.fishCount)
			{
				return -(fishCount - num);
			}
			return 0;
		}
		else
		{
			if (smi.cavity.creatures.Count <= 1)
			{
				return 0;
			}
			int num2 = smi.cavity.numCells / smi.def.spaceRequiredPerCreature;
			if (num2 < smi.cavity.creatures.Count)
			{
				return -(smi.cavity.creatures.Count - num2);
			}
			return 0;
		}
	}

	private static bool IsOvercrowded(OvercrowdingMonitor.Instance smi)
	{
		if (smi.def.spaceRequiredPerCreature == 0)
		{
			return false;
		}
		FishOvercrowdingMonitor.Instance smi2 = smi.GetSMI<FishOvercrowdingMonitor.Instance>();
		if (smi2 == null)
		{
			return smi.cavity != null && smi.cavity.creatures.Count > 1 && smi.cavity.numCells / smi.cavity.creatures.Count < smi.def.spaceRequiredPerCreature;
		}
		int fishCount = smi2.fishCount;
		if (fishCount > 0)
		{
			return smi2.cellCount / fishCount < smi.def.spaceRequiredPerCreature;
		}
		int num = Grid.PosToCell(smi);
		return Grid.IsValidCell(num) && !Grid.IsLiquid(num);
	}

	private static void UpdateState(OvercrowdingMonitor.Instance smi, float dt)
	{
		OvercrowdingMonitor.UpdateCavity(smi, dt);
		bool flag = OvercrowdingMonitor.IsConfined(smi);
		bool flag2 = OvercrowdingMonitor.IsOvercrowded(smi);
		bool flag3 = !smi.isBaby && OvercrowdingMonitor.IsFutureOvercrowded(smi);
		KPrefabID component = smi.gameObject.GetComponent<KPrefabID>();
		Effect effect = (smi.isFish ? smi.fishOvercrowdedEffect : smi.overcrowdedEffect);
		component.SetTag(GameTags.Creatures.Confined, flag);
		component.SetTag(GameTags.Creatures.Overcrowded, flag2);
		component.SetTag(GameTags.Creatures.Expecting, flag3);
		if (!smi.isFish)
		{
			smi.overcrowdedModifier.SetValue((float)OvercrowdingMonitor.CalculateOvercrowdedModifer(smi));
		}
		else
		{
			smi.fishOvercrowdedModifier.SetValue((float)OvercrowdingMonitor.CalculateOvercrowdedModifer(smi));
		}
		OvercrowdingMonitor.SetEffect(smi, smi.stuckEffect, flag);
		OvercrowdingMonitor.SetEffect(smi, effect, !flag && flag2);
		OvercrowdingMonitor.SetEffect(smi, smi.futureOvercrowdedEffect, !flag && flag3);
	}

	private static void SetEffect(OvercrowdingMonitor.Instance smi, Effect effect, bool set)
	{
		Effects component = smi.GetComponent<Effects>();
		if (set)
		{
			component.Add(effect, false);
			return;
		}
		component.Remove(effect);
	}

	private static void UpdateCavity(OvercrowdingMonitor.Instance smi, float dt)
	{
		CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(Grid.PosToCell(smi));
		if (cavityForCell != smi.cavity)
		{
			KPrefabID component = smi.GetComponent<KPrefabID>();
			if (smi.cavity != null)
			{
				if (smi.HasTag(GameTags.Egg))
				{
					smi.cavity.RemoveFromCavity(component, smi.cavity.eggs);
				}
				else
				{
					smi.cavity.RemoveFromCavity(component, smi.cavity.creatures);
				}
				Game.Instance.roomProber.UpdateRoom(cavityForCell);
			}
			smi.cavity = cavityForCell;
			if (smi.cavity != null)
			{
				if (smi.HasTag(GameTags.Egg))
				{
					smi.cavity.eggs.Add(component);
				}
				else
				{
					smi.cavity.creatures.Add(component);
				}
				Game.Instance.roomProber.UpdateRoom(smi.cavity);
			}
		}
	}

	public const float OVERCROWDED_FERTILITY_DEBUFF = -1f;

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
			FishOvercrowdingMonitor.Def def3 = master.gameObject.GetDef<FishOvercrowdingMonitor.Def>();
			this.isFish = def3 != null;
			this.futureOvercrowdedEffect = new Effect("FutureOvercrowded", CREATURES.MODIFIERS.FUTURE_OVERCROWDED.NAME, CREATURES.MODIFIERS.FUTURE_OVERCROWDED.TOOLTIP, 0f, true, false, true, null, -1f, 0f, null, "");
			this.futureOvercrowdedEffect.Add(new AttributeModifier(Db.Get().Amounts.Fertility.deltaAttribute.Id, -1f, CREATURES.MODIFIERS.FUTURE_OVERCROWDED.NAME, true, false, true));
			this.overcrowdedEffect = new Effect("Overcrowded", CREATURES.MODIFIERS.OVERCROWDED.NAME, CREATURES.MODIFIERS.OVERCROWDED.TOOLTIP, 0f, true, false, true, null, -1f, 0f, null, "");
			this.overcrowdedModifier = new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, 0f, CREATURES.MODIFIERS.OVERCROWDED.NAME, false, false, false);
			this.overcrowdedEffect.Add(this.overcrowdedModifier);
			this.fishOvercrowdedEffect = new Effect("Overcrowded", CREATURES.MODIFIERS.OVERCROWDED.NAME, CREATURES.MODIFIERS.OVERCROWDED.FISHTOOLTIP, 0f, true, false, true, null, -1f, 0f, null, "");
			this.fishOvercrowdedModifier = new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, -5f, CREATURES.MODIFIERS.OVERCROWDED.NAME, false, false, false);
			this.fishOvercrowdedEffect.Add(this.fishOvercrowdedModifier);
			this.stuckEffect = new Effect("Confined", CREATURES.MODIFIERS.CONFINED.NAME, CREATURES.MODIFIERS.CONFINED.TOOLTIP, 0f, true, false, true, null, -1f, 0f, null, "");
			this.stuckEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, -10f, CREATURES.MODIFIERS.CONFINED.NAME, false, false, true));
			this.stuckEffect.Add(new AttributeModifier(Db.Get().Amounts.Fertility.deltaAttribute.Id, -1f, CREATURES.MODIFIERS.CONFINED.NAME, true, false, true));
			OvercrowdingMonitor.UpdateState(this, 0f);
		}

		protected override void OnCleanUp()
		{
			if (this.cavity == null)
			{
				return;
			}
			KPrefabID component = base.master.GetComponent<KPrefabID>();
			if (base.HasTag(GameTags.Egg))
			{
				this.cavity.RemoveFromCavity(component, this.cavity.eggs);
				return;
			}
			this.cavity.RemoveFromCavity(component, this.cavity.creatures);
		}

		public void RoomRefreshUpdateCavity()
		{
			OvercrowdingMonitor.UpdateState(this, 0f);
		}

		public CavityInfo cavity;

		public bool isBaby;

		public bool isFish;

		public Effect futureOvercrowdedEffect;

		public Effect overcrowdedEffect;

		public AttributeModifier overcrowdedModifier;

		public Effect fishOvercrowdedEffect;

		public AttributeModifier fishOvercrowdedModifier;

		public Effect stuckEffect;
	}
}
