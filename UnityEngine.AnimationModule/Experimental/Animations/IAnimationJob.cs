using System;

namespace UnityEngine.Experimental.Animations
{
	public interface IAnimationJob
	{
		void ProcessAnimation(AnimationStream stream);

		void ProcessRootMotion(AnimationStream stream);
	}
}
