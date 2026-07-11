using System;
using System.Collections.Generic;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
	internal class AnimationOutputWeightProcessor : ITimelineEvaluateCallback
	{
		public AnimationOutputWeightProcessor(AnimationPlayableOutput output)
		{
			this.m_Output = output;
			output.SetWeight(0f);
			this.FindMixers();
		}

		private void FindMixers()
		{
			this.m_Mixers.Clear();
			this.m_PoseMixer = AnimationMixerPlayable.Null;
			this.m_LayerMixer = AnimationLayerMixerPlayable.Null;
			this.m_MotionXPlayable = AnimationMotionXToDeltaPlayable.Null;
			Playable sourcePlayable = this.m_Output.GetSourcePlayable<AnimationPlayableOutput>();
			int sourceOutputPort = this.m_Output.GetSourceOutputPort<AnimationPlayableOutput>();
			if (sourcePlayable.IsValid<Playable>() && sourceOutputPort >= 0 && sourceOutputPort < sourcePlayable.GetInputCount<Playable>())
			{
				Playable playable = sourcePlayable.GetInput(sourceOutputPort);
				Playable playable2 = playable;
				if (playable2.IsPlayableOfType<AnimationMotionXToDeltaPlayable>())
				{
					this.m_MotionXPlayable = (AnimationMotionXToDeltaPlayable)playable2;
					playable = this.m_MotionXPlayable.GetInput(0);
				}
				if (playable.IsValid<Playable>() && playable.IsPlayableOfType<AnimationMixerPlayable>())
				{
					this.m_PoseMixer = (AnimationMixerPlayable)playable;
					Playable input = this.m_PoseMixer.GetInput(0);
					if (input.IsValid<Playable>() && input.IsPlayableOfType<AnimationLayerMixerPlayable>())
					{
						this.m_LayerMixer = (AnimationLayerMixerPlayable)input;
					}
				}
				else if (playable.IsValid<Playable>() && playable.IsPlayableOfType<AnimationLayerMixerPlayable>())
				{
					this.m_LayerMixer = (AnimationLayerMixerPlayable)playable;
				}
				if (this.m_LayerMixer.IsValid<AnimationLayerMixerPlayable>())
				{
					int inputCount = this.m_LayerMixer.GetInputCount<AnimationLayerMixerPlayable>();
					for (int i = 0; i < inputCount; i++)
					{
						this.FindMixers(this.m_LayerMixer, i, this.m_LayerMixer.GetInput(i));
					}
				}
			}
		}

		private void FindMixers(Playable parent, int port, Playable node)
		{
			if (node.IsValid<Playable>())
			{
				Type playableType = node.GetPlayableType();
				if (playableType == typeof(AnimationMixerPlayable) || playableType == typeof(AnimationLayerMixerPlayable))
				{
					int inputCount = node.GetInputCount<Playable>();
					for (int i = 0; i < inputCount; i++)
					{
						this.FindMixers(node, i, node.GetInput(i));
					}
					AnimationOutputWeightProcessor.WeightInfo weightInfo = new AnimationOutputWeightProcessor.WeightInfo
					{
						parentMixer = parent,
						mixer = node,
						port = port,
						modulate = (playableType == typeof(AnimationLayerMixerPlayable))
					};
					this.m_Mixers.Add(weightInfo);
				}
				else
				{
					int inputCount2 = node.GetInputCount<Playable>();
					for (int j = 0; j < inputCount2; j++)
					{
						this.FindMixers(parent, port, node.GetInput(j));
					}
				}
			}
		}

		public void Evaluate()
		{
			this.m_Output.SetWeight(1f);
			for (int i = 0; i < this.m_Mixers.Count; i++)
			{
				AnimationOutputWeightProcessor.WeightInfo weightInfo = this.m_Mixers[i];
				float num = ((!weightInfo.modulate) ? 1f : weightInfo.parentMixer.GetInputWeight(weightInfo.port));
				weightInfo.parentMixer.SetInputWeight(weightInfo.port, num * WeightUtility.NormalizeMixer(weightInfo.mixer));
			}
			float num2 = WeightUtility.NormalizeMixer(this.m_LayerMixer);
			Animator target = this.m_Output.GetTarget();
			if (!(target == null))
			{
				bool flag = !Application.isPlaying && this.m_MotionXPlayable.IsValid<AnimationMotionXToDeltaPlayable>() && this.m_MotionXPlayable.IsAbsoluteMotion();
				if (flag)
				{
					this.m_PoseMixer.SetInputWeight(0, num2);
					this.m_PoseMixer.SetInputWeight(1, 1f - num2);
				}
				else
				{
					if (!this.m_PoseMixer.Equals(AnimationMixerPlayable.Null))
					{
						this.m_PoseMixer.SetInputWeight(0, 1f);
						this.m_PoseMixer.SetInputWeight(1, 0f);
					}
					this.m_Output.SetWeight(num2);
				}
			}
		}

		private AnimationPlayableOutput m_Output;

		private AnimationMotionXToDeltaPlayable m_MotionXPlayable;

		private AnimationMixerPlayable m_PoseMixer;

		private AnimationLayerMixerPlayable m_LayerMixer;

		private readonly List<AnimationOutputWeightProcessor.WeightInfo> m_Mixers = new List<AnimationOutputWeightProcessor.WeightInfo>();

		private struct WeightInfo
		{
			public Playable mixer;

			public Playable parentMixer;

			public int port;

			public bool modulate;
		}
	}
}
