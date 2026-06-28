using System;
using Klei;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class WarmBlooded : StateMachineComponent<WarmBlooded.StatesInstance>
{
	protected override void OnPrefabInit()
	{
		this.energyTransferWithWorld = new AttributeModifier("TemperatureDelta", 0f, DUPLICANTS.MODIFIERS.TEMPEXCHANGE.NAME, false);
	}

	protected override void OnSpawn()
	{
		this.GetAttributes().Add(DUPLICANTS.MODIFIERS.TEMPEXCHANGE.NAME, this.energyTransferWithWorld);
		this.externalTemperature = this.GetAmounts().Get("ExternalTemperature");
		this.externalTemperature.value = Grid.Temperature[Grid.PosToCell(this)];
		this.temperature = Db.Get().Amounts.Temperature.Lookup(base.gameObject);
		this.insulation = Db.Get().AttributeConverters.TemperatureInsulation.Lookup(base.gameObject);
		base.smi.StartSM();
		this.Subscribe(-1195989806, new EventSystem.EventHandler(this.OnEquippedItem));
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

	private void TransferWithEnvironment(float dt)
	{
		int num = Grid.PosToCell(this);
		if (!Grid.IsValidCell(num))
		{
			return;
		}
		num = Grid.CellAbove(num);
		Element element = Grid.Element[num];
		this.energyTransferWithWorld.SetValue(0f);
		if (element.IsVacuum || element.IsSolid)
		{
			return;
		}
		PrimaryElement component = base.GetComponent<PrimaryElement>();
		float num2 = SimUtil.CalculateEnergyFlowCreatures(dt, num, this.temperature.value, component.Mass, 3.47f, 1f, 2f, 0.005f);
		if (num2 != 0f)
		{
			SimUtil.CheckValidValue(num2);
			float num3 = SimUtil.EnergyFlowToTemperatureDelta(num2, 3.47f, component.Mass);
			SimUtil.CheckValidValue(num3);
			if (Mathf.Abs(num3) > 10f)
			{
				Output.LogWarningWithObj(base.gameObject, new object[]
				{
					"Sudden temperature change on [" + num3 + "]",
					base.name
				});
			}
			if (this.insulation != null)
			{
				num3 *= 1f - this.insulation.Evaluate();
			}
			this.energyTransferWithWorld.SetValue(num3);
			SimMessages.ModifyEnergy(num, -num2, SimMessages.EnergySourceID.WarmBlooded);
		}
		this.externalTemperature.value = Grid.Temperature[num];
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

	public float temperatureDeltaJoules;

	public GameObject frozenPrefab;

	[MyCmpAdd]
	private Notifier notifier;

	[MyCmpAdd]
	private InfraredVisualizer infraredVisualizer;

	private AttributeModifier energyTransferWithWorld;

	private AmountInstance externalTemperature;

	public AmountInstance temperature;

	private AttributeConverterInstance insulation;

	private SuitTank suitTank;

	public class StatesInstance : GameStateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded>.GameInstance
	{
		public StatesInstance(WarmBlooded smi)
			: base(smi)
		{
			this.bodyRegulator = new AttributeModifier("TemperatureDelta", 0f, DUPLICANTS.MODIFIERS.HOMEOSTASIS.NAME, false);
			this.burningCalories = new AttributeModifier("CaloriesDelta", 0f, DUPLICANTS.MODIFIERS.BURNINGCALORIES.NAME, false);
			base.master.GetAttributes().Add(DUPLICANTS.MODIFIERS.HOMEOSTASIS.NAME, this.bodyRegulator);
			base.master.GetAttributes().Add(DUPLICANTS.MODIFIERS.BURNINGCALORIES.NAME, this.burningCalories);
			base.master.SetTemperatureImmediate(310.15f);
		}

		public float TemperatureDeltaNet
		{
			get
			{
				return this.bodyRegulator.Value + base.master.energyTransferWithWorld.Value;
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
				return base.master.temperature.value;
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

		public AttributeModifier bodyRegulator;

		public AttributeModifier burningCalories;
	}

	public class States : GameStateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.alive.normal;
			this.root.EventTransition(GameHashes.Died, this.dead, null).Update(delegate(WarmBlooded.StatesInstance smi)
			{
				smi.UpdateTank(smi.dt);
			});
			this.alive.normal.Transition(this.alive.cold.transition, (WarmBlooded.StatesInstance smi) => smi.IsCold()).Transition(this.alive.hot.transition, (WarmBlooded.StatesInstance smi) => smi.IsHot());
			this.alive.cold.transition.ScheduleGoTo(5f, this.alive.cold.regulating);
			this.alive.cold.regulating.Transition(this.alive.normal, (WarmBlooded.StatesInstance smi) => !smi.IsCold()).ToggleStatusItem(Db.Get().DuplicantStatusItems.BodyRegulatingHeating, null).Enter(delegate(WarmBlooded.StatesInstance smi)
			{
				PrimaryElement component = smi.master.GetComponent<PrimaryElement>();
				float num = SimUtil.EnergyFlowToTemperatureDelta(48f, 3.47f, component.Mass);
				smi.bodyRegulator.SetValue(num);
				smi.burningCalories.SetValue(-1645.7144f);
			})
				.Exit(delegate(WarmBlooded.StatesInstance smi)
				{
					smi.bodyRegulator.SetValue(0f);
					smi.burningCalories.SetValue(0f);
				});
			this.alive.hot.transition.ScheduleGoTo(5f, this.alive.hot.regulating);
			this.alive.hot.regulating.Transition(this.alive.normal, (WarmBlooded.StatesInstance smi) => !smi.IsHot()).ToggleStatusItem(Db.Get().DuplicantStatusItems.BodyRegulatingCooling, null).Enter(delegate(WarmBlooded.StatesInstance smi)
			{
				PrimaryElement component2 = smi.master.GetComponent<PrimaryElement>();
				float num2 = SimUtil.EnergyFlowToTemperatureDelta(48f, 3.47f, component2.Mass);
				smi.bodyRegulator.SetValue(-num2);
				smi.burningCalories.SetValue(-1645.7144f);
			})
				.Exit(delegate(WarmBlooded.StatesInstance smi)
				{
					smi.bodyRegulator.SetValue(0f);
				});
			this.dead.Enter(delegate(WarmBlooded.StatesInstance smi)
			{
				smi.master.enabled = false;
			});
		}

		public WarmBlooded.States.AliveState alive;

		public GameStateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded>.State dead;

		public class RegulatingState : GameStateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded>.State
		{
			public GameStateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded>.State transition;

			public GameStateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded>.State regulating;
		}

		public class AliveState : GameStateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded>.State
		{
			public GameStateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded>.State normal;

			public WarmBlooded.States.RegulatingState cold;

			public WarmBlooded.States.RegulatingState hot;
		}
	}
}
