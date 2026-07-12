using System;
using System.Collections.Generic;
using UnityEngine;

public class RunningWeightedAverage
{
	public RunningWeightedAverage(float minValue = -3.4028235E+38f, float maxValue = 3.4028235E+38f, int sampleCount = 20, bool allowZero = true)
	{
		this.min = minValue;
		this.max = maxValue;
		this.ignoreZero = !allowZero;
		this.samples = new List<global::Tuple<float, float>>();
	}

	public float GetUnweightedAverage
	{
		get
		{
			return this.GetAverageOfLastSeconds(4f);
		}
	}

	public bool HasEverHadValidValues
	{
		get
		{
			return this.validSampleCount >= this.maxSamples;
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
		if (this.validSampleCount <= this.maxSamples)
		{
			this.validSampleCount++;
		}
		this.samples.Add(new global::Tuple<float, float>(value, timeOfRecord));
		if (this.samples.Count > this.maxSamples)
		{
			this.samples.RemoveAt(0);
		}
	}

	public int ValidRecordsInLastSeconds(float seconds)
	{
		int num = 0;
		int num2 = this.samples.Count - 1;
		while (num2 >= 0 && Time.time - this.samples[num2].second <= seconds)
		{
			num++;
			num2--;
		}
		return num;
	}

	private float GetAverageOfLastSeconds(float seconds)
	{
		float num = 0f;
		int num2 = 0;
		int num3 = this.samples.Count - 1;
		while (num3 >= 0 && Time.time - this.samples[num3].second <= seconds)
		{
			num += this.samples[num3].first;
			num2++;
			num3--;
		}
		if (num2 == 0)
		{
			return 0f;
		}
		return num / (float)num2;
	}

	private List<global::Tuple<float, float>> samples = new List<global::Tuple<float, float>>();

	private float min;

	private float max;

	private bool ignoreZero;

	private int validSampleCount;

	private int maxSamples = 20;
}
