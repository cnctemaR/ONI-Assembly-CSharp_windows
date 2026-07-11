using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

public class Growing : StateMachineComponent<Growing.StatesInstance>, IGameObjectEffectDescriptor
{
	private static void AddToScenePartitioner(Growing.StatesInstance smi)
	{
		Extents extents = new Extents(Grid.PosToCell(smi), smi.Get<OccupyArea>().OccupiedCellsOffsets);
		smi.partitionerEntry = GameScenePartitioner.Instance.Add(smi.gameObject.name, smi.GetComponent<KPrefabID>(), extents, GameScenePartitioner.Instance.plants, null);
	}

	private static void RemoveFromScenePartitioner(Growing.StatesInstance smi)
	{
		GameScenePartitioner.Instance.Free(ref smi.partitionerEntry);
	}

	public bool Replanted
	{
		get
		{
			return this.replanted;
		}
	}

	private Crop crop
	{
		get
		{
			if (this._crop == null)
			{
				this._crop = base.GetComponent<Crop>();
			}
			return this._crop;
		}
	}

	protected override void OnPrefabInit()
	{
		Amounts amounts = base.gameObject.GetAmounts();
		this.maturity = amounts.Add(new AmountInstance(Db.Get().Amounts.Maturity, base.gameObject));
		this.baseMaturityMax = new AttributeModifier(this.maturity.maxAttribute.Id, this.growthTime / 600f, null, false, false, true);
		this.maturity.maxAttribute.Add(this.baseMaturityMax);
		this.oldAge = amounts.Add(new AmountInstance(Db.Get().Amounts.OldAge, base.gameObject));
		base.OnPrefabInit();
		base.Subscribe(1119167081, new Action<object>(this.OnNewGameSpawn));
		base.Subscribe(1309017699, new Action<object>(this.OnReplant));
		base.Subscribe(1272413801, new Action<object>(this.ResetGrowth));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		base.gameObject.AddTag(GameTags.Plant);
	}

	private void OnNewGameSpawn(object data)
	{
		this.maturity.SetValue(this.maturity.maxAttribute.GetTotalValue() * global::UnityEngine.Random.Range(0f, 1f));
	}

