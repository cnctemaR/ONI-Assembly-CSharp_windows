using System;
using Klei.AI;
using UnityEngine;

public class MilkProductionMonitor : GameStateMachine<MilkProductionMonitor, MilkProductionMonitor.Instance, IStateMachineTarget, MilkProductionMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.producing;
		this.producing.DefaultState(this.producing.paused).EventHandler(GameHashes.CaloriesConsumed, delegate(MilkProductionMonitor.Instance smi, object data)
		{
			smi.OnCaloriesConsumed(data);
		});
		this.producing.paused.Transition(this.producing.full, new StateMachine<MilkProductionMonitor, MilkProductionMonitor.Instance, IStateMachineTarget, MilkProductionMonitor.Def>.Transition.ConditionCallback(MilkProductionMonitor.IsFull), UpdateRate.SIM_1000ms).Transition(this.producing.producing, new StateMachine<MilkProductionMonitor, MilkProductionMonitor.Instance, IStateMachineTarget, MilkProductionMonitor.Def>.Transition.ConditionCallback(MilkProductionMonitor.IsProducing), UpdateRate.SIM_1000ms);
		this.producing.producing.Transition(this.producing.full, new StateMachine<MilkProductionMonitor, MilkProductionMonitor.Instance, IStateMachineTarget, MilkProductionMonitor.Def>.Transition.ConditionCallback(MilkProductionMonitor.IsFull), UpdateRate.SIM_1000ms).Transition(this.producing.paused, GameStateMachine<MilkProductionMonitor, MilkProductionMonitor.Instance, IStateMachineTarget, MilkProductionMonitor.Def>.Not(new StateMachine<MilkProductionMonitor, MilkProductionMonitor.Instance, IStateMachineTarget, MilkProductionMonitor.Def>.Transition.ConditionCallback(MilkProductionMonitor.IsProducing)), UpdateRate.SIM_1000ms).ToggleCritterEmotion(Db.Get().CritterEmotions.WellFed, null);
		this.producing.full.ToggleStatusItem(Db.Get().CreatureStatusItems.MilkFull, null).Transition(this.producing.paused, GameStateMachine<MilkProductionMonitor, MilkProductionMonitor.Instance, IStateMachineTarget, MilkProductionMonitor.Def>.Not(new StateMachine<MilkProductionMonitor, MilkProductionMonitor.Instance, IStateMachineTarget, MilkProductionMonitor.Def>.Transition.ConditionCallback(MilkProductionMonitor.IsFull)), UpdateRate.SIM_1000ms).Enter(delegate(MilkProductionMonitor.Instance smi)
		{
			smi.gameObject.AddTag(GameTags.Creatures.RequiresMilking);
		});
	}

	private static bool IsProducing(MilkProductionMonitor.Instance smi)
	{
		return !smi.IsFull && smi.IsUnderProductionEffect;
	}

	private static bool IsFull(MilkProductionMonitor.Instance smi)
	{
		return smi.IsFull;
	}

	private static bool HasCapacity(MilkProductionMonitor.Instance smi)
	{
		return !smi.IsFull;
	}

	public MilkProductionMonitor.ProducingStates producing;

	public class Def : StateMachine.BaseDef
	{
		public override void Configure(GameObject prefab)
		{
			prefab.GetComponent<Modifiers>().initialAmounts.Add(Db.Get().Amounts.MilkProduction.Id);
		}

		public SimHashes element = SimHashes.Milk;

		public Func<CreatureCalorieMonitor.CaloriesConsumedEvent, bool> dietConditionForMilkProduction;

		public string effectId;

		public float Capacity = 200f;

		public float CaloriesPerCycle = 1000f;

		public float HappinessRequired;
	}

	public class ProducingStates : GameStateMachine<MilkProductionMonitor, MilkProductionMonitor.Instance, IStateMachineTarget, MilkProductionMonitor.Def>.State
	{
		public GameStateMachine<MilkProductionMonitor, MilkProductionMonitor.Instance, IStateMachineTarget, MilkProductionMonitor.Def>.State paused;

		public GameStateMachine<MilkProductionMonitor, MilkProductionMonitor.Instance, IStateMachineTarget, MilkProductionMonitor.Def>.State producing;

		public GameStateMachine<MilkProductionMonitor, MilkProductionMonitor.Instance, IStateMachineTarget, MilkProductionMonitor.Def>.State full;
	}

	public new class Instance : GameStateMachine<MilkProductionMonitor, MilkProductionMonitor.Instance, IStateMachineTarget, MilkProductionMonitor.Def>.GameInstance
	{
		public float MilkAmount
		{
			get
			{
				return this.MilkPercentage / 100f * base.def.Capacity;
			}
		}

		public float MilkPercentage
		{
			get
			{
				return this.milkAmountInstance.value;
			}
		}

		public bool IsFull
		{
			get
			{
				return this.MilkPercentage >= this.milkAmountInstance.GetMax();
			}
		}

		public bool IsUnderProductionEffect
		{
			get
			{
				return this.milkAmountInstance.GetDelta() > 0f;
			}
		}

		public Instance(IStateMachineTarget master, MilkProductionMonitor.Def def)
			: base(master, def)
		{
		}

		public override void StartSM()
		{
			this.milkAmountInstance = Db.Get().Amounts.MilkProduction.Lookup(base.gameObject);
			if (base.def.effectId != null)
			{
				this.effectInstance = this.effects.Get(base.smi.def.effectId);
			}
			base.StartSM();
		}

		public void OnCaloriesConsumed(object data)
		{
			if (base.def.effectId == null)
			{
				return;
			}
			CreatureCalorieMonitor.CaloriesConsumedEvent value = ((Boxed<CreatureCalorieMonitor.CaloriesConsumedEvent>)data).value;
			if (base.def.dietConditionForMilkProduction != null && !base.def.dietConditionForMilkProduction(value))
			{
				return;
			}
			this.effectInstance = this.effects.Get(base.smi.def.effectId);
			if (this.effectInstance == null)
			{
				this.effectInstance = this.effects.Add(base.smi.def.effectId, true);
			}
			this.effectInstance.timeRemaining += value.calories / base.smi.def.CaloriesPerCycle * 600f;
		}

		private void RemoveMilk(float amount)
		{
			if (this.milkAmountInstance != null)
			{
				float num = Mathf.Min(this.milkAmountInstance.GetMin(), this.MilkPercentage - amount);
				this.milkAmountInstance.SetValue(num);
			}
		}

		public PrimaryElement ExtractMilk(float desiredAmount)
		{
			float num = Mathf.Min(desiredAmount, this.MilkAmount);
			float temperature = base.GetComponent<PrimaryElement>().Temperature;
			if (num <= 0f)
			{
				return null;
			}
			this.RemoveMilk(num);
			PrimaryElement component = LiquidSourceManager.Instance.CreateChunk(base.def.element, num, temperature, 0, 0, base.transform.GetPosition()).GetComponent<PrimaryElement>();
			component.KeepZeroMassObject = false;
			return component;
		}

		public PrimaryElement ExtractMilkIntoElementChunk(float desiredAmount, PrimaryElement elementChunk)
		{
			if (elementChunk == null || elementChunk.ElementID != base.def.element)
			{
				return null;
			}
			float num = Mathf.Min(desiredAmount, this.MilkAmount);
			float temperature = base.GetComponent<PrimaryElement>().Temperature;
			this.RemoveMilk(num);
			float mass = elementChunk.Mass;
			float finalTemperature = GameUtil.GetFinalTemperature(elementChunk.Temperature, mass, temperature, num);
			elementChunk.SetMassTemperature(mass + num, finalTemperature);
			return elementChunk;
		}

		public PrimaryElement ExtractMilkIntoStorage(float desiredAmount, Storage storage)
		{
			float num = Mathf.Min(desiredAmount, this.MilkAmount);
			float temperature = base.GetComponent<PrimaryElement>().Temperature;
			this.RemoveMilk(num);
			return storage.AddLiquid(base.def.element, num, temperature, 0, 0, false, true);
		}

		public Action<float> OnMilkAmountChanged;

		public AmountInstance milkAmountInstance;

		public EffectInstance effectInstance;

		[MyCmpGet]
		private Effects effects;
	}
}
