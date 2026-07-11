using System;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
	public class DirectorControlPlayable : PlayableBehaviour
	{
		public static ScriptPlayable<DirectorControlPlayable> Create(PlayableGraph graph, PlayableDirector director)
		{
			ScriptPlayable<DirectorControlPlayable> scriptPlayable;
			if (director == null)
			{
				scriptPlayable = ScriptPlayable<DirectorControlPlayable>.Null;
			}
			else
			{
				ScriptPlayable<DirectorControlPlayable> scriptPlayable2 = ScriptPlayable<DirectorControlPlayable>.Create(graph, 0);
				scriptPlayable2.GetBehaviour().director = director;
				scriptPlayable = scriptPlayable2;
			}
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
			if (!(this.director == null) && this.director.isActiveAndEnabled && !(this.director.playableAsset == null))
			{
				this.m_SyncTime |= info.evaluationType == FrameData.EvaluationType.Evaluate || this.DetectDiscontinuity(playable, info);
				this.SyncSpeed((double)info.effectiveSpeed);
				this.SyncPlayState(playable.GetGraph<Playable>(), playable.GetTime<Playable>());
			}
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
				}
				else
				{
					this.director.Stop();
				}
			}
		}

		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
			if (!(this.director == null) && this.director.isActiveAndEnabled && !(this.director.playableAsset == null))
			{
				if (this.m_SyncTime || this.DetectOutOfSync(playable))
				{
					this.UpdateTime(playable);
					this.director.Evaluate();
				}
				this.m_SyncTime = false;
			}
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

		private void SyncPlayState(PlayableGraph graph, double playableTime)
		{
			bool flag = playableTime >= this.m_AssetDuration && this.director.extrapolationMode == DirectorWrapMode.None;
			if (graph.IsPlaying() && !flag)
			{
				this.director.Play();
			}
			else
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
				if (this.director.extrapolationMode == DirectorWrapMode.None)
				{
					return false;
				}
				if (this.director.extrapolationMode == DirectorWrapMode.Hold)
				{
					num = this.m_AssetDuration;
				}
				else if (this.m_AssetDuration > 1.401298464324817E-45)
				{
					num %= this.m_AssetDuration;
				}
			}
			return !Mathf.Approximately((float)num, (float)this.director.time);
		}

		private void UpdateTime(Playable playable)
		{
			double num = Math.Max(0.1, this.director.playableAsset.duration);
			DirectorWrapMode extrapolationMode = this.director.extrapolationMode;
			if (extrapolationMode != DirectorWrapMode.Hold)
			{
				if (extrapolationMode != DirectorWrapMode.Loop)
				{
					if (extrapolationMode == DirectorWrapMode.None)
					{
						this.director.time = playable.GetTime<Playable>();
					}
				}
				else
				{
					this.director.time = Math.Max(0.0, playable.GetTime<Playable>() % num);
				}
			}
			else
			{
				this.director.time = Math.Min(num, Math.Max(0.0, playable.GetTime<Playable>()));
			}
		}

		public PlayableDirector director;

		private bool m_SyncTime = false;

		private double m_AssetDuration = double.MaxValue;
	}
}
