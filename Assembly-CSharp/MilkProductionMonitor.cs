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
		this.producing.full.ToggleStatusItem((MilkProductionMonitor.Instance smi) => smi.def.fullStatusItem, null).Transition(this.producing.paused, GameStateMachine<MilkProductionMonitor, MilkProductionMonitor.Instance, IStateMachineTarget, MilkProductionMonitor.Def>.Not(new StateMachine<MilkProductionMonitor, MilkProductionMonitor.Instance, IStateMachineTarget, MilkProductionMonitor.Def>.Transition.ConditionCallback(MilkProductionMonitor.IsFull)), UpdateRate.SIM_1000ms).Enter(delegate(MilkProductionMonitor.Instance smi)
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
			if (this.fullStatusItem == null)
			{
				this.fullStatusItem = Db.Get().CreatureStatusItems.MilkFull;
			}
		}

		public SimHashes element = SimHashes.Milk;

		public Func<CreatureCalorieMonitor.CaloriesConsumedEvent, bool> dietConditionForMilkProduction;

		public string effectId;

		public float Capacity = 200f;

		public float CaloriesPerCycle = 1000f;

		public float HappinessRequired;

		public StatusItem fullStatusItem;
	}

	public class ProducingStates : GameStateMachine<MilkProductionMonitor, MilkProductionMonitor.Instance, IStateMachineTarget, MilkProductionMonitor.Def>.State
	{
		public GameStateMachine<MilkProductionMonitor, MilkProductionMonitor.Instance, IStateMachineTarget, MilkProductionMonitor.Def>.State paused;

		public GameStateMachine<MilkProductionMonitor, MilkProductionMonitor.Instance, IStateMachineTarget, MilkProductionMonitor.Def>.State producing;

		public GameStateMachine<MilkProductionMonitor, MilkProductionMonitor.Instance, IStateMachineTarget, MilkProductionMonitor.Def>.State full;
	}

	public new class Instance : GameStateMachine<MilkProductionMonitor, MilkProductionMonitor.Instance, IStateMachineTarget, MilkProductionMonitor.Def>.GameInstance, IMilkable
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

		public bool IsReadyToBeMilked()
		{
			return base.GetComponent<KPrefabID>().HasTag(GameTags.Creatures.RequiresMilking);
		}

		public SimHashes GetMilkElement()
		{
			return base.def.element;
		}

		public void MilkingComplete(Storage storage)
		{
			AmountInstance amountInstance = base.gameObject.GetAmounts().Get(Db.Get().Amounts.MilkProduction.Id);
			if (amountInstance.value > 0f)
			{
				float num = amountInstance.value * (base.def.Capacity / amountInstance.GetMax());
				storage.GetComponent<Storage>().AddLiquid(base.def.element, num, 310.15f, byte.MaxValue, 0, false, true);
				amountInstance.SetValue(0f);
			}
			base.GetComponent<KPrefabID>().RemoveTag(GameTags.Creatures.RequiresMilking);
		}

		private void RemoveMilk(float amount)
		{
			if (this.milkAmountInstance != null)
			{
				float num = Mathf.Max(this.milkAmountInstance.GetMin(), this.MilkPercentage - amount);
				this.milkAmountInstance.SetValue(num);
			}
		}

		public void RemoveMilkFromAmount(float amountKG)
		{
			float num = amountKG / base.def.Capacity * 100f;
			this.RemoveMilk(num);
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
