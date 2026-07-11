using System;

namespace UnityEngine
{
	public class WaitForSecondsRealtime : CustomYieldInstruction
	{
		public WaitForSecondsRealtime(float time)
		{
			this.waitTime = time;
		}

		public float waitTime { get; set; }

		public override bool keepWaiting
		{
			get
			{
				if (this.m_WaitUntilTime < 0f)
				{
					this.m_WaitUntilTime = Time.realtimeSinceStartup + this.waitTime;
				}
				bool flag = Time.realtimeSinceStartup < this.m_WaitUntilTime;
				if (!flag)
				{
					this.m_WaitUntilTime = -1f;
				}
				return flag;
			}
		}

		private float m_WaitUntilTime = -1f;
	}
}
