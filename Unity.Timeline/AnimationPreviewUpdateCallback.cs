using System;
using System.Collections.Generic;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
	internal class AnimationPreviewUpdateCallback : ITimelineEvaluateCallback
	{
		public AnimationPreviewUpdateCallback(AnimationPlayableOutput output)
		{
			this.m_Output = output;
			Playable sourcePlayable = this.m_Output.GetSourcePlayable<AnimationPlayableOutput>();
			if (sourcePlayable.IsValid<Playable>())
			{
				this.m_Graph = sourcePlayable.GetGraph<Playable>();
			}
		}

		public void Evaluate()
		{
			if (!this.m_Graph.IsValid())
			{
				return;
			}
			if (this.m_PreviewComponents == null)
			{
				this.FetchPreviewComponents();
			}
			foreach (IAnimationWindowPreview animationWindowPreview in this.m_PreviewComponents)
			{
				if (animationWindowPreview != null)
				{
					animationWindowPreview.UpdatePreviewGraph(this.m_Graph);
				}
			}
		}

		private void FetchPreviewComponents()
		{
			this.m_PreviewComponents = new List<IAnimationWindowPreview>();
			Animator target = this.m_Output.GetTarget();
			if (target == null)
			{
				return;
			}
			GameObject gameObject = target.gameObject;
			this.m_PreviewComponents.AddRange(gameObject.GetComponents<IAnimationWindowPreview>());
		}

		private AnimationPlayableOutput m_Output;

		private PlayableGraph m_Graph;

		private List<IAnimationWindowPreview> m_PreviewComponents;
	}
}
