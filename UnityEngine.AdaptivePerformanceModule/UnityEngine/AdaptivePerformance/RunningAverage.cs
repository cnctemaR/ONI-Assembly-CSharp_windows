using System;

namespace UnityEngine.AdaptivePerformance
{
	internal class RunningAverage
	{
		public RunningAverage(int sampleWindowSize = 100)
		{
			this.m_Values = new float[sampleWindowSize];
		}

		public int GetNumValues()
		{
			return this.m_NumValues;
		}

		public int GetSampleWindowSize()
		{
			return this.m_Values.Length;
		}

		public float GetAverageOr(float defaultValue)
		{
			return (this.m_NumValues > 0) ? this.m_AverageValue : defaultValue;
		}

		public float GetMostRecentValueOr(float defaultValue)
		{
			return (this.m_NumValues > 0) ? this.m_Values[this.m_LastIndex] : defaultValue;
		}

		public void AddValue(float NewValue)
		{
			int num = (this.m_LastIndex + 1) % this.m_Values.Length;
			float num2 = this.m_Values[num];
			this.m_LastIndex = num;
			this.m_Values[this.m_LastIndex] = NewValue;
			float num3 = this.m_AverageValue * (float)this.m_NumValues + NewValue - num2;
			this.m_NumValues = Mathf.Min(this.m_NumValues + 1, this.m_Values.Length);
			this.m_AverageValue = num3 / (float)this.m_NumValues;
		}

		public void Reset()
		{
			this.m_NumValues = 0;
			this.m_LastIndex = -1;
			this.m_AverageValue = 0f;
			Array.Clear(this.m_Values, 0, this.m_Values.Length);
		}

		private float[] m_Values = null;

		private int m_NumValues = 0;

		private int m_LastIndex = -1;

		private float m_AverageValue = 0f;
	}
}
