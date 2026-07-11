using System;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
	internal class ActivationMixerPlayable : PlayableBehaviour
	{
		public static ScriptPlayable<ActivationMixerPlayable> Create(PlayableGraph graph, int inputCount)
		{
			return ScriptPlayable<ActivationMixerPlayable>.Create(graph, inputCount);
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
			}
		}

		public override void OnPlayableDestroy(Playable playable)
		{
			if (this.m_BoundGameObject == null)
			{
				return;
			}
			switch (this.m_PostPlaybackState)
			{
			case ActivationTrack.PostPlaybackState.Active:
				this.m_BoundGameObject.SetActive(true);
				return;
			case ActivationTrack.PostPlaybackState.Inactive:
				this.m_BoundGameObject.SetActive(false);
				return;
			case ActivationTrack.PostPlaybackState.Revert:
				this.m_BoundGameObject.SetActive(this.m_BoundGameObjectInitialStateIsActive);
				break;
			case ActivationTrack.PostPlaybackState.LeaveAsIs:
				break;
			default:
				return;
			}
		}

		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
			if (this.m_BoundGameObject == null)
			{
				this.m_BoundGameObject = playerData as GameObject;
				this.m_BoundGameObjectInitialStateIsActive = this.m_BoundGameObject != null && this.m_BoundGameObject.activeSelf;
			}
			if (this.m_BoundGameObject == null)
			{
				return;
			}
			int inputCount = playable.GetInputCount<Playable>();
			bool flag = false;
			for (int i = 0; i < inputCount; i++)
			{
				if (playable.GetInputWeight(i) > 0f)
				{
					flag = true;
					break;
				}
			}
			this.m_BoundGameObject.SetActive(flag);
		}

		private ActivationTrack.PostPlaybackState m_PostPlaybackState;

		private bool m_BoundGameObjectInitialStateIsActive;

		private GameObject m_BoundGameObject;
	}
}
