using System;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/GenericUIProgressBar")]
public class GenericUIProgressBar : KMonoBehaviour
{
	public void SetMaxValue(float max)
	{
		this.maxValue = max;
	}

	public void SetFillPercentage(float value)
	{
		this.fill.fillAmount = value;
		this.label.text = Util.FormatWholeNumber(Mathf.Min(this.maxValue, this.maxValue * value)) + "/" + this.maxValue.ToString();
	}

	public Image fill;

	public LocText label;

	private float maxValue;
}
