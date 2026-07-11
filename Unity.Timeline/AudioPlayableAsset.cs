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
				if (this.m_Clip == null)
				{
					return base.duration;
				}
				return (double)this.m_Clip.samples / (double)this.m_Clip.frequency;
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
			if (this.m_Clip == null)
			{
				return Playable.Null;
			}
			AudioClipPlayable audioClipPlayable = AudioClipPlayable.Create(graph, this.m_Clip, this.m_Loop);
			audioClipPlayable.GetHandle().SetScriptInstance(this.m_ClipProperties.Clone());
			return audioClipPlayable;
		}

		public ClipCaps clipCaps
		{
			get
			{
				return ClipCaps.ClipIn | ClipCaps.SpeedMultiplier | ClipCaps.Blending | (this.m_Loop ? ClipCaps.Looping : ClipCaps.None);
			}
		}

		[SerializeField]
		private AudioClip m_Clip;

		[SerializeField]
		private bool m_Loop;

		[SerializeField]
		[HideInInspector]
		private float m_bufferingTime = 0.1f;

		[SerializeField]
		private AudioClipProperties m_ClipProperties = new AudioClipProperties();
	}
}
