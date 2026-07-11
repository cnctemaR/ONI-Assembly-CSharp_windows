using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
public class PressureVulnerable : StateMachineComponent<PressureVulnerable.StatesInstance>, IGameObjectEffectDescriptor, IWiltCause, ISim1000ms
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

	public PressureVulnerable.PressureState GetExternalPressureState
	{
		get
		{
			return this.pressureState;
		}
	}

	public float GetExternalPressure
	{
		get
		{
			return this.GetPressureOverArea(this.cell);
		}
	}

	public Element GetExternalElement
	{
		get
		{
			return Grid.Element[this.cell];
		}
	}

	public bool IsLethal
	{
		get
		{
			return this.GetExternalPressureState == PressureVulnerable.PressureState.LethalHigh || this.GetExternalPressureState == PressureVulnerable.PressureState.LethalLow || !this.IsSafeElement(this.GetExternalElement);
		}
	}

	public bool IsNormal
	{
		get
		{
			return this.IsSafeElement(this.GetExternalElement) && this.GetExternalPressureState == PressureVulnerable.PressureState.Normal;
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
		this.cell = Grid.PosToCell(this);
		base.smi.sm.pressure.Set(1f, base.smi);
		base.smi.sm.safe_element.Set(this.IsSafeElement(this.GetExternalElement), base.smi);
		base.smi.StartSM();
	}

	public void Configure(SimHashes[] safeAtmospheres = null)
	{
		this.pressure_sensitive = false;
		this.pressureWarning_Low = float.MinValue;
		this.pressureLethal_Low = float.MinValue;
		this.pressureLethal_High = float.MaxValue;
		this.pressureWarning_High = float.MaxValue;
		this.safe_atmospheres = new HashSet<Element>();
		if (safeAtmospheres != null)
		{
			foreach (SimHashes simHashes in safeAtmospheres)
			{
				this.safe_atmospheres.Add(ElementLoader.FindElementByHash(simHashes));
			}
		}
	}

	public void Configure(float pressureWarningLow = 0.25f, float pressureLethalLow = 0.01f, float pressureWarningHigh = 10f, float pressureLethalHigh = 30f, SimHashes[] safeAtmospheres = null)
	{
		this.pressure_sensitive = true;
		this.pressureWarning_Low = pressureWarningLow;
		this.pressureLethal_Low = pressureLethalLow;
		this.pressureLethal_High = pressureLethalHigh;
		this.pressureWarning_High = pressureWarningHigh;
		this.safe_atmospheres = new HashSet<Element>();
		if (safeAtmospheres != null)
		{
			foreach (SimHashes simHashes in safeAtmospheres)
			{
				this.safe_atmospheres.Add(ElementLoader.FindElementByHash(simHashes));
			}
		}
	}

	WiltCondition.Condition[] IWiltCause.Conditions
	{
		get
		{
			return new WiltCondition.Condition[]
			{
				WiltCondition.Condition.Pressure,
				WiltCondition.Condition.AtmosphereElement
			};
		}
	}

	public string WiltStateString
	{
		get
		{
			string text = string.Empty;
			if (base.smi.IsInsideState(base.smi.sm.warningLow) || base.smi.IsInsideState(base.smi.sm.lethalLow))
			{
				text += Db.Get().CreatureStatusItems.AtmosphericPressureTooLow.resolveStringCallback(CREATURES.STATUSITEMS.ATMOSPHERICPRESSURETOOLOW.NAME, this);
			}
			else if (base.smi.IsInsideState(base.smi.sm.warningHigh) || base.smi.IsInsideState(base.smi.sm.lethalHigh))
			{
				text += Db.Get().CreatureStatusItems.AtmosphericPressureTooHigh.resolveStringCallback(CREATURES.STATUSITEMS.ATMOSPHERICPRESSURETOOHIGH.NAME, this);
			}
			else if (base.smi.IsInsideState(base.smi.sm.unsafeElement))
			{
				text += Db.Get().CreatureStatusItems.WrongAtmosphere.resolveStringCallback(CREATURES.STATUSITEMS.WRONGATMOSPHERE.NAME, this);
			}
			return text;
		}
	}

	public bool IsCellSafe(int cell)
	{
		return this.IsSafeElement(this.GetExternalElement) && this.IsSafePressure(this.GetPressureOverArea(cell));
	}

	public bool IsSafeElement(Element element)
	{
		return this.safe_atmospheres == null || this.safe_atmospheres.Count == 0 || this.safe_atmospheres.Contains(element);
	}

	public bool IsSafePressure(float pressure)
	{
		return !this.pressure_sensitive || (pressure > this.pressureLethal_Low && pressure < this.pressureLethal_High);
	}

	public void Sim1000ms(float dt)
	{
		float pressureOverArea = this.GetPressureOverArea(this.cell);
		base.smi.sm.pressure.Set(pressureOverArea, base.smi);
		this.displayPressureAmount.value = pressureOverArea;
		base.smi.sm.safe_element.Set(this.IsSafeElement(this.GetExternalElement), base.smi);
	}

	private float GetPressureOverArea(int cell)
	{
		PressureVulnerable.testAreaPressure = 0f;
		PressureVulnerable.testAreaCount = 0;
		this.occupyArea.TestArea(cell, null, PressureVulnerable.testAreaCB);
		this.occupyArea.TestAreaAbove(cell, null, PressureVulnerable.testAreaCB);
		PressureVulnerable.testAreaPressure = ((PressureVulnerable.testAreaCount <= 0) ? 0f : (PressureVulnerable.testAreaPressure / (float)PressureVulnerable.testAreaCount));
		return PressureVulnerable.testAreaPressure;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (this.pressure_sensitive)
		{
			list.Add(new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.REQUIRES_PRESSURE, GameUtil.GetFormattedMass(this.pressureWarning_Low, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_PRESSURE, GameUtil.GetFormattedMass(this.pressureWarning_Low, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), Descriptor.DescriptorType.Requirement, false));
		}
		if (this.safe_atmospheres != null && this.safe_atmospheres.Count > 0)
		{
			string text = string.Empty;
			foreach (Element element in this.safe_atmospheres)
			{
				text = text + "\n        • " + element.name;
			}
			list.Add(new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.REQUIRES_ATMOSPHERE, text), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_ATMOSPHERE, text), Descriptor.DescriptorType.Requirement, false));
		}
		return list;
	}

	private OccupyArea _occupyArea;

	public float pressureLethal_Low;

	public float pressureWarning_Low;

	public float pressureWarning_High;

	public float pressureLethal_High;

	private static float testAreaPressure;

	private static int testAreaCount;

	private static Func<int, object, bool> testAreaCB = delegate(int test_cell, object data)
	{
		if (Grid.IsGas(test_cell))
		{
			PressureVulnerable.testAreaPressure += Grid.Mass[test_cell];
			PressureVulnerable.testAreaCount++;
		}
		return true;
	};

	private AmountInstance displayPressureAmount;

	public bool pressure_sensitive = true;

	public HashSet<Element> safe_atmospheres = new HashSet<Element>();

	private int cell;

	private PressureVulnerable.PressureState pressureState = PressureVulnerable.PressureState.Normal;

	public class StatesInstance : GameStateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.GameInstance
	{
		public StatesInstance(PressureVulnerable master)
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

	public class States : GameStateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.normal;
			this.lethalLow.Enter(delegate(PressureVulnerable.StatesInstance smi)
			{
				smi.master.pressureState = PressureVulnerable.PressureState.LethalLow;
			}).TriggerOnEnter(GameHashes.LowPressureFatal, null).ParamTransition<float>(this.pressure, this.warningLow, (PressureVulnerable.StatesInstance smi, float p) => p > smi.master.pressureLethal_Low)
				.ParamTransition<bool>(this.safe_element, this.unsafeElement, (PressureVulnerable.StatesInstance smi, bool p) => !p);
			this.lethalHigh.Enter(delegate(PressureVulnerable.StatesInstance smi)
			{
				smi.master.pressureState = PressureVulnerable.PressureState.LethalHigh;
			}).TriggerOnEnter(GameHashes.HighPressureFatal, null).ParamTransition<float>(this.pressure, this.warningHigh, (PressureVulnerable.StatesInstance smi, float p) => p < smi.master.pressureLethal_High)
				.ParamTransition<bool>(this.safe_element, this.unsafeElement, (PressureVulnerable.StatesInstance smi, bool p) => !p);
			this.warningLow.Enter(delegate(PressureVulnerable.StatesInstance smi)
			{
				smi.master.pressureState = PressureVulnerable.PressureState.WarningLow;
			}).TriggerOnEnter(GameHashes.LowPressureWarning, null).ParamTransition<float>(this.pressure, this.lethalLow, (PressureVulnerable.StatesInstance smi, float p) => p < smi.master.pressureLethal_Low)
				.ParamTransition<float>(this.pressure, this.normal, (PressureVulnerable.StatesInstance smi, float p) => p > smi.master.pressureWarning_Low)
				.ParamTransition<bool>(this.safe_element, this.unsafeElement, (PressureVulnerable.StatesInstance smi, bool p) => !p);
			this.unsafeElement.ParamTransition<bool>(this.safe_element, this.normal, (PressureVulnerable.StatesInstance smi, bool p) => p).TriggerOnExit(GameHashes.CorrectAtmosphere).TriggerOnEnter(GameHashes.WrongAtmosphere, null);
			this.warningHigh.Enter(delegate(PressureVulnerable.StatesInstance smi)
			{
				smi.master.pressureState = PressureVulnerable.PressureState.WarningHigh;
			}).TriggerOnEnter(GameHashes.HighPressureWarning, null).ParamTransition<float>(this.pressure, this.lethalHigh, (PressureVulnerable.StatesInstance smi, float p) => p > smi.master.pressureLethal_High)
				.ParamTransition<float>(this.pressure, this.normal, (PressureVulnerable.StatesInstance smi, float p) => p < smi.master.pressureWarning_High)
				.ParamTransition<bool>(this.safe_element, this.unsafeElement, (PressureVulnerable.StatesInstance smi, bool p) => !p);
			this.normal.Enter(delegate(PressureVulnerable.StatesInstance smi)
			{
				smi.master.pressureState = PressureVulnerable.PressureState.Normal;
			}).TriggerOnEnter(GameHashes.OptimalPressureAchieved, null).ParamTransition<float>(this.pressure, this.warningHigh, (PressureVulnerable.StatesInstance smi, float p) => p > smi.master.pressureWarning_High)
				.ParamTransition<float>(this.pressure, this.warningLow, (PressureVulnerable.StatesInstance smi, float p) => p < smi.master.pressureWarning_Low)
				.ParamTransition<bool>(this.safe_element, this.unsafeElement, (PressureVulnerable.StatesInstance smi, bool p) => !p);
		}

		public StateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.FloatParameter pressure;

		public StateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.BoolParameter safe_element;

		public GameStateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.State unsafeElement;

		public GameStateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.State lethalLow;

		public GameStateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.State lethalHigh;

		public GameStateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.State warningLow;

		public GameStateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.State warningHigh;

		public GameStateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.State normal;
	}

	public enum PressureState
	{
		LethalLow,
		WarningLow,
		Normal,
		WarningHigh,
		LethalHigh
	}
}
