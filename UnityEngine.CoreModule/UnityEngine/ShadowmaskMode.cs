using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>The rendering mode of Shadowmask.</para>
	/// </summary>
	public enum ShadowmaskMode
	{
		/// <summary>
		///   <para>Static shadow casters won't be rendered into realtime shadow maps. All shadows from static casters are handled via Shadowmasks and occlusion from Light Probes.</para>
		/// </summary>
		Shadowmask,
		/// <summary>
		///   <para>Static shadow casters will be rendered into realtime shadow maps. Shadowmasks and occlusion from Light Probes will only be used past the realtime shadow distance.</para>
		/// </summary>
		DistanceShadowmask
	}
}
