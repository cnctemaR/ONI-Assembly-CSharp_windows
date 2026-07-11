using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>The behavior to apply when calling ParticleSystem.Stop|Stop.</para>
	/// </summary>
	public enum ParticleSystemStopBehavior
	{
		/// <summary>
		///   <para>Stops particle system emitting and removes all existing emitted particles.</para>
		/// </summary>
		StopEmittingAndClear,
		/// <summary>
		///   <para>Stops particle system emitting any further particles. All existing particles will remain until they expire.</para>
		/// </summary>
		StopEmitting
	}
}
