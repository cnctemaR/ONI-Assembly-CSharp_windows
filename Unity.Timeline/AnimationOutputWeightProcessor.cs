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
			Playable sourcePlayable = this.m_Output.GetSourcePlayable<AnimationPlayableOutput>();
			int sourceOutputPort = this.m_Output.GetSourceOutputPort<AnimationPlayableOutput>();
			this.m_Mixers.Clear();
			this.FindMixers(sourcePlayable, sourceOutputPort, sourcePlayable.GetInput(sourceOutputPort));
		}

		private void FindMixers(Playable parent, int port, Playable node)
		{
			if (!node.IsValid<Playable>())
			{
				return;
			}
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
					port = port
				};
				this.m_Mixers.Add(weightInfo);
				return;
			}
			int inputCount2 = node.GetInputCount<Playable>();
			for (int j = 0; j < inputCount2; j++)
			{
				this.FindMixers(parent, port, node.GetInput(j));
			}
		}

		public void Evaluate()
		{
			float num = 1f;
			this.m_Output.SetWeight(1f);
			for (int i = 0; i < this.m_Mixers.Count; i++)
			{
				AnimationOutputWeightProcessor.WeightInfo weightInfo = this.m_Mixers[i];
				num = WeightUtility.NormalizeMixer(weightInfo.mixer);
				weightInfo.parentMixer.SetInputWeight(weightInfo.port, num);
			}
			if (Application.isPlaying)
			{
				this.m_Output.SetWeight(num);
			}
		}

		private AnimationPlayableOutput m_Output;

		private AnimationMotionXToDeltaPlayable m_MotionXPlayable;

		private readonly List<AnimationOutputWeightProcessor.WeightInfo> m_Mixers = new List<AnimationOutputWeightProcessor.WeightInfo>();

		private struct WeightInfo
		{
			public Playable mixer;

			public Playable parentMixer;

			public int port;
		}
	}
}
