using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Klei.AI;

public class DrinkMilkMonitor : GameStateMachine<DrinkMilkMonitor, DrinkMilkMonitor.Instance, IStateMachineTarget, DrinkMilkMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.lookingToDrinkMilk;
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		this.lookingToDrinkMilk.ParamTransition<float>(this.cooldown, this.satisfied, GameStateMachine<DrinkMilkMonitor, DrinkMilkMonitor.Instance, IStateMachineTarget, DrinkMilkMonitor.Def>.IsGTZero).OnSignal(this.didFinishDrinkingMilk, this.applyEffect).PreBrainUpdate(new Action<DrinkMilkMonitor.Instance>(DrinkMilkMonitor.FindMilkFeederTarget))
			.ToggleBehaviour(GameTags.Creatures.Behaviour_TryToDrinkMilkFromFeeder, (DrinkMilkMonitor.Instance smi) => !smi.targetMilkFeeder.IsNullOrStopped() && !smi.targetMilkFeeder.IsReserved(), null)
			.Exit(delegate(DrinkMilkMonitor.Instance smi)
			{
				smi.targetMilkFeeder = null;
			});
		this.applyEffect.Enter(new StateMachine<DrinkMilkMonitor, DrinkMilkMonitor.Instance, IStateMachineTarget, DrinkMilkMonitor.Def>.State.Callback(DrinkMilkMonitor.ApplyEffect)).Enter(new StateMachine<DrinkMilkMonitor, DrinkMilkMonitor.Instance, IStateMachineTarget, DrinkMilkMonitor.Def>.State.Callback(DrinkMilkMonitor.EnterCooldown)).EnterGoTo(this.satisfied);
		this.satisfied.ParamTransition<float>(this.cooldown, this.lookingToDrinkMilk, GameStateMachine<DrinkMilkMonitor, DrinkMilkMonitor.Instance, IStateMachineTarget, DrinkMilkMonitor.Def>.IsLTEZero).ScheduleGoTo((DrinkMilkMonitor.Instance smi) => this.cooldown.Get(smi), this.lookingToDrinkMilk).Update(new Action<DrinkMilkMonitor.Instance, float>(DrinkMilkMonitor.CooldownUpdate), UpdateRate.SIM_1000ms, false);
	}

	private static void EnterCooldown(DrinkMilkMonitor.Instance smi)
	{
		smi.sm.cooldown.Set(600f, smi, false);
	}

	private static void ApplyEffect(DrinkMilkMonitor.Instance smi)
	{
		smi.ApplyDrinkEffect();
	}

	private static void CooldownUpdate(DrinkMilkMonitor.Instance smi, float dt)
	{
		float num = smi.sm.cooldown.Get(smi) - dt;
		smi.sm.cooldown.Set(num, smi, false);
	}

	private static void FindMilkFeederTarget(DrinkMilkMonitor.Instance smi)
	{
		DrinkMilkMonitor.<>c__DisplayClass13_0 CS$<>8__locals1;
		CS$<>8__locals1.smi = smi;
		int num = Grid.PosToCell(CS$<>8__locals1.smi.gameObject);
		if (!Grid.IsValidCell(num))
		{
			return;
		}
		List<MilkFeeder.Instance> items = Components.MilkFeeders.GetItems((int)Grid.WorldIdx[num]);
		if (items == null || items.Count == 0)
		{
			return;
		}
		using (ListPool<MilkFeeder.Instance, DrinkMilkMonitor>.PooledList pooledList = PoolsFor<DrinkMilkMonitor>.AllocateList<MilkFeeder.Instance>())
		{
			CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(num);
			bool flag = !CS$<>8__locals1.smi.isAquaticCreature;
			if (cavityForCell != null && cavityForCell.room != null && (!flag || cavityForCell.room.roomType == Db.Get().RoomTypes.CreaturePen))
			{
				foreach (MilkFeeder.Instance instance in items)
				{
					if (!instance.IsNullOrDestroyed())
					{
						bool flag2 = instance.PrefabID() == "UnderwaterMilkFeeder";
						int num2 = Grid.OffsetCell(Grid.PosToCell(instance), flag2 ? DrinkMilkMonitor.UnderwaterCellOffset : DrinkMilkMonitor.GroundCellOffset);
						CavityInfo cavityForCell2 = Game.Instance.roomProber.GetCavityForCell(num2);
						if (CS$<>8__locals1.smi.isAquaticCreature == flag2 && cavityForCell2 == cavityForCell && instance.IsReadyToStartFeeding())
						{
							pooledList.Add(instance);
						}
					}
				}
			}
			DrinkMilkMonitor.<>c__DisplayClass13_1 CS$<>8__locals2;
			CS$<>8__locals2.canDrown = CS$<>8__locals1.smi.drowningMonitor != null && CS$<>8__locals1.smi.drowningMonitor.canDrownToDeath && !CS$<>8__locals1.smi.drowningMonitor.livesUnderWater;
			CS$<>8__locals1.smi.targetMilkFeeder = null;
			CS$<>8__locals1.smi.doesTargetMilkFeederHaveSpaceForCritter = false;
			CS$<>8__locals2.resultCost = -1;
			foreach (MilkFeeder.Instance instance2 in pooledList)
			{
				DrinkMilkMonitor.<>c__DisplayClass13_2 CS$<>8__locals3;
				CS$<>8__locals3.milkFeeder = instance2;
				if (DrinkMilkMonitor.<FindMilkFeederTarget>g__ConsiderCell|13_0(CS$<>8__locals1.smi.GetDrinkCellOf(CS$<>8__locals3.milkFeeder, false), ref CS$<>8__locals1, ref CS$<>8__locals2, ref CS$<>8__locals3))
				{
					CS$<>8__locals1.smi.doesTargetMilkFeederHaveSpaceForCritter = false;
				}
				else if (DrinkMilkMonitor.<FindMilkFeederTarget>g__ConsiderCell|13_0(CS$<>8__locals1.smi.GetDrinkCellOf(CS$<>8__locals3.milkFeeder, true), ref CS$<>8__locals1, ref CS$<>8__locals2, ref CS$<>8__locals3))
				{
					CS$<>8__locals1.smi.doesTargetMilkFeederHaveSpaceForCritter = true;
				}
			}
		}
	}

	[CompilerGenerated]
	internal static bool <FindMilkFeederTarget>g__ConsiderCell|13_0(int cell, ref DrinkMilkMonitor.<>c__DisplayClass13_0 A_1, ref DrinkMilkMonitor.<>c__DisplayClass13_1 A_2, ref DrinkMilkMonitor.<>c__DisplayClass13_2 A_3)
	{
		if (A_2.canDrown && !A_1.smi.drowningMonitor.IsCellSafe(cell))
		{
			return false;
		}
		int navigationCost = A_1.smi.navigator.GetNavigationCost(cell);
		if (navigationCost == -1)
		{
			return false;
		}
		if (navigationCost < A_2.resultCost || A_2.resultCost == -1)
		{
			A_2.resultCost = navigationCost;
			A_1.smi.targetMilkFeeder = A_3.milkFeeder;
			return true;
		}
		return false;
	}

	public GameStateMachine<DrinkMilkMonitor, DrinkMilkMonitor.Instance, IStateMachineTarget, DrinkMilkMonitor.Def>.State lookingToDrinkMilk;

	public GameStateMachine<DrinkMilkMonitor, DrinkMilkMonitor.Instance, IStateMachineTarget, DrinkMilkMonitor.Def>.State applyEffect;

	public GameStateMachine<DrinkMilkMonitor, DrinkMilkMonitor.Instance, IStateMachineTarget, DrinkMilkMonitor.Def>.State satisfied;

	private StateMachine<DrinkMilkMonitor, DrinkMilkMonitor.Instance, IStateMachineTarget, DrinkMilkMonitor.Def>.Signal didFinishDrinkingMilk;

	private StateMachine<DrinkMilkMonitor, DrinkMilkMonitor.Instance, IStateMachineTarget, DrinkMilkMonitor.Def>.FloatParameter cooldown;

	private static CellOffset UnderwaterCellOffset = new CellOffset(0, -1);

	private static CellOffset GroundCellOffset = new CellOffset(0, 0);

	public class Def : StateMachine.BaseDef
	{
		public bool consumesMilk = true;

		public DrinkMilkStates.Def.DrinkCellOffsetGetFn drinkCellOffsetGetFn;
	}

	public new class Instance : GameStateMachine<DrinkMilkMonitor, DrinkMilkMonitor.Instance, IStateMachineTarget, DrinkMilkMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, DrinkMilkMonitor.Def def)
			: base(master, def)
		{
			this.isAquaticCreature = base.HasTag(GameTags.Creatures.Swimmer);
		}

		public void ApplyDrinkEffect()
		{
			if (base.def.consumesMilk)
			{
				string text = null;
				for (int i = 0; i < MilkFeederConfig.EffectsPerDrinkableLiquid.Length; i++)
				{
					Tag first = MilkFeederConfig.EffectsPerDrinkableLiquid[i].first;
					string second = MilkFeederConfig.EffectsPerDrinkableLiquid[i].second;
					if (this.lastConsumedElementTag == first)
					{
						text = second;
						break;
					}
				}
				if (string.IsNullOrEmpty(text))
				{
					return;
				}
				base.GetComponent<Effects>().Add(text, true);
			}
		}

		public void NotifyFinishedDrinkingMilkFrom(MilkFeeder.Instance milkFeeder)
		{
			Tag tag = Tag.Invalid;
			this.lastConsumedElementTag = Tag.Invalid;
			if (milkFeeder != null && base.def.consumesMilk)
			{
				tag = milkFeeder.ConsumeMilkForOneFeeding();
			}
			this.lastConsumedElementTag = tag;
			base.sm.didFinishDrinkingMilk.Trigger(base.smi);
		}

		public int GetDrinkCellOf(MilkFeeder.Instance milkFeeder, bool isTwoByTwoCritterCramped)
		{
			return Grid.OffsetCell(Grid.PosToCell(milkFeeder), base.def.drinkCellOffsetGetFn(milkFeeder, this, isTwoByTwoCritterCramped));
		}

		public bool isAquaticCreature;

		public MilkFeeder.Instance targetMilkFeeder;

		public bool doesTargetMilkFeederHaveSpaceForCritter;

		public Tag lastConsumedElementTag = Tag.Invalid;

		[MyCmpReq]
		public Navigator navigator;

		[MyCmpGet]
		public DrowningMonitor drowningMonitor;
	}
}
