using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>The action to perform when the Particle System stops.</para>
	/// </summary>
	public enum ParticleSystemStopAction
	{
		/// <summary>
		///   <para>Do nothing.</para>
		/// </summary>
		None,
		/// <summary>
		///   <para>Disable the GameObject containing the Particle System.</para>
		/// </summary>
		Disable,
		/// <summary>
		///   <para>Destroy the GameObject containing the Particle System.</para>
		/// </summary>
		Destroy,
		/// <summary>
		///   <para>Call OnParticleSystemStopped on the ParticleSystem script.</para>
		/// </summary>
		Callback
	}
}
