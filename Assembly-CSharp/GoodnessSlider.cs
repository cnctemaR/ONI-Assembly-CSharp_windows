using System;
using UnityEngine;
using UnityEngine.UI;

public class GoodnessSlider : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.Spawn();
		this.UpdateValues();
	}

	public void UpdateValues()
	{
		Graphic graphic = this.text;
		Color color = this.gradient.Evaluate(this.slider.value);
		this.fill.color = color;
		graphic.color = color;
		for (int i = 0; i < this.gradient.colorKeys.Length; i++)
		{
			if (this.gradient.colorKeys[i].time < this.slider.value)
			{
				this.text.text = this.names[i];
			}
			if (i == this.gradient.colorKeys.Length - 1 && this.gradient.colorKeys[i - 1].time < this.slider.value)
			{
				this.text.text = this.names[i];
			}
		}
	}

	public Image icon;

	public Text text;

	public Slider slider;

	public Image fill;

	public Gradient gradient;

	public string[] names;
}
