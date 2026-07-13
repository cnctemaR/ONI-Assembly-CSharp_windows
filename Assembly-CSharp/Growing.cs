using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TemplateClasses;
using UnityEngine;

public class Growing : StateMachineComponent<Growing.StatesInstance>, IGameObjectEffectDescriptor, IManageGrowingStates
{
	protected override void OnPrefabInit()
	{
		Amounts amounts = base.gameObject.GetAmounts();
		this.maturity = amounts.Get(Db.Get().Amounts.Maturity);
		this.oldAge = amounts.Add(new AmountInstance(Db.Get().Amounts.OldAge, base.gameObject));
		this.oldAge.maxAttribute.ClearModifiers();
		this.oldAge.maxAttribute.Add(new AttributeModifier(Db.Get().Amounts.OldAge.maxAttribute.Id, this.maxAge, null, false, false, true));
		base.OnPrefabInit();
		base.Subscribe<Growing>(1119167081, Growing.OnNewGameSpawnDelegate);
		base.Subscribe<Growing>(1272413801, Growing.ResetGrowthDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		base.gameObject.AddTag(GameTags.GrowingPlant);
	}

	private void OnNewGameSpawn(object data)
	{
		Prefab prefab = (Prefab)data;
		if (prefab.amounts != null)
		{
			foreach (Prefab.template_amount_value template_amount_value in prefab.amounts)
			{
				if (template_amount_value.id == this.maturity.amount.Id && template_amount_value.value == this.GetMaxMaturity())
				{
					return;
				}
			}
		}
		if (this.maturity == null)
		{
			KCrashReporter.ReportDevNotification("Maturity.OnNewGameSpawn", Environment.StackTrace, "", false, null);
		}
		this.maturity.SetValue(this.maturity.maxAttribute.GetTotalValue() * this.MaxMaturityValuePercentageToSpawnWith * global::UnityEngine.Random.Range(0f, 1f));
	}

	public void OverrideMaturityLevel(float percent)
	{
		float num = this.maturity.GetMax() * percent;
		this.maturity.SetValue(num);
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
		this.maturity.value = this.maturity.GetMax();
	}

	public float GetMaxMaturity()
	{
		return this.maturity.GetMax();
	}

	public float PercentOfCurrentHarvest()
	{
		return this.maturity.value / this.maturity.GetMax();
	}

	public float TimeUntilNextHarvest()
	{
		return (this.maturity.GetMax() - this.maturity.value) / this.maturity.GetDelta();
	}

	public float DomesticGrowthTime()
	{
		return this.maturity.GetMax() / base.smi.baseGrowingRate.Value;
	}

	public float WildGrowthTime()
	{
		return this.maturity.GetMax() / base.smi.wildGrowingRate.Value;
	}

	public float PercentGrown()
	{
		return this.maturity.value / this.maturity.GetMax();
	}

	public void ResetGrowth(object data = null)
	{
		this.maturity.value = 0f;
	}

	public float PercentOldAge()
	{
		if (!this.shouldGrowOld)
		{
			return 0f;
		}
		return this.oldAge.value / this.oldAge.GetMax();
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		Klei.AI.Attribute maxAttribute = Db.Get().Amounts.Maturity.maxAttribute;
		list.Add(new Descriptor(go.GetComponent<Modifiers>().GetPreModifiedAttributeDescription(maxAttribute), go.GetComponent<Modifiers>().GetPreModifiedAttributeToolTip(maxAttribute), Descriptor.DescriptorType.Requirement, false));
		return list;
	}

	public void ConsumeMass(float mass_to_consume)
	{
		float value = this.maturity.value;
		mass_to_consume = Mathf.Min(mass_to_consume, value);
		this.maturity.value = this.maturity.value - mass_to_consume;
		base.gameObject.Trigger(-1793167409, null);
	}

	public void ConsumeGrowthUnits(float units_to_consume, float unit_maturity_ratio)
	{
		float num = units_to_consume / unit_maturity_ratio;
		num = Mathf.Clamp(num, 0f, this.maturity.value);
		this.maturity.value -= num;
		base.gameObject.Trigger(-1793167409, null);
	}

	public Crop GetCropComponent()
	{
		return base.GetComponent<Crop>();
	}

	public bool IsWildPlanted()
	{
		return !this.rm.Replanted;
	}

	public Func<GameObject, bool> CustomGrowStallCondition_IsStalled;

	public float MaxMaturityValuePercentageToSpawnWith = 1f;

	public float GROWTH_RATE = 0.0016666667f;

	public float WILD_GROWTH_RATE = 0.00041666668f;

	public bool shouldGrowOld = true;

	public float maxAge = 2400f;

	private AmountInstance maturity;

	private AmountInstance oldAge;

	[MyCmpGet]
	private WiltCondition wiltCondition;

	[MyCmpReq]
	private KSelectable selectable;

	[MyCmpReq]
	private Modifiers modifiers;

	[MyCmpReq]
	private ReceptacleMonitor rm;

	private static readonly EventSystem.IntraObjectHandler<Growing> OnNewGameSpawnDelegate = new EventSystem.IntraObjectHandler<Growing>(delegate(Growing component, object data)
	{
		component.OnNewGameSpawn(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Growing> ResetGrowthDelegate = new EventSystem.IntraObjectHandler<Growing>(delegate(Growing component, object data)
	{
		component.ResetGrowth(data);
	});

	public class StatesInstance : GameStateMachine<Growing.States, Growing.StatesInstance, Growing, object>.GameInstance
	{
		public StatesInstance(Growing master)
			: base(master)
		{
			this.baseGrowingRate = new AttributeModifier(master.maturity.deltaAttribute.Id, master.GROWTH_RATE, CREATURES.STATS.MATURITY.GROWING, false, false, true);
			this.wildGrowingRate = new AttributeModifier(master.maturity.deltaAttribute.Id, master.WILD_GROWTH_RATE, CREATURES.STATS.MATURITY.GROWINGWILD, false, false, true);
			this.getOldRate = new AttributeModifier(master.oldAge.deltaAttribute.Id, master.shouldGrowOld ? 1f : 0f, null, false, false, true);
			this.harvestable = base.GetComponent<Harvestable>();
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

		public bool IsWilting()
		{
			return base.master.wiltCondition != null && base.master.wiltCondition.IsWilting();
		}

		public bool IsStalledByCustomCondition()
		{
			bool flag = false;
			if (base.master.CustomGrowStallCondition_IsStalled != null)
			{
				flag = base.master.CustomGrowStallCondition_IsStalled(base.master.gameObject);
			}
			return flag;
		}

		public bool CanExitStalled()
		{
			return !this.IsWilting() && (base.master.CustomGrowStallCondition_IsStalled == null || !base.master.CustomGrowStallCondition_IsStalled(base.master.gameObject));
		}

		public AttributeModifier baseGrowingRate;

		public AttributeModifier wildGrowingRate;

		public AttributeModifier getOldRate;

		public Harvestable harvestable;
	}

	public class States : GameStateMachine<Growing.States, Growing.StatesInstance, Growing>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.growing;
			base.serializable = StateMachine.SerializeType.Both_DEPRECATED;
			this.growing.EventTransition(GameHashes.Wilt, this.stalled, (Growing.StatesInstance smi) => smi.IsWilting()).EventTransition(GameHashes.CropSleep, this.stalled, (Growing.StatesInstance smi) => smi.IsStalledByCustomCondition()).EventTransition(GameHashes.ReceptacleMonitorChange, this.growing.planted, (Growing.StatesInstance smi) => !smi.master.IsWildPlanted())
				.EventTransition(GameHashes.ReceptacleMonitorChange, this.growing.wild, (Growing.StatesInstance smi) => smi.master.IsWildPlanted())
				.EventTransition(GameHashes.PlanterStorage, this.growing.planted, (Growing.StatesInstance smi) => !smi.master.IsWildPlanted())
				.EventTransition(GameHashes.PlanterStorage, this.growing.wild, (Growing.StatesInstance smi) => smi.master.IsWildPlanted())
				.TriggerOnEnter(GameHashes.Grow, null)
				.Update("CheckGrown", delegate(Growing.StatesInstance smi, float dt)
				{
					if (smi.ReachedNextHarvest())
					{
						smi.GoTo(this.grown);
					}
				}, UpdateRate.SIM_4000ms, false)
				.ToggleStatusItem(Db.Get().CreatureStatusItems.Growing, (Growing.StatesInstance smi) => smi.master.GetComponent<IManageGrowingStates>())
				.Enter(delegate(Growing.StatesInstance smi)
				{
					GameStateMachine<Growing.States, Growing.StatesInstance, Growing, object>.State state = (smi.master.IsWildPlanted() ? this.growing.wild : this.growing.planted);
					smi.GoTo(state);
				});
			this.growing.wild.ToggleAttributeModifier("GrowingWild", (Growing.StatesInstance smi) => smi.wildGrowingRate, null);
			this.growing.planted.ToggleAttributeModifier("Growing", (Growing.StatesInstance smi) => smi.baseGrowingRate, null);
			this.stalled.EventTransition(GameHashes.WiltRecover, this.growing, (Growing.StatesInstance smi) => smi.CanExitStalled()).EventTransition(GameHashes.CropWakeUp, this.growing, (Growing.StatesInstance smi) => smi.CanExitStalled());
			this.grown.DefaultState(this.grown.idle).TriggerOnEnter(GameHashes.Grow, null).Update("CheckNotGrown", delegate(Growing.StatesInstance smi, float dt)
			{
				if (!smi.ReachedNextHarvest())
				{
					smi.GoTo(this.growing);
				}
			}, UpdateRate.SIM_4000ms, false)
				.ToggleAttributeModifier("GettingOld", (Growing.StatesInstance smi) => smi.getOldRate, null)
				.Enter(delegate(Growing.StatesInstance smi)
				{
					smi.ClampGrowthToHarvest();
				})
				.Exit(delegate(Growing.StatesInstance smi)
				{
					smi.master.oldAge.SetValue(0f);
				});
			this.grown.idle.Update("CheckNotGrown", delegate(Growing.StatesInstance smi, float dt)
			{
				if (smi.master.shouldGrowOld && smi.master.oldAge.value >= smi.master.oldAge.GetMax() && smi.harvestable && smi.harvestable.CanBeHarvested)
				{
					if (smi.harvestable.harvestDesignatable != null)
					{
						bool harvestWhenReady = smi.harvestable.harvestDesignatable.HarvestWhenReady;
						smi.harvestable.ForceCancelHarvest(null);
						smi.harvestable.Harvest();
						if (harvestWhenReady && smi.harvestable != null)
						{
							smi.harvestable.harvestDesignatable.SetHarvestWhenReady(true);
						}
					}
					else
					{
						smi.harvestable.ForceCancelHarvest(null);
						smi.harvestable.Harvest();
					}
					smi.master.maturity.SetValue(0f);
					smi.master.oldAge.SetValue(0f);
				}
			}, UpdateRate.SIM_4000ms, false);
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
		}
	}
}
