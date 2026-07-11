using System;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
	[TrackClipType(typeof(AudioPlayableAsset), false)]
	[TrackBindingType(typeof(AudioSource))]
	[Serializable]
	public class AudioTrack : TrackAsset
	{
		public TimelineClip CreateClip(AudioClip clip)
		{
			TimelineClip timelineClip;
			if (clip == null)
			{
				timelineClip = null;
			}
			else
			{
				TimelineClip timelineClip2 = base.CreateDefaultClip();
				AudioPlayableAsset audioPlayableAsset = timelineClip2.asset as AudioPlayableAsset;
				if (audioPlayableAsset != null)
				{
					audioPlayableAsset.clip = clip;
				}
				timelineClip2.duration = (double)clip.length;
				timelineClip2.displayName = clip.name;
				timelineClip = timelineClip2;
			}
			return timelineClip;
		}

		internal override Playable OnCreatePlayableGraph(PlayableGraph graph, GameObject go, IntervalTree<RuntimeElement> tree)
		{
			AudioMixerPlayable audioMixerPlayable = AudioMixerPlayable.Create(graph, base.clips.Length, false);
			for (int i = 0; i < base.clips.Length; i++)
			{
				TimelineClip timelineClip = base.clips[i];
				PlayableAsset playableAsset = timelineClip.asset as PlayableAsset;
				if (!(playableAsset == null))
				{
					float num = 0.1f;
					AudioPlayableAsset audioPlayableAsset = timelineClip.asset as AudioPlayableAsset;
					if (audioPlayableAsset != null)
					{
						num = audioPlayableAsset.bufferingTime;
					}
					Playable playable = playableAsset.CreatePlayable(graph, go);
					if (playable.IsValid<Playable>())
					{
						tree.Add(new ScheduleRuntimeClip(timelineClip, playable, audioMixerPlayable, (double)num, 0.1));
						graph.Connect<Playable, AudioMixerPlayable>(playable, 0, audioMixerPlayable, i);
						playable.SetSpeed(timelineClip.timeScale);
						playable.SetDuration(timelineClip.extrapolatedDuration);
						audioMixerPlayable.SetInputWeight(playable, 1f);
					}
				}
			}
			return audioMixerPlayable;
		}

		public override IEnumerable<PlayableBinding> outputs
		{
			get
			{
				yield return AudioPlayableBinding.Create(base.name, this);
				yield break;
			}
		}
	}
}
