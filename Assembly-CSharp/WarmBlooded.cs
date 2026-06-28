using System;
using Klei;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class WarmBlooded : StateMachineComponent<WarmBlooded.StatesInstance>
{
	protected override void OnPrefabInit()
	{
		this.primaryElement = base.GetComponent<PrimaryElement>();
	}

	protected override void OnSpawn()
	{
		this.externalTemperature = this.GetAmounts().Get(Db.Get().Amounts.ExternalTemperature);
		this.externalTemperature.value = Grid.Temperature[Grid.PosToCell(this)];
		this.temperature = Db.Get().Amounts.Temperature.Lookup(base.gameObject);
		base.smi.StartSM();
		this.Subscribe(-1195989806, new Action<object>(this.OnEquippedItem));
	}

	private void SimUpdate(float dt)
	{
		if (this.monitorInstance != null)
		{
			this.monitorInstance.UpdateTemperatureOnSimUpdate(dt);
		}
		else
		{
			this.monitorInstance = base.gameObject.GetSMI<TemperatureMonitor.Instance>();
		}
	}

	private void OnEquippedItem(object data)
	{
		KPrefabID kprefabID = (KPrefabID)data;
		if (kprefabID != null)
		{
			foreach (Tag tag in kprefabID.Tags)
			{
				if (tag == GameTags.TemperatureSuit)
				{
					this.suitTank = kprefabID.GetComponent<SuitTank>();
					if (this.suitTank != null)
					{
						NameDisplayScreen.Instance.SetSuitTankDisplay(base.gameObject, new Func<float>(this.GetTankPercentage), true);
						break;
					}
				}
			}
		}
	}

	private void OnUnequippedItem(object data)
	{
		KPrefabID kprefabID = (KPrefabID)data;
		if (kprefabID != null && GameTags.AllSuitTags.Contains(kprefabID.PrefabTag))
		{
			foreach (Tag tag in kprefabID.Tags)
			{
				if (tag == GameTags.TemperatureSuit)
				{
					NameDisplayScreen.Instance.SetSuitTankDisplayState(false, base.gameObject);
					this.suitTank = null;
					break;
				}
			}
		}
	}

	public bool IsAtReasonableTemperature()
	{
		return !base.smi.IsHot() && !base.smi.IsCold();
	}

	public void SetTemperatureImmediate(float t)
	{
		this.temperature.value = t;
	}

	private float GetTankPercentage()
	{
		if (this.suitTank == null)
		{
			return 0f;
		}
		return this.suitTank.PercentFull();
	}

	public const float TRANSITION_DELAY_HOT = 3f;

	public const float TRANSITION_DELAY_COLD = 3f;

	public float temperatureDeltaJoules;

	public GameObject frozenPrefab;

	[MyCmpAdd]
	private Notifier notifier;

	private AmountInstance externalTemperature;

	public AmountInstance temperature;

	private PrimaryElement primaryElement;

	private TemperatureMonitor.Instance monitorInstance;

	private SuitTank suitTank;

	public class StatesInstance : GameStateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded, object>.GameInstance
	{
		public StatesInstance(WarmBlooded smi)
			: base(smi)
		{
			this.baseTemperatureModification = new AttributeModifier("TemperatureDelta", 0f, DUPLICANTS.MODIFIERS.BASEDUPLICANT.NAME, false, false);
			this.bodyRegulator = new AttributeModifier("TemperatureDelta", 0f, DUPLICANTS.MODIFIERS.HOMEOSTASIS.NAME, false, false);
			this.bodyRegulator.UIOnly = true;
			this.baseTemperatureModification.UIOnly = true;
			this.burningCalories = new AttributeModifier("CaloriesDelta", 0f, DUPLICANTS.MODIFIERS.BURNINGCALORIES.NAME, false, false);
			base.master.GetAttributes().Add(DUPLICANTS.MODIFIERS.HOMEOSTASIS.NAME, this.bodyRegulator);
			base.master.GetAttributes().Add(DUPLICANTS.MODIFIERS.BURNINGCALORIES.NAME, this.burningCalories);
			base.master.GetAttributes().Add(DUPLICANTS.MODIFIERS.BASEDUPLICANT.NAME, this.baseTemperatureModification);
			base.master.SetTemperatureImmediate(310.15f);
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

		public bool IsHot()
		{
			return this.BodyTemperature > 310.15f;
		}

		public bool IsCold()
		{
			return this.BodyTemperature < 310.15f;
		}

		public void UpdateTank(float dt)
		{
			int num = Grid.PosToCell(base.master);
			base.master.externalTemperature.value = Grid.Temperature[num];
			if (base.master.suitTank != null && !base.master.suitTank.IsEmpty())
			{
				base.master.suitTank.amount -= dt;
			}
		}

		public AttributeModifier baseTemperatureModification;

		public AttributeModifier bodyRegulator;

		public AttributeModifier averageBodyRegulation;

		public AttributeModifier burningCalories;

		public float averageInternalTemperature;
	}

	public class States : GameStateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.alive.normal;
			this.root.EventTransition(GameHashes.Died, this.dead, null).Enter(delegate(WarmBlooded.StatesInstance smi)
			{
				PrimaryElement component = smi.master.GetComponent<PrimaryElement>();
				float num = SimUtil.EnergyFlowToTemperatureDelta(0.08368001f, component.Element.specificHeatCapacity, component.Mass);
				smi.baseTemperatureModification.SetValue(num);
				CreatureSimTemperatureTransfer component2 = smi.master.GetComponent<CreatureSimTemperatureTransfer>();
				component2.NonSimTemperatureModifiers.Add(smi.baseTemperatureModification);
				component2.NonSimTemperatureModifiers.Add(smi.bodyRegulator);
			}).Update(delegate(WarmBlooded.StatesInstance smi)
			{
				smi.UpdateTank(smi.dt);
			});
			this.alive.normal.Transition(this.alive.cold.transition, (WarmBlooded.StatesInstance smi) => smi.IsCold()).Transition(this.alive.hot.transition, (WarmBlooded.StatesInstance smi) => smi.IsHot());
			this.alive.cold.transition.ScheduleGoTo(3f, this.alive.cold.regulating).Transition(this.alive.normal, (WarmBlooded.StatesInstance smi) => !smi.IsCold());
			this.alive.cold.regulating.Transition(this.alive.normal, (WarmBlooded.StatesInstance smi) => !smi.IsCold()).Update(delegate(WarmBlooded.StatesInstance smi)
			{
				PrimaryElement component3 = smi.master.GetComponent<PrimaryElement>();
				float num2 = SimUtil.EnergyFlowToTemperatureDelta(0.08368001f, component3.Element.specificHeatCapacity, component3.Mass);
				float num3 = SimUtil.EnergyFlowToTemperatureDelta(0.5578667f, component3.Element.specificHeatCapacity, component3.Mass);
				float num4 = 310.15f - smi.BodyTemperature;
				float num5 = 1f;
				if (num3 + num2 > num4)
				{
					num5 = Mathf.Max(0f, num4 - num2) / num3;
				}
				smi.bodyRegulator.SetValue(num3 * num5);
				smi.burningCalories.SetValue(-0.5578667f * num5 * 1000f / 4184f);
			}).Exit(delegate(WarmBlooded.StatesInstance smi)
			{
				smi.bodyRegulator.SetValue(0f);
				smi.burningCalories.SetValue(0f);
			});
			this.alive.hot.transition.ScheduleGoTo(3f, this.alive.hot.regulating).Transition(this.alive.normal, (WarmBlooded.StatesInstance smi) => !smi.IsHot());
			this.alive.hot.regulating.Transition(this.alive.normal, (WarmBlooded.StatesInstance smi) => !smi.IsHot()).Update(delegate(WarmBlooded.StatesInstance smi)
			{
				PrimaryElement component4 = smi.master.GetComponent<PrimaryElement>();
				float num6 = SimUtil.EnergyFlowToTemperatureDelta(0.5578667f, component4.Element.specificHeatCapacity, component4.Mass);
				float num7 = 310.15f - smi.BodyTemperature;
				float num8 = 1f;
				if ((num6 - smi.baseTemperatureModification.Value) * smi.dt < num7)
				{
					num8 = Mathf.Clamp(num7 / ((num6 - smi.baseTemperatureModification.Value) * smi.dt), 0f, 1f);
				}
				smi.bodyRegulator.SetValue(-num6 * num8);
				smi.burningCalories.SetValue(-0.5578667f * num8 / 4184f);
			}).Exit(delegate(WarmBlooded.StatesInstance smi)
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
