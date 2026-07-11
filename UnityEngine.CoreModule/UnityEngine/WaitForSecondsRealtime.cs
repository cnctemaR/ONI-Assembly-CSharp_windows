using System;

namespace UnityEngine
{
	public class WaitForSecondsRealtime : CustomYieldInstruction
	{
		public float waitTime { get; set; }

		public override bool keepWaiting
		{
			get
			{
				bool flag = this.m_WaitUntilTime < 0f;
				if (flag)
				{
					this.m_WaitUntilTime = Time.realtimeSinceStartup + this.waitTime;
				}
				bool flag2 = Time.realtimeSinceStartup < this.m_WaitUntilTime;
				bool flag3 = !flag2;
				if (flag3)
				{
					this.m_WaitUntilTime = -1f;
				}
				return flag2;
			}
		}

		public WaitForSecondsRealtime(float time)
		{
			this.waitTime = time;
		}

		private float m_WaitUntilTime = -1f;
	}
}
