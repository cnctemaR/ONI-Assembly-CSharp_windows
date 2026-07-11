using System;
using STRINGS;
using UnityEngine;

public class ThresholdSwitchSideScreen : SideScreenContent, IRender200ms
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.aboveToggle.onClick += delegate
		{
			this.OnConditionButtonClicked(true);
		};
		this.belowToggle.onClick += delegate
		{
			this.OnConditionButtonClicked(false);
		};
		LocText component = this.aboveToggle.transform.GetChild(0).GetComponent<LocText>();
		LocText component2 = this.belowToggle.transform.GetChild(0).GetComponent<LocText>();
		component.SetText(UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.ABOVE_BUTTON);
		component2.SetText(UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.BELOW_BUTTON);
		this.thresholdSlider.onDrag += delegate
		{
			this.ReceiveValueFromSlider(this.thresholdSlider.GetValueForPercentage(GameUtil.GetRoundedTemperatureInKelvin(this.thresholdSlider.value)));
		};
		this.thresholdSlider.onPointerDown += delegate
		{
			this.ReceiveValueFromSlider(this.thresholdSlider.GetValueForPercentage(GameUtil.GetRoundedTemperatureInKelvin(this.thresholdSlider.value)));
		};
		this.thresholdSlider.onMove += delegate
		{
			this.ReceiveValueFromSlider(this.thresholdSlider.GetValueForPercentage(GameUtil.GetRoundedTemperatureInKelvin(this.thresholdSlider.value)));
		};
		this.numberInput.onEndEdit += delegate
		{
			this.ReceiveValueFromInput(this.numberInput.currentValue);
		};
		this.numberInput.decimalPlaces = 1;
	}

	public void Render200ms(float dt)
	{
		if (this.target == null)
		{
			this.target = null;
			return;
		}
		this.UpdateLabels();
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<IThresholdSwitch>() != null;
	}

	public override void SetTarget(GameObject new_target)
	{
		this.target = null;
		if (new_target == null)
		{
			global::Debug.LogError("Invalid gameObject received", null);
			return;
		}
		this.target = new_target;
		this.thresholdSwitch = this.target.GetComponent<IThresholdSwitch>();
		if (this.thresholdSwitch == null)
		{
			this.target = null;
			global::Debug.LogError("The gameObject received does not contain a IThresholdSwitch component", null);
			return;
		}
		this.UpdateLabels();
		if (this.target.GetComponent<IThresholdSwitch>().LayoutType == ThresholdScreenLayoutType.SliderBar)
		{
			this.thresholdSlider.gameObject.SetActive(true);
			this.thresholdSlider.minValue = 0f;
			this.thresholdSlider.maxValue = 100f;
			this.thresholdSlider.SetRanges(this.thresholdSwitch.GetRanges);
			this.thresholdSlider.value = this.thresholdSlider.GetPercentageFromValue(this.thresholdSwitch.Threshold);
			this.thresholdSlider.GetComponentInChildren<ToolTip>();
		}
		else
		{
			this.thresholdSlider.gameObject.SetActive(false);
		}
		MultiToggle incrementMinorToggle = this.incrementMinor.GetComponent<MultiToggle>();
		incrementMinorToggle.onClick = delegate
		{
			this.UpdateThresholdValue(this.thresholdSwitch.Threshold + (float)this.thresholdSwitch.IncrementScale);
			incrementMinorToggle.ChangeState(1);
		};
		incrementMinorToggle.onStopHold = delegate
		{
			incrementMinorToggle.ChangeState(0);
		};
		MultiToggle incrementMajorToggle = this.incrementMajor.GetComponent<MultiToggle>();
		incrementMajorToggle.onClick = delegate
		{
			this.UpdateThresholdValue(this.thresholdSwitch.Threshold + 10f * (float)this.thresholdSwitch.IncrementScale);
			incrementMajorToggle.ChangeState(1);
		};
		incrementMajorToggle.onStopHold = delegate
		{
			incrementMajorToggle.ChangeState(0);
		};
		MultiToggle decrementMinorToggle = this.decrementMinor.GetComponent<MultiToggle>();
		decrementMinorToggle.onClick = delegate
		{
			this.UpdateThresholdValue(this.thresholdSwitch.Threshold - (float)this.thresholdSwitch.IncrementScale);
			decrementMinorToggle.ChangeState(1);
		};
		decrementMinorToggle.onStopHold = delegate
		{
			decrementMinorToggle.ChangeState(0);
		};
		MultiToggle decrementMajorToggle = this.decrementMajor.GetComponent<MultiToggle>();
		decrementMajorToggle.onClick = delegate
		{
			this.UpdateThresholdValue(this.thresholdSwitch.Threshold - 10f * (float)this.thresholdSwitch.IncrementScale);
			decrementMajorToggle.ChangeState(1);
		};
		decrementMajorToggle.onStopHold = delegate
		{
			decrementMajorToggle.ChangeState(0);
		};
		this.unitsLabel.text = this.thresholdSwitch.ThresholdValueUnits();
		this.numberInput.minValue = this.thresholdSwitch.GetRangeMinInputField();
		this.numberInput.maxValue = this.thresholdSwitch.GetRangeMaxInputField();
		this.numberInput.Activate();
		this.UpdateTargetThresholdLabel();
		this.OnConditionButtonClicked(this.thresholdSwitch.ActivateAboveThreshold);
	}

	private void OnThresholdValueChanged(float new_value)
	{
		this.thresholdSwitch.Threshold = new_value;
		this.UpdateTargetThresholdLabel();
	}

	private void OnConditionButtonClicked(bool activate_above_threshold)
	{
		this.thresholdSwitch.ActivateAboveThreshold = activate_above_threshold;
		if (activate_above_threshold)
		{
			this.belowToggle.isOn = true;
			this.aboveToggle.isOn = false;
			this.belowToggle.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Inactive);
			this.aboveToggle.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Active);
		}
		else
		{
			this.belowToggle.isOn = false;
			this.aboveToggle.isOn = true;
			this.belowToggle.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Active);
			this.aboveToggle.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Inactive);
		}
		this.UpdateTargetThresholdLabel();
	}

	private void UpdateTargetThresholdLabel()
	{
		this.numberInput.SetDisplayValue(this.thresholdSwitch.Format(this.thresholdSwitch.Threshold, false));
		if (this.thresholdSwitch.ActivateAboveThreshold)
		{
			this.thresholdSlider.GetComponentInChildren<ToolTip>().SetSimpleTooltip(string.Format(this.thresholdSwitch.AboveToolTip, this.thresholdSwitch.Format(this.thresholdSwitch.Threshold, true)));
			this.thresholdSlider.GetComponentInChildren<ToolTip>().tooltipPositionOffset = new Vector2(0f, 25f);
		}
		else
		{
			this.thresholdSlider.GetComponentInChildren<ToolTip>().SetSimpleTooltip(string.Format(this.thresholdSwitch.BelowToolTip, this.thresholdSwitch.Format(this.thresholdSwitch.Threshold, true)));
			this.thresholdSlider.GetComponentInChildren<ToolTip>().tooltipPositionOffset = new Vector2(0f, 25f);
		}
	}

	private void ReceiveValueFromSlider(float newValue)
	{
		this.UpdateThresholdValue(this.thresholdSwitch.ProcessedSliderValue(newValue));
	}

	private void ReceiveValueFromInput(float newValue)
	{
		this.UpdateThresholdValue(this.thresholdSwitch.ProcessedInputValue(newValue));
	}

	private void UpdateThresholdValue(float newValue)
	{
		if (newValue < this.thresholdSwitch.RangeMin)
		{
			newValue = this.thresholdSwitch.RangeMin;
		}
		if (newValue > this.thresholdSwitch.RangeMax)
		{
			newValue = this.thresholdSwitch.RangeMax;
		}
		this.thresholdSwitch.Threshold = newValue;
		NonLinearSlider nonLinearSlider = this.thresholdSlider;
		if (nonLinearSlider != null)
		{
			this.thresholdSlider.value = nonLinearSlider.GetPercentageFromValue(newValue);
		}
		else
		{
			this.thresholdSlider.value = newValue;
		}
		this.UpdateTargetThresholdLabel();
	}

	private void UpdateLabels()
	{
		this.currentValue.text = string.Format(UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.CURRENT_VALUE, this.thresholdSwitch.ThresholdValueName, this.thresholdSwitch.Format(this.thresholdSwitch.CurrentValue, true));
	}

	public override string GetTitle()
	{
		if (this.target != null)
		{
			return this.thresholdSwitch.Title;
		}
		return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TITLE;
	}

	private GameObject target;

	private IThresholdSwitch thresholdSwitch;

	[SerializeField]
	private LocText currentValue;

	[SerializeField]
	private LocText tresholdValue;

	[SerializeField]
	private KToggle aboveToggle;

	[SerializeField]
	private KToggle belowToggle;

	[Header("Slider")]
	[SerializeField]
	private NonLinearSlider thresholdSlider;

	[Header("Number Input")]
	[SerializeField]
	private KNumberInputField numberInput;

	[SerializeField]
	private LocText unitsLabel;

	[Header("Increment Buttons")]
	[SerializeField]
	private GameObject incrementMinor;

	[SerializeField]
	private GameObject incrementMajor;

	[SerializeField]
	private GameObject decrementMinor;

	[SerializeField]
	private GameObject decrementMajor;
}
