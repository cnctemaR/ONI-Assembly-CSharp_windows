using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Enum describing what part of a light contribution can be baked.</para>
	/// </summary>
	[Flags]
	public enum LightmapBakeType
	{
		/// <summary>
		///   <para>Realtime lights cast run time light and shadows. They can change position, orientation, color, brightness, and many other properties at run time. No lighting gets baked into lightmaps or light probes..</para>
		/// </summary>
		Realtime = 4,
		/// <summary>
		///   <para>Baked lights cannot move or change in any way during run time. All lighting for static objects gets baked into lightmaps. Lighting and shadows for dynamic objects gets baked into Light Probes.</para>
		/// </summary>
		Baked = 2,
		/// <summary>
		///   <para>Mixed lights allow a mix of realtime and baked lighting, based on the Mixed Lighting Mode used. These lights cannot move, but can change color and intensity at run time. Changes to color and intensity only affect direct lighting as indirect lighting gets baked. If using Subtractive mode, changes to color or intensity are not calculated at run time on static objects.</para>
		/// </summary>
		Mixed = 1
	}
}
