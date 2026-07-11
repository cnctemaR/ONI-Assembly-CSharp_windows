using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>The sorting mode for particle systems.</para>
	/// </summary>
	public enum ParticleSystemSortMode
	{
		/// <summary>
		///   <para>No sorting.</para>
		/// </summary>
		None,
		/// <summary>
		///   <para>Sort based on distance.</para>
		/// </summary>
		Distance,
		/// <summary>
		///   <para>Sort the oldest particles to the front.</para>
		/// </summary>
		OldestInFront,
		/// <summary>
		///   <para>Sort the youngest particles to the front.</para>
		/// </summary>
		YoungestInFront
	}
}
