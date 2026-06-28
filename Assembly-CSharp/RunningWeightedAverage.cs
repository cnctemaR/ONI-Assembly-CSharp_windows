using System;

public class RunningWeightedAverage
{
	public RunningWeightedAverage(float minValue = -3.4028235E+38f, float maxValue = 3.4028235E+38f, int sampleCount = 15, bool allowZero = true)
	{
		this.min = minValue;
		this.max = maxValue;
		this.ignoreZero = !allowZero;
		this.samples = new float[sampleCount];
	}

	public float GetWeightedAverage
	{
		get
		{
			return this.WeightedAverage();
		}
	}

	public float GetUnweightedAverage
	{
		get
		{
			return this.WeightedAverage();
		}
	}

	public void AddSample(float value)
	{
		if (this.ignoreZero && value == 0f)
		{
			return;
		}
		if (value > this.max)
		{
			value = this.max;
		}
		if (value < this.min)
		{
			value = this.min;
		}
		if (this.validValues < this.samples.Length)
		{
			this.validValues++;
		}
		for (int i = 0; i < this.samples.Length - 1; i++)
		{
			this.samples[i] = this.samples[i + 1];
		}
		this.samples[this.samples.Length - 1] = value;
	}

	private float WeightedAverage()
	{
		float num = 0f;
		float num2 = 0f;
		for (int i = this.samples.Length - 1; i > this.samples.Length - 1 - this.validValues; i--)
		{
			float num3 = (float)(i + 1) / ((float)this.validValues + 1f);
			num += this.samples[i] * num3;
			num2 += num3;
		}
		num /= num2;
		if (float.IsNaN(num))
		{
			return 0f;
		}
		return num;
	}

	private float UnweightedAverage()
	{
		float num = 0f;
		for (int i = this.samples.Length - 1; i > this.samples.Length - 1 - this.validValues; i--)
		{
			num += this.samples[i];
		}
		num /= (float)this.samples.Length;
		if (float.IsNaN(num))
		{
			return 0f;
		}
		return num;
	}

	private float[] samples;

	private float min;

	private float max;

	private bool ignoreZero;

	private int validValues;
}
