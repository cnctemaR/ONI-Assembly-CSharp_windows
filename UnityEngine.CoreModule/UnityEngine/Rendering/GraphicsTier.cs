using System;

namespace UnityEngine.Rendering
{
	/// <summary>
	///   <para>Graphics Tier.
	/// See Also: Graphics.activeTier.</para>
	/// </summary>
	public enum GraphicsTier
	{
		/// <summary>
		///   <para>The first graphics tier (Low) - corresponds to shader define UNITY_HARDWARE_TIER1.</para>
		/// </summary>
		Tier1,
		/// <summary>
		///   <para>The second graphics tier (Medium) - corresponds to shader define UNITY_HARDWARE_TIER2.</para>
		/// </summary>
		Tier2,
		/// <summary>
		///   <para>The third graphics tier (High) - corresponds to shader define UNITY_HARDWARE_TIER3.</para>
		/// </summary>
		Tier3
	}
}
