using System;
using KSerialization;
using STRINGS;

[SerializationConfig(MemberSerialization.OptIn)]
public class TemperatureControlledSwitch : CircuitSwitch, ISaveLoadable, IThresholdSwitch
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
	}

	private void SimUpdate(float dt)
	{
		if (this.simUpdateCounter < 8)
		{
			this.temperatures[this.simUpdateCounter] = Grid.Temperature[Grid.PosToCell(this)];
			this.simUpdateCounter++;
			return;
		}
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

	public float GetTemperature()
	{
		return this.averageTemp;
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

	public string Format(float value)
	{
		return GameUtil.GetFormattedTemperature(value, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true);
	}

	virtual bool IThresholdSwitch.IsConnected()
	{
		return base.IsConnected();
	}

	private const int NumFrameDelay = 8;

	private HandleVector<int>.Handle structureTemperature;

	private int simUpdateCounter;

	[Serialize]
	public float thresholdTemperature = 280f;

	[Serialize]
	public bool activateOnWarmerThan;

	public float minTemp;

	public float maxTemp = 373.15f;

	private float[] temperatures = new float[8];

	private float averageTemp;
}
