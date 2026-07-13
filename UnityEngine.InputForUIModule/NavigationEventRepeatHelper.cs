using System;
using Unity.IntegerTime;

namespace UnityEngine.InputForUI
{
	internal class NavigationEventRepeatHelper
	{
		public void Reset()
		{
			this.m_ConsecutiveMoveCount = 0;
			this.m_LastDirection = NavigationEvent.Direction.None;
			this.m_PrevActionTime = DiscreteTime.Zero;
		}

		public bool ShouldSendMoveEvent(DiscreteTime timestamp, NavigationEvent.Direction direction, bool axisButtonsWherePressedThisFrame)
		{
			bool flag = axisButtonsWherePressedThisFrame || direction != this.m_LastDirection || timestamp > this.m_PrevActionTime + ((this.m_ConsecutiveMoveCount == 1) ? this.m_InitialRepeatDelay : this.m_ConsecutiveRepeatDelay);
			bool flag2;
			if (flag)
			{
				this.m_ConsecutiveMoveCount = ((direction == this.m_LastDirection) ? (this.m_ConsecutiveMoveCount + 1) : 1);
				this.m_LastDirection = direction;
				this.m_PrevActionTime = timestamp;
				flag2 = true;
			}
			else
			{
				flag2 = false;
			}
			return flag2;
		}

		private int m_ConsecutiveMoveCount;

		private NavigationEvent.Direction m_LastDirection;

		private DiscreteTime m_PrevActionTime;

		private readonly DiscreteTime m_InitialRepeatDelay = new DiscreteTime(0.5f);

		private readonly DiscreteTime m_ConsecutiveRepeatDelay = new DiscreteTime(0.1f);
	}
}
