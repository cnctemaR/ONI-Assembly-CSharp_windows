using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>The mode in which particles are emitted.</para>
	/// </summary>
	[Obsolete("ParticleSystemEmissionType no longer does anything. Time and Distance based emission are now both always active.", false)]
	public enum ParticleSystemEmissionType
	{
		/// <summary>
		///   <para>Emit over time.</para>
		/// </summary>
		Time,
		/// <summary>
		///   <para>Emit when emitter moves.</para>
		/// </summary>
		Distance
	}
}
