using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Suspends the coroutine execution for the given amount of seconds using unscaled time.</para>
	/// </summary>
	public class WaitForSecondsRealtime : CustomYieldInstruction
	{
		/// <summary>
		///   <para>Creates a yield instruction to wait for a given number of seconds using unscaled time.</para>
		/// </summary>
		/// <param name="time"></param>
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
