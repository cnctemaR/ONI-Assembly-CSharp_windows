using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Enum describing what lighting mode to be used with Mixed lights.</para>
	/// </summary>
	public enum MixedLightingMode
	{
		/// <summary>
		///   <para>Mixed lights provide realtime direct lighting while indirect light is baked into lightmaps and light probes.</para>
		/// </summary>
		IndirectOnly,
		/// <summary>
		///   <para>Mixed lights provide realtime direct lighting. Indirect lighting gets baked into lightmaps and light probes. Shadowmasks and light probe occlusion get generated for baked shadows. The Shadowmask Mode used at run time can be set in the Quality Settings panel.</para>
		/// </summary>
		Shadowmask = 2,
		/// <summary>
		///   <para>Mixed lights provide baked direct and indirect lighting for static objects. Dynamic objects receive realtime direct lighting and cast shadows on static objects using the main directional light in the scene.</para>
		/// </summary>
		Subtractive = 1
	}
}
