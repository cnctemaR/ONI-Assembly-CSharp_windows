using System;
using STRINGS;
using UnityEngine;
using UnityEngine.Events;

public class ThresholdSwitchSideScreen : SideScreenContent
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
		this.thresholdSlider.onValueChanged.AddListener(new UnityAction<float>(this.OnThresholdValueChanged));
	}

	private void SimUpdate(float dt)
	{
		if (this.target == null)
		{
			return;
		}
		this.UpdateLabels();
	}

	public override void SetTarget(GameObject new_target)
	{
		if (new_target == null)
		{
			Debug.LogError("Invalid gameObject received");
			return;
		}
		this.target = new_target.GetComponent<IThresholdSwitch>();
		if (this.target == null)
		{
			Debug.LogError("The gameObject received does not contain a IThresholdSwitch component");
			return;
		}
		this.UpdateLabels();
		this.thresholdSlider.onValueChanged.RemoveListener(new UnityAction<float>(this.OnThresholdValueChanged));
		this.thresholdSlider.minValue = this.target.RangeMin;
		this.thresholdSlider.maxValue = this.target.RangeMax;
		this.thresholdSlider.value = this.target.Threshold;
		this.thresholdSlider.onValueChanged.AddListener(new UnityAction<float>(this.OnThresholdValueChanged));
		this.thresholdSlider.GetComponentInChildren<ToolTip>();
		this.UpdateTargetThresholdLabel();
		this.OnConditionButtonClicked(this.target.ActivateAboveThreshold);
	}

	private void OnThresholdValueChanged(float new_value)
	{
		this.target.Threshold = new_value;
		this.UpdateTargetThresholdLabel();
	}

	private void OnConditionButtonClicked(bool activate_above_threshold)
	{
		this.target.ActivateAboveThreshold = activate_above_threshold;
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
		this.tresholdValue.text = this.target.Format(this.target.Threshold);
		if (this.target.ActivateAboveThreshold)
		{
			this.thresholdSlider.GetComponentInChildren<ToolTip>().SetSimpleTooltip(string.Format(this.target.AboveToolTip, this.target.Format(this.target.Threshold)));
		}
		else
		{
			this.thresholdSlider.GetComponentInChildren<ToolTip>().SetSimpleTooltip(string.Format(this.target.BelowToolTip, this.target.Format(this.target.Threshold)));
		}
	}

	private void UpdateLabels()
	{
		this.currentValue.text = string.Format(UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.CURRENT_VALUE, this.target.ThresholdValueName, this.target.Format(this.target.CurrentValue));
	}

	private IThresholdSwitch target;

	[SerializeField]
	private LocText currentValue;

	[SerializeField]
	private LocText tresholdValue;

	[SerializeField]
	private KToggle aboveToggle;

	[SerializeField]
	private KToggle belowToggle;

	[SerializeField]
	private KSlider thresholdSlider;
}
