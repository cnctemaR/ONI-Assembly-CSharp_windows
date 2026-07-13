using System;

public class RunningAverage
{
	public RunningAverage(float minValue = -3.4028235E+38f, float maxValue = 3.4028235E+38f, int sampleCount = 15, bool allowZero = true)
	{
		this.samples = new RingBuffer<float>(sampleCount, float.NaN);
		this.min = minValue;
		this.max = maxValue;
		this.ignoreZero = !allowZero;
	}

	public float AverageValue
	{
		get
		{
			return this.GetAverage();
		}
	}

	public void AddSample(float value)
	{
		if (value < this.min || value > this.max || (this.ignoreZero && value == 0f))
		{
			return;
		}
		this.samples.Add(value);
	}

	private float GetAverage()
	{
		float num = 0f;
		int num2 = 0;
		for (int i = 0; i < this.samples.Count; i++)
		{
			float num3 = this.samples[i];
			if (num3 != float.NaN)
			{
				num += num3;
				num2++;
			}
		}
		if (num2 == 0)
		{
			return float.NaN;
		}
		return num / (float)num2;
	}

	private RingBuffer<float> samples;

	private float min;

	private float max;

	private bool ignoreZero;
}
