using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicPressureSensor : Switch, ISaveLoadable, IThresholdSwitch, ISim200ms
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<LogicPressureSensor>(-905833192, LogicPressureSensor.OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		LogicPressureSensor component = gameObject.GetComponent<LogicPressureSensor>();
		if (component != null)
		{
			this.Threshold = component.Threshold;
			this.ActivateAboveThreshold = component.ActivateAboveThreshold;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.OnToggle += this.OnSwitchToggled;
		this.UpdateLogicCircuit();
		this.UpdateVisualState(true);
		this.wasOn = this.switchedOn;
	}

	public void Sim200ms(float dt)
	{
		int num = Grid.PosToCell(this);
		if (this.sampleIdx < 8)
		{
			float num2 = ((!Grid.Element[num].IsState(this.desiredState)) ? 0f : Grid.Mass[num]);
			this.samples[this.sampleIdx] = num2;
			this.sampleIdx++;
			return;
		}
		this.sampleIdx = 0;
		float currentValue = this.CurrentValue;
		if (this.activateAboveThreshold)
		{
			if ((currentValue > this.threshold && !base.IsSwitchedOn) || (currentValue <= this.threshold && base.IsSwitchedOn))
			{
				this.Toggle();
			}
		}
		else if ((currentValue > this.threshold && base.IsSwitchedOn) || (currentValue <= this.threshold && !base.IsSwitchedOn))
		{
			this.Toggle();
		}
	}

	private void OnSwitchToggled(bool toggled_on)
	{
		this.UpdateLogicCircuit();
		this.UpdateVisualState(false);
	}

	public float Threshold
	{
		get
		{
			return this.threshold;
		}
		set
		{
			this.threshold = value;
		}
	}

	public bool ActivateAboveThreshold
	{
		get
		{
			return this.activateAboveThreshold;
		}
		set
		{
			this.activateAboveThreshold = value;
		}
	}

	public float CurrentValue
	{
		get
		{
			float num = 0f;
			for (int i = 0; i < 8; i++)
			{
				num += this.samples[i];
			}
			return num / 8f;
		}
	}

	public float RangeMin
	{
		get
		{
			return this.rangeMin;
		}
	}

	public float RangeMax
	{
		get
		{
			return this.rangeMax;
		}
	}

	public float GetRangeMinInputField()
	{
		return (this.desiredState != Element.State.Gas) ? this.rangeMin : (this.rangeMin * 1000f);
	}

	public float GetRangeMaxInputField()
	{
		return (this.desiredState != Element.State.Gas) ? this.rangeMax : (this.rangeMax * 1000f);
	}

	public LocString ThresholdValueName
	{
		get
		{
			return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.PRESSURE;
		}
	}

	public string AboveToolTip
	{
		get
		{
			return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.PRESSURE_TOOLTIP_ABOVE;
		}
	}

	public string BelowToolTip
	{
		get
		{
			return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.PRESSURE_TOOLTIP_BELOW;
		}
	}

	public string Format(float value, bool units)
	{
		GameUtil.MetricMassFormat metricMassFormat;
		if (this.desiredState == Element.State.Gas)
		{
			metricMassFormat = GameUtil.MetricMassFormat.Gram;
		}
		else
		{
			metricMassFormat = GameUtil.MetricMassFormat.Kilogram;
		}
		return GameUtil.GetFormattedMass(value, GameUtil.TimeSlice.None, metricMassFormat, units, "{0:0.#}");
	}

	public float ProcessedSliderValue(float input)
	{
		if (this.desiredState == Element.State.Gas)
		{
			input = Mathf.Round(input * 1000f) / 1000f;
		}
		else
		{
			input = Mathf.Round(input);
		}
		return input;
	}

	public float ProcessedInputValue(float input)
	{
		if (this.desiredState == Element.State.Gas)
		{
			input /= 1000f;
		}
		return input;
	}

	public LocString ThresholdValueUnits()
	{
		return GameUtil.GetCurrentMassUnit(this.desiredState == Element.State.Gas);
	}

	public LocString Title
	{
		get
		{
			return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TITLE;
		}
	}

	public ThresholdScreenLayoutType LayoutType
	{
		get
		{
			return ThresholdScreenLayoutType.SliderBar;
		}
	}

	public int IncrementScale
	{
		get
		{
			return 1;
		}
	}

	public NonLinearSlider.Range[] GetRanges
	{
		get
		{
			return NonLinearSlider.GetDefaultRange(this.RangeMax);
		}
	}

	private void UpdateLogicCircuit()
	{
		base.GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, (!this.switchedOn) ? 0 : 1);
	}

	private void UpdateVisualState(bool force = false)
	{
		if (this.wasOn != this.switchedOn || force)
		{
			this.wasOn = this.switchedOn;
			KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
			component.Play((!this.switchedOn) ? "on_pst" : "on_pre", KAnim.PlayMode.Once, 1f, 0f);
			component.Queue((!this.switchedOn) ? "off" : "on", KAnim.PlayMode.Once, 1f, 0f);
		}
	}

	[SerializeField]
	[Serialize]
	private float threshold;

	[SerializeField]
	[Serialize]
	private bool activateAboveThreshold = true;

	private bool wasOn;

	public float rangeMin;

	public float rangeMax = 1f;

	public Element.State desiredState = Element.State.Gas;

	private const int WINDOW_SIZE = 8;

	private float[] samples = new float[8];

	private int sampleIdx;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<LogicPressureSensor> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicPressureSensor>(delegate(LogicPressureSensor component, object data)
	{
		component.OnCopySettings(data);
	});
}
