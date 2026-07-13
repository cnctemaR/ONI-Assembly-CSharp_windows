using System;

namespace UnityEngine.AdaptivePerformance
{
	public class AdaptiveFramerate : AdaptivePerformanceScaler
	{
		protected override void Awake()
		{
			base.Awake();
			this.m_FirstTimeStart = 0;
			bool flag = this.m_Settings == null;
			if (!flag)
			{
				base.ApplyDefaultSetting(this.m_Settings.scalerSettings.AdaptiveFramerate);
			}
		}

		protected override void OnDisabled()
		{
			bool flag = this.m_FirstTimeStart < 2;
			if (flag)
			{
				this.m_FirstTimeStart++;
			}
			else
			{
				Application.targetFrameRate = this.m_DefaultFPS;
			}
		}

		protected override void OnEnabled()
		{
			bool flag = this.m_FirstTimeStart < 2;
			if (!flag)
			{
				this.m_DefaultFPS = Application.targetFrameRate;
				Application.targetFrameRate = (int)this.MaxBound;
			}
		}

		protected override void OnLevelIncrease()
		{
			base.OnLevelIncrease();
			int num = 1;
			bool flag = Holder.Instance.Indexer.PerformanceAction == StateAction.FastDecrease;
			if (flag)
			{
				num = 5;
			}
			int num2 = Application.targetFrameRate - num;
			bool flag2 = (float)num2 >= this.MinBound && (float)num2 <= this.MaxBound;
			if (flag2)
			{
				Application.targetFrameRate = num2;
			}
		}

		protected override void OnLevelDecrease()
		{
			base.OnLevelDecrease();
			int num = Application.targetFrameRate + 5;
			bool flag = (float)num >= this.MinBound && (float)num <= this.MaxBound;
			if (flag)
			{
				Application.targetFrameRate = num;
			}
		}

		private int m_DefaultFPS;

		private int m_FirstTimeStart = 0;
	}
}
