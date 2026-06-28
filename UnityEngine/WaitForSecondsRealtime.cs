using System;

namespace UnityEngine
{
	public class WaitForSecondsRealtime : CustomYieldInstruction
	{
		public WaitForSecondsRealtime(float time)
		{
			this.waitTime = Time.realtimeSinceStartup + time;
		}

		public override bool keepWaiting
		{
			get
			{
				return Time.realtimeSinceStartup < this.waitTime;
			}
		}

		private float waitTime;
	}
}
