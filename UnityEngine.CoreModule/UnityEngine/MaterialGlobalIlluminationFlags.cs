using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>How the material interacts with lightmaps and lightprobes.</para>
	/// </summary>
	[Flags]
	public enum MaterialGlobalIlluminationFlags
	{
		/// <summary>
		///   <para>The emissive lighting does not affect Global Illumination at all.</para>
		/// </summary>
		None = 0,
		/// <summary>
		///   <para>The emissive lighting will affect realtime Global Illumination. It emits lighting into realtime lightmaps and realtime lightprobes.</para>
		/// </summary>
		RealtimeEmissive = 1,
		/// <summary>
		///   <para>The emissive lighting affects baked Global Illumination. It emits lighting into baked lightmaps and baked lightprobes.</para>
		/// </summary>
		BakedEmissive = 2,
		/// <summary>
		///   <para>The emissive lighting is guaranteed to be black. This lets the lightmapping system know that it doesn't have to extract emissive lighting information from the material and can simply assume it is completely black.</para>
		/// </summary>
		EmissiveIsBlack = 4,
		/// <summary>
		///   <para>Helper Mask to be used to query the enum only based on whether realtime GI or baked GI is set, ignoring all other bits.</para>
		/// </summary>
		AnyEmissive = 3
	}
}
