using System;

namespace UnityEngine.Experimental.UIElements
{
	public struct TimerState
	{
		public long deltaTime
		{
			get
			{
				return this.now - this.start;
			}
		}

		public long start;

		public long now;
	}
}
