using System;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
	internal class RuntimeClip : RuntimeClipBase
	{
		public RuntimeClip(TimelineClip clip, Playable clipPlayable, Playable parentMixer)
		{
			this.Create(clip, clipPlayable, parentMixer);
		}

		public override double start
		{
			get
			{
				return this.m_Clip.extrapolatedStart;
			}
		}

		public override double duration
		{
			get
			{
				return this.m_Clip.extrapolatedDuration;
			}
		}

		private void Create(TimelineClip clip, Playable clipPlayable, Playable parentMixer)
		{
			this.m_Clip = clip;
			this.m_Playable = clipPlayable;
			this.m_ParentMixer = parentMixer;
			clipPlayable.Pause<Playable>();
		}

		public TimelineClip clip
		{
			get
			{
				return this.m_Clip;
			}
		}

		public Playable mixer
		{
			get
			{
				return this.m_ParentMixer;
			}
		}

		public Playable playable
		{
			get
			{
				return this.m_Playable;
			}
		}

		public override bool enable
		{
			set
			{
				if (value && this.m_Playable.GetPlayState<Playable>() != PlayState.Playing)
				{
					this.m_Playable.Play<Playable>();
				}
				else if (!value && this.m_Playable.GetPlayState<Playable>() != PlayState.Paused)
				{
					this.m_Playable.Pause<Playable>();
					if (this.m_ParentMixer.IsValid<Playable>())
					{
						this.m_ParentMixer.SetInputWeight(this.m_Playable, 0f);
					}
				}
			}
		}

		public void SetTime(double time)
		{
			this.m_Playable.SetTime(time);
		}

		public void SetDuration(double duration)
		{
			this.m_Playable.SetDuration(duration);
		}

		public override void EvaluateAt(double localTime, FrameData frameData)
		{
			this.enable = true;
			float num;
			if (this.clip.IsPreExtrapolatedTime(localTime))
			{
				num = this.clip.EvaluateMixIn((double)((float)this.clip.start));
			}
			else if (this.clip.IsPostExtrapolatedTime(localTime))
			{
				num = this.clip.EvaluateMixOut((double)((float)this.clip.end));
			}
			else
			{
				num = this.clip.EvaluateMixIn(localTime) * this.clip.EvaluateMixOut(localTime);
			}
			if (this.mixer.IsValid<Playable>())
			{
				this.mixer.SetInputWeight(this.playable, num);
			}
			double num2 = this.clip.ToLocalTime(localTime);
			if (num2.CompareTo(0.0) >= 0)
			{
				this.SetTime(num2);
			}
			this.SetDuration(this.clip.extrapolatedDuration);
		}

		private TimelineClip m_Clip;

		private Playable m_Playable;

		private Playable m_ParentMixer;
	}
}
