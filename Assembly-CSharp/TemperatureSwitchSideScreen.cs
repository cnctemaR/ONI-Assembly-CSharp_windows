using System;
using STRINGS;
using UnityEngine;

public class TemperatureSwitchSideScreen : SideScreenContent
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.increaseButton.onClick += delegate
		{
			this.OnTemperatureChangeButtonClicked(false);
		};
		this.decreaseButton.onClick += delegate
		{
			this.OnTemperatureChangeButtonClicked(true);
		};
		this.coolerButton.onClick += delegate
		{
			this.OnConditionButtonClicked(false);
		};
		this.warmerButton.onClick += delegate
		{
			this.OnConditionButtonClicked(true);
		};
	}

	private void SetValidContentState(bool valid)
	{
		if (this.validContent.activeInHierarchy != valid)
		{
			this.validContent.SetActive(valid);
		}
		if (this.warningLabel.activeInHierarchy == valid)
		{
			this.warningLabel.SetActive(!valid);
		}
	}

	private void SimUpdate(float dt)
	{
		if (this.targetTemperatureSwitch == null)
		{
			return;
		}
		if (!this.targetTemperatureSwitch.IsConnected())
		{
			this.SetValidContentState(false);
			return;
		}
		this.SetValidContentState(true);
		this.UpdateLabels();
	}

	public override void SetTarget(GameObject target)
	{
		if (target == null)
		{
			Debug.LogError("Invalid gameObject received");
			return;
		}
		this.targetTemperatureSwitch = target.GetComponent<TemperatureControlledSwitch>();
		if (this.targetTemperatureSwitch == null)
		{
			Debug.LogError("The gameObject received does not contain a TimedSwitch component");
			return;
		}
		if (!this.targetTemperatureSwitch.IsConnected())
		{
			this.SetValidContentState(false);
		}
		else
		{
			this.SetValidContentState(true);
			this.UpdateLabels();
		}
		this.UpdateTargetTemperatureLabel();
		this.OnConditionButtonClicked(this.targetTemperatureSwitch.activateOnWarmerThan);
	}

	private void OnTemperatureChangeButtonClicked(bool increase)
	{
		this.targetTemperatureSwitch.thresholdTemperature += ((!increase) ? (-1f) : 1f);
		this.UpdateTargetTemperatureLabel();
	}

	private void OnConditionButtonClicked(bool isWarmer)
	{
		this.targetTemperatureSwitch.activateOnWarmerThan = isWarmer;
		if (isWarmer)
		{
			this.coolerButton.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Inactive);
			this.warmerButton.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Active);
		}
		else
		{
			this.coolerButton.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Active);
			this.warmerButton.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Inactive);
		}
	}

	private void UpdateTargetTemperatureLabel()
	{
		this.targetTemperature.text = GameUtil.GetFormattedTemperature(this.targetTemperatureSwitch.thresholdTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute);
	}

	private void UpdateLabels()
	{
		this.currentTemperature.text = string.Format(UI.UISIDESCREENS.TEMPERATURESWITCHSIDESCREEN.CURRENTTEMPERATURE, GameUtil.GetFormattedTemperature(this.targetTemperatureSwitch.StructureTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute));
	}

	private const float MIN_CHANGE = 1f;

	private TemperatureControlledSwitch targetTemperatureSwitch;

	[SerializeField]
	private GameObject validContent;

	[SerializeField]
	private GameObject warningLabel;

	[SerializeField]
	private LocText currentTemperature;

	[SerializeField]
	private LocText targetTemperature;

	[SerializeField]
	private KButton coolerButton;

	[SerializeField]
	private KButton warmerButton;

	[SerializeField]
	private KButton increaseButton;

	[SerializeField]
	private KButton decreaseButton;
}
