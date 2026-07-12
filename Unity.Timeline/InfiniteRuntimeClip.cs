using System;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
	internal class InfiniteRuntimeClip : RuntimeElement
	{
		public InfiniteRuntimeClip(Playable playable)
		{
			this.m_Playable = playable;
		}

		public override long intervalStart
		{
			get
			{
				return 0L;
			}
		}

		public override long intervalEnd
		{
			get
			{
				return InfiniteRuntimeClip.kIntervalEnd;
			}
		}

		public override bool enable
		{
			set
			{
				if (value)
				{
					this.m_Playable.Play<Playable>();
					return;
				}
				this.m_Playable.Pause<Playable>();
			}
		}

		public override void EvaluateAt(double localTime, FrameData frameData)
		{
			this.m_Playable.SetTime(localTime);
		}

		public override void DisableAt(double localTime, double rootDuration, FrameData frameData)
		{
			this.m_Playable.SetTime(localTime);
			this.enable = false;
		}

		private Playable m_Playable;

		private static readonly long kIntervalEnd = DiscreteTime.GetNearestTick(TimelineClip.kMaxTimeValue);
	}
}
