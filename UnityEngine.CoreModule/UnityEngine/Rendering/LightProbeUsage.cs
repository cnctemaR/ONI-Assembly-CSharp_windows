using System;

namespace UnityEngine.Rendering
{
	/// <summary>
	///   <para>Light probe interpolation type.</para>
	/// </summary>
	public enum LightProbeUsage
	{
		/// <summary>
		///   <para>Light Probes are not used. The scene's ambient probe is provided to the shader.</para>
		/// </summary>
		Off,
		/// <summary>
		///   <para>Simple light probe interpolation is used.</para>
		/// </summary>
		BlendProbes,
		/// <summary>
		///   <para>Uses a 3D grid of interpolated light probes.</para>
		/// </summary>
		UseProxyVolume,
		/// <summary>
		///   <para>The light probe shader uniform values are extracted from the material property block set on the renderer.</para>
		/// </summary>
		CustomProvided = 4
	}
}
