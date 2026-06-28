using System;
using STRINGS;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TemperatureSwitchSideScreen : SideScreenContent
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.coolerToggle.onClick += delegate
		{
			this.OnConditionButtonClicked(false);
		};
		this.warmerToggle.onClick += delegate
		{
			this.OnConditionButtonClicked(true);
		};
		LocText component = this.coolerToggle.transform.GetChild(0).GetComponent<LocText>();
		LocText component2 = this.warmerToggle.transform.GetChild(0).GetComponent<LocText>();
		component.SetText(UI.UISIDESCREENS.TEMPERATURESWITCHSIDESCREEN.COLDER_BUTTON);
		component2.SetText(UI.UISIDESCREENS.TEMPERATURESWITCHSIDESCREEN.WARMER_BUTTON);
		Slider.SliderEvent sliderEvent = new Slider.SliderEvent();
		sliderEvent.AddListener(new UnityAction<float>(this.OnTargetTemperatureChanged));
		this.targetTemperatureSlider.onValueChanged = sliderEvent;
	}

	private void SimUpdate(float dt)
	{
		if (!(this.targetTemperatureSwitch == null))
		{
			this.UpdateLabels();
		}
	}

	public override void SetTarget(GameObject target)
	{
		if (target == null)
		{
			global::Debug.LogError("Invalid gameObject received", null);
		}
		else
		{
			this.targetTemperatureSwitch = target.GetComponent<TemperatureControlledSwitch>();
			if (this.targetTemperatureSwitch == null)
			{
				global::Debug.LogError("The gameObject received does not contain a TimedSwitch component", null);
			}
			else
			{
				this.UpdateLabels();
				this.UpdateTargetTemperatureLabel();
				this.OnConditionButtonClicked(this.targetTemperatureSwitch.activateOnWarmerThan);
			}
		}
	}

	private void OnTargetTemperatureChanged(float new_value)
	{
		this.targetTemperatureSwitch.thresholdTemperature = new_value;
		this.UpdateTargetTemperatureLabel();
	}

	private void OnConditionButtonClicked(bool isWarmer)
	{
		this.targetTemperatureSwitch.activateOnWarmerThan = isWarmer;
		if (isWarmer)
		{
			this.coolerToggle.isOn = false;
			this.warmerToggle.isOn = true;
			this.coolerToggle.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Inactive);
			this.warmerToggle.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Active);
		}
		else
		{
			this.coolerToggle.isOn = true;
			this.warmerToggle.isOn = false;
			this.coolerToggle.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Active);
			this.warmerToggle.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Inactive);
		}
	}

	private void UpdateTargetTemperatureLabel()
	{
		this.targetTemperature.text = GameUtil.GetFormattedTemperature(this.targetTemperatureSwitch.thresholdTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true);
	}

	private void UpdateLabels()
	{
		this.currentTemperature.text = string.Format(UI.UISIDESCREENS.TEMPERATURESWITCHSIDESCREEN.CURRENT_TEMPERATURE, GameUtil.GetFormattedTemperature(this.targetTemperatureSwitch.GetTemperature(), GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
	}

	private TemperatureControlledSwitch targetTemperatureSwitch;

	[SerializeField]
	private LocText currentTemperature;

	[SerializeField]
	private LocText targetTemperature;

	[SerializeField]
	private KToggle coolerToggle;

	[SerializeField]
	private KToggle warmerToggle;

	[SerializeField]
	private KSlider targetTemperatureSlider;
}
