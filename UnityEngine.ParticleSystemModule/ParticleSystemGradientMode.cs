using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>The particle gradient mode (Shuriken).</para>
	/// </summary>
	public enum ParticleSystemGradientMode
	{
		/// <summary>
		///   <para>Use a single color for the ParticleSystem.MinMaxGradient.</para>
		/// </summary>
		Color,
		/// <summary>
		///   <para>Use a single color gradient for the ParticleSystem.MinMaxGradient.</para>
		/// </summary>
		Gradient,
		/// <summary>
		///   <para>Use a random value between 2 colors for the ParticleSystem.MinMaxGradient.</para>
		/// </summary>
		TwoColors,
		/// <summary>
		///   <para>Use a random value between 2 color gradients for the ParticleSystem.MinMaxGradient.</para>
		/// </summary>
		TwoGradients,
		/// <summary>
		///   <para>Define a list of colors in the ParticleSystem.MinMaxGradient, to be chosen from at random.</para>
		/// </summary>
		RandomColor
	}
}
