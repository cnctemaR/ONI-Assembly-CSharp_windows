using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
public class TemperatureVulnerable : StateMachineComponent<TemperatureVulnerable.StatesInstance>, IGameObjectEffectDescriptor
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

	public float InternalTemperature
	{
		get
		{
			return this.primaryElement.Temperature;
		}
	}

	public TemperatureVulnerable.TemperatureState GetExternalState
	{
		get
		{
			return this.externalTemperatureState;
		}
	}

	public bool IsLethal
	{
		get
		{
			return this.GetExternalState == TemperatureVulnerable.TemperatureState.LethalHot || this.GetExternalState == TemperatureVulnerable.TemperatureState.LethalCold;
		}
	}

	public bool IsNormal
	{
		get
		{
			return this.GetExternalState == TemperatureVulnerable.TemperatureState.Normal || this.GetExternalState == TemperatureVulnerable.TemperatureState.Perfect;
		}
	}

	public bool IsPerfect
	{
		get
		{
			return this.GetExternalState == TemperatureVulnerable.TemperatureState.Perfect;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		float num = this.externalTemperatureWarning_Low + 0.5f * (this.externalTemperatureWarning_High - this.externalTemperatureWarning_Low);
		this.primaryElement.Temperature = num;
		base.smi.sm.externalTemp.Set(num, base.smi);
		this.handle = GameScheduler.Instance.SchedulePeriodic(base.name, 1f, new Action<object>(this.UpdateTemperature), null, null, 0f, null);
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		this.handle.Clear();
		base.OnCleanUp();
	}

	public void Configure(float tempWarningLow = 283f, float tempLethalLow = 263f, float tempWarningHigh = 294f, float tempLethalHigh = 343f, float tempPerfectLow = 0f, float tempPerfectHigh = 0f)
	{
		this.externalTemperatureWarning_Low = tempWarningLow;
		this.externalTemperatureLethal_Low = tempLethalLow;
		this.externalTemperatureLethal_High = tempLethalHigh;
		this.externalTemperatureWarning_High = tempWarningHigh;
		this.externalTemperaturePerfect_Low = tempPerfectLow;
		this.externalTemperaturePerfect_High = tempPerfectHigh;
	}

	public bool IsCellSafe(int cell)
	{
		float averageTemperature = this.GetAverageTemperature(cell);
		return averageTemperature > -1f && averageTemperature > this.externalTemperatureLethal_Low && averageTemperature < this.externalTemperatureLethal_High;
	}

	public void UpdateTemperature(object data)
	{
		int num = Grid.PosToCell(base.gameObject);
		if (!Grid.IsValidCell(num))
		{
			return;
		}
		float averageTemperature = this.GetAverageTemperature(num);
		if (averageTemperature > -1f)
		{
			base.smi.sm.externalTemp.Set(averageTemperature, base.smi);
		}
	}

	private float GetAverageTemperature(int cell)
	{
		float temperature = 0f;
		int count = 0;
		this.occupyArea.TestArea(cell, delegate(int testCell)
		{
			if (Grid.Cell[testCell].mass > 0.1f)
			{
				temperature += Grid.Temperature[testCell];
				count++;
			}
			return true;
		});
		if (count > 0)
		{
			return temperature / (float)count;
		}
		return -1f;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return new List<Descriptor>
		{
			new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.REQUIRES_TEMPERATURE, GameUtil.GetFormattedTemperature(this.externalTemperatureWarning_Low, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, false), GameUtil.GetFormattedTemperature(this.externalTemperatureWarning_High, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true)), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_TEMPERATURE, GameUtil.GetFormattedTemperature(this.externalTemperatureWarning_Low, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, false), GameUtil.GetFormattedTemperature(this.externalTemperatureWarning_High, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true)), Descriptor.DescriptorType.Requirement, false),
			new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.IDEAL_TEMPERATURE, GameUtil.GetFormattedTemperature(this.externalTemperaturePerfect_Low, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, false), GameUtil.GetFormattedTemperature(this.externalTemperaturePerfect_High, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true)), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.IDEAL_TEMPERATURE, GameUtil.GetFormattedTemperature(this.externalTemperaturePerfect_Low, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, false), GameUtil.GetFormattedTemperature(this.externalTemperaturePerfect_High, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true)), Descriptor.DescriptorType.CropOptimumCondition, false)
		};
	}

	private const float minimumMassForReading = 0.1f;

	private OccupyArea _occupyArea;

	public float externalTemperatureLethal_Low;

	public float externalTemperatureWarning_Low;

	public float externalTemperaturePerfect_Low;

	public float externalTemperaturePerfect_High;

	public float externalTemperatureWarning_High;

	public float externalTemperatureLethal_High;

	[MyCmpReq]
	private PrimaryElement primaryElement;

	[MyCmpReq]
	private SimTemperatureTransfer temperatureTransfer;

	private TemperatureVulnerable.TemperatureState externalTemperatureState = TemperatureVulnerable.TemperatureState.Normal;

	private SchedulerHandle handle;

	public class StatesInstance : GameStateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.GameInstance
	{
		public StatesInstance(TemperatureVulnerable master)
			: base(master)
		{
			AmountInstance amountInstance = Db.Get().Amounts.Maturity.Lookup(base.gameObject);
			if (amountInstance != null)
			{
				this.hasMaturity = true;
				this.badConditionModifier = new AttributeModifier(Db.Get().Amounts.YieldBonus.deltaAttribute.Id, 0f / amountInstance.GetMax(), CREATURES.STATS.YIELDBONUS.MODIFIERS.NOT_PERFECT_TEMPERATURE, false, false);
				this.goodConditionModifier = new AttributeModifier(Db.Get().Amounts.YieldBonus.deltaAttribute.Id, 0.00041666668f / amountInstance.GetMax(), CREATURES.STATS.YIELDBONUS.MODIFIERS.PERFECT_TEMPERATURE, false, false);
			}
		}

		public AttributeModifier badConditionModifier;

		public AttributeModifier goodConditionModifier;

		public bool hasMaturity;
	}

	public class States : GameStateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.normal;
			this.lethalCold.ToggleStatusItem(Db.Get().CreatureStatusItems.Cold, (TemperatureVulnerable.StatesInstance smi) => smi.master).TriggerOnEnter(GameHashes.TooColdFatal, null).ParamTransition<float>(this.externalTemp, this.warningCold, (TemperatureVulnerable.StatesInstance smi, float p) => p > smi.master.externalTemperatureLethal_Low)
				.Enter(delegate(TemperatureVulnerable.StatesInstance smi)
				{
					smi.master.externalTemperatureState = TemperatureVulnerable.TemperatureState.LethalCold;
				});
			this.lethalHot.ToggleStatusItem(Db.Get().CreatureStatusItems.Hot, (TemperatureVulnerable.StatesInstance smi) => smi.master).TriggerOnEnter(GameHashes.TooHotFatal, null).ParamTransition<float>(this.externalTemp, this.warningHot, (TemperatureVulnerable.StatesInstance smi, float p) => p < smi.master.externalTemperatureLethal_High)
				.Enter(delegate(TemperatureVulnerable.StatesInstance smi)
				{
					smi.master.externalTemperatureState = TemperatureVulnerable.TemperatureState.LethalHot;
				});
			this.warningCold.ToggleStatusItem(Db.Get().CreatureStatusItems.Cold, (TemperatureVulnerable.StatesInstance smi) => smi.master).TriggerOnEnter(GameHashes.TooColdWarning, null).ParamTransition<float>(this.externalTemp, this.lethalCold, (TemperatureVulnerable.StatesInstance smi, float p) => p < smi.master.externalTemperatureLethal_Low)
				.ParamTransition<float>(this.externalTemp, this.normal, (TemperatureVulnerable.StatesInstance smi, float p) => p > smi.master.externalTemperatureWarning_Low)
				.Enter(delegate(TemperatureVulnerable.StatesInstance smi)
				{
					smi.master.externalTemperatureState = TemperatureVulnerable.TemperatureState.WarningCold;
				});
			this.warningHot.ToggleStatusItem(Db.Get().CreatureStatusItems.Hot, (TemperatureVulnerable.StatesInstance smi) => smi.master).TriggerOnEnter(GameHashes.TooHotWarning, null).ParamTransition<float>(this.externalTemp, this.lethalHot, (TemperatureVulnerable.StatesInstance smi, float p) => p > smi.master.externalTemperatureLethal_High)
				.ParamTransition<float>(this.externalTemp, this.normal, (TemperatureVulnerable.StatesInstance smi, float p) => p < smi.master.externalTemperatureWarning_High)
				.Enter(delegate(TemperatureVulnerable.StatesInstance smi)
				{
					smi.master.externalTemperatureState = TemperatureVulnerable.TemperatureState.WarningHot;
				});
			this.normal.DefaultState(this.normal.okay).TriggerOnEnter(GameHashes.OptimalTemperatureAchieved, null).ParamTransition<float>(this.externalTemp, this.warningHot, (TemperatureVulnerable.StatesInstance smi, float p) => p > smi.master.externalTemperatureWarning_High)
				.ParamTransition<float>(this.externalTemp, this.warningCold, (TemperatureVulnerable.StatesInstance smi, float p) => p < smi.master.externalTemperatureWarning_Low);
			this.normal.okay.ToggleAttributeModifier("Bad temperature", (TemperatureVulnerable.StatesInstance smi) => smi.badConditionModifier, (TemperatureVulnerable.StatesInstance smi) => smi.hasMaturity).ParamTransition<float>(this.externalTemp, this.normal.perfect, (TemperatureVulnerable.StatesInstance smi, float p) => p > smi.master.externalTemperaturePerfect_Low && p < smi.master.externalTemperaturePerfect_High).Enter(delegate(TemperatureVulnerable.StatesInstance smi)
			{
				smi.master.externalTemperatureState = TemperatureVulnerable.TemperatureState.Normal;
			});
			this.normal.perfect.ToggleStatusItem(Db.Get().CreatureStatusItems.PerfectTemperature, (TemperatureVulnerable.StatesInstance smi) => smi.master).ToggleAttributeModifier("Good temperature", (TemperatureVulnerable.StatesInstance smi) => smi.goodConditionModifier, (TemperatureVulnerable.StatesInstance smi) => smi.hasMaturity).ParamTransition<float>(this.externalTemp, this.normal.okay, (TemperatureVulnerable.StatesInstance smi, float p) => p < smi.master.externalTemperaturePerfect_Low || p > smi.master.externalTemperaturePerfect_High)
				.Enter(delegate(TemperatureVulnerable.StatesInstance smi)
				{
					smi.master.externalTemperatureState = TemperatureVulnerable.TemperatureState.Perfect;
				});
		}

		public StateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.FloatParameter externalTemp;

		public GameStateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.State lethalCold;

		public GameStateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.State lethalHot;

		public GameStateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.State warningCold;

		public GameStateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.State warningHot;

		public TemperatureVulnerable.States.NormalStates normal;

		public class NormalStates : GameStateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.State
		{
			public GameStateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.State okay;

			public GameStateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.State perfect;
		}
	}

	public enum TemperatureState
	{
		LethalCold,
		WarningCold,
		Normal,
		Perfect,
		WarningHot,
		LethalHot
	}
}
