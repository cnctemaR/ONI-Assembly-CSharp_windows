using System;

namespace UnityEngine.AdaptivePerformance
{
	internal class TemperatureTrend
	{
		private void PopOldestValue()
		{
			double num = (double)this.m_TimeStamps[this.m_OldestValueIndex];
			double num2 = (double)this.m_Temperature[this.m_OldestValueIndex];
			this.m_SumX -= num;
			this.m_SumY -= num2;
			this.m_SumXY -= num * num2;
			this.m_SumXX -= num * num;
			this.m_OldestValueIndex = (this.m_OldestValueIndex + 1) % 200;
			this.m_NumValues--;
		}

		private void PushNewValue(float tempLevel, float timestamp)
		{
			this.m_TimeStamps[this.m_NextValueIndex] = timestamp;
			this.m_Temperature[this.m_NextValueIndex] = tempLevel;
			this.m_NextValueIndex = (this.m_NextValueIndex + 1) % 200;
			this.m_NumValues++;
			double num = (double)timestamp;
			double num2 = (double)tempLevel;
			this.m_SumX += num;
			this.m_SumY += num2;
			this.m_SumXY += num * num2;
			this.m_SumXX += num * num;
		}

		public TemperatureTrend(bool useProviderTrend)
		{
			this.m_UseProviderTrend = useProviderTrend;
		}

		public void Reset()
		{
			this.m_NumValues = 0;
			this.m_OldestValueIndex = 0;
			this.m_NextValueIndex = 0;
			this.m_SumX = 0.0;
			this.m_SumY = 0.0;
			this.m_SumXY = 0.0;
			this.m_SumXX = 0.0;
			this.ThermalTrend = 0f;
		}

		public float ThermalTrend { get; private set; }

		private void UpdateTrend()
		{
			bool flag = this.m_NumValues < 2;
			if (flag)
			{
				this.ThermalTrend = 0f;
			}
			else
			{
				double num = (double)this.m_NumValues * this.m_SumXY - this.m_SumX * this.m_SumY;
				double num2 = (double)this.m_NumValues * this.m_SumXX - this.m_SumX * this.m_SumX;
				double num3 = num / num2;
				num3 /= 0.005;
				bool flag2 = num3 >= 1.0;
				if (flag2)
				{
					this.ThermalTrend = 1f;
				}
				else
				{
					bool flag3 = num3 >= -1.0;
					if (flag3)
					{
						bool flag4 = Math.Abs(num3) < 1E-05;
						if (flag4)
						{
							this.ThermalTrend = 0f;
						}
						else
						{
							this.ThermalTrend = (float)num3;
						}
					}
					else
					{
						bool flag5 = num3 <= -1.0;
						if (flag5)
						{
							this.ThermalTrend = -1f;
						}
						else
						{
							this.ThermalTrend = 0f;
						}
					}
				}
			}
		}

		public void Update(float temperatureTrendFromProvider, float newTemperatureLevel, bool changed, float newTemperatureTimestamp)
		{
			bool useProviderTrend = this.m_UseProviderTrend;
			if (useProviderTrend)
			{
				this.ThermalTrend = temperatureTrendFromProvider;
			}
			else
			{
				newTemperatureLevel = newTemperatureLevel * newTemperatureLevel * newTemperatureLevel;
				bool flag = this.m_NumValues == 0;
				if (flag)
				{
					this.PushNewValue(newTemperatureLevel, newTemperatureTimestamp);
					this.UpdateTrend();
				}
				else
				{
					bool flag2 = false;
					float num = this.m_TimeStamps[this.m_OldestValueIndex];
					float num2 = num + 0.1f * (float)this.m_NumValues;
					bool flag3 = newTemperatureTimestamp - num > 20f;
					if (flag3)
					{
						this.PopOldestValue();
						flag2 = true;
					}
					bool flag4 = changed || newTemperatureTimestamp >= num2;
					if (flag4)
					{
						bool flag5 = this.m_NumValues == 200;
						if (flag5)
						{
							this.PopOldestValue();
						}
						this.PushNewValue(newTemperatureLevel, newTemperatureTimestamp);
						flag2 = true;
					}
					bool flag6 = flag2;
					if (flag6)
					{
						this.UpdateTrend();
					}
				}
			}
		}

		public int NumValues
		{
			get
			{
				return this.m_NumValues;
			}
			set
			{
				this.m_NumValues = value;
			}
		}

		private bool m_UseProviderTrend;

		private double m_SumX;

		private double m_SumY;

		private double m_SumXY;

		private double m_SumXX;

		private const int MeasurementTimeframeSeconds = 20;

		private const int UpdateFrequency = 10;

		private const int SamplesCapacity = 200;

		private const double SlopeAtMaxTrend = 0.005;

		private float[] m_TimeStamps = new float[200];

		private float[] m_Temperature = new float[200];

		private int m_NumValues;

		private int m_NextValueIndex;

		private int m_OldestValueIndex;
	}
}
