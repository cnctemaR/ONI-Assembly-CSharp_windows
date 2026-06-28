using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
public class TemperatureVulnerable : StateMachineComponent<TemperatureVulnerable.StatesInstance>, IGameObjectEffectDescriptor, IWiltCause
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
			return this.GetInternalTemperatureState == TemperatureVulnerable.TemperatureState.Normal || this.GetInternalTemperatureState == TemperatureVulnerable.TemperatureState.Perfect;
		}
	}

	public bool IsPerfect
	{
		get
		{
			return this.GetInternalTemperatureState == TemperatureVulnerable.TemperatureState.Perfect;
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
			string text;
			if (base.smi.IsInsideState(base.smi.sm.warningCold))
			{
				text = Db.Get().CreatureStatusItems.Cold_Crop.resolveStringCallback(CREATURES.STATUSITEMS.COLD_CROP.NAME, this);
			}
			else if (base.smi.IsInsideState(base.smi.sm.warningHot))
			{
				text = Db.Get().CreatureStatusItems.Hot_Crop.resolveStringCallback(CREATURES.STATUSITEMS.HOT_CROP.NAME, this);
			}
			else
			{
				text = "";
			}
			return text;
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
		this.handle = GameScheduler.Instance.SchedulePeriodic("TemperatureVulnerable", 1f, new Action<object>(this.UpdateTemperature), null, null, 0f, null);
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		this.handle.ClearScheduler();
		base.OnCleanUp();
	}

	public void Configure(float tempWarningLow = 283f, float tempLethalLow = 263f, float tempWarningHigh = 294f, float tempLethalHigh = 343f, float tempPerfectLow = 0f, float tempPerfectHigh = 0f)
	{
		this.internalTemperatureWarning_Low = tempWarningLow;
		this.internalTemperatureLethal_Low = tempLethalLow;
		this.internalTemperatureLethal_High = tempLethalHigh;
		this.internalTemperatureWarning_High = tempWarningHigh;
		this.internalTemperaturePerfect_Low = tempPerfectLow;
		this.internalTemperaturePerfect_High = tempPerfectHigh;
	}

	public bool IsCellSafe(int cell)
	{
		float averageTemperature = this.GetAverageTemperature(cell);
		return averageTemperature > -1f && averageTemperature > this.internalTemperatureLethal_Low && averageTemperature < this.internalTemperatureLethal_High;
	}

	public void UpdateTemperature(object data)
	{
		int num = Grid.PosToCell(base.gameObject);
		if (Grid.IsValidCell(num))
		{
			base.smi.sm.internalTemp.Set(this.InternalTemperature, base.smi);
			this.displayTemperatureAmount.value = this.InternalTemperature;
		}
	}

	private float GetAverageTemperature(int cell)
	{
		float temperature = 0f;
		int count = 0;
		this.occupyArea.TestArea(cell, null, delegate(int testCell, object data)
		{
			if (Grid.Cell[testCell].mass > 0.1f)
			{
				temperature += Grid.Temperature[testCell];
				count++;
			}
			return true;
		});
		float num;
		if (count > 0)
		{
			num = temperature / (float)count;
		}
		else
		{
			num = -1f;
		}
		return num;
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
			}
		}

		public bool hasMaturity = false;
	}

	public class States : GameStateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.normal;
			this.lethalCold.TriggerOnEnter(GameHashes.TooColdFatal, null).ParamTransition<float>(this.internalTemp, this.warningCold, (TemperatureVulnerable.StatesInstance smi, float p) => p > smi.master.internalTemperatureLethal_Low).Enter(delegate(TemperatureVulnerable.StatesInstance smi)
			{
				smi.master.internalTemperatureState = TemperatureVulnerable.TemperatureState.LethalCold;
			});
			this.lethalHot.TriggerOnEnter(GameHashes.TooHotFatal, null).ParamTransition<float>(this.internalTemp, this.warningHot, (TemperatureVulnerable.StatesInstance smi, float p) => p < smi.master.internalTemperatureLethal_High).Enter(delegate(TemperatureVulnerable.StatesInstance smi)
			{
				smi.master.internalTemperatureState = TemperatureVulnerable.TemperatureState.LethalHot;
			});
			this.warningCold.TriggerOnEnter(GameHashes.TooColdWarning, null).ParamTransition<float>(this.internalTemp, this.lethalCold, (TemperatureVulnerable.StatesInstance smi, float p) => p < smi.master.internalTemperatureLethal_Low).ParamTransition<float>(this.internalTemp, this.normal, (TemperatureVulnerable.StatesInstance smi, float p) => p > smi.master.internalTemperatureWarning_Low)
				.Enter(delegate(TemperatureVulnerable.StatesInstance smi)
				{
					smi.master.internalTemperatureState = TemperatureVulnerable.TemperatureState.WarningCold;
				})
				.Exit(delegate(TemperatureVulnerable.StatesInstance smi)
				{
				});
			this.warningHot.TriggerOnEnter(GameHashes.TooHotWarning, null).ParamTransition<float>(this.internalTemp, this.lethalHot, (TemperatureVulnerable.StatesInstance smi, float p) => p > smi.master.internalTemperatureLethal_High).ParamTransition<float>(this.internalTemp, this.normal, (TemperatureVulnerable.StatesInstance smi, float p) => p < smi.master.internalTemperatureWarning_High)
				.Enter(delegate(TemperatureVulnerable.StatesInstance smi)
				{
					smi.master.internalTemperatureState = TemperatureVulnerable.TemperatureState.WarningHot;
				})
				.Exit(delegate(TemperatureVulnerable.StatesInstance smi)
				{
				});
			this.normal.DefaultState(this.normal.okay).TriggerOnEnter(GameHashes.OptimalTemperatureAchieved, null).ParamTransition<float>(this.internalTemp, this.warningHot, (TemperatureVulnerable.StatesInstance smi, float p) => p > smi.master.internalTemperatureWarning_High)
				.ParamTransition<float>(this.internalTemp, this.warningCold, (TemperatureVulnerable.StatesInstance smi, float p) => p < smi.master.internalTemperatureWarning_Low);
			this.normal.okay.ParamTransition<float>(this.internalTemp, this.normal.perfect, (TemperatureVulnerable.StatesInstance smi, float p) => p > smi.master.internalTemperaturePerfect_Low && p < smi.master.internalTemperaturePerfect_High).Enter(delegate(TemperatureVulnerable.StatesInstance smi)
			{
				smi.master.internalTemperatureState = TemperatureVulnerable.TemperatureState.Normal;
			});
			this.normal.perfect.ParamTransition<float>(this.internalTemp, this.normal.okay, (TemperatureVulnerable.StatesInstance smi, float p) => p < smi.master.internalTemperaturePerfect_Low || p > smi.master.internalTemperaturePerfect_High).Enter(delegate(TemperatureVulnerable.StatesInstance smi)
			{
				smi.master.internalTemperatureState = TemperatureVulnerable.TemperatureState.Perfect;
			});
		}

		public StateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.FloatParameter internalTemp;

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
