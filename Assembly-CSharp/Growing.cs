using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

public class Growing : StateMachineComponent<Growing.StatesInstance>, IGameObjectEffectDescriptor
{
	public bool Replanted
	{
		get
		{
			return this.replanted;
		}
	}

	protected override void OnPrefabInit()
	{
		Amounts amounts = base.gameObject.GetAmounts();
		this.maturity = amounts.Add(new AmountInstance(Db.Get().Amounts.Maturity, base.gameObject));
		this.baseMaturityMax = new AttributeModifier(this.maturity.maxAttribute.Id, this.GetTotalGrowthTime() / 600f, null, false, false);
		this.maturity.maxAttribute.Add("Base", this.baseMaturityMax);
		this.oldAge = amounts.Add(new AmountInstance(Db.Get().Amounts.OldAge, base.gameObject));
		this.yieldBonus = amounts.Add(new AmountInstance(Db.Get().Amounts.YieldBonus, base.gameObject));
		base.OnPrefabInit();
		this.Subscribe(1119167081, new Action<object>(this.OnNewGameSpawn));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	private void OnNewGameSpawn(object data)
	{
		this.maturity.SetValue(this.maturity.maxAttribute.GetTotalValue() * global::UnityEngine.Random.Range(0f, 1f));
	}

	public void Configure(float baseGrowthTime, float reGrowthTime)
	{
		this.baseGrowthTime = baseGrowthTime;
		this.reGrowthTime = reGrowthTime;
	}

	public bool ReachedNextHarvest()
	{
		return this.PercentOfCurrentHarvest() >= 1f;
	}

	public bool IsGrown()
	{
		return this.maturity.value == this.maturity.GetMax();
	}

	public bool CanGrow()
	{
		return !this.IsGrown();
	}

	public bool IsGrowing()
	{
		return this.maturity.GetDelta() > 0f;
	}

	public void ClampGrowthToHarvest()
	{
		if (this.crop == null)
		{
			this.maturity.value = this.maturity.GetMax();
		}
		else
		{
			this.maturity.value = this.GetTotalGrowthForHarvest(this.crop.GetTimesHarvested() + 1) / 600f;
		}
	}

	public float PercentOfCurrentHarvest()
	{
		if (this.crop == null)
		{
			return this.maturity.value / this.maturity.GetMax();
		}
		float totalGrowthForHarvest = this.GetTotalGrowthForHarvest(this.crop.GetTimesHarvested());
		float totalGrowthForHarvest2 = this.GetTotalGrowthForHarvest(this.crop.GetTimesHarvested() + 1);
		float num = this.maturity.value * 600f;
		return (num - totalGrowthForHarvest) / (totalGrowthForHarvest2 - totalGrowthForHarvest);
	}

	public float TimeUntilNextHarvest()
	{
		if (this.crop == null)
		{
			float num = this.maturity.GetMax() - this.maturity.value;
			return num / this.maturity.GetDelta();
		}
		float num2 = this.GetTotalGrowthForHarvest(this.crop.GetTimesHarvested() + 1) / 600f - this.maturity.value;
		return num2 / this.maturity.GetDelta();
	}

	public float PercentGrown()
	{
		return this.maturity.value / this.maturity.GetMax();
	}

	public void OnReplant()
	{
		this.replanted = true;
	}

	public void ResetGrowth()
	{
		this.maturity.value = (this.baseGrowthTime - this.reGrowthTime) / 600f;
	}

	public float PercentOldAge()
	{
		return this.oldAge.value / this.oldAge.GetMax();
	}

	private float GetTotalGrowthTime()
	{
		if (this.crop == null)
		{
			return this.baseGrowthTime;
		}
		return this.baseGrowthTime + this.reGrowthTime * (float)(this.crop.GetTotalHarvests() - 1);
	}

	private float GetTotalGrowthForHarvest(int harvest)
	{
		if (harvest <= 0)
		{
			return 0f;
		}
		if (this.crop == null)
		{
			return this.baseGrowthTime;
		}
		return this.baseGrowthTime + this.reGrowthTime * (float)(harvest - 1);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (this.baseGrowthTime != this.reGrowthTime && this.reGrowthTime != 0f)
		{
			list.Add(new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.GROWTHTIME_REGROWTH, GameUtil.GetFormattedCycles(this.baseGrowthTime, string.Empty), GameUtil.GetFormattedCycles(this.reGrowthTime, string.Empty)), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.GROWTHTIME_REGROWTH, GameUtil.GetFormattedCycles(this.baseGrowthTime, string.Empty), GameUtil.GetFormattedCycles(this.reGrowthTime, string.Empty)), Descriptor.DescriptorType.Requirement, false));
		}
		else
		{
			list.Add(new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.GROWTHTIME_SIMPLE, GameUtil.GetFormattedCycles(this.baseGrowthTime, string.Empty)), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.GROWTHTIME_SIMPLE, GameUtil.GetFormattedCycles(this.baseGrowthTime, string.Empty)), Descriptor.DescriptorType.Requirement, false));
		}
		return list;
	}

	public float baseGrowthTime;

	public float reGrowthTime;

	private AmountInstance maturity;

	private AmountInstance oldAge;

	private AttributeModifier baseMaturityMax;

	private AmountInstance yieldBonus;

	[Serialize]
	private bool replanted;

	[MyCmpGet]
	private WiltCondition wiltCondition;

	[MyCmpReq]
	private KSelectable selectable;

	[MyCmpReq]
	private Modifiers modifiers;

	[MyCmpGet]
	private Crop crop;

	public class StatesInstance : GameStateMachine<Growing.States, Growing.StatesInstance, Growing, object>.GameInstance
	{
		public StatesInstance(Growing master)
			: base(master)
		{
			this.baseGrowingRate = new AttributeModifier(master.maturity.deltaAttribute.Id, 0.0016666667f, CREATURES.STATS.MATURITY.GROWING, false, false);
			this.wildGrowingRate = new AttributeModifier(master.maturity.deltaAttribute.Id, 0.00041666668f, CREATURES.STATS.MATURITY.GROWINGWILD, false, false);
			this.getOldRate = new AttributeModifier(master.oldAge.deltaAttribute.Id, 1f, null, false, false);
		}

		public bool IsGrown()
		{
			return base.master.IsGrown();
		}

		public bool ReachedNextHarvest()
		{
			return base.master.ReachedNextHarvest();
		}

		public void ClampGrowthToHarvest()
		{
			base.master.ClampGrowthToHarvest();
		}

		public void PauseYieldBonus(bool pause)
		{
			base.master.yieldBonus.paused = pause;
		}

		public bool IsWilting()
		{
			return base.master.wiltCondition != null && base.master.wiltCondition.IsWilting();
		}

		public AttributeModifier baseGrowingRate;

		public AttributeModifier wildGrowingRate;

		public AttributeModifier getOldRate;
	}

	public class States : GameStateMachine<Growing.States, Growing.StatesInstance, Growing>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.growing;
			base.serializable = true;
			this.root.EventTransition(GameHashes.Wilt, this.stalled, (Growing.StatesInstance smi) => smi.IsWilting());
			this.growing.Transition(this.grown, (Growing.StatesInstance smi) => smi.ReachedNextHarvest()).ToggleStatusItem(Db.Get().CreatureStatusItems.Growing, (Growing.StatesInstance smi) => smi.master.GetComponent<Growing>()).Enter(delegate(Growing.StatesInstance smi)
			{
				if (smi.master.replanted)
				{
					smi.GoTo(this.growing.planted);
				}
				else
				{
					smi.GoTo(this.growing.wild);
				}
			});
			this.growing.wild.ToggleAttributeModifier("GrowingWild", (Growing.StatesInstance smi) => smi.wildGrowingRate, null);
			this.growing.planted.ToggleAttributeModifier("Growing", (Growing.StatesInstance smi) => smi.baseGrowingRate, null);
			this.grown.DefaultState(this.grown.idle).TriggerOnEnter(GameHashes.Grow, null).Transition(this.growing, (Growing.StatesInstance smi) => !smi.ReachedNextHarvest())
				.ToggleAttributeModifier("GettingOld", (Growing.StatesInstance smi) => smi.getOldRate, null)
				.Enter(delegate(Growing.StatesInstance smi)
				{
					smi.ClampGrowthToHarvest();
					smi.PauseYieldBonus(true);
				})
				.Exit(delegate(Growing.StatesInstance smi)
				{
					smi.master.oldAge.SetValue(0f);
					smi.PauseYieldBonus(false);
				});
			this.grown.idle.Transition(this.grown.try_self_harvest, (Growing.StatesInstance smi) => smi.master.oldAge.value >= smi.master.oldAge.GetMax());
			this.grown.try_self_harvest.Enter(delegate(Growing.StatesInstance smi)
			{
				Harvestable component = smi.master.GetComponent<Harvestable>();
				if (component)
				{
					component.Harvest();
				}
				smi.master.oldAge.SetValue(0f);
			}).GoTo(this.grown.idle);
			this.stalled.EventTransition(GameHashes.WiltRecover, this.growing, (Growing.StatesInstance smi) => !smi.IsWilting()).Enter(delegate(Growing.StatesInstance smi)
			{
				smi.PauseYieldBonus(true);
			}).Exit(delegate(Growing.StatesInstance smi)
			{
				smi.PauseYieldBonus(false);
			});
		}

		public Growing.States.GrowingStates growing;

		public GameStateMachine<Growing.States, Growing.StatesInstance, Growing, object>.State stalled;

		public Growing.States.GrownStates grown;

		public class GrowingStates : GameStateMachine<Growing.States, Growing.StatesInstance, Growing, object>.State
		{
			public GameStateMachine<Growing.States, Growing.StatesInstance, Growing, object>.State wild;

			public GameStateMachine<Growing.States, Growing.StatesInstance, Growing, object>.State planted;
		}

		public class GrownStates : GameStateMachine<Growing.States, Growing.StatesInstance, Growing, object>.State
		{
			public GameStateMachine<Growing.States, Growing.StatesInstance, Growing, object>.State idle;

			public GameStateMachine<Growing.States, Growing.StatesInstance, Growing, object>.State try_self_harvest;
		}
	}
}
