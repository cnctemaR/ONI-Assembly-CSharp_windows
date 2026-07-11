using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>What action to perform when the particle trigger module passes a test.</para>
	/// </summary>
	public enum ParticleSystemOverlapAction
	{
		/// <summary>
		///   <para>Do nothing.</para>
		/// </summary>
		Ignore,
		/// <summary>
		///   <para>Kill all particles that pass this test.</para>
		/// </summary>
		Kill,
		/// <summary>
		///   <para>Send the OnParticleTrigger command to the particle system's script.</para>
		/// </summary>
		Callback
	}
}
