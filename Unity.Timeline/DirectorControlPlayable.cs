using System;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
	public class DirectorControlPlayable : PlayableBehaviour
	{
		public static ScriptPlayable<DirectorControlPlayable> Create(PlayableGraph graph, PlayableDirector director)
		{
			if (director == null)
			{
				return ScriptPlayable<DirectorControlPlayable>.Null;
			}
			ScriptPlayable<DirectorControlPlayable> scriptPlayable = ScriptPlayable<DirectorControlPlayable>.Create(graph, 0);
			scriptPlayable.GetBehaviour().director = director;
			return scriptPlayable;
		}

		public override void OnPlayableDestroy(Playable playable)
		{
			if (this.director != null && this.director.playableAsset != null)
			{
				this.director.Stop();
			}
		}

		public override void PrepareFrame(Playable playable, FrameData info)
		{
			if (this.director == null || !this.director.isActiveAndEnabled || this.director.playableAsset == null)
			{
				return;
			}
			this.m_SyncTime |= info.evaluationType == FrameData.EvaluationType.Evaluate || this.DetectDiscontinuity(playable, info);
			this.SyncSpeed((double)info.effectiveSpeed);
			this.SyncStart(playable.GetGraph<Playable>(), playable.GetTime<Playable>());
		}

		public override void OnBehaviourPlay(Playable playable, FrameData info)
		{
			this.m_SyncTime = true;
			if (this.director != null && this.director.playableAsset != null)
			{
				this.m_AssetDuration = this.director.playableAsset.duration;
			}
		}

		public override void OnBehaviourPause(Playable playable, FrameData info)
		{
			if (this.director != null && this.director.playableAsset != null)
			{
				if (info.effectivePlayState == PlayState.Playing)
				{
					this.director.Pause();
					return;
				}
				this.director.Stop();
			}
		}

		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
			if (this.director == null || !this.director.isActiveAndEnabled || this.director.playableAsset == null)
			{
				return;
			}
			if (this.m_SyncTime || this.DetectOutOfSync(playable))
			{
				this.UpdateTime(playable);
				if (this.director.playableGraph.IsValid())
				{
					this.director.playableGraph.Evaluate();
					this.director.playableGraph.SynchronizeEvaluation(playable.GetGraph<Playable>());
				}
				else
				{
					this.director.Evaluate();
				}
			}
			this.m_SyncTime = false;
			this.SyncStop(playable.GetGraph<Playable>(), playable.GetTime<Playable>());
		}

		private void SyncSpeed(double speed)
		{
			if (this.director.playableGraph.IsValid())
			{
				int rootPlayableCount = this.director.playableGraph.GetRootPlayableCount();
				for (int i = 0; i < rootPlayableCount; i++)
				{
					Playable rootPlayable = this.director.playableGraph.GetRootPlayable(i);
					if (rootPlayable.IsValid<Playable>())
					{
						rootPlayable.SetSpeed(speed);
					}
				}
			}
		}

		private void SyncStart(PlayableGraph graph, double time)
		{
			if (this.director.state == PlayState.Playing || !graph.IsPlaying() || (this.director.extrapolationMode == DirectorWrapMode.None && time > this.m_AssetDuration))
			{
				return;
			}
			if (graph.IsMatchFrameRateEnabled())
			{
				this.director.Play(graph.GetFrameRate());
				return;
			}
			this.director.Play();
		}

		private void SyncStop(PlayableGraph graph, double time)
		{
			if (this.director.state == PlayState.Paused || (graph.IsPlaying() && (this.director.extrapolationMode != DirectorWrapMode.None || time < this.m_AssetDuration)))
			{
				return;
			}
			if (this.director.state == PlayState.Paused)
			{
				return;
			}
			if ((this.director.extrapolationMode == DirectorWrapMode.None && time > this.m_AssetDuration) || !graph.IsPlaying())
			{
				this.director.Pause();
			}
		}

		private bool DetectDiscontinuity(Playable playable, FrameData info)
		{
			return Math.Abs(playable.GetTime<Playable>() - playable.GetPreviousTime<Playable>() - info.m_DeltaTime * (double)info.m_EffectiveSpeed) > DiscreteTime.tickValue;
		}

		private bool DetectOutOfSync(Playable playable)
		{
			double num = playable.GetTime<Playable>();
			if (playable.GetTime<Playable>() >= this.m_AssetDuration)
			{
				switch (this.director.extrapolationMode)
				{
				case DirectorWrapMode.Hold:
					num = this.m_AssetDuration;
					break;
				case DirectorWrapMode.Loop:
					num %= this.m_AssetDuration;
					break;
				case DirectorWrapMode.None:
					num = this.m_AssetDuration;
					break;
				}
			}
			return !Mathf.Approximately((float)num, (float)this.director.time);
		}

		private void UpdateTime(Playable playable)
		{
			double num = Math.Max(0.1, this.director.playableAsset.duration);
			switch (this.director.extrapolationMode)
			{
			case DirectorWrapMode.Hold:
				this.director.time = Math.Min(num, Math.Max(0.0, playable.GetTime<Playable>()));
				return;
			case DirectorWrapMode.Loop:
				this.director.time = Math.Max(0.0, playable.GetTime<Playable>() % num);
				return;
			case DirectorWrapMode.None:
				this.director.time = Math.Min(num, Math.Max(0.0, playable.GetTime<Playable>()));
				return;
			default:
				return;
			}
		}

		public PlayableDirector director;

		private bool m_SyncTime;

		private double m_AssetDuration = double.MaxValue;
	}
}
