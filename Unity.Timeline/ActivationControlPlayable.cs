using System;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
	public class ActivationControlPlayable : PlayableBehaviour
	{
		public static ScriptPlayable<ActivationControlPlayable> Create(PlayableGraph graph, GameObject gameObject, ActivationControlPlayable.PostPlaybackState postPlaybackState)
		{
			if (gameObject == null)
			{
				return ScriptPlayable<ActivationControlPlayable>.Null;
			}
			ScriptPlayable<ActivationControlPlayable> scriptPlayable = ScriptPlayable<ActivationControlPlayable>.Create(graph, 0);
			ActivationControlPlayable behaviour = scriptPlayable.GetBehaviour();
			behaviour.gameObject = gameObject;
			behaviour.postPlayback = postPlaybackState;
			return scriptPlayable;
		}

		public override void OnBehaviourPlay(Playable playable, FrameData info)
		{
			if (this.gameObject == null)
			{
				return;
			}
			this.gameObject.SetActive(true);
		}

		public override void OnBehaviourPause(Playable playable, FrameData info)
		{
			if (this.gameObject != null && info.effectivePlayState == PlayState.Paused)
			{
				this.gameObject.SetActive(false);
			}
		}

		public override void ProcessFrame(Playable playable, FrameData info, object userData)
		{
			if (this.gameObject != null)
			{
				this.gameObject.SetActive(true);
			}
		}

		public override void OnGraphStart(Playable playable)
		{
			if (this.gameObject != null && this.m_InitialState == ActivationControlPlayable.InitialState.Unset)
			{
				this.m_InitialState = (this.gameObject.activeSelf ? ActivationControlPlayable.InitialState.Active : ActivationControlPlayable.InitialState.Inactive);
			}
		}

		public override void OnPlayableDestroy(Playable playable)
		{
			if (this.gameObject == null || this.m_InitialState == ActivationControlPlayable.InitialState.Unset)
			{
				return;
			}
			switch (this.postPlayback)
			{
			case ActivationControlPlayable.PostPlaybackState.Active:
				this.gameObject.SetActive(true);
				return;
			case ActivationControlPlayable.PostPlaybackState.Inactive:
				this.gameObject.SetActive(false);
				return;
			case ActivationControlPlayable.PostPlaybackState.Revert:
				this.gameObject.SetActive(this.m_InitialState == ActivationControlPlayable.InitialState.Active);
				return;
			default:
				return;
			}
		}

		public GameObject gameObject;

		public ActivationControlPlayable.PostPlaybackState postPlayback = ActivationControlPlayable.PostPlaybackState.Revert;

		private ActivationControlPlayable.InitialState m_InitialState;

		public enum PostPlaybackState
		{
			Active,
			Inactive,
			Revert
		}

		private enum InitialState
		{
			Unset,
			Active,
			Inactive
		}
	}
}
