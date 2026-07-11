using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Which mode the Custom Data module uses to generate its data.</para>
	/// </summary>
	public enum ParticleSystemCustomDataMode
	{
		/// <summary>
		///   <para>Don't generate any data.</para>
		/// </summary>
		Disabled,
		/// <summary>
		///   <para>Generate data using ParticleSystem.MinMaxCurve.</para>
		/// </summary>
		Vector,
		/// <summary>
		///   <para>Generate data using ParticleSystem.MinMaxGradient.</para>
		/// </summary>
		Color
	}
}
