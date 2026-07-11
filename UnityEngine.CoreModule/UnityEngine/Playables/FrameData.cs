using System;

namespace UnityEngine.Playables
{
	public struct FrameData
	{
		private bool HasFlags(FrameData.Flags flag)
		{
			return (this.m_Flags & flag) == flag;
		}

		public ulong frameId
		{
			get
			{
				return this.m_FrameID;
			}
		}

		public float deltaTime
		{
			get
			{
				return (float)this.m_DeltaTime;
			}
		}

		public float weight
		{
			get
			{
				return this.m_Weight;
			}
		}

		public float effectiveWeight
		{
			get
			{
				return this.m_EffectiveWeight;
			}
		}

		public double effectiveParentDelay
		{
			get
			{
				return this.m_EffectiveParentDelay;
			}
		}

		public float effectiveParentSpeed
		{
			get
			{
				return this.m_EffectiveParentSpeed;
			}
		}

		public float effectiveSpeed
		{
			get
			{
				return this.m_EffectiveSpeed;
			}
		}

		public FrameData.EvaluationType evaluationType
		{
			get
			{
				return (!this.HasFlags(FrameData.Flags.Evaluate)) ? FrameData.EvaluationType.Playback : FrameData.EvaluationType.Evaluate;
			}
		}

		public bool seekOccurred
		{
			get
			{
				return this.HasFlags(FrameData.Flags.SeekOccured);
			}
		}

		public bool timeLooped
		{
			get
			{
				return this.HasFlags(FrameData.Flags.Loop);
			}
		}

		public bool timeHeld
		{
			get
			{
				return this.HasFlags(FrameData.Flags.Hold);
			}
		}

		public PlayableOutput output
		{
			get
			{
				return this.m_Output;
			}
		}

		public PlayState effectivePlayState
		{
			get
			{
				PlayState playState;
				if (this.HasFlags(FrameData.Flags.EffectivePlayStateDelayed))
				{
					playState = PlayState.Delayed;
				}
				else if (this.HasFlags(FrameData.Flags.EffectivePlayStatePlaying))
				{
					playState = PlayState.Playing;
				}
				else
				{
					playState = PlayState.Paused;
				}
				return playState;
			}
		}

		internal ulong m_FrameID;

		internal double m_DeltaTime;

		internal float m_Weight;

		internal float m_EffectiveWeight;

		internal double m_EffectiveParentDelay;

		internal float m_EffectiveParentSpeed;

		internal float m_EffectiveSpeed;

		internal FrameData.Flags m_Flags;

		internal PlayableOutput m_Output;

		[Flags]
		internal enum Flags
		{
			Evaluate = 1,
			SeekOccured = 2,
			Loop = 4,
			Hold = 8,
			EffectivePlayStateDelayed = 16,
			EffectivePlayStatePlaying = 32
		}

		public enum EvaluationType
		{
			Evaluate,
			Playback
		}
	}
}
