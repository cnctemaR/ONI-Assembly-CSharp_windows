using System;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
	[TrackClipType(typeof(ActivationPlayableAsset))]
	[TrackBindingType(typeof(GameObject))]
	[Serializable]
	public class ActivationTrack : TrackAsset
	{
		internal override bool CanCompileClips()
		{
			return !base.hasClips || base.CanCompileClips();
		}

		public ActivationTrack.PostPlaybackState postPlaybackState
		{
			get
			{
				return this.m_PostPlaybackState;
			}
			set
			{
				this.m_PostPlaybackState = value;
				this.UpdateTrackMode();
			}
		}

		public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
		{
			ScriptPlayable<ActivationMixerPlayable> scriptPlayable = ActivationMixerPlayable.Create(graph, inputCount);
			this.m_ActivationMixer = scriptPlayable.GetBehaviour();
			this.UpdateTrackMode();
			return scriptPlayable;
		}

		internal void UpdateTrackMode()
		{
			if (this.m_ActivationMixer != null)
			{
				this.m_ActivationMixer.postPlaybackState = this.m_PostPlaybackState;
			}
		}

		public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
		{
			GameObject gameObjectBinding = base.GetGameObjectBinding(director);
			if (gameObjectBinding != null)
			{
				driver.AddFromName(gameObjectBinding, "m_IsActive");
			}
		}

		protected override void OnCreateClip(TimelineClip clip)
		{
			clip.displayName = "Active";
			base.OnCreateClip(clip);
		}

		[SerializeField]
		private ActivationTrack.PostPlaybackState m_PostPlaybackState = ActivationTrack.PostPlaybackState.LeaveAsIs;

		private ActivationMixerPlayable m_ActivationMixer;

		public enum PostPlaybackState
		{
			Active,
			Inactive,
			Revert,
			LeaveAsIs
		}
	}
}
