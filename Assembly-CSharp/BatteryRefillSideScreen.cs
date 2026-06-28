using System;
using UnityEngine;

public class BatteryRefillSideScreen : SideScreenContent
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.percentSlider.onReleaseHandle += delegate
		{
			this.percentSlider.value = Mathf.Round(this.percentSlider.value * 100f) / 100f;
			this.SetBatteryRefillPercent();
		};
		this.percentSlider.onDrag += delegate
		{
			this.ReceiveValueFromSlider(this.percentSlider.value);
		};
		this.percentSlider.onMove += delegate
		{
			this.ReceiveValueFromSlider(this.percentSlider.value);
		};
		this.percentSlider.onPointerDown += delegate
		{
			this.ReceiveValueFromSlider(this.percentSlider.value);
		};
		this.numberInput.minValue = 0f;
		this.numberInput.maxValue = 100f;
		this.numberInput.onEndEdit += delegate
		{
			this.SetBatteryRefillPercent();
			this.ReceiveValueFromInput(this.numberInput.currentValue);
		};
	}

	public override void SetTarget(GameObject new_target)
	{
		if (new_target == null)
		{
			global::Debug.LogError("Invalid gameObject received", null);
			return;
		}
		this.target = new_target.GetComponent<IBatteryRefillControl>();
		if (this.target == null)
		{
			global::Debug.LogError("The gameObject received does not contain a Manual Generator component", null);
			return;
		}
		this.percentSlider.value = this.target.BatteryRefillPercent;
		this.numberInput.Activate();
		this.UpdatePercentLabel(this.target.BatteryRefillPercent);
	}

	private void ReceiveValueFromSlider(float input)
	{
		input = Mathf.Round(input * 100f) / 100f;
		this.UpdatePercentLabel(input);
	}

	private void ReceiveValueFromInput(float input)
	{
		input = Mathf.Round(input * 10f) / 10f;
		input /= 100f;
		this.percentSlider.value = input;
		this.SetBatteryRefillPercent();
		this.UpdatePercentLabel(input);
	}

	private void SetBatteryRefillPercent()
	{
		float num = this.percentSlider.value;
		if (num > 1f)
		{
			num = 1f;
		}
		else if (num < 0f)
		{
			num = 0f;
		}
		this.target.BatteryRefillPercent = num;
	}

	private void UpdatePercentLabel(float value)
	{
		this.numberInput.SetDisplayValue((value * 100f).ToString("F0"));
	}

	private IBatteryRefillControl target;

	[SerializeField]
	private KSlider percentSlider;

	[SerializeField]
	private LocText currentPercentLabel;

	[SerializeField]
	[Header("Input Field")]
	private KNumberInputField numberInput;

	[SerializeField]
	private LocText unitsLabel;
}
