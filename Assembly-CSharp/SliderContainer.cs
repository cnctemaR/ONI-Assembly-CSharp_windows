using System;
using UnityEngine;
using UnityEngine.Events;

[AddComponentMenu("KMonoBehaviour/scripts/SliderContainer")]
public class SliderContainer : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.slider.onValueChanged.AddListener(new UnityAction<float>(this.UpdateSliderLabel));
	}

	public void UpdateSliderLabel(float newValue)
	{
		if (this.isPercentValue)
		{
			this.valueLabel.text = (newValue * 100f).ToString("F0") + "%";
			return;
		}
		this.valueLabel.text = newValue.ToString();
	}

	public bool isPercentValue = true;

	public KSlider slider;

	public LocText nameLabel;

	public LocText valueLabel;
}
