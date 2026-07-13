using System;
using System.Collections.Generic;

namespace UnityEngine.AdaptivePerformance
{
	internal class PerformanceStateTracker
	{
		public float Trend { get; set; }

		public PerformanceStateTracker(int sampleCapacity)
		{
			this.m_Samples = new Queue<float>(sampleCapacity);
			this.m_SampleCapacity = sampleCapacity;
		}

		public StateAction Update()
		{
			float averageFrameTime = Holder.Instance.PerformanceStatus.FrameTiming.AverageFrameTime;
			bool flag = averageFrameTime > 0f;
			if (flag)
			{
				float num = 1f / this.GetEffectiveTargetFrameRate();
				float num2 = averageFrameTime / num - 1f;
				this.m_Samples.Enqueue(num2);
				bool flag2 = this.m_Samples.Count > this.m_SampleCapacity;
				if (flag2)
				{
					this.m_Samples.Dequeue();
				}
			}
			float num3 = 0f;
			foreach (float num4 in this.m_Samples)
			{
				num3 += num4;
			}
			num3 /= (float)this.m_Samples.Count;
			this.Trend = num3;
			bool flag3 = (double)this.Trend >= 0.3;
			StateAction stateAction;
			if (flag3)
			{
				stateAction = StateAction.FastDecrease;
			}
			else
			{
				bool flag4 = (double)this.Trend >= 0.15;
				if (flag4)
				{
					stateAction = StateAction.Decrease;
				}
				else
				{
					stateAction = StateAction.Stale;
				}
			}
			return stateAction;
		}

		protected virtual float GetEffectiveTargetFrameRate()
		{
			return AdaptivePerformanceManager.EffectiveTargetFrameRate();
		}

		private Queue<float> m_Samples;

		private int m_SampleCapacity;
	}
}
