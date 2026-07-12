using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Klei.AI;
using UnityEngine;

public class DrinkMilkMonitor : GameStateMachine<DrinkMilkMonitor, DrinkMilkMonitor.Instance, IStateMachineTarget, DrinkMilkMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.lookingToDrinkMilk;
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		this.root.OnSignal(this.didFinishDrinkingMilk, this.applyEffect).Enter(delegate(DrinkMilkMonitor.Instance smi)
		{
			this.remainingSecondsForEffect.Set(Mathf.Clamp(this.remainingSecondsForEffect.Get(smi), 0f, 600f), smi, false);
		}).ParamTransition<float>(this.remainingSecondsForEffect, this.satisfied, (DrinkMilkMonitor.Instance smi, float val) => val > 0f);
		this.lookingToDrinkMilk.PreBrainUpdate(new Action<DrinkMilkMonitor.Instance>(DrinkMilkMonitor.FindMilkFeederTarget)).ToggleBehaviour(GameTags.Creatures.Behaviour_TryToDrinkMilkFromFeeder, (DrinkMilkMonitor.Instance smi) => !smi.targetMilkFeeder.IsNullOrStopped() && !smi.targetMilkFeeder.IsReserved(), null).Exit(delegate(DrinkMilkMonitor.Instance smi)
		{
			smi.targetMilkFeeder = null;
		});
		this.applyEffect.Enter(delegate(DrinkMilkMonitor.Instance smi)
		{
			this.remainingSecondsForEffect.Set(600f, smi, false);
		}).EnterTransition(this.satisfied, (DrinkMilkMonitor.Instance smi) => true);
		this.satisfied.Enter(delegate(DrinkMilkMonitor.Instance smi)
		{
			if (smi.def.consumesMilk)
			{
				smi.GetComponent<Effects>().Add("HadMilk", false).timeRemaining = this.remainingSecondsForEffect.Get(smi);
			}
		}).Exit(delegate(DrinkMilkMonitor.Instance smi)
		{
			if (smi.def.consumesMilk)
			{
				smi.GetComponent<Effects>().Remove("HadMilk");
			}
			this.remainingSecondsForEffect.Set(-1f, smi, false);
		}).ScheduleGoTo((DrinkMilkMonitor.Instance smi) => this.remainingSecondsForEffect.Get(smi), this.lookingToDrinkMilk)
			.Update(delegate(DrinkMilkMonitor.Instance smi, float deltaSeconds)
			{
				this.remainingSecondsForEffect.Delta(-deltaSeconds, smi);
				if (this.remainingSecondsForEffect.Get(smi) < 0f)
				{
					smi.GoTo(this.lookingToDrinkMilk);
				}
			}, UpdateRate.SIM_1000ms, false);
	}

	private static void FindMilkFeederTarget(DrinkMilkMonitor.Instance smi)
	{
		DrinkMilkMonitor.<>c__DisplayClass8_0 CS$<>8__locals1;
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
			if (cavityForCell != null && cavityForCell.room != null && cavityForCell.room.roomType == Db.Get().RoomTypes.CreaturePen)
			{
				foreach (MilkFeeder.Instance instance in items)
				{
					if (!instance.IsNullOrDestroyed())
					{
						int num2 = Grid.PosToCell(instance);
						if (Game.Instance.roomProber.GetCavityForCell(num2) == cavityForCell && instance.IsReadyToStartFeeding())
						{
							pooledList.Add(instance);
						}
					}
				}
			}
			DrinkMilkMonitor.<>c__DisplayClass8_1 CS$<>8__locals2;
			CS$<>8__locals2.canDrown = CS$<>8__locals1.smi.drowningMonitor != null && CS$<>8__locals1.smi.drowningMonitor.canDrownToDeath && !CS$<>8__locals1.smi.drowningMonitor.livesUnderWater;
			CS$<>8__locals1.smi.targetMilkFeeder = null;
			CS$<>8__locals1.smi.doesTargetMilkFeederHaveSpaceForCritter = false;
			CS$<>8__locals2.resultCost = -1;
			foreach (MilkFeeder.Instance instance2 in pooledList)
			{
				DrinkMilkMonitor.<>c__DisplayClass8_2 CS$<>8__locals3;
				CS$<>8__locals3.milkFeeder = instance2;
				if (DrinkMilkMonitor.<FindMilkFeederTarget>g__ConsiderCell|8_0(CS$<>8__locals1.smi.GetDrinkCellOf(CS$<>8__locals3.milkFeeder, false), ref CS$<>8__locals1, ref CS$<>8__locals2, ref CS$<>8__locals3))
				{
					CS$<>8__locals1.smi.doesTargetMilkFeederHaveSpaceForCritter = false;
				}
				else if (DrinkMilkMonitor.<FindMilkFeederTarget>g__ConsiderCell|8_0(CS$<>8__locals1.smi.GetDrinkCellOf(CS$<>8__locals3.milkFeeder, true), ref CS$<>8__locals1, ref CS$<>8__locals2, ref CS$<>8__locals3))
				{
					CS$<>8__locals1.smi.doesTargetMilkFeederHaveSpaceForCritter = true;
				}
			}
		}
	}

	[CompilerGenerated]
	internal static bool <FindMilkFeederTarget>g__ConsiderCell|8_0(int cell, ref DrinkMilkMonitor.<>c__DisplayClass8_0 A_1, ref DrinkMilkMonitor.<>c__DisplayClass8_1 A_2, ref DrinkMilkMonitor.<>c__DisplayClass8_2 A_3)
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

	private StateMachine<DrinkMilkMonitor, DrinkMilkMonitor.Instance, IStateMachineTarget, DrinkMilkMonitor.Def>.FloatParameter remainingSecondsForEffect;

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
		}

		public void NotifyFinishedDrinkingMilkFrom(MilkFeeder.Instance milkFeeder)
		{
			if (milkFeeder != null && base.def.consumesMilk)
			{
				milkFeeder.ConsumeMilkForOneFeeding();
			}
			base.sm.didFinishDrinkingMilk.Trigger(base.smi);
		}

		public int GetDrinkCellOf(MilkFeeder.Instance milkFeeder, bool isTwoByTwoCritterCramped)
		{
			return Grid.OffsetCell(Grid.PosToCell(milkFeeder), base.def.drinkCellOffsetGetFn(milkFeeder, this, isTwoByTwoCritterCramped));
		}

		public MilkFeeder.Instance targetMilkFeeder;

		public bool doesTargetMilkFeederHaveSpaceForCritter;

		[MyCmpReq]
		public Navigator navigator;

		[MyCmpGet]
		public DrowningMonitor drowningMonitor;
	}
}
