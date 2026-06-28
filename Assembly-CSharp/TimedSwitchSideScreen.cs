using System;
using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimedSwitchSideScreen : SideScreenContent
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.onTimeIncreaseButton.onClick += delegate
		{
			this.OnButtonClicked(true, true);
		};
		this.onTimeDecreaseButton.onClick += delegate
		{
			this.OnButtonClicked(true, false);
		};
		this.offTimeIncreaseButton.onClick += delegate
		{
			this.OnButtonClicked(false, true);
		};
		this.offTimeDecreaseButton.onClick += delegate
		{
			this.OnButtonClicked(false, false);
		};
		this.onButton.onClick += delegate
		{
			this.targetTimedSwitch.SetState(true);
		};
		this.offButton.onClick += delegate
		{
			this.targetTimedSwitch.SetState(false);
		};
		this.onTimeInputField.onEndEdit.AddListener(delegate(string str)
		{
			this.OnEndEdit(this.onTimeInputField, str);
		});
		this.offTimeInputField.onEndEdit.AddListener(delegate(string str)
		{
			this.OnEndEdit(this.offTimeInputField, str);
		});
	}

	private void OnEndEdit(TMP_InputField inputField, string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			this.UpdateInputFields();
			return;
		}
		float num = 0f;
		if (float.TryParse(str, out num))
		{
			if (num < 0f)
			{
				num = 0f;
			}
			if (inputField == this.onTimeInputField)
			{
				this.targetTimedSwitch.onTime = num;
			}
			else
			{
				this.targetTimedSwitch.offTime = num;
			}
		}
		this.UpdateInputFields();
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
		if (this.targetTimedSwitch == null)
		{
			return;
		}
		if (!this.targetTimedSwitch.IsConnected())
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
			global::Debug.LogError("Invalid gameObject received", null);
			return;
		}
		this.targetTimedSwitch = target.GetComponent<TimedSwitch>();
		if (this.targetTimedSwitch == null)
		{
			global::Debug.LogError("The gameObject received does not contain a TimedSwitch component", null);
			return;
		}
		if (!this.targetTimedSwitch.IsConnected())
		{
			this.SetValidContentState(false);
		}
		else
		{
			this.SetValidContentState(true);
			this.OnToggle(this.targetTimedSwitch.IsSwitchedOn);
			this.UpdateLabels();
		}
		this.UpdateInputFields();
	}

	private void UpdateInputFields()
	{
		this.onTimeInputField.text = this.targetTimedSwitch.onTime.ToString("F0");
		this.offTimeInputField.text = this.targetTimedSwitch.offTime.ToString("F0");
	}

	private void OnToggle(bool newState)
	{
		this.onTimeBG.enabled = newState;
		this.offTimeBG.enabled = !newState;
		if (newState)
		{
			this.onButton.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Active);
			this.offButton.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Inactive);
		}
		else
		{
			this.onButton.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Inactive);
			this.offButton.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Active);
		}
	}

	public void OnButtonClicked(bool onTime, bool increase)
	{
		if (onTime)
		{
			float num = ((!this.targetTimedSwitch.IsSwitchedOn) ? this.targetTimedSwitch.onTime : this.targetTimedSwitch.switchTime);
			num += ((!increase) ? (-1f) : 1f);
			this.SetOnTime(num);
		}
		else
		{
			float num2 = ((!this.targetTimedSwitch.IsSwitchedOn) ? this.targetTimedSwitch.switchTime : this.targetTimedSwitch.offTime);
			num2 += ((!increase) ? (-1f) : 1f);
			this.SetOffTime(num2);
		}
	}

	public void SetOnTime(float value)
	{
		this.SetTime(value, ref this.targetTimedSwitch.onTime, this.targetTimedSwitch.IsSwitchedOn);
	}

	public void SetOffTime(float value)
	{
		this.SetTime(value, ref this.targetTimedSwitch.offTime, !this.targetTimedSwitch.IsSwitchedOn);
	}

	public void SetTime(float newValue, ref float valueReference, bool updateCondition)
	{
		if (newValue < 0f)
		{
			newValue = 0f;
		}
		valueReference = newValue;
		if (updateCondition)
		{
			this.targetTimedSwitch.switchTime = newValue;
		}
		this.UpdateInputFields();
	}

	private void UpdateLabels()
	{
		string text = ((!this.targetTimedSwitch.IsSwitchedOn) ? UI.UISIDESCREENS.TIMEDSWITCHSIDESCREEN.TIMETOACTIVATE : UI.UISIDESCREENS.TIMEDSWITCHSIDESCREEN.TIMETODEACTIVATE);
		this.currentTime.text = string.Format(text, this.targetTimedSwitch.switchTime.ToString("F0"));
	}

	private const float MIN_TIME_CHANGE = 1f;

	private const string TIME_FORMAT = "F0";

	private TimedSwitch targetTimedSwitch;

	[SerializeField]
	[Header("Header")]
	private GameObject validContent;

	[SerializeField]
	private GameObject warningLabel;

	[SerializeField]
	private LocText currentTime;

	[SerializeField]
	private KButton onButton;

	[SerializeField]
	private KButton offButton;

	[Header("On Time")]
	[SerializeField]
	private KButton onTimeIncreaseButton;

	[SerializeField]
	private KButton onTimeDecreaseButton;

	[SerializeField]
	private TMP_InputField onTimeInputField;

	[SerializeField]
	private Image onTimeBG;

	[Header("Off Time")]
	[SerializeField]
	private KButton offTimeIncreaseButton;

	[SerializeField]
	private KButton offTimeDecreaseButton;

	[SerializeField]
	private TMP_InputField offTimeInputField;

	[SerializeField]
	private Image offTimeBG;
}
