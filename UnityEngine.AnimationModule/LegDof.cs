using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Enumeration of all the muscles in a leg.</para>
	/// </summary>
	public enum LegDof
	{
		/// <summary>
		///   <para>The upper leg front-back muscle.</para>
		/// </summary>
		UpperLegFrontBack,
		/// <summary>
		///   <para>The upper leg in-out muscle.</para>
		/// </summary>
		UpperLegInOut,
		/// <summary>
		///   <para>The upper leg roll in-out muscle.</para>
		/// </summary>
		UpperLegRollInOut,
		/// <summary>
		///   <para>The leg close-open muscle.</para>
		/// </summary>
		LegCloseOpen,
		/// <summary>
		///   <para>The leg roll in-out muscle.</para>
		/// </summary>
		LegRollInOut,
		/// <summary>
		///   <para>The foot close-open muscle.</para>
		/// </summary>
		FootCloseOpen,
		/// <summary>
		///   <para>The foot in-out muscle.</para>
		/// </summary>
		FootInOut,
		/// <summary>
		///   <para>The toes up-down muscle.</para>
		/// </summary>
		ToesUpDown,
		/// <summary>
		///   <para>The last value of the LegDof enum.</para>
		/// </summary>
		LastLegDof
	}
}