	public void Configure(float baseGrowthTime)
	{
		this.growthTime = baseGrowthTime;
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

	public float PercentOfCurrentHarvest()
	{
		return this.maturity.value / this.maturity.GetMax();
	}

	public float TimeUntilNextHarvest()
	{
		float num = this.maturity.GetMax() - this.maturity.value;
		return num / this.maturity.GetDelta();
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

	public void OnReplant(object data)
	{
		this.replanted = true;
	}

	public void ResetGrowth(object data = null)
	{
		this.maturity.value = 0f;
	}

	public float PercentOldAge()
	{
		return this.oldAge.value / this.oldAge.GetMax();
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return new List<Descriptor>
		{
			new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.GROWTHTIME_SIMPLE, GameUtil.GetFormattedCycles(this.growthTime, string.Empty)), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.GROWTHTIME_SIMPLE, GameUtil.GetFormattedCycles(this.growthTime, string.Empty)), Descriptor.DescriptorType.Requirement, false)
		};
	}

	public float growthTime;

	private AmountInstance maturity;

	private AmountInstance oldAge;

	private AttributeModifier baseMaturityMax;

	[Serialize]
	private bool replanted;

	[MyCmpGet]
	private WiltCondition wiltCondition;

	[MyCmpReq]
	private KSelectable selectable;

	[MyCmpReq]
	private Modifiers modifiers;

	private Crop _crop;

	public class StatesInstance : GameStateMachine<Growing.States, Growing.StatesInstance, Growing, object>.GameInstance
	{
		public StatesInstance(Growing master)
			: base(master)
		{
			this.baseGrowingRate = new AttributeModifier(master.maturity.deltaAttribute.Id, 0.0016666667f, CREATURES.STATS.MATURITY.GROWING, false, false, true);
			this.wildGrowingRate = new AttributeModifier(master.maturity.deltaAttribute.Id, 0.00041666668f, CREATURES.STATS.MATURITY.GROWINGWILD, false, false, true);
			this.getOldRate = new AttributeModifier(master.oldAge.deltaAttribute.Id, 1f, null, false, false, true);
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

		public bool IsSleeping()
		{
			CropSleepingMonitor.Instance smi = base.master.GetSMI<CropSleepingMonitor.Instance>();
			return smi != null && smi.IsSleeping();
		}

		public bool CanExitStalled()
		{
			return !this.IsWilting() && !this.IsSleeping() && !this.IsGrown();
		}

		public AttributeModifier baseGrowingRate;

		public AttributeModifier wildGrowingRate;

		public AttributeModifier getOldRate;

		public HandleVector<int>.Handle partitionerEntry;
	}

	public class States : GameStateMachine<Growing.States, Growing.StatesInstance, Growing>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.growing;
			base.serializable = true;
			this.root.EventTransition(GameHashes.Wilt, this.stalled, (Growing.StatesInstance smi) => smi.IsWilting()).EventTransition(GameHashes.CropSleep, this.stalled, (Growing.StatesInstance smi) => smi.IsSleeping()).Enter(new StateMachine<Growing.States, Growing.StatesInstance, Growing, object>.State.Callback(Growing.AddToScenePartitioner))
				.Exit(new StateMachine<Growing.States, Growing.StatesInstance, Growing, object>.State.Callback(Growing.RemoveFromScenePartitioner));
			this.growing.TriggerOnEnter(GameHashes.Grow, null).Update("CheckGrown", delegate(Growing.StatesInstance smi, float dt)
			{
				if (smi.ReachedNextHarvest())
				{
					smi.GoTo(this.grown);
				}
			}, UpdateRate.SIM_4000ms, false).ToggleStatusItem(Db.Get().CreatureStatusItems.Growing, (Growing.StatesInstance smi) => smi.master.GetComponent<Growing>())
				.Enter(delegate(Growing.StatesInstance smi)
				{
					GameStateMachine<Growing.States, Growing.StatesInstance, Growing, object>.State state = ((!smi.master.replanted) ? this.growing.wild : this.growing.planted);
					smi.GoTo(state);
				});
			this.growing.wild.ToggleAttributeModifier("GrowingWild", (Growing.StatesInstance smi) => smi.wildGrowingRate, null);
			this.growing.planted.ToggleAttributeModifier("Growing", (Growing.StatesInstance smi) => smi.baseGrowingRate, null);
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
				if (smi.master.oldAge.value >= smi.master.oldAge.GetMax())
				{
					smi.GoTo(this.grown.try_self_harvest);
				}
			}, UpdateRate.SIM_4000ms, false);
			this.grown.try_self_harvest.Enter(delegate(Growing.StatesInstance smi)
			{
				Harvestable component = smi.master.GetComponent<Harvestable>();
				if (component)
				{
					bool harvestWhenReady = component.HarvestWhenReady;
					component.ForceCancelHarvest(null);
					component.Harvest();
					if (harvestWhenReady && component != null)
					{
						component.SetHarvestWhenReady(true);
					}
				}
				smi.master.maturity.SetValue(0f);
				smi.master.oldAge.SetValue(0f);
			}).GoTo(this.grown.idle);
			this.stalled.EventTransition(GameHashes.WiltRecover, this.growing, (Growing.StatesInstance smi) => smi.CanExitStalled()).EventTransition(GameHashes.CropWakeUp, this.growing, (Growing.StatesInstance smi) => smi.CanExitStalled()).Update("Growing.stalled", delegate(Growing.StatesInstance smi, float dt)
			{
				if (smi.master.oldAge.value >= smi.master.oldAge.GetMax())
				{
					smi.GoTo(this.grown.try_self_harvest);
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

			public GameStateMachine<Growing.States, Growing.StatesInstance, Growing, object>.State try_self_harvest;
		}
	}
}
