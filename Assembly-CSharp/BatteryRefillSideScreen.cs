using System;
using STRINGS;
using UnityEngine;
using UnityEngine.Events;

public class BatteryRefillSideScreen : SideScreenContent
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.percentSlider.onReleaseHandle += this.SetBatteryRefillPercent;
		this.percentSlider.onValueChanged.AddListener(new UnityAction<float>(this.UpdatePercentLabel));
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
		this.UpdatePercentLabel(this.target.BatteryRefillPercent);
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
		this.currentPercentLabel.text = string.Format(UI.UISIDESCREENS.MANUALGENERATORSIDESCREEN.CURRENT_THRESHOLD, (value * 100f).ToString("F0"));
	}

	private IBatteryRefillControl target;

	[SerializeField]
	private KSlider percentSlider;

	[SerializeField]
	private LocText currentPercentLabel;
}
