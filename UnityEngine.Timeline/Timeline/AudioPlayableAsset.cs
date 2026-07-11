using System;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
	[Serializable]
	public class AudioPlayableAsset : PlayableAsset, ITimelineClipAsset
	{
		internal float bufferingTime
		{
			get
			{
				return this.m_bufferingTime;
			}
			set
			{
				this.m_bufferingTime = value;
			}
		}

		public AudioClip clip
		{
			get
			{
				return this.m_Clip;
			}
			set
			{
				this.m_Clip = value;
			}
		}

		public bool loop
		{
			get
			{
				return this.m_Loop;
			}
			set
			{
				this.m_Loop = value;
			}
		}

		public override double duration
		{
			get
			{
				double num;
				if (this.m_Clip == null)
				{
					num = base.duration;
				}
				else
				{
					num = (double)this.m_Clip.samples / (double)this.m_Clip.frequency;
				}
				return num;
			}
		}

		public override IEnumerable<PlayableBinding> outputs
		{
			get
			{
				yield return AudioPlayableBinding.Create(base.name, this);
				yield break;
			}
		}

		public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
		{
			Playable playable;
			if (this.m_Clip == null)
			{
				playable = Playable.Null;
			}
			else
			{
				playable = AudioClipPlayable.Create(graph, this.m_Clip, this.m_Loop);
			}
			return playable;
		}

		public ClipCaps clipCaps
		{
			get
			{
				return ClipCaps.ClipIn | ClipCaps.SpeedMultiplier | ClipCaps.Blending | ((!this.m_Loop) ? ClipCaps.None : ClipCaps.Looping);
			}
		}

		[SerializeField]
		private AudioClip m_Clip;

		[SerializeField]
		private bool m_Loop;

		[SerializeField]
		[HideInInspector]
		private float m_bufferingTime = 0.1f;
	}
}
