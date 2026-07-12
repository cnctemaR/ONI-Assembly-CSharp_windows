using System;
using Klei;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class WarmBlooded : StateMachineComponent<WarmBlooded.StatesInstance>
{
	public static bool IsCold(WarmBlooded.StatesInstance smi)
	{
		return !smi.IsSimpleHeatProducer() && smi.IsCold();
	}

	public static bool IsHot(WarmBlooded.StatesInstance smi)
	{
		return !smi.IsSimpleHeatProducer() && smi.IsHot();
	}

	public static void WarmingRegulator(WarmBlooded.StatesInstance smi, float dt)
	{
		PrimaryElement component = smi.master.GetComponent<PrimaryElement>();
		float num = SimUtil.EnergyFlowToTemperatureDelta(smi.master.CoolingKW, component.Element.specificHeatCapacity, component.Mass);
		float num2 = smi.IdealTemperature - smi.BodyTemperature;
		float num3 = 1f;
		if ((num - smi.baseTemperatureModification.Value) * dt < num2)
		{
			num3 = Mathf.Clamp(num2 / ((num - smi.baseTemperatureModification.Value) * dt), 0f, 1f);
		}
		smi.bodyRegulator.SetValue(-num * num3);
		if (smi.master.complexity == WarmBlooded.ComplexityType.FullHomeostasis)
		{
			smi.burningCalories.SetValue(-smi.master.CoolingKW * num3 / smi.master.KCal2Joules);
		}
	}

	public static void CoolingRegulator(WarmBlooded.StatesInstance smi, float dt)
	{
		PrimaryElement component = smi.master.GetComponent<PrimaryElement>();
		float num = SimUtil.EnergyFlowToTemperatureDelta(smi.master.BaseGenerationKW, component.Element.specificHeatCapacity, component.Mass);
		float num2 = SimUtil.EnergyFlowToTemperatureDelta(smi.master.WarmingKW, component.Element.specificHeatCapacity, component.Mass);
		float num3 = smi.IdealTemperature - smi.BodyTemperature;
		float num4 = 1f;
		if (num2 + num > num3)
		{
			num4 = Mathf.Max(0f, num3 - num) / num2;
		}
		smi.bodyRegulator.SetValue(num2 * num4);
		if (smi.master.complexity == WarmBlooded.ComplexityType.FullHomeostasis)
		{
			smi.burningCalories.SetValue(-smi.master.WarmingKW * num4 * 1000f / smi.master.KCal2Joules);
		}
	}

	protected override void OnPrefabInit()
	{
		this.temperature = Db.Get().Amounts.Get(this.TemperatureAmountName).Lookup(base.gameObject);
		this.primaryElement = base.GetComponent<PrimaryElement>();
	}

	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	public void SetTemperatureImmediate(float t)
	{
		this.temperature.value = t;
	}

	[MyCmpAdd]
	private Notifier notifier;

	public AmountInstance temperature;

	private PrimaryElement primaryElement;

	public WarmBlooded.ComplexityType complexity = WarmBlooded.ComplexityType.FullHomeostasis;

	public string TemperatureAmountName = "Temperature";

	public float IdealTemperature = 310.15f;

	public float BaseGenerationKW = 0.08368001f;

	public string BaseTemperatureModifierDescription = DUPLICANTS.MODIFIERS.BASEDUPLICANT.NAME;

	public float KCal2Joules = 4184f;

	public float WarmingKW = 0.5578667f;

	public float CoolingKW = 0.5578667f;

	public string CaloriesModifierDescription = DUPLICANTS.MODIFIERS.BURNINGCALORIES.NAME;

	public string BodyRegulatorModifierDescription = DUPLICANTS.MODIFIERS.HOMEOSTASIS.NAME;

	public const float TRANSITION_DELAY_HOT = 3f;

	public const float TRANSITION_DELAY_COLD = 3f;

	public enum ComplexityType
	{
		SimpleHeatProduction,
		HomeostasisWithoutCaloriesImpact,
		FullHomeostasis
	}

	public class StatesInstance : GameStateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded, object>.GameInstance
	{
		public StatesInstance(WarmBlooded smi)
			: base(smi)
		{
			this.baseTemperatureModification = new AttributeModifier(base.master.TemperatureAmountName + "Delta", 0f, base.master.BaseTemperatureModifierDescription, false, true, false);
			base.master.GetAttributes().Add(this.baseTemperatureModification);
			if (base.master.complexity != WarmBlooded.ComplexityType.SimpleHeatProduction)
			{
				this.bodyRegulator = new AttributeModifier(base.master.TemperatureAmountName + "Delta", 0f, base.master.BodyRegulatorModifierDescription, false, true, false);
				base.master.GetAttributes().Add(this.bodyRegulator);
			}
			if (base.master.complexity == WarmBlooded.ComplexityType.FullHomeostasis)
			{
				this.burningCalories = new AttributeModifier("CaloriesDelta", 0f, base.master.CaloriesModifierDescription, false, false, false);
				base.master.GetAttributes().Add(this.burningCalories);
			}
			base.master.SetTemperatureImmediate(this.IdealTemperature);
		}

		public float IdealTemperature
		{
			get
			{
				return base.master.IdealTemperature;
			}
		}

		public float TemperatureDelta
		{
			get
			{
				return this.bodyRegulator.Value;
			}
		}

		public float BodyTemperature
		{
			get
			{
				return base.master.primaryElement.Temperature;
			}
		}

		public bool IsSimpleHeatProducer()
		{
			return base.master.complexity == WarmBlooded.ComplexityType.SimpleHeatProduction;
		}

		public bool IsHot()
		{
			return this.BodyTemperature > this.IdealTemperature;
		}

		public bool IsCold()
		{
			return this.BodyTemperature < this.IdealTemperature;
		}

		public AttributeModifier baseTemperatureModification;

		public AttributeModifier bodyRegulator;

		public AttributeModifier burningCalories;
	}

	public class States : GameStateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.alive.normal;
			this.root.TagTransition(GameTags.Dead, this.dead, false).Enter(delegate(WarmBlooded.StatesInstance smi)
			{
				PrimaryElement component = smi.master.GetComponent<PrimaryElement>();
				float num = SimUtil.EnergyFlowToTemperatureDelta(smi.master.BaseGenerationKW, component.Element.specificHeatCapacity, component.Mass);
				smi.baseTemperatureModification.SetValue(num);
				CreatureSimTemperatureTransfer component2 = smi.master.GetComponent<CreatureSimTemperatureTransfer>();
				component2.NonSimTemperatureModifiers.Add(smi.baseTemperatureModification);
				if (!smi.IsSimpleHeatProducer())
				{
					component2.NonSimTemperatureModifiers.Add(smi.bodyRegulator);
				}
			});
			this.alive.normal.Transition(this.alive.cold.transition, new StateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded, object>.Transition.ConditionCallback(WarmBlooded.IsCold), UpdateRate.SIM_200ms).Transition(this.alive.hot.transition, new StateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded, object>.Transition.ConditionCallback(WarmBlooded.IsHot), UpdateRate.SIM_200ms);
			this.alive.cold.transition.ScheduleGoTo(3f, this.alive.cold.regulating).Transition(this.alive.normal, GameStateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded, object>.Not(new StateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded, object>.Transition.ConditionCallback(WarmBlooded.IsCold)), UpdateRate.SIM_200ms);
			this.alive.cold.regulating.Transition(this.alive.normal, GameStateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded, object>.Not(new StateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded, object>.Transition.ConditionCallback(WarmBlooded.IsCold)), UpdateRate.SIM_200ms).Update("ColdRegulating", new Action<WarmBlooded.StatesInstance, float>(WarmBlooded.CoolingRegulator), UpdateRate.SIM_200ms, false).Exit(delegate(WarmBlooded.StatesInstance smi)
			{
				smi.bodyRegulator.SetValue(0f);
				if (smi.master.complexity == WarmBlooded.ComplexityType.FullHomeostasis)
				{
					smi.burningCalories.SetValue(0f);
				}
			});
			this.alive.hot.transition.ScheduleGoTo(3f, this.alive.hot.regulating).Transition(this.alive.normal, GameStateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded, object>.Not(new StateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded, object>.Transition.ConditionCallback(WarmBlooded.IsHot)), UpdateRate.SIM_200ms);
			this.alive.hot.regulating.Transition(this.alive.normal, GameStateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded, object>.Not(new StateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded, object>.Transition.ConditionCallback(WarmBlooded.IsHot)), UpdateRate.SIM_200ms).Update("WarmRegulating", new Action<WarmBlooded.StatesInstance, float>(WarmBlooded.WarmingRegulator), UpdateRate.SIM_200ms, false).Exit(delegate(WarmBlooded.StatesInstance smi)
			{
				smi.bodyRegulator.SetValue(0f);
			});
			this.dead.Enter(delegate(WarmBlooded.StatesInstance smi)
			{
				smi.master.enabled = false;
			});
		}

		public WarmBlooded.States.AliveState alive;

		public GameStateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded, object>.State dead;

		public class RegulatingState : GameStateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded, object>.State
		{
			public GameStateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded, object>.State transition;

			public GameStateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded, object>.State regulating;
		}

		public class AliveState : GameStateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded, object>.State
		{
			public GameStateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded, object>.State normal;

			public WarmBlooded.States.RegulatingState cold;

			public WarmBlooded.States.RegulatingState hot;
		}
	}
}
