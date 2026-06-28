using System;
using STRINGS;
using UnityEngine;
using UnityEngine.Events;

public class ManualGeneratorSideScreen : SideScreenContent
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.percentSlider.onReleaseHandle += this.SetBatteryRefillPercent;
		this.percentSlider.onValueChanged.AddListener(new UnityAction<float>(this.UpdatePercentLabel));
	}

	public override void SetTarget(GameObject target)
	{
		if (target == null)
		{
			Debug.LogError("Invalid gameObject received");
			return;
		}
		this.targetManualGenerator = target.GetComponent<ManualGenerator>();
		if (this.targetManualGenerator == null)
		{
			Debug.LogError("The gameObject received does not contain a Manual Generator component");
			return;
		}
		this.percentSlider.value = this.targetManualGenerator.batteryRefillPercent;
		this.UpdatePercentLabel(this.targetManualGenerator.batteryRefillPercent);
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
		this.targetManualGenerator.batteryRefillPercent = num;
	}

	private void UpdatePercentLabel(float value)
	{
		this.currentPercentLabel.text = string.Format(UI.UISIDESCREENS.MANUALGENERATORSIDESCREEN.CURRENT_THRESHOLD, (value * 100f).ToString("F0"));
	}

	private ManualGenerator targetManualGenerator;

	[SerializeField]
	private KSlider percentSlider;

	[SerializeField]
	private LocText currentPercentLabel;
}
