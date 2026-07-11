using System;

namespace UnityEngine.Experimental.Animations
{
	public interface IAnimationJob
	{
		/// <summary>
		///   <para>Defines what to do when processing the animation.</para>
		/// </summary>
		/// <param name="stream">The animation stream to work on.</param>
		void ProcessAnimation(AnimationStream stream);

		/// <summary>
		///   <para>Defines what to do when processing the root motion.</para>
		/// </summary>
		/// <param name="stream">The animation stream to work on.</param>
		void ProcessRootMotion(AnimationStream stream);
	}
}
