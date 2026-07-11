using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>The space to simulate particles in.</para>
	/// </summary>
	public enum ParticleSystemSimulationSpace
	{
		/// <summary>
		///   <para>Simulate particles in local space.</para>
		/// </summary>
		Local,
		/// <summary>
		///   <para>Simulate particles in world space.</para>
		/// </summary>
		World,
		/// <summary>
		///   <para>Simulate particles relative to a custom transform component, defined by ParticleSystem.MainModule.customSimulationSpace.</para>
		/// </summary>
		Custom
	}
}
