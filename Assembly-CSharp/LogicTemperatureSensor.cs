using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicTemperatureSensor : Switch, ISaveLoadable, IThresholdSwitch
{
	public float StructureTemperature
	{
		get
		{
			return GameComps.StructureTemperatures.GetData(this.structureTemperature).Temperature;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.structureTemperature = GameComps.StructureTemperatures.GetHandle(base.gameObject);
		base.OnToggle += this.OnSwitchToggled;
		this.UpdateVisualState(true);
		this.wasOn = this.switchedOn;
	}

	private void SimUpdate(float dt)
	{
		if (this.simUpdateCounter < 8)
		{
			this.temperatures[this.simUpdateCounter] = Grid.Temperature[Grid.PosToCell(this)];
			this.simUpdateCounter++;
		}
		else
		{
			this.simUpdateCounter = 0;
			this.averageTemp = 0f;
			for (int i = 0; i < 8; i++)
			{
				this.averageTemp += this.temperatures[i];
			}
			this.averageTemp /= 8f;
			if (this.activateOnWarmerThan)
			{
				if ((this.averageTemp > this.thresholdTemperature && !base.IsSwitchedOn) || (this.averageTemp < this.thresholdTemperature && base.IsSwitchedOn))
				{
					this.Toggle();
				}
			}
			else if ((this.averageTemp > this.thresholdTemperature && base.IsSwitchedOn) || (this.averageTemp < this.thresholdTemperature && !base.IsSwitchedOn))
			{
				this.Toggle();
			}
		}
	}

	public float GetTemperature()
	{
		return this.averageTemp;
	}

	private void OnSwitchToggled(bool toggled_on)
	{
		this.UpdateVisualState(false);
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

	public float Threshold
	{
		get
		{
			return this.thresholdTemperature;
		}
		set
		{
			this.thresholdTemperature = value;
		}
	}

	public bool ActivateAboveThreshold
	{
		get
		{
			return this.activateOnWarmerThan;
		}
		set
		{
			this.activateOnWarmerThan = value;
		}
	}

	public float CurrentValue
	{
		get
		{
			return this.GetTemperature();
		}
	}

	public float RangeMin
	{
		get
		{
			return this.minTemp;
		}
	}

	public float RangeMax
	{
		get
		{
			return this.maxTemp;
		}
	}

	public float GetRangeMinInputField()
	{
		return GameUtil.GetConvertedTemperature(this.RangeMin);
	}

	public float GetRangeMaxInputField()
	{
		return GameUtil.GetConvertedTemperature(this.RangeMax);
	}

	public LocString ThresholdValueName
	{
		get
		{
			return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TEMPERATURE;
		}
	}

	public string AboveToolTip
	{
		get
		{
			return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TEMPERATURE_TOOLTIP_ABOVE;
		}
	}

	public string BelowToolTip
	{
		get
		{
			return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TEMPERATURE_TOOLTIP_BELOW;
		}
	}

	public string Format(float value, bool units)
	{
		return GameUtil.GetFormattedTemperature(value, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, units);
	}

	public float ProcessedSliderValue(float input)
	{
		return Mathf.Round(input);
	}

	public float ProcessedInputValue(float input)
	{
		return GameUtil.GetTemperatureConvertedToKelvin(input);
	}

	public LocString ThresholdValueUnits()
	{
		LocString locString = null;
		GameUtil.TemperatureUnit temperatureUnit = GameUtil.temperatureUnit;
		if (temperatureUnit != GameUtil.TemperatureUnit.Celsius)
		{
			if (temperatureUnit != GameUtil.TemperatureUnit.Fahrenheit)
			{
				if (temperatureUnit == GameUtil.TemperatureUnit.Kelvin)
				{
					locString = UI.UNITSUFFIXES.TEMPERATURE.KELVIN;
				}
			}
			else
			{
				locString = UI.UNITSUFFIXES.TEMPERATURE.FAHRENHEIT;
			}
		}
		else
		{
			locString = UI.UNITSUFFIXES.TEMPERATURE.CELSIUS;
		}
		return locString;
	}

	private HandleVector<int>.Handle structureTemperature;

	private int simUpdateCounter = 0;

	[Serialize]
	public float thresholdTemperature = 280f;

	[Serialize]
	public bool activateOnWarmerThan;

	public float minTemp = 0f;

	public float maxTemp = 373.15f;

	private const int NumFrameDelay = 8;

	private float[] temperatures = new float[8];

	private float averageTemp;

	private bool wasOn = false;
}
