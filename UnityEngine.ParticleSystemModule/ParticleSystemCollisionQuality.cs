using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Quality of world collisions. Medium and low quality are approximate and may leak particles.</para>
	/// </summary>
	public enum ParticleSystemCollisionQuality
	{
		/// <summary>
		///   <para>The most accurate world collisions.</para>
		/// </summary>
		High,
		/// <summary>
		///   <para>Approximate world collisions.</para>
		/// </summary>
		Medium,
		/// <summary>
		///   <para>Fastest and most approximate world collisions.</para>
		/// </summary>
		Low
	}
}
