using System;
using UnityEngine.Audio;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
	[Serializable]
	internal class AudioMixerProperties : PlayableBehaviour
	{
		public override void PrepareFrame(Playable playable, FrameData info)
		{
			if (!playable.IsValid<Playable>() || !playable.IsPlayableOfType<AudioMixerPlayable>())
			{
				return;
			}
			int inputCount = playable.GetInputCount<Playable>();
			for (int i = 0; i < inputCount; i++)
			{
				if (playable.GetInputWeight(i) > 0f)
				{
					Playable input = playable.GetInput(i);
					if (input.IsValid<Playable>() && input.IsPlayableOfType<AudioClipPlayable>())
					{
						AudioClipPlayable audioClipPlayable = (AudioClipPlayable)input;
						AudioClipProperties @object = input.GetHandle().GetObject<AudioClipProperties>();
						audioClipPlayable.SetVolume(Mathf.Clamp01(this.volume * @object.volume));
						audioClipPlayable.SetStereoPan(Mathf.Clamp(this.stereoPan, -1f, 1f));
						audioClipPlayable.SetSpatialBlend(Mathf.Clamp01(this.spatialBlend));
					}
				}
			}
		}

		[Range(0f, 1f)]
		public float volume = 1f;

		[Range(-1f, 1f)]
		public float stereoPan;

		[Range(0f, 1f)]
		public float spatialBlend;
	}
}
