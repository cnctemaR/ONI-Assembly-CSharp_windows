using System;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

public class Growing : StateMachineComponent<Growing.StatesInstance>
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
		base.OnPrefabInit();
		Amounts amounts = base.gameObject.GetAmounts();
		this.maturity = amounts.Add(new AmountInstance(Db.Get().Amounts.Maturity, base.gameObject));
		this.baseMaturityMax = new AttributeModifier(this.maturity.maxAttribute.Id, this.baseGrowthTime / 600f, null, false);
		this.maturity.maxAttribute.Add("Base", this.baseMaturityMax);
		this.oldAge = amounts.Add(new AmountInstance(Db.Get().Amounts.OldAge, base.gameObject));
		this.Subscribe(1119167081, new EventSystem.EventHandler(this.OnNewGameSpawn));
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

	public bool IsGrown()
	{
		return this.maturity.value == this.maturity.GetMax();
	}

	public bool CanGrow()
	{
		return !this.IsGrown();
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
		this.regrowing = true;
	}

	public float PercentOldAge()
	{
		return this.oldAge.value / this.oldAge.GetMax();
	}

	public float baseGrowthTime;

	public float reGrowthTime;

	private AmountInstance maturity;

	private AmountInstance oldAge;

	private AttributeModifier baseMaturityMax;

	[Serialize]
	private bool replanted;

	[Serialize]
	private bool regrowing;

	[MyCmpGet]
	private WiltCondition wiltCondition;

	[MyCmpReq]
	private KSelectable selectable;

	[MyCmpReq]
	private Modifiers modifiers;

	public class StatesInstance : GameStateMachine<Growing.States, Growing.StatesInstance, Growing>.GameInstance
	{
		public StatesInstance(Growing master)
			: base(master)
		{
			this.baseGrowingRate = new AttributeModifier(master.maturity.deltaAttribute.Id, 0.0016666667f, CREATURES.STATS.MATURITY.GROWING, false);
			this.wildGrowingRate = new AttributeModifier(master.maturity.deltaAttribute.Id, 0.00041666668f, CREATURES.STATS.MATURITY.GROWINGWILD, false);
			this.getOldRate = new AttributeModifier(master.oldAge.deltaAttribute.Id, 1f, null, false);
		}

		public bool IsGrown()
		{
			return base.master.IsGrown();
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
			this.growing.Transition(this.grown, (Growing.StatesInstance smi) => smi.IsGrown()).ToggleStatusItem(Db.Get().CreatureStatusItems.Growing, (Growing.StatesInstance smi) => smi.master.GetComponent<Growing>()).Enter(delegate(Growing.StatesInstance smi)
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
			this.growing.wild.ToggleAttributeModifier("GrowingWild", (Growing.StatesInstance smi) => smi.wildGrowingRate);
			this.growing.planted.ToggleAttributeModifier("Growing", (Growing.StatesInstance smi) => smi.baseGrowingRate);
			this.grown.DefaultState(this.grown.idle).TriggerOnEnter(GameHashes.Grow, null).Transition(this.growing, (Growing.StatesInstance smi) => !smi.IsGrown())
				.ToggleAttributeModifier("GettingOld", (Growing.StatesInstance smi) => smi.getOldRate);
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
			this.stalled.EventTransition(GameHashes.WiltRecover, this.growing, (Growing.StatesInstance smi) => !smi.IsWilting());
		}

		public Growing.States.GrowingStates growing;

		public GameStateMachine<Growing.States, Growing.StatesInstance, Growing>.State stalled;

		public Growing.States.GrownStates grown;

		public class GrowingStates : GameStateMachine<Growing.States, Growing.StatesInstance, Growing>.State
		{
			public GameStateMachine<Growing.States, Growing.StatesInstance, Growing>.State wild;

			public GameStateMachine<Growing.States, Growing.StatesInstance, Growing>.State planted;
		}

		public class GrownStates : GameStateMachine<Growing.States, Growing.StatesInstance, Growing>.State
		{
			public GameStateMachine<Growing.States, Growing.StatesInstance, Growing>.State idle;

			public GameStateMachine<Growing.States, Growing.StatesInstance, Growing>.State try_self_harvest;
		}
	}
}
