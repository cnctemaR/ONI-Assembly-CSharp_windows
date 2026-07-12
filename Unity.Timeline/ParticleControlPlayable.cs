using System;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
	public class ParticleControlPlayable : PlayableBehaviour
	{
		public static ScriptPlayable<ParticleControlPlayable> Create(PlayableGraph graph, ParticleSystem component, uint randomSeed)
		{
			if (component == null)
			{
				return ScriptPlayable<ParticleControlPlayable>.Null;
			}
			ScriptPlayable<ParticleControlPlayable> scriptPlayable = ScriptPlayable<ParticleControlPlayable>.Create(graph, 0);
			scriptPlayable.GetBehaviour().Initialize(component, randomSeed);
			return scriptPlayable;
		}

		public ParticleSystem particleSystem { get; private set; }

		public void Initialize(ParticleSystem ps, uint randomSeed)
		{
			this.m_RandomSeed = Math.Max(1U, randomSeed);
			this.particleSystem = ps;
			ParticleControlPlayable.SetRandomSeed(this.particleSystem, this.m_RandomSeed);
		}

		private static void SetRandomSeed(ParticleSystem particleSystem, uint randomSeed)
		{
			if (particleSystem == null)
			{
				return;
			}
			particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
			if (particleSystem.useAutoRandomSeed)
			{
				particleSystem.useAutoRandomSeed = false;
				particleSystem.randomSeed = randomSeed;
			}
			for (int i = 0; i < particleSystem.subEmitters.subEmittersCount; i++)
			{
				ParticleControlPlayable.SetRandomSeed(particleSystem.subEmitters.GetSubEmitterSystem(i), randomSeed += 1U);
			}
		}

		public override void PrepareFrame(Playable playable, FrameData data)
		{
			if (this.particleSystem == null || !this.particleSystem.gameObject.activeInHierarchy)
			{
				this.m_LastPlayableTime = float.MaxValue;
				return;
			}
			float num = (float)playable.GetTime<Playable>();
			float time = this.particleSystem.time;
			if (this.m_LastPlayableTime > num || !Mathf.Approximately(time, this.m_LastParticleTime))
			{
				this.Simulate(num, true);
			}
			else if (this.m_LastPlayableTime < num)
			{
				this.Simulate(num - this.m_LastPlayableTime, false);
			}
			this.m_LastPlayableTime = num;
			this.m_LastParticleTime = this.particleSystem.time;
		}

		public override void OnBehaviourPlay(Playable playable, FrameData info)
		{
			this.m_LastPlayableTime = float.MaxValue;
		}

		public override void OnBehaviourPause(Playable playable, FrameData info)
		{
			this.m_LastPlayableTime = float.MaxValue;
		}

		private void Simulate(float time, bool restart)
		{
			float maximumDeltaTime = Time.maximumDeltaTime;
			if (restart)
			{
				this.particleSystem.Simulate(0f, false, true, false);
			}
			while (time > maximumDeltaTime)
			{
				this.particleSystem.Simulate(maximumDeltaTime, false, false, false);
				time -= maximumDeltaTime;
			}
			if (time > 0f)
			{
				this.particleSystem.Simulate(time, false, false, false);
			}
		}

		private const float kUnsetTime = 3.4028235E+38f;

		private float m_LastPlayableTime = float.MaxValue;

		private float m_LastParticleTime = float.MaxValue;

		private uint m_RandomSeed = 1U;
	}
}
