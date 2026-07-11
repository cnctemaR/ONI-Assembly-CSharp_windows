using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ConduitTemperatureSensor : ConduitThresholdSensor, IThresholdSwitch
{
	public override float CurrentValue
	{
		get
		{
			int num = Grid.PosToCell(this);
			ConduitFlow flowManager = Conduit.GetFlowManager(this.conduitType);
			ConduitFlow.ConduitContents contents = flowManager.GetContents(num);
			if (contents.mass > 0f)
			{
				this.lastValue = contents.temperature;
			}
			return this.lastValue;
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
		return GameUtil.GetConvertedTemperature(this.RangeMin, false);
	}

	public float GetRangeMaxInputField()
	{
		return GameUtil.GetConvertedTemperature(this.RangeMax, false);
	}

	public LocString Title
	{
		get
		{
			return UI.UISIDESCREENS.TEMPERATURESWITCHSIDESCREEN.TITLE;
		}
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
		return GameUtil.GetFormattedTemperature(value, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, units, false);
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
			return new NonLinearSlider.Range[]
			{
				new NonLinearSlider.Range(25f, 260f),
				new NonLinearSlider.Range(50f, 400f),
				new NonLinearSlider.Range(12f, 1500f),
				new NonLinearSlider.Range(13f, 10000f)
			};
		}
	}

	public float rangeMin;

	public float rangeMax = 373.15f;

	[Serialize]
	private float lastValue;
}
