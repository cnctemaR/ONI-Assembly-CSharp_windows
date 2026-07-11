using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.AI
{
	/// <summary>
	///   <para>Level of obstacle avoidance.</para>
	/// </summary>
	[MovedFrom("UnityEngine")]
	public enum ObstacleAvoidanceType
	{
		/// <summary>
		///   <para>Disable avoidance.</para>
		/// </summary>
		NoObstacleAvoidance,
		/// <summary>
		///   <para>Enable simple avoidance. Low performance impact.</para>
		/// </summary>
		LowQualityObstacleAvoidance,
		/// <summary>
		///   <para>Medium avoidance. Medium performance impact.</para>
		/// </summary>
		MedQualityObstacleAvoidance,
		/// <summary>
		///   <para>Good avoidance. High performance impact.</para>
		/// </summary>
		GoodQualityObstacleAvoidance,
		/// <summary>
		///   <para>Enable highest precision. Highest performance impact.</para>
		/// </summary>
		HighQualityObstacleAvoidance
	}
}
