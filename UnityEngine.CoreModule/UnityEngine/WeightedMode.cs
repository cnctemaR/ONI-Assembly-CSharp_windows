using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Sets which weights to use when calculating curve segments.</para>
	/// </summary>
	public enum WeightedMode
	{
		/// <summary>
		///   <para>Exclude both inWeight or outWeight when calculating curve segments.</para>
		/// </summary>
		None,
		/// <summary>
		///   <para>Include inWeight when calculating the previous curve segment.</para>
		/// </summary>
		In,
		/// <summary>
		///   <para>Include outWeight when calculating the next curve segment.</para>
		/// </summary>
		Out,
		/// <summary>
		///   <para>Include inWeight and outWeight when calculating curve segments.</para>
		/// </summary>
		Both
	}
}
