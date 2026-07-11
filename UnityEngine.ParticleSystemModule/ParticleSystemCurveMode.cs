using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>The particle curve mode (Shuriken).</para>
	/// </summary>
	public enum ParticleSystemCurveMode
	{
		/// <summary>
		///   <para>Use a single constant for the ParticleSystem.MinMaxCurve.</para>
		/// </summary>
		Constant,
		/// <summary>
		///   <para>Use a single curve for the ParticleSystem.MinMaxCurve.</para>
		/// </summary>
		Curve,
		/// <summary>
		///   <para>Use a random value between 2 curves for the ParticleSystem.MinMaxCurve.</para>
		/// </summary>
		TwoCurves,
		/// <summary>
		///   <para>Use a random value between 2 constants for the ParticleSystem.MinMaxCurve.</para>
		/// </summary>
		TwoConstants
	}
}
