using System;

namespace UnityEngine.Experimental.Rendering
{
	/// <summary>
	///   <para>Visible reflection probes sorting options.</para>
	/// </summary>
	public enum ReflectionProbeSortOptions
	{
		/// <summary>
		///   <para>Do not sort reflection probes.</para>
		/// </summary>
		None,
		/// <summary>
		///   <para>Sort probes by importance.</para>
		/// </summary>
		Importance,
		/// <summary>
		///   <para>Sort probes from largest to smallest.</para>
		/// </summary>
		Size,
		/// <summary>
		///   <para>Sort probes by importance, then by size.</para>
		/// </summary>
		ImportanceThenSize
	}
}
