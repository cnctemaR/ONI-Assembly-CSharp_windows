using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Enumeration of all the muscles in a finger.</para>
	/// </summary>
	public enum FingerDof
	{
		/// <summary>
		///   <para>The proximal down-up muscle.</para>
		/// </summary>
		ProximalDownUp,
		/// <summary>
		///   <para>The proximal in-out muscle.</para>
		/// </summary>
		ProximalInOut,
		/// <summary>
		///   <para>The intermediate close-open muscle.</para>
		/// </summary>
		IntermediateCloseOpen,
		/// <summary>
		///   <para>The distal close-open muscle.</para>
		/// </summary>
		DistalCloseOpen,
		/// <summary>
		///   <para>The last value of the FingerDof enum.</para>
		/// </summary>
		LastFingerDof
	}
}
