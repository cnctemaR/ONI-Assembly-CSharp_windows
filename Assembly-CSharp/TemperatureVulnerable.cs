using System;
using System.Collections.Generic;
using System.Diagnostics;
using Klei.AI;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
public class TemperatureVulnerable : StateMachineComponent<TemperatureVulnerable.StatesInstance>, IGameObjectEffectDescriptor, IWiltCause, ISim1000ms
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

	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<float, float> OnTemperature;

	public float InternalTemperature
	{
		get
		{
			return this.primaryElement.Temperature;
		}
	}

	public TemperatureVulnerable.TemperatureState GetInternalTemperatureState
	{
		get
		{
			return this.internalTemperatureState;
		}
	}

	public bool IsLethal
	{
		get
		{
			return this.GetInternalTemperatureState == TemperatureVulnerable.TemperatureState.LethalHot || this.GetInternalTemperatureState == TemperatureVulnerable.TemperatureState.LethalCold;
		}
	}

	public bool IsNormal
	{
		get
		{
			return this.GetInternalTemperatureState == TemperatureVulnerable.TemperatureState.Normal;
		}
	}

	WiltCondition.Condition[] IWiltCause.Conditions
	{
		get
		{
			return new WiltCondition.Condition[1];
		}
	}

	public string WiltStateString
	{
		get
		{
			if (base.smi.IsInsideState(base.smi.sm.warningCold))
			{
				return Db.Get().CreatureStatusItems.Cold_Crop.resolveStringCallback(CREATURES.STATUSITEMS.COLD_CROP.NAME, this);
			}
			if (base.smi.IsInsideState(base.smi.sm.warningHot))
			{
				return Db.Get().CreatureStatusItems.Hot_Crop.resolveStringCallback(CREATURES.STATUSITEMS.HOT_CROP.NAME, this);
			}
			return string.Empty;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Amounts amounts = base.gameObject.GetAmounts();
		this.displayTemperatureAmount = amounts.Add(new AmountInstance(Db.Get().Amounts.Temperature, base.gameObject));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.sm.internalTemp.Set(this.primaryElement.Temperature, base.smi);
		base.smi.StartSM();
	}

	public void Configure(float tempWarningLow, float tempLethalLow, float tempWarningHigh, float tempLethalHigh)
	{
		this.internalTemperatureWarning_Low = tempWarningLow;
		this.internalTemperatureLethal_Low = tempLethalLow;
		this.internalTemperatureLethal_High = tempLethalHigh;
		this.internalTemperatureWarning_High = tempWarningHigh;
	}

	public bool IsCellSafe(int cell)
	{
		float averageTemperature = this.GetAverageTemperature(cell);
		return averageTemperature > -1f && averageTemperature > this.internalTemperatureLethal_Low && averageTemperature < this.internalTemperatureLethal_High;
	}

	public void Sim1000ms(float dt)
	{
		int num = Grid.PosToCell(base.gameObject);
		if (!Grid.IsValidCell(num))
		{
			return;
		}
		base.smi.sm.internalTemp.Set(this.InternalTemperature, base.smi);
		this.displayTemperatureAmount.value = this.InternalTemperature;
		if (this.OnTemperature != null)
		{
			this.OnTemperature(dt, this.InternalTemperature);
		}
	}

	private static bool GetAverageTemperatureCb(int cell, object data)
	{
		TemperatureVulnerable temperatureVulnerable = data as TemperatureVulnerable;
		if (Grid.Mass[cell] > 0.1f)
		{
			temperatureVulnerable.averageTemp += Grid.Temperature[cell];
			temperatureVulnerable.cellCount++;
		}
		return true;
	}

	private float GetAverageTemperature(int cell)
	{
		this.averageTemp = 0f;
		this.cellCount = 0;
		this.occupyArea.TestArea(cell, this, new Func<int, object, bool>(TemperatureVulnerable.GetAverageTemperatureCb));
		if (this.cellCount > 0)
		{
			return this.averageTemp / (float)this.cellCount;
		}
		return -1f;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return new List<Descriptor>
		{
			new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.REQUIRES_TEMPERATURE, GameUtil.GetFormattedTemperature(this.internalTemperatureWarning_Low, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, false), GameUtil.GetFormattedTemperature(this.internalTemperatureWarning_High, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true)), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_TEMPERATURE, GameUtil.GetFormattedTemperature(this.internalTemperatureWarning_Low, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, false), GameUtil.GetFormattedTemperature(this.internalTemperatureWarning_High, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true)), Descriptor.DescriptorType.Requirement, false)
		};
	}

	private OccupyArea _occupyArea;

	public float internalTemperatureLethal_Low;

	public float internalTemperatureWarning_Low;

	public float internalTemperaturePerfect_Low;

	public float internalTemperaturePerfect_High;

	public float internalTemperatureWarning_High;

	public float internalTemperatureLethal_High;

	private const float minimumMassForReading = 0.1f;

	[MyCmpReq]
	private PrimaryElement primaryElement;

	[MyCmpReq]
	private SimTemperatureTransfer temperatureTransfer;

	private AmountInstance displayTemperatureAmount;

	private TemperatureVulnerable.TemperatureState internalTemperatureState = TemperatureVulnerable.TemperatureState.Normal;

	private float averageTemp;

	private int cellCount;

	public class StatesInstance : GameStateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.GameInstance
	{
		public StatesInstance(TemperatureVulnerable master)
			: base(master)
		{
			AmountInstance amountInstance = Db.Get().Amounts.Maturity.Lookup(base.gameObject);
			if (amountInstance != null)
			{
				this.hasMaturity = true;
			}
		}

		public bool hasMaturity;
	}

	public class States : GameStateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.normal;
			this.lethalCold.Enter(delegate(TemperatureVulnerable.StatesInstance smi)
			{
				smi.master.internalTemperatureState = TemperatureVulnerable.TemperatureState.LethalCold;
			}).TriggerOnEnter(GameHashes.TooColdFatal, null).ParamTransition<float>(this.internalTemp, this.warningCold, (TemperatureVulnerable.StatesInstance smi, float p) => p > smi.master.internalTemperatureLethal_Low)
				.Enter(new StateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.State.Callback(TemperatureVulnerable.States.Kill));
			this.lethalHot.Enter(delegate(TemperatureVulnerable.StatesInstance smi)
			{
				smi.master.internalTemperatureState = TemperatureVulnerable.TemperatureState.LethalHot;
			}).TriggerOnEnter(GameHashes.TooHotFatal, null).ParamTransition<float>(this.internalTemp, this.warningHot, (TemperatureVulnerable.StatesInstance smi, float p) => p < smi.master.internalTemperatureLethal_High)
				.Enter(new StateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.State.Callback(TemperatureVulnerable.States.Kill));
			this.warningCold.Enter(delegate(TemperatureVulnerable.StatesInstance smi)
			{
				smi.master.internalTemperatureState = TemperatureVulnerable.TemperatureState.WarningCold;
			}).TriggerOnEnter(GameHashes.TooColdWarning, null).ParamTransition<float>(this.internalTemp, this.lethalCold, (TemperatureVulnerable.StatesInstance smi, float p) => p < smi.master.internalTemperatureLethal_Low)
				.ParamTransition<float>(this.internalTemp, this.normal, (TemperatureVulnerable.StatesInstance smi, float p) => p > smi.master.internalTemperatureWarning_Low);
			this.warningHot.Enter(delegate(TemperatureVulnerable.StatesInstance smi)
			{
				smi.master.internalTemperatureState = TemperatureVulnerable.TemperatureState.WarningHot;
			}).TriggerOnEnter(GameHashes.TooHotWarning, null).ParamTransition<float>(this.internalTemp, this.lethalHot, (TemperatureVulnerable.StatesInstance smi, float p) => p > smi.master.internalTemperatureLethal_High)
				.ParamTransition<float>(this.internalTemp, this.normal, (TemperatureVulnerable.StatesInstance smi, float p) => p < smi.master.internalTemperatureWarning_High);
			this.normal.Enter(delegate(TemperatureVulnerable.StatesInstance smi)
			{
				smi.master.internalTemperatureState = TemperatureVulnerable.TemperatureState.Normal;
			}).TriggerOnEnter(GameHashes.OptimalTemperatureAchieved, null).ParamTransition<float>(this.internalTemp, this.warningHot, (TemperatureVulnerable.StatesInstance smi, float p) => p > smi.master.internalTemperatureWarning_High)
				.ParamTransition<float>(this.internalTemp, this.warningCold, (TemperatureVulnerable.StatesInstance smi, float p) => p < smi.master.internalTemperatureWarning_Low);
		}

		private static void Kill(StateMachine.Instance smi)
		{
			DeathMonitor.Instance smi2 = smi.GetSMI<DeathMonitor.Instance>();
			if (smi2 != null)
			{
				smi2.Kill(Db.Get().Deaths.Generic);
			}
		}

		public StateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.FloatParameter internalTemp;

		public GameStateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.State lethalCold;

		public GameStateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.State lethalHot;

		public GameStateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.State warningCold;

		public GameStateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.State warningHot;

		public GameStateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.State normal;
	}

	public enum TemperatureState
	{
		LethalCold,
		WarningCold,
		Normal,
		WarningHot,
		LethalHot
	}
}
