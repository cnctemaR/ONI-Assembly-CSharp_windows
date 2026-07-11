using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicGateFilter : LogicGate, ISingleSliderControl, ISliderControl
{
	public float DelayAmount
	{
		get
		{
			return this.delayAmount;
		}
		set
		{
			this.delayAmount = value;
			if (this.schedulerHandle.IsValid && this.schedulerHandle.TimeRemaining > this.delayAmount)
			{
				this.schedulerHandle.ClearScheduler();
				this.schedulerHandle = GameScheduler.Instance.Schedule("logic delay", this.delayAmount, new Action<object>(this.OnDelay), null, null);
			}
		}
	}

	public string SliderTitleKey
	{
		get
		{
			return "STRINGS.UI.UISIDESCREENS.LOGIC_DELAY_SIDE_SCREEN.TITLE";
		}
	}

	public string SliderUnits
	{
		get
		{
			return UI.UNITSUFFIXES.SECOND;
		}
	}

	public float GetSliderMin(int index)
	{
		return 0.1f;
	}

	public float GetSliderMax(int index)
	{
		return 200f;
	}

	public float GetSliderValue(int index)
	{
		return this.delayAmount;
	}

	public void SetSliderValue(float value, int index)
	{
		this.delayAmount = value;
	}

	public string GetSliderTooltipKey(int index)
	{
		return "STRINGS.UI.UISIDESCREENS.LOGIC_DELAY_SIDE_SCREEN.TOOLTIP";
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		this.meter = new MeterController(component, "meter_target", "meter", Meter.Offset.UserSpecified, Grid.SceneLayer.WireBridgesFront, Vector3.zero, null);
		this.meter.SetPositionPercent(0f);
	}

	private void Update()
	{
		if (this.schedulerHandle.IsValid)
		{
			float timeRemaining = this.schedulerHandle.TimeRemaining;
			this.meter.SetPositionPercent(timeRemaining / this.delayAmount);
		}
	}

	protected override int GetCustomValue(int val1, int val2)
	{
		if (val1 == 0)
		{
			this.input_was_previously_negative = true;
			if (this.schedulerHandle.IsValid)
			{
				this.schedulerHandle.ClearScheduler();
			}
			this.meter.SetPositionPercent(1f);
		}
		else if (!this.schedulerHandle.IsValid)
		{
			if (this.input_was_previously_negative)
			{
				this.schedulerHandle = GameScheduler.Instance.Schedule("logic delay", this.delayAmount, new Action<object>(this.OnDelay), null, null);
			}
			this.input_was_previously_negative = false;
		}
		return (val1 != 0 && this.schedulerHandle.TimeRemaining <= 0f) ? 1 : 0;
	}

	private void OnDelay(object data)
	{
		if (this.cleaningUp)
		{
			return;
		}
		this.meter.SetPositionPercent(0f);
		if (this.outputValue == 1)
		{
			return;
		}
		int outputCell = base.OutputCell;
		if (!(Game.Instance.logicCircuitSystem.GetNetworkForCell(outputCell) is LogicCircuitNetwork))
		{
			return;
		}
		this.outputValue = 1;
		base.RefreshAnimation();
	}

	private SchedulerHandle schedulerHandle;

	[Serialize]
	private bool input_was_previously_negative;

	[Serialize]
	private float delayAmount = 5f;

	private MeterController meter;
}
