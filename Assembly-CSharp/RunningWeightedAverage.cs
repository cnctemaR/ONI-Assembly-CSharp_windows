using System;
using UnityEngine;

public class RunningWeightedAverage
{
	public RunningWeightedAverage(float minValue = -3.4028235E+38f, float maxValue = 3.4028235E+38f, int sampleCount = 20, bool allowZero = true)
	{
		this.min = minValue;
		this.max = maxValue;
		this.ignoreZero = !allowZero;
		this.samples = new RingBuffer<RunningWeightedAverage.Entry>(sampleCount, new RunningWeightedAverage.Entry
		{
			time = float.NaN,
			value = float.NaN
		});
	}

	public float GetUnweightedAverage
	{
		get
		{
			return this.GetAverageOfLastSeconds(4f);
		}
	}

	public void AddSample(float value, float timeOfRecord)
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
		this.samples.Add(new RunningWeightedAverage.Entry
		{
			time = timeOfRecord,
			value = value
		});
	}

	public int ValidRecordsInLastSeconds(float seconds)
	{
		int num = 0;
		float time = Time.time;
		for (int i = 0; i < this.samples.Count; i++)
		{
			RunningWeightedAverage.Entry entry = this.samples[i];
			if (entry.time != float.NaN && time - entry.time <= seconds)
			{
				num++;
			}
		}
		return num;
	}

	private float GetAverageOfLastSeconds(float seconds)
	{
		float num = 0f;
		int num2 = 0;
		float time = Time.time;
		for (int i = 0; i < this.samples.Count; i++)
		{
			RunningWeightedAverage.Entry entry = this.samples[i];
			if (entry.time != float.NaN && time - entry.time <= seconds)
			{
				num += entry.value;
				num2++;
			}
		}
		if (num2 == 0)
		{
			return 0f;
		}
		return num / (float)num2;
	}

	private RingBuffer<RunningWeightedAverage.Entry> samples;

	private float min;

	private float max;

	private bool ignoreZero;

	private struct Entry
	{
		public bool IsValid()
		{
			return this.time != float.NaN;
		}

		public float time;

		public float value;
	}
}
