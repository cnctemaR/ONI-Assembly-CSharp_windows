using System;
using UnityEngine;

public class NonLinearSlider : KSlider
{
	public static NonLinearSlider.Range[] GetDefaultRange(float maxValue)
	{
		return new NonLinearSlider.Range[]
		{
			new NonLinearSlider.Range(100f, maxValue)
		};
	}

	protected override void Start()
	{
		base.Start();
		base.minValue = 0f;
		base.maxValue = 100f;
	}

	public void SetRanges(NonLinearSlider.Range[] ranges)
	{
		this.ranges = ranges;
	}

	public float GetPercentageFromValue(float value)
	{
		float num = 0f;
		float num2 = 0f;
		for (int i = 0; i < this.ranges.Length; i++)
		{
			if (value > num2 && value <= this.ranges[i].peakValue)
			{
				float num3 = (value - num2) / (this.ranges[i].peakValue - num2);
				return Mathf.Lerp(num, num + this.ranges[i].width, num3);
			}
			num += this.ranges[i].width;
			num2 = this.ranges[i].peakValue;
		}
		return 100f;
	}

	public float GetValueForPercentage(float percentage)
	{
		float num = 0f;
		float num2 = 0f;
		for (int i = 0; i < this.ranges.Length; i++)
		{
			if (percentage > num && num + this.ranges[i].width >= percentage)
			{
				float num3 = (percentage - num) / this.ranges[i].width;
				return Mathf.Lerp(num2, this.ranges[i].peakValue, num3);
			}
			num += this.ranges[i].width;
			num2 = this.ranges[i].peakValue;
		}
		return num2;
	}

	protected override void Set(float input, bool sendCallback)
	{
		base.Set(input, sendCallback);
	}

	public NonLinearSlider.Range[] ranges;

	[Serializable]
	public struct Range
	{
		public Range(float width, float peakValue)
		{
			this.width = width;
			this.peakValue = peakValue;
		}

		public float width;

		public float peakValue;
	}
}
