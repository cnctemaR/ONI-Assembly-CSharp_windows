using System;

namespace UnityEngine.Experimental.GlobalIllumination
{
	/// <summary>
	///   <para>The lightmode. A light can be realtime, mixed, baked or unknown. Unknown lights will be ignored by the baking backends.</para>
	/// </summary>
	public enum LightMode : byte
	{
		/// <summary>
		///   <para>The light is realtime. No contribution will be baked in lightmaps or light probes.</para>
		/// </summary>
		Realtime,
		/// <summary>
		///   <para>The light is mixed. Mixed lights are interpreted based on the global light mode setting in the lighting window.</para>
		/// </summary>
		Mixed,
		/// <summary>
		///   <para>The light is fully baked and has no realtime component.</para>
		/// </summary>
		Baked,
		/// <summary>
		///   <para>The light should be ignored by the baking backends.</para>
		/// </summary>
		Unknown
	}
}
