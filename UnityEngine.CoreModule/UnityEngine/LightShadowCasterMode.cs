using System;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>Allows mixed lights to control shadow caster culling when Shadowmasks are present.</para>
	/// </summary>
	[NativeHeader("Runtime/Camera/SharedLightData.h")]
	public enum LightShadowCasterMode
	{
		/// <summary>
		///   <para>Use the global Shadowmask Mode from the quality settings.</para>
		/// </summary>
		Default,
		/// <summary>
		///   <para>Render only non-lightmapped objects into the shadow map. This corresponds with the Shadowmask mode.</para>
		/// </summary>
		NonLightmappedOnly,
		/// <summary>
		///   <para>Render all shadow casters into the shadow map. This corresponds with the distance Shadowmask mode.</para>
		/// </summary>
		Everything
	}
}
