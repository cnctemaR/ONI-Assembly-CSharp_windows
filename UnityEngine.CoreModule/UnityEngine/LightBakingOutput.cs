using System;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>Struct describing the result of a Global Illumination bake for a given light.</para>
	/// </summary>
	[NativeHeader("Runtime/Camera/SharedLightData.h")]
	public struct LightBakingOutput
	{
		/// <summary>
		///   <para>In case of a LightmapBakeType.Mixed light, contains the index of the light as seen from the occlusion probes point of view if any, otherwise -1.</para>
		/// </summary>
		public int probeOcclusionLightIndex;

		/// <summary>
		///   <para>In case of a LightmapBakeType.Mixed light, contains the index of the occlusion mask channel to use if any, otherwise -1.</para>
		/// </summary>
		public int occlusionMaskChannel;

		/// <summary>
		///   <para>This property describes what part of a light's contribution was baked.</para>
		/// </summary>
		[NativeName("lightmapBakeMode.lightmapBakeType")]
		public LightmapBakeType lightmapBakeType;

		/// <summary>
		///   <para>In case of a LightmapBakeType.Mixed light, describes what Mixed mode was used to bake the light, irrelevant otherwise.</para>
		/// </summary>
		[NativeName("lightmapBakeMode.mixedLightingMode")]
		public MixedLightingMode mixedLightingMode;

		/// <summary>
		///   <para>Is the light contribution already stored in lightmaps and/or lightprobes?</para>
		/// </summary>
		public bool isBaked;
	}
}
