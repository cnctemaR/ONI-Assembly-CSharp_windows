using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
public class PressureVulnerable : StateMachineComponent<PressureVulnerable.StatesInstance>, IGameObjectEffectDescriptor
{
	private OccupyArea occupyArea
	{
		get
		{
			if (this._occupyArea == null)
			{
				this._occupyArea = base.GetComponent<OccupyArea>();
			}
			return this._occupyArea;
		}
	}

	public PressureVulnerable.PressureState GetExternalState
	{
		get
		{
			return this.pressureState;
		}
	}

	public bool IsLethal
	{
		get
		{
			return this.GetExternalState == PressureVulnerable.PressureState.LethalHigh || this.GetExternalState == PressureVulnerable.PressureState.LethalLow;
		}
	}

	public bool IsNormal
	{
		get
		{
			return this.GetExternalState == PressureVulnerable.PressureState.Normal || this.GetExternalState == PressureVulnerable.PressureState.Perfect;
		}
	}

	public bool IsPerfect
	{
		get
		{
			return this.GetExternalState == PressureVulnerable.PressureState.Perfect;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Amounts amounts = base.gameObject.GetAmounts();
		this.displayPressureAmount = amounts.Add(new AmountInstance(Db.Get().Amounts.AirPressure, base.gameObject));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.sm.pressure.Set(1f, base.smi);
		this.handle = GameScheduler.Instance.SchedulePeriodic(base.name, 1f, new Action<object>(this.UpdatePressure), null, null, 0f, null);
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		this.handle.Clear();
		base.OnCleanUp();
	}

	public void Configure(float pressureWarningLow = 0.25f, float pressureLethalLow = 0.01f, float pressureWarningHigh = 10f, float pressureLethalHigh = 30f, float pressurePerfectLow = 0.75f, float pressurePerfectHigh = 5f)
	{
		this.pressureWarning_Low = pressureWarningLow;
		this.pressureLethal_Low = pressureLethalLow;
		this.pressureLethal_High = pressureLethalHigh;
		this.pressureWarning_High = pressureWarningHigh;
		this.pressurePerfect_Low = pressurePerfectLow;
		this.pressurePerfect_High = pressurePerfectHigh;
	}

	public bool IsCellSafe(int cell)
	{
		float pressureOverArea = this.GetPressureOverArea(cell);
		return pressureOverArea > this.pressureLethal_Low && pressureOverArea < this.pressureLethal_High;
	}

	public void UpdatePressure(object data)
	{
		int num = Grid.PosToCell(base.gameObject);
		float pressureOverArea = this.GetPressureOverArea(num);
		base.smi.sm.pressure.Set(pressureOverArea, base.smi);
		this.displayPressureAmount.value = pressureOverArea;
	}

	private float GetPressureOverArea(int cell)
	{
		float pressure = 0f;
		int count = 0;
		this.occupyArea.TestArea(cell, delegate(int testCell)
		{
			if (Grid.IsGas(testCell))
			{
				pressure += Grid.Cell[testCell].mass;
				count++;
			}
			return true;
		});
		this.occupyArea.TestAreaAbove(cell, delegate(int testCell)
		{
			if (Grid.IsGas(testCell))
			{
				pressure += Grid.Cell[testCell].mass;
				count++;
			}
			return true;
		});
		pressure = ((count <= 0) ? 0f : (pressure / (float)count));
		return pressure;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return new List<Descriptor>
		{
			new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.REQUIRES_PRESSURE, GameUtil.GetFormattedMass(this.pressureWarning_Low, GameUtil.TimeSlice.None, true, "{0:0.#}")), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_PRESSURE, GameUtil.GetFormattedMass(this.pressureWarning_Low, GameUtil.TimeSlice.None, true, "{0:0.#}")), Descriptor.DescriptorType.Requirement, false),
			new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.IDEAL_PRESSURE, GameUtil.GetFormattedMass(this.pressurePerfect_Low, GameUtil.TimeSlice.None, true, "{0:0.#}"), GameUtil.GetFormattedMass(this.pressurePerfect_High, GameUtil.TimeSlice.None, true, "{0:0.#}")), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.IDEAL_PRESSURE, GameUtil.GetFormattedMass(this.pressurePerfect_Low, GameUtil.TimeSlice.None, true, "{0:0.#}"), GameUtil.GetFormattedMass(this.pressurePerfect_High, GameUtil.TimeSlice.None, true, "{0:0.#}")), Descriptor.DescriptorType.CropOptimumCondition, false)
		};
	}

	private OccupyArea _occupyArea;

	public float pressureLethal_Low;

	public float pressureWarning_Low;

	public float pressurePerfect_Low;

	public float pressurePerfect_High;

	public float pressureWarning_High;

	public float pressureLethal_High;

	private AmountInstance displayPressureAmount;

	private PressureVulnerable.PressureState pressureState = PressureVulnerable.PressureState.Normal;

	private SchedulerHandle handle;

	public class StatesInstance : GameStateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.GameInstance
	{
		public StatesInstance(PressureVulnerable master)
			: base(master)
		{
			AmountInstance amountInstance = Db.Get().Amounts.Maturity.Lookup(base.gameObject);
			if (amountInstance != null)
			{
				this.hasMaturity = true;
				this.badConditionModifier = new AttributeModifier(Db.Get().Amounts.YieldBonus.deltaAttribute.Id, 0f / amountInstance.GetMax(), CREATURES.STATS.YIELDBONUS.MODIFIERS.NOT_PERFECT_PRESSURE, false, false);
				this.goodConditionModifier = new AttributeModifier(Db.Get().Amounts.YieldBonus.deltaAttribute.Id, 0.00041666668f / amountInstance.GetMax(), CREATURES.STATS.YIELDBONUS.MODIFIERS.PERFECT_PRESSURE, false, false);
			}
		}

		public AttributeModifier badConditionModifier;

		public AttributeModifier goodConditionModifier;

		public bool hasMaturity;
	}

	public class States : GameStateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.normal;
			this.lethalLow.ToggleStatusItem(Db.Get().CreatureStatusItems.AtmosphericPressureTooLow, (PressureVulnerable.StatesInstance smi) => smi.master).TriggerOnEnter(GameHashes.LowPressureFatal, null).ParamTransition<float>(this.pressure, this.warningLow, (PressureVulnerable.StatesInstance smi, float p) => p > smi.master.pressureLethal_Low)
				.Enter(delegate(PressureVulnerable.StatesInstance smi)
				{
					smi.master.pressureState = PressureVulnerable.PressureState.LethalLow;
				});
			this.lethalHigh.ToggleStatusItem(Db.Get().CreatureStatusItems.AtmosphericPressureTooHigh, (PressureVulnerable.StatesInstance smi) => smi.master).TriggerOnEnter(GameHashes.HighPressureFatal, null).ParamTransition<float>(this.pressure, this.warningHigh, (PressureVulnerable.StatesInstance smi, float p) => p < smi.master.pressureLethal_High)
				.Enter(delegate(PressureVulnerable.StatesInstance smi)
				{
					smi.master.pressureState = PressureVulnerable.PressureState.LethalHigh;
				});
			this.warningLow.ToggleStatusItem(Db.Get().CreatureStatusItems.AtmosphericPressureTooLow, (PressureVulnerable.StatesInstance smi) => smi.master).TriggerOnEnter(GameHashes.LowPressureWarning, null).ParamTransition<float>(this.pressure, this.lethalLow, (PressureVulnerable.StatesInstance smi, float p) => p < smi.master.pressureLethal_Low)
				.ParamTransition<float>(this.pressure, this.normal, (PressureVulnerable.StatesInstance smi, float p) => p > smi.master.pressureWarning_Low)
				.Enter(delegate(PressureVulnerable.StatesInstance smi)
				{
					smi.master.pressureState = PressureVulnerable.PressureState.WarningLow;
				});
			this.warningHigh.ToggleStatusItem(Db.Get().CreatureStatusItems.AtmosphericPressureTooHigh, (PressureVulnerable.StatesInstance smi) => smi.master).TriggerOnEnter(GameHashes.HighPressureWarning, null).ParamTransition<float>(this.pressure, this.lethalHigh, (PressureVulnerable.StatesInstance smi, float p) => p > smi.master.pressureLethal_High)
				.ParamTransition<float>(this.pressure, this.normal, (PressureVulnerable.StatesInstance smi, float p) => p < smi.master.pressureWarning_High)
				.Enter(delegate(PressureVulnerable.StatesInstance smi)
				{
					smi.master.pressureState = PressureVulnerable.PressureState.WarningHigh;
				});
			this.normal.DefaultState(this.normal.okay).TriggerOnEnter(GameHashes.OptimalPressureAchieved, null).ParamTransition<float>(this.pressure, this.warningHigh, (PressureVulnerable.StatesInstance smi, float p) => p > smi.master.pressureWarning_High)
				.ParamTransition<float>(this.pressure, this.warningLow, (PressureVulnerable.StatesInstance smi, float p) => p < smi.master.pressureWarning_Low);
			this.normal.okay.ToggleAttributeModifier("Bad pressure", (PressureVulnerable.StatesInstance smi) => smi.badConditionModifier, (PressureVulnerable.StatesInstance smi) => smi.hasMaturity).ParamTransition<float>(this.pressure, this.normal.perfect, (PressureVulnerable.StatesInstance smi, float p) => p > smi.master.pressurePerfect_Low && p < smi.master.pressurePerfect_High).Enter(delegate(PressureVulnerable.StatesInstance smi)
			{
				smi.master.pressureState = PressureVulnerable.PressureState.Normal;
			});
			this.normal.perfect.ToggleStatusItem(Db.Get().CreatureStatusItems.PerfectAtmosphericPressure, (PressureVulnerable.StatesInstance smi) => smi.master).ToggleAttributeModifier("Good pressure", (PressureVulnerable.StatesInstance smi) => smi.goodConditionModifier, (PressureVulnerable.StatesInstance smi) => smi.hasMaturity).ParamTransition<float>(this.pressure, this.normal.okay, (PressureVulnerable.StatesInstance smi, float p) => p < smi.master.pressurePerfect_Low || p > smi.master.pressurePerfect_High)
				.Enter(delegate(PressureVulnerable.StatesInstance smi)
				{
					smi.master.pressureState = PressureVulnerable.PressureState.Perfect;
				});
		}

		public StateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.FloatParameter pressure;

		public GameStateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.State lethalLow;

		public GameStateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.State lethalHigh;

		public GameStateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.State warningLow;

		public GameStateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.State warningHigh;

		public PressureVulnerable.States.NormalStates normal;

		public class NormalStates : GameStateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.State
		{
			public GameStateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.State okay;

			public GameStateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.State perfect;
		}
	}

	public enum PressureState
	{
		LethalLow,
		WarningLow,
		Normal,
		Perfect,
		WarningHigh,
		LethalHigh
	}
}
