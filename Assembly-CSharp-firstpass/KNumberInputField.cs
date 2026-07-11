using System;
using UnityEngine;

public class KNumberInputField : KInputField
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	public void SetAmount(float newValue)
	{
		newValue = Mathf.Clamp(newValue, this.minValue, this.maxValue);
		if (this.decimalPlaces != -1)
		{
			float num = Mathf.Pow(10f, (float)this.decimalPlaces);
			newValue = Mathf.Round(newValue * num) / num;
		}
		this.currentValue = newValue;
		base.SetDisplayValue(this.currentValue.ToString());
	}

	protected override void ProcessInput(string input)
	{
		input = ((input == "") ? this.minValue.ToString() : input);
		float num = this.minValue;
		try
		{
			num = float.Parse(input);
			this.SetAmount(num);
		}
		catch
		{
		}
	}

	public int decimalPlaces = -1;

	public float currentValue;

	public float minValue;

	public float maxValue;
}
