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
			this.m_SystemTime = 0f;
			this.SetRandomSeed();
		}

		private void SetRandomSeed()
		{
			this.particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
			ParticleSystem[] componentsInChildren = this.particleSystem.gameObject.GetComponentsInChildren<ParticleSystem>();
			uint num = this.m_RandomSeed;
			foreach (ParticleSystem particleSystem in componentsInChildren)
			{
				if (particleSystem.useAutoRandomSeed)
				{
					particleSystem.useAutoRandomSeed = false;
					particleSystem.randomSeed = num;
					num += 1U;
				}
			}
		}

		public override void PrepareFrame(Playable playable, FrameData data)
		{
			if (this.particleSystem == null || !this.particleSystem.gameObject.activeInHierarchy)
			{
				return;
			}
			float num = (float)playable.GetTime<Playable>();
			if (Mathf.Approximately(this.m_LastTime, -1f) || !Mathf.Approximately(this.m_LastTime, num))
			{
				float num2 = Time.fixedDeltaTime * 0.5f;
				float num3 = num;
				float num4 = num3 - this.m_LastTime;
				float num5 = this.particleSystem.main.startDelay.Evaluate(this.particleSystem.randomSeed);
				float num6 = this.particleSystem.main.duration + num5;
				float num7 = ((num3 > num6) ? this.m_SystemTime : (this.m_SystemTime - num5));
				if (num3 < this.m_LastTime || num3 < num2 || Mathf.Approximately(this.m_LastTime, -1f) || num4 > this.particleSystem.main.duration || Mathf.Abs(num7 - this.particleSystem.time) >= Time.maximumParticleDeltaTime)
				{
					this.particleSystem.Simulate(0f, true, true);
					this.particleSystem.Simulate(num3, true, false);
					this.m_SystemTime = num3;
				}
				else
				{
					float num8 = ((num3 > num6) ? this.particleSystem.main.duration : num6);
					float num9 = num3 % num8;
					float num10 = num9 - this.m_SystemTime;
					if (num10 < -num2)
					{
						num10 = num9 + num6 - this.m_SystemTime;
					}
					this.particleSystem.Simulate(num10, true, false);
					this.m_SystemTime += num10;
				}
				this.m_LastTime = num;
			}
		}

		public override void OnBehaviourPlay(Playable playable, FrameData info)
		{
			this.m_LastTime = -1f;
		}

		public override void OnBehaviourPause(Playable playable, FrameData info)
		{
			this.m_LastTime = -1f;
		}

		private const float kUnsetTime = -1f;

		private float m_LastTime = -1f;

		private uint m_RandomSeed = 1U;

		private float m_SystemTime;
	}
}
