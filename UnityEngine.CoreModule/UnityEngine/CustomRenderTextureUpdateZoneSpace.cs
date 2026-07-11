using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Space in which coordinates are provided for Update Zones.</para>
	/// </summary>
	public enum CustomRenderTextureUpdateZoneSpace
	{
		/// <summary>
		///   <para>Coordinates are normalized. (0, 0) is top left and (1, 1) is bottom right.</para>
		/// </summary>
		Normalized,
		/// <summary>
		///   <para>Coordinates are expressed in pixels. (0, 0) is top left (width, height) is bottom right.</para>
		/// </summary>
		Pixel
	}
}
