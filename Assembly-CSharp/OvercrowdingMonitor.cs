using System;
using System.Diagnostics;
using Klei.AI;
using STRINGS;

public class OvercrowdingMonitor : GameStateMachine<OvercrowdingMonitor, OvercrowdingMonitor.Instance, IStateMachineTarget, OvercrowdingMonitor.Def>
{
	[Conditional("DETAILED_OVERCROWDING_MONITOR_PROFILE")]
	private static void BeginDetailedSample(string regionName)
	{
	}

	[Conditional("DETAILED_OVERCROWDING_MONITOR_PROFILE")]
	private static void EndDetailedSample(string regionName)
	{
	}

	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		this.root.Update(new Action<OvercrowdingMonitor.Instance, float>(OvercrowdingMonitor.UpdateState), UpdateRate.SIM_1000ms, true);
	}

	private static bool IsConfined(OvercrowdingMonitor.Instance smi)
	{
		if (smi.kpid.HasAnyTags(OvercrowdingMonitor.confinementImmunity))
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
			if (smi.fishOvercrowdingMonitor.cellCount < smi.def.spaceRequiredPerCreature)
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
		if (smi.cavity != null)
		{
			int num = smi.cavity.creatures.Count + smi.cavity.eggs.Count;
			return num != 0 && smi.cavity.eggs.Count != 0 && smi.cavity.numCells / num < smi.def.spaceRequiredPerCreature;
		}
		return false;
	}

	private static int CalculateOvercrowdedModifer(OvercrowdingMonitor.Instance smi)
	{
		if (smi.fishOvercrowdingMonitor != null)
		{
			int fishCount = smi.fishOvercrowdingMonitor.fishCount;
			if (fishCount <= 0)
			{
				return 0;
			}
			int num = smi.fishOvercrowdingMonitor.cellCount / smi.def.spaceRequiredPerCreature;
			if (num < smi.fishOvercrowdingMonitor.fishCount)
			{
				return -(fishCount - num);
			}
			return 0;
		}
		else
		{
			if (smi.cavity == null)
			{
				return 0;
			}
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
		if (smi.fishOvercrowdingMonitor == null)
		{
			return smi.cavity != null && smi.cavity.creatures.Count > 1 && smi.cavity.numCells / smi.cavity.creatures.Count < smi.def.spaceRequiredPerCreature;
		}
		int fishCount = smi.fishOvercrowdingMonitor.fishCount;
		if (fishCount > 0)
		{
			return smi.fishOvercrowdingMonitor.cellCount / fishCount < smi.def.spaceRequiredPerCreature;
		}
		int num = Grid.PosToCell(smi);
		return Grid.IsValidCell(num) && !Grid.IsLiquid(num);
	}

	private static void UpdateState(OvercrowdingMonitor.Instance smi, float dt)
	{
		bool flag = smi.kpid.HasTag(GameTags.Creatures.Confined);
		bool flag2 = smi.kpid.HasTag(GameTags.Creatures.Expecting);
		bool flag3 = smi.kpid.HasTag(GameTags.Creatures.Overcrowded);
		OvercrowdingMonitor.UpdateCavity(smi, dt);
		if (smi.def.spaceRequiredPerCreature == 0)
		{
			return;
		}
		bool flag4 = OvercrowdingMonitor.IsConfined(smi);
		bool flag5 = OvercrowdingMonitor.IsOvercrowded(smi);
		if (flag5)
		{
			if (!smi.isFish)
			{
				smi.overcrowdedModifier.SetValue((float)OvercrowdingMonitor.CalculateOvercrowdedModifer(smi));
			}
			else
			{
				smi.fishOvercrowdedModifier.SetValue((float)OvercrowdingMonitor.CalculateOvercrowdedModifer(smi));
			}
		}
		bool flag6 = !smi.isBaby && OvercrowdingMonitor.IsFutureOvercrowded(smi);
		if (flag != flag4 || flag2 != flag6 || flag3 != flag5)
		{
			KPrefabID kpid = smi.kpid;
			Effect effect = (smi.isFish ? smi.fishOvercrowdedEffect : smi.overcrowdedEffect);
			kpid.SetTag(GameTags.Creatures.Confined, flag4);
			kpid.SetTag(GameTags.Creatures.Overcrowded, flag5);
			kpid.SetTag(GameTags.Creatures.Expecting, flag6);
			OvercrowdingMonitor.SetEffect(smi, smi.stuckEffect, flag4);
			OvercrowdingMonitor.SetEffect(smi, effect, !flag4 && flag5);
			OvercrowdingMonitor.SetEffect(smi, smi.futureOvercrowdedEffect, !flag4 && flag6);
		}
	}

	private static void SetEffect(OvercrowdingMonitor.Instance smi, Effect effect, bool set)
	{
		if (set)
		{
			smi.effects.Add(effect, false);
			return;
		}
		smi.effects.Remove(effect);
	}

	private static void UpdateCavity(OvercrowdingMonitor.Instance smi, float dt)
	{
		CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(Grid.PosToCell(smi));
		if (cavityForCell != smi.cavity)
		{
			if (smi.cavity != null)
			{
				if (smi.kpid.HasTag(GameTags.Egg))
				{
					smi.cavity.RemoveFromCavity(smi.kpid, smi.cavity.eggs);
				}
				else
				{
					smi.cavity.RemoveFromCavity(smi.kpid, smi.cavity.creatures);
				}
				Game.Instance.roomProber.UpdateRoom(cavityForCell);
			}
			smi.cavity = cavityForCell;
			if (smi.cavity != null)
			{
				if (smi.kpid.HasTag(GameTags.Egg))
				{
					smi.cavity.eggs.Add(smi.kpid);
				}
				else
				{
					smi.cavity.creatures.Add(smi.kpid);
				}
				Game.Instance.roomProber.UpdateRoom(smi.cavity);
			}
		}
	}

	public const float OVERCROWDED_FERTILITY_DEBUFF = -1f;

	public static Tag[] confinementImmunity = new Tag[]
	{
		GameTags.Creatures.Burrowed,
		GameTags.Creatures.Digger
	};

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
			if (this.kpid.HasTag(GameTags.Egg))
			{
				this.cavity.RemoveFromCavity(this.kpid, this.cavity.eggs);
				return;
			}
			this.cavity.RemoveFromCavity(this.kpid, this.cavity.creatures);
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

		[MyCmpReq]
		public KPrefabID kpid;

		[MyCmpReq]
		public Effects effects;

		[MySmiGet]
		public FishOvercrowdingMonitor.Instance fishOvercrowdingMonitor;
	}
}
